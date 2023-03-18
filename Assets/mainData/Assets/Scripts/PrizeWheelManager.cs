using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml.XPath;

public class PrizeWheelManager : StaticDataDefinition, IStaticDataDefinition
{
	public enum WheelTypeEnum
	{
		MainGold,
		MainSilver,
		MainUser,
		CardGold,
		CardSilver,
		CraftingGold,
		CraftingSilver,
		FractalGold,
		FractalSilver
	}

	public enum CurrencyEnum
	{
		Gold,
		Silver,
		Composite,
		DingDongs
	}

	public struct WheelTypeInfo
	{
		public WheelTypeEnum WheelType;

		public int XmlIndex;

		public CurrencyEnum SupportingCurrency;

		public WheelTypeInfo(WheelTypeEnum wheelType, int xmlIndex, CurrencyEnum supportingCurrency)
		{
			WheelType = wheelType;
			XmlIndex = xmlIndex;
			SupportingCurrency = supportingCurrency;
		}
	}

	public static string[] CurrencyEnumStringIds = new string[4]
	{
		"#PRIZEWHEEL_GOLD",
		"#PRIZEWHEEL_SILVER",
		"#PRIZEWHEEL_SILVER",
		"#PRIZEWHEEL_SILVER"
	};

	private readonly Dictionary<WheelTypeEnum, PrizeWheel> prizeWheels;

	private readonly Dictionary<int, WheelTypeInfo> prizeWheelInfo;

	private bool isReady;

	public Dictionary<WheelTypeEnum, PrizeWheel> PrizeWheels
	{
		get
		{
			return prizeWheels;
		}
	}

	public Dictionary<int, WheelTypeInfo> PrizeWheelInfo
	{
		get
		{
			return prizeWheelInfo;
		}
	}

	public bool IsReady
	{
		get
		{
			return isReady;
		}
		set
		{
			isReady = value;
		}
	}

	public PrizeWheelManager()
	{
		prizeWheels = new Dictionary<WheelTypeEnum, PrizeWheel>();
		prizeWheelInfo = new Dictionary<int, WheelTypeInfo>();
		prizeWheelInfo[-1] = new WheelTypeInfo(WheelTypeEnum.MainGold, -1, CurrencyEnum.Gold);
		prizeWheelInfo[-2] = new WheelTypeInfo(WheelTypeEnum.MainSilver, -2, CurrencyEnum.Silver);
		prizeWheelInfo[1] = new WheelTypeInfo(WheelTypeEnum.CraftingGold, 1, CurrencyEnum.Gold);
		prizeWheelInfo[2] = new WheelTypeInfo(WheelTypeEnum.FractalGold, 2, CurrencyEnum.Gold);
		prizeWheelInfo[3] = new WheelTypeInfo(WheelTypeEnum.CraftingSilver, 3, CurrencyEnum.Silver);
		prizeWheelInfo[4] = new WheelTypeInfo(WheelTypeEnum.FractalSilver, 4, CurrencyEnum.Silver);
		prizeWheelInfo[5] = new WheelTypeInfo(WheelTypeEnum.CardGold, 5, CurrencyEnum.Gold);
		prizeWheelInfo[6] = new WheelTypeInfo(WheelTypeEnum.CardSilver, 6, CurrencyEnum.Silver);
		foreach (int value in Enum.GetValues(typeof(WheelTypeEnum)))
		{
			PrizeWheel prizeWheel = new PrizeWheel(this);
			prizeWheel.WheelType = (WheelTypeEnum)value;
			foreach (WheelTypeInfo value2 in prizeWheelInfo.Values)
			{
				if (value2.WheelType == (WheelTypeEnum)value)
				{
					prizeWheel.SupportingCurrency = value2.SupportingCurrency;
					prizeWheel.XmlId = value2.XmlIndex;
					break;
				}
			}
			prizeWheels[(WheelTypeEnum)value] = prizeWheel;
		}
		isReady = false;
	}

	public PrizeWheel GetPrizeWheelByXmlId(int id)
	{
		foreach (PrizeWheel value in prizeWheels.Values)
		{
			if (value.XmlId == id)
			{
				return value;
			}
		}
		return null;
	}

	public PrizeWheel GetCurrentWheelState()
	{
		BitArray bitArray = new BitArray(40);
		UserProfile profile = AppShell.Instance.Profile;
		if (profile != null)
		{
			bitArray = profile.PrizeWheelState;
		}
		PrizeWheel prizeWheel = new PrizeWheel(this);
		prizeWheel.WheelType = WheelTypeEnum.MainUser;
		PrizeWheel prizeWheel2 = prizeWheels[WheelTypeEnum.MainGold];
		PrizeWheel prizeWheel3 = prizeWheels[WheelTypeEnum.MainSilver];
		for (int i = 1; i <= 24; i++)
		{
			PrizeWheelStop prizeWheelStop = prizeWheel2[i];
			if (prizeWheelStop == null)
			{
				prizeWheel[i] = prizeWheel3[i];
				CspUtils.DebugLog("Stop: " + i + " is not a gold stop, so reverting to silver.");
			}
			else
			{
				prizeWheel[i] = ((!bitArray[i - 1]) ? prizeWheel2[i] : prizeWheel3[i]);
			}
		}
		return prizeWheel;
	}

	public void SetUserWheelState(PrizeWheelStop stop, bool goldUsed)
	{
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null)
		{
			CspUtils.DebugLog("Profile not available for user. No wheel state could be determined.");
			return;
		}
		BitArray prizeWheelState = profile.PrizeWheelState;
		prizeWheelState[stop.index - 1] = goldUsed;
		profile.PrizeWheelState = prizeWheelState;
	}

	public void InitializeFromData(DataWarehouse data)
	{
		isReady = false;
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(PrizeWheelStop));
		XPathNavigator value = data.GetValue("wheel/gold");
		XPathNodeIterator xPathNodeIterator = value.SelectChildren("stop", string.Empty);
		while (xPathNodeIterator.MoveNext())
		{
			string outerXml = xPathNodeIterator.Current.OuterXml;
			PrizeWheelStop prizeWheelStop = xmlSerializer.Deserialize(new StringReader(outerXml)) as PrizeWheelStop;
			if (prizeWheelStop != null && prizeWheelStop.index <= 24)
			{
				PrizeWheels[WheelTypeEnum.MainGold][prizeWheelStop.index] = prizeWheelStop;
				prizeWheelStop.Wheel = PrizeWheels[WheelTypeEnum.MainGold];
				prizeWheelStop.SupportingCurrency = CurrencyEnum.Gold;
			}
		}
		value = data.GetValue("wheel/silver");
		xPathNodeIterator = value.SelectChildren("stop", string.Empty);
		while (xPathNodeIterator.MoveNext())
		{
			string outerXml2 = xPathNodeIterator.Current.OuterXml;
			PrizeWheelStop prizeWheelStop2 = xmlSerializer.Deserialize(new StringReader(outerXml2)) as PrizeWheelStop;
			if (prizeWheelStop2 != null && prizeWheelStop2.index <= 24)
			{
				PrizeWheels[WheelTypeEnum.MainSilver][prizeWheelStop2.index] = prizeWheelStop2;
				prizeWheelStop2.Wheel = PrizeWheels[WheelTypeEnum.MainSilver];
				prizeWheelStop2.SupportingCurrency = CurrencyEnum.Silver;
			}
		}
		value = data.GetValue("wheel/subwheels");
		xPathNodeIterator = value.SelectChildren("subwheel", string.Empty);
		while (xPathNodeIterator.MoveNext())
		{
			XPathNodeIterator xPathNodeIterator2 = xPathNodeIterator.Current.SelectChildren("stop", string.Empty);
			while (xPathNodeIterator2.MoveNext())
			{
				string outerXml3 = xPathNodeIterator2.Current.OuterXml;
				PrizeWheelStop prizeWheelStop3 = xmlSerializer.Deserialize(new StringReader(outerXml3)) as PrizeWheelStop;
				if (prizeWheelStop3 == null || !prizeWheelStop3.subwheelId.HasValue || prizeWheelStop3.index > 24)
				{
					continue;
				}
				if (prizeWheelInfo.ContainsKey(prizeWheelStop3.subwheelId.Value))
				{
					WheelTypeInfo wheelTypeInfo = prizeWheelInfo[prizeWheelStop3.subwheelId.Value];
					WheelTypeEnum wheelType = wheelTypeInfo.WheelType;
					if (PrizeWheels.ContainsKey(wheelType))
					{
						PrizeWheels[wheelType][prizeWheelStop3.index] = prizeWheelStop3;
						prizeWheelStop3.Wheel = PrizeWheels[wheelType];
						prizeWheelStop3.SupportingCurrency = PrizeWheels[wheelType].SupportingCurrency;
					}
					else
					{
						CspUtils.DebugLog("Can't find prize wheel matching type: " + wheelType);
					}
				}
				else
				{
					CspUtils.DebugLog("Cant match up prize wheel: " + prizeWheelStop3.subwheelId.Value);
				}
			}
		}
		List<KeyValuePair<PrizeWheel, int>> list = new List<KeyValuePair<PrizeWheel, int>>();
		foreach (PrizeWheel value2 in prizeWheels.Values)
		{
			if (value2.WheelType != WheelTypeEnum.MainUser)
			{
				for (int i = 1; i <= prizeWheels[value2.WheelType].Count; i++)
				{
					PrizeWheelStop prizeWheelStop4 = prizeWheels[value2.WheelType][i];
					if (prizeWheelStop4 == null)
					{
						list.Add(new KeyValuePair<PrizeWheel, int>(value2, i));
					}
				}
			}
		}
		if (list.Count > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<PrizeWheel, int> item in list)
			{
				stringBuilder.Append(item.Key.WheelType + "_" + item.Value + Environment.NewLine);
			}
			CspUtils.DebugLog("Invalid wheel configuration. Following are missing." + Environment.NewLine + stringBuilder);
			CspUtils.DebugLog("Soon, the Prize Wheel will be disabled when this condition occurs");
		}
		isReady = (list.Count == 0);
	}
}
