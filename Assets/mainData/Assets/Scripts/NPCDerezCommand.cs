using System;
using UnityEngine;

[Serializable]
public class NPCDerezCommand : NPCCommandBase
{
	public float rezDelay = 2f;

	public float fadeTime = 2f;

	public bool respawn = true;

	private SkinnedMeshRenderer renderer;

	public NPCDerezCommand()
	{
		type = NPCCommandTypeEnum.Derez;
		interruptable = false;
	}

	public override void Start()
	{
		Start(null);
	}

	public override void Start(params object[] initValues)
	{
		base.Start(initValues);
		renderer = Utils.GetComponent<SkinnedMeshRenderer>(gameObject, Utils.SearchChildren);
	}

	public override void Suspend()
	{
		base.Suspend();
	}

	public override void Resume()
	{
		base.Resume();
	}

	public override void Completed()
	{
		base.Completed();
	}

	public override NPCCommandResultEnum Update()
	{
		if (Time.time - startTime <= fadeTime)
		{
			Material[] materials = renderer.materials;
			foreach (Material material in materials)
			{
				Color color = material.color;
				float r = color.r;
				Color color2 = material.color;
				float g = color2.g;
				Color color3 = material.color;
				material.color = new Color(r, g, color3.b, 1f - (Time.time - startTime) / fadeTime);
			}
			return NPCCommandResultEnum.InProgress;
		}
		if (respawn)
		{
			SpawnData component = Utils.GetComponent<SpawnData>(gameObject);
			if (component != null)
			{
				CharacterSpawn spawner = component.spawner;
				spawner.SpawnOnTime(rezDelay);
				UnityEngine.Object.Destroy(gameObject);
			}
			return NPCCommandResultEnum.Completed;
		}
		UnityEngine.Object.Destroy(gameObject);
		return NPCCommandResultEnum.Completed;
	}

	public override string ToString()
	{
		return type.ToString() + ": (" + rezDelay + ", respawn:" + respawn + ")";
	}
}
