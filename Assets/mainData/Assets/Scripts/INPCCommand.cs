public interface INPCCommand
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

	NPCCommandTypeEnum Type
	{
		get;
	}

	void Init(NPCCommandManager manager);

	void Start();

	void Start(params object[] initValues);

	NPCCommandResultEnum Update();

	void Suspend();

	void Resume();

	void Completed();

	new string ToString();
}
