using System.Collections.Generic;
using UnityEngine;

public class NPCCommandManager : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected int commandSetCounter;

	protected INPCCommand currentCommand;

	private int historyBufferSize = 50;

	protected List<KeyValuePair<bool, INPCCommand>> commandHistory;

	protected List<KeyValuePair<int, string>> problemHistory;

	protected List<INPCCommand> commandList;

	public bool mustHaveCommands = true;

	private static NPCCommandResultEnum result;

	public int CommandSetCounter
	{
		get
		{
			return commandSetCounter;
		}
		set
		{
			commandSetCounter = value;
		}
	}

	public INPCCommand CurrentCommand
	{
		get
		{
			return currentCommand;
		}
	}

	public List<KeyValuePair<bool, INPCCommand>> CommandHistory
	{
		get
		{
			return commandHistory;
		}
	}

	public List<KeyValuePair<int, string>> ProblemHistory
	{
		get
		{
			return problemHistory;
		}
	}

	public List<INPCCommand> CommandList
	{
		get
		{
			return commandList;
		}
	}

	private void Awake()
	{
		commandList = new List<INPCCommand>();
		commandHistory = new List<KeyValuePair<bool, INPCCommand>>();
		problemHistory = new List<KeyValuePair<int, string>>();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (currentCommand != null)
		{
			result = currentCommand.Update();
			if (result == NPCCommandResultEnum.Completed || result == NPCCommandResultEnum.Failed)
			{
				nextCommand();
			}
		}
	}

	private void nextCommand()
	{
		INPCCommand iNPCCommand = currentCommand;
		commandList.Remove(currentCommand);
		currentCommand = null;
		if (commandList.Count > 0)
		{
			currentCommand = commandList[0];
			if (currentCommand.Suspended)
			{
				problemHistory.Add(new KeyValuePair<int, string>(CurrentCommand.CommandSet, CurrentCommand.ToString() + " Resuming"));
				currentCommand.Resume();
			}
			else
			{
				currentCommand.Start();
			}
		}
		else
		{
			if (!mustHaveCommands)
			{
				return;
			}
			problemHistory.Add(new KeyValuePair<int, string>(iNPCCommand.CommandSet, " Empty of commands!"));
			if (Application.isEditor)
			{
				if (base.gameObject.GetComponent<ColorShifter>() == null)
				{
					ColorShifter colorShifter = base.gameObject.AddComponent<ColorShifter>();
					if (colorShifter != null)
					{
						colorShifter.startColor = Color.red;
						colorShifter.endColor = Color.black;
					}
				}
			}
			else
			{
				NPCDerezCommand nPCDerezCommand = new NPCDerezCommand();
				nPCDerezCommand.respawn = true;
				nPCDerezCommand.rezDelay = 5f;
				AddCommand(nPCDerezCommand);
			}
		}
	}

	public void AddCommand(INPCCommand command)
	{
		AddCommand(command, false);
	}

	public void AddCommand(INPCCommand command, bool interrupt)
	{
		if (interrupt)
		{
			if (CurrentCommand != null)
			{
				if (!CurrentCommand.Interruptable)
				{
					CspUtils.DebugLog("Current command is not interruptable.");
					return;
				}
				currentCommand.Suspend();
				problemHistory.Add(new KeyValuePair<int, string>(CurrentCommand.CommandSet, CurrentCommand.ToString() + " Interrupted"));
			}
			addCommand(0, command);
			currentCommand = command;
			currentCommand.Start();
		}
		else
		{
			addCommand(-1, command);
			if (commandList.Count == 1)
			{
				command.Start();
				currentCommand = command;
			}
		}
	}

	public void ForceCommand(INPCCommand command)
	{
		commandList.Clear();
		currentCommand = null;
		AddCommand(command, false);
	}

	private void addCommand(int index, INPCCommand command)
	{
		if (!command.Initialized)
		{
			command.Init(this);
		}
		if (index >= 0)
		{
			commandList.Insert(index, command);
		}
		else
		{
			commandList.Add(command);
		}
		command.CommandSet = CommandSetCounter;
		commandHistory.Add(new KeyValuePair<bool, INPCCommand>(index == 0, command));
		if (commandHistory.Count > historyBufferSize)
		{
			commandHistory.RemoveAt(0);
		}
	}

	public void InsertQueuedCommand(INPCCommand command)
	{
		if (commandList.Count == 0)
		{
			AddCommand(command);
		}
		else
		{
			addCommand(1, command);
		}
	}
}
