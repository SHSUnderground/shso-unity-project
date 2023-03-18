using System;
using UnityEngine;

public class CharacterTestThrow : CharacterTestBase
{
	protected class PickupState : IShsState
	{
		public void Enter(Type previousState)
		{
			foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
			{
				ThrowableGround throwableGround = character.GetComponentInChildren(typeof(ThrowableGround)) as ThrowableGround;
				if (throwableGround == null)
				{
					CspUtils.DebugLog("No Ground Component found on chosen prefab");
					break;
				}
				throwableGround.transform.parent = null;
				throwableGround.OnMouseClick(character);
			}
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	protected class MoveState : IShsState
	{
		public void Enter(Type previousState)
		{
			foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
			{
				character.motionController.setDestination(character.transform.position + character.transform.forward * 7f);
			}
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	protected class ThrowState : IShsState
	{
		public void Enter(Type previousState)
		{
			foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
			{
				character.combatController.beginAttack(null, false);
			}
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	public GameObject groundObjectPrefab;

	public Vector3 groundObjectOffset;

	public float timeBetweenStages = 3f;

	protected float advanceStateTime;

	protected ShsFSM testFSM;

	public void Update()
	{
		if (advanceStateTime == 0f && testFSM != null && testFSM.GetCurrentState() != null)
		{
			bool flag = true;
			foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
			{
				if (!(character.behaviorManager.getBehavior() is BehaviorMovement))
				{
					flag = false;
				}
			}
			if (flag)
			{
				advanceStateTime = Time.time + timeBetweenStages;
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
		testFSM = new ShsFSM();
		testFSM.PushState(new ThrowState());
		testFSM.PushState(new MoveState());
		testFSM.PushState(new PickupState());
		foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(groundObjectPrefab, character.transform.position + groundObjectOffset, groundObjectPrefab.transform.rotation) as GameObject;
			gameObject.transform.parent = character.transform;
			character.networkComponent.AnnounceObjectSpawn(gameObject, string.Empty, string.Empty);
		}
		advanceStateTime = Time.time + 1f;
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
