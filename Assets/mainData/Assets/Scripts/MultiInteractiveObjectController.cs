using UnityEngine;

[AddComponentMenu("Interactive Object/Multiple Interactive Object Controller")]
public class MultiInteractiveObjectController : InteractiveObjectController
{
	public InteractiveObjectController[] Controllers;

	public override void Initialize(InteractiveObject owner, GameObject model)
	{
		base.Initialize(owner, model);
	}

	public override InteractiveObject.StateIdx GetStateForPlayer(GameObject player)
	{
		if (player == null)
		{
			return InteractiveObject.StateIdx.Hidden;
		}
		CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(player);
		if (component == null)
		{
			return InteractiveObject.StateIdx.Hidden;
		}
		for (int i = 0; i < Controllers.Length; i++)
		{
			if (Controllers[i].GetStateForPlayer(player) == InteractiveObject.StateIdx.Enable)
			{
				return InteractiveObject.StateIdx.Enable;
			}
		}
		return InteractiveObject.StateIdx.Hidden;
	}

	public override void AttemptedInvalidUse(GameObject player)
	{
		if (!(player != null))
		{
			return;
		}
		int num = 0;
		while (true)
		{
			if (num < Controllers.Length)
			{
				if (!Controllers[num].CanPlayerUse(player))
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		Controllers[num].AttemptedInvalidUse(player);
	}

	public override bool CanPlayerUse(GameObject player)
	{
		for (int i = 0; i < Controllers.Length; i++)
		{
			if (Controllers[i].CanPlayerUse(player))
			{
				return true;
			}
		}
		return false;
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		for (int i = 0; i < Controllers.Length; i++)
		{
			if (Controllers[i].CanPlayerUse(player))
			{
				return Controllers[i].StartWithPlayer(player, onDone);
			}
		}
		return false;
	}
}
