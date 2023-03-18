using System.Collections.Generic;
using UnityEngine;

public class HqIcon
{
	private GameObject claimedIcon;

	private List<Material> iconMaterials;

	private Texture2D currentTexture;

	public Vector3 Position
	{
		set
		{
			claimedIcon.transform.position = value;
		}
	}

	public Texture2D Icon
	{
		get
		{
			return currentTexture;
		}
		set
		{
			currentTexture = value;
			foreach (Material iconMaterial in iconMaterials)
			{
				if (iconMaterial.GetTexture("_MainTex") != value)
				{
					iconMaterial.SetTexture("_MainTex", value);
				}
			}
		}
	}

	public bool Visible
	{
		get
		{
			return claimedIcon.active;
		}
		set
		{
			if (claimedIcon.active != value)
			{
				Utils.ActivateTree(claimedIcon, value);
			}
		}
	}

	public HqIcon(Transform parent, Vector3 position)
	{
		iconMaterials = new List<Material>();
		AssetBundle assetBundle = HqController2.Instance.GetAssetBundle("HQ/hq_shared");
		if (!(assetBundle != null))
		{
			return;
		}
		GameObject gameObject = assetBundle.Load("ClaimIcon") as GameObject;
		if (!(gameObject != null))
		{
			return;
		}
		claimedIcon = (Object.Instantiate(gameObject) as GameObject);
		if (!(claimedIcon != null))
		{
			return;
		}
		Utils.SetLayerTree(claimedIcon, 2);
		Utils.ActivateTree(claimedIcon, false);
		claimedIcon.transform.parent = parent;
		claimedIcon.transform.position = position;
		for (int i = 0; i < claimedIcon.transform.childCount; i++)
		{
			GameObject gameObject2 = claimedIcon.transform.GetChild(i).gameObject;
			if (gameObject2.renderer != null)
			{
				Material[] materials = gameObject2.renderer.materials;
				foreach (Material item in materials)
				{
					iconMaterials.Add(item);
				}
			}
		}
	}
}
