using LitJson;
using System.Collections.Generic;

public class SHSNewsRewardJson
{
	private static SHSNewsRewardJson _instance;

	private TransactionMonitor _monitor;

	private Dictionary<string, List<SHSNewsReward>> _jsonResponse;

	public static SHSNewsRewardJson Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new SHSNewsRewardJson();
			}
			return _instance;
		}
	}

	public static void Release()
	{
		_instance = null;
	}

	public void RequestDailyReward(TransactionMonitor monitor)
	{
		_monitor = monitor;
		if (_monitor != null)
		{
			_monitor.AddStep("news_daily_rewards");
		}
		AppShell.Instance.WebService.StartRequest("resources$data/json/daily-rewards", OnDailyRewardWebResponse, null, ShsWebService.ShsWebServiceType.RASP);
	}

	public List<SHSNewsReward> GetDailyRewardData()
	{
		return (!HasDailyRewardData()) ? null : _jsonResponse["daily_rewards"];
	}

	public bool HasDailyRewardData()
	{
		return _jsonResponse != null && _jsonResponse.ContainsKey("daily_rewards");
	}

	public void OnDailyRewardWebResponse(ShsWebResponse response)
	{
		if (response.Status == 200)
		{
			_jsonResponse = JsonMapper.ToObject<Dictionary<string, List<SHSNewsReward>>>(response.Body);
		}
		else
		{
			CspUtils.DebugLog("Failed to obtain daily rewards from request URI: " + response.RequestUri);
		}
		if (_monitor != null)
		{
			_monitor.CompleteStep("news_daily_rewards");
		}
	}
}
