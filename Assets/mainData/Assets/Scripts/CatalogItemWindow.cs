using System;
using System.Collections.Generic;
using UnityEngine;

public class CatalogItemWindow : GUISimpleControlWindow
{
	private GUIButton _background;

	public CatalogItem item;

	private GUISimpleControlWindow _content;

	private GUISimpleControlWindow _priceContent;

	private GUIImage _ownedTag;

	public string actualName = string.Empty;

	public static Vector2 MAX_ICON_SIZE = new Vector2(163f, 163f);

	public CatalogItemWindow(ShoppingWindowContentPanel parentPanel, CatalogItem item)
	{
		this.item = item;
		_background = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(240f, 235f), new Vector2(0f, 0f));
		if (item.agentOnly)
		{
			_background.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|itemBGagent", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		}
		else
		{
			_background.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|itemBG", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		}
		_background.Click += delegate
		{
			parentPanel.selectItem(item);
		};
		Add(_background);
		_content = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(new Vector2(240f, 235f), new Vector2(0f, 0f));
		_content.Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		_content.Traits.HitTestType = HitTestTypeEnum.Transparent;
		_content.SetControlFlag(ControlFlagSetting.HitTestIgnore, true, false);
		Add(_content);
		GUISimpleControlWindow icon = item.ownableDef.getIcon(MAX_ICON_SIZE);
		float x = MAX_ICON_SIZE.x;
		Vector2 size = icon.Size;
		float x2 = 15f + (x - size.x) / 2f;
		float y = MAX_ICON_SIZE.y;
		Vector2 size2 = icon.Size;
		icon.Position = new Vector2(x2, 55f + (y - size2.y) / 2f);
		icon.IsVisible = true;
		_content.Add(icon);
		CatalogItem.Flag[] array = (CatalogItem.Flag[])Enum.GetValues(typeof(CatalogItem.Flag));
		foreach (CatalogItem.Flag flag in array)
		{
			if ((item.flags & (int)flag) != 0)
			{
				attachTag(flag);
			}
		}
		Color color = GUILabel.GenColor(0, 0, 0);
		if (item.agentOnly)
		{
			color = GUILabel.GenColor(98, 34, 164);
		}
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(7f, 5f), new Vector2(230f, 48f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(255, 255, 255), color, color, new Vector2(0f, 4f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel.BackColorAlpha = 1f;
		gUIStrokeTextLabel.WordWrap = true;
		gUIStrokeTextLabel.VerticalKerning += 5;
		gUIStrokeTextLabel.Text = item.name;
		gUIStrokeTextLabel.Id = "itemNameLabel";
		Add(gUIStrokeTextLabel);
		actualName = gUIStrokeTextLabel.Text;
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetSize(new Vector2(92f, 54f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage.Position = new Vector2(135f, 166f);
		gUIImage.TextureSource = "shopping_bundle|priceoverlay";
		gUIImage.Id = "priceOverlay";
		Add(gUIImage);
		_ownedTag = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(72f, 62f), new Vector2(150f, 160f));
		_ownedTag.TextureSource = "shopping_bundle|tag_owned";
		Add(_ownedTag);
		_ownedTag.IsVisible = false;
		if (OwnableDefinition.isUniqueAndOwned(item.ownableTypeID, AppShell.Instance.Profile))
		{
			_ownedTag.IsVisible = true;
		}
		else
		{
			_priceContent = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(gUIImage.Size, gUIImage.Position);
			Add(_priceContent);
			_priceContent.IsVisible = true;
			if (OwnableDefinition.isUniqueItem(item.ownableTypeID))
			{
				AppShell.Instance.EventMgr.AddListener<ShoppingItemPurchasedMessage>(OnShoppingItemPurchasedMessage);
			}
			float num = 1f;
			Entitlements.ServerValueEntitlement serverValueEntitlement = Singleton<Entitlements>.instance.EntitlementsSet[Entitlements.EntitlementFlagEnum.SubscriptionType] as Entitlements.ServerValueEntitlement;
			if (serverValueEntitlement.Value == "12")
			{
				num = 0.900001f;
			}
			Vector2 vector = new Vector2(4f, 4f);
			if (item.goldPrice <= 0 || item.shardPrice <= 0)
			{
				vector += new Vector2(0f, 11f);
			}
			if (item.goldPrice > 0)
			{
				GUIImage gUIImage2 = new GUIImage();
				gUIImage2.SetSize(new Vector2(27f, 24f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				gUIImage2.Position = vector;
				gUIImage2.TextureSource = "shopping_bundle|gold_icon";
				gUIImage2.Id = "goldIcon";
				_priceContent.Add(gUIImage2);
				GUIStrokeTextLabel gUIStrokeTextLabel2 = new GUIStrokeTextLabel();
				gUIStrokeTextLabel2.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, vector + new Vector2(27f, 1f), new Vector2(63f, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				gUIStrokeTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 226, 90), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.UpperLeft);
				gUIStrokeTextLabel2.BackColorAlpha = 1f;
				gUIStrokeTextLabel2.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
				gUIStrokeTextLabel2.Text = ShoppingWindow.convertNumber((int)((float)item.goldPrice * (float)item.quantity * num));
				gUIStrokeTextLabel2.Id = "goldPriceLabel";
				_priceContent.Add(gUIStrokeTextLabel2);
				vector += new Vector2(0f, 23f);
			}
			if (item.shardPrice > 0)
			{
				GUIImage gUIImage3 = new GUIImage();
				gUIImage3.SetSize(new Vector2(21f, 26f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				gUIImage3.Position = vector + new Vector2(3f, 0f);
				gUIImage3.TextureSource = "shopping_bundle|newfractal_shopping";
				gUIImage3.Id = "shardIcon";
				_priceContent.Add(gUIImage3);
				GUIStrokeTextLabel gUIStrokeTextLabel3 = new GUIStrokeTextLabel();
				gUIStrokeTextLabel3.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, vector + new Vector2(27f, 1f), new Vector2(63f, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				gUIStrokeTextLabel3.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 136, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.UpperLeft);
				gUIStrokeTextLabel3.BackColorAlpha = 1f;
				gUIStrokeTextLabel3.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
				gUIStrokeTextLabel3.Text = ShoppingWindow.convertNumber((int)((float)item.shardPrice * (float)item.quantity * num));
				gUIStrokeTextLabel3.Id = "shardPriceLabel";
				_priceContent.Add(gUIStrokeTextLabel3);
			}
		}
		int num2 = item.quantity;
		if (item.ownableDef.category == OwnableDefinition.Category.MoneyBag && item.ownableDef.metadata != string.Empty)
		{
			num2 = int.Parse(item.ownableDef.metadata);
		}
		if (num2 > 1)
		{
			GUIImage gUIImage4 = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(102f, 60f), new Vector2(135f, 61f));
			gUIImage4.TextureSource = "shopping_bundle|shopping_item_quantity_container";
			Add(gUIImage4);
			GUILabel gUILabel = GUIControl.CreateControlTopLeftFrame<GUILabel>(new Vector2(60f, 35f), gUIImage4.Position + new Vector2(25f, 15f));
			gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 33, GUILabel.GenColor(0, 120, 255), TextAnchor.MiddleCenter);
			gUILabel.Text = string.Format("{0:n0}", num2);
			gUILabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
			gUILabel.Rotation = -5f;
			Add(gUILabel);
		}
		int num3 = 0;
		List<GUIControl> list = new List<GUIControl>();
		Vector2 a = new Vector2(192f, 51f);
		int num4 = 28;
		foreach (Keyword keyword in item.ownableDef.getKeywords())
		{
			if (keyword.hasContext("store") && keyword.icon.Length > 0)
			{
				GUIImageWithEvents gUIImageWithEvents = GUIControl.CreateControlTopLeftFrame<GUIImageWithEvents>(new Vector2(26f, 26f), a + new Vector2(0f, num3 * num4));
				gUIImageWithEvents.TextureSource = keyword.icon;
				Add(gUIImageWithEvents);
				list.Add(gUIImageWithEvents);
				gUIImageWithEvents.Id = keyword.keyword;
				gUIImageWithEvents.IsVisible = true;
				gUIImageWithEvents.ToolTip = new NamedToolTipInfo(keyword.tooltip, new Vector2(20f, 0f));
				num3++;
			}
		}
		if (item.ownableDef.category == OwnableDefinition.Category.Sidekick)
		{
			PetData data = PetDataManager.getData(item.ownableTypeID);
			if (data != null)
			{
				foreach (SpecialAbility ability in data.abilities)
				{
					GUISimpleControlWindow gUISimpleControlWindow = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(new Vector2(30f, 30f), a + new Vector2(0f, num3 * num4));
					Add(gUISimpleControlWindow);
					gUISimpleControlWindow.IsVisible = true;
					list.Add(gUISimpleControlWindow);
					string id = string.Empty;
					GUIImageWithEvents gUIImageWithEvents;
					if (ability.requiredOwnable > 0)
					{
						OwnableDefinition def = OwnableDefinition.getDef(ability.requiredOwnable);
						gUIImageWithEvents = GUIControl.CreateControlTopLeftFrame<GUIImageWithEvents>(new Vector2(30f, 30f), Vector2.zero);
						if (def.name.Substring(def.name.Length - 1, 1) == "2")
						{
							gUIImageWithEvents.TextureSource = "shopping_bundle|badge";
							id = "#CRAFT_SIDEKICK_GOLD";
						}
						else
						{
							gUIImageWithEvents.TextureSource = "shopping_bundle|badge_silver";
							id = "#CRAFT_SIDEKICK_SILVER";
						}
						gUISimpleControlWindow.Add(gUIImageWithEvents);
						gUIImageWithEvents.Id = ability.name;
						gUIImageWithEvents.IsVisible = true;
					}
					gUIImageWithEvents = GUIControl.CreateControlTopLeftFrame<GUIImageWithEvents>(new Vector2(26f, 26f), new Vector2(2f, 2f));
					gUIImageWithEvents.TextureSource = ability.icon;
					gUISimpleControlWindow.Add(gUIImageWithEvents);
					gUIImageWithEvents.Id = ability.name;
					gUIImageWithEvents.IsVisible = true;
					gUIImageWithEvents.ToolTip = new NamedToolTipInfo(AppShell.Instance.stringTable.GetString(id) + " " + AppShell.Instance.stringTable.GetString(ability.name), new Vector2(20f, 0f));
					num3++;
				}
			}
		}
		if (num3 > 4)
		{
			num4 = 20;
			for (int j = 0; j < list.Count; j++)
			{
				GUIControl gUIControl = list[j];
				if (j % 2 == 0)
				{
					gUIControl.SetPosition(a + new Vector2(2f, 2 + j * num4));
				}
				else
				{
					gUIControl.SetPosition(a + new Vector2(-14f, 2 + j * num4));
				}
			}
		}
		SetSize(new Vector2(240f, 235f));
	}

	private void OnShoppingItemPurchasedMessage(ShoppingItemPurchasedMessage message)
	{
		if (OwnableDefinition.isUniqueItem(item.ownableTypeID) && message.OwnableId == string.Empty + item.ownableTypeID)
		{
			_priceContent.IsVisible = false;
			Remove(_priceContent);
			_priceContent = null;
			_ownedTag.IsVisible = true;
		}
	}

	private void attachTag(CatalogItem.Flag flag)
	{
		Vector2 size = new Vector2(116f, 90f);
		Vector2 offset = new Vector2(13f, 131f);
		switch (flag)
		{
		case CatalogItem.Flag.IsNew:
		{
			GUIImage gUIImage = GUIControl.CreateControlTopLeftFrame<GUIImage>(size, offset);
			gUIImage.TextureSource = "shopping_bundle|tag_new";
			Add(gUIImage);
			break;
		}
		case CatalogItem.Flag.Featured:
		{
			GUIImage gUIImage = GUIControl.CreateControlTopLeftFrame<GUIImage>(size, offset);
			gUIImage.TextureSource = "shopping_bundle|tag_featured";
			Add(gUIImage);
			break;
		}
		case CatalogItem.Flag.OnSale:
		{
			GUIImage gUIImage = GUIControl.CreateControlTopLeftFrame<GUIImage>(size, offset);
			gUIImage.TextureSource = "shopping_bundle|tag_sale";
			Add(gUIImage);
			break;
		}
		case CatalogItem.Flag.OneDay:
		{
			GUIImage gUIImage = GUIControl.CreateControlTopLeftFrame<GUIImage>(size, offset);
			gUIImage.TextureSource = "shopping_bundle|tag_oneday";
			Add(gUIImage);
			break;
		}
		case CatalogItem.Flag.OneWeek:
		{
			GUIImage gUIImage = GUIControl.CreateControlTopLeftFrame<GUIImage>(size, offset);
			gUIImage.TextureSource = "shopping_bundle|tag_oneweek";
			Add(gUIImage);
			break;
		}
		case CatalogItem.Flag.EarlyAccess:
		{
			GUIImage gUIImage = GUIControl.CreateControlTopLeftFrame<GUIImage>(size, offset);
			gUIImage.TextureSource = "shopping_bundle|tag_earlyaccess";
			Add(gUIImage);
			break;
		}
		}
	}

	public void OnDestroy()
	{
		CspUtils.DebugLog("OnDestroy");
		AppShell.Instance.EventMgr.RemoveListener<ShoppingItemPurchasedMessage>(OnShoppingItemPurchasedMessage);
	}
}
