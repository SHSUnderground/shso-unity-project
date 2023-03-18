using UnityEngine;

public class SHSMySquadRewardIcon : GUISimpleControlWindow
{
	protected GUIImage rewardIcon;

	protected GUIImage rewardText;

	protected LargeSerumAnimation serumAnimation;

	public ChallengeInfo Challenge
	{
		set
		{
			if (value.Reward.grantMode == ChallengeGrantMode.Manual)
			{
				serumAnimation.IsVisible = true;
				serumAnimation.BeginSerumDrip();
				rewardIcon.IsVisible = false;
				rewardText.IsVisible = false;
			}
			else if (value.Reward.rewardType == ChallengeRewardType.Hero)
			{
				rewardIcon.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 7f), new Vector2(115f, 115f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				rewardIcon.TextureSource = "characters_bundle|expandedtooltip_render_" + value.Reward.qualifier;
				rewardIcon.IsVisible = true;
				rewardText.IsVisible = true;
				serumAnimation.IsVisible = false;
			}
			else
			{
				rewardIcon.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(132f, 132f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				rewardIcon.TextureSource = value.IconPath;
				rewardIcon.IsVisible = true;
				rewardText.IsVisible = true;
				serumAnimation.IsVisible = false;
			}
		}
	}

	public SHSMySquadRewardIcon()
	{
		SetSize(200f, 200f);
		rewardIcon = new GUIImage();
		rewardIcon.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		rewardIcon.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(200f, 200f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Add(rewardIcon);
		rewardText = new GUIImage();
		rewardText.Id = "rewardText";
		rewardText.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(5f, -55f), new Vector2(128f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		rewardText.TextureSource = "mysquadgadget_bundle|L_mshs_mysquad_challenge_p1_reward_label";
		Add(rewardText);
		serumAnimation = new LargeSerumAnimation();
		serumAnimation.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		serumAnimation.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 13f), new Vector2(200f, 200f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Add(serumAnimation);
	}
}
