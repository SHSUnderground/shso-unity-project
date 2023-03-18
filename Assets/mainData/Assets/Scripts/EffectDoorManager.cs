using System;
using UnityEngine;

public class EffectDoorManager : DoorManager
{
	public EffectSequence effectSequence;

	public bool letComponentPlaySequence;

	public string componentToAdd = string.Empty;

	public float componentDelay = 0.25f;

	public float componentDuration = 30f;

	public EffectSequence componentRevertSequence;

	public override void ExitWithPlayer(GameObject player, OnDone onDone, bool resetCamera)
	{
		if (effectSequence != null)
		{
			AttachSequence(player);
		}
		if (!string.IsNullOrEmpty(componentToAdd))
		{
			AttachComponent(player);
		}
		base.ExitWithPlayer(player, onDone, resetCamera);
	}

	private void AttachSequence(GameObject player)
	{
		if (!letComponentPlaySequence && !IsSequenceAttached(player))
		{
			EffectSequence.PlayOneShot(effectSequence, player);
		}
	}

	private bool IsSequenceAttached(GameObject player)
	{
		foreach (Transform item in player.transform)
		{
			if (Utils.GetBasePrefabName(item.gameObject) == effectSequence.name)
			{
				return true;
			}
		}
		return false;
	}

	private void AttachComponent(GameObject player)
	{
		Type type = Type.GetType(componentToAdd);
		if (type != null && player.GetComponent(type) == null)
		{
			Component component = player.AddComponent(type);
			IComponentTimeInit componentTimeInit = component as IComponentTimeInit;
			if (componentTimeInit != null)
			{
				componentTimeInit.SetDelay(componentDelay);
				componentTimeInit.SetDuration(componentDuration);
			}
			if (letComponentPlaySequence)
			{
				component.SendMessage("SetApplySequence", effectSequence, SendMessageOptions.DontRequireReceiver);
			}
			component.SendMessage("SetRevertSequence", componentRevertSequence, SendMessageOptions.DontRequireReceiver);
			component.SendMessage("SetSequenceOwner", player, SendMessageOptions.DontRequireReceiver);
		}
	}
}
