public class PrizeWheel : PrizeWheelBase
{
	protected PrizeWheelManager.WheelTypeEnum wheelType;

	protected PrizeWheelManager.CurrencyEnum supportingCurrency;

	protected PrizeWheelManager manager;

	protected int xmlId;

	public PrizeWheelManager.WheelTypeEnum WheelType
	{
		get
		{
			return wheelType;
		}
		set
		{
			wheelType = value;
		}
	}

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

	public PrizeWheelManager Manager
	{
		get
		{
			return manager;
		}
	}

	public int XmlId
	{
		get
		{
			return xmlId;
		}
		set
		{
			xmlId = value;
		}
	}

	public PrizeWheel(PrizeWheelManager manager)
	{
		this.manager = manager;
	}
}
