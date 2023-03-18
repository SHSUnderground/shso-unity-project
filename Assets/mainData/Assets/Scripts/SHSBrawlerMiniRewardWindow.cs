using System.Collections.Generic;
using UnityEngine;

public class SHSBrawlerMiniRewardWindow : GUIDynamicWindow
{
	private const int textBlockOffset = 25;

	private const int rewardBlockOffset = 15;

	private const float awardIconShrinkPercentage = 0.75f;

	private const float buttonShrinkPercentage = 0.75f;

	private const float okButtonOffset = 25f;

	private const string TOP_LABEL_TEXT = "#MINI_REWARD_TITLE_CONGRATS";

	private const string AWARD_TEXT = "#MINI_REWARD_TEXT";

	private const float TIMEOUT = 12f;

	private string ownableTypeId;

	private GUIImage backgroundLeft;

	private GUIImage backgroundRight;

	private GUIImage backgroundMiddle;

	private GUILabel topLabel;

	private GUILabel awardLabel;

	private GUILabel awardNameLabel;

	private GUIImage awardIcon;

	private GUIButton okButton;

	private readonly Vector2 OWNABLE_ICON_SIZE = new Vector2(48f, 48f);

	private float timeoutTime;

	public SHSBrawlerMiniRewardWindow(string ownableTypeId)
	{
		this.ownableTypeId = ownableTypeId;
	}

	public override bool InitializeResources(bool reload)
	{
		if (reload)
		{
			return base.InitializeResources(reload);
		}
		List<GUILabel> list = new List<GUILabel>();
		awardLabel = new GUILabel();
		awardLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
		awardLabel.Text = "#MINI_REWARD_TEXT";
		list.Add(awardLabel);
		awardIcon = new GUIImage();
		awardIcon.SetSize(OWNABLE_ICON_SIZE);
		if (!string.IsNullOrEmpty(ownableTypeId))
		{
			ItemDefinition value = null;
			AppShell.Instance.ItemDictionary.TryGetValue(ownableTypeId, out value);
			if (value == null)
			{
				ExpendableDefinition value2 = null;
				AppShell.Instance.ExpendablesManager.ExpendableTypes.TryGetValue(ownableTypeId, out value2);
				if (value2 == null)
				{
					CspUtils.DebugLog("SHSBrawlerMiniRewardWindow: Ownable ID invalid; exiting stage.");
					AppShell.Instance.EventMgr.Fire(this, new BrawlerSummaryCompleteMessage());
					return base.InitializeResources(reload);
				}
			}
		}
		GUIContent gUIContent = new GUIContent();
		topLabel = new GUILabel();
		topLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 239, 104), TextAnchor.UpperCenter);
		topLabel.Text = "#MINI_REWARD_TITLE_CONGRATS";
		topLabel.IsVisible = true;
		list.Add(topLabel);
		backgroundLeft = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(57f, 192f), Vector2.zero);
		backgroundLeft.SetPosition(new Vector2(0f, 0f));
		backgroundLeft.TextureSource = "notification_bundle|mshs_levelup_window_left";
		float textBlockSize = GetTextBlockSize(list);
		Vector2 size = awardIcon.Size;
		backgroundMiddle = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(textBlockSize + size.x, 192f), Vector2.zero);
		GUIImage gUIImage = backgroundMiddle;
		Vector2 position = backgroundLeft.Position;
		float x = position.x;
		Vector2 size2 = backgroundLeft.Size;
		float x2 = x + size2.x;
		Vector2 position2 = backgroundLeft.Position;
		gUIImage.SetPosition(new Vector2(x2, position2.y));
		backgroundMiddle.TextureSource = "notification_bundle|mshs_levelup_window_center";
		backgroundRight = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(54f, 192f), Vector2.zero);
		GUIImage gUIImage2 = backgroundRight;
		Vector2 position3 = backgroundMiddle.Position;
		float x3 = position3.x;
		Vector2 size3 = backgroundMiddle.Size;
		float x4 = x3 + size3.x;
		Vector2 position4 = backgroundMiddle.Position;
		gUIImage2.SetPosition(new Vector2(x4, position4.y));
		backgroundRight.TextureSource = "notification_bundle|mshs_levelup_window_right";
		okButton = new GUIButton();
		okButton.SetSize(new Vector2(96f, 96f));
		GUIButton gUIButton = okButton;
		Vector2 position5 = backgroundMiddle.Position;
		float x5 = position5.x;
		Vector2 size4 = backgroundMiddle.Size;
		float num = x5 + size4.x * 0.5f;
		Vector2 size5 = okButton.Size;
		float x6 = num - size5.x * 0.5f;
		Vector2 position6 = backgroundLeft.Position;
		float y = position6.y;
		Vector2 size6 = backgroundLeft.Size;
		float num2 = y + size6.y;
		Vector2 size7 = okButton.Size;
		gUIButton.SetPosition(new Vector2(x6, num2 - size7.y + 25f));
		okButton.StyleInfo = new SHSButtonStyleInfo("notification_bundle|simple_ok_button");
		okButton.Click += delegate
		{
			Hide();
		};
		gUIContent.text = topLabel.Text;
		Vector2 vector = topLabel.Style.UnityStyle.CalcSize(gUIContent);
		GUILabel gUILabel = topLabel;
		Vector2 position7 = backgroundLeft.Position;
		float x7 = position7.x;
		Vector2 size8 = backgroundLeft.Size;
		float num3 = x7 + size8.x;
		Vector2 size9 = backgroundMiddle.Size;
		float x8 = num3 + (size9.x * 0.5f - vector.x * 0.5f);
		Vector2 position8 = backgroundLeft.Position;
		gUILabel.Position = new Vector2(x8, position8.y + 25f);
		topLabel.SetSize(new Vector2(vector.x, vector.y));
		Vector2 position9 = backgroundLeft.Position;
		float x9 = position9.x;
		Vector2 size10 = backgroundLeft.Size;
		float x10 = x9 + size10.x;
		Vector2 position10 = backgroundLeft.Position;
		Vector2 vector2 = new Vector2(x10, position10.y + 25f);
		awardLabel.Position = new Vector2(105f, 60f);
		gUIContent.text = awardLabel.Text;
		vector = awardLabel.Style.UnityStyle.CalcSize(gUIContent);
		awardLabel.SetSize(vector.x, vector.y);
		awardIcon.Position = new Vector2(vector2.x, vector2.y + vector.y + 15f);
		Add(backgroundLeft);
		Add(backgroundMiddle);
		Add(backgroundRight);
		Add(topLabel);
		Add(awardIcon);
		Add(awardLabel);
		Add(okButton);
		Vector2 offset = new Vector2(175f, 75f);
		Vector2 size11 = backgroundLeft.Size;
		float x11 = size11.x;
		Vector2 size12 = backgroundMiddle.Size;
		float num4 = x11 + size12.x;
		Vector2 size13 = backgroundRight.Size;
		float x12 = num4 + size13.x;
		Vector2 size14 = backgroundLeft.Size;
		SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, offset, new Vector2(x12, size14.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		timeoutTime = Time.time + 12f;
		return base.InitializeResources(reload);
	}

	public override void Update()
	{
		if (Time.time > timeoutTime)
		{
			Hide();
		}
	}

	public override void Hide()
	{
		AppShell.Instance.EventMgr.Fire(this, new BrawlerSummaryCompleteMessage());
		base.Hide();
	}

	protected float GetTextBlockSize(List<GUILabel> textLabels)
	{
		float num = 0f;
		foreach (GUILabel textLabel in textLabels)
		{
			Vector2 vector = textLabel.Style.UnityStyle.CalcSize(new GUIContent(textLabel.Text));
			if (num < vector.x)
			{
				num = vector.x;
			}
		}
		return num;
	}
}
