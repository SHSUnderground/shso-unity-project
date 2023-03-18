using System.Collections.Generic;
using UnityEngine;

public class ChallengeManagerFanfareManager
{
	public enum FanfareStateEnum
	{
		Inactive,
		Display
	}

	private FanfareStateEnum currentState;

	private string inactiveReason;

	private Queue<FanfareInstance> fanfareQueue;

	protected ChallengeManager manager;

	protected float fanfareInitialDelay = 1f;

	private float fakeDisplayTimer;

	public FanfareStateEnum CurrentState
	{
		get
		{
			return currentState;
		}
	}

	public string InactiveReason
	{
		get
		{
			return inactiveReason;
		}
	}

	public Queue<FanfareInstance> FanfareQueue
	{
		get
		{
			return fanfareQueue;
		}
	}

	public ChallengeManagerFanfareManager(ChallengeManager manager)
	{
		fanfareQueue = new Queue<FanfareInstance>();
		this.manager = manager;
	}

	public void OnChallengeCompletionVerified(int challengeServerId, int nextChallengeServerId)
	{
		if (ChallengeManagerServerStub.CurrentMode == ChallengeManagerServerStub.ServerMode.ClientSimulation)
		{
			ChallengeInfo value;
			manager.ChallengeDictionary.TryGetValue(challengeServerId, out value);
			if (value == null)
			{
				CspUtils.DebugLog("Can't add fanfare. Unknown challengeServerID : " + challengeServerId);
				return;
			}
			FanfareInstance fanfareInstance = new FanfareInstance();
			fanfareInstance.challengeCompleted = value;
			fanfareInstance.nextChallengeServerId = nextChallengeServerId;
			fanfareInstance.timeAdded = Time.time;
			fanfareQueue.Enqueue(fanfareInstance);
		}
	}

	public void Update()
	{
		if (fanfareQueue.Count == 0)
		{
			inactiveReason = "No entries in queue.";
		}
		else if (currentState == FanfareStateEnum.Display)
		{
			if (Time.time - fakeDisplayTimer > 5f)
			{
				FanfareInstance fanfareInstance = fanfareQueue.Dequeue();
				if (fanfareInstance.nextChallengeServerId != 0)
				{
					manager.SetActiveChallenge(fanfareInstance.nextChallengeServerId);
				}
				currentState = FanfareStateEnum.Inactive;
			}
		}
		else if (fanfareQueue.Peek().timeAdded + fanfareInitialDelay < Time.time)
		{
			if (fanfareCapableCheck())
			{
				currentState = FanfareStateEnum.Display;
				fakeDisplayTimer = Time.time;
				inactiveReason = string.Empty;
			}
		}
		else
		{
			inactiveReason = "waiting for standard delay";
		}
	}

	private bool fanfareCapableCheck()
	{
		if (!(GameController.GetController() is SocialSpaceController))
		{
			inactiveReason = "Not in Social Space";
			return false;
		}
		SocialSpaceController socialSpaceController = GameController.GetController() as SocialSpaceController;
		if (socialSpaceController.LocalPlayer == null)
		{
			inactiveReason = "Player not spawned";
			return false;
		}
		CharacterController component = socialSpaceController.LocalPlayer.GetComponent<CharacterController>();
		if (component == null || !component.isGrounded)
		{
			inactiveReason = "Not on ground";
			return false;
		}
		BehaviorManager component2 = socialSpaceController.LocalPlayer.GetComponent<BehaviorManager>();
		if (!(component2.getBehavior() is BehaviorMovement) || !component2.currentBehaviorInterruptible(typeof(BehaviorMovement)))
		{
			inactiveReason = "Not in interruptible behaviour.";
			return false;
		}
		return true;
	}
}
