using System.Collections;
using UnityEngine;

public class PolymorphSpawn : CharacterSpawn
{
	public PolymorphForm[] polymorphForms;

	public bool prespawnForms = true;

	private PolymorphForm mSpawnForm;

	private ArrayList mForms;

	private bool inPrespawn;

	public override void Awake()
	{
		base.Awake();
		PolymorphForm polymorphForm = new PolymorphForm();
		polymorphForm.characterName = CharacterName;
		polymorphForm.r2Attack = R2Attack;
		polymorphForm.changeToEnemy = false;
		polymorphForm.startingEffect = startingEffect;
		int num = (polymorphForms != null) ? polymorphForms.Length : 0;
		mForms = new ArrayList(num + 5);
		mForms.Add(polymorphForm);
		if (polymorphForms != null)
		{
			mForms.AddRange(polymorphForms);
		}
	}

	public bool IsInOriginalForm(GameObject polymorph)
	{
		return GetOriginalForm().InForm(polymorph);
	}

	public string GetOriginalFormName()
	{
		return GetOriginalForm().characterName;
	}

	public PolymorphForm GetOriginalForm()
	{
		return GetForm(0);
	}

	public PolymorphForm GetRandomForm()
	{
		int formCount = GetFormCount();
		return (formCount <= 1) ? null : GetForm(Random.Range(1, formCount));
	}

	public PolymorphForm GetForm(string characterName)
	{
		if (characterName == null || characterName == string.Empty)
		{
			return null;
		}
		foreach (PolymorphForm mForm in mForms)
		{
			if (characterName == mForm.characterName)
			{
				return mForm;
			}
		}
		return null;
	}

	public PolymorphForm GetForm(int index)
	{
		if (index < 0 || index >= mForms.Count)
		{
			return null;
		}
		return mForms[index] as PolymorphForm;
	}

	public int GetFormCount()
	{
		return mForms.Count;
	}

	public void AddForm(string characterName, string startingEffect, CombatController.Faction faction)
	{
		if (GetForm(characterName) == null)
		{
			PolymorphForm polymorphForm = new PolymorphForm();
			polymorphForm.Initialize(characterName, 1, faction == CombatController.Faction.Enemy, startingEffect);
			mForms.Add(polymorphForm);
		}
	}

	public override void SetCharacterName(string newName)
	{
		base.SetCharacterName(newName);
		SetForm(GetForm(newName));
	}

	public override void Prespawn()
	{
		if (!prespawnForms)
		{
			base.Prespawn();
			return;
		}
		ICharacterCache characterCache = GameController.GetController().CharacterCache;
		if (characterCache == null)
		{
			return;
		}
		inPrespawn = true;
		int num = 0;
		bool flag = true;
		while (flag && num < GetFormCount())
		{
			PolymorphForm form = GetForm(num);
			if (form == null)
			{
				num++;
				continue;
			}
			SetCharacterName(form.characterName);
			if (characterCache.IsCharacterCached(SpawnName))
			{
				num++;
				continue;
			}
			flag = false;
			base.Prespawn();
		}
		inPrespawn = !flag;
		SetCharacterName(GetOriginalForm().characterName);
	}

	public void SetForm(PolymorphForm form)
	{
		mSpawnForm = form;
		if (mSpawnForm != null)
		{
			SpawnName = ((!mSpawnForm.changeToEnemy) ? CharacterName : ("evil_" + CharacterName));
			if (!inPrespawn)
			{
				forceAICombat = mSpawnForm.changeToEnemy;
				startingEffect = mSpawnForm.startingEffect;
			}
		}
		else
		{
			SpawnName = CharacterName;
			if (!inPrespawn)
			{
				forceAICombat = false;
				startingEffect = string.Empty;
			}
		}
	}

	protected override void PrespawnComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		if (exit == TransactionMonitor.ExitCondition.Success)
		{
			CharacterSpawnData characterSpawnData = userData as CharacterSpawnData;
			if (characterSpawnData != null)
			{
				SetCharacterName(characterSpawnData.ModelName);
			}
			else
			{
				CspUtils.DebugLog("CharacterSpawnData not found for PolymorphSpawn PrespawnComplete");
			}
			base.PrespawnComplete(exit, error, userData);
		}
		Prespawn();
	}

	protected override void FinalSpawnSetup(GameObject newCharacter, CharacterSpawnData spawnData)
	{
		base.FinalSpawnSetup(newCharacter, spawnData);
		if (mSpawnForm != null && mSpawnForm.changeToEnemy)
		{
			CharacterGlobals characterGlobals = newCharacter.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
			if (characterGlobals.combatController != null)
			{
				characterGlobals.combatController.faction = CombatController.Faction.Enemy;
			}
			if (characterGlobals.brawlerCharacterAI != null)
			{
				characterGlobals.brawlerCharacterAI.InitializeAttackFrequency(3f);
			}
		}
	}
}
