using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetCommandManager : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected int commandSetCounter;

	protected IPetCommand currentCommand;

	private int historyBufferSize = 50;

	protected List<KeyValuePair<bool, IPetCommand>> commandHistory;

	protected List<KeyValuePair<int, string>> problemHistory;

	protected List<IPetCommand> commandList;

	public bool mustHaveCommands = true;

	public PetData petData;

	public GameObject target;

	public bool isLocalPlayerPet;

	private static PetCommandResultEnum result;

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

	public IPetCommand CurrentCommand
	{
		get
		{
			return currentCommand;
		}
	}

	public List<KeyValuePair<bool, IPetCommand>> CommandHistory
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

	public List<IPetCommand> CommandList
	{
		get
		{
			return commandList;
		}
	}

	private void Awake()
	{
		commandList = new List<IPetCommand>();
		commandHistory = new List<KeyValuePair<bool, IPetCommand>>();
		problemHistory = new List<KeyValuePair<int, string>>();
		if (SocialSpaceController.Instance != null && target == SocialSpaceController.Instance.LocalPlayer)
		{
			CspUtils.DebugLog("PetCommandManager is local pet");
			isLocalPlayerPet = true;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (currentCommand != null)
		{
			result = currentCommand.Update();
			if (result == PetCommandResultEnum.Completed || result == PetCommandResultEnum.Failed)
			{
				nextCommand();
			}
		}
	}

	private void nextCommand()
	{
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
			PetWaitForCharacterMoveCommand petWaitForCharacterMoveCommand = new PetWaitForCharacterMoveCommand();
			petWaitForCharacterMoveCommand.target = target;
			petWaitForCharacterMoveCommand.fidgetDelay = petData.idleTimeRange;
			petWaitForCharacterMoveCommand.fidgetData = petData.getRandomFidget();
			petWaitForCharacterMoveCommand.pigeonDeathHack = petData.pigeonDeathHack;
			AddCommand(petWaitForCharacterMoveCommand, true);
		}
	}

	public void AddCommand(IPetCommand command)
	{
		AddCommand(command, false);
	}

	public void AddCommand(IPetCommand command, bool interrupt)
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

	public void ForceCommand(IPetCommand command)
	{
		commandList.Clear();
		currentCommand = null;
		AddCommand(command, false);
	}

	private void addCommand(int index, IPetCommand command)
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
		commandHistory.Add(new KeyValuePair<bool, IPetCommand>(index == 0, command));
		if (commandHistory.Count > historyBufferSize)
		{
			commandHistory.RemoveAt(0);
		}
	}

	public void InsertQueuedCommand(IPetCommand command)
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

	public void spawned()
	{
		StartCoroutine(resetActivityObjects());
	}

	protected IEnumerator resetActivityObjects()
	{
		yield return new WaitForSeconds(1f);
		CspUtils.DebugLog("changeCurrentPet checking activity objects");
		Object[] aos = Object.FindSceneObjectsOfType(typeof(ActivityObject));
		Object[] array = aos;
		for (int i = 0; i < array.Length; i++)
		{
			ActivityObject ao = (ActivityObject)array[i];
			ao.ResetForNewPet(petData);
		}
	}
}
