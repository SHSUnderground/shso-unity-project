using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSEmoteChatBar : SHSHudWindows
{
	public class EmoteBarEmotes : GUISimpleControlWindow
	{
		public class AnimOpenAndClose : SHSAnimations
		{
			public static AnimClip Open(EmoteBarEmotes bar)
			{
				return Generic.AnimationBounceTransitionIn(CHAT_SIZE, 0.2f, bar.bkg) ^ Generic.AnimationFadeTransitionIn(bar.leftButton, bar.rightButton, bar.ToOpenChat);
			}

			public static AnimClip Close(EmoteBarEmotes bar)
			{
				return Generic.AnimationBounceTransitionOut(CHAT_SIZE, 0.2f, bar.bkg) ^ Generic.AnimationFadeTransitionOut(bar.leftButton, bar.rightButton, bar.ToOpenChat) ^ Generic.AnimationFadeTransitionOut(bar.emoteButtons.ToArray());
			}

			public static AnimClip AnimateOpenComplete(EmoteBarEmotes bar)
			{
				return Custom.Function(GenericPaths.LinearWithWiggle(-110f, 0f, 0.7f), delegate(float x)
				{
					bar.buttonOffsetY = x;
				}) ^ Custom.Function(GenericPaths.LinearWithWiggle(0f, 1f, 0.7f), delegate(float x)
				{
					bar.buttonPercentageX = x;
					bar.RefreshPosition();
				});
			}
		}

		public class EmoteButton : GUISimpleControlWindow
		{
			private EmotesDefinition.EmoteDefinition def;

			private GUIAnimatedButton EmoteImage;

			private bool EmoteEnabled = true;

			public EmoteButton(EmotesDefinition.EmoteDefinition def)
			{
				this.def = def;
				SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle);
				SetSize(128f, 128f);
				EmoteImage = GUIControl.CreateControlFrameCentered<GUIAnimatedButton>(new Vector2(128f, 128f), Vector2.zero);
				EmoteImage.TextureSource = "communication_bundle|emote_" + def.command;
				EmoteImage.SetupButton(0.9f, 1f, 0.8f);
				EmoteImage.HitTestType = HitTestTypeEnum.Circular;
				EmoteImage.HitTestSize = new Vector2(0.4f, 0.4f);
				EmoteImage.Click += delegate(GUIControl sender, GUIClickEvent EventData)
				{
					if (EmoteEnabled)
					{
						AppShell.Instance.EventMgr.Fire(sender, new EmoteMessage(GameController.GetController().LocalPlayer, def.id));
					}
				};
				string name = "#emotechat_" + def.command;
				EmoteImage.ToolTip = new NamedToolTipInfo(name);
				Add(EmoteImage);
			}

			public void validateEmote(string characterName)
			{
				string failReason;
				bool flag = EmoteEnabled = EmotesDefinition.Instance.RequirementsCheck(def.id, characterName, out failReason);
				EmoteImage.TextureSource = ((!flag) ? "communication_bundle|emote_powerlocked" : ("communication_bundle|emote_" + def.command));
				EmoteImage.ToolTip = new NamedToolTipInfo((!flag) ? failReason : ("#emotechat_" + def.command));
			}

			public void UpdatePosition(float pos, float OffsetY, float percentageX)
			{
				pos *= percentageX;
				float num = Mathf.Abs(pos);
				float x = Mathf.Sign(pos) * EmoteChatAnim.LocationX(num, -2f);
				float y = 0f - EmoteChatAnim.LocationY(num, -2f) - OffsetY;
				Offset = new Vector2(x, y);
				float num2 = 128f * (1f - 0.1f * num) - Mathf.Clamp01(num - 2f) * 10f;
				EmoteImage.SetSize(num2, num2);
				SetSize(num2, num2);
				Rotation = pos * 10f;
				if (num < 3.5f)
				{
					IsVisible = true;
					Alpha = Mathf.Clamp01(3.5f - num);
				}
				else
				{
					IsVisible = false;
				}
			}
		}

		public static readonly Vector2 CHAT_SIZE = new Vector2(507f, 81f);

		public static readonly Vector2 CHAT_SIZE_BLOCK = new Vector2(940f, 81f);

		public static readonly Vector2 CHAT_SIZE_BLOCK_OFFSET = new Vector2(0f, 363f);

		public static readonly Vector2 HIDDEN_OFFSET = new Vector2(0f, CHAT_SIZE.y + 20f);

		public static readonly Vector2 VISIBLE_OFFSET = new Vector2(0f, 0f);

		private GUIImage bkg;

		private GUIButton ToOpenChat;

		private GUIButton rightButton;

		private GUIButton leftButton;

		private CircularList<EmoteButton> emoteButtons;

		private float CurrentTargetPosition = 2.5f;

		private float CurrentPosition = 2.5f;

		private float buttonOffsetY = -110f;

		private float buttonPercentageX;

		private AnimClip currentMovementAnimation;

		public EmoteBarEmotes(SHSEmoteChatBar ParentWindow)
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
			bkg.TextureSource = "communication_bundle|mshs_emote_bar_background";
			Add(bkg);
			emoteButtons = GetEmoteButtonList();
			foreach (EmoteButton emoteButton in emoteButtons)
			{
				Add(emoteButton);
			}
			rightButton = GUIControl.CreateControlBottomFrameCentered<GUIButton>(new Vector2(64f, 64f), new Vector2(198f, -21f));
			rightButton.StyleInfo = new SHSButtonStyleInfo("communication_bundle|mshs_emote_scroll_button_right");
			rightButton.ToolTip = new NamedToolTipInfo("#emotebar_more");
			Add(rightButton);
			rightButton.Click += delegate
			{
				GoRight();
			};
			leftButton = GUIControl.CreateControlBottomFrameCentered<GUIButton>(new Vector2(64f, 64f), new Vector2(-198f, -21f));
			leftButton.StyleInfo = new SHSButtonStyleInfo("communication_bundle|mshs_emote_scroll_button_left");
			leftButton.ToolTip = new NamedToolTipInfo("#emotebar_more");
			Add(leftButton);
			leftButton.Click += delegate
			{
				GoLeft();
			};
			ToOpenChat = GUIControl.CreateControlBottomFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(0f, -64f));
			ToOpenChat.StyleInfo = new SHSButtonStyleInfo("communication_bundle|mshs_button_switch_to_open_chat");
			ToOpenChat.HitTestType = HitTestTypeEnum.Alpha;
			ToOpenChat.EntitlementFlag = Entitlements.EntitlementFlagEnum.OpenChatAllow;
			Add(ToOpenChat);
			ToOpenChat.ToolTip = new NamedToolTipInfo("#open_chat_tooltip", new Vector2(45f, 55f));
			ToOpenChat.ToolTip.ContextStringLookup[GUIContext.Status.Disabled] = "#TT_FEATURE_OFF";
			ToOpenChat.Click += delegate
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
		}

		private CircularList<EmoteButton> GetEmoteButtonList()
		{
			CircularList<EmoteButton> circularList = new CircularList<EmoteButton>();
			circularList.AddRange(GetEmoteButtonByType(EmotesDefinition.EmoteCategoriesEnum.PowerEmote));
			circularList.AddRange(GetEmoteButtonByType(EmotesDefinition.EmoteCategoriesEnum.Positive));
			circularList.AddRange(GetEmoteButtonByType(EmotesDefinition.EmoteCategoriesEnum.Aggressive));
			circularList.AddRange(GetEmoteButtonByType(EmotesDefinition.EmoteCategoriesEnum.Reactive));
			return circularList;
		}

		private CircularList<EmoteButton> GetEmoteButtonByType(EmotesDefinition.EmoteCategoriesEnum category)
		{
			CircularList<EmoteButton> circularList = new CircularList<EmoteButton>();
			List<EmotesDefinition.EmoteDefinition> emotesByCategory = EmotesDefinition.Instance.GetEmotesByCategory(category);
			foreach (EmotesDefinition.EmoteDefinition item in emotesByCategory)
			{
				circularList.Add(new EmoteButton(item));
			}
			return circularList;
		}

		public override void OnShow()
		{
			base.OnShow();
			AppShell.Instance.EventMgr.AddListener<CharacterSelectedMessage>(OnCharacterSelected);
			AppShell.Instance.EventMgr.AddListener<LeveledUpMessage>(OnLeveledUp);
			RefreshPosition();
			validateEmotes();
		}

		public override void OnHide()
		{
			base.OnHide();
			AppShell.Instance.EventMgr.RemoveListener<CharacterSelectedMessage>(OnCharacterSelected);
			AppShell.Instance.EventMgr.RemoveListener<LeveledUpMessage>(OnLeveledUp);
		}

		private void OnCharacterSelected(CharacterSelectedMessage message)
		{
			validateEmotes();
		}

		private void OnLeveledUp(LeveledUpMessage msg)
		{
			validateEmotes();
		}

		private void validateEmotes()
		{
			string text;
			if (AppShell.Instance.Profile != null)
			{
				text = AppShell.Instance.Profile.SelectedCostume;
				if (string.IsNullOrEmpty(text))
				{
					text = AppShell.Instance.Profile.LastSelectedCostume;
				}
			}
			else
			{
				text = GameController.GetController().LocalPlayer.name;
			}
			foreach (EmoteButton emoteButton in emoteButtons)
			{
				emoteButton.validateEmote(text);
			}
		}

		public void RefreshPosition()
		{
			UpdatePosition(CurrentPosition);
		}

		public void UpdatePosition(float x)
		{
			CurrentPosition = x;
			emoteButtons.BasePosition = x;
			List<double> map = emoteButtons.GetMap();
			for (int i = 0; i < emoteButtons.Count; i++)
			{
				emoteButtons[i].UpdatePosition((float)map[i], buttonOffsetY, buttonPercentageX);
			}
		}

		public void GoLeft()
		{
			CurrentTargetPosition -= 1f;
			base.AnimationPieceManager.SwapOut(ref currentMovementAnimation, AnimClipBuilder.Custom.Function(SHSAnimations.GenericPaths.LinearWithBounce(CurrentPosition, CurrentTargetPosition, 0.3f), UpdatePosition));
		}

		public void GoRight()
		{
			CurrentTargetPosition += 1f;
			base.AnimationPieceManager.SwapOut(ref currentMovementAnimation, AnimClipBuilder.Custom.Function(SHSAnimations.GenericPaths.LinearWithBounce(CurrentPosition, CurrentTargetPosition, 0.3f), UpdatePosition));
		}
	}

	public class EmoteChatAnim : SHSAnimations
	{
		public static AnimClip ToOpenChat(SHSEmoteChatBar bar)
		{
			return SlideOpenChatWidget(bar, 0.4f, true) ^ SlideEmoteBar(bar, 0.4f, false);
		}

		public static AnimClip ToEmoteChat(SHSEmoteChatBar bar)
		{
			return SlideOpenChatWidget(bar, 0.4f, false) ^ SlideEmoteBar(bar, 0.4f, true);
		}

		public static AnimClip SlideEmoteBar(SHSEmoteChatBar bar, float animationTime, bool toEmoteBar)
		{
			Vector2 start = (!toEmoteBar) ? EmoteBarEmotes.VISIBLE_OFFSET : EmoteBarEmotes.HIDDEN_OFFSET;
			Vector2 end = (!toEmoteBar) ? EmoteBarEmotes.HIDDEN_OFFSET : EmoteBarEmotes.VISIBLE_OFFSET;
			AnimPath path = GenericPaths.LinearWithBounce(0f, 1f, animationTime);
			return Custom.Function(path, delegate(float t)
			{
				bar.iconBar.Offset = Vector2.Lerp(start, end, t);
			});
		}

		public static AnimClip SlideOpenChatWidget(SHSEmoteChatBar bar, float animationTime, bool toOpenChat)
		{
			Vector2 start = (!toOpenChat) ? OpenChatWidget.VISIBLE_OFFSET : OpenChatWidget.HIDDEN_OFFSET;
			Vector2 end = (!toOpenChat) ? OpenChatWidget.HIDDEN_OFFSET : OpenChatWidget.VISIBLE_OFFSET;
			AnimPath path = GenericPaths.LinearWithBounce(0f, 1f, animationTime);
			return Custom.Function(path, delegate(float t)
			{
				bar.openChatWidget.Offset = Vector2.Lerp(start, end, t);
			});
		}

		public static float LocationX(float loc, float offset)
		{
			return Mathf.Pow(loc, 0.9f) * 62f + offset;
		}

		public static float LocationY(float loc, float offset)
		{
			return loc * offset;
		}
	}

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

		public EmoteGoodiesBar(SHSEmoteChatBar ParentWindow)
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
				AppShell.Instance.EventMgr.Fire(this, new ToggleButtonVisibilityMessage(SHSHudWheels.ButtonType.Inventory));
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

	public class CircularScrollBar : GUISimpleControlWindow
	{
		public class CircularScrollButton : GUISimpleControlWindow
		{
			protected GUIAnimatedButton buttonImage;

			protected bool enabled = true;

			protected Vector2 buttonSize;

			protected float baseYOffset;

			public CircularScrollButton(string textureSource, Vector2 size)
			{
				buttonSize = size;
				SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle);
				SetSize(buttonSize);
				buttonImage = GUIControl.CreateControlFrameCentered<GUIAnimatedButton>(new Vector2(128f, 128f), Vector2.zero);
				buttonImage.TextureSource = textureSource;
				buttonImage.SetupButton(0.9f, 1f, 0.8f);
				buttonImage.HitTestType = HitTestTypeEnum.Circular;
				buttonImage.HitTestSize = new Vector2(0.4f, 0.4f);
				Add(buttonImage);
			}

			public void UpdatePosition(float pos, float OffsetY, float percentageX)
			{
				pos *= percentageX;
				float num = Mathf.Abs(pos);
				float x = Mathf.Sign(pos) * EmoteChatAnim.LocationX(num, -2f);
				float num2 = 0f - EmoteChatAnim.LocationY(num, 0f) - OffsetY;
				Offset = new Vector2(x, num2 + baseYOffset);
				float num3 = buttonSize.x * (1f - 0.1f * num) - Mathf.Clamp01(num - 2f) * 10f;
				buttonImage.SetSize(num3, num3);
				SetSize(num3, num3);
				Rotation = pos * 10f;
				if (num < 3.5f)
				{
					IsVisible = true;
					Alpha = Mathf.Clamp01(3.5f - num);
				}
				else
				{
					IsVisible = false;
				}
			}
		}

		public class EmoteButton : CircularScrollButton
		{
			private EmotesDefinition.EmoteDefinition def;

			public EmoteButton(EmotesDefinition.EmoteDefinition def)
				: base("communication_bundle|emote_" + def.command, new Vector2(128f, 128f))
			{
				this.def = def;
				buttonImage.Click += delegate(GUIControl sender, GUIClickEvent EventData)
				{
					if (enabled)
					{
						AppShell.Instance.EventMgr.Fire(sender, new EmoteMessage(GameController.GetController().LocalPlayer, def.id));
					}
				};
				string name = "#emotechat_" + def.command;
				buttonImage.ToolTip = new NamedToolTipInfo(name);
			}

			public override void OnShow()
			{
				base.OnShow();
				AppShell.Instance.EventMgr.AddListener<CharacterSelectedMessage>(OnCharacterSelected);
				AppShell.Instance.EventMgr.AddListener<LeveledUpMessage>(OnLeveledUp);
				validateEmote();
			}

			public override void OnHide()
			{
				base.OnHide();
				AppShell.Instance.EventMgr.RemoveListener<CharacterSelectedMessage>(OnCharacterSelected);
				AppShell.Instance.EventMgr.RemoveListener<LeveledUpMessage>(OnLeveledUp);
			}

			private void OnCharacterSelected(CharacterSelectedMessage message)
			{
				validateEmote();
			}

			private void OnLeveledUp(LeveledUpMessage msg)
			{
				validateEmote();
			}

			public void validateEmote()
			{
				string text;
				if (AppShell.Instance.Profile != null)
				{
					text = AppShell.Instance.Profile.SelectedCostume;
					if (string.IsNullOrEmpty(text))
					{
						text = AppShell.Instance.Profile.LastSelectedCostume;
					}
				}
				else
				{
					text = GameController.GetController().LocalPlayer.name;
				}
				string failReason;
				bool flag = enabled = EmotesDefinition.Instance.RequirementsCheck(def.id, text, out failReason);
				buttonImage.TextureSource = ((!flag) ? "communication_bundle|emote_powerlocked" : ("communication_bundle|emote_" + def.command));
				buttonImage.ToolTip = new NamedToolTipInfo((!flag) ? failReason : ("#emotechat_" + def.command));
			}
		}

		public class GoodieButton : CircularScrollButton
		{
			protected Expendable expendableDef;

			protected GUILabel ItemCount;

			protected GUIImage NumberOfItems;

			protected bool isBuyable;

			protected string toolTipText;

			public string ExpendableId
			{
				get
				{
					return expendableDef.Definition.OwnableTypeId;
				}
			}

			public int Quantity
			{
				get
				{
					return expendableDef.Quantity;
				}
			}

			public GoodieButton(Expendable expendable)
				: base(expendable.Definition.EmoteBarIcon + "_normal", new Vector2(128f, 128f))
			{
				expendableDef = expendable;
				if (AppShell.Instance.NewShoppingManager != null && AppShell.Instance.NewShoppingManager.itemForSale(int.Parse(expendableDef.Definition.OwnableTypeId)))
				{
					isBuyable = true;
				}
				else
				{
					isBuyable = false;
				}
				buttonImage.Click += delegate
				{
					if (Quantity <= 0 && isBuyable)
					{
						ShoppingWindow shoppingWindow = new ShoppingWindow(int.Parse(expendableDef.Definition.OwnableTypeId));
						shoppingWindow.launch();
					}
					else if (enabled)
					{
						AppShell.Instance.ExpendablesManager.UseExpendable(expendableDef.Definition.OwnableTypeId);
						if (Quantity <= 1)
						{
							enabled = false;
							buttonImage.IsEnabled = false;
						}
					}
				};
				NumberOfItems = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(26f, 26f), new Vector2(16f, 16f));
				NumberOfItems.TextureSource = "persistent_bundle|inventory_stacked_indicator";
				NumberOfItems.Alpha = 0f;
				Add(NumberOfItems);
				ItemCount = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(26f, 26f), new Vector2(16f, 16f));
				ItemCount.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(26, 39, 62), TextAnchor.MiddleCenter);
				ItemCount.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Ignore;
				ItemCount.Alpha = 0f;
				Add(ItemCount);
				buttonImage.MouseOver += delegate
				{
					buttonImage.TextureSource = expendable.Definition.EmoteBarIcon + "_highlight";
					NumberOfItems.Alpha = 1f;
					ItemCount.Alpha = 1f;
				};
				buttonImage.MouseOut += delegate
				{
					buttonImage.TextureSource = expendable.Definition.EmoteBarIcon + "_normal";
					NumberOfItems.Alpha = 0f;
					ItemCount.Alpha = 0f;
				};
				toolTipText = expendable.Definition.Name;
				buttonImage.ToolTip = new NamedToolTipInfo(toolTipText, new Vector2(0f, -16f));
			}

			public void ValidateGoodie()
			{
				PrerequisiteCheckResult prerequisiteCheckResult = AppShell.Instance.ExpendablesManager.CanExpend(expendableDef.Definition);
				Vector2 vector = new Vector2(0f, 0f);
				string a = toolTipText;
				if (Quantity <= 0)
				{
					if (!isBuyable)
					{
						buttonImage.IsEnabled = false;
						enabled = false;
						toolTipText = "#EXP_NO_MORE";
						vector = new Vector2(0f, -16f);
					}
					else if (prerequisiteCheckResult.State == PrerequisiteCheckStateEnum.Usable)
					{
						toolTipText = "#EXP_BUY_MORE";
						vector = new Vector2(0f, -16f);
						buttonImage.IsEnabled = true;
						enabled = true;
					}
					else
					{
						toolTipText = prerequisiteCheckResult.StateExplanation;
						vector = new Vector2(-50f, -16f);
						buttonImage.IsEnabled = false;
						enabled = false;
					}
				}
				else
				{
					enabled = (prerequisiteCheckResult.State == PrerequisiteCheckStateEnum.Usable);
					if (enabled)
					{
						toolTipText = expendableDef.Definition.Name;
						vector = new Vector2(0f, -16f);
					}
					else
					{
						toolTipText = prerequisiteCheckResult.StateExplanation;
						vector = new Vector2(-50f, -16f);
					}
					buttonImage.IsEnabled = enabled;
				}
				if (a != toolTipText)
				{
					buttonImage.ToolTip = new NamedToolTipInfo(toolTipText, vector);
					if (GUIManager.Instance != null && GUIManager.Instance.TooltipManager != null && GUIManager.Instance.TooltipManager.CurrentOverControl == buttonImage)
					{
						GUIManager.Instance.TooltipManager.RefreshToolTip();
					}
				}
				ItemCount.Text = Quantity.ToString();
			}
		}

		public CircularList<CircularScrollButton> buttons;

		public float buttonOffsetY = -110f;

		public float buttonPercentageX;

		private float CurrentTargetPosition = 2.5f;

		private float CurrentPosition = 2.5f;

		private AnimClip currentMovementAnimation;

		public CircularScrollBar(CircularList<CircularScrollButton> Buttons)
		{
			buttons = Buttons;
			foreach (CircularScrollButton button in buttons)
			{
				Add(button);
			}
		}

		public override void OnShow()
		{
			base.OnShow();
			RefreshPosition();
		}

		public void AddButton(CircularScrollButton button)
		{
			if (!buttons.Contains(button))
			{
				buttons.Add(button);
				Add(button);
				RefreshPosition();
			}
		}

		public void ResetButtons(CircularList<CircularScrollButton> Buttons)
		{
			for (int num = buttons.Count - 1; num >= 0; num--)
			{
				RemoveButton(buttons[num]);
			}
			buttons.Clear();
			buttons = Buttons;
			foreach (CircularScrollButton button in buttons)
			{
				Add(button);
			}
			RefreshPosition();
		}

		public void RemoveButton(CircularScrollButton button)
		{
			if (buttons.Contains(button))
			{
				button.Hide();
				buttons.Remove(button);
				Remove(button);
			}
		}

		public void RefreshPosition()
		{
			UpdatePosition(CurrentPosition);
		}

		public void UpdatePosition(float x)
		{
			CurrentPosition = x;
			buttons.BasePosition = x;
			List<double> map = buttons.GetMap();
			for (int i = 0; i < buttons.Count; i++)
			{
				buttons[i].UpdatePosition((float)map[i], buttonOffsetY, buttonPercentageX);
			}
		}

		public void GoLeft()
		{
			CurrentTargetPosition -= 1f;
			base.AnimationPieceManager.SwapOut(ref currentMovementAnimation, AnimClipBuilder.Custom.Function(SHSAnimations.GenericPaths.LinearWithBounce(CurrentPosition, CurrentTargetPosition, 0.3f), UpdatePosition));
		}

		public void GoRight()
		{
			CurrentTargetPosition += 1f;
			base.AnimationPieceManager.SwapOut(ref currentMovementAnimation, AnimClipBuilder.Custom.Function(SHSAnimations.GenericPaths.LinearWithBounce(CurrentPosition, CurrentTargetPosition, 0.3f), UpdatePosition));
		}
	}

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

		public SHSEmoteChatBar mainWindow;

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

		public OpenChatWidget(SHSEmoteChatBar mainWindow)
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
				AppShell.Instance.EventReporter.ReportOpenChatMessage(AppShell.Instance.PlayerDictionary.GetPlayerId(AppShell.Instance.ServerConnection.GetGameUserId()), AppShell.Instance.ServerConnection.GetRoomName(), entryBox.Text);
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

	private OpenChatWidget openChatWidget;

	private GUISimpleControlWindow iconBar;

	public static bool CurrentVisibility = true;

	public static bool IsOpenChatVisible;

	private bool openChatVisible;

	private bool animationInProgress;

	public SHSEmoteChatBar()
	{
		CspUtils.DebugLog("SHSEmoteChatBar()");
		SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle);
		SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		Traits.HitTestType = HitTestTypeEnum.Transparent;
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		EmoteGoodiesBar emoteBarEmotes = new EmoteGoodiesBar(this);
		Add(emoteBarEmotes);
		openChatWidget = new OpenChatWidget(this);
		Add(openChatWidget);
		base.AnimationOnOpen = delegate
		{
			return EmoteGoodiesBar.AnimOpenAndClose.Open(emoteBarEmotes);
		};
		base.AnimationOnClose = delegate
		{
			return EmoteGoodiesBar.AnimOpenAndClose.Close(emoteBarEmotes);
		};
		base.AnimationOpenFinished += delegate
		{
			base.AnimationPieceManager.Add(EmoteGoodiesBar.AnimOpenAndClose.AnimateOpenComplete(emoteBarEmotes));
		};
		iconBar = emoteBarEmotes;
	}

	public void RequestOpenOpenChat()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		if (!animationInProgress && !base.AnimationInProgress)
		{
			if (openChatVisible)
			{
				openChatWidget.Focus();
				return;
			}
			animationInProgress = true;
			AnimClip animClip = EmoteChatAnim.ToOpenChat(this);
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				animationInProgress = false;
				openChatWidget.Focus();
				openChatVisible = true;
			};
			base.AnimationPieceManager.Add(animClip);
		}
	}

	public void RequestOpenEmoteChat()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		if (!animationInProgress && !base.AnimationInProgress)
		{
			animationInProgress = true;
			AnimClip animClip = EmoteChatAnim.ToEmoteChat(this);
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				animationInProgress = false;
				openChatVisible = false;
			};
			base.AnimationPieceManager.Add(animClip);
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		IsOpenChatVisible = true;
	}

	public override void OnHide()
	{
		base.OnHide();
		IsOpenChatVisible = false;
	}
}
