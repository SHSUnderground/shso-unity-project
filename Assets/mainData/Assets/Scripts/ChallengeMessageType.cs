using System;
using System.Collections.Generic;
using System.Reflection;

public class ChallengeMessageType : Singleton<ChallengeMessageType>
{
	public const string DefaultMessage = "";

	public const string ColliderMessage = "collider_message";

	public const string UIMessage = "ui_message";

	public const string CharacterMessage = "character_message";

	public const string ShoppingMessage = "shopping_message";

	public const string AnimationMessage = "animation_message";

	public const string CardGameMessage = "card_game_message";

	public const string BrawlerMessage = "brawler_message";

	public const string WandererChallengeMessage = "wanderer_message";

	public const string IOMessage = "io_message";

	public const string GameSystemMessage = "gs_message";

	private HashSet<string> _typeSet;

	public ChallengeMessageType()
	{
		_typeSet = new HashSet<string>();
	}

	public void BuildIdSet()
	{
		Type typeFromHandle = typeof(ChallengeMessageType);
		FieldInfo[] fields = typeFromHandle.GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			string text = (string)fieldInfo.GetValue(this);
			if (text != null && !_typeSet.Add(text))
			{
				CspUtils.DebugLog("ChallengeMessageId::BuildIdSet() - public message id string <" + text + "> defined more than once");
			}
		}
	}

	public bool IsType(string type)
	{
		return _typeSet.Contains(type);
	}
}
