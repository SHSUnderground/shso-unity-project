using System;
using UnityEngine;

[Serializable]
public class ScenarioEventTime
{
	public string ScenarioEvent;

	public float Time;

	[HideInInspector]
	public bool Fired;
}
