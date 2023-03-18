using UnityEngine;

public class AchievementJumpActionContainer : GUISimpleControlWindow
{
	public delegate void JumpActionClickDelegate();

	private NewAchievement _targetAchievement;

	private string jumpActionMissionName = string.Empty;

	public event JumpActionClickDelegate OnJumpActionClickEvent;

	public AchievementJumpActionContainer(NewAchievement achievement)
	{
		_targetAchievement = achievement;
		GUIButton launchbutton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(120f, 40f), Vector2.zero);
		launchbutton.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|widegoldbutton", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		launchbutton.IsVisible = true;
		Add(launchbutton);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, launchbutton.Position + new Vector2(0f, -5f), launchbutton.Size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 226, 90), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel.BackColorAlpha = 1f;
		gUIStrokeTextLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUIStrokeTextLabel.IsVisible = true;
		Add(gUIStrokeTextLabel);
		bool flag = false;
		string[] array = _targetAchievement.jumpAction.Split(',');
		if (array[0] == "mission")
		{
			if (array.Length <= 1)
			{
				CspUtils.DebugLog("Got a jumpAction of " + _targetAchievement.jumpAction + " but was missing the mission name");
			}
			else
			{
				jumpActionMissionName = array[1];
				flag = true;
				launchbutton.Click += delegate
				{
					if (this.OnJumpActionClickEvent != null)
					{
						this.OnJumpActionClickEvent();
					}
					new JumpActionExecutor(_targetAchievement.jumpAction);
				};
				gUIStrokeTextLabel.Text = "#ACH_LAUNCH_PLAY";
			}
		}
		else if (array[0] == "craft")
		{
			if (array.Length <= 1)
			{
				CspUtils.DebugLog("Got a jumpAction of " + _targetAchievement.jumpAction + " but was missing the ownable ID");
			}
			else
			{
				int num = int.Parse(array[1]);
				flag = true;
				launchbutton.Click += delegate
				{
					if (this.OnJumpActionClickEvent != null)
					{
						this.OnJumpActionClickEvent();
					}
					new JumpActionExecutor(_targetAchievement.jumpAction);
				};
				gUIStrokeTextLabel.Text = "#ACH_LAUNCH_CRAFT";
			}
		}
		if (launchbutton != null)
		{
			launchbutton.Click += delegate
			{
				AppShell.Instance.disableControl(launchbutton);
			};
		}
		if (!flag)
		{
			Remove(launchbutton);
			Remove(gUIStrokeTextLabel);
		}
		SetSize(launchbutton.Size);
	}
}
