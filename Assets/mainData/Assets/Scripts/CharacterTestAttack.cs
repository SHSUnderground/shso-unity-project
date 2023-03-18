using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTestAttack : CharacterTestBase
{
	protected class StandardAttackState : IShsState
	{
		public void Enter(Type previousState)
		{
			CharacterTestAttack characterTestAttack = CharacterTest.Instance.GetCurrentTest() as CharacterTestAttack;
			foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
			{
				GameObject targetObject = characterTestAttack.targetObjects[character];
				character.combatController.pursueTarget(targetObject, false);
			}
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	protected class SecondaryAttackState : IShsState
	{
		public int secondaryAttack = 1;

		public virtual void Enter(Type previousState)
		{
			CharacterTestAttack characterTestAttack = CharacterTest.Instance.GetCurrentTest() as CharacterTestAttack;
			foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
			{
				character.combatController.SetSecondaryAttack(secondaryAttack);
				PlayerCombatController playerCombatController = character.combatController as PlayerCombatController;
				if (playerCombatController != null)
				{
					playerCombatController.setPower(100f);
				}
				GameObject gameObject = characterTestAttack.targetObjects[character];
				gameObject.transform.parent = null;
				gameObject.transform.position = character.transform.position + character.transform.forward * characterTestAttack.targetDistance;
				character.combatController.pursueTarget(gameObject, true);
			}
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	protected class SecondaryAttackState1 : SecondaryAttackState
	{
		public override void Enter(Type previousState)
		{
			secondaryAttack = 1;
			base.Enter(previousState);
		}
	}

	protected class SecondaryAttackState2 : SecondaryAttackState
	{
		public override void Enter(Type previousState)
		{
			secondaryAttack = 2;
			base.Enter(previousState);
		}
	}

	protected class SecondaryAttackState3 : SecondaryAttackState
	{
		public override void Enter(Type previousState)
		{
			secondaryAttack = 3;
			base.Enter(previousState);
		}
	}

	protected class SuperAttackState : IShsState
	{
		public void Enter(Type previousState)
		{
			foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
			{
				PlayerCombatController playerCombatController = character.combatController as PlayerCombatController;
				if (playerCombatController != null)
				{
					playerCombatController.setPower(100f);
					playerCombatController.UsePower();
				}
			}
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	public GameObject targetObjectPrefab;

	public float targetDistance;

	public bool LeftClickAttacks = true;

	public bool RightClickAttacks = true;

	public bool SuperAttack = true;

	public float timeBetweenStates = 3f;

	protected float advanceStateTime;

	public Dictionary<CharacterGlobals, GameObject> targetObjects;

	protected ShsFSM testFSM;

	public void Start()
	{
		targetObjects = new Dictionary<CharacterGlobals, GameObject>();
	}

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
				advanceStateTime = Time.time + timeBetweenStates;
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
		if (SuperAttack)
		{
			testFSM.PushState(new SuperAttackState());
		}
		if (RightClickAttacks)
		{
			testFSM.PushState(new SecondaryAttackState3());
			testFSM.PushState(new SecondaryAttackState2());
			testFSM.PushState(new SecondaryAttackState1());
		}
		if (LeftClickAttacks)
		{
			testFSM.PushState(new StandardAttackState());
		}
		foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(targetObjectPrefab, character.transform.position + character.transform.forward * targetDistance, targetObjectPrefab.transform.rotation) as GameObject;
			character.networkComponent.AnnounceObjectSpawn(gameObject, string.Empty, string.Empty);
			targetObjects.Add(character, gameObject);
			gameObject.transform.parent = character.gameObject.transform;
		}
		advanceStateTime = Time.time + 1f;
	}

	protected void AdvanceState()
	{
		testFSM.PopState();
		if (testFSM.GetCurrentState() == null)
		{
			testFSM = null;
			foreach (KeyValuePair<CharacterGlobals, GameObject> targetObject in targetObjects)
			{
				UnityEngine.Object.Destroy(targetObject.Value);
			}
			targetObjects.Clear();
			CharacterTest.Instance.TestDone();
		}
	}
}
