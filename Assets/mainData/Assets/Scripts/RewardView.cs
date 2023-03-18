using UnityEngine;

public class RewardView : GUISimpleControlWindow
{
	protected GUIDropShadowTextLabel rewardValue;

	protected GUIImage rewardIcon;

	protected GUIStrokeTextLabel rewardLabel;

	protected SmallSerumAnimation serumAnimation;

	public int ChallengeId
	{
		set
		{
			if (!AppShell.Instance.ChallengeManager.ChallengeDictionary.ContainsKey(value))
			{
				return;
			}
			ChallengeInfo challengeInfo = AppShell.Instance.ChallengeManager.ChallengeDictionary[value];
			if (challengeInfo.Reward.grantMode == ChallengeGrantMode.Auto)
			{
				if (challengeInfo.Reward.rewardType == ChallengeRewardType.Hero)
				{
					rewardValue.Text = AppShell.Instance.CharacterDescriptionManager[challengeInfo.Reward.qualifier].CharacterName;
					rewardIcon.SetPositionAndSize(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(-20f, 0f), new Vector2(82f, 82f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
					rewardIcon.TextureSource = "characters_bundle|token_" + challengeInfo.Reward.qualifier;
					return;
				}
				string value2 = challengeInfo.Reward.value;
				string id = string.Empty;
				if (challengeInfo.Reward.rewardType == ChallengeRewardType.Gold)
				{
					id = "#DAILY_REWARDS_GOLD";
				}
				else if (challengeInfo.Reward.rewardType == ChallengeRewardType.Silver)
				{
					id = "#DAILY_REWARD_SILVER";
				}
				else if (challengeInfo.Reward.rewardType == ChallengeRewardType.Tickets)
				{
					id = "#DAILY_REWARD_TICKETS";
				}
				rewardValue.Text = string.Format(AppShell.Instance.stringTable[id], value2);
				rewardIcon.TextureSource = string.Format("{0}_small", challengeInfo.IconPath);
			}
			else
			{
				rewardIcon.IsVisible = false;
				rewardValue.Text = "#SQ_SERUM_CHALLENGE_REWARD";
				serumAnimation = new SmallSerumAnimation();
				serumAnimation.SetPositionAndSize(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(-40f, 0f), new Vector2(58f, 70f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				Add(serumAnimation);
				serumAnimation.BeginSerumDrip();
			}
		}
	}

	public RewardView(int challengeId)
	{
		rewardLabel = new GUIStrokeTextLabel();
		rewardLabel.Id = "rewardLabel";
		rewardLabel.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -12f), new Vector2(200f, 25f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		rewardLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(195, 222, 255), GUILabel.GenColor(4, 30, 77), GUILabel.GenColor(4, 30, 77), new Vector2(0f, 0f), TextAnchor.MiddleLeft);
		rewardLabel.BackColorAlpha = 0f;
		rewardLabel.Bold = true;
		rewardLabel.Text = "#SQ_CHALLENGE_REWARD";
		Add(rewardLabel);
		rewardValue = new GUIDropShadowTextLabel();
		rewardValue.Id = "rewardValue";
		rewardValue.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-25f, 8f), new Vector2(150f, 25f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		rewardValue.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(195, 223, 255), GUILabel.GenColor(0, 26, 67), new Vector2(1f, 1f), TextAnchor.MiddleLeft);
		rewardValue.BackColorAlpha = 0f;
		rewardValue.Bold = true;
		rewardValue.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		Add(rewardValue);
		rewardIcon = new GUIImage();
		rewardIcon.Id = "rewardIcon";
		rewardIcon.SetPositionAndSize(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(130f, 130f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Add(rewardIcon);
		ChallengeId = challengeId;
	}

	public override void OnShow()
	{
		base.OnShow();
		if (serumAnimation != null)
		{
			serumAnimation.BeginSerumDrip();
		}
	}
}
