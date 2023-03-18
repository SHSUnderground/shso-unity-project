using System.Collections.Generic;
using UnityEngine;

public class HqItem : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public class Claim
	{
		public HqItem item;

		public DockPoint dockPoint;

		private AIControllerHQ claimer;

		private HqIcon claimerIcon;

		public Vector3 Position
		{
			get
			{
				if (dockPoint != null)
				{
					return dockPoint.transform.position;
				}
				if (item != null)
				{
					return item.gameObject.transform.position;
				}
				CspUtils.DebugLog("Trying to retrieve position on a claim that has neither a dock point or an hqitem!");
				return Vector3.zero;
			}
		}

		public AIControllerHQ Claimer
		{
			get
			{
				return claimer;
			}
			set
			{
				if (claimer != null && value != claimer)
				{
					CspUtils.DebugLog("Claim.Claimer is not null! Current claimer is " + claimer.CharacterName + " " + value.CharacterName + " is trying to claim " + item.gameObject.name);
				}
				if (value != null)
				{
					claimer = value;
				}
				else
				{
					CspUtils.DebugLog("Claim.Claimer is being set to null! Use ReleaseClaim instead!");
				}
				SetIcon();
			}
		}

		public Claim(HqItem item, DockPoint dockPoint)
		{
			this.item = item;
			this.dockPoint = dockPoint;
			Transform transform = item.transform;
			Vector3 position = Position;
			float x = position.x;
			Vector3 position2 = Position;
			float y = position2.y + 1f;
			Vector3 position3 = Position;
			claimerIcon = new HqIcon(transform, new Vector3(x, y, position3.z));
		}

		private void SetIcon()
		{
			if (!(item != null) || !(item.gameObject != null) || !item.gameObject.active || claimerIcon == null)
			{
				return;
			}
			if (claimer != null)
			{
				if (claimer.Icon != null)
				{
					claimerIcon.Icon = claimer.Icon;
				}
				if (item.Paused && !claimerIcon.Visible)
				{
					claimerIcon.Visible = true;
				}
			}
			if (claimer == null || (!item.Paused && claimerIcon.Visible))
			{
				claimerIcon.Visible = false;
			}
		}

		public void ReleaseClaim(AIControllerHQ aiController)
		{
			if (claimer == aiController)
			{
				claimer = null;
				SetIcon();
			}
		}

		public void Update()
		{
			SetIcon();
		}
	}

	protected class SequencePlayInfo
	{
		public List<GameObject> users;

		public EffectSequence sequenceInstance;

		public SequencePlayInfo(EffectSequence sequence)
		{
			users = new List<GameObject>();
			sequenceInstance = sequence;
		}
	}

	protected HqRoom2 room;

	protected Vector3 position;

	protected ItemDefinition itemDefinition;

	protected bool loadedOrSaved;

	protected Dictionary<string, SequencePlayInfo> sequencesPlaying;

	protected Quaternion defaultRotation;

	protected Component[] dockPoints;

	protected Component[] entryPoints;

	protected bool isInAIControl;

	protected Quaternion rotation;

	protected bool isDestroyed;

	protected bool isSaved;

	protected List<Claim> claims = new List<Claim>();

	protected AIControllerHQ controller;

	protected bool paused;

	public Claim[] Claims
	{
		get
		{
			return claims.ToArray();
		}
	}

	public virtual bool Paused
	{
		get
		{
			return paused;
		}
		set
		{
			paused = value;
			if (sequencesPlaying != null)
			{
				foreach (SequencePlayInfo value2 in sequencesPlaying.Values)
				{
					if (value2.sequenceInstance != null)
					{
						value2.sequenceInstance.Paused = value;
					}
				}
			}
		}
	}

	public Quaternion Rotation
	{
		get
		{
			return base.gameObject.transform.rotation;
		}
		set
		{
			base.gameObject.transform.rotation = value;
		}
	}

	public Vector3 Position
	{
		get
		{
			return base.gameObject.transform.position;
		}
		set
		{
			base.gameObject.transform.position = value;
		}
	}

	public HqRoom2 Room
	{
		get
		{
			return room;
		}
		set
		{
			room = value;
		}
	}

	public Component[] EntryPoints
	{
		get
		{
			if (entryPoints == null)
			{
				entryPoints = Utils.GetComponents<EntryPoint>(base.gameObject, Utils.SearchChildren, true);
			}
			return entryPoints;
		}
	}

	public Component[] DockPoints
	{
		get
		{
			if (dockPoints == null)
			{
				dockPoints = Utils.GetComponents<DockPoint>(base.gameObject, Utils.SearchChildren, true);
			}
			return dockPoints;
		}
	}

	public virtual ItemDefinition ItemDefinition
	{
		get
		{
			return itemDefinition;
		}
	}

	public bool IsMultiUser
	{
		get
		{
			if (ItemDefinition != null && ItemDefinition.UseInfo != null)
			{
				return ItemDefinition.UseInfo.Uses.Count > 1;
			}
			return false;
		}
	}

	public Claim NextAvailableClaim
	{
		get
		{
			foreach (Claim claim in claims)
			{
				if (claim.Claimer == null)
				{
					return claim;
				}
			}
			return null;
		}
	}

	public bool IsUsableItem
	{
		get
		{
			if (ItemDefinition == null)
			{
				return false;
			}
			return ItemDefinition.CanAIUse;
		}
	}

	public bool IsBeingUsedByAI
	{
		get
		{
			foreach (Claim claim in claims)
			{
				if (claim.Claimer != null)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool IsInAIControl
	{
		get
		{
			return controller != null;
		}
	}

	public AIControllerHQ AIInControl
	{
		get
		{
			return controller;
		}
	}

	public bool IsDestroyed
	{
		get
		{
			return isDestroyed;
		}
		set
		{
			isDestroyed = value;
		}
	}

	public bool IsSaved
	{
		get
		{
			return isSaved;
		}
	}

	public virtual float HungerValue
	{
		get
		{
			if (ItemDefinition != null)
			{
				return ItemDefinition.HungerValue;
			}
			return 0f;
		}
	}

	public bool IsMoving
	{
		get
		{
			if (base.gameObject != null && base.gameObject.rigidbody != null && !base.gameObject.rigidbody.isKinematic)
			{
				return base.gameObject.rigidbody.velocity.sqrMagnitude > Physics.sleepVelocity;
			}
			return false;
		}
	}

	protected void CreateClaims()
	{
		claims.Clear();
		if (DockPoints.Length > 0)
		{
			Component[] array = DockPoints;
			for (int i = 0; i < array.Length; i++)
			{
				DockPoint dockPoint = (DockPoint)array[i];
				Claim item = new Claim(this, dockPoint);
				claims.Add(item);
			}
		}
		else if (IsUsableItem)
		{
			Claim item2 = new Claim(this, null);
			claims.Add(item2);
		}
	}

	public void Initialize(HqRoom2 room)
	{
		Position = Vector3.zero;
		Rotation = Quaternion.identity;
		itemDefinition = null;
		Room = room;
		sequencesPlaying = new Dictionary<string, SequencePlayInfo>();
		loadedOrSaved = false;
		CreateClaims();
	}

	public void Initialize(HqRoom2 room, ItemDefinition itemDef)
	{
		Room = room;
		itemDefinition = itemDef;
		sequencesPlaying = new Dictionary<string, SequencePlayInfo>();
		loadedOrSaved = false;
		CreateClaims();
	}

	public bool IsUpright(DockPoint dp)
	{
		if (ItemDefinition != null && ItemDefinition.UseInfo != null && ItemDefinition.UseInfo.Uses != null && ItemDefinition.UseInfo.Uses.ContainsKey(dp.Name))
		{
			Use use = ItemDefinition.UseInfo.Uses[dp.Name];
			float num = Vector3.Angle(base.gameObject.transform.forward, use.UseVector);
			return num < use.MaxUseAngleDelta;
		}
		return false;
	}

	public void Destroy()
	{
		CancelUsers();
		AssetBundle assetBundle = HqController2.Instance.GetAssetBundle("HQ/hq_shared");
		if (!(assetBundle != null))
		{
			return;
		}
		GameObject gameObject = assetBundle.Load("hq_genericdestruct_sequence") as GameObject;
		if (gameObject != null)
		{
			GameObject go = Object.Instantiate(gameObject, Vector3.zero, defaultRotation) as GameObject;
			EffectSequence component = Utils.GetComponent<EffectSequence>(go, Utils.SearchChildren);
			component.Initialize(base.gameObject, null, null);
			component.StartSequence();
			Utils.ActivateTree(base.gameObject, false);
			isDestroyed = true;
			if (!isSaved)
			{
				Room.DelItem(base.gameObject);
				AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(new string[1]
				{
					ItemDefinition.Id
				}, CollectionResetMessage.ActionType.Add, "Items"));
				Object.Destroy(base.gameObject);
			}
		}
	}

	public float StartSequence(string sequenceName, GameObject requestor, EffectSequence.OnSequenceEvent eventCallback, EffectSequence.OnSequenceDone doneCallback)
	{
		if (ItemDefinition != null && ItemDefinition.UseInfo != null)
		{
			SequenceInfo sequenceByName = ItemDefinition.UseInfo.GetSequenceByName(sequenceName);
			if (sequenceByName != null)
			{
				if (sequencesPlaying.ContainsKey(sequenceName) && sequencesPlaying[sequenceName].sequenceInstance == null)
				{
					sequencesPlaying.Remove(sequenceName);
				}
				if (!sequencesPlaying.ContainsKey(sequenceName))
				{
					GameObject prefab = sequenceByName.Prefab;
					if (prefab != null)
					{
						GameObject go = Object.Instantiate(prefab, Vector3.zero, defaultRotation) as GameObject;
						EffectSequence component = Utils.GetComponent<EffectSequence>(go, Utils.SearchChildren);
						if (component == null)
						{
							CspUtils.DebugLog("Effect sequence in prefab is null: " + prefab.name + " Please make sure the item definition is correct.");
							return 0f;
						}
						component.Initialize(base.gameObject, doneCallback, eventCallback);
						component.StartSequence();
						SequencePlayInfo sequencePlayInfo = new SequencePlayInfo(component);
						sequencePlayInfo.users.Add(requestor);
						sequencesPlaying[sequenceName] = sequencePlayInfo;
						return component.Lifetime;
					}
					CspUtils.DebugLog("ItemSequence prefab is null for " + ItemDefinition.Name + " " + sequenceByName.SequencePrefabName);
				}
				else
				{
					SequencePlayInfo sequencePlayInfo2 = sequencesPlaying[sequenceName];
					if (!sequencePlayInfo2.users.Contains(requestor))
					{
						sequencePlayInfo2.users.Add(requestor);
					}
				}
			}
		}
		return 0f;
	}

	public void StopSequence(string sequenceName, GameObject requestor)
	{
		if (ItemDefinition == null || ItemDefinition.UseInfo == null || !sequencesPlaying.ContainsKey(sequenceName))
		{
			return;
		}
		SequencePlayInfo sequencePlayInfo = sequencesPlaying[sequenceName];
		if (!sequencePlayInfo.users.Contains(requestor))
		{
			return;
		}
		sequencePlayInfo.users.Remove(requestor);
		if (sequencePlayInfo.users.Count == 0)
		{
			if (sequencesPlaying[sequenceName].sequenceInstance != null)
			{
				sequencesPlaying[sequenceName].sequenceInstance.Cancel();
			}
			sequencesPlaying.Remove(sequenceName);
		}
	}

	public void ResetRotation()
	{
		base.gameObject.transform.rotation = defaultRotation;
	}

	public Claim GetClosestAvailableClaim(Vector3 point)
	{
		Claim result = null;
		float num = float.MaxValue;
		foreach (Claim claim in claims)
		{
			if (claim != null && claim.Claimer == null)
			{
				float sqrMagnitude = (claim.Position - point).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = claim;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	public DockPoint GetDockPointByName(string dockPointName)
	{
		if (DockPoints != null)
		{
			Component[] array = DockPoints;
			for (int i = 0; i < array.Length; i++)
			{
				DockPoint dockPoint = (DockPoint)array[i];
				if (dockPoint.Name == dockPointName)
				{
					return dockPoint;
				}
			}
		}
		return null;
	}

	public void CancelUsers()
	{
		Claim[] array = Claims;
		foreach (Claim claim in array)
		{
			if (claim.Claimer != null)
			{
				claim.Claimer.TerminateObjectUse(base.gameObject);
			}
		}
		if (sequencesPlaying != null)
		{
			foreach (SequencePlayInfo value in sequencesPlaying.Values)
			{
				if (value.sequenceInstance != null)
				{
					value.sequenceInstance.Cancel();
				}
			}
		}
	}

	public virtual bool Use(AIControllerHQ ai, DockPoint dockPoint, BehaviorBase.OnBehaviorDone doneCallback)
	{
		if (isDestroyed)
		{
			return false;
		}
		BehaviorUseItem behaviorUseItem = ai.ChangeBehavior<BehaviorUseItem>(false);
		if (behaviorUseItem != null)
		{
			behaviorUseItem.Initialize(this, dockPoint, doneCallback);
			return true;
		}
		return false;
	}

	public void Save()
	{
		isSaved = true;
	}

	public void RevertToLastSave()
	{
		CancelUsers();
		if (isSaved && IsDestroyed)
		{
			Utils.ActivateTree(base.gameObject, true);
			isDestroyed = false;
		}
	}

	public void Update()
	{
		foreach (Claim claim in claims)
		{
			claim.Update();
		}
	}

	public bool TakeControl(AIControllerHQ aiController)
	{
		if (controller == null)
		{
			Claim[] array = Claims;
			foreach (Claim claim in array)
			{
				if (claim.Claimer != null && claim.Claimer != aiController)
				{
					claim.Claimer.TerminateObjectUse(base.gameObject);
				}
			}
			controller = aiController;
			return true;
		}
		return false;
	}

	public void ReleaseControl(AIControllerHQ aiController)
	{
		if (controller == aiController)
		{
			controller = null;
		}
	}

	public bool CanThrow(AIControllerHQ aiController)
	{
		if (controller != null && controller != aiController)
		{
			return false;
		}
		if (IsDestroyed)
		{
			return false;
		}
		if (IsBeingUsedByAI)
		{
			return false;
		}
		if (ItemDefinition != null)
		{
			return ItemDefinition.Weight < aiController.AIData.strength;
		}
		return true;
	}

	public bool CanDestroy(AIControllerHQ aiController)
	{
		if (controller != null && controller != aiController)
		{
			return false;
		}
		if (IsDestroyed)
		{
			return false;
		}
		if (IsBeingUsedByAI)
		{
			return false;
		}
		if (ItemDefinition != null)
		{
			return ItemDefinition.Strength < aiController.AIData.strength;
		}
		return true;
	}
}
