using System.Collections.Generic;
using System.Xml.XPath;

public class CardScenarioDefinition : StaticDataDefinition, IStaticDataDefinition
{
	public string Id;

	public string Name;

	public string Description;

	public string Geometry_Bundle;

	public string Scenario_Bundle;

	public string Sponsor;

	public int SponsorDeckId;

	public List<string> Links;

	public List<CardScenarioRule> Rules;

	public List<CardScenarioRequirement> Requirements;

	public List<CardScenarioReward> Rewards;

	public CardScenarioDefinition()
	{
		Links = new List<string>();
		Rules = new List<CardScenarioRule>();
		Requirements = new List<CardScenarioRequirement>();
		Rewards = new List<CardScenarioReward>();
	}

	public void InitializeFromData(DataWarehouse data)
	{
		XPathNavigator value = data.GetValue(".");
		value.MoveToFirstChild();
		Id = value.LocalName;
		Name = data.GetString("//name");
		Description = data.GetString("//description");
		Geometry_Bundle = data.GetString("//geometry_bundle");
		Scenario_Bundle = data.GetString("//scenario_bundle");
		Sponsor = data.GetString("//sponsor");
		SponsorDeckId = data.GetInt("//sponsordeckId");
		foreach (DataWarehouse item in data.GetIterator("//links/link"))
		{
			Links.Add(item.GetString("text()"));
		}
		foreach (DataWarehouse item2 in data.GetIterator("//customrules/rule"))
		{
			CardScenarioRule cardScenarioRule = new CardScenarioRule();
			cardScenarioRule.Key = item2.GetString("rulekey");
			cardScenarioRule.Value = item2.GetString("rulevalue");
			cardScenarioRule.ConstraintKey = item2.GetString("ruleconstraint");
			Rules.Add(cardScenarioRule);
		}
		foreach (DataWarehouse item3 in data.GetIterator("//requirements/requirement"))
		{
			CardScenarioRequirement cardScenarioRequirement = new CardScenarioRequirement();
			cardScenarioRequirement.RequirementKey = item3.GetString("name");
			cardScenarioRequirement.RequirementValue = item3.GetString("value");
			Requirements.Add(cardScenarioRequirement);
		}
		foreach (DataWarehouse item4 in data.GetIterator("//rewards/reward"))
		{
			CardScenarioReward cardScenarioReward = new CardScenarioReward();
			cardScenarioReward.RewardType = item4.GetString("type");
			cardScenarioReward.Value = item4.GetString("value");
			Rewards.Add(cardScenarioReward);
		}
	}
}
