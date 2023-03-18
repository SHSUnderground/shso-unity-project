using UnityEngine;

public class ThrowableGround : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject carryPrefab;

	public float pickupDistance = 1.354f;

	public float weight = 3f;

	public string DestroyEvent;

	public Quaternion originalRotation;

	public float originalCharacterRotation;

	protected float destinationY;

	protected GameObject pickupCharacter;

	protected bool pendingResponse;

	public GameObject PickupCharacter
	{
		get
		{
			return pickupCharacter;
		}
		set
		{
			pickupCharacter = value;
		}
	}

	public void Start()
	{
		destinationY = -999f;
		Collider[] componentsInChildren = base.gameObject.GetComponentsInChildren<Collider>();
		Collider[] array = componentsInChildren;
		foreach (Collider collider in array)
		{
			collider.isTrigger = true;
		}
	}

	public void Update()
	{
		if (destinationY > -999f)
		{
			Vector3 position = base.transform.position;
			if (position.y <= destinationY)
			{
				Vector3 position2 = base.transform.position;
				position2.y = destinationY;
				base.transform.position = position2;
				base.rigidbody.useGravity = false;
			}
		}
	}

	public void setDestinationY(float newDestinationY)
	{
		destinationY = newDestinationY;
	}

	public void OnMouseRolloverEnter(object data)
	{
		MouseRollover mouseRollover = data as MouseRollover;
		if (!mouseRollover.allowUserInput)
		{
			return;
		}
		CharacterMotionController motionController = mouseRollover.charGlobals.motionController;
		if (!(motionController != null) || !(motionController.pickupStrength < weight))
		{
			CombatController combatController = mouseRollover.charGlobals.combatController;
			if (!(combatController != null) || !combatController.IsInteractRestricted)
			{
				GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Interactable);
				GUIManager.Instance.AttachMouseoverThrowableIndicator(base.gameObject);
			}
		}
	}

	public void OnMouseRolloverExit()
	{
		GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Normal);
		GUIManager.Instance.DetachMouseoverIndicator();
	}

	public bool OnMouseClick(CharacterGlobals charGlobals)
	{
		if (charGlobals.motionController.pickupStrength < weight)
		{
			return false;
		}
		if (charGlobals.combatController.IsInteractRestricted)
		{
			return false;
		}
		BehaviorManager behaviorManager = charGlobals.behaviorManager;
		if (behaviorManager == null)
		{
			CspUtils.DebugLog("Player should always have a BehaviorManager");
			return false;
		}
		BehaviorApproach behaviorApproach = behaviorManager.requestChangeBehavior(typeof(BehaviorApproach), true) as BehaviorApproach;
		if (behaviorApproach == null)
		{
			return false;
		}
		bool flag = false;
		Vector3 vector = Vector3.zero;
		Quaternion rotation = Quaternion.identity;
		Vector3 vector2 = base.transform.position - charGlobals.transform.position;
		vector2.y = 0f;
		float num = Vector3.Angle(vector2.normalized, Vector3.back);
		float num2 = 0f;
		Vector3 position = base.transform.position;
		float x = position.x;
		Vector3 position2 = charGlobals.transform.position;
		if (x > position2.x)
		{
			num = 0f - num;
		}
		while (!flag && num2 < 360f)
		{
			Vector3 vector3 = Quaternion.AngleAxis(num, Vector3.up) * Vector3.forward;
			vector = vector3 * pickupDistance + base.transform.position;
			bool flag2 = true;
			if (Physics.CheckSphere(vector + new Vector3(0f, charGlobals.characterController.height * 0.5f, 0f), charGlobals.characterController.radius, 804756969))
			{
				flag2 = false;
			}
			if (flag2 && Physics.Raycast(base.transform.position, vector3, pickupDistance, 804756969))
			{
				flag2 = false;
			}
			if (flag2)
			{
				flag = true;
				rotation = Quaternion.LookRotation(vector3);
			}
			else
			{
				num2 += 15f;
				num += ((num2 % 10f != 0f) ? (0f - num2) : num2);
			}
		}
		if (num2 >= 360f)
		{
			return false;
		}
		behaviorApproach.Initialize(vector, rotation, true, ApproachArrived, null, 0.1f, 100f, true, true, false);
		behaviorApproach.setTarget(base.gameObject);
		GUIManager.Instance.DetachAttackingIndicator();
		return true;
	}

	protected void ApproachArrived(GameObject player)
	{
		PickupCharacter = player;
		if (!pendingResponse)
		{
			pendingResponse = true;
			AppShell.Instance.ServerConnection.Game.TakeOwnership(base.gameObject, OnOwnershipChange);
		}
	}

	protected void OnOwnershipChange(GameObject go, bool bAssumedOwnership)
	{
		if (!(base.gameObject == go))
		{
			return;
		}
		pendingResponse = false;
		BehaviorManager behaviorManager = PickupCharacter.GetComponent(typeof(BehaviorManager)) as BehaviorManager;
		if (behaviorManager == null)
		{
			CspUtils.DebugLog("Player should always have a BehaviorManager");
		}
		else if (bAssumedOwnership)
		{
			BehaviorPickupThrowable behaviorPickupThrowable = behaviorManager.requestChangeBehavior(typeof(BehaviorPickupThrowable), false) as BehaviorPickupThrowable;
			if (behaviorPickupThrowable != null && (PickupCharacter == null || !behaviorPickupThrowable.Initialize(this)))
			{
				behaviorManager.endBehavior();
			}
		}
		else
		{
			behaviorManager.endBehavior();
		}
	}
}
