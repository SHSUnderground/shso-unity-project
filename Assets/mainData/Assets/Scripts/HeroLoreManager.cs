using LitJson;
using System.Collections.Generic;

public class HeroLoreManager
{
	public const int MAX_LORE = 3;

	private TransactionMonitor _monitor;

	private Dictionary<string, int> _heroLoreMap;

	public int GetHeroLore(string hero)
	{
		if (_heroLoreMap == null)
		{
			CspUtils.DebugLog("HeroLoreManager::GetHeroLore() - hero lore map not created");
			return -1;
		}
		if (string.IsNullOrEmpty(hero))
		{
			CspUtils.DebugLog("HeroLoreManager::GetHeroLore() - argument hero string is null or empty");
			return -1;
		}
		if (_heroLoreMap.ContainsKey(hero))
		{
			return _heroLoreMap[hero];
		}
		CspUtils.DebugLog("HeroLoreManager::GetHeroLore() - hero string <" + hero + "> not contained in hero lore map");
		return -1;
	}

	public void RequestHeroLoreValues(TransactionMonitor monitor)
	{
		_monitor = monitor;
		if (_monitor != null)
		{
			_monitor.AddStep("hero_lore");
		}
		AppShell.Instance.WebService.StartRequest("resources$data/json/hero_lore_bonus.py", OnHeroLoreWebResponse, null, ShsWebService.ShsWebServiceType.RASP);
	}

	private void OnHeroLoreWebResponse(ShsWebResponse response)
	{
		if (response.Status == 200)
		{
			Dictionary<string, List<HeroLoreBonus>> dictionary = JsonMapper.ToObject<Dictionary<string, List<HeroLoreBonus>>>(response.Body);
			if (dictionary != null && dictionary.ContainsKey("hero_lore_bonus"))
			{
				_heroLoreMap = new Dictionary<string, int>();
				foreach (HeroLoreBonus item in dictionary["hero_lore_bonus"])
				{
					if (!string.IsNullOrEmpty(item.hero))
					{
						if (_heroLoreMap.ContainsKey(item.hero))
						{
							CspUtils.DebugLog("HeroLoreManager::OnHeroLoreWebResponse() - duplicate key <" + item.hero + "> found in hero lore map");
						}
						else
						{
							_heroLoreMap.Add(item.hero, item.lore_bonus);
						}
					}
				}
			}
		}
		else
		{
			CspUtils.DebugLog("HeroLoreManager::OnHeroLoreWebResponse() - failed to obtain hero lore from request URI: " + response.RequestUri);
		}
		if (_monitor != null)
		{
			_monitor.CompleteStep("hero_lore");
		}
	}
}
