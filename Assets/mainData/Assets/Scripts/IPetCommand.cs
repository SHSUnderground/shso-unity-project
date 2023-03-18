public interface IPetCommand
{
	int CommandSet
	{
		get;
		set;
	}

	bool Suspended
	{
		get;
	}

	bool Interruptable
	{
		get;
	}

	bool Initialized
	{
		get;
	}

	PetCommandTypeEnum Type
	{
		get;
	}

	void Init(PetCommandManager manager);

	void Start();

	void Start(params object[] initValues);

	PetCommandResultEnum Update();

	void Suspend();

	void Resume();

	void Completed();

	new string ToString();
}
