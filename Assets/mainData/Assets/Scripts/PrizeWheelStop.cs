using System;
using System.Xml.Serialization;

[Serializable]
[XmlRoot(ElementName = "stop")]
public class PrizeWheelStop
{
	public enum PrizeTypeEnum
	{
		Money,
		SubWheel,
		DictatedByWheelType
	}

	[XmlElement(ElementName = "wheel_stop")]
	[XmlElement(ElementName = "stop")]
	public int index;

	private PrizeTypeEnum prizeType;

	private string xmlPrizeType;

	[XmlElement(ElementName = "item_id")]
	public XmlNullableInt itemId;

	[XmlElement(ElementName = "description")]
	public string description;

	[XmlElement(ElementName = "coin_value")]
	public XmlNullableInt coinValue;

	[XmlElement(ElementName = "card_type_id")]
	public string cardId;

	[XmlElement(ElementName = "sub_wheel_id")]
	public XmlNullableInt subwheelId;

	[XmlElement(ElementName = "rarity")]
	public string rarity;

	[XmlElement(ElementName = "quantity")]
	public int quantity;

	[XmlIgnore]
	private PrizeWheel wheel;

	[XmlIgnore]
	protected PrizeWheelManager.CurrencyEnum supportingCurrency;

	[XmlIgnore]
	public PrizeTypeEnum PrizeType
	{
		get
		{
			return prizeType;
		}
		set
		{
			prizeType = value;
		}
	}

	[XmlElement(ElementName = "prize_type")]
	public string XmlPrizeType
	{
		get
		{
			return xmlPrizeType;
		}
		set
		{
			xmlPrizeType = value;
			switch (value)
			{
			case "S":
				prizeType = PrizeTypeEnum.SubWheel;
				break;
			case "M":
				prizeType = PrizeTypeEnum.Money;
				break;
			default:
				prizeType = PrizeTypeEnum.DictatedByWheelType;
				break;
			}
		}
	}

	[XmlIgnore]
	public PrizeWheel Wheel
	{
		get
		{
			return wheel;
		}
		set
		{
			wheel = value;
		}
	}

	[XmlIgnore]
	public PrizeWheelManager.CurrencyEnum SupportingCurrency
	{
		get
		{
			return supportingCurrency;
		}
		set
		{
			supportingCurrency = value;
		}
	}

	public override string ToString()
	{
		return string.Format("<stop wheelstop='{0}' prize_type='{1}' item_id='{2}' description='{3}' coin_value='{4}' card_type_id='{5}' sub_wheel_id='{6}' rarity='{7}' quantity='{8}' />", Wheel, PrizeType, itemId, description, coinValue, cardId, subwheelId, rarity, quantity);
	}
}
