using System;
using System.Collections;
using UnityEngine;

public class BehaviorHotspotWait : BehaviorBase
{
	public delegate void WaitFinished(GameObject obj);

	public delegate void WaitCancelled(GameObject obj);

	protected MultiHotspotCoordinator coordinator;

	protected float waitOffset;

	protected WaitFinished onWaitFinished;

	protected WaitCancelled onWaitCancelled;

	private bool receivedFinishTime;

	private bool isLockedIn;

	public void Initialize(MultiHotspotCoordinator coordinator, float waitOffset, WaitFinished onWaitFinished, WaitCancelled onWaitCancelled)
	{
		this.coordinator = coordinator;
		this.waitOffset = waitOffset;
		this.onWaitFinished = (WaitFinished)Delegate.Combine(this.onWaitFinished, onWaitFinished);
		this.onWaitCancelled = (WaitCancelled)Delegate.Combine(this.onWaitCancelled, onWaitCancelled);
		isLockedIn = false;
		receivedFinishTime = false;
		charGlobals.StartCoroutine(WaitForCoordinator());
		MultiHotspotCoordinatorUser.Attach(owningObject, coordinator);
	}

	protected IEnumerator WaitForCoordinator()
	{
		if (!Utils.IsLocalPlayer(charGlobals))
		{
			yield return charGlobals.StartCoroutine(WaitForNetFinishTime(coordinator.TotalCountDownTime));
			if (!receivedFinishTime)
			{
				Cancelled();
			}
		}
		else if (coordinator.LaunchTime < Time.time)
		{
			coordinator.LaunchTime = Time.time + coordinator.TotalCountDownTime;
		}
		yield return charGlobals.StartCoroutine(WaitForFinishTime(coordinator.LaunchTime - waitOffset));
		Finished();
	}

	protected IEnumerator WaitForNetFinishTime(float timeout)
	{
		while (owningObject != null && !receivedFinishTime && timeout > 0f)
		{
			if (coordinator.LaunchTime > Time.time)
			{
				receivedFinishTime = true;
				continue;
			}
			timeout -= Time.deltaTime;
			yield return 0;
		}
	}

	protected IEnumerator WaitForFinishTime(float finishTime)
	{
		float currentTime = Time.time;
		if (finishTime > currentTime)
		{
			yield return new WaitForSeconds(finishTime - currentTime);
		}
	}

	public override void destinationChanged()
	{
		if (!isLockedIn)
		{
			if (charGlobals.behaviorManager.getBehavior() == this)
			{
				charGlobals.behaviorManager.endBehavior();
			}
			Cancelled();
		}
	}

	protected void Finished()
	{
		if (onWaitFinished != null)
		{
			onWaitFinished(owningObject);
		}
	}

	protected void Cancelled()
	{
		if (charGlobals.networkComponent != null)
		{
			charGlobals.networkComponent.QueueNetAction(new NetActionCancel());
		}
		if (onWaitCancelled != null)
		{
			onWaitCancelled(owningObject);
		}
		MultiHotspotCoordinatorUser.Detach(owningObject);
	}

	public override void behaviorEnd()
	{
		charGlobals.StopAllCoroutines();
		base.behaviorEnd();
	}

	public override bool useMotionController()
	{
		return coordinator.DisplayTime > coordinator.TimeLockedIn;
	}

	public override bool allowUserInput()
	{
		return coordinator.DisplayTime > coordinator.TimeLockedIn;
	}
}
