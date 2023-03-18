using System;
using UnityEngine;

public class CharacterTestJump : CharacterTestBase
{
	protected class JumpInPlaceState : IShsState
	{
		public void Enter(Type previousState)
		{
			foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
			{
				character.motionController.jumpPressed();
			}
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	protected class RunningJumpState : IShsState
	{
		public void Enter(Type previousState)
		{
			foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
			{
				character.motionController.setDestination(character.motionController.getDestination() + new Vector3(0f, 0f, 10f));
				character.motionController.jumpPressed();
			}
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
			foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
			{
				character.motionController.setDestination(character.motionController.getDestination() + new Vector3(0f, 0f, -10f));
			}
		}
	}

	protected class LoopingFallState : IShsState
	{
		public void Enter(Type previousState)
		{
			foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
			{
				character.motionController.teleportTo(character.motionController.getDestination() + new Vector3(0f, 50f, 0f));
				character.motionController.setIsOnGround(true);
			}
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	public bool jumpInPlace = true;

	public bool runningJump = true;

	public bool loopingFall = true;

	public float timeBetweenJumps = 2f;

	protected float advanceStateTime;

	protected ShsFSM testFSM;

	public void Update()
	{
		if (advanceStateTime == 0f && testFSM != null && testFSM.GetCurrentState() != null)
		{
			bool flag = true;
			foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
			{
				if (!character.motionController.IsOnGround())
				{
					flag = false;
				}
			}
			if (flag)
			{
				advanceStateTime = Time.time + timeBetweenJumps;
			}
		}
		if (advanceStateTime > 0f && Time.time >= advanceStateTime)
		{
			advanceStateTime = 0f;
			AdvanceState();
		}
	}

	public override void Activate()
	{
		advanceStateTime = 0f;
		testFSM = new ShsFSM();
		if (loopingFall)
		{
			testFSM.PushState(new LoopingFallState());
		}
		if (runningJump)
		{
			testFSM.PushState(new RunningJumpState());
		}
		if (jumpInPlace)
		{
			testFSM.PushState(new JumpInPlaceState());
		}
		AdvanceState();
	}

	protected void AdvanceState()
	{
		testFSM.PopState();
		if (testFSM.GetCurrentState() == null)
		{
			testFSM = null;
			CharacterTest.Instance.TestDone();
		}
	}
}
