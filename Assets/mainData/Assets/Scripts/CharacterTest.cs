using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected class JumpTestState : IShsState
	{
		public void Enter(Type previousState)
		{
			Instance.ActivateTest(typeof(CharacterTestJump));
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	protected class EmoteTestState : IShsState
	{
		public void Enter(Type previousState)
		{
			Instance.ActivateTest(typeof(CharacterTestEmote));
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	protected class RecoilTestState : IShsState
	{
		public void Enter(Type previousState)
		{
			Instance.ActivateTest(typeof(CharacterTestRecoil));
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	protected class AttackTestState : IShsState
	{
		public void Enter(Type previousState)
		{
			Instance.ActivateTest(typeof(CharacterTestAttack));
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	protected class SplineTestState : IShsState
	{
		public void Enter(Type previousState)
		{
			Instance.ActivateTest(typeof(CharacterTestSpline));
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	protected class ThrowTestState : IShsState
	{
		public void Enter(Type previousState)
		{
			Instance.ActivateTest(typeof(CharacterTestThrow));
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	[HideInInspector]
	public string characterName = string.Empty;

	public bool isPlayer = true;

	public float timeBetweenTests = 3f;

	public string _________ = string.Empty;

	public bool jumps = true;

	public bool emotes = true;

	public bool attacks = true;

	public bool recoils = true;

	public bool pickupAndThrow = true;

	public bool spline = true;

	public string __________ = string.Empty;

	public bool go;

	protected ShsFSM testFSM;

	protected CharacterTestBase currentTest;

	protected float advanceStateTime;

	protected CharacterTestSpawn[] spawners;

	protected static CharacterTest instance;

	protected Quaternion cameraRotation;

	public static CharacterTest Instance
	{
		get
		{
			return instance;
		}
	}

	public void Start()
	{
		instance = this;
		cameraRotation = Camera.main.transform.rotation;
	}

	public void Update()
	{
		if (Camera.main.transform.parent != null)
		{
			Camera.main.transform.LookAt(Camera.main.transform.parent.position + Vector3.up);
		}
		Pickup[] array = UnityEngine.Object.FindObjectsOfType(typeof(Pickup)) as Pickup[];
		Pickup[] array2 = array;
		foreach (Pickup pickup in array2)
		{
			UnityEngine.Object.Destroy(pickup.gameObject);
		}
		if (go)
		{
			go = false;
			Camera.main.transform.parent = null;
			spawners = (UnityEngine.Object.FindObjectsOfType(typeof(CharacterTestSpawn)) as CharacterTestSpawn[]);
			CharacterTestSpawn[] array3 = spawners;
			foreach (CharacterTestSpawn characterTestSpawn in array3)
			{
				characterTestSpawn.DespawnSpawnedCharacters();
				characterTestSpawn.CharacterName = characterName;
				characterTestSpawn.IsPlayer = isPlayer;
				characterTestSpawn.IsAI = !isPlayer;
				characterTestSpawn.Triggered(null);
			}
			testFSM = new ShsFSM();
			if (spline)
			{
				testFSM.PushState(new SplineTestState());
			}
			if (pickupAndThrow)
			{
				testFSM.PushState(new ThrowTestState());
			}
			if (recoils)
			{
				testFSM.PushState(new RecoilTestState());
			}
			if (attacks)
			{
				testFSM.PushState(new AttackTestState());
			}
			if (emotes)
			{
				testFSM.PushState(new EmoteTestState());
			}
			if (jumps)
			{
				testFSM.PushState(new JumpTestState());
			}
			advanceStateTime = Time.time + 5f;
		}
		if (advanceStateTime != 0f && Time.time > advanceStateTime)
		{
			advanceStateTime = 0f;
			testFSM.PopState();
			if (testFSM.GetCurrentState() == null)
			{
				testFSM = null;
			}
		}
	}

	public void TestDone()
	{
		currentTest = null;
		advanceStateTime = Time.time + timeBetweenTests;
	}

	public List<CharacterGlobals> GetCharacters()
	{
		List<CharacterGlobals> list = new List<CharacterGlobals>();
		CharacterTestSpawn[] array = spawners;
		foreach (CharacterTestSpawn characterTestSpawn in array)
		{
			CharacterGlobals spawnedCharGlobal = characterTestSpawn.GetSpawnedCharGlobal();
			if (spawnedCharGlobal != null)
			{
				list.Add(spawnedCharGlobal);
			}
		}
		return list;
	}

	public void ActivateTest(Type testType)
	{
		CharacterTestBase characterTestBase = Instance.GetComponent(testType) as CharacterTestBase;
		if (characterTestBase == null)
		{
			CspUtils.DebugLog("Component of type " + testType.ToString() + " missing, test will not run");
			return;
		}
		currentTest = characterTestBase;
		characterTestBase.Activate();
	}

	public CharacterTestBase GetCurrentTest()
	{
		return currentTest;
	}
}
