using UnityEngine;

public class HQService
{
	public static void GrantXP(string heroName, int amount)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("hero_name", heroName);
		wWWForm.AddField("award_xp", amount);
		AppShell.Instance.WebService.StartRequest("resources$users/hq_grant_xp.py", delegate(ShsWebResponse response)
		{
			OnGrantXPResponse(response, heroName, amount);
		}, wWWForm.data, ShsWebService.ShsWebServiceType.RASP);
	}

	private static void OnGrantXPResponse(ShsWebResponse response, string heroName, int amount)
	{
		if (response.Status == 200)
		{
			CspUtils.DebugLog(string.Format("Granted {0} XP to hero {1}", amount, heroName));
		}
		else
		{
			CspUtils.DebugLog(string.Format("Granting {0} XP to hero {1} FAILED! {2} - {3}", amount, heroName, response.Status, response.Body));
		}
	}
}
