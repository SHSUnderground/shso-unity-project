using UnityEngine;

public class HqBoomBox : HqTriggerArea
{
	protected const string DANCE_EMOTE_COMMAND = "dance";

	protected sbyte danceEmoteId;

	protected override void AddNewEntry(out Entry data, GameObject obj)
	{
		if (!isOn)
		{
			data = null;
			return;
		}
		AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(obj, Utils.SearchChildren);
		if (component == null || !component.IsInDefaultBehavior)
		{
			data = null;
			return;
		}
		data = new Entry();
		data.lastUpdate = Time.time;
		data.hqComp = component;
		objects[obj] = data;
		component.DoEmote(danceEmoteId);
	}

	protected override void RemoveEntry(GameObject go)
	{
		if (!(go != null))
		{
			return;
		}
		Entry value;
		objects.TryGetValue(go, out value);
		if (value != null && value.hqComp != null)
		{
			AIControllerHQ aIControllerHQ = value.hqComp as AIControllerHQ;
			if (aIControllerHQ != null)
			{
				aIControllerHQ.StopEmote();
			}
		}
		base.RemoveEntry(go);
	}

	protected override bool IsValidGameObject(GameObject go)
	{
		if (go == null || Utils.GetComponent<AIControllerHQ>(go) == null)
		{
			return false;
		}
		return true;
	}

	protected override GameObject GetRelevantGameObject(GameObject go)
	{
		AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(go);
		if (component != null)
		{
			return go;
		}
		HqObject2 component2 = Utils.GetComponent<HqObject2>(go);
		if (component2 != null)
		{
			return go;
		}
		return null;
	}

	public override void Start()
	{
		base.Start();
		danceEmoteId = -1;
	}

	public override void Update()
	{
		base.Update();
		if (danceEmoteId == -1 && EmotesDefinition.Instance != null)
		{
			EmotesDefinition.EmoteDefinition emoteByCommand = EmotesDefinition.Instance.GetEmoteByCommand("dance");
			if (emoteByCommand != null)
			{
				danceEmoteId = emoteByCommand.id;
			}
		}
	}
}
