using System.Collections.Generic;
using UnityEngine;

public class VOInputMissionBriefingSpeaker : IVOInputResolver
{
	protected static Dictionary<string, string> missionBriefingSpeakers = new Dictionary<string, string>();

	public static void AddMissionBriefingSpeaker(string mission, string speaker)
	{
		missionBriefingSpeakers[mission] = speaker;
	}

	public void SetVOParams(string[] parameters)
	{
	}

	public string ResolveVOInput(GameObject emitter, IEnumerable<string> previousInputs)
	{
		foreach (string previousInput in previousInputs)
		{
			string value;
			if (missionBriefingSpeakers.TryGetValue(previousInput, out value))
			{
				return value;
			}
		}
		return string.Empty;
	}
}
