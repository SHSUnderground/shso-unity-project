using UnityEngine;

public class SHSLevelUpWindow : GUIDynamicWindow
{
	protected LeveledUpMessage leveledUpMessage;

	protected SHSLeveledUpNotifyWindow leveledUpNotifyWindow;

	protected SHSLevelUpAnimation levelUpAnimation;

	public SHSLevelUpWindow(LeveledUpMessage msg)
	{
		leveledUpMessage = msg;
	}

	public override bool InitializeResources(bool reload)
	{
		leveledUpNotifyWindow = new SHSLeveledUpNotifyWindow(leveledUpMessage);
		leveledUpNotifyWindow.IsVisible = true;
		levelUpAnimation = new SHSLevelUpAnimation();
		levelUpAnimation.IsVisible = true;
		Add(leveledUpNotifyWindow);
		Add(levelUpAnimation);
		SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		AppShell.Instance.EventMgr.AddListener<LeveledUpAwardHiddenMessage>(OnLevelUpWindowHide);
		return base.InitializeResources(reload);
	}

	public override void Hide()
	{
		AppShell.Instance.EventMgr.RemoveListener<LeveledUpAwardHiddenMessage>(OnLevelUpWindowHide);
		base.Hide();
	}

	public override void OnShow()
	{
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("level_up"));
	}

	protected void OnLevelUpWindowHide(LeveledUpAwardHiddenMessage e)
	{
		Hide();
	}
}
