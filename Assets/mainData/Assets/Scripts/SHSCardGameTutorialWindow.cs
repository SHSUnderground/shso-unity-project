using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSCardGameTutorialWindow : GUIDynamicWindow
{
	private class LabelInfo
	{
		public string text;

		public Vector2 offset;

		public Vector2 size;

		public int fontSize;

		public LabelInfo(string Text, Vector2 Offset, Vector2 Size, int FontSize)
		{
			text = Text;
			offset = Offset;
			size = Size;
			fontSize = FontSize;
		}
	}

	private const string slideShowPath = "cardgame_tutorial_bundle|L_cardlaunch_LearnToPlay_Rules_{0}";

	private const int slideShowCount = 16;

	private const int MAX_LABEL_COUNT = 3;

	private GUIImage rulesImage;

	private AnimClip FadeAnimation;

	private int currentSlide = 1;

	private SHSCardGameGadgetWindow parentWindow;

	private GUILabel[] labels = new GUILabel[3];

	private List<List<LabelInfo>> slideLabelInfo;

	private GUISimpleControlWindow[] captionWindows = new GUISimpleControlWindow[3];

	public SHSCardGameTutorialWindow(SHSCardGameGadgetWindow parentWindow)
	{
		slideLabelInfo = new List<List<LabelInfo>>();
		PopulateLabelInfoList(slideLabelInfo);
		this.parentWindow = parentWindow;
		SetSize(new Vector2(1022f, 644f));
		SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
		rulesImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(959f, 606f), Vector2.zero);
		rulesImage.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		rulesImage.Click += delegate
		{
			NextSlide();
		};
		rulesImage.TextureSource = string.Format("cardgame_tutorial_bundle|L_cardlaunch_LearnToPlay_Rules_{0}", currentSlide);
		rulesImage.Alpha = 0f;
		Add(rulesImage);
		GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(-482f, 0f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|arrow_left");
		gUIButton.Click += delegate
		{
			PreviousSlide();
		};
		Add(gUIButton);
		GUIButton gUIButton2 = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(482f, 0f));
		gUIButton2.StyleInfo = new SHSButtonStyleInfo("common_bundle|arrow_right");
		gUIButton2.Click += delegate
		{
			NextSlide();
		};
		Add(gUIButton2);
		GUIButton gUIButton3 = new GUIButton();
		gUIButton3.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(442f, 280f), new Vector2(256f, 256f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton3.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton3.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_airlock_quitbutton");
		gUIButton3.Click += delegate
		{
			Close();
		};
		Add(gUIButton3);
		for (int i = 0; i < 3; i++)
		{
			captionWindows[i] = new GUISimpleControlWindow();
			captionWindows[i].SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(300f, 300f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			labels[i] = new GUILabel();
			labels[i].SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f));
			labels[i].SetupText(GUIFontManager.SupportedFontEnum.Komica, 26, GUILabel.GenColor(52, 75, 87), TextAnchor.UpperLeft);
			labels[i].Id = "LABEL_" + i.ToString();
			labels[i].Text = "LABEL_" + i.ToString();
			labels[i].SetSize(labels[i].Style.UnityStyle.CalcSize(new GUIContent(labels[i].Text)));
			labels[i].IsVisible = false;
			captionWindows[i].Add(labels[i]);
			Add(captionWindows[i]);
		}
		base.AnimationPieceManager.RemoveIfUnfinished(FadeAnimation);
		FadeAnimation = SHSAnimations.Generic.FadeIn(rulesImage, 0.5f);
		base.AnimationPieceManager.Add(FadeAnimation);
		if (parentWindow != null)
		{
			parentWindow.WindowsVisible = false;
		}
		SetLabels(0);
	}

	public override void OnShow()
	{
		base.OnShow();
		ShsAudioSource.PlayAutoSound(ShsAudioSourceList.GetList("CardGameTutorial").GetSource("music"));
	}

	private void PreviousSlide()
	{
		currentSlide--;
		if (currentSlide < 1)
		{
			Close();
			return;
		}
		rulesImage.TextureSource = string.Format("cardgame_tutorial_bundle|L_cardlaunch_LearnToPlay_Rules_{0}", currentSlide);
		SetLabels(currentSlide - 1);
	}

	private void NextSlide()
	{
		currentSlide++;
		if (currentSlide > 16)
		{
			Close();
			return;
		}
		rulesImage.TextureSource = string.Format("cardgame_tutorial_bundle|L_cardlaunch_LearnToPlay_Rules_{0}", currentSlide);
		SetLabels(currentSlide - 1);
	}

	private void SetLabels(int slideNumber)
	{
		if (slideNumber >= slideLabelInfo.Count)
		{
			return;
		}
		List<LabelInfo> list = slideLabelInfo[slideNumber];
		for (int i = 0; i < 3; i++)
		{
			if (i < list.Count)
			{
				LabelInfo labelInfo = list[i];
				labels[i].SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f));
				labels[i].FontSize = labelInfo.fontSize;
				labels[i].Text = labelInfo.text;
				labels[i].SetSize(labelInfo.size);
				labels[i].IsVisible = true;
				GUISimpleControlWindow gUISimpleControlWindow = captionWindows[i];
				gUISimpleControlWindow.Offset = labelInfo.offset;
				gUISimpleControlWindow.SetSize(labelInfo.size);
				captionWindows[i].IsVisible = true;
			}
			else
			{
				labels[i].IsVisible = false;
				captionWindows[i].IsVisible = false;
			}
		}
	}

	private void Close()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		base.AnimationPieceManager.RemoveIfUnfinished(FadeAnimation);
		FadeAnimation = SHSAnimations.Generic.FadeOut(rulesImage, 0.5f);
		FadeAnimation.OnFinished += (Action)(object)(Action)delegate
		{
			Hide();
			if (parentWindow != null)
			{
				parentWindow.WindowsVisible = true;
			}
		};
		base.AnimationPieceManager.Add(FadeAnimation);
		AppShell.Instance.AudioManager.RequestCrossfade(null);
	}

	private void PopulateLabelInfoList(List<List<LabelInfo>> infoList)
	{
		LabelInfo item = new LabelInfo("#CGG_HTP_SLIDE01A", new Vector2(30f, -138f), new Vector2(280f, 200f), 26);
		List<LabelInfo> list = new List<LabelInfo>();
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE01B", new Vector2(-188f, 46f), new Vector2(250f, 200f), 26);
		list.Add(item);
		infoList.Add(list);
		item = new LabelInfo("#CGG_HTP_SLIDE02A", new Vector2(66f, -165f), new Vector2(303f, 200f), 26);
		list = new List<LabelInfo>();
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE02B", new Vector2(-162f, 42f), new Vector2(275f, 200f), 26);
		list.Add(item);
		infoList.Add(list);
		item = new LabelInfo("#CGG_HTP_SLIDE03A", new Vector2(16f, -140f), new Vector2(300f, 200f), 26);
		list = new List<LabelInfo>();
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE03B", new Vector2(320f, 18f), new Vector2(250f, 200f), 26);
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE03C", new Vector2(-245f, 38f), new Vector2(270f, 200f), 26);
		list.Add(item);
		infoList.Add(list);
		item = new LabelInfo("#CGG_HTP_SLIDE04A", new Vector2(-163f, -164f), new Vector2(290f, 200f), 26);
		list = new List<LabelInfo>();
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE04B", new Vector2(293f, -36f), new Vector2(330f, 200f), 26);
		list.Add(item);
		infoList.Add(list);
		item = new LabelInfo("#CGG_HTP_SLIDE05A", new Vector2(35f, -118f), new Vector2(310f, 200f), 26);
		list = new List<LabelInfo>();
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE05B", new Vector2(325f, 63f), new Vector2(285f, 200f), 26);
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE05C", new Vector2(312f, 250f), new Vector2(250f, 200f), 26);
		list.Add(item);
		infoList.Add(list);
		item = new LabelInfo("#CGG_HTP_SLIDE06A", new Vector2(-176f, -123f), new Vector2(250f, 200f), 26);
		list = new List<LabelInfo>();
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE06B", new Vector2(-305f, 45f), new Vector2(240f, 200f), 26);
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE06C", new Vector2(280f, 240f), new Vector2(250f, 200f), 26);
		list.Add(item);
		infoList.Add(list);
		item = new LabelInfo("#CGG_HTP_SLIDE07A", new Vector2(-157f, -131f), new Vector2(260f, 200f), 26);
		list = new List<LabelInfo>();
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE07B", new Vector2(357f, 138f), new Vector2(170f, 200f), 26);
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE07C", new Vector2(312f, 230f), new Vector2(290f, 200f), 26);
		list.Add(item);
		infoList.Add(list);
		item = new LabelInfo("#CGG_HTP_SLIDE08A", new Vector2(-109f, -163f), new Vector2(300f, 200f), 26);
		list = new List<LabelInfo>();
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE08B", new Vector2(223f, 47f), new Vector2(230f, 200f), 26);
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE08C", new Vector2(223f, 145f), new Vector2(230f, 200f), 26);
		list.Add(item);
		infoList.Add(list);
		item = new LabelInfo("#CGG_HTP_SLIDE09A", new Vector2(-141f, -132f), new Vector2(250f, 200f), 26);
		list = new List<LabelInfo>();
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE09B", new Vector2(-195f, -20f), new Vector2(180f, 200f), 26);
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE09C", new Vector2(325f, 204f), new Vector2(250f, 200f), 26);
		list.Add(item);
		infoList.Add(list);
		item = new LabelInfo("#CGG_HTP_SLIDE10A", new Vector2(-98f, -188f), new Vector2(190f, 200f), 26);
		list = new List<LabelInfo>();
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE10B", new Vector2(120f, -122f), new Vector2(245f, 200f), 26);
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE10C", new Vector2(312f, 238f), new Vector2(300f, 250f), 26);
		list.Add(item);
		infoList.Add(list);
		item = new LabelInfo("#CGG_HTP_SLIDE11A", new Vector2(1f, -87f), new Vector2(250f, 200f), 26);
		list = new List<LabelInfo>();
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE11B", new Vector2(277f, 196f), new Vector2(320f, 200f), 26);
		list.Add(item);
		infoList.Add(list);
		item = new LabelInfo("#CGG_HTP_SLIDE12A", new Vector2(-320f, -168f), new Vector2(250f, 200f), 26);
		list = new List<LabelInfo>();
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE12B", new Vector2(68f, 288f), new Vector2(240f, 200f), 26);
		list.Add(item);
		infoList.Add(list);
		item = new LabelInfo("#CGG_HTP_SLIDE13A", new Vector2(319f, -182f), new Vector2(265f, 200f), 26);
		list = new List<LabelInfo>();
		list.Add(item);
		item = new LabelInfo("#CGG_HTP_SLIDE13B", new Vector2(277f, 5f), new Vector2(350f, 200f), 26);
		list.Add(item);
		infoList.Add(list);
		item = new LabelInfo("#CGG_HTP_SLIDE14A", new Vector2(-268f, -70f), new Vector2(335f, 200f), 22);
		list = new List<LabelInfo>();
		list.Add(item);
		infoList.Add(list);
		item = new LabelInfo("#CGG_HTP_SLIDE15A", new Vector2(-205f, 92f), new Vector2(250f, 300f), 24);
		list = new List<LabelInfo>();
		list.Add(item);
		infoList.Add(list);
		item = new LabelInfo("#CGG_HTP_SLIDE16A", new Vector2(20f, 72f), new Vector2(238f, 200f), 18);
		list = new List<LabelInfo>();
		list.Add(item);
		infoList.Add(list);
	}
}
