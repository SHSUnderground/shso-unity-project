using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSCardGameBoosterPackWindow : GUIDynamicWindow
{
	public class BoosterPackSelection : SHSSelectionWindow<BoosterPack, GUIDrawTexture>
	{
		public BoosterPackSelection(GUISlider slider)
			: base(slider, 480f, new Vector2(97f, 126f), 12)
		{
			SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
			TopOffsetAdjustHeight = 0f;
			BottomOffsetAdjustHeight = 0f;
		}
	}

	public class BoosterPack : SHSSelectionItem<GUIDrawTexture>, IComparable<BoosterPack>
	{
		public int boosterPackID = -1;

		public BoosterPack(int id)
		{
			boosterPackID = id;
			item = new GUIDrawTexture();
			item.SetSize(new Vector2(97f, 126f));
			item.TextureSource = "shopping_bundle|L_shopping_booster_pack_Unleashed";
			item.ToolTip = new NamedToolTipInfo(id.ToString());
			OwnableDefinition def = OwnableDefinition.getDef(boosterPackID);
			if (def != null)
			{
				item.TextureSource = string.Format("shopping_bundle|L_shopping_booster_pack_{0}", def.name);
				item.ToolTip = new NamedToolTipInfo(def.shoppingName);
			}
			else
			{
				item.TextureSource = string.Empty;
			}
			itemSize = new Vector2(97f, 126f);
			currentState = SelectionState.Active;
		}

		public int CompareTo(BoosterPack other)
		{
			return boosterPackID.CompareTo(other.boosterPackID);
		}
	}

	private BoosterPackSelection selection;

	private GUISlider slider;

	public SHSCardGameBoosterPackWindow()
	{
		SetSize(new Vector2(670f, 442f));
		SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -50f));
		GUIImage gUIImage = new GUIImage();
		gUIImage.TextureSource = "deckbuilder_bundle|L_deckbuilder_boosterpacks_container";
		gUIImage.SetSize(new Vector2(670f, 442f));
		gUIImage.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
		Add(gUIImage);
		slider = new GUISlider();
		slider.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(-55f, 14f));
		slider.SetSize(50f, 263f);
		slider.IsVisible = false;
		slider.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		selection = new BoosterPackSelection(slider);
		selection.Id = "boosterPackSelection";
		selection.SetSize(new Vector2(480f, 268f));
		selection.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 15f));
		Add(selection);
		Add(slider);
		GUIButton gUIButton = new GUIButton();
		gUIButton.Id = "closeBoosterPackButton";
		gUIButton.SetPositionAndSize(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(-60f, 43f), new Vector2(45f, 45f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|button_close");
		gUIButton.Click += delegate
		{
			Hide();
			DeckBuilderController deckBuilderController = (DeckBuilderController)GameController.GetController();
			deckBuilderController.FetchCardCollection();
		};
		Add(gUIButton);
		GUILabel gUILabel = new GUILabel();
		gUILabel.Text = "Click to unwrap!";
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 20, Color.white, TextAnchor.MiddleCenter);
		gUILabel.SetPositionAndSize(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 85f), new Vector2(300f, 20f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Add(gUILabel);
		EnumerateBoosterPacks();
	}

	private void AddNewButton(int boosterId)
	{
		BoosterPack boosterPack = new BoosterPack(boosterId);
		selection.AddItem(boosterPack);
		boosterPack.item.Id = "boosterPack" + boosterId.ToString() + "-" + selection.items.Count;
		selection.SortItemList();
		boosterPack.item.Click += delegate
		{
			GUIManager.Instance.ShowDynamicWindow(new SHSCardGameBoosterPackContentsWindow(), ModalLevelEnum.Default);
			BoosterPackService.OpenBoosterPack(boosterId, delegate(ShsWebResponse response)
			{
				if (response.Status == 200)
				{
					EnumerateBoosterPacks();
				}
			});
		};
	}

	public void EnumerateBoosterPacks()
	{
		AppShell.Instance.Profile.StartBoosterPacksFetch(delegate
		{
			selection.ClearItems();
			AvailableBoosterPacksCollection availableBoosterPacks = AppShell.Instance.Profile.AvailableBoosterPacks;
			int num = 0;
			foreach (KeyValuePair<string, AvailableBoosterPack> item in availableBoosterPacks)
			{
				AvailableBoosterPack value = item.Value;
				num += value.Quantity;
				for (int i = 0; i < value.Quantity; i++)
				{
					AddNewButton(int.Parse(value.BoosterPackId));
				}
			}
			if (num > 8)
			{
				slider.IsVisible = true;
			}
			else
			{
				slider.IsVisible = false;
			}
			CspUtils.DebugLog("User has " + num + " booster packs");
		});
	}
}
