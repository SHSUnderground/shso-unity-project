public class SHSCounterBank
{
	protected bool readOnly;

	protected long id = -2147483648L;

	public bool ReadOnly
	{
		get
		{
			return readOnly;
		}
		set
		{
			readOnly = value;
		}
	}

	public long Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public SHSCounterBank()
	{
	}

	public SHSCounterBank(long id)
	{
		this.id = id;
	}
}
