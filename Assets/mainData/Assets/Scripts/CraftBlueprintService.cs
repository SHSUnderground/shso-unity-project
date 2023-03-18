using UnityEngine;

public class CraftBlueprintService
{
	public delegate void OnResponseDelegate(ShsWebResponse response);

	public static void CraftBlueprint(int blueprintID, OnResponseDelegate onResponse)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("blueprint_id", blueprintID);
		AppShell.Instance.WebService.StartRequest("resources$users/" + AppShell.Instance.Profile.UserId + "/craft_blueprint/", delegate(ShsWebResponse response)
		{
			OnCraftBlueprintRequestResponse(response, blueprintID);
			if (onResponse != null)
			{
				onResponse(response);
			}
		}, wWWForm.data, ShsWebService.ShsWebServiceType.RASP);
	}

	private static void OnCraftBlueprintRequestResponse(ShsWebResponse response, int blueprintID)
	{
		if (response.Status == 200)
		{
			CspUtils.DebugLog("Craft Blueprint Request for " + blueprintID + " acknowledged.");
		}
		else
		{
			CspUtils.DebugLog("Craft Blueprint Request for " + blueprintID + " FAILED! " + response.Status + " - " + response.Body);
		}
	}
}
