using UnityEngine;

public class MysteryBoxService
{
	public delegate void OnResponseDelegate(ShsWebResponse response);

	public static void OpenMysteryBox(int mysteryBoxOwnableID, OnResponseDelegate onResponse)
	{
		//Commented out by doggo
		/*WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("ownable_type_id", mysteryBoxOwnableID);
		AppShell.Instance.WebService.StartRequest("resources$users/open_mystery_box.py", delegate(ShsWebResponse response)
		{
			OnOpenMysteryBoxRequestResponse(response, mysteryBoxOwnableID);
			if (onResponse != null)
			{
				onResponse(response);
			}
		}, wWWForm.data, ShsWebService.ShsWebServiceType.RASP);*/
		
		//Uses SFS Instead!
		AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume,"open_mystery_box","",1,mysteryBoxOwnableID.ToString());
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
