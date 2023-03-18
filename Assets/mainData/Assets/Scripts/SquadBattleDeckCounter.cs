using UnityEngine;

public class SquadBattleDeckCounter : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Texture[] textures;

	public int Count = 40;

	public int MaxCount = 40;

	public int MidCount = 20;

	public int MinCount = 10;

	public bool apply;

	protected MeshRenderer meshRenderer;

	protected MeshRenderer backdropMeshRenderer;

	protected Animation counterAnimation;

	private void Awake()
	{
		Transform transform = Utils.FindNodeInChildren(base.transform, "deck_number");
		if (transform == null)
		{
			CspUtils.DebugLog("No deck_number object found, deck counter will not function");
			return;
		}
		meshRenderer = (transform.GetComponent(typeof(MeshRenderer)) as MeshRenderer);
		if (meshRenderer == null)
		{
			CspUtils.DebugLog("No mesh renderer found in deck_number object, deck counter will not function!");
		}
		Transform transform2 = Utils.FindNodeInChildren(base.transform, "deck_number_backdrop");
		if (transform2 == null)
		{
			CspUtils.DebugLog("No deck_number_backdrop object found, backdrop will not be colorized");
		}
		else
		{
			backdropMeshRenderer = (transform2.GetComponent(typeof(MeshRenderer)) as MeshRenderer);
		}
		counterAnimation = (transform.GetComponent(typeof(Animation)) as Animation);
	}

	private void Update()
	{
		if (apply)
		{
			setDeckCount(Count);
			apply = false;
		}
	}

	public void setDeckCount(int newCount)
	{
		int num = textures.Length - 1;
		if (newCount < 0)
		{
			newCount = 0;
		}
		else if (newCount > num)
		{
			CspUtils.DebugLog("User has more than " + num + " cards - capping display at " + num);
			newCount = num;
		}
		bool flag = Count != newCount;
		Count = newCount;
		meshRenderer.material.mainTexture = textures[newCount];
		if (flag && counterAnimation != null)
		{
			counterAnimation.Rewind("change");
			counterAnimation.Play("change");
		}
		if (newCount <= MinCount)
		{
			Color color = Color.Lerp(Color.yellow, Color.red, 1f - (float)newCount / (float)MinCount);
			backdropMeshRenderer.material.color = color;
		}
		else if (newCount <= MidCount)
		{
			Color color2 = Color.Lerp(Color.white, Color.yellow, 1f - (float)(newCount - MidCount) / (float)MidCount);
			backdropMeshRenderer.material.color = color2;
		}
		else
		{
			backdropMeshRenderer.material.color = Color.white;
		}
	}

	public void setVisible(bool visible)
	{
		meshRenderer.enabled = visible;
		backdropMeshRenderer.enabled = visible;
	}
}
