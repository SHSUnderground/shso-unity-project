using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NPCCommandHint
{
	public static Dictionary<NPCCommandTypeEnum, Type> CommandList;

	public NPCCommandTypeEnum NPCCommand;

	public GameObject target;

	public string storedProperties;

	[HideInInspector]
	public SerializedKeyValueContainer storedPropertiesContainer;

	public NPCCommandHint()
	{
		storedPropertiesContainer = new SerializedKeyValueContainer();
		storedPropertiesContainer.OnContainerInvoked += storedPropertiesContainer_OnContainerInvoked;
		storedPropertiesContainer.OnContainerChanged += storedPropertiesContainer_OnContainerChanged;
	}

	static NPCCommandHint()
	{
		CommandList = new Dictionary<NPCCommandTypeEnum, Type>();
		CommandList[NPCCommandTypeEnum.None] = null;
		CommandList[NPCCommandTypeEnum.Start] = typeof(NPCStartCommand);
		CommandList[NPCCommandTypeEnum.MoveToCurrentNode] = typeof(NPCMoveToCurrentNodeCommand);
		CommandList[NPCCommandTypeEnum.MoveToNextNode] = typeof(NPCMoveToNextNodeCommand);
		CommandList[NPCCommandTypeEnum.Wander] = typeof(NPCWanderCommand);
		CommandList[NPCCommandTypeEnum.Emote] = typeof(NPCEmoteCommand);
		CommandList[NPCCommandTypeEnum.Delay] = typeof(NPCDelayCommand);
		CommandList[NPCCommandTypeEnum.React] = typeof(NPCReactCommand);
		CommandList[NPCCommandTypeEnum.Derez] = typeof(NPCDerezCommand);
		CommandList[NPCCommandTypeEnum.TurnTo] = typeof(NPCTurnToCommand);
	}

	private void storedPropertiesContainer_OnContainerChanged()
	{
		storedProperties = storedPropertiesContainer.PropertyString;
	}

	private void storedPropertiesContainer_OnContainerInvoked()
	{
		storedPropertiesContainer.PropertyString = storedProperties;
	}
}
