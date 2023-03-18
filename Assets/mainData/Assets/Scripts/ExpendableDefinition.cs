using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

[Serializable]
[XmlRoot(ElementName = "expendable")]
public class ExpendableDefinition
{
	public enum Categories
	{
		Boosts,
		WorldEffect,
		Internal,
		MysteryBox
	}

	[XmlElement(ElementName = "id")]
	public string OwnableTypeId;

	[XmlElement(ElementName = "name")]
	public string Name;

	[XmlElement(ElementName = "description")]
	public string Description;

	[XmlElement(ElementName = "inventory_icon")]
	public string InventoryIcon;

	[XmlElement(ElementName = "hoverhelp_icon")]
	public string HoverHelpIcon;

	[XmlElement(ElementName = "shopping_icon")]
	public string ShoppingIcon;

	[XmlElement(ElementName = "emotebar_icon")]
	public string EmoteBarIcon;

	[XmlElement(ElementName = "duration")]
	public int Duration;

	[XmlElement(ElementName = "cooldown")]
	public int Cooldown;

	[XmlElement(ElementName = "prereq_method")]
	public string PrerequisiteMethod = string.Empty;

	[XmlElement(ElementName = "confirm_use")]
	public bool ConfirmUse;

	[XmlElement(ElementName = "exclusive")]
	public bool Exclusive = true;

	[XmlArrayItem("category")]
	[XmlArray(ElementName = "sub_category_list")]
	public List<Categories> CategoryList;

	[XmlElement(ElementName = "preeffect_handler")]
	public string PreEffectHandler;

	[XmlElement(ElementName = "expend_effect_handler")]
	public string ExpendEffectHandler;

	[XmlArrayItem("parameter")]
	[XmlArray(ElementName = "parameters")]
	public List<ExpendHandlerParameters> Parameters;

	[XmlArray(ElementName = "combat_effects")]
	[XmlArrayItem("effect")]
	public List<ExpendCombatEffect> CombatEffects;

	public string[] CategoryNames
	{
		get
		{
			string[] array = new string[CategoryList.Count];
			for (int i = 0; i < CategoryList.Count; i++)
			{
				array[i] = CategoryList[i].ToString();
			}
			return array;
		}
	}

	public bool IsValid
	{
		get
		{
			return !string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(Name);
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("ID: " + OwnableTypeId + Environment.NewLine);
		stringBuilder.Append("Name: " + Name + Environment.NewLine);
		stringBuilder.Append("Desc: " + Description + Environment.NewLine);
		stringBuilder.Append("InventoryIcon: " + InventoryIcon + Environment.NewLine);
		stringBuilder.Append("HoverHelpIcon: " + HoverHelpIcon + Environment.NewLine);
		stringBuilder.Append("ShoppingIcon: " + ShoppingIcon + Environment.NewLine);
		stringBuilder.Append("EmoteBarIcon: " + EmoteBarIcon + Environment.NewLine);
		stringBuilder.Append("Duration: " + Duration + Environment.NewLine);
		stringBuilder.Append("Cooldown: " + Cooldown + Environment.NewLine);
		stringBuilder.Append("Categories: " + string.Join(",", CategoryNames) + Environment.NewLine);
		stringBuilder.Append("PreEffectHandler: " + PreEffectHandler + Environment.NewLine);
		stringBuilder.Append("ExpendEffectHandler: " + ExpendEffectHandler + Environment.NewLine);
		foreach (ExpendHandlerParameters parameter in Parameters)
		{
			stringBuilder.AppendLine(" -- " + parameter.Key + ":" + parameter.Value);
		}
		return stringBuilder.ToString();
	}
}
