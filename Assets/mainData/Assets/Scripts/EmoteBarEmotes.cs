using System.Collections.Generic;
using UnityEngine;

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

	public EmoteBarEmotes(NewEmoteChatBar ParentWindow)
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
