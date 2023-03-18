using System;
using System.Collections.Generic;
using UnityEngine;

public class EmoteGoodiesBar : GUISimpleControlWindow
{
	public enum Mode
	{
		Emotes,
		Goodies
	}

	public class AnimOpenAndClose : SHSAnimations
	{
		public static AnimClip Open(EmoteGoodiesBar bar)
		{
			return Generic.AnimationBounceTransitionIn(CHAT_SIZE, 0.2f, bar.bkg) ^ Generic.AnimationFadeTransitionIn(bar.leftButton, bar.rightButton, bar.openChatButton);
		}

		public static AnimClip Close(EmoteGoodiesBar bar)
		{
			return Generic.AnimationBounceTransitionOut(CHAT_SIZE, 0.2f, bar.bkg) ^ Generic.AnimationFadeTransitionOut(bar.leftButton, bar.rightButton, bar.openChatButton) ^ Generic.AnimationFadeTransitionOut(bar.emoteBar.buttons.ToArray()) ^ Generic.AnimationFadeTransitionOut(bar.goodieBar.buttons.ToArray());
		}

		public static AnimClip AnimateOpenComplete(EmoteGoodiesBar bar)
		{
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Expected O, but got Unknown
			AnimClip animClip = Custom.Function(GenericPaths.LinearWithWiggle(-110f, 0f, 0.7f), delegate(float x)
			{
				bar.emoteBar.buttonOffsetY = x;
			}) ^ Custom.Function(GenericPaths.LinearWithWiggle(0f, 1f, 0.7f), delegate(float x)
			{
				bar.emoteBar.buttonPercentageX = x;
				bar.emoteBar.RefreshPosition();
			}) ^ Custom.Function(GenericPaths.LinearWithWiggle(-110f, 0f, 0.7f), delegate(float x)
			{
				bar.goodieBar.buttonOffsetY = x;
			}) ^ Custom.Function(GenericPaths.LinearWithWiggle(0f, 1f, 0.7f), delegate(float x)
			{
				bar.goodieBar.buttonPercentageX = x;
				bar.goodieBar.RefreshPosition();
				bar.goodieBar.IsVisible = false;
			});
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				bar.OpenComplete();
			};
			return animClip;
		}
	}

	public static readonly Vector2 CHAT_SIZE = new Vector2(632f, 78f);

	public static readonly Vector2 CHAT_SIZE_BLOCK = new Vector2(940f, 81f);

	public static readonly Vector2 CHAT_SIZE_BLOCK_OFFSET = new Vector2(0f, 363f);

	public static readonly Vector2 HIDDEN_OFFSET = new Vector2(0f, CHAT_SIZE.y + 20f);

	public static readonly Vector2 VISIBLE_OFFSET = new Vector2(0f, 0f);

	private GUIImage bkg;

	private GUIButton openChatButton;

	private GUIButton rightButton;

	private GUIButton leftButton;

	private GUIButton switchButton;

	private GUIButton inventoryButton;

	private Mode currentMode;

	private CircularScrollBar emoteBar;

	private CircularScrollBar goodieBar;

	private bool openComplete;

	private static readonly int NUM_SLOTS = 6;

	private static readonly Vector2 SWITCH_TT_OFFSET = new Vector2(-33f, 74f);

	public EmoteGoodiesBar(NewEmoteChatBar ParentWindow)
	{
		SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle);
		SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		Traits.HitTestType = HitTestTypeEnum.Transparent;
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		GUIChildControl gUIChildControl = GUIControl.CreateControlBottomFrameCentered<GUIChildControl>(CHAT_SIZE_BLOCK, CHAT_SIZE_BLOCK_OFFSET);
		gUIChildControl.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		gUIChildControl.Traits.BlockTestType = BlockTestTypeEnum.Circular;
		gUIChildControl.Traits.HitTestType = HitTestTypeEnum.Circular;
		gUIChildControl.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Inherit;
		gUIChildControl.IsVisible = false;
		Add(gUIChildControl);
		bkg = GUIControl.CreateControlBottomFrame<GUIImage>(CHAT_SIZE, Vector2.zero);
		bkg.TextureSource = "communication_bundle|emote_goodie_bar_background";
		Add(bkg);
		CircularList<CircularScrollBar.CircularScrollButton> emoteButtonList = GetEmoteButtonList();
		emoteBar = new CircularScrollBar(emoteButtonList);
		emoteBar.Id = "emoteBar";
		emoteBar.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		Add(emoteBar);
		emoteBar.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		emoteBar.IsVisible = true;
		CircularList<CircularScrollBar.CircularScrollButton> goodieButtonList = GetGoodieButtonList();
		goodieBar = new CircularScrollBar(goodieButtonList);
		goodieBar.Id = "goodieBar";
		goodieBar.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		Add(goodieBar);
		goodieBar.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		goodieBar.IsVisible = false;
		currentMode = Mode.Emotes;
		rightButton = GUIControl.CreateControlBottomFrameCentered<GUIButton>(new Vector2(64f, 64f), new Vector2(193f, -38f));
		rightButton.StyleInfo = new SHSButtonStyleInfo("communication_bundle|emote_bar_right_scroll");
		rightButton.Id = "rightButton";
		rightButton.ToolTip = new NamedToolTipInfo("#emotebar_more");
		Add(rightButton);
		rightButton.Click += delegate
		{
			if (currentMode == Mode.Emotes)
			{
				emoteBar.GoRight();
			}
			else
			{
				goodieBar.GoRight();
			}
		};
		leftButton = GUIControl.CreateControlBottomFrameCentered<GUIButton>(new Vector2(64f, 64f), new Vector2(-189f, -40f));
		leftButton.StyleInfo = new SHSButtonStyleInfo("communication_bundle|emote_bar_left_scroll");
		leftButton.Id = "leftButton";
		leftButton.ToolTip = new NamedToolTipInfo("#emotebar_more");
		Add(leftButton);
		leftButton.Click += delegate
		{
			if (currentMode == Mode.Emotes)
			{
				emoteBar.GoLeft();
			}
			else
			{
				goodieBar.GoLeft();
			}
		};
		switchButton = GUIControl.CreateControlBottomFrame<GUIButton>(new Vector2(128f, 128f), new Vector2(0f, 0f));
		switchButton.StyleInfo = new SHSButtonStyleInfo("communication_bundle|button_switch_to_goodies");
		switchButton.HitTestType = HitTestTypeEnum.Alpha;
		switchButton.ToolTip = NoToolTipInfo.Instance;
		switchButton.Click += delegate
		{
			SwitchMode();
		};
		switchButton.IsEnabled = false;
		Add(switchButton);
		openChatButton = GUIControl.CreateControlBottomFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(-260f, -64f));
		openChatButton.StyleInfo = new SHSButtonStyleInfo("communication_bundle|button_to_keyboard_chat");
		openChatButton.HitTestType = HitTestTypeEnum.Alpha;
		openChatButton.Id = "openChatButton";
		openChatButton.EntitlementFlag = Entitlements.EntitlementFlagEnum.OpenChatAllow;
		Add(openChatButton);
		openChatButton.ToolTip = new NamedToolTipInfo("#open_chat_tooltip", new Vector2(12f, 68f));
		openChatButton.ToolTip.ContextStringLookup[GUIContext.Status.Disabled] = "#TT_FEATURE_OFF";
		openChatButton.Click += delegate
		{
			if (!Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ParentalOpenChatDeny))
			{
				GUIManager.Instance.ShowErrorDialog(SHSErrorCodes.Code.ParentalControlLimit, string.Empty);
			}
			else
			{
				ParentWindow.RequestOpenOpenChat();
			}
		};
		inventoryButton = GUIControl.CreateControlBottomFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(266f, -64f));
		inventoryButton.Id = "inventoryButton";
		inventoryButton.StyleInfo = new SHSButtonStyleInfo("communication_bundle|button_to_inventory");
		inventoryButton.HitTestType = HitTestTypeEnum.Alpha;
		inventoryButton.Click += delegate
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			if (SHSInventoryAnimatedWindow.instance != null)
			{
				SHSInventoryAnimatedWindow.instance.ToggleClosed();
			}
			else
			{
				SHSInventoryAnimatedWindow newInventoryWindow = new SHSInventoryAnimatedWindow();
				newInventoryWindow.OnToggleClose += (Action)(object)(Action)delegate
				{
					newInventoryWindow.Hide();
					newInventoryWindow.Dispose();
				};
				GUIManager.Instance.ShowDynamicWindow(newInventoryWindow, "SHSMainWindow", DrawOrder.DrawFirst, DrawPhaseHintEnum.PreDraw, ModalLevelEnum.None);
			}
		};
		inventoryButton.ToolTip = new NamedToolTipInfo("#TT_HUDWHEEL_7", new Vector2(56f, 72f));
		Add(inventoryButton);
		openComplete = false;
	}

	public override void OnShow()
	{
		AppShell.Instance.EventMgr.AddListener<PotionFetchCompleteMessage>(OnPotionFetchedMessage);
		base.OnShow();
	}

	public override void OnHide()
	{
		AppShell.Instance.EventMgr.RemoveListener<PotionFetchCompleteMessage>(OnPotionFetchedMessage);
		base.OnHide();
	}

	private void OnPotionFetchedMessage(PotionFetchCompleteMessage msg)
	{
		goodieBar.ResetButtons(GetGoodieButtonList());
	}

	private void SwitchMode()
	{
		if (currentMode == Mode.Emotes)
		{
			if (goodieBar.buttons.Count <= NUM_SLOTS)
			{
				rightButton.IsEnabled = false;
				leftButton.IsEnabled = false;
				rightButton.ToolTip = NoToolTipInfo.Instance;
				leftButton.ToolTip = NoToolTipInfo.Instance;
			}
			else
			{
				rightButton.IsEnabled = true;
				leftButton.IsEnabled = true;
				rightButton.ToolTip = new NamedToolTipInfo("#emotebar_more_goodies");
				leftButton.ToolTip = new NamedToolTipInfo("#emotebar_more_goodies");
			}
			switchButton.StyleInfo = new SHSButtonStyleInfo("communication_bundle|button_switch_to_emotes");
			switchButton.ToolTip = new NamedToolTipInfo("#TT_EMOTEBAR_SWITCH_EMOTES", SWITCH_TT_OFFSET);
			GUIManager.Instance.TooltipManager.RefreshToolTip();
			currentMode = Mode.Goodies;
			emoteBar.IsVisible = false;
			goodieBar.IsVisible = true;
			goodieBar.RefreshPosition();
		}
		else
		{
			if (currentMode != Mode.Goodies)
			{
				return;
			}
			switchButton.StyleInfo = new SHSButtonStyleInfo("communication_bundle|button_switch_to_goodies");
			switchButton.ToolTip = new NamedToolTipInfo("#TT_EMOTEBAR_SWITCH_GOODIES", SWITCH_TT_OFFSET);
			currentMode = Mode.Emotes;
			emoteBar.IsVisible = true;
			goodieBar.IsVisible = false;
			GUIManager.Instance.TooltipManager.RefreshToolTip();
			for (int num = goodieBar.buttons.Count - 1; num >= 0; num--)
			{
				CircularScrollBar.GoodieButton goodieButton = goodieBar.buttons[num] as CircularScrollBar.GoodieButton;
				if (goodieButton.Quantity <= 0)
				{
					goodieBar.RemoveButton(goodieButton);
				}
			}
			rightButton.ToolTip = new NamedToolTipInfo("#emotebar_more");
			rightButton.IsEnabled = true;
			leftButton.ToolTip = new NamedToolTipInfo("#emotebar_more");
			leftButton.IsEnabled = true;
			emoteBar.RefreshPosition();
		}
	}

	private CircularList<CircularScrollBar.CircularScrollButton> GetEmoteButtonList()
	{
		CircularList<CircularScrollBar.CircularScrollButton> circularList = new CircularList<CircularScrollBar.CircularScrollButton>();
		circularList.AddRange(GetEmoteButtonByType(EmotesDefinition.EmoteCategoriesEnum.PowerEmote));
		circularList.AddRange(GetEmoteButtonByType(EmotesDefinition.EmoteCategoriesEnum.Positive));
		circularList.AddRange(GetEmoteButtonByType(EmotesDefinition.EmoteCategoriesEnum.Aggressive));
		circularList.AddRange(GetEmoteButtonByType(EmotesDefinition.EmoteCategoriesEnum.Reactive));
		return circularList;
	}

	private CircularList<CircularScrollBar.CircularScrollButton> GetEmoteButtonByType(EmotesDefinition.EmoteCategoriesEnum category)
	{
		CircularList<CircularScrollBar.CircularScrollButton> circularList = new CircularList<CircularScrollBar.CircularScrollButton>();
		List<EmotesDefinition.EmoteDefinition> emotesByCategory = EmotesDefinition.Instance.GetEmotesByCategory(category);
		foreach (EmotesDefinition.EmoteDefinition item in emotesByCategory)
		{
			circularList.Add(new CircularScrollBar.EmoteButton(item));
		}
		return circularList;
	}

	private CircularList<CircularScrollBar.CircularScrollButton> GetGoodieButtonList()
	{
		CircularList<CircularScrollBar.CircularScrollButton> circularList = new CircularList<CircularScrollBar.CircularScrollButton>();
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null)
		{
			CspUtils.DebugLog("No profile found (are you offline?): No expendables will be added to the emote bar.");
			return circularList;
		}
		foreach (KeyValuePair<string, Expendable> item in profile.ExpendablesCollection)
		{
			if (item.Value == null || item.Value.Definition == null)
			{
				CspUtils.DebugLog("Expendable <" + item.Key + "> does not have a definition - aborting inventory addition");
			}
			else if (item.Value.Definition.CategoryList == null || item.Value.Definition.CategoryList.Count == 0)
			{
				CspUtils.DebugLog("Expendable <" + item.Key + "> is not categorized - aborting inventory addition");
			}
			else if (!item.Value.Definition.CategoryList.Contains(ExpendableDefinition.Categories.Internal) && !item.Value.Definition.CategoryList.Contains(ExpendableDefinition.Categories.Boosts))
			{
				circularList.Add(new CircularScrollBar.GoodieButton(item.Value));
			}
		}
		return circularList;
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		foreach (CircularScrollBar.CircularScrollButton button in goodieBar.buttons)
		{
			CircularScrollBar.GoodieButton goodieButton = button as CircularScrollBar.GoodieButton;
			goodieButton.ValidateGoodie();
		}
		if (!openComplete)
		{
			switchButton.IsEnabled = false;
		}
		else
		{
			if (currentMode != 0)
			{
				return;
			}
			bool isEnabled = switchButton.IsEnabled;
			switchButton.IsEnabled = (goodieBar.buttons.Count > 0);
			if (isEnabled != switchButton.IsEnabled || switchButton.ToolTip == NoToolTipInfo.Instance)
			{
				if (switchButton.IsEnabled)
				{
					switchButton.ToolTip = new NamedToolTipInfo("#TT_EMOTEBAR_SWITCH_GOODIES", SWITCH_TT_OFFSET);
				}
				else
				{
					switchButton.ToolTip = new NamedToolTipInfo("#TT_EMOTEBAR_SWITCH_NO_GOODIES", SWITCH_TT_OFFSET);
				}
				if (GUIManager.Instance != null && GUIManager.Instance.TooltipManager != null && GUIManager.Instance.TooltipManager.CurrentOverControl == switchButton)
				{
					GUIManager.Instance.TooltipManager.RefreshToolTip();
				}
			}
		}
	}

	public void OpenComplete()
	{
		openComplete = true;
	}
}
