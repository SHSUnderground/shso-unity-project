using System;
using UnityEngine;

[Serializable]
public class PetDerezCommand : PetCommandBase
{
	public float rezDelay = 2f;

	public float fadeTime = 2f;

	public bool respawn = true;

	private SkinnedMeshRenderer renderer;

	public PetDerezCommand()
	{
		type = PetCommandTypeEnum.Derez;
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

	public override PetCommandResultEnum Update()
	{
		CspUtils.DebugLog("PetDerezCommand - Update " + (Time.time - startTime));
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
			return PetCommandResultEnum.InProgress;
		}
		CspUtils.DebugLog("PetDerezCommand - Update - destroy game object");
		UnityEngine.Object.Destroy(gameObject);
		return PetCommandResultEnum.Completed;
	}

	public override string ToString()
	{
		return type.ToString() + ": (" + rezDelay + ", respawn:" + respawn + ")";
	}
}
