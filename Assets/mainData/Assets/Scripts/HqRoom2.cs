using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public class HqRoom2 : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum AccessState
	{
		Unpurchased,
		Locked,
		Unlocked,
		Unauthorized
	}

	protected struct AISaveData
	{
		public string characterName;

		public Vector3 position;

		public Quaternion rotation;
	}

	protected struct ItemSaveData
	{
		public Vector3 position;

		public Quaternion rotation;
	}

	internal class ViewMode : IShsState
	{
		internal HqRoom2 parent;

		internal ViewMode(HqRoom2 parent)
		{
			this.parent = parent;
		}

		public void Enter(Type previousState)
		{
			List<Animation> list = new List<Animation>();
			if (parent.animationData != null && previousState != typeof(PlacementMode))
			{
				foreach (Animation animation in parent.animations)
				{
					if (animation == null)
					{
						list.Add(animation);
					}
					else
					{
						if (!parent.animationData.ContainsKey(animation))
						{
							parent.animationData[animation] = new Dictionary<AnimationState, float>();
						}
						foreach (AnimationState item in animation)
						{
							parent.animationData[animation][item] = item.speed;
							item.speed = 0f;
						}
					}
				}
			}
			foreach (Animation item2 in list)
			{
				if (parent.animations.Contains(item2))
				{
					parent.animations.Remove(item2);
				}
			}
			List<HqObject2> list2 = new List<HqObject2>();
			foreach (HqItem value in parent.placedItems.Values)
			{
				if (!(value.gameObject == null))
				{
					HqObject2 component = Utils.GetComponent<HqObject2>(value.gameObject);
					if (component != null)
					{
						if (value.IsDestroyed)
						{
							list2.Add(component);
						}
						else
						{
							component.GotoPlacementMode(true);
						}
					}
					value.Paused = true;
				}
			}
			foreach (HqObject2 item3 in list2)
			{
				item3.GotoDestroyedMode();
			}
			foreach (HqFixedItem fixedItem in parent.fixedItems)
			{
				fixedItem.Paused = true;
			}
			foreach (ParticleEmitter emitter in parent.emitters)
			{
				if (emitter != null)
				{
					emitter.emit = false;
					emitter.ClearParticles();
				}
			}
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	internal class PlacementMode : IShsState
	{
		internal HqRoom2 parent;

		internal PlacementMode(HqRoom2 parent)
		{
			this.parent = parent;
		}

		public void Enter(Type previousState)
		{
			foreach (KeyValuePair<int, HqItem> placedItem in parent.placedItems)
			{
				HqItem value = placedItem.Value;
				HqObject2 hqObject = null;
				try
				{
					hqObject = Utils.GetComponent<HqObject2>(value.gameObject);
				}
				catch
				{
					CspUtils.DebugLog("hq object: " + placedItem.Key + " is no longer instantiated.");
				}
				if (!(hqObject == null) && !value.IsDestroyed)
				{
					hqObject.GotoPlacementMode(false);
				}
			}
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	internal class FlingaMode : IShsState
	{
		internal HqRoom2 parent;

		internal FlingaMode(HqRoom2 parent)
		{
			this.parent = parent;
		}

		public void Enter(Type previousState)
		{
			foreach (HqItem value in parent.placedItems.Values)
			{
				HqObject2 component = Utils.GetComponent<HqObject2>(value.gameObject);
				if (!(component == null) && !value.IsDestroyed)
				{
					component.GotoFlingaMode();
				}
			}
			if (parent.animationData != null)
			{
				foreach (Animation animation in parent.animations)
				{
					if (parent.animationData.ContainsKey(animation))
					{
						Dictionary<AnimationState, float> dictionary = parent.animationData[animation];
						foreach (AnimationState item in animation)
						{
							if (dictionary.ContainsKey(item))
							{
								item.speed = dictionary[item];
							}
						}
					}
				}
			}
			foreach (HqItem placedItem in parent.PlacedItems)
			{
				if (!(placedItem == null))
				{
					placedItem.Paused = false;
				}
			}
			foreach (HqFixedItem fixedItem in parent.fixedItems)
			{
				fixedItem.Paused = false;
			}
			foreach (ParticleEmitter emitter in parent.emitters)
			{
				if (emitter != null)
				{
					emitter.emit = true;
				}
			}
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	[Serializable]
	public class ZoomData
	{
		public float distance;

		public Vector2 min;

		public Vector2 max;

		public bool terrainFollow;
	}

	public class CameraController
	{
		public CameraLite cameraRoom;

		protected HqRoom2 owner;

		protected Vector3 cameraStartPos = Vector3.zero;

		protected Vector3 cameraGroundCenter = Vector3.zero;

		protected int cameraZoomLevel = -1;

		protected Vector3 camBlendStart;

		protected Vector3 camBlendDesired;

		protected float camBlendTime = -1f;

		protected float camBlendDuration = 1f;

		protected float terrainOffset;

		protected float currentOffset;

		protected float cameraSpeed;

		public void Initialize(HqRoom2 owner)
		{
			this.owner = owner;
			cameraRoom = owner.roomCamera;
			if (cameraRoom == null)
			{
				cameraRoom = Utils.GetComponent<CameraLite>(owner.gameObject, Utils.SearchChildren);
			}
			if (cameraRoom == null)
			{
				CspUtils.DebugLogError(owner.name + " does not have a room camera");
				return;
			}
			if (owner.centerMarker != null)
			{
				Plane plane = new Plane(Vector3.up, cameraRoom.transform.position);
				Ray ray = new Ray(owner.centerMarker.transform.position, -cameraRoom.transform.forward);
				float enter;
				plane.Raycast(ray, out enter);
				cameraStartPos = ray.GetPoint(enter);
			}
			else
			{
				cameraStartPos = cameraRoom.transform.position;
			}
			cameraGroundCenter = GetGroundPoint(cameraStartPos, cameraRoom.transform.forward);
		}

		public void Reset()
		{
			cameraRoom.transform.position = cameraStartPos;
			camBlendDesired = cameraStartPos;
			camBlendTime = -1f;
			cameraZoomLevel = -1;
			SetZoomLevel(1, -1f);
		}

		public void ScrollVector(Vector2 dir)
		{
			if (cameraZoomLevel < 0)
			{
				return;
			}
			Vector3 forward = Camera.main.transform.forward;
			float x = forward.x;
			Vector3 forward2 = cameraRoom.transform.forward;
			Vector3 a = new Vector3(x, 0f, forward2.z);
			a.Normalize();
			Vector3 vector = camBlendDesired;
			camBlendDesired += a * dir.y;
			if (!IsCameraInsideAllBounds(camBlendDesired))
			{
				camBlendDesired = vector;
			}
			vector = camBlendDesired;
			Vector3 a2 = new Vector3(a.z, 0f, 0f - a.x);
			camBlendDesired += a2 * dir.x;
			if (!IsCameraInsideAllBounds(camBlendDesired))
			{
				camBlendDesired = vector;
			}
			if (owner.zoomLimits[cameraZoomLevel].terrainFollow)
			{
				Vector3 groundPoint = GetGroundPoint(camBlendDesired, cameraRoom.transform.forward);
				groundPoint += cameraRoom.transform.forward * (-1.5f * owner.ceilingHeight);
				int layerMask = 163840;
				RaycastHit hitInfo;
				if (!Physics.Raycast(groundPoint, cameraRoom.transform.forward, out hitInfo, 500f, layerMask))
				{
					return;
				}
				vector = camBlendDesired;
				Vector3 a3 = hitInfo.point + -cameraRoom.transform.forward * owner.zoomLimits[cameraZoomLevel].distance;
				float magnitude = (a3 - camBlendDesired).magnitude;
				if (magnitude > 0.01f)
				{
					terrainOffset = magnitude;
					if (a3.y < camBlendDesired.y)
					{
						terrainOffset *= -1f;
					}
				}
			}
			else
			{
				terrainOffset = 0f;
				currentOffset = 0f;
			}
		}

		public void ZoomUp()
		{
			SetZoomLevel(cameraZoomLevel + 1, 1f);
		}

		public void ZoomDown()
		{
			SetZoomLevel(cameraZoomLevel - 1, 1f);
		}

		public void SetZoomLevel(int zoom, float blend)
		{
			zoom = MathfEx.Clamp(zoom, 0, owner.zoomLimits.Length - 1);
			if (zoom != cameraZoomLevel)
			{
				cameraZoomLevel = zoom;
				Vector3 groundPoint = GetGroundPoint(cameraRoom.transform);
				Vector3 vector = cameraRoom.transform.forward.normalized * (0f - owner.zoomLimits[cameraZoomLevel].distance);
				Vector3 vector2 = vector;
				vector2.x += groundPoint.x;
				vector2.z += groundPoint.z;
				camBlendDesired = vector2;
				if (!IsCameraInsideAllBounds(camBlendDesired))
				{
					vector.x += cameraGroundCenter.x;
					vector.z += cameraGroundCenter.z;
					camBlendDesired = vector;
				}
				ScrollVector(Vector2.zero);
				if (blend > 0f)
				{
					camBlendTime = blend;
					camBlendStart = cameraRoom.transform.position;
				}
				AppShell.Instance.EventMgr.Fire(this, new HQRoomZoomChangedMessage(cameraZoomLevel));
			}
		}

		public void Update()
		{
			if (HqController2.Instance != null && cameraZoomLevel > -1)
			{
				currentOffset = Mathf.SmoothDamp(currentOffset, terrainOffset, ref cameraSpeed, owner.terrainFollowSmoothing);
				camBlendDesired = GetGroundPoint(camBlendDesired, cameraRoom.transform.forward) + cameraRoom.transform.forward * (0f - (owner.zoomLimits[cameraZoomLevel].distance + currentOffset));
				if (camBlendTime > 0f)
				{
					camBlendTime -= Time.deltaTime / camBlendDuration;
				}
				if (camBlendTime > 0f)
				{
					cameraRoom.transform.position = MathfEx.Sinerp(camBlendStart, camBlendDesired, 1f - camBlendTime);
				}
				else
				{
					cameraRoom.transform.position = camBlendDesired;
				}
			}
			if (owner.DebugCameraPosition)
			{
				if (owner.DebugCameraPositionReset)
				{
					owner.DebugCameraPositionReset = false;
					owner.camera2dMin = new Vector2(float.MaxValue, float.MaxValue);
					owner.camera2dMax = new Vector2(float.MinValue, float.MinValue);
				}
				Ray ray = new Ray(cameraRoom.transform.position, cameraRoom.transform.forward);
				float enter;
				if (new Plane(Vector3.up, Vector3.zero).Raycast(ray, out enter))
				{
					Vector3 eulerAngles = cameraRoom.transform.rotation.eulerAngles;
					Quaternion rotation = Quaternion.Euler(0f, 0f - eulerAngles.y, 0f);
					Vector3 vector = rotation * ray.GetPoint(enter);
					owner.camera2D = new Vector2(vector.x, vector.z);
					owner.camera2dMin.x = Mathf.Min(owner.camera2dMin.x, owner.camera2D.x);
					owner.camera2dMin.y = Mathf.Min(owner.camera2dMin.y, owner.camera2D.y);
					owner.camera2dMax.x = Mathf.Max(owner.camera2dMax.x, owner.camera2D.x);
					owner.camera2dMax.y = Mathf.Max(owner.camera2dMax.y, owner.camera2D.y);
				}
			}
		}

		private Vector3 GetGroundPoint(Transform t)
		{
			return GetGroundPoint(t.position, t.forward);
		}

		private Vector3 GetGroundPoint(Vector3 pos, Vector3 dir)
		{
			Plane plane = new Plane(Vector3.up, Vector3.zero);
			Ray ray = new Ray(pos, dir);
			float enter;
			plane.Raycast(ray, out enter);
			return ray.GetPoint(enter);
		}

		private bool IsCameraInsideAllBounds(Vector3 pos)
		{
			for (int i = 0; i < 4; i++)
			{
				if (!IsCameraInsideBounds(i, pos))
				{
					return false;
				}
			}
			return true;
		}

		private bool IsCameraInsideBounds(int LeftRightDownUp, Vector3 pos)
		{
			if (owner.DebugIgnoreLimits)
			{
				return true;
			}
			Vector3 eulerAngles = cameraRoom.transform.rotation.eulerAngles;
			Quaternion rotation = Quaternion.Euler(0f, 0f - eulerAngles.y, 0f);
			Vector3 vector = (!owner.DebugUseNewZoomLimits) ? (rotation * pos) : (rotation * GetGroundPoint(pos, cameraRoom.transform.forward));
			switch (LeftRightDownUp)
			{
			case 0:
				if (vector.x > owner.zoomLimits[cameraZoomLevel].min.x)
				{
					return true;
				}
				break;
			case 1:
				if (vector.x < owner.zoomLimits[cameraZoomLevel].max.x)
				{
					return true;
				}
				break;
			case 2:
				if (vector.z > owner.zoomLimits[cameraZoomLevel].min.y)
				{
					return true;
				}
				break;
			case 3:
				if (vector.z < owner.zoomLimits[cameraZoomLevel].max.y)
				{
					return true;
				}
				break;
			}
			return false;
		}
	}

	protected const string AI_FIELD_TOKEN = "ai=";

	public CameraLite roomCamera;

	public CameraLite heroCamera;

	public CenterMarker centerMarker;

	public float ceilingHeight = 40f;

	public ZoomData[] zoomLimits = new ZoomData[4];

	public float terrainFollowSmoothing = 0.2f;

	public ShsFSM fsm;

	public bool DebugCameraPosition;

	public Vector2 camera2D;

	public Vector2 camera2dMin;

	public Vector2 camera2dMax;

	public bool DebugCameraPositionReset;

	public bool DebugUseNewZoomLimits;

	public bool DebugIgnoreLimits;

	protected string id = string.Empty;

	protected string typeid;

	protected int item_cap = 40;

	protected List<string> savedHeroData;

	protected List<string> savedItemData;

	protected Vector3 respawn;

	protected Bounds killZoneBounds;

	private CameraController cameraCtl;

	protected List<Color> themeColors;

	protected Hashtable roomKeyValues = new Hashtable();

	protected int saveTransId;

	protected Dictionary<int, HqItem> placedItems;

	protected int placedId;

	protected Dictionary<int, bool> placedIdUsed;

	protected List<PathNode> doorNodes;

	protected List<PathNode> pathNodes;

	protected List<AIControllerHQ> aiInRoom;

	protected List<HqItem> aiUsableItems;

	protected List<AIControllerHQ> aiSpawnedInRoom;

	protected List<HqAIProxy> proxiesInRoom = new List<HqAIProxy>();

	protected List<HqFixedItem> fixedItems;

	protected Dictionary<Animation, Dictionary<AnimationState, float>> animationData;

	protected List<Animation> animations;

	protected List<ParticleEmitter> emitters;

	protected PathFinder[] pathfinders;

	protected Dictionary<string, AISaveData> savedAI = new Dictionary<string, AISaveData>();

	protected Dictionary<int, ItemSaveData> savedItems = new Dictionary<int, ItemSaveData>();

	[CompilerGenerated]
	private bool _003CHasScrolled_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CHasZoomed_003Ek__BackingField;

	public string Id
	{
		get
		{
			return id;
		}
	}

	public string TypeId
	{
		get
		{
			return typeid;
		}
	}

	public IEnumerable<HqItem> PlacedItems
	{
		get
		{
			return placedItems.Values;
		}
	}

	public List<HqItem> AIUsableItems
	{
		get
		{
			return aiUsableItems;
		}
	}

	public CameraLite CameraRoom
	{
		get
		{
			return cameraCtl.cameraRoom;
		}
	}

	public CameraLite CameraHero
	{
		get
		{
			return heroCamera;
		}
	}

	public string ThemeName
	{
		get
		{
			return roomKeyValues["theme"] as string;
		}
	}

	public List<AIControllerHQ> AIInRoom
	{
		get
		{
			return aiInRoom;
		}
	}

	public virtual AccessState RoomState
	{
		get
		{
			if (!roomKeyValues.ContainsKey("roomstate"))
			{
				roomKeyValues["roomstate"] = AccessState.Unlocked.ToString();
			}
			return (AccessState)(int)Enum.Parse(typeof(AccessState), (string)roomKeyValues["roomstate"]);
		}
		set
		{
			roomKeyValues["roomstate"] = value.ToString();
		}
	}

	public Vector3 IntroMark
	{
		get
		{
			if (pathNodes != null)
			{
				foreach (PathNode pathNode in pathNodes)
				{
					if (pathNode.gameObject.name == "intro_mark")
					{
						return pathNode.transform.position;
					}
				}
			}
			return Vector3.zero;
		}
	}

	public PathFinder[] Pathfinders
	{
		get
		{
			if (pathfinders == null)
			{
				pathfinders = Utils.GetComponents<PathFinder>(base.gameObject, Utils.SearchChildren, true);
			}
			return pathfinders;
		}
	}

	public bool ItemCapReached
	{
		get
		{
			return placedItems.Values.Count >= item_cap;
		}
	}

	public List<string> SavedItemIds
	{
		get
		{
			if (savedItemData != null)
			{
				List<string> list = new List<string>(savedItemData.Count);
				{
					foreach (string savedItemDatum in savedItemData)
					{
						string[] array = savedItemDatum.Split('|');
						list.Add(array[0]);
					}
					return list;
				}
			}
			return null;
		}
	}

	public Bounds KillZoneBounds
	{
		get
		{
			return killZoneBounds;
		}
	}

	public bool HasScrolled
	{
		[CompilerGenerated]
		get
		{
			return _003CHasScrolled_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CHasScrolled_003Ek__BackingField = value;
		}
	}

	public bool HasZoomed
	{
		[CompilerGenerated]
		get
		{
			return _003CHasZoomed_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CHasZoomed_003Ek__BackingField = value;
		}
	}

	public Vector3 RandomDoor
	{
		get
		{
			Vector3 result = Vector3.zero;
			if (doorNodes != null && doorNodes.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, doorNodes.Count - 1);
				result = doorNodes[index].transform.position;
			}
			return result;
		}
	}

	public Vector3 RandomLocation
	{
		get
		{
			List<PathNode> list = new List<PathNode>(pathNodes);
			while (list.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, list.Count - 1);
				PathNode pathNode = list[index];
				bool flag = true;
				foreach (HqItem value in placedItems.Values)
				{
					HqObject2 component = Utils.GetComponent<HqObject2>(value.gameObject);
					if (component != null && component.Bounds.Contains(pathNode.transform.position))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return pathNode.transform.position;
				}
				list.RemoveAt(index);
			}
			return Vector3.zero;
		}
	}

	public void OnEnable()
	{
		HqController2.Instance.ResetShadows();
	}

	public void Start()
	{
		HasScrolled = false;
		HasZoomed = false;
	}

	public virtual void Initialize(RoomStaticData roomData)
	{
		id = roomData.id.ToLower();
		typeid = roomData.typeid;
		item_cap = roomData.item_cap;
		cameraCtl = new CameraController();
		cameraCtl.Initialize(this);
		placedItems = new Dictionary<int, HqItem>();
		placedIdUsed = new Dictionary<int, bool>();
		placedId = 0;
		aiUsableItems = new List<HqItem>();
		doorNodes = new List<PathNode>();
		pathNodes = new List<PathNode>();
		LoadGeometry();
		List<GameObject> locks = new List<GameObject>();
		float min = float.MaxValue;
		float max = float.MinValue;
		GameObject bottom = null;
		GameObject top = null;
		bool killZoneBoundsSet = false;
		Utils.ForEachTree(base.gameObject, delegate(GameObject go)
		{
			string text = go.name.ToLower();
			if (text.Contains("lock_collision"))
			{
				locks.Add(go);
				Vector3 center = go.collider.bounds.center;
				if (center.y < min)
				{
					Vector3 center2 = go.collider.bounds.center;
					min = center2.y;
					bottom = go;
				}
				Vector3 center3 = go.collider.bounds.center;
				if (center3.y > max)
				{
					Vector3 center4 = go.collider.bounds.center;
					max = center4.y;
					top = go;
				}
				if (!killZoneBoundsSet)
				{
					killZoneBoundsSet = true;
					killZoneBounds = new Bounds(go.collider.bounds.center, go.collider.bounds.size);
				}
				else
				{
					killZoneBounds.Encapsulate(go.collider.bounds);
				}
			}
		});
		foreach (GameObject item in locks)
		{
			if (item != top)
			{
				Utils.AddComponent<HqKillZone>(item);
			}
		}
		if (bottom != null)
		{
			Vector3 position = bottom.transform.position;
			position.y -= 0.5f;
			bottom.transform.position = position;
		}
		if (top != null)
		{
			respawn = killZoneBounds.center;
			//ref Vector3 reference = ref respawn;
			Vector3 min2 = top.collider.bounds.min;
			respawn.y = min2.y - 2f;
		}
		aiInRoom = new List<AIControllerHQ>();
		aiSpawnedInRoom = new List<AIControllerHQ>();
		fsm = new ShsFSM();
		fsm.AddState(new ViewMode(this));
		fsm.AddState(new PlacementMode(this));
		fsm.AddState(new FlingaMode(this));
		fsm.GotoState<ViewMode>();
		Deactivate();
		AppShell.Instance.EventMgr.Fire(this, new HQRoomInitializedMessage(this));
	}

	public virtual void Activate()
	{
		Utils.ActivateTree(base.gameObject, true);
		foreach (KeyValuePair<int, HqItem> placedItem in placedItems)
		{
			HqItem value = placedItem.Value;
			if (value.IsDestroyed)
			{
				Utils.ActivateTree(value.gameObject, false);
			}
			HqObject2 hqObject = null;
			try
			{
				hqObject = Utils.GetComponent<HqObject2>(value.gameObject);
			}
			catch
			{
				CspUtils.DebugLog("hq object: " + placedItem.Key + " is no longer instantiated.");
			}
			if (!(hqObject == null))
			{
				hqObject.Activate();
			}
		}
		foreach (AIControllerHQ item in AIInRoom)
		{
			if (item.Mode == AIControllerHQ.AiMode.Sim)
			{
				item.Activate();
				item.Awaken();
			}
		}
		if (HqController2.Instance != null && cameraCtl != null)
		{
			cameraCtl.Reset();
		}
	}

	public virtual void Deactivate()
	{
		foreach (AIControllerHQ item in AIInRoom)
		{
			if (item.Mode == AIControllerHQ.AiMode.Active)
			{
				item.DeActivate();
			}
		}
		Utils.ActivateTree(base.gameObject, false);
	}

	public virtual void Update()
	{
		if (cameraCtl != null)
		{
			cameraCtl.Update();
		}
	}

	public virtual void ScrollLeft(float speed)
	{
		cameraCtl.ScrollVector(Vector2.right * ((0f - speed) * Time.deltaTime));
		HasScrolled = true;
	}

	public virtual void ScrollRight(float speed)
	{
		cameraCtl.ScrollVector(Vector2.right * (speed * Time.deltaTime));
		HasScrolled = true;
	}

	public virtual void ScrollDown(float speed)
	{
		cameraCtl.ScrollVector(Vector2.up * ((0f - speed) * Time.deltaTime));
		HasScrolled = true;
	}

	public virtual void ScrollUp(float speed)
	{
		cameraCtl.ScrollVector(Vector2.up * (speed * Time.deltaTime));
		HasScrolled = true;
	}

	public virtual void ScrollVector(Vector2 dir)
	{
		cameraCtl.ScrollVector(dir);
		HasScrolled = true;
	}

	public virtual void ZoomUp()
	{
		cameraCtl.ZoomUp();
		HasZoomed = true;
	}

	public virtual void ZoomDown()
	{
		cameraCtl.ZoomDown();
		HasZoomed = true;
	}

	public virtual void GotoViewMode()
	{
		fsm.GotoState<ViewMode>();
	}

	public virtual void GotoPlacementMode()
	{
		fsm.GotoState<PlacementMode>();
	}

	public virtual void GotoFlingaMode()
	{
		fsm.GotoState<FlingaMode>();
	}

	private void ApplyThemeColors(List<Color> replacements)
	{
		Transform transform = Utils.FindNodeInChildren(base.transform, "Geometry");
		if (transform == null)
		{
			return;
		}
		GameObject gameObject = transform.gameObject;
		if (gameObject == null)
		{
			CspUtils.DebugLog("Failed to find Geometry node");
			return;
		}
		MeshFilter[] components = Utils.GetComponents<MeshFilter>(gameObject, Utils.SearchChildren, true);
		MeshFilter[] array = components;
		foreach (MeshFilter meshFilter in array)
		{
			Color[] colors = meshFilter.mesh.colors;
			if (colors == null || colors.Length <= 0)
			{
				continue;
			}
			for (int j = 0; j < colors.Length; j++)
			{
				for (int k = 0; k < themeColors.Count; k++)
				{
					if (CompareRGB(colors[j], themeColors[k], 0.0001f))
					{
						colors[j] = replacements[k];
						break;
					}
				}
			}
			meshFilter.mesh.colors = colors;
		}
	}

	public virtual void ApplyTheme(ThemeStaticData theme, bool markDirty)
	{
		ApplyThemeColors(theme.colors);
		roomKeyValues["theme"] = theme.name.Trim();
		if (themeColors != theme.colors)
		{
			themeColors = theme.colors;
		}
	}

	public virtual void ApplyTheme(string themeName, bool makeDirty)
	{
		if (themeName != null)
		{
			themeName = themeName.Trim();
			List<Color> replacements = null;
			if (!HqController2.Instance.GetRoomThemeFromName(themeName, out replacements))
			{
				CspUtils.DebugLog("Could not get theme colors: " + themeName);
				replacements = HqController2.Instance.ThemeBaseColors;
			}
			ApplyThemeColors(replacements);
			roomKeyValues["theme"] = themeName;
			if (themeColors != replacements)
			{
				themeColors = replacements;
			}
		}
	}

	public virtual HqItem AddItem(GameObject go)
	{
		HqObject2 component = Utils.GetComponent<HqObject2>(go);
		if (component == null)
		{
			return null;
		}
		go.transform.parent = base.transform;
		component.PlacedId = GetNewPlacedId();
		Item value = null;
		UserProfile profile = HqController2.Instance.Profile;
		if (profile != null && component.InventoryId != null)
		{
			profile.AvailableItems.TryGetValue(component.InventoryId, out value);
		}
		PlacedItem placedItem = go.AddComponent<PlacedItem>();
		if (placedItem != null)
		{
			placedItem.Initialize(component.PlacedId, value, go.transform.position, go.transform.rotation, this);
		}
		placedItems.Add(component.PlacedId, placedItem);
		if (placedItem.IsUsableItem)
		{
			aiUsableItems.Add(placedItem);
		}
		return placedItem;
	}

	public virtual void DelItem(GameObject go)
	{
		HqObject2 component = Utils.GetComponent<HqObject2>(go);
		if (component == null || component.PlacedId < 0)
		{
			return;
		}
		if (placedItems.ContainsKey(component.PlacedId))
		{
			HqItem item = placedItems[component.PlacedId];
			if (aiUsableItems.Contains(item))
			{
				aiUsableItems.Remove(item);
			}
			placedItems.Remove(component.PlacedId);
		}
		else
		{
			CspUtils.DebugLog("item was not found");
		}
		if (savedItems.ContainsKey(component.PlacedId))
		{
			savedItems.Remove(component.PlacedId);
			ImplicitSave();
		}
	}

	public virtual void MoveItem(GameObject go)
	{
		HqObject2 component = Utils.GetComponent<HqObject2>(go);
		if (!(component == null) && component.PlacedId >= 0)
		{
			HqItem hqItem = placedItems[component.PlacedId];
			hqItem.Position = go.transform.position;
			hqItem.Rotation = go.transform.rotation;
		}
	}

	public HqItem GetItem(int placedId)
	{
		if (placedItems.ContainsKey(placedId))
		{
			return placedItems[placedId];
		}
		return null;
	}

	public virtual void RevertToLastSave()
	{
		List<HqItem> list = new List<HqItem>();
		foreach (HqItem value in placedItems.Values)
		{
			HqObject2 component = Utils.GetComponent<HqObject2>(value.gameObject);
			if (component != null && savedItems.ContainsKey(component.PlacedId))
			{
				ItemSaveData itemSaveData = savedItems[component.PlacedId];
				value.gameObject.transform.position = itemSaveData.position;
				value.gameObject.transform.rotation = itemSaveData.rotation;
				value.RevertToLastSave();
			}
			else
			{
				list.Add(value);
			}
		}
		foreach (HqItem item in list)
		{
			DelItem(item.gameObject);
			AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(new string[1]
			{
				item.ItemDefinition.Id
			}, CollectionResetMessage.ActionType.Add, "Items"));
			UnityEngine.Object.Destroy(item.gameObject);
		}
		List<AIControllerHQ> list2 = new List<AIControllerHQ>();
		List<AISaveData> list3 = new List<AISaveData>(savedAI.Values);
		foreach (AIControllerHQ item2 in AIInRoom)
		{
			if (!savedAI.ContainsKey(item2.CharacterName))
			{
				list2.Add(item2);
			}
			else
			{
				Transform transform = item2.gameObject.transform;
				AISaveData aISaveData = savedAI[item2.CharacterName];
				transform.position = aISaveData.position;
				Transform transform2 = item2.gameObject.transform;
				AISaveData aISaveData2 = savedAI[item2.CharacterName];
				transform2.rotation = aISaveData2.rotation;
				item2.ChangeBehavior<BehaviorPaused>(true);
				list3.Remove(savedAI[item2.CharacterName]);
			}
		}
		if (list2.Count > 0)
		{
			HqRoom2 hqRoom = null;
			foreach (HqRoom2 value2 in HqController2.Instance.Rooms.Values)
			{
				if (value2 != this && value2.RoomState == AccessState.Unlocked)
				{
					hqRoom = value2;
					break;
				}
			}
			foreach (AIControllerHQ item3 in list2)
			{
				if (item3.LastRoom != null)
				{
					item3.GoToRoom(item3.LastRoom);
				}
				else if (item3.SpawnRoom != this)
				{
					item3.GoToRoom(item3.SpawnRoom);
				}
				else if (hqRoom != null)
				{
					item3.GoToRoom(hqRoom);
				}
			}
		}
		foreach (AISaveData item4 in list3)
		{
			AIControllerHQ aIController = HqController2.Instance.GetAIController(item4.characterName);
			if (aIController != null)
			{
				aIController.gameObject.transform.position = item4.position;
				aIController.gameObject.transform.rotation = item4.rotation;
				aIController.CurrentRoom = this;
				aIController.ChangeBehavior<BehaviorPaused>(true);
			}
			else
			{
				GameObject gameObject = HqAIProxy.CreateProxy(item4.characterName);
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.transform.localPosition = item4.position;
				gameObject.transform.localRotation = item4.rotation;
				HqAIProxy component2 = Utils.GetComponent<HqAIProxy>(gameObject);
				if (component2 == null)
				{
					CspUtils.DebugLog("Could not find HqAIProxy component!");
					break;
				}
				component2.Spawn(item4.characterName, item4.position, this);
				HqController2.Instance.AddProxy(component2);
				AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(new string[1]
				{
					item4.characterName
				}, CollectionResetMessage.ActionType.Remove, "Heroes"));
			}
		}
	}

	public void ImplicitSave()
	{
		string text = BuildSaveData();
		saveTransId++;
		int localsaveTransId = saveTransId;
		AppShell.Instance.SharedHashTable["HQ_" + id] = text.Split('\n');
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("room_data", text);
		string uri = "resources$users/" + HqController2.Instance.Profile.UserId + "/hq/room/" + id + "/save/";
		AppShell.Instance.WebService.StartRequest(uri, delegate(ShsWebResponse response)
		{
			if (localsaveTransId == saveTransId)
			{
				OnSaveResponse(response);
			}
		}, wWWForm.data, ShsWebService.ShsWebServiceType.RASP);
	}

	public virtual void Save()
	{
		if (HqController2.Instance.Profile != null)
		{
			SaveAIData();
			List<HqItem> list = new List<HqItem>();
			savedItems.Clear();
			foreach (HqItem value2 in placedItems.Values)
			{
				if (!value2.IsDestroyed)
				{
					PlacedItem placedItem = value2 as PlacedItem;
					ItemSaveData value = default(ItemSaveData);
					value.position = placedItem.transform.position;
					value.rotation = placedItem.transform.rotation;
					savedItems[placedItem.PlacedId] = value;
					value2.Save();
				}
				else
				{
					list.Add(value2);
				}
			}
			foreach (HqItem item in list)
			{
				DelItem(item.gameObject);
				AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(new string[1]
				{
					item.ItemDefinition.Id
				}, CollectionResetMessage.ActionType.Add, "Items"));
				UnityEngine.Object.Destroy(item.gameObject);
			}
			ImplicitSave();
		}
	}

	public void LoadFromFile(string[] lines)
	{
		Reset();
		LoadSaveData(lines);
		if (HqController2.Instance.ActiveRoom != this)
		{
			foreach (AIControllerHQ item in AIInRoom)
			{
				if (item.Mode == AIControllerHQ.AiMode.Active)
				{
					item.DeActivate();
				}
			}
			Utils.ActivateTree(base.gameObject, false);
		}
		if (ThemeName != null && HqController2.Instance.ThemeExists(ThemeName))
		{
			ApplyTheme(ThemeName, false);
		}
	}

	public virtual void LoadSaveData(string[] lines)
	{
		try
		{
			savedItems.Clear();
			int num = 1;
			string[] array = lines[num].Split('|');
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length != 0)
				{
					int num2 = array[i].IndexOf('=');
					if (num2 != -1)
					{
						string key = array[i].Substring(0, num2);
						string value = array[i].Substring(num2 + 1);
						roomKeyValues[key] = value;
					}
				}
			}
			num++;
			if (lines.Length > num && lines[num].StartsWith("ai="))
			{
				string text = lines[num].Remove(0, "ai=".Length);
				savedHeroData = new List<string>(text.Split(','));
				num++;
			}
			for (int j = num; j < lines.Length; j++)
			{
				if (lines[j].Length != 0)
				{
					if (savedItemData == null)
					{
						savedItemData = new List<string>();
					}
					savedItemData.Add(lines[j]);
				}
			}
		}
		catch (Exception ex)
		{
			CspUtils.DebugLog("Exception parsing room data: " + ex.StackTrace);
		}
	}

	public string BuildSaveData()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(id);
		int num = 1;
		foreach (DictionaryEntry roomKeyValue in roomKeyValues)
		{
			stringBuilder.Append(roomKeyValue.Key.ToString());
			stringBuilder.Append("=");
			stringBuilder.Append(roomKeyValue.Value.ToString().Trim());
			if (num < roomKeyValues.Count)
			{
				stringBuilder.Append("|");
			}
			num++;
		}
		stringBuilder.AppendLine();
		if (savedAI != null && savedAI.Keys.Count > 0)
		{
			stringBuilder.Append("ai=");
			int num2 = 0;
			foreach (AISaveData value in savedAI.Values)
			{
				stringBuilder.Append(value.characterName);
				if (num2 < savedAI.Values.Count - 1 && savedAI.Values.Count > 0)
				{
					stringBuilder.Append(",");
				}
				num2++;
			}
			stringBuilder.AppendLine();
		}
		foreach (int key in savedItems.Keys)
		{
			if (placedItems.ContainsKey(key))
			{
				PlacedItem placedItem = placedItems[key] as PlacedItem;
				if (placedItem != null)
				{
					placedItem.AppendToSave(stringBuilder);
				}
			}
		}
		return stringBuilder.ToString();
	}

	public virtual void SpawnAI(AIControllerHQ ai)
	{
		aiSpawnedInRoom.Add(ai);
		ai.SpawnRoom = this;
	}

	public void DespawnAI(AIControllerHQ ai)
	{
		if (aiSpawnedInRoom.Contains(ai))
		{
			aiSpawnedInRoom.Remove(ai);
		}
	}

	public void AddProxy(HqAIProxy proxy)
	{
		proxiesInRoom.Add(proxy);
	}

	public void RemoveProxy(HqAIProxy proxy)
	{
		if (proxiesInRoom.Contains(proxy))
		{
			proxiesInRoom.Remove(proxy);
		}
	}

	public bool IsSpawning(string characterName)
	{
		foreach (HqAIProxy item in proxiesInRoom)
		{
			if (item.characterName == characterName)
			{
				return true;
			}
		}
		return false;
	}

	public void RespawnObject(HqObject2 hqobj)
	{
		if (fsm.GetCurrentState() != typeof(FlingaMode))
		{
			CspUtils.DebugLog("Unexpected call to RespawnObject: " + hqobj.gameObject.name + " current state is " + fsm.GetCurrentState().ToString());
			return;
		}
		if (hqobj.rigidbody != null)
		{
			hqobj.rigidbody.velocity = Vector3.zero;
			hqobj.rigidbody.isKinematic = true;
			hqobj.rigidbody.useGravity = false;
		}
		hqobj.gameObject.transform.position = respawn;
		if (hqobj.rigidbody != null)
		{
			hqobj.rigidbody.isKinematic = false;
			hqobj.rigidbody.useGravity = true;
		}
		AppShell.Instance.EventMgr.Fire(this, new HQRoomRespawnMessage(hqobj.gameObject));
	}

	public void RespawnAI(AIControllerHQ ai)
	{
		ai.Paused = true;
		Vector3 randomDoor = RandomDoor;
		if (randomDoor == Vector3.zero)
		{
			randomDoor = respawn;
		}
		ai.gameObject.transform.position = randomDoor;
		ai.Paused = false;
		ai.DropAI();
	}

	public Vector3 GetDoor(Vector3 point)
	{
		Vector3 result = Vector3.zero;
		PathFinder pathFinder = FindClosestPathFinder(point);
		if (pathFinder != null)
		{
			PathNode[] components = Utils.GetComponents<PathNode>(pathFinder, Utils.SearchChildren);
			PathNode pathNode = null;
			PathNode[] array = components;
			foreach (PathNode pathNode2 in array)
			{
				if (pathNode2.Type == PathNodeBase.NodeType.Door)
				{
					if (pathNode == null)
					{
						pathNode = pathNode2;
					}
					else if ((pathNode.transform.position - point).sqrMagnitude > (pathNode2.transform.position - point).sqrMagnitude)
					{
						pathNode = pathNode2;
					}
				}
			}
			if (pathNode != null)
			{
				result = pathNode.transform.position;
			}
		}
		return result;
	}

	public PathFinder FindClosestPathFinder(Vector3 point)
	{
		PathFinder result = null;
		if (Pathfinders != null && Pathfinders.Length > 0)
		{
			float num = 0f;
			bool flag = false;
			for (int i = 0; i < Pathfinders.Length; i++)
			{
				PathNode pathNode = Pathfinders[i].ClosestPathNode(point);
				if (!(pathNode != null))
				{
					continue;
				}
				if (!flag)
				{
					num = (pathNode.transform.position - point).sqrMagnitude;
					result = Pathfinders[i];
					flag = true;
					continue;
				}
				float sqrMagnitude = (pathNode.transform.position - point).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = Pathfinders[i];
				}
			}
		}
		return result;
	}

	public virtual void Reset()
	{
		List<string> list = new List<string>();
		foreach (HqItem value in placedItems.Values)
		{
			PlacedItem placedItem = value as PlacedItem;
			if (placedItem != null && placedItem.InventoryItem != null)
			{
				list.Add(placedItem.InventoryItem.Id);
			}
			UnityEngine.Object.Destroy(value.gameObject);
		}
		AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(list.ToArray(), CollectionResetMessage.ActionType.Add, "Items"));
		placedItems.Clear();
		aiInRoom.Clear();
		aiSpawnedInRoom.Clear();
		aiUsableItems.Clear();
		foreach (HqAIProxy item in proxiesInRoom)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		proxiesInRoom.Clear();
		Save();
		foreach (HqFixedItem fixedItem in fixedItems)
		{
			aiUsableItems.Add(fixedItem);
		}
	}

	public void ConsumeItem(GameObject obj)
	{
		HqObject2 component = Utils.GetComponent<HqObject2>(obj);
		if (component != null && placedItems.ContainsKey(component.PlacedId))
		{
			PlacedItem placedItem = placedItems[component.PlacedId] as PlacedItem;
			if (placedItem != null)
			{
				placedItem.Consume();
			}
			DelItem(obj);
		}
		UnityEngine.Object.Destroy(obj);
	}

	public void AddAI(AIControllerHQ ai)
	{
		if (!aiInRoom.Contains(ai))
		{
			aiInRoom.Add(ai);
		}
	}

	public void RemoveAI(AIControllerHQ ai)
	{
		if (aiInRoom.Contains(ai))
		{
			aiInRoom.Remove(ai);
		}
	}

	public void PlayTransporterEffect(Vector3 location)
	{
		if (HqController2.Instance.TransporterFX != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(HqController2.Instance.TransporterFX, location, Quaternion.identity) as GameObject;
			gameObject.transform.parent = base.gameObject.transform;
			EffectSequence component = Utils.GetComponent<EffectSequence>(gameObject, Utils.SearchChildren);
			if (component == null)
			{
				CspUtils.DebugLog("Could not find transporter effect sequence.");
				return;
			}
			component.Initialize(null, null, null);
			component.StartSequence();
		}
	}

	protected virtual void LoadGeometry()
	{
		CachePathInformation();
		fixedItems = new List<HqFixedItem>(Utils.GetComponents<HqFixedItem>(base.gameObject, Utils.SearchChildren, true));
		foreach (HqFixedItem fixedItem in fixedItems)
		{
			aiUsableItems.Add(fixedItem);
		}
		animations = new List<Animation>(Utils.GetComponents<Animation>(base.gameObject, Utils.SearchChildren, true));
		if (animations.Count > 0)
		{
			animationData = new Dictionary<Animation, Dictionary<AnimationState, float>>();
			foreach (Animation animation in animations)
			{
				animationData[animation] = new Dictionary<AnimationState, float>();
				foreach (AnimationState item in animation)
				{
					animationData[animation][item] = item.speed;
				}
			}
		}
		emitters = new List<ParticleEmitter>(Utils.GetComponents<ParticleEmitter>(base.gameObject, Utils.SearchChildren, true));
		themeColors = HqController2.Instance.ThemeBaseColors;
		if (ThemeName != null && HqController2.Instance.ThemeExists(ThemeName))
		{
			ApplyTheme(ThemeName, false);
		}
		else
		{
			ApplyTheme(HqController2.Instance.DefaultThemeName, false);
		}
	}

	protected void RemoveAIFromSaveData(AIControllerHQ ai)
	{
		if (AIInRoom.Contains(ai))
		{
			AIInRoom.Remove(ai);
		}
		if (savedAI.ContainsKey(ai.CharacterName))
		{
			savedAI.Remove(ai.CharacterName);
		}
	}

	protected void SaveAIData()
	{
		savedAI.Clear();
		Dictionary<HqRoom2, List<AIControllerHQ>> dictionary = new Dictionary<HqRoom2, List<AIControllerHQ>>();
		foreach (AIControllerHQ item in AIInRoom)
		{
			AISaveData value = default(AISaveData);
			value.characterName = item.CharacterName;
			value.position = item.gameObject.transform.position;
			value.rotation = item.gameObject.transform.rotation;
			savedAI[item.CharacterName] = value;
			item.SaveHungerValue();
			foreach (HqRoom2 value2 in HqController2.Instance.Rooms.Values)
			{
				if (value2 != this && value2.savedAI.ContainsKey(item.CharacterName))
				{
					if (!dictionary.ContainsKey(value2))
					{
						dictionary[value2] = new List<AIControllerHQ>();
					}
					dictionary[value2].Add(item);
				}
			}
		}
		foreach (KeyValuePair<HqRoom2, List<AIControllerHQ>> item2 in dictionary)
		{
			foreach (AIControllerHQ item3 in item2.Value)
			{
				item2.Key.RemoveAIFromSaveData(item3);
			}
			item2.Key.ImplicitSave();
		}
	}

	private void OnSaveResponse(ShsWebResponse response)
	{
		if (response.Status == 200 && response.Body.Contains("saved"))
		{
			CspUtils.DebugLog("Room successfully saved: " + response.Body);
			return;
		}
		CspUtils.DebugLog("Error saving room: " + response.Status);
		CspUtils.DebugLog("  " + response.Body);
	}

	protected bool CompareRGB(Color rh, Color lh, float tolerance)
	{
		return MathfEx.Approx(rh.r, lh.r, tolerance) && MathfEx.Approx(rh.g, lh.g, tolerance) && MathfEx.Approx(rh.b, lh.b, tolerance);
	}

	protected int GetNewPlacedId()
	{
		do
		{
			placedId++;
		}
		while (placedIdUsed.ContainsKey(placedId));
		placedIdUsed.Add(placedId, true);
		return placedId;
	}

	private void LoadSaveData()
	{
		string[] array = AppShell.Instance.SharedHashTable["HQ_" + id] as string[];
		if (array == null)
		{
			CspUtils.DebugLog("Could not find save data for room " + id);
		}
		else
		{
			LoadSaveData(array);
		}
	}

	public void LoadItems()
	{
		if (savedItemData != null)
		{
			foreach (string savedItemDatum in savedItemData)
			{
				PlacedItem placedItem = PlacedItem.LoadFromSave(savedItemDatum, this);
				if (placedItem != null)
				{
					placedIdUsed.Add(placedItem.PlacedId, true);
					placedItems.Add(placedItem.PlacedId, placedItem);
					if (placedItem.IsUsableItem)
					{
						aiUsableItems.Add(placedItem);
					}
					ItemSaveData value = default(ItemSaveData);
					value.position = placedItem.transform.position;
					value.rotation = placedItem.transform.rotation;
					savedItems[placedItem.PlacedId] = value;
					placedItem.Save();
				}
			}
		}
	}

	public void LoadAI()
	{
		if (savedHeroData != null)
		{
			List<PathNode> list = new List<PathNode>(pathNodes);
			foreach (string savedHeroDatum in savedHeroData)
			{
				if (!string.IsNullOrEmpty(savedHeroDatum.Trim()))
				{
					Vector3 vector = Vector3.zero;
					if (list.Count > 0)
					{
						int index = UnityEngine.Random.Range(0, list.Count);
						vector = list[index].transform.position;
						list.RemoveAt(index);
					}
					else
					{
						CspUtils.DebugLog("AvailableNodes count == 0!!!!");
					}
					if (PlaceAICharacter(savedHeroDatum.Trim(), vector))
					{
						AISaveData value = default(AISaveData);
						value.characterName = savedHeroDatum.Trim();
						value.position = vector;
						value.rotation = Quaternion.identity;
						savedAI[savedHeroDatum.Trim()] = value;
					}
				}
			}
		}
	}

	public bool PlaceAICharacter(string characterName, Vector3 spawnLocation)
	{
		UserProfile profile = HqController2.Instance.Profile;
		if (profile == null)
		{
			CspUtils.DebugLog("Could not get UserProfile");
			return false;
		}
		HeroPersisted value = null;
		if (!profile.AvailableCostumes.TryGetValue(characterName, out value))
		{
			CspUtils.DebugLog("Could not get HeroPersisted for " + characterName);
			return false;
		}
		if (!HqController2.Instance.IsAllowableCharacter(characterName))
		{
			CspUtils.DebugLog(characterName + " is not an allowable character");
			return false;
		}
		GameObject gameObject = HqAIProxy.CreateProxy(characterName);
		gameObject.transform.parent = base.transform;
		gameObject.transform.position = spawnLocation;
		HqAIProxy component = Utils.GetComponent<HqAIProxy>(gameObject);
		if (component == null)
		{
			CspUtils.DebugLog("Could not find HqAIProxy component!");
			return false;
		}
		component.Spawn(characterName, spawnLocation, this);
		HqController2.Instance.AddProxy(component);
		value.Placed = true;
		return true;
	}

	private void CachePathInformation()
	{
		if (Pathfinders == null)
		{
			return;
		}
		for (int i = 0; i < Pathfinders.Length; i++)
		{
			if (!(Pathfinders[i] != null))
			{
				continue;
			}
			PathNode[] components = Utils.GetComponents<PathNode>(Pathfinders[i], Utils.SearchChildren, true);
			for (int j = 0; j < components.Length; j++)
			{
				pathNodes.Add(components[j]);
				if (components[j].Type == PathNodeBase.NodeType.Door)
				{
					doorNodes.Add(components[j]);
				}
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(killZoneBounds.center, killZoneBounds.size);
	}
}
