using UnityEngine;

public class SquadBattleKeeperPanel : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public SquadBattlePortraitTexture[] portraitTextures;

	public void SetPortrait(string newCharacterName)
	{
		Transform transform = Utils.FindNodeInChildren(base.transform, "portrait");
		if (transform == null)
		{
			CspUtils.DebugLog("Object 'portrait' not found as child of Keeper panel, portraits will not be set to proper textures");
			return;
		}
		MeshRenderer meshRenderer = transform.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
		if (meshRenderer == null)
		{
			CspUtils.DebugLog("Object 'portrait' does not have a mesh renderer, portraits will not be set to proper textures");
			return;
		}
		SquadBattlePortraitTexture[] array = portraitTextures;
		int num = 0;
		SquadBattlePortraitTexture squadBattlePortraitTexture;
		while (true)
		{
			if (num < array.Length)
			{
				squadBattlePortraitTexture = array[num];
				if (squadBattlePortraitTexture.characterName == newCharacterName.ToLower())
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		meshRenderer.material.mainTexture = squadBattlePortraitTexture.texture;
	}
}
