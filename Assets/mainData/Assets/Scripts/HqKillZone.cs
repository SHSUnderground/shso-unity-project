using System;
using UnityEngine;

public class HqKillZone : HqTrigger
{
	public EffectSequence effectOnCollision;

	public override bool IsOn
	{
		get
		{
			return true;
		}
	}

	private void OnCollisionStay(Collision otherCollision)
	{
		HqTrigger component = Utils.GetComponent<HqTrigger>(otherCollision.collider, Utils.SearchChildren);
		if (!(component != null) && IsOn)
		{
			RespawnCollider(otherCollision.collider);
		}
	}

	private void OnTriggerStay(Collider otherCollider)
	{
		HqTrigger component = Utils.GetComponent<HqTrigger>(otherCollider, Utils.SearchChildren);
		if (!(component != null) && IsOn)
		{
			RespawnCollider(otherCollider);
		}
	}

	protected void RespawnCollider(Collider otherCollider)
	{
		Type state = HqController2.Instance.State;
		if (state != typeof(HqController2.HqControllerFlinga) || (otherCollider.gameObject.layer != 13 && otherCollider.gameObject.layer != 12) || Utils.GetComponent<EntryPoint>(otherCollider) != null || Utils.GetComponent<DockPoint>(otherCollider) != null)
		{
			return;
		}
		HqRoom2 hqRoom = FindRoom(otherCollider.gameObject);
		if (hqRoom == null)
		{
			CspUtils.DebugLog("Could not find the parent room for object that hit killzone " + otherCollider.gameObject.name);
			return;
		}
		AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(otherCollider, Utils.SearchChildren);
		if (component != null)
		{
			if (component.Mode == AIControllerHQ.AiMode.Active)
			{
				PlayCollisionSequence();
				hqRoom.RespawnAI(component);
			}
			return;
		}
		HqObject2 component2 = Utils.GetComponent<HqObject2>(otherCollider.gameObject, Utils.SearchParents);
		if (component2 != null && component2.AIController != null)
		{
			PlayCollisionSequence();
			hqRoom.RespawnAI(component2.AIController);
			return;
		}
		Rigidbody rigidbody = HqController2.Instance.GetRigidbody(otherCollider.gameObject);
		if (rigidbody == null || rigidbody.gameObject == null)
		{
			return;
		}
		if (component2 == null)
		{
			component2 = Utils.GetComponent<HqObject2>(rigidbody);
			if (component2 == null)
			{
				return;
			}
		}
		HqItem component3 = Utils.GetComponent<HqItem>(rigidbody);
		if (!(component3 != null) || !component3.IsInAIControl)
		{
			hqRoom.RespawnObject(component2);
			PlayCollisionSequence();
			if (hqRoom != HqController2.Instance.ActiveRoom)
			{
				CspUtils.DebugLog("Trying to respawn an object that hit killzone in another room: " + otherCollider.gameObject.name);
				Utils.ActivateTree(component3.gameObject, false);
			}
		}
	}

	protected HqRoom2 FindRoom(GameObject obj)
	{
		HqRoom2 hqRoom = null;
		while (obj != null)
		{
			hqRoom = Utils.GetComponent<HqRoom2>(obj);
			if (hqRoom != null)
			{
				break;
			}
			AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(obj);
			if (component != null)
			{
				hqRoom = component.CurrentRoom;
				break;
			}
			obj = ((!(obj.transform.parent != null)) ? null : obj.transform.parent.gameObject);
		}
		return hqRoom;
	}

	protected void PlayCollisionSequence()
	{
		if (effectOnCollision != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(effectOnCollision.gameObject) as GameObject;
			EffectSequence component = gameObject.GetComponent<EffectSequence>();
			component.Initialize(base.gameObject, DestroyEffectSequence, null);
			component.StartSequence();
		}
	}

	private void DestroyEffectSequence(EffectSequence seq)
	{
		UnityEngine.Object.Destroy(seq.gameObject);
	}
}
