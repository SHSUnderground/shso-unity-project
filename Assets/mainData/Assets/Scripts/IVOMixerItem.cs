public interface IVOMixerItem
{
	ResolvedVOAction Action
	{
		get;
	}

	bool Started
	{
		get;
	}

	bool Finished
	{
		get;
	}

	void Play();

	void Cancel();
}
