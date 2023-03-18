using System;
using UnityEngine;

[Serializable]
public class HappinessLevelInfo
{
	public AIControllerHQ.Mood mood;

	public GameObject particleSystem;

	public HappinessLevelInfo(AIControllerHQ.Mood mood)
	{
		this.mood = mood;
	}

	public static implicit operator HappinessLevelInfo(AIControllerHQ.Mood mood)
	{
		return new HappinessLevelInfo(mood);
	}
}
