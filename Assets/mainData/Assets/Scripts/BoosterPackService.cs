using UnityEngine;

public class BoosterPackService
{
	public delegate void OnResponseDelegate(ShsWebResponse response);

	public static void OpenBoosterPack(int boosterPackId, OnResponseDelegate onResponse)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("ownable_type_id", boosterPackId);
		AppShell.Instance.WebService.StartRequest("resources$users/open_booster_pack.py", delegate(ShsWebResponse response)
		{
			OnOpenBoosterPackRequestResponse(response, boosterPackId);
			if (onResponse != null)
			{
				onResponse(response);
			}
		}, wWWForm.data, ShsWebService.ShsWebServiceType.RASP);
	}

	private static void OnOpenBoosterPackRequestResponse(ShsWebResponse response, int boosterPackId)
	{
		if (response.Status == 200)
		{
			CspUtils.DebugLog("Open Booster Request for " + boosterPackId + " acknowledged.");
		}
		else
		{
			CspUtils.DebugLog("Open Booster Request for " + boosterPackId + " FAILED! " + response.Status + " - " + response.Body);
		}
	}
}
