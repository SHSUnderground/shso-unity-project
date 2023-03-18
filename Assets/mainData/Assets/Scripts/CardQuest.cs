using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

public class CardQuest : IComparable<CardQuest>
{
	public const int RequiredQuestPartCount = 2;

	public const int PartOneIndex = 1;

	public const int PartTwoIndex = 4;

	protected List<CardQuestPart> parts;

	[CompilerGenerated]
	private string _003CSponsor_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CName_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CIsValid_003Ek__BackingField;

	public string Sponsor
	{
		[CompilerGenerated]
		get
		{
			return _003CSponsor_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CSponsor_003Ek__BackingField = value;
		}
	}

	public string Name
	{
		[CompilerGenerated]
		get
		{
			return _003CName_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CName_003Ek__BackingField = value;
		}
	}

	public bool IsValid
	{
		[CompilerGenerated]
		get
		{
			return _003CIsValid_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CIsValid_003Ek__BackingField = value;
		}
	}

	public List<CardQuestPart> Parts
	{
		get
		{
			return parts;
		}
		private set
		{
			parts = value;
		}
	}

	public CardQuest(string gatekeeper)
	{
		parts = new List<CardQuestPart>(2);
		Sponsor = gatekeeper;
		IsValid = false;
	}

	public void AddQuestPart(CardQuestPart cardQuestPart)
	{
		if (IsValid)
		{
			CspUtils.DebugLog("Adding a quest part to an already configured quest part");
			return;
		}
		if (cardQuestPart.Nodes[0].Stage < 4)
		{
			parts.Insert(0, cardQuestPart);
			cardQuestPart.PartType = CardQuestPartsTypeEnum.Easy;
		}
		else if (cardQuestPart.Nodes[0].Stage >= 1)
		{
			parts.Insert(1, cardQuestPart);
			cardQuestPart.PartType = CardQuestPartsTypeEnum.Hard;
		}
		else
		{
			CspUtils.DebugLog("Invalid card quest part passed into card quest Add method." + cardQuestPart.Nodes[0].Id);
		}
		cardQuestPart.ParentQuest = this;
		Name = cardQuestPart.Subname2;
		IsValid = (parts.Count == 2 && parts[0] != null && parts[0].IsValid && parts[1] != null && parts[1].IsValid);
	}

	public int CompareTo(CardQuest other)
	{
		return Sponsor.CompareTo(other.Sponsor);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Quest Sponsor:" + Sponsor + Environment.NewLine);
		foreach (CardQuestPart part in Parts)
		{
			stringBuilder.Append(part.ToString());
		}
		stringBuilder.Append("IsValid:" + IsValid + Environment.NewLine + Environment.NewLine);
		return stringBuilder.ToString();
	}
}
