using UnityEngine;

public class MysteryBoxService
{
	public delegate void OnResponseDelegate(ShsWebResponse response);

	public static void OpenMysteryBox(int mysteryBoxOwnableID, OnResponseDelegate onResponse)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("ownable_type_id", mysteryBoxOwnableID);
		AppShell.Instance.WebService.StartRequest("resources$users/" + AppShell.Instance.Profile.UserId + "/open_mystery_box/", delegate(ShsWebResponse response)
		{
			OnOpenMysteryBoxRequestResponse(response, mysteryBoxOwnableID);
			if (onResponse != null)
			{
				onResponse(response);
			}
		}, wWWForm.data, ShsWebService.ShsWebServiceType.RASP);
	}

	private static void OnOpenMysteryBoxRequestResponse(ShsWebResponse response, int mysteryBoxOwnableID)
	{
		if (response.Status == 200)
		{
			CspUtils.DebugLog("Open Mystery Box Request for " + mysteryBoxOwnableID + " acknowledged.");
		}
		else
		{
			CspUtils.DebugLog("Open Mystery Box Request for " + mysteryBoxOwnableID + " FAILED! " + response.Status + " - " + response.Body);
		}
	}
}
