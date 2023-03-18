using System.Collections.Generic;
using UnityEngine;

public class PrioritizedIOCList : InteractiveObjectController
{
	public List<InteractiveObjectController> controllers;

	private bool inUse;

	private void Awake()
	{
		foreach (InteractiveObjectController controller in controllers)
		{
			controller.gameObject.active = false;
		}
	}

	public override void Initialize(InteractiveObject owner, GameObject model)
	{
		base.Initialize(owner, model);
		foreach (InteractiveObjectController controller in controllers)
		{
			controller.gameObject.active = true;
			controller.Initialize(owner, model);
		}
	}

	public override InteractiveObject.StateIdx GetStateForPlayer(GameObject player)
	{
		InteractiveObject.StateIdx stateIdx = InteractiveObject.StateIdx.Hidden;
		foreach (InteractiveObjectController controller in controllers)
		{
			InteractiveObject.StateIdx stateForPlayer = controller.GetStateForPlayer(player);
			if (stateForPlayer > stateIdx)
			{
				stateIdx = stateForPlayer;
			}
		}
		return stateIdx;
	}

	public override bool CanPlayerUse(GameObject player)
	{
		return !inUse && GetFirstUsable(player) != null;
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		InteractiveObjectController firstUsable = GetFirstUsable(player);
		if (firstUsable.StartWithPlayer(player, delegate(GameObject p, CompletionStateEnum completionState)
		{
			inUse = false;
			if (onDone != null)
			{
				onDone(p, completionState);
			}
		}))
		{
			inUse = true;
			return true;
		}
		return false;
	}

	private InteractiveObjectController GetFirstUsable(GameObject player)
	{
		foreach (InteractiveObjectController controller in controllers)
		{
			if (controller.CanPlayerUse(player))
			{
				return controller;
			}
		}
		return null;
	}
}
