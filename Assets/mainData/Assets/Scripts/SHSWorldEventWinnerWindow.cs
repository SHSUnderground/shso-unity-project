using System;
using UnityEngine;

public class SHSWorldEventWinnerWindow : GUIDynamicWindow
{
	protected GUIImage bgImage;

	protected GUIImage contratulationsTextImage;

	protected GUIImage heroImage;

	protected GUIButton okButton;

	protected GUIStrokeTextLabel _text;

	protected bool _topScore;

	protected int _rewardID;

	public SHSWorldEventWinnerWindow(int rewardID, bool topScore)
	{
		_topScore = topScore;
		_rewardID = rewardID;
	}

	public override bool InitializeResources(bool reload)
	{
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Expected O, but got Unknown
		SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero, new Vector2(593f, 356f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		bgImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(593f, 356f), new Vector2(0f, 0f));
		bgImage.TextureSource = "common_bundle|world_event_congratulations_box";
		Add(bgImage);
		OwnableDefinition def = OwnableDefinition.getDef(_rewardID);
		heroImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(153f, 163f), new Vector2(15f, 120f));
		heroImage.TextureSource = def.shoppingIcon;
		heroImage.IsVisible = false;
		Add(heroImage);
		contratulationsTextImage = GUIControl.CreateControlTopFrameCentered<GUIImage>(new Vector2(402f, 58f), new Vector2(10f, 60f));
		contratulationsTextImage.TextureSource = "common_bundle|L_congratulations_title";
		contratulationsTextImage.IsVisible = false;
		Add(contratulationsTextImage);
		okButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(38f, 146f));
		okButton.Click += delegate
		{
			onClick();
		};
		okButton.HitTestSize = new Vector2(0.5f, 0.5f);
		okButton.HitTestType = HitTestTypeEnum.Circular;
		okButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_mshs_button_ok");
		okButton.IsVisible = false;
		Add(okButton);
		_text = new GUIStrokeTextLabel();
		_text.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 24, GUILabel.GenColor(214, 214, 214), GUILabel.GenColor(23, 23, 23), GUILabel.GenColor(23, 23, 23), new Vector2(3f, 3f), TextAnchor.MiddleCenter);
		_text.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(190f, 70f), new Vector2(250f, 250f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		string text = def.name;
		if (def.category == OwnableDefinition.Category.Hero)
		{
			text = string.Format("#CIN_{0}_EXNM", text.ToUpper());
		}
		_text.Text = string.Format(AppShell.Instance.stringTable["#WORLD_EVENT_WIN_" + ((!_topScore) ? "RANDOM" : "TOPSCORE")], AppShell.Instance.stringTable[text]);
		_text.IsVisible = false;
		Add(_text);
		AnimClip animClip = AnimClipBuilder.Absolute.SizeX(AnimClipBuilder.Path.Linear(0f, 593f, 1f), bgImage) ^ AnimClipBuilder.Absolute.SizeY(AnimClipBuilder.Path.Linear(0f, 356f, 1f), bgImage) ^ AnimClipBuilder.Absolute.OffsetX(AnimClipBuilder.Path.Linear(190f, 0f, 1f), this);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			heroImage.IsVisible = true;
			contratulationsTextImage.IsVisible = true;
			okButton.IsVisible = true;
			_text.IsVisible = true;
		};
		base.AnimationPieceManager.Add(animClip);
		return base.InitializeResources(reload);
	}

	protected void onClick()
	{
		Hide();
	}

	protected override void dispose(bool disposing)
	{
		base.dispose(disposing);
		bgImage = null;
		contratulationsTextImage = null;
		heroImage = null;
	}
}
