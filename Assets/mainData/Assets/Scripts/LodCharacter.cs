using UnityEngine;

[AddComponentMenu("LOD/Character")]
public class LodCharacter : LodBase
{
	public bool useDefaultSettings = true;

	protected SkinnedMeshRenderer[] models;

	protected bool skinsEnabled = true;

	public void Awake()
	{
		if (useDefaultSettings)
		{
			distances = new float[3];
			distances[0] = 15f;
			distances[1] = 20f;
			distances[2] = 30f;
			mode = Mode.Distance;
		}
		models = new SkinnedMeshRenderer[4];
	}

	public override void Start()
	{
		base.Start();
		SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
		if (componentsInChildren.Length == 0)
		{
			CspUtils.DebugLog(base.gameObject.name + " does not have a SkinnedMeshRenderer");
			return;
		}
		SkinnedMeshRenderer[] array = componentsInChildren;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
		{
			if (skinnedMeshRenderer.gameObject.name.ToLower().Contains("lod0"))
			{
				models[0] = skinnedMeshRenderer;
			}
			else if (skinnedMeshRenderer.gameObject.name.ToLower().Contains("lod1"))
			{
				models[1] = skinnedMeshRenderer;
			}
			else if (skinnedMeshRenderer.gameObject.name.ToLower().Contains("lod2"))
			{
				models[2] = skinnedMeshRenderer;
			}
			else if (skinnedMeshRenderer.gameObject.name.ToLower().Contains("lod3"))
			{
				models[3] = skinnedMeshRenderer;
			}
		}
		if (models[0] == null)
		{
			models[0] = componentsInChildren[0];
		}
		if (models[1] == null)
		{
			models[1] = models[0];
		}
		if (models[2] == null)
		{
			models[2] = models[1];
		}
		if (models[3] == null)
		{
			models[3] = models[2];
		}
	}

	public override void SetLod(int lod)
	{
		if (models[0] != null)
		{
			switch (lod)
			{
			case 0:
				models[1].enabled = false;
				models[2].enabled = false;
				models[3].enabled = false;
				models[0].enabled = skinsEnabled;
				models[0].quality = SkinQuality.Auto;
				break;
			case 1:
				models[0].enabled = false;
				models[2].enabled = false;
				models[3].enabled = false;
				models[1].enabled = skinsEnabled;
				models[1].quality = SkinQuality.Bone2;
				break;
			case 2:
				models[0].enabled = false;
				models[1].enabled = false;
				models[3].enabled = false;
				models[2].enabled = skinsEnabled;
				models[2].quality = SkinQuality.Bone1;
				break;
			default:
				models[0].enabled = false;
				models[1].enabled = false;
				models[2].enabled = false;
				models[3].enabled = skinsEnabled;
				models[3].quality = SkinQuality.Bone1;
				break;
			}
		}
	}

	public override bool IsVisible()
	{
		return models[lod].isVisible && skinsEnabled;
	}

	public override Bounds GetBounds()
	{
		return models[lod].bounds;
	}

	public override void SetVisible(bool visible)
	{
		skinsEnabled = visible;
		base.SetVisible(visible);
	}
}
