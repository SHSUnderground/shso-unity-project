using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

[XmlRoot(ElementName = "menuchatgroup")]
public class MenuChatGroup
{
	[XmlIgnore]
	public MenuChatGroup Parent;

	[XmlIgnore]
	public bool IsRoot;

	[XmlElement(ElementName = "entryid")]
	public int Id;

	[XmlElement(ElementName = "phrasekey")]
	public string PhraseKey;

	[DefaultValue("")]
	[XmlElement(ElementName = "emoteid")]
	public string EmoteId;

	protected List<MenuChatGroup> menuchatgroups;

	[XmlArrayItem("menuchatgroup")]
	[XmlArray(ElementName = "menuchatgroups")]
	public List<MenuChatGroup> MenuChatGroups
	{
		get
		{
			return menuchatgroups;
		}
		set
		{
			menuchatgroups = value;
		}
	}

	public MenuChatGroup()
	{
		MenuChatGroups = new List<MenuChatGroup>();
	}

	public void ToTree()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("*ROOT*" + Environment.NewLine);
		ToTree(stringBuilder, 0);
		CspUtils.DebugLog(stringBuilder.ToString());
	}

	public void ToTree(StringBuilder sb, int level)
	{
		for (int i = 0; i < level; i++)
		{
			sb.Append("\t");
		}
		sb.Append(string.Format("{0}: {1} ({2}){3}", Id, PhraseKey, EmoteId, Environment.NewLine));
		for (int j = 0; j < menuchatgroups.Count; j++)
		{
			menuchatgroups[j].ToTree(sb, level + 1);
		}
	}

	public void WireUpLineage()
	{
		foreach (MenuChatGroup menuChatGroup in MenuChatGroups)
		{
			menuChatGroup.Parent = this;
			menuChatGroup.WireUpLineage();
		}
	}
}
