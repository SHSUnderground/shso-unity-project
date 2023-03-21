using UnityEngine;

public class OwnablesManager
{
	public delegate void OwnablesFetchResponseDelegate(ShsWebResponse response, OwnableDefinition.Category category);

	public delegate void OwnablesFetchCompleteDelegate(bool success);

	public static void StartOwnablesFetch(OwnableDefinition.Category category, OwnablesFetchResponseDelegate callback, long UserId)
	{
		WWWForm wWWForm = new WWWForm();
		if (category != OwnableDefinition.Category.All)
		{
			wWWForm.AddField("category", OwnableDefinition.getStringFromCategory(category));
		}
		wWWForm.AddField("user", UserId.ToString());	// CSP
		AppShell.Instance.WebService.StartRequest("resources$users/inventory.py", delegate(ShsWebResponse response)
		{
			if (callback != null)
			{
				callback(response, category);
			}
			if (response.Status == 200)
			{
				AppShell.Instance.EventMgr.Fire(AppShell.Instance, new OwnableFetchedMessage(category));
			}
		}, wWWForm.data);
	}
}
