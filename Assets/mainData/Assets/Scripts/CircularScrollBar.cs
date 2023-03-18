using System.Collections.Generic;
using UnityEngine;

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
				AppShell.Instance.EventMgr.Fire(sender, new EmoteMessage(GameController.GetController().LocalPlayer, def.id));
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
			if (AppShell.Instance.Profile == null)
			{
				text = ((!(GameController.GetController() == null) && !(GameController.GetController().LocalPlayer == null)) ? GameController.GetController().LocalPlayer.name : "falcon");
			}
			else
			{
				text = AppShell.Instance.Profile.SelectedCostume;
				if (string.IsNullOrEmpty(text))
				{
					text = AppShell.Instance.Profile.LastSelectedCostume;
				}
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
			if (AppShell.Instance.NewShoppingManager != null)
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
