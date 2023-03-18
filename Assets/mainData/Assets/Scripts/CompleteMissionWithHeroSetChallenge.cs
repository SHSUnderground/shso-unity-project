using System.Collections.Generic;

public class CompleteMissionWithHeroSetChallenge : ChallengeBitCounter
{
	private string _eventTrigger;

	public bool IsTrackedEvent(string evt)
	{
		return evt == _eventTrigger;
	}

	private bool ContainsAllTrackedHeroes(string[] heroNames)
	{
		if (heroNames == null || heroNames.Length != objectToBitMap.Count)
		{
			return false;
		}
		foreach (string obj in heroNames)
		{
			if (!HasBit(obj))
			{
				return false;
			}
		}
		return true;
	}

	protected override void OnClientChallengeEvent(object[] data)
	{
		if (data != null && data.Length != 0 && data[0] != null && IsTrackedEvent(data[0].ToString()))
		{
			List<BrawlerStatManager.CharacterScoreData> allStatBlocks = BrawlerStatManager.instance.GetAllStatBlocks();
			if (allStatBlocks != null && allStatBlocks.Count > 0)
			{
				foreach (BrawlerStatManager.CharacterScoreData item in allStatBlocks)
				{
					if (!IsBitSet(item.modelName))
					{
						SetBit(item.modelName);
					}
					if (IsClientChallengeMet())
					{
						break;
					}
				}
			}
			else if (!IsBitSet(AppShell.Instance.Profile.SelectedCostume))
			{
				SetBit(AppShell.Instance.Profile.SelectedCostume);
			}
		}
	}

	public override void Initialize(ChallengeManager manager, ChallengeInfo info, ISHSCounterType counter, ChallengeManager.ChallengeCompleteDelegate onChallengeComplete)
	{
		base.Initialize(manager, info, counter, onChallengeComplete);
		foreach (ChallengeInfoParameters parameter in info.Parameters)
		{
			if (parameter.key == "event_trigger")
			{
				_eventTrigger = parameter.value;
			}
			if (parameter.key == "hero_trigger")
			{
				AssignBit(parameter.value);
			}
		}
	}
}
