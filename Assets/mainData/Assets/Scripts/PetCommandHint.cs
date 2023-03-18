using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PetCommandHint
{
	public static Dictionary<PetCommandTypeEnum, Type> CommandList;

	public PetCommandTypeEnum PetCommand;

	public GameObject target;

	public string storedProperties;

	[HideInInspector]
	public SerializedKeyValueContainer storedPropertiesContainer;

	public PetCommandHint()
	{
		storedPropertiesContainer = new SerializedKeyValueContainer();
		storedPropertiesContainer.OnContainerInvoked += storedPropertiesContainer_OnContainerInvoked;
		storedPropertiesContainer.OnContainerChanged += storedPropertiesContainer_OnContainerChanged;
	}

	static PetCommandHint()
	{
		CommandList = new Dictionary<PetCommandTypeEnum, Type>();
		CommandList[PetCommandTypeEnum.None] = null;
		CommandList[PetCommandTypeEnum.FollowCharacter] = typeof(PetFollowCharacterCommand);
		CommandList[PetCommandTypeEnum.WaitForCharacterMove] = typeof(PetWaitForCharacterMoveCommand);
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
