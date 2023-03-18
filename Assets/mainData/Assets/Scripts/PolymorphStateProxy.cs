public class PolymorphStateProxy
{
	private PolymorphController _owner;

	private PolymorphStateData _data;

	public PolymorphController Owner
	{
		get
		{
			return _owner;
		}
	}

	public PolymorphStateData Data
	{
		get
		{
			return _data;
		}
		set
		{
			_data = value;
		}
	}

	public PolymorphStateProxy(PolymorphController owner)
	{
		_owner = owner;
	}
}
