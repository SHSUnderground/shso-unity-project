using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml.XPath;
using UnityEngine;

public class SmartTipsManager : StaticDataDefinition, IStaticDataDefinition
{
	public const int MAX_TIP_LENGTH = 128;

	private Dictionary<string, SmartTipTitle> smartTipTitles;

	private Dictionary<string, List<SmartTip>> categorySmartTipDictionary;

	private List<SmartTip> smartTips;

	private static SmartTipUsageInstance defaultSmartTipUsageInstance = default(SmartTipUsageInstance);

	public Dictionary<string, SmartTipTitle> SmartTipTitles
	{
		get
		{
			return smartTipTitles;
		}
	}

	public Dictionary<string, List<SmartTip>> CategorySmartTipDictionary
	{
		get
		{
			return categorySmartTipDictionary;
		}
	}

	public List<SmartTip> SmartTips
	{
		get
		{
			return smartTips;
		}
	}

	public static SmartTipUsageInstance DefaultSmartTipUsageInstance
	{
		get
		{
			return defaultSmartTipUsageInstance;
		}
	}

	public SmartTipsManager()
	{
		smartTipTitles = new Dictionary<string, SmartTipTitle>();
		categorySmartTipDictionary = new Dictionary<string, List<SmartTip>>();
		smartTips = new List<SmartTip>();
		defaultSmartTipUsageInstance = default(SmartTipUsageInstance);
		defaultSmartTipUsageInstance.tip = string.Empty;
		defaultSmartTipUsageInstance.titleIcon = string.Empty;
		defaultSmartTipUsageInstance.titleKey = string.Empty;
	}

	public void InitializeFromData(DataWarehouse data)
	{
		XPathNavigator value = data.GetValue(".");
		value.MoveToFirstChild();
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(SmartTipTitle));
		XPathNodeIterator xPathNodeIterator = value.Select("//TipTitles/TipTitle");
		while (xPathNodeIterator.MoveNext())
		{
			string outerXml = xPathNodeIterator.Current.OuterXml;
			SmartTipTitle smartTipTitle = xmlSerializer.Deserialize(new StringReader(outerXml)) as SmartTipTitle;
			if (smartTipTitle != null)
			{
				if (smartTipTitles.ContainsKey(smartTipTitle.Key))
				{
					CspUtils.DebugLog("Duplicate smart tip title: " + smartTipTitle.Key);
				}
				else
				{
					smartTipTitles[smartTipTitle.Key] = smartTipTitle;
				}
			}
			else
			{
				CspUtils.DebugLog("Unable to bring in Smart Tip Title. Input xml is:" + outerXml);
			}
		}
		XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(SmartTip));
		XPathNodeIterator xPathNodeIterator2 = value.Select("//SmartTips/SmartTip");
		while (xPathNodeIterator2.MoveNext())
		{
			string outerXml2 = xPathNodeIterator2.Current.OuterXml;
			SmartTip smartTip = xmlSerializer2.Deserialize(new StringReader(outerXml2)) as SmartTip;
			if (smartTip != null)
			{
				bool flag = false;
				SmartTipCategory[] smartTipCategories = smartTip.SmartTipCategories;
				foreach (SmartTipCategory smartTipCategory in smartTipCategories)
				{
					if (!(smartTipCategory.Weight <= 0f))
					{
						if (categorySmartTipDictionary.ContainsKey(smartTipCategory.Name))
						{
							categorySmartTipDictionary[smartTipCategory.Name].Add(smartTip);
						}
						else
						{
							List<SmartTip> list = new List<SmartTip>();
							list.Add(smartTip);
							categorySmartTipDictionary[smartTipCategory.Name] = list;
						}
						flag = true;
					}
				}
				smartTips.Add(smartTip);
				if (!flag)
				{
					CspUtils.DebugLog("SmartTip " + smartTip.tipKey + " has no category weightings, and will never be shown.");
				}
			}
			else
			{
				CspUtils.DebugLog("Unable to bring in Smart Tip. Input xml is:" + outerXml2);
			}
		}
	}

	public SmartTipUsageInstance GetSmartTip(string category)
	{
		List<string> list = new List<string>();
		list.Add(category);
		return GetSmartTip(list);
	}

	public SmartTipUsageInstance GetSmartTip(List<string> categories)
	{
		//Discarded unreachable code: IL_02bf
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null || ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.ProTips, 1) == 0)
		{
			return DefaultSmartTipUsageInstance;
		}
		List<KeyValuePair<SmartTip, float>> list = new List<KeyValuePair<SmartTip, float>>();
		float num = 0f;
		SmartTipUsageInstance result = default(SmartTipUsageInstance);
		bool flag = false;
		foreach (string category in categories)
		{
			if (categorySmartTipDictionary.ContainsKey(category))
			{
				foreach (SmartTip item in categorySmartTipDictionary[category])
				{
					bool flag2 = Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.IsPayingSubscriber);
					if (item.Target == SmartTip.PlayerTargetEnum.Any || ((!flag2 || item.Target != SmartTip.PlayerTargetEnum.FreePlayer) && (flag2 || item.Target != 0)))
					{
						SmartTipCategory[] smartTipCategories = item.SmartTipCategories;
						foreach (SmartTipCategory smartTipCategory in smartTipCategories)
						{
							if (smartTipCategory.Name == category)
							{
								flag = true;
								num += smartTipCategory.Weight;
								list.Add(new KeyValuePair<SmartTip, float>(item, smartTipCategory.Weight));
							}
						}
					}
				}
			}
		}
		if (!flag || list.Count == 0)
		{
			return defaultSmartTipUsageInstance;
		}
		float num2 = UnityEngine.Random.Range(0f, num);
		float num3 = 0f;
		try
		{
			foreach (KeyValuePair<SmartTip, float> item2 in list)
			{
				num3 += item2.Value;
				if (num3 >= num2)
				{
					SmartTipCategory[] smartTipCategories2 = item2.Key.SmartTipCategories;
					foreach (SmartTipCategory smartTipCategory2 in smartTipCategories2)
					{
						if (categories.Count > 0 && categories.Contains(smartTipCategory2.Name))
						{
							smartTipCategory2.Weight = Math.Max(1f, smartTipCategory2.Weight - item2.Key.Erosion);
						}
					}
					result.tip = item2.Key.tipKey;
					result.titleKey = item2.Key.titleKey;
					result.titleIcon = smartTipTitles[item2.Key.titleKey].Icon;
					item2.Key.UseCount++;
					return result;
				}
			}
			return result;
		}
		catch (Exception)
		{
			return DefaultSmartTipUsageInstance;
		}
	}
}
