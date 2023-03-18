using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTestRecoil : CharacterTestBase
{
	public bool Small = true;

	public bool SmallRear = true;

	public bool Large = true;

	public bool Knockdown = true;

	public bool Launch = true;

	public bool Dance = true;

	public bool Stun = true;

	public float timeBetweenRecoils = 2f;

	protected bool activated;

	protected float nextRecoilTime;

	protected List<CombatController.ImpactResultData> impactList;

	protected List<Vector3> impactPositionList;

	public void Start()
	{
		impactList = new List<CombatController.ImpactResultData>();
		impactPositionList = new List<Vector3>();
	}

	public void Update()
	{
		if (!activated)
		{
			return;
		}
		if (nextRecoilTime == 0f)
		{
			bool flag = true;
			foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
			{
				if (!(character.behaviorManager.getBehavior() is BehaviorMovement))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				nextRecoilTime = Time.time + timeBetweenRecoils;
			}
		}
		else if (Time.time > nextRecoilTime)
		{
			nextRecoilTime = 0f;
			playNextRecoil();
		}
	}

	protected void playNextRecoil()
	{
		if (impactList.Count == 0)
		{
			CharacterTest.Instance.TestDone();
			return;
		}
		CombatController.ImpactResultData impactResultData = impactList[0];
		impactList.RemoveAt(0);
		Vector3 a = impactPositionList[0];
		impactPositionList.RemoveAt(0);
		Type newBehaviorType = CombatController.AttackData.RecoilBehaviorType[(int)impactResultData.recoil];
		foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
		{
			BehaviorRecoil behaviorRecoil = character.behaviorManager.requestChangeBehavior(newBehaviorType, false) as BehaviorRecoil;
			if (behaviorRecoil != null)
			{
				behaviorRecoil.Initialize(character.gameObject, a + character.transform.position, impactResultData);
			}
		}
	}

	public override void Activate()
	{
		nextRecoilTime = 0f;
		if (Small)
		{
			CombatController.ImpactResultData impactResultData = new CombatController.ImpactResultData();
			impactResultData.recoil = CombatController.AttackData.RecoilType.Small;
			impactList.Add(impactResultData);
			impactPositionList.Add(Vector3.forward);
		}
		if (SmallRear)
		{
			CombatController.ImpactResultData impactResultData = new CombatController.ImpactResultData();
			impactResultData.recoil = CombatController.AttackData.RecoilType.Small;
			impactList.Add(impactResultData);
			impactPositionList.Add(Vector3.back);
		}
		if (Large)
		{
			CombatController.ImpactResultData impactResultData = new CombatController.ImpactResultData();
			impactResultData.recoil = CombatController.AttackData.RecoilType.Large;
			impactResultData.pushbackVelocity = new ModifierData(3f);
			impactResultData.pushbackDuration = new ModifierData(0.5f);
			impactList.Add(impactResultData);
			impactPositionList.Add(Vector3.forward);
		}
		if (Knockdown)
		{
			CombatController.ImpactResultData impactResultData = new CombatController.ImpactResultData();
			impactResultData.recoil = CombatController.AttackData.RecoilType.Knockdown;
			impactResultData.knockdownDuration = new ModifierData(1f);
			impactResultData.pushbackVelocity = new ModifierData(3f);
			impactResultData.pushbackDuration = new ModifierData(0.5f);
			impactList.Add(impactResultData);
			impactPositionList.Add(Vector3.forward);
		}
		if (Launch)
		{
			CombatController.ImpactResultData impactResultData = new CombatController.ImpactResultData();
			impactResultData.recoil = CombatController.AttackData.RecoilType.Launch;
			impactResultData.knockdownDuration = new ModifierData(1f);
			impactResultData.launchVelocity = new ModifierData(-30f);
			impactResultData.pushbackVelocity = new ModifierData(3f);
			impactResultData.pushbackDuration = new ModifierData(0.5f);
			impactList.Add(impactResultData);
			impactPositionList.Add(Vector3.forward);
		}
		if (Dance)
		{
			CombatController.ImpactResultData impactResultData = new CombatController.ImpactResultData();
			impactResultData.recoil = CombatController.AttackData.RecoilType.Dance;
			impactResultData.knockdownDuration = new ModifierData(5f);
			impactList.Add(impactResultData);
			impactPositionList.Add(Vector3.zero);
		}
		if (Stun)
		{
			CombatController.ImpactResultData impactResultData = new CombatController.ImpactResultData();
			impactResultData.recoil = CombatController.AttackData.RecoilType.Stun;
			impactResultData.knockdownDuration = new ModifierData(5f);
			impactList.Add(impactResultData);
			impactPositionList.Add(Vector3.zero);
		}
		activated = true;
	}
}
