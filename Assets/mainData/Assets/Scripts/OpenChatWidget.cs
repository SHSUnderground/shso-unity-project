using System;
using System.Collections.Generic;
using UnityEngine;

public class OpenChatWidget : GUISimpleControlWindow, IInputHandler
{
	public class BlockTestSpot : GUISimpleControlWindow
	{
		public BlockTestSpot()
		{
			HitTestType = HitTestTypeEnum.Rect;
			BlockTestType = BlockTestTypeEnum.Rect;
			IsVisible = false;
		}
	}

	private class LogBoxItem : SHSSelectionItem<GUISimpleControlWindow>
	{
		private class SingleLineLabel : GUILabel
		{
			public SingleLineLabel()
			{
				base.WordWrap = true;
				base.NoLineLimit = true;
				base.VerticalKerning = 0;
			}

			public string GetOverflow()
			{
				CalculateTextLayout();
				if (base.LineCount <= 1)
				{
					return null;
				}
				LabelLine labelLine = labelLines[0];
				string text = labelLine.text;
				string[] array = new string[labelLines.Count - 1];
				for (int i = 1; i < labelLines.Count; i++)
				{
					int num = i - 1;
					LabelLine labelLine2 = labelLines[i];
					array[num] = labelLine2.text;
				}
				Text = text;
				return string.Join(" ", array);
			}
		}

		private SingleLineLabel label;

		private bool systemMessage;

		public LogBoxItem(Color color, string name, string text, bool systemMessage)
		{
			this.systemMessage = systemMessage;
			item = GUIControl.CreateControlAbsolute<GUISimpleControlWindow>(LOG_BOX_ITEM_SIZE, new Vector2(0f, 0f));
			itemSize = LOG_BOX_ITEM_SIZE;
			float x = 0f;
			if (!string.IsNullOrEmpty(name))
			{
				GUILabel gUILabel = GUIControl.CreateControl<GUILabel>(LOG_BOX_ITEM_SIZE, new Vector2(0f, 0f), DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft);
				gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, color, TextAnchor.MiddleLeft);
				gUILabel.Text = name + ":";
				item.Add(gUILabel);
				x = gUILabel.GetTextWidth() + 7f;
			}
			label = GUIControl.CreateControl<SingleLineLabel>(LOG_BOX_ITEM_SIZE - new Vector2(x, 0f), new Vector2(0f, 0f), DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
			label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, (!systemMessage) ? Color.black : color, TextAnchor.MiddleLeft);
			label.Text = text;
			item.Add(label);
		}

		public LogBoxItem Split()
		{
			string overflow = label.GetOverflow();
			if (overflow == null)
			{
				return null;
			}
			return new LogBoxItem(label.TextColor, string.Empty, overflow, systemMessage);
		}
	}

	private class LogBoxWindow : SHSSelectionWindow<LogBoxItem, GUISimpleControlWindow>
	{
		public LogBoxWindow(GUISlider slider, Vector2 lineSize)
			: base(slider, lineSize.x, lineSize, 8)
		{
			TopOffsetAdjustHeight = 0f;
			BottomOffsetAdjustHeight = 0f;
			slider.FireChanged();
		}
	}

	private const int LOG_BOX_MAX_LINES = 100;

	public const float FADE_OUT_AMOUNT = 0.6f;

	public const float TEXT_OFFSET_FROM_NAME = 7f;

	public NewEmoteChatBar mainWindow;

	public static readonly Vector2 WIDGET_SIZE = new Vector2(600f, 180f);

	public static readonly Vector2 HIDDEN_OFFSET = new Vector2(0f, WIDGET_SIZE.y + 20f);

	public static readonly Vector2 VISIBLE_OFFSET = new Vector2(0f, 0f);

	public static readonly Vector2 ENTRY_BOX_SIZE = new Vector2(576f, 58f);

	public static readonly Vector2 ENTRY_BOX_OFFSET = new Vector2(0f, 0f);

	public static readonly Vector2 ENTRY_BOX_TEXT_SIZE = new Vector2(449f, 37f);

	public static readonly Vector2 ENTRY_BOX_TEXT_OFFSET = new Vector2(2f, -12f);

	public static readonly Vector2 LOG_BOX_SIZE = new Vector2(512f, 134f);

	public static readonly Vector2 LOG_BOX_OFFSET = new Vector2(0f, -50f);

	public static readonly Vector2 LOG_BOX_TEXT_SIZE = new Vector2(446f, 108f);

	public static readonly Vector2 LOG_BOX_TEXT_OFFSET = new Vector2(-17f, -65f);

	public static readonly Vector2 LOG_BOX_ITEM_SIZE = new Vector2(LOG_BOX_TEXT_SIZE.x, 18f);

	public static readonly Vector2 SLIDER_UP_SIZE = new Vector2(32f, 32f);

	public static readonly Vector2 SLIDER_DOWN_SIZE = new Vector2(32f, 32f);

	public static readonly Vector2 SLIDER_SIZE = new Vector2(40f, 120f);

	public static readonly Vector2 SLIDER_OFFSET = new Vector2(-29f, -28f);

	public static readonly Vector2 SLIDER_UP_OFFSET = new Vector2(-30f, -50f);

	public static readonly Vector2 SLIDER_DOWN_OFFSET = new Vector2(SLIDER_UP_OFFSET.x, -7f);

	public static readonly Vector2 ENTER_BUTTON_SIZE = new Vector2(64f, 64f);

	public static readonly Vector2 ENTER_BUTTON_OFFSET = new Vector2(258f, -27f);

	private GUIImage entryBoxImage;

	private GUIImage entryBoxImage_Inactive;

	private GUIImage logBoxImage;

	private LogBoxWindow logBox;

	private GUISlider logBoxSlider;

	private GUITextField entryBox;

	private GUIButton enterButton;

	private GUIButton ToEmoteChat;

	private AnimClip fadeLogBox;

	private bool logBoxFocused;

	private AnimClip fadeEntryBox;

	private bool entryBoxFocused;

	private bool smoothScroll;

	private bool snapScrollOnUpdate;

	private bool filterEntryBoxOnUpdate;

	private AnimClip fadeWigitAnimation;

	SHSInput.InputRequestorType IInputHandler.InputRequestorType
	{
		get
		{
			return SHSInput.InputRequestorType.UI;
		}
	}

	bool IInputHandler.CanHandleInput
	{
		get
		{
			return true;
		}
	}

	public OpenChatWidget(NewEmoteChatBar mainWindow)
	{
		this.mainWindow = mainWindow;
		SetSize(WIDGET_SIZE);
		SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, HIDDEN_OFFSET);
		Add(GUIControl.CreateControlBottomFrame<BlockTestSpot>(new Vector2(562f, 51f), new Vector2(0f, 0f)));
		Add(GUIControl.CreateControlBottomFrame<BlockTestSpot>(new Vector2(518f, 178f), new Vector2(10f, 0f)));
		GUIHotSpotButton gUIHotSpotButton = GUIControl.CreateControlBottomFrame<GUIHotSpotButton>(new Vector2(562f, 178f), new Vector2(0f, 0f));
		gUIHotSpotButton.MouseOver += delegate
		{
			FadeWigit(true);
		};
		gUIHotSpotButton.MouseOut += delegate
		{
			FadeWigit(false);
		};
		Add(gUIHotSpotButton);
		GUIHotSpotButton gUIHotSpotButton2 = GUIControl.CreateControlBottomFrameCentered<GUIHotSpotButton>(new Vector2(20f, 32f), new Vector2(-218f, -29f));
		gUIHotSpotButton2.Click += delegate
		{
			Focus();
		};
		Add(gUIHotSpotButton2);
		entryBoxImage = GUIControl.CreateControlBottomFrame<GUIImage>(ENTRY_BOX_SIZE, ENTRY_BOX_OFFSET);
		entryBoxImage.TextureSource = "communication_bundle|mshs_text_entry_field_inuse";
		Add(entryBoxImage);
		entryBoxImage_Inactive = GUIControl.CreateControlBottomFrame<GUIImage>(ENTRY_BOX_SIZE, ENTRY_BOX_OFFSET);
		entryBoxImage_Inactive.TextureSource = "communication_bundle|mshs_text_entry_field_notinuse";
		entryBoxImage_Inactive.Alpha = 0f;
		Add(entryBoxImage_Inactive);
		entryBox = GUIControl.CreateControlBottomFrame<GUITextField>(ENTRY_BOX_TEXT_SIZE, ENTRY_BOX_TEXT_OFFSET);
		entryBox.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		entryBox.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, GUILabel.GenColor(16, 44, 57), TextAnchor.MiddleLeft);
		entryBox.BackgroundColor = new Color(1f, 1f, 1f, 0f);
		entryBox.MaxLength = 140;
		entryBox.Id = "Entry Box";
		entryBox.WordWrap = false;
		entryBox.Changed += delegate
		{
			filterEntryBoxOnUpdate = true;
		};
		entryBox.OnEnter += delegate
		{
			SendCurrentMessage();
		};
		entryBox.Click += delegate
		{
		};
		entryBox.ToolTip = new NamedToolTipInfo("#safe_chat_text_field");
		Add(entryBox);
		logBoxSlider = GUIControl.CreateControl<GUISlider>(SLIDER_SIZE, SLIDER_OFFSET, DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
		logBoxSlider.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		foreach (IGUIControl control in logBoxSlider.ControlList)
		{
			control.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		}
		logBoxImage = GUIControl.CreateControlBottomFrame<GUIImage>(LOG_BOX_SIZE, LOG_BOX_OFFSET);
		logBoxImage.TextureSource = "communication_bundle|mshs_chat_log_background_inuse";
		logBoxImage.Alpha = 0.6f;
		Add(logBoxImage);
		logBox = new LogBoxWindow(logBoxSlider, LOG_BOX_ITEM_SIZE);
		logBox.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, LOG_BOX_TEXT_OFFSET, LOG_BOX_TEXT_SIZE, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		logBox.SetBackground(new Color(0.5f, 0.5f, 0.5f, 0f));
		logBox.UpdateDisplay();
		Add(logBox);
		logBoxSlider.IsRefreshSuppressed = true;
		logBoxSlider.Value = logBoxSlider.Max;
		logBoxSlider.ArrowsEnabled = true;
		logBoxSlider.ArrowsAlwaysOn = true;
		logBoxSlider.ScrollButtonSize = new Vector2(45f, 45f);
		logBoxSlider.SliderThickness = 18f;
		logBoxSlider.VerticalCapHeight = 11f;
		logBoxSlider.VerticalArrowOffset = 11f;
		logBoxSlider.VerticalButtonOffset = 20f;
		logBoxSlider.ArrowStartTexture = "common_bundle|arrow_up";
		logBoxSlider.ArrowEndTexture = "common_bundle|arrow_down";
		logBoxSlider.StartArrow.SetSize(new Vector2(42f, 42f));
		logBoxSlider.EndArrow.SetSize(new Vector2(42f, 42f));
		GUISlider gUISlider = logBoxSlider;
		Vector2 lOG_BOX_ITEM_SIZE = LOG_BOX_ITEM_SIZE;
		gUISlider.TickValue = lOG_BOX_ITEM_SIZE.y;
		logBoxSlider.SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		logBoxSlider.IsRefreshSuppressed = false;
		logBoxSlider.RefreshLayout();
		logBoxSlider.Changed += delegate
		{
			snapScrollOnUpdate = true;
		};
		logBoxSlider.ThumbButton.MouseDown += delegate
		{
			smoothScroll = true;
		};
		logBoxSlider.ThumbButton.MouseUp += delegate
		{
			smoothScroll = false;
			snapScrollOnUpdate = true;
		};
		logBoxSlider.ThumbButton.MouseOut += delegate
		{
			smoothScroll = false;
			snapScrollOnUpdate = true;
		};
		Add(logBoxSlider);
		enterButton = GUIControl.CreateControlBottomFrameCentered<GUIButton>(ENTER_BUTTON_SIZE, ENTER_BUTTON_OFFSET);
		enterButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		enterButton.HitTestType = HitTestTypeEnum.Rect;
		enterButton.HitTestSize = new Vector2(0.5f, 0.5f);
		enterButton.StyleInfo = new SHSButtonStyleInfo("communication_bundle|mshs_button_return");
		enterButton.Click += delegate
		{
			SendCurrentMessage();
		};
		enterButton.ToolTip = new NamedToolTipInfo("#safe_chat_return");
		Add(enterButton);
		ToEmoteChat = GUIControl.CreateControlBottomFrameCentered<GUIButton>(new Vector2(64f, 64f), new Vector2(-257f, -27f));
		ToEmoteChat.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		ToEmoteChat.StyleInfo = new SHSButtonStyleInfo("communication_bundle|mshs_button_switch_to_emotes");
		ToEmoteChat.HitTestType = HitTestTypeEnum.Rect;
		ToEmoteChat.ToolTip = new NamedToolTipInfo("#safe_chat_to_emote_chat");
		ToEmoteChat.HitTestSize = new Vector2(0.781f, 0.718f);
		ToEmoteChat.Click += delegate
		{
			mainWindow.RequestOpenEmoteChat();
		};
		Add(ToEmoteChat);
	}

	Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> IInputHandler.GetKeyList(KeyInputState inputState)
	{
		throw new NotImplementedException();
	}

	void IInputHandler.ConfigureKeyBanks()
	{
		throw new NotImplementedException();
	}

	bool IInputHandler.IsDescendantHandler(IInputHandler handler)
	{
		return this == handler;
	}

	public void FadeWigit(bool fadeOn)
	{
		float num = (!fadeOn) ? 0.6f : 1f;
		float num2 = (!fadeOn) ? 1f : 0.6f;
		float finish = (!fadeOn) ? 0.6f : 0.85f;
		float time = SHSAnimations.GenericFunctions.FrationalTime(num2, num, logBoxSlider.AnimationAlpha, 0.2f);
		AnimClip newPiece = AnimClipBuilder.Absolute.AnimationAlpha(AnimClipBuilder.Path.Linear(logBoxSlider.AnimationAlpha, num, time), logBoxSlider) ^ AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(entryBoxImage.Alpha, num, time), entryBoxImage) ^ AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(entryBoxImage_Inactive.Alpha, num2, time), entryBoxImage_Inactive) ^ AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(logBoxImage.Alpha, finish, time), logBoxImage);
		base.AnimationPieceManager.SwapOut(ref fadeWigitAnimation, newPiece);
	}

	public override void Update()
	{
		base.Update();
		if (snapScrollOnUpdate && !smoothScroll)
		{
			snapScrollOnUpdate = false;
			logBoxSlider.Value = TruncatedSliderValue(logBoxSlider.Value);
		}
		if (filterEntryBoxOnUpdate)
		{
			filterEntryBoxOnUpdate = false;
			FilterEntryBox();
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		CspUtils.DebugLog("OpenChatWidget OnShow() called!");
		AppShell.Instance.EventMgr.AddListener<OpenChatMessage>(OnChatMessage);
		FadeWigit(false);
	}

	public void Focus()
	{
		GUIManager.Instance.FocusManager.getFocus(entryBox);
	}

	public override void OnHide()
	{
		AppShell.Instance.EventMgr.RemoveListener<OpenChatMessage>(OnChatMessage);
		base.OnHide();
	}

	private float TruncatedSliderValue(float value)
	{
		Vector2 lOG_BOX_ITEM_SIZE = LOG_BOX_ITEM_SIZE;
		float num = Mathf.Round(value / lOG_BOX_ITEM_SIZE.y);
		Vector2 lOG_BOX_ITEM_SIZE2 = LOG_BOX_ITEM_SIZE;
		return num * lOG_BOX_ITEM_SIZE2.y;
	}

	private void AddMessage(OpenChatMessage.MessageStyle style, string name, string text, bool systemMessage)
	{
		CspUtils.DebugLog("AddMessage " + name + " " + text);

		Color messageColor = CommunicationManager.GetMessageColor(style);
		bool flag = TruncatedSliderValue(logBoxSlider.Value) == TruncatedSliderValue(logBoxSlider.Max);
		LogBoxItem logBoxItem = new LogBoxItem(messageColor, name, text, systemMessage);
		while (logBoxItem != null)
		{
			LogBoxItem item = logBoxItem;
			logBoxItem = logBoxItem.Split();
			logBox.AddItem(item);
		}
		while (logBox.items.Count > 100)
		{
			logBox.RemoveItem(logBox.items[0]);
		}
		if (style == OpenChatMessage.MessageStyle.Self || (flag && !smoothScroll))
		{
			logBox.UpdateDisplay();
			logBoxSlider.Value = logBoxSlider.Max;
		}
	}

	private void SendCurrentMessage()
	{
		if (!string.IsNullOrEmpty(entryBox.Text))
		{
			if (string.IsNullOrEmpty(entryBox.Text.Trim()))
			{
				entryBox.Text = string.Empty;
				return;
			}
			//AppShell.Instance.EventReporter.ReportOpenChatMessage(AppShell.Instance.PlayerDictionary.GetPlayerId(AppShell.Instance.ServerConnection.GetGameUserId()), AppShell.Instance.ServerConnection.GetRoomName(), entryBox.Text);
			// CSP commented above line and added line below in it's place.
			AppShell.Instance.EventReporter.ReportOpenChatMessage(AppShell.Instance.ServerConnection.GetGameUserId(), AppShell.Instance.ServerConnection.GetRoomName(), DUtils.dutil(entryBox.Text));
			entryBox.Text = string.Empty;
		}
	}

	private void OnChatMessage(OpenChatMessage msg)
	{
		string name = string.Empty;
		bool systemMessage = false;
		string empty = string.Empty;
		if (msg.messageStyle != OpenChatMessage.MessageStyle.System)
		{
			name = ((!string.IsNullOrEmpty(msg.sendingPlayerName)) ? msg.sendingPlayerName : "????");
			empty = msg.message;
		}
		else
		{
			empty = "[System]: " + msg.message;
			systemMessage = true;
		}
		AddMessage(msg.messageStyle, name, empty, systemMessage);
	}

	private void FilterEntryBox()
	{
		string text = entryBox.Text;
		string text2 = CommunicationManager.FilterChatString(text);
		if (text2 != text)
		{
			entryBox.Text = text2;
		}
	}
}
