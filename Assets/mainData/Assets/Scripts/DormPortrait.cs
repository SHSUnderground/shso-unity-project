using UnityEngine;

[AddComponentMenu("Hq/DormPortait")]
public class DormPortrait : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected string assignedCharacter;

	protected Texture2D portraitTexture;

	public string AssignedCharacter
	{
		get
		{
			return assignedCharacter;
		}
		set
		{
			if (assignedCharacter != value)
			{
				assignedCharacter = value;
				if (value != null)
				{
					AppShell.Instance.DataManager.LoadGameData("Characters/" + assignedCharacter, OnCharacterDataLoaded);
					return;
				}
				CspUtils.DebugLogWarning("No character portrait found for <" + assignedCharacter + ">");
				portraitTexture = null;
			}
		}
	}

	protected void OnCharacterDataLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			return;
		}
		DataWarehouse data = response.Data;
		CharacterDefinition characterDefinition = new CharacterDefinition();
		characterDefinition.InitializeFromData(data);
		if (characterDefinition.InventoryIconName != null)
		{
			portraitTexture = GUIManager.Instance.LoadTexture(characterDefinition.InventoryIconName);
			if (base.gameObject.renderer != null)
			{
				Renderer renderer = base.gameObject.renderer;
				renderer.enabled = true;
				if (renderer.material != null)
				{
					renderer.material.SetTexture("_MainTex", portraitTexture);
				}
			}
		}
		else
		{
			CspUtils.DebugLog("InventoryIconName is null. :)");
		}
	}

	private void Start()
	{
		if (base.gameObject.renderer != null)
		{
			base.gameObject.renderer.enabled = false;
		}
	}
}
