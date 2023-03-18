using UnityEngine;

public class ShoppingService
{
	private static int guidCounter;

	public static string PurchaseItem(int itemId, int catalogOwnableId, int catalogOwnableSaleID, int quantity, int useShards = 0)
	{
		guidCounter++;
		string guid = guidCounter.ToString();
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("item_id", itemId);
		wWWForm.AddField("catalog_ownable_id", catalogOwnableId);
		wWWForm.AddField("catalog_ownable_sale_id", catalogOwnableSaleID);
		wWWForm.AddField("quantity", quantity);
		wWWForm.AddField("guid", guid);
		wWWForm.AddField("useShards", useShards);
		CspUtils.DebugLog("PurchaseItem itemId: " + itemId);
		CspUtils.DebugLog("PurchaseItem catalog_ownable_id: " + catalogOwnableId);
		CspUtils.DebugLog("PurchaseItem useShards: " + useShards);
		AppShell.Instance.WebService.StartRequest("resources$users/" + AppShell.Instance.Profile.UserId + "/purchase_item.py", delegate(ShsWebResponse response)
		{
			OnPurchaseItemResponse(response, itemId, guid);
		}, wWWForm.data, ShsWebService.ShsWebServiceType.RASP);
		return guid;
	}

	private static void OnPurchaseItemResponse(ShsWebResponse response, int itemId, string guid)
	{
		if (response.Status == 200)
		{
			CspUtils.DebugLog("Purchase of item: " + itemId + " acknowledged.");
			ShoppingPurchaseAcknowledgeMessage msg = new ShoppingPurchaseAcknowledgeMessage(guid, true, response.Status, string.Empty);
			AppShell.Instance.EventMgr.Fire(AppShell.Instance, msg);

			//////////// CSP added this block ////////////////////////////////////////////////
			// CSP - temporarily added following line to circument need for msg from server to complete shopping purchase.
			AppShell.Instance.EventMgr.Fire(AppShell.Instance, new ShoppingPurchaseCompletedMessage("true", "success", guid));
				
			int fractals = int.Parse(response.Body);  // fractal balance after purchase
			//if (sHSSocialMainWindow != null)
			//{
			//	sHSSocialMainWindow.OnDisplayFractalBalance(FractalActivitySpawnPoint.FractalType.Fractal, Convert.ToInt32(payload["total_fractals"]));
			//}
			AppShell.Instance.Profile.Shards = fractals;
			AppShell.Instance.EventMgr.Fire(null, new CurrencyUpdateMessage());
			CspUtils.DebugLog("fractals: " + fractals);
			//////////////////////////////////////////////////////////////////////////////
		}
		else
		{
			CspUtils.DebugLog("Purchase of item: " + itemId + " FAILED, " + response.Status + " - " + response.Body);
			ShoppingPurchaseAcknowledgeMessage msg2 = new ShoppingPurchaseAcknowledgeMessage(guid, false, response.Status, response.Body);
			AppShell.Instance.EventMgr.Fire(AppShell.Instance, msg2);
		}
	}

	public static void SellItem(int itemId, int quantity)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("item_id", itemId);
		wWWForm.AddField("quantity", quantity);
		wWWForm.AddField("guid", "1");
		AppShell.Instance.WebService.StartRequest("resources$users/" + AppShell.Instance.Profile.UserId + "/sell_item/", delegate(ShsWebResponse response)
		{
			OnSellItemResponse(response, itemId, "1");
		}, wWWForm.data, ShsWebService.ShsWebServiceType.RASP);
	}

	private static void OnSellItemResponse(ShsWebResponse response, int itemId, string guid)
	{
		if (response.Status == 200)
		{
			CspUtils.DebugLog("Sale of item: " + itemId + " acknowledged.");
		}
		else
		{
			CspUtils.DebugLog("Sale of item: " + itemId + " FAILED, " + response.Status + " - " + response.Body);
		}
	}
}
