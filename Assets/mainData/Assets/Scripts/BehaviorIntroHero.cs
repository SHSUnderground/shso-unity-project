using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorIntroHero : BehaviorWander
{
	private float delayTime = 1f;

	private bool playedTransportFX;

	public override void Initialize(OnWanderEnd wanderEnd, PathFinder pf, PathFinder.DoesObstacleBlock obstacleCheck)
	{
		base.Initialize(wanderEnd, pf, obstacleCheck);
		HqController2.Instance.Input.SpecialIntroSetHeroCamera(owningObject.GetComponent<AIControllerHQ>());
		playedTransportFX = false;
		AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(owningObject, Utils.SearchChildren);
		if (component != null)
		{
			component.StopRendering();
		}
	}

	public override void behaviorEnd()
	{
		HqController2.Instance.Input.SpecialIntroClearHeroCamera(HqController2.Instance.initial_pullback_time);
		base.behaviorEnd();
	}

	public override void behaviorCancel()
	{
		HqController2.Instance.Input.SpecialIntroClearHeroCamera(HqController2.Instance.initial_pullback_time);
		base.behaviorCancel();
	}

	public override void behaviorUpdate()
	{
		if (Paused)
		{
			return;
		}
		if (elapsedTime < delayTime)
		{
			elapsedTime += Time.deltaTime;
			return;
		}
		if (!playedTransportFX)
		{
			HqController2.Instance.ActiveRoom.PlayTransporterEffect(owningObject.transform.position);
			playedTransportFX = true;
			AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(owningObject, Utils.SearchChildren);
			if (component != null)
			{
				component.StartRendering();
			}
		}
		base.behaviorUpdate();
	}

	public override bool useMotionController()
	{
		if (elapsedTime < delayTime)
		{
			return false;
		}
		return base.useMotionController();
	}

	protected override void ChooseDestination()
	{
		if (pathfinder != null && HqController2.Instance.ActiveRoom.IntroMark != Vector3.zero)
		{
			List<PathNode> list = pathfinder.FindPath(owningObject.transform.position, HqController2.Instance.ActiveRoom.IntroMark, true, obstacleCheckCallback);
			if (list != null)
			{
				currentPath.Clear();
				foreach (PathNode item2 in list)
				{
					Vector3 position = item2.transform.position;
					float x = position.x;
					Vector3 position2 = item2.transform.position;
					float y = position2.y + 0.35f;
					Vector3 position3 = item2.transform.position;
					Vector3 item = new Vector3(x, y, position3.z);
					currentPath.Add(item);
				}
				if (currentPath.Count > 0)
				{
					currentPathNodeIndex = 0;
					charGlobals.motionController.setDestination(currentPath[currentPathNodeIndex]);
					wanderState = CurrentState.Wandering;
					previousPosition = owningObject.transform.position;
					return;
				}
			}
		}
		base.ChooseDestination();
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		if (newBehaviorType == typeof(BehaviorInteract))
		{
			return false;
		}
		return base.allowInterrupt(newBehaviorType);
	}
}
