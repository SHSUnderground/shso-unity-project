using System;
using System.Collections.Generic;
using UnityEngine;

public class HqObject2 : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	internal class HqObjectPlacement : IDisposable, IShsState
	{
		protected HqObject2 parent;

		internal HqObjectPlacement(HqObject2 parent)
		{
			this.parent = parent;
		}

		public void Dispose()
		{
			parent = null;
		}

		public void Enter(Type previousState)
		{
			parent.SetThrowableLayer();
			PlacedItem component = Utils.GetComponent<PlacedItem>(parent.gameObject, Utils.SearchChildren);
			if (component != null)
			{
				component.SetThrowableLayer();
			}
			if (parent.gameObject.rigidbody != null)
			{
				parent.gameObject.rigidbody.isKinematic = true;
				parent.gameObject.rigidbody.useGravity = false;
			}
			Collider[] ourColliders = parent.ourColliders;
			foreach (Collider collider in ourColliders)
			{
				collider.isTrigger = true;
			}
			parent.isUsableByAI = true;
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	internal class HqObjectPlacementSelected : IDisposable, IShsState
	{
		protected HqObject2 parent;

		internal HqObjectPlacementSelected(HqObject2 parent)
		{
			this.parent = parent;
		}

		public void Dispose()
		{
			parent = null;
		}

		public void Enter(Type previousState)
		{
			SelectedObjectController component = Utils.GetComponent<SelectedObjectController>(Camera.main);
			component.AddObject(parent.gameObject);
			Utils.SetLayerTree(parent.gameObject, 2);
			parent.gameObject.rigidbody.isKinematic = true;
			parent.gameObject.rigidbody.useGravity = false;
			Collider[] ourColliders = parent.ourColliders;
			foreach (Collider collider in ourColliders)
			{
				collider.isTrigger = true;
			}
			parent.isUsableByAI = false;
			HqItem component2 = Utils.GetComponent<HqItem>(parent.gameObject);
			if (component2 != null)
			{
				component2.CancelUsers();
			}
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
			SelectedObjectController component = Utils.GetComponent<SelectedObjectController>(Camera.main);
			component.DelObject(parent.gameObject);
			parent.SetThrowableLayer();
			PlacedItem component2 = Utils.GetComponent<PlacedItem>(parent.gameObject, Utils.SearchChildren);
			if (component2 != null)
			{
				component2.SetThrowableLayer();
			}
		}
	}

	internal class HqObjectFlinga : IShsState
	{
		protected const float maxSleepingTime = 0.75f;

		protected const float minSpeed = 0.25f;

		protected const float minVerticalMoveTime = 0.25f;

		protected HqObject2 parent;

		protected float sleepyTime;

		protected bool isAwake;

		protected Vector3 lastPosition;

		internal HqObjectFlinga(HqObject2 parent)
		{
			this.parent = parent;
		}

		public void Dispose()
		{
			parent = null;
		}

		public void Enter(Type previousState)
		{
			parent.isUsableByAI = true;
			parent.SetThrowableLayer();
			PlacedItem component = Utils.GetComponent<PlacedItem>(parent.gameObject, Utils.SearchChildren);
			if (component != null)
			{
				component.SetThrowableLayer();
			}
			Collider[] ourColliders = parent.ourColliders;
			foreach (Collider collider in ourColliders)
			{
				EntryPoint component2 = Utils.GetComponent<EntryPoint>(collider);
				HqTrigger component3 = Utils.GetComponent<HqTrigger>(collider);
				if (component2 != null || component3 != null)
				{
					collider.isTrigger = true;
				}
				else
				{
					collider.isTrigger = false;
				}
			}
			HqItem component4 = Utils.GetComponent<HqItem>(parent.gameObject);
			if (parent.aiController == null && (component4 == null || !component4.IsInAIControl))
			{
				parent.gameObject.rigidbody.isKinematic = false;
				parent.gameObject.rigidbody.useGravity = true;
			}
			sleepyTime = Time.time;
			isAwake = false;
			lastPosition = parent.gameObject.transform.position;
			parent.StartActiveSequence();
			parent.gameObject.active = false;
			parent.gameObject.active = true;
		}

		public void Update()
		{
			if (parent.gameObject.rigidbody != null && !parent.gameObject.rigidbody.isKinematic && parent.gameObject.rigidbody.velocity.magnitude > HqController2.Instance.Input.tweakFlingaHand.maxVelocity)
			{
				parent.gameObject.rigidbody.velocity = parent.gameObject.rigidbody.velocity.normalized * HqController2.Instance.Input.tweakFlingaHand.maxVelocity;
			}
			if (parent.aiController != null && parent.gameObject.rigidbody != null)
			{
				if (!parent.gameObject.rigidbody.IsSleeping())
				{
					sleepyTime = Time.time;
				}
			}
			else
			{
				sleepyTime = Time.time + 0.75f;
			}
			if (!isAwake && Time.time - sleepyTime >= 0.75f && parent.aiController != null)
			{
				CspUtils.DebugLog("Waking up " + parent.name);
				parent.aiController.DropAI();
				isAwake = true;
			}
		}

		public void Leave(Type nextState)
		{
			parent.StopActiveSequence();
		}
	}

	internal class HqObjectFlingaSelected : IDisposable, IShsState
	{
		protected HqObject2 parent;

		internal HqObjectFlingaSelected(HqObject2 parent)
		{
			this.parent = parent;
		}

		public void Dispose()
		{
			parent = null;
		}

		public void Enter(Type previousState)
		{
			parent.isUsableByAI = false;
			SelectedObjectController component = Utils.GetComponent<SelectedObjectController>(Camera.main);
			component.AddObject(parent.gameObject);
			Utils.SetLayerTree(parent.gameObject, 2);
			parent.gameObject.rigidbody.isKinematic = true;
			parent.gameObject.rigidbody.useGravity = false;
			Collider[] ourColliders = parent.ourColliders;
			foreach (Collider collider in ourColliders)
			{
				EntryPoint component2 = Utils.GetComponent<EntryPoint>(collider);
				HqTrigger component3 = Utils.GetComponent<HqTrigger>(collider);
				if (component2 != null || component3 != null)
				{
					collider.isTrigger = true;
				}
				else
				{
					collider.isTrigger = false;
				}
			}
			HqItem component4 = Utils.GetComponent<HqItem>(parent.gameObject);
			if (component4 != null)
			{
				component4.CancelUsers();
			}
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
			SelectedObjectController component = Utils.GetComponent<SelectedObjectController>(Camera.main);
			component.DelObject(parent.gameObject);
			parent.SetThrowableLayer();
			PlacedItem component2 = Utils.GetComponent<PlacedItem>(parent.gameObject, Utils.SearchChildren);
			if (component2 != null)
			{
				component2.SetThrowableLayer();
			}
		}
	}

	internal class HqObjectDestroyed : IDisposable, IShsState
	{
		protected HqObject2 parent;

		internal HqObjectDestroyed(HqObject2 parent)
		{
			this.parent = parent;
		}

		public void Enter(Type previousState)
		{
			parent.isUsableByAI = false;
			if (parent != null)
			{
				Utils.ActivateTree(parent.gameObject, false);
			}
		}

		public void Update()
		{
		}

		public void Dispose()
		{
			parent = null;
		}

		public void Leave(Type nextState)
		{
		}
	}

	public delegate void DelegateFixedUpdate();

	protected const int MAX_COLLISION_AUDIO = 3;

	public bool fixedUpdate;

	public Vector3 fixedUpdatePos = Vector3.zero;

	public Quaternion fixedUpdateRot = Quaternion.identity;

	public DelegateFixedUpdate fixedUpdateFunction;

	public EffectSequence activeSequence;

	protected EffectSequence activeSequenceInst;

	protected HqRoom2 room;

	internal ShsFSM fsm;

	internal Collider[] ourColliders;

	internal Bounds localBounds;

	protected AIControllerHQ aiController;

	protected string iconSource;

	protected string inventoryId;

	protected int placedId = -1;

	protected Quaternion defaultRotation = Quaternion.identity;

	protected ShsAudioSource[] collisionAudioSources = new ShsAudioSource[3];

	protected ShsAudioSource[] worldCollisionAudioSources = new ShsAudioSource[3];

	protected bool isUsableByAI;

	public Bounds Bounds
	{
		get
		{
			Bounds result = localBounds;
			result.center = base.transform.position;
			return result;
		}
	}

	public AIControllerHQ AIController
	{
		get
		{
			return aiController;
		}
		set
		{
			aiController = value;
		}
	}

	public string IconSource
	{
		get
		{
			if (iconSource != null)
			{
				return iconSource;
			}
			if (inventoryId != null && HqController2.Instance.Profile != null)
			{
				UserProfile profile = HqController2.Instance.Profile;
				Item value = null;
				if (profile.AvailableItems.TryGetValue(inventoryId, out value))
				{
					return "items_bundle|" + value.Definition.Icon;
				}
			}
			return "persistent_bundle|inventory_icon_wip_normal";
		}
		set
		{
			iconSource = value;
		}
	}

	public string InventoryId
	{
		get
		{
			return inventoryId;
		}
		set
		{
			inventoryId = value;
			PhysicsInit component = Utils.GetComponent<PhysicsInit>(base.gameObject);
			if (component != null)
			{
				Item value2 = null;
				HqController2.Instance.Profile.AvailableItems.TryGetValue(inventoryId, out value2);
				if (value2 != null && value2.Definition.Material != 0)
				{
					string value3 = value2.Definition.Material.ToString();
					try
					{
						component.PhysicsMaterial = (PhysicMaterialEx.MaterialType)(int)Enum.Parse(typeof(PhysicMaterialEx.MaterialType), value3);
					}
					catch
					{
						CspUtils.DebugLog("Error parsing physics material!");
					}
				}
			}
		}
	}

	public int PlacedId
	{
		get
		{
			return placedId;
		}
		set
		{
			if (placedId == -1 || value == -1)
			{
				placedId = value;
			}
			else
			{
				CspUtils.DebugLog("Object already has a PlaceId");
			}
		}
	}

	public Quaternion DefaultRotation
	{
		get
		{
			return defaultRotation;
		}
	}

	public Type State
	{
		get
		{
			return fsm.GetCurrentState();
		}
	}

	protected HqRoom2 Room
	{
		get
		{
			if (room == null)
			{
				room = Utils.GetComponent<HqRoom2>(base.transform, Utils.SearchParents);
			}
			return room;
		}
	}

	public bool IsUsableByAI
	{
		get
		{
			return isUsableByAI;
		}
	}

	public void Awake()
	{
		InitColliders();
		fsm = new ShsFSM();
		fsm.AddState(new HqObjectPlacement(this));
		fsm.AddState(new HqObjectPlacementSelected(this));
		fsm.AddState(new HqObjectFlinga(this));
		fsm.AddState(new HqObjectFlingaSelected(this));
		fsm.AddState(new HqObjectDestroyed(this));
		fsm.GotoState<HqObjectPlacement>();
	}

	public void OnDisable()
	{
		if (Camera.main != null)
		{
			SelectedObjectController component = Camera.main.GetComponent<SelectedObjectController>();
			if (component != null)
			{
				component.DelObject(base.gameObject);
			}
		}
	}

	public void Start()
	{
		defaultRotation = base.transform.rotation;
		bool flag = false;
		Quaternion rotation = Quaternion.identity;
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		if (!flag)
		{
			MeshFilter component = Utils.GetComponent<MeshFilter>(base.gameObject, Utils.SearchChildren);
			if (component != null)
			{
				flag = true;
				rotation = component.transform.rotation;
				vector = component.mesh.bounds.min;
				vector2 = component.mesh.bounds.max;
			}
		}
		if (!flag)
		{
			Collider component2 = Utils.GetComponent<Collider>(base.gameObject);
			if (component2 != null)
			{
				flag = true;
				vector = component2.bounds.min - base.transform.position;
				vector2 = component2.bounds.max - base.transform.position;
			}
		}
		if (!flag)
		{
			SkinnedMeshRenderer component3 = Utils.GetComponent<SkinnedMeshRenderer>(base.gameObject, Utils.SearchChildren);
			if (component3 != null)
			{
				flag = true;
				vector = component3.sharedMesh.bounds.min;
				vector2 = component3.sharedMesh.bounds.max;
			}
		}
		if (!flag)
		{
			CspUtils.DebugLog("Unable to initialize local bounds");
		}
		Vector3[] array = new Vector3[8]
		{
			rotation * new Vector3(vector.x, vector.y, vector.z),
			rotation * new Vector3(vector.x, vector.y, vector2.z),
			rotation * new Vector3(vector.x, vector2.y, vector.z),
			rotation * new Vector3(vector.x, vector2.y, vector2.z),
			rotation * new Vector3(vector2.x, vector.y, vector.z),
			rotation * new Vector3(vector2.x, vector.y, vector2.z),
			rotation * new Vector3(vector2.x, vector2.y, vector.z),
			rotation * new Vector3(vector2.x, vector2.y, vector2.z)
		};
		localBounds = new Bounds(array[0], Vector3.zero);
		for (int i = 1; i < array.Length; i++)
		{
			localBounds.Encapsulate(array[i]);
		}
		for (int j = 0; j < collisionAudioSources.Length; j++)
		{
			collisionAudioSources[j] = null;
		}
		if (activeSequence != null && base.animation != null && !activeSequence.AutoStart)
		{
			base.animation.Stop();
		}
	}

	public void Activate()
	{
		if (State == typeof(HqObjectFlinga))
		{
			StartActiveSequence();
		}
		else if (activeSequence != null && base.animation != null && !activeSequence.AutoStart)
		{
			base.animation.Stop();
		}
	}

	public void Update()
	{
		fsm.Update();
	}

	public void FixedUpdate()
	{
		if (fixedUpdateFunction != null)
		{
			fixedUpdateFunction();
		}
		if (State == typeof(HqObjectFlinga) && (base.rigidbody.velocity.sqrMagnitude > base.rigidbody.sleepVelocity * base.rigidbody.sleepVelocity || base.rigidbody.angularVelocity.sqrMagnitude > base.rigidbody.sleepAngularVelocity * base.rigidbody.sleepAngularVelocity) && Room != null && !Room.KillZoneBounds.Contains(base.gameObject.transform.position))
		{
			CspUtils.DebugLog(base.gameObject.name + " is outside killzone bounds. Respawning.");
			Room.RespawnObject(this);
		}
		else if (fixedUpdate)
		{
			base.rigidbody.MovePosition(fixedUpdatePos);
			base.rigidbody.MoveRotation(fixedUpdateRot);
			fixedUpdate = false;
		}
	}

	public void OnCollisionEnter(Collision c)
	{
		PlayCollisionAudio(c);
	}

	public void OnCollisionExit(Collision c)
	{
	}

	public void OnTriggerEnter(Collider c)
	{
	}

	public void OnTriggerExit(Collider c)
	{
	}

	public void GotoPlacementMode(bool force)
	{
		if (force || fsm.GetCurrentState() != typeof(HqObjectPlacementSelected))
		{
			fsm.GotoState<HqObjectPlacement>();
		}
	}

	public void GotoPlacementSelectedMode()
	{
		fsm.GotoState<HqObjectPlacementSelected>();
	}

	public void GotoFlingaMode()
	{
		fsm.GotoState<HqObjectFlinga>();
	}

	public void GotoFlingaSelectedMode()
	{
		fsm.GotoState<HqObjectFlingaSelected>();
	}

	public void GotoDestroyedMode()
	{
		fsm.GotoState<HqObjectDestroyed>();
	}

	public void CopyFrom(HqObject2 otherComp)
	{
		activeSequence = otherComp.activeSequence;
	}

	protected void DrawBoundingBox(Bounds b)
	{
		Vector3[] array = new Vector3[8]
		{
			b.min,
			default(Vector3),
			default(Vector3),
			default(Vector3),
			default(Vector3),
			default(Vector3),
			default(Vector3),
			default(Vector3)
		};
		//ref Vector3 reference = ref array[1];
		Vector3 min = b.min;
		float x = min.x;
		Vector3 min2 = b.min;
		float y = min2.y;
		Vector3 max = b.max;
		array[1] = new Vector3(x, y, max.z);
		//ref Vector3 reference2 = ref array[2];
		Vector3 max2 = b.max;
		float x2 = max2.x;
		Vector3 min3 = b.min;
		float y2 = min3.y;
		Vector3 min4 = b.min;
		array[2] = new Vector3(x2, y2, min4.z);
		//ref Vector3 reference3 = ref array[3];
		Vector3 max3 = b.max;
		float x3 = max3.x;
		Vector3 min5 = b.min;
		float y3 = min5.y;
		Vector3 max4 = b.max;
		array[3] = new Vector3(x3, y3, max4.z);
		//ref Vector3 reference4 = ref array[4];
		Vector3 min6 = b.min;
		float x4 = min6.x;
		Vector3 max5 = b.max;
		float y4 = max5.y;
		Vector3 min7 = b.min;
		array[4] = new Vector3(x4, y4, min7.z);
		//ref Vector3 reference5 = ref array[5];
		Vector3 min8 = b.min;
		float x5 = min8.x;
		Vector3 max6 = b.max;
		float y5 = max6.y;
		Vector3 max7 = b.max;
		array[5] = new Vector3(x5, y5, max7.z);
		//ref Vector3 reference6 = ref array[6];
		Vector3 max8 = b.max;
		float x6 = max8.x;
		Vector3 max9 = b.max;
		float y6 = max9.y;
		Vector3 min9 = b.min;
		array[6] = new Vector3(x6, y6, min9.z);
		array[7] = b.max;
		Debug.DrawLine(array[0], array[1], Color.green);
		Debug.DrawLine(array[0], array[2], Color.green);
		Debug.DrawLine(array[1], array[3], Color.green);
		Debug.DrawLine(array[2], array[3], Color.green);
		Debug.DrawLine(array[4], array[5], Color.green);
		Debug.DrawLine(array[4], array[6], Color.green);
		Debug.DrawLine(array[5], array[7], Color.green);
		Debug.DrawLine(array[6], array[7], Color.green);
		Debug.DrawLine(array[0], array[4], Color.green);
		Debug.DrawLine(array[1], array[5], Color.green);
		Debug.DrawLine(array[2], array[6], Color.green);
		Debug.DrawLine(array[3], array[7], Color.green);
	}

	protected void DrawSquare(Vector3 point, Color c)
	{
		Debug.DrawLine(new Vector3(point.x - 0.5f, point.y, point.z + 0.5f), new Vector3(point.x + 0.5f, point.y, point.z + 0.5f), c);
		Debug.DrawLine(new Vector3(point.x + 0.5f, point.y, point.z + 0.5f), new Vector3(point.x + 0.5f, point.y, point.z - 0.5f), c);
		Debug.DrawLine(new Vector3(point.x + 0.5f, point.y, point.z - 0.5f), new Vector3(point.x - 0.5f, point.y, point.z - 0.5f), c);
		Debug.DrawLine(new Vector3(point.x - 0.5f, point.y, point.z - 0.5f), new Vector3(point.x - 0.5f, point.y, point.z + 0.5f), c);
	}

	public bool PlaceAt(Vector3 target)
	{
		if (ourColliders.Length == 0)
		{
			CspUtils.DebugLog("HqObject has no colliders: " + base.gameObject.name);
			return false;
		}
		Bounds bounds = default(Bounds);
		for (int i = 0; i < ourColliders.Length; i++)
		{
			if (i == 0)
			{
				bounds = new Bounds(ourColliders[i].bounds.center, ourColliders[i].bounds.size);
			}
			else
			{
				bounds.Encapsulate(ourColliders[i].bounds);
			}
		}
		Vector3 center = bounds.center;
		float y = center.y;
		Vector3 position = base.gameObject.transform.position;
		float num = y - position.y;
		float x = target.x;
		float y2 = target.y;
		Vector3 size = bounds.size;
		bounds.center = new Vector3(x, y2 + size.y / 2f, target.z);
		Vector3[] array = new Vector3[9]
		{
			bounds.max,
			default(Vector3),
			default(Vector3),
			default(Vector3),
			default(Vector3),
			default(Vector3),
			default(Vector3),
			default(Vector3),
			default(Vector3)
		};
		//ref Vector3 reference = ref array[1];
		Vector3 min = bounds.min;
		float x2 = min.x;
		Vector3 max = bounds.max;
		float y3 = max.y;
		Vector3 max2 = bounds.max;
		array[1] = new Vector3(x2, y3, max2.z);
		//ref Vector3 reference2 = ref array[2];
		Vector3 max3 = bounds.max;
		float x3 = max3.x;
		Vector3 max4 = bounds.max;
		float y4 = max4.y;
		Vector3 min2 = bounds.min;
		array[2] = new Vector3(x3, y4, min2.z);
		//ref Vector3 reference3 = ref array[3];
		Vector3 min3 = bounds.min;
		float x4 = min3.x;
		Vector3 max5 = bounds.max;
		float y5 = max5.y;
		Vector3 min4 = bounds.min;
		array[3] = new Vector3(x4, y5, min4.z);
		//ref Vector3 reference4 = ref array[4];
		Vector3 center2 = bounds.center;
		float x5 = center2.x;
		Vector3 max6 = bounds.max;
		float y6 = max6.y;
		Vector3 center3 = bounds.center;
		array[4] = new Vector3(x5, y6, center3.z);
		array[5] = (array[0] + array[1]) * 0.5f;
		array[6] = (array[1] + array[3]) * 0.5f;
		array[7] = (array[3] + array[2]) * 0.5f;
		array[8] = (array[2] + array[0]) * 0.5f;
		for (int j = 0; j < array.Length; j++)
		{
			Debug.DrawRay(array[j], Vector3.down * Room.ceilingHeight * 2f, Color.cyan);
		}
		int layerMask = 4694016;
		bool flag = true;
		float num2 = float.MinValue;
		Vector3[] array2 = array;
		foreach (Vector3 vector in array2)
		{
			Vector3 origin = vector;
			origin.y = Room.ceilingHeight;
			Ray ray = new Ray(origin, Vector3.down);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, Room.ceilingHeight * 2f, layerMask))
			{
				if (hitInfo.collider != null)
				{
					HqObject2 component = Utils.GetComponent<HqObject2>(hitInfo.collider.gameObject, Utils.SearchChildren);
					if (component != null)
					{
						Vector3 max7 = hitInfo.collider.bounds.max;
						if (max7.y > num2)
						{
							Vector3 max8 = hitInfo.collider.bounds.max;
							num2 = max8.y;
						}
					}
				}
				Vector3 point = hitInfo.point;
				if (point.y > num2)
				{
					Vector3 point2 = hitInfo.point;
					num2 = point2.y;
				}
				continue;
			}
			flag = false;
			break;
		}
		float num3 = num2;
		Vector3 size2 = bounds.size;
		float y7 = num3 + size2.y / 2f - num;
		if (flag)
		{
			Vector3[] array3 = new Vector3[4];
			//ref Vector3 reference5 = ref array3[0];
			Vector3 min5 = bounds.min;
			float x6 = min5.x;
			Vector3 min6 = bounds.min;
			array3[0] = new Vector3(x6, y7, min6.z);
			//ref Vector3 reference6 = ref array3[1];
			Vector3 max9 = bounds.max;
			float x7 = max9.x;
			Vector3 min7 = bounds.min;
			array3[1] = new Vector3(x7, y7, min7.z);
			//ref Vector3 reference7 = ref array3[2];
			Vector3 max10 = bounds.max;
			float x8 = max10.x;
			Vector3 max11 = bounds.max;
			array3[2] = new Vector3(x8, y7, max11.z);
			//ref Vector3 reference8 = ref array3[3];
			Vector3 min8 = bounds.min;
			float x9 = min8.x;
			Vector3 max12 = bounds.max;
			array3[3] = new Vector3(x9, y7, max12.z);
			int num4 = 1;
			int num5 = 0;
			while (num5 < 4)
			{
				if (num5 == 3)
				{
					num4 = 0;
				}
				Debug.DrawLine(array3[num5], array3[num4], Color.red);
				if (Physics.Linecast(array3[num5], array3[num4], 4595712))
				{
					flag = false;
					break;
				}
				num5++;
				num4++;
			}
			if (flag)
			{
				Vector3 b = array3[1] - array3[0];
				Vector3 b2 = array3[2] - array3[1];
				Vector3 vector2 = b2.normalized * 0.5f;
				Vector3 vector3 = b.normalized * 0.5f;
				Vector3 vector4 = array3[0] + vector2;
				int num6 = (int)(b2.magnitude / 0.5f);
				for (int l = 0; l < num6; l++)
				{
					Debug.DrawLine(vector4, vector4 + b, Color.blue);
					if (Physics.Linecast(vector4, vector4 + b, 4595712))
					{
						flag = false;
						break;
					}
					vector4 += vector2;
				}
				if (flag)
				{
					vector4 = array3[0] + vector3;
					int num7 = (int)(b.magnitude / 0.5f);
					for (int m = 0; m < num7; m++)
					{
						Debug.DrawLine(vector4, vector4 + b2, Color.green);
						if (Physics.Linecast(vector4, vector4 + b2, 4595712))
						{
							flag = false;
							break;
						}
						vector4 += vector3;
					}
				}
			}
		}
		if (flag)
		{
			Vector3 vector5 = new Vector3(target.x, y7, target.z);
			Vector3 vector6 = vector5 + bounds.extents;
			if (vector6.y > Room.ceilingHeight)
			{
				return false;
			}
			base.gameObject.transform.localPosition = vector5;
			DrawSquare(base.gameObject.transform.position, Color.yellow);
			return true;
		}
		return false;
	}

	public void SetThrowableLayer()
	{
		Utils.SetLayerTree(base.gameObject, 13);
		HqTrigger[] components = Utils.GetComponents<HqTrigger>(base.gameObject, Utils.SearchChildren);
		if (components != null)
		{
			HqTrigger[] array = components;
			foreach (HqTrigger hqTrigger in array)
			{
				hqTrigger.gameObject.layer = 2;
			}
		}
	}

	protected void PlayCollisionAudio(Collision c)
	{
		if (!(c.relativeVelocity.sqrMagnitude > 1f))
		{
			return;
		}
		ShsAudioSource audioSource = GetAudioSource();
		if (audioSource != null)
		{
			audioSource.Play();
		}
		if (c.gameObject.layer == 17 || c.gameObject.layer == 15)
		{
			audioSource = GetWorldAudioSource();
			if (audioSource != null)
			{
				audioSource.Play();
			}
		}
	}

	protected ShsAudioSource GetAudioSource()
	{
		ShsAudioSource shsAudioSource = null;
		for (int i = 0; i < collisionAudioSources.Length; i++)
		{
			if (collisionAudioSources[i] != null)
			{
				if (!collisionAudioSources[i].IsPlaying)
				{
					shsAudioSource = collisionAudioSources[i];
					break;
				}
				continue;
			}
			ShsAudioSource shsAudioSource2 = null;
			CustomPhysicsSFX component = Utils.GetComponent<CustomPhysicsSFX>(base.gameObject, Utils.SearchChildren);
			if (component != null)
			{
				shsAudioSource2 = component.collisionAudio;
			}
			else
			{
				PhysicsInit component2 = Utils.GetComponent<PhysicsInit>(base.gameObject, Utils.SearchChildren);
				if (component2 != null)
				{
					PhysicMaterialEx physicMaterialEx = PhysicMatMapping.Instance[component2.PhysicsMaterial];
					if (physicMaterialEx != null)
					{
						shsAudioSource2 = physicMaterialEx.GetCollisionAudio(component2.volume);
					}
				}
			}
			if (shsAudioSource2 != null)
			{
				collisionAudioSources[i] = (UnityEngine.Object.Instantiate(shsAudioSource2) as ShsAudioSource);
				shsAudioSource = collisionAudioSources[i];
				if (shsAudioSource != null)
				{
					shsAudioSource.transform.parent = base.gameObject.transform;
					shsAudioSource.transform.localPosition = Vector3.zero;
				}
				break;
			}
		}
		return shsAudioSource;
	}

	protected ShsAudioSource GetWorldAudioSource()
	{
		ShsAudioSource shsAudioSource = null;
		for (int i = 0; i < worldCollisionAudioSources.Length; i++)
		{
			if (worldCollisionAudioSources[i] != null)
			{
				if (!worldCollisionAudioSources[i].IsPlaying)
				{
					shsAudioSource = worldCollisionAudioSources[i];
					break;
				}
				continue;
			}
			PhysicsInit component = Utils.GetComponent<PhysicsInit>(base.gameObject, Utils.SearchChildren);
			if (!(component != null))
			{
				continue;
			}
			PhysicMaterialEx physicMaterialEx = PhysicMatMapping.Instance[PhysicMaterialEx.MaterialType.HQ];
			if (physicMaterialEx == null)
			{
				continue;
			}
			ShsAudioSource collisionAudio = physicMaterialEx.GetCollisionAudio(component.volume);
			if (collisionAudio != null)
			{
				worldCollisionAudioSources[i] = (UnityEngine.Object.Instantiate(collisionAudio) as ShsAudioSource);
				shsAudioSource = worldCollisionAudioSources[i];
				if (shsAudioSource != null)
				{
					shsAudioSource.transform.parent = base.gameObject.transform;
					shsAudioSource.transform.localPosition = Vector3.zero;
				}
				break;
			}
		}
		return shsAudioSource;
	}

	protected void StartActiveSequence()
	{
		if (activeSequence != null && activeSequenceInst == null)
		{
			activeSequenceInst = (UnityEngine.Object.Instantiate(activeSequence, Vector3.zero, Quaternion.identity) as EffectSequence);
			activeSequenceInst.transform.parent = base.gameObject.transform;
			activeSequenceInst.Initialize(base.gameObject, null, null);
			activeSequenceInst.transform.localPosition = new Vector3(0f, 0f, 0f);
			activeSequenceInst.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
			activeSequenceInst.StartSequence();
		}
	}

	protected void StopActiveSequence()
	{
		if (activeSequenceInst != null)
		{
			activeSequenceInst.Cancel();
			if (base.gameObject.animation != null)
			{
				base.gameObject.animation.Stop();
			}
			UnityEngine.Object.Destroy(activeSequenceInst);
			activeSequenceInst = null;
		}
	}

	protected void InitColliders()
	{
		Collider[] components = Utils.GetComponents<Collider>(base.gameObject, Utils.SearchChildren, true);
		List<Collider> list = new List<Collider>();
		Collider[] array = components;
		foreach (Collider collider in array)
		{
			EntryPoint component = Utils.GetComponent<EntryPoint>(collider.gameObject);
			HqToy component2 = Utils.GetComponent<HqToy>(collider.gameObject);
			if (component == null && component2 == null)
			{
				list.Add(collider);
			}
		}
		ourColliders = list.ToArray();
	}
}
