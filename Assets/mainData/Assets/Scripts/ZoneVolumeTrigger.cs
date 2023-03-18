using UnityEngine;

[RequireComponent(typeof(ZoneToData))]
public class ZoneVolumeTrigger : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public void OnTriggerEnter(Collider other)
	{
		PlayerInputController component = Utils.GetComponent<PlayerInputController>(other);
		if (!(component == null))
		{
			ZoneToData data = Utils.GetComponent<ZoneToData>(this);
			if (!(data == null) && data.zoneName != null && !(data.zoneName == string.Empty))
			{
				GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, "Go to the next district?", delegate(string Id, GUIDialogWindow.DialogState state)
				{
					if (state == GUIDialogWindow.DialogState.Ok)
					{
						AppShell.Instance.SharedHashTable["SocialSpaceLevel"] = data.zoneName;
						AppShell.Instance.SharedHashTable["SocialSpaceSpawnPoint"] = data.spawnName;
						AppShell.Instance.SharedHashTable["SocialSpaceCharacter"] = other.gameObject.name;
						AppShell.Instance.SharedHashTable["SocialSpaceTicket"] = null;
						AppShell.Instance.Transition(GameController.ControllerType.SocialSpace);
					}
				}, GUIControl.ModalLevelEnum.Default);
			}
		}
	}
}
