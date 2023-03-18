using System;
using UnityEngine;

[Serializable]
public class PolymorphForm
{
	public string characterName = string.Empty;

	public int r2Attack = 1;

	public bool changeToEnemy;

	public string startingEffect = string.Empty;

	public void Initialize(string characterName, int r2Attack)
	{
		this.characterName = characterName;
		this.r2Attack = r2Attack;
	}

	public void Initialize(string characterName, int r2Attack, bool changeToEnemy, string startingEffect)
	{
		this.characterName = characterName;
		this.r2Attack = r2Attack;
		this.changeToEnemy = changeToEnemy;
		this.startingEffect = startingEffect;
	}

	public bool IsValid()
	{
		return !(characterName == string.Empty) && r2Attack >= 0;
	}

	public bool InForm(GameObject polymorph)
	{
		return polymorph.name == characterName;
	}
}
