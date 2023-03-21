using System;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingWindowDetailPanel : GUIDialogWindow
{
	public const float SHOPPING_TIMEOUT_TIME = 15f;

	private GUIImage _background;

	private GUIImage _burst;

	private CatalogItem _item;

	private GUISimpleControlWindow _iconHolder;

	private GUISimpleControlWindow _icon;

	private GUIStrokeTextLabel _itemNameLabel;

	private GUIImage _purchasedIcon;

	private GUISimpleControlWindow _buyWithGoldButton;

	private GUIButton _actualGoldButton;

	private GUIStrokeTextLabel _goldPriceLabel;

	private GUISimpleControlWindow _buyWithGoldDisabledButton;

	private GUIStrokeTextLabel _goldPriceDisabledLabel;

	private GUISimpleControlWindow _buyWithFractalsButton;

	private GUIButton _actualFractalButton;

	private GUIStrokeTextLabel _fractalPriceLabel;

	private GUISimpleControlWindow _buyWithFractalsDisabledButton;

	private GUIStrokeTextLabel _fractalPriceDisabledLabel;

	private GUIImage _quantityIndicator;

	private GUILabel _quantityLabel;

	private GUIImage _agentBadge;

	private GUILabel _itemDescLabel;

	private AnimClip _timeoutCounter;

	private bool _purchaseAcknowledgeMessageRecieved;

	private bool _purchaseCompletedMessageRecieved;

	private bool _purchaseInProgress;

	private string _transactionGUID;

	private GUISimpleControlWindow _abilityIconHolder;

	private List<GUIImageWithEvents> _abilityIcons = new List<GUIImageWithEvents>();

	private static Vector2 MAX_ICON_SIZE = new Vector2(150f, 150f);

	private float ggg;

	public ShoppingWindowDetailPanel()
	{
		_background = new GUIImage();
		_background.SetSize(new Vector2(244f, 399f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_background.Position = Vector2.zero;
		_background.TextureSource = "shopping_bundle|rightpanel";
		_background.Id = "Background";
		Add(_background);
		_burst = new GUIImage();
		_burst.SetSize(new Vector2(230f, 270f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_burst.Position = new Vector2(14f, 10f);
		_burst.TextureSource = "shopping_bundle|detailpanelburst";
		Add(_burst);
		_iconHolder = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(MAX_ICON_SIZE, new Vector2(30f, 18f));
		Add(_iconHolder);
		_iconHolder.IsVisible = true;
		_itemNameLabel = new GUIStrokeTextLabel();
		_itemNameLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(10f, 167f), new Vector2(227f, 98f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_itemNameLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 20, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(0f, 4f), TextAnchor.UpperCenter);
		_itemNameLabel.BackColorAlpha = 1f;
		_itemNameLabel.WordWrap = true;
		_itemNameLabel.VerticalKerning += 5;
		_itemNameLabel.Text = string.Empty;
		Add(_itemNameLabel);
		_itemDescLabel = new GUILabel();
		_itemDescLabel.FontSize = 14;
		_itemDescLabel.FontFace = GUIFontManager.SupportedFontEnum.Komica;
		_itemDescLabel.TextColor = GUILabel.GenColor(0, 0, 0);
		_itemDescLabel.TextAlignment = TextAnchor.UpperCenter;
		_itemDescLabel.WordWrap = true;
		_itemDescLabel.Text = string.Empty;
		GUILabel itemDescLabel = _itemDescLabel;
		Vector2 position = _itemNameLabel.Position;
		itemDescLabel.SetPosition(18f, position.y - 3f);
		_itemDescLabel.SetSize(217f, 120f);
		Add(_itemDescLabel);
		_agentBadge = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(53f, 54f), new Vector2(0f, 0f));
		_agentBadge.TextureSource = "shopping_bundle|agentbadge";
		Add(_agentBadge);
		_agentBadge.IsVisible = false;
		_purchasedIcon = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(54f, 47f), new Vector2(95f, 333f));
		_purchasedIcon.TextureSource = "shopping_bundle|tag_owned";
		Add(_purchasedIcon);
		_buyWithGoldButton = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(new Vector2(106f, 80f), new Vector2(0f, 304f));
		Add(_buyWithGoldButton);
		_buyWithGoldButton.IsVisible = false;
		_actualGoldButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(106f, 80f), new Vector2(0f, 0f));
		_actualGoldButton.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|buynowgold", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		_actualGoldButton.Click += delegate
		{
			initiatePurchase(true);
		};
		_buyWithGoldButton.Add(_actualGoldButton);
		_goldPriceLabel = new GUIStrokeTextLabel();
		_goldPriceLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(44f, 37f), new Vector2(53f, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_goldPriceLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 14, GUILabel.GenColor(255, 226, 90), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.UpperLeft);
		_goldPriceLabel.BackColorAlpha = 1f;
		_goldPriceLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		_goldPriceLabel.Text = string.Empty;
		_buyWithGoldButton.Add(_goldPriceLabel);
		_buyWithGoldDisabledButton = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(_buyWithGoldButton.Size, _buyWithGoldButton.Position);
		Add(_buyWithGoldDisabledButton);
		_buyWithGoldDisabledButton.IsVisible = false;
		GUIButton gUIButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(101f, 76f), new Vector2(0f, 0f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|buynowgold_disabled", SHSButtonStyleInfo.SupportedStatesEnum.Normal);
		gUIButton.Click += delegate
		{
			clickedDisabledButton(true);
		};
		_buyWithGoldDisabledButton.Add(gUIButton);
		_goldPriceDisabledLabel = new GUIStrokeTextLabel();
		_goldPriceDisabledLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(41f, 35f), new Vector2(53f, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_goldPriceDisabledLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 14, GUILabel.GenColor(213, 198, 131), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.UpperLeft);
		_goldPriceDisabledLabel.BackColorAlpha = 1f;
		_goldPriceDisabledLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		_goldPriceDisabledLabel.Text = string.Empty;
		_buyWithGoldDisabledButton.Add(_goldPriceDisabledLabel);
		_buyWithFractalsButton = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(new Vector2(106f, 80f), new Vector2(0f, 304f));
		Add(_buyWithFractalsButton);
		_buyWithFractalsButton.IsVisible = false;
		_actualFractalButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(106f, 80f), new Vector2(0f, 0f));
		_actualFractalButton.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|buynowfractals", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		_actualFractalButton.Click += delegate
		{
			initiatePurchase(false);
		};
		_buyWithFractalsButton.Add(_actualFractalButton);
		_fractalPriceLabel = new GUIStrokeTextLabel();
		_fractalPriceLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(44f, 37f), new Vector2(53f, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_fractalPriceLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 14, GUILabel.GenColor(255, 136, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.UpperLeft);
		_fractalPriceLabel.BackColorAlpha = 1f;
		_fractalPriceLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		_fractalPriceLabel.Text = string.Empty;
		_buyWithFractalsButton.Add(_fractalPriceLabel);
		_buyWithFractalsDisabledButton = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(_buyWithFractalsButton.Size, _buyWithFractalsButton.Position);
		Add(_buyWithFractalsDisabledButton);
		_buyWithFractalsDisabledButton.IsVisible = false;
		GUIButton gUIButton2 = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(101f, 76f), new Vector2(0f, 0f));
		gUIButton2.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|buynowfractals_disabled", SHSButtonStyleInfo.SupportedStatesEnum.Normal);
		gUIButton2.Click += delegate
		{
			clickedDisabledButton(false);
		};
		_buyWithFractalsDisabledButton.Add(gUIButton2);
		_fractalPriceDisabledLabel = new GUIStrokeTextLabel();
		_fractalPriceDisabledLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(41f, 35f), new Vector2(53f, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_fractalPriceDisabledLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 14, GUILabel.GenColor(225, 166, 225), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.UpperLeft);
		_fractalPriceDisabledLabel.BackColorAlpha = 1f;
		_fractalPriceDisabledLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		_fractalPriceDisabledLabel.Text = string.Empty;
		_buyWithFractalsDisabledButton.Add(_fractalPriceDisabledLabel);
		_quantityIndicator = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(102f, 50f), new Vector2(135f, 76f));
		_quantityIndicator.TextureSource = "shopping_bundle|shopping_item_quantity_container";
		Add(_quantityIndicator);
		_quantityIndicator.IsVisible = false;
		_quantityLabel = GUIControl.CreateControlTopLeftFrame<GUILabel>(new Vector2(60f, 35f), _quantityIndicator.Position + new Vector2(25f, 10f));
		_quantityLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 33, GUILabel.GenColor(0, 120, 255), TextAnchor.MiddleCenter);
		_quantityLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		_quantityLabel.Rotation = -5f;
		Add(_quantityLabel);
		_quantityLabel.IsVisible = false;
		Vector2 size = _background.Size;
		_abilityIconHolder = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(new Vector2(40f, size.y), new Vector2(210f, 30f));
		Add(_abilityIconHolder);
		_abilityIconHolder.IsVisible = true;
		SetSize(_background.Size);
	}

	public void setCatalogItem(CatalogItem item)
	{
		if (!_purchaseInProgress)
		{
			_item = item;
			refresh();
		}
	}

	private void refresh()
	{
		if (_icon != null)
		{
			_iconHolder.Remove(_icon);
			_icon.Dispose();
		}
		if (_item.ownableDef != null) // CSP added if conditional to this block
		{
			_icon = _item.ownableDef.getIcon(MAX_ICON_SIZE);
			GUISimpleControlWindow icon = _icon;
			float x = MAX_ICON_SIZE.x;
			Vector2 size = _icon.Size;
			float x2 = (x - size.x) / 2f;
			float y = MAX_ICON_SIZE.y;
			Vector2 size2 = _icon.Size;
			icon.Position = new Vector2(x2, (y - size2.y) / 2f);
			_icon.IsVisible = true;
			_iconHolder.Add(_icon);
		}

		_itemDescLabel.Text = _item.description;
		_itemNameLabel.Text = _item.name;
		float textBlockSize = GUINotificationWindow.GetTextBlockSize(new GUILabel[1]
		{
			_itemNameLabel
		}, GUINotificationWindow.BlockSizeType.Height);
		CspUtils.DebugLog("refresh ");
		CspUtils.DebugLog("**" + _item.description + "**");
		CspUtils.DebugLog("**" + _item.name + "**");
		object[] obj = new object[4]
		{
			"  ",
			null,
			null,
			null
		};
		Vector2 position = _itemNameLabel.Position;
		obj[1] = position.y;
		obj[2] = " ";
		obj[3] = textBlockSize;
		CspUtils.DebugLog(string.Concat(obj));
		GUILabel itemDescLabel = _itemDescLabel;
		Vector2 position2 = _itemNameLabel.Position;
		itemDescLabel.SetPosition(18f, position2.y + textBlockSize + 3f);
		CspUtils.DebugLog("enable refresh ");
		_actualGoldButton.IsEnabled = true;
		_actualFractalButton.IsEnabled = true;
		if (_item.agentOnly)
		{
			_agentBadge.IsVisible = true;
		}
		else
		{
			_agentBadge.IsVisible = false;
		}
		if (_item.quantity > 1)
		{
			_quantityIndicator.IsVisible = true;
			_quantityLabel.Text = string.Format("{0:n0}", _item.quantity);
			_quantityLabel.IsVisible = true;
		}
		else
		{
			_quantityIndicator.IsVisible = false;
			_quantityLabel.IsVisible = false;
		}
		_abilityIconHolder.RemoveAllControls();
		foreach (GUIImageWithEvents abilityIcon in _abilityIcons)
		{
			abilityIcon.Dispose();
		}
		_abilityIcons.Clear();
		int num = 0;

		if (_item.ownableDef != null) // CSP added if conditional to this block
		{
			foreach (Keyword keyword in _item.ownableDef.getKeywords())
			{
				if (keyword.hasContext("store") && keyword.icon.Length > 0)
				{
					GUIImageWithEvents gUIImageWithEvents = GUIControl.CreateControlTopLeftFrame<GUIImageWithEvents>(new Vector2(26f, 26f), new Vector2(0f, num * 28));
					gUIImageWithEvents.TextureSource = keyword.icon;
					_abilityIconHolder.Add(gUIImageWithEvents);
					_abilityIcons.Add(gUIImageWithEvents);
					gUIImageWithEvents.Id = keyword.keyword;
					gUIImageWithEvents.IsVisible = true;
					gUIImageWithEvents.ToolTip = new NamedToolTipInfo(keyword.tooltip, new Vector2(20f, 0f));
					num++;
				}
			}
			if (_item.ownableDef.category == OwnableDefinition.Category.Sidekick)
			{
				PetData data = PetDataManager.getData(_item.ownableTypeID);
				if (data != null)
				{
					foreach (SpecialAbility ability in data.abilities)
					{
						string id = string.Empty;
						GUIImageWithEvents gUIImageWithEvents;
						if (ability.requiredOwnable > 0)
						{
							OwnableDefinition def = OwnableDefinition.getDef(ability.requiredOwnable);
							gUIImageWithEvents = GUIControl.CreateControlTopLeftFrame<GUIImageWithEvents>(new Vector2(30f, 30f), new Vector2(0f, num * 28));
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
							_abilityIconHolder.Add(gUIImageWithEvents);
							_abilityIcons.Add(gUIImageWithEvents);
							gUIImageWithEvents.Id = ability.name;
							gUIImageWithEvents.IsVisible = true;
						}
						gUIImageWithEvents = GUIControl.CreateControlTopLeftFrame<GUIImageWithEvents>(new Vector2(26f, 26f), new Vector2(2f, num * 28 + 2));
						gUIImageWithEvents.TextureSource = ability.icon;
						_abilityIconHolder.Add(gUIImageWithEvents);
						_abilityIcons.Add(gUIImageWithEvents);
						gUIImageWithEvents.Id = ability.name;
						gUIImageWithEvents.IsVisible = true;
						gUIImageWithEvents.ToolTip = new NamedToolTipInfo(AppShell.Instance.stringTable.GetString(id) + " " + AppShell.Instance.stringTable.GetString(ability.name), new Vector2(20f, 0f));
						num++;
					}
				}
			}
		}

		refreshCurrency();
	}

	public void refreshCurrency()
	{
		if (OwnableDefinition.isUniqueAndOwned(_item.ownableTypeID, AppShell.Instance.Profile))
		{
			_purchasedIcon.IsVisible = true;
			_buyWithGoldButton.IsVisible = false;
			_buyWithFractalsButton.IsVisible = false;
			_buyWithGoldDisabledButton.IsVisible = false;
			_buyWithFractalsDisabledButton.IsVisible = false;
			return;
		}
		float num = 1f;
		Entitlements.ServerValueEntitlement serverValueEntitlement = Singleton<Entitlements>.instance.EntitlementsSet[Entitlements.EntitlementFlagEnum.SubscriptionType] as Entitlements.ServerValueEntitlement;
		if (serverValueEntitlement.Value == "12")
		{
			num = 0.900001f;
		}
		_purchasedIcon.IsVisible = false;
		_buyWithGoldButton.IsVisible = false;
		_buyWithFractalsButton.IsVisible = false;
		_buyWithGoldDisabledButton.IsVisible = false;
		_buyWithFractalsDisabledButton.IsVisible = false;
		CspUtils.DebugLog(_item.goldPrice + " " + _item.shardPrice);
		Vector2 position = new Vector2(28f, 312f);
		if (_item.goldPrice <= 0 || _item.shardPrice <= 0)
		{
			position += new Vector2(55f, 0f);
		}
		if (_item.goldPrice > 0)
		{
			_goldPriceLabel.Text = ShoppingWindow.convertNumber((int)((float)_item.goldPrice * (float)_item.quantity * num));
			_buyWithGoldButton.SetPosition(position);
			_buyWithGoldDisabledButton.SetPosition(position);
			_goldPriceDisabledLabel.Text = ShoppingWindow.convertNumber((int)((float)_item.goldPrice * (float)_item.quantity * num));
			if (AppShell.Instance.Profile.Gold < (int)((float)(_item.goldPrice * _item.quantity) * num) || (_item.agentOnly && !Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.IsPayingSubscriber) && !Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldPlayAllow)))
			{
				_buyWithGoldDisabledButton.IsVisible = true;
			}
			else
			{
				_buyWithGoldButton.IsVisible = true;
			}
			position += new Vector2(114f, 0f);
		}
		if (_item.shardPrice > 0)
		{
			_fractalPriceLabel.Text = ShoppingWindow.convertNumber((int)((float)_item.shardPrice * (float)_item.quantity * num));
			_buyWithFractalsButton.SetPosition(position);
			_fractalPriceDisabledLabel.Text = ShoppingWindow.convertNumber((int)((float)_item.shardPrice * (float)_item.quantity * num));
			_buyWithFractalsDisabledButton.SetPosition(position);
			if (AppShell.Instance.Profile.Shards < (int)((float)(_item.shardPrice * _item.quantity) * num) || (_item.agentOnly && !Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.IsPayingSubscriber) && !Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldPlayAllow)))
			{
				_buyWithFractalsDisabledButton.IsVisible = true;
			}
			else
			{
				_buyWithFractalsButton.IsVisible = true;
			}
			position += new Vector2(108f, 0f);
		}
	}

	private void clickedDisabledButton(bool goldButton)
	{
		if (_item.agentOnly && !Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.IsPayingSubscriber) && !Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldPlayAllow))
		{
			ShoppingWindow.instance.displayBuyMembership();
		}
		else if (goldButton)
		{
			ShoppingWindow.instance.displayBuyGold();
		}
		else
		{
			ShoppingWindow.instance.displayGetFractals();
		}
	}

	private void initiatePurchase(bool withGold)
	{
		ShoppingWindow.instance.resetError();
		CspUtils.DebugLog("initiatePurchase");
		_actualGoldButton.IsEnabled = false;
		_actualFractalButton.IsEnabled = false;
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("purchase_click_down"));
		AppShell.Instance.EventMgr.AddListener<ShoppingPurchaseAcknowledgeMessage>(ShoppingPurchaseAcknowledge);
		AppShell.Instance.EventMgr.AddListener<ShoppingPurchaseCompletedMessage>(ShoppingPurchaseCompleted);
		_purchaseAcknowledgeMessageRecieved = false;
		_purchaseCompletedMessageRecieved = false;
		_purchaseInProgress = true;
		if (!withGold)
		{
			_transactionGUID = ShoppingService.PurchaseItem(_item.ownableTypeID, _item.catalogOwnableID, _item.catalogOwnableSaleID, _item.quantity, 1);
		}
		else
		{
			_transactionGUID = ShoppingService.PurchaseItem(_item.ownableTypeID, _item.catalogOwnableID, _item.catalogOwnableSaleID, _item.quantity);
		}
		FireOffTimeoutCounter();
	}

	private void FireOffTimeoutCounter()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		AnimClip animClip = SHSAnimations.Generic.Wait(15f);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			ProcessShoppingResults(false, "Shopping Timeout", "No Response form server after: " + 15f + " seconds.");
		};
		base.AnimationPieceManager.SwapOut(ref _timeoutCounter, animClip);
	}

	private void ShoppingPurchaseAcknowledge(ShoppingPurchaseAcknowledgeMessage message)
	{
		if (!(_transactionGUID != message.guid))
		{
			_purchaseAcknowledgeMessageRecieved = true;
			AppShell.Instance.EventMgr.RemoveListener<ShoppingPurchaseAcknowledgeMessage>(ShoppingPurchaseAcknowledge);
			ProcessShoppingResults(message.result, "Acknowledgment of purchase failed: " + message.resultCode, message.resultReason);
		}
	}

	private void ShoppingPurchaseCompleted(ShoppingPurchaseCompletedMessage message)
	{
		if (!(_transactionGUID != message.guid))
		{
			_purchaseCompletedMessageRecieved = true;
			AppShell.Instance.EventMgr.RemoveListener<ShoppingPurchaseCompletedMessage>(ShoppingPurchaseCompleted);
			ProcessShoppingResults(message.success, "Purchase Completed Message Failed", message.result);
			CspUtils.DebugLog("ShoppingPurchaseCompleted: " + message.guid);
		}
	}

	private void ProcessShoppingResults(bool success, string nonSucessMessage, string result)
	{
		if (!success)
		{
			PurchaseHasFailed(nonSucessMessage, result);
		}
		if (_purchaseAcknowledgeMessageRecieved && _purchaseCompletedMessageRecieved)
		{
			PurchaseHasSucceeded();
		}
	}

	protected void OnHeroXpLevelResponse(ShsWebResponse response)
	{
		if (response.Status == 200)
		{
			try
			{
				CspUtils.DebugLog("OnHeroXpLevelResponse " + response.Body);
				AppShell.Instance.Profile.AvailableCostumes.UpdateItemsFromData(response.Body);
			}
			catch (Exception arg)
			{
				CspUtils.DebugLog("Exception occurred while processing the HeroXPLevelUpResponse: <" + arg + ">.");
			}
		}
		else
		{
			CspUtils.DebugLog("Unable to retrieve updated Hero XP/Level info.  Proceeding with existing info.");
		}
	}

	private void PurchaseHasSucceeded()
	{
		base.AnimationPieceManager.RemoveIfUnfinished(_timeoutCounter);
		CspUtils.DebugLog("PurchaseHasSucceeded " + _item.ownableDef.name);
		if (_item.ownableDef.category == OwnableDefinition.Category.Badge)
		{
			AppShell.Instance.Profile.Badges.Add(string.Empty + _item.ownableDef.ownableTypeID, new Badge(string.Empty + _item.ownableDef.ownableTypeID, 1));
			AppShell.Instance.WebService.StartRequest("resources$users/heroes.py", OnHeroXpLevelResponse);
		}
		else if (_item.ownableDef.category == OwnableDefinition.Category.Title)
		{
			TitleManager.awardTitle(_item.ownableDef.ownableTypeID, true);
		}
		else if (_item.ownableDef.category == OwnableDefinition.Category.Medallion)
		{
			TitleManager.awardMedallion(_item.ownableDef.ownableTypeID, true);
		}
		else if (_item.ownableDef.category == OwnableDefinition.Category.Sidekick)
		{
			PetDataManager.purchasedPetByID(_item.ownableDef.ownableTypeID, true);
		}
		else if (_item.ownableDef.category == OwnableDefinition.Category.Bundle)
		{
			AppShell.Instance.Profile.Bundles.Add(string.Empty + _item.ownableDef.ownableTypeID, new Bundle(string.Empty + _item.ownableDef.ownableTypeID, 1));
		}
		AppShell.Instance.EventMgr.Fire(this, new ShoppingItemPurchasedMessage(_item.ownableDef.category, string.Empty + _item.ownableTypeID, _item.ownableDef.name));
		int num = 1;
		if (_item.ownableDef.category != OwnableDefinition.Category.BoosterPack && _item.ownableDef.category != OwnableDefinition.Category.MysteryBox && _item.ownableDef.category == OwnableDefinition.Category.Hero)
		{
			SHSSocialMainWindow sHSSocialMainWindow = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
			if (sHSSocialMainWindow != null)
			{
				sHSSocialMainWindow.OnHeroUnlocked(_item.ownableTypeID);
			}
		}
		switch (_item.ownableDef.category)
		{
		case OwnableDefinition.Category.BoosterPack:
		{
			SHSBoosterPackOpeningWindow dialogWindow2 = new SHSBoosterPackOpeningWindow(_item.ownableTypeID, _item.ownableDef.shoppingIcon, null);
			GUIManager.Instance.ShowDynamicWindow(dialogWindow2, ModalLevelEnum.Full);
			break;
		}
		case OwnableDefinition.Category.MysteryBox:
		{
			CspUtils.DebugLog("PurchaseHasSucceeded for a mystery box");
			SHSMysteryBoxOpeningWindow dialogWindow = new SHSMysteryBoxOpeningWindow(_item.ownableTypeID, _item.ownableDef.name, null);
			GUIManager.Instance.ShowDynamicWindow(dialogWindow, ModalLevelEnum.Full);
			break;
		}
		case OwnableDefinition.Category.Bundle:
		{
			if (_item.ownableDef.metadata.Length <= 0)
			{
				break;
			}
			string[] array = _item.ownableDef.metadata.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				OwnableDefinition def = OwnableDefinition.getDef(int.Parse(text));
				if (def != null)
				{
					AppShell.Instance.EventMgr.Fire(this, new ShoppingItemPurchasedMessage(def.category, string.Empty + text, def.name));
				}
			}
			break;
		}
		}
		AppShell.Instance.Profile.StartCurrencyFetch();
		CspUtils.DebugLog("enable success ");
		_actualGoldButton.IsEnabled = true;
		_actualFractalButton.IsEnabled = true;
		_purchaseInProgress = false;
		refreshCurrency();
	}

	private string translatePurchaseErrorCode(string error)
	{
		switch (error)
		{
		case "PLAYER_NOT_ENOUGH_GOLD":
			return "#need_more_gold";
		case "PLAYER_NOT_ENOUGH_SILVER":
			return "#need_more_silver";
		case "PLAYER_NOT_ENOUGH_SHARDS":
			return "#NEED_MORE_FRACTALS";
		case "OWNABLE_TYPE_NOT_FOUND":
		case "UNKNOWN_OWNABLE_TYPE":
		case "ITEM_PRICE_NOT_FOUND":
		case "ITEM_HAS_NO_VALID_PRICE":
		case "ITEM_QUANTITY_IS_REQUIRED":
		case "INTERNAL_ERROR":
		case "ITEM_PRICE_HAS_EXPIRED":
			return "#SHOP_ERROR_GENERAL";
		case "PLAYER_ALREADY_HAS_HERO":
		case "PLAYER_ALREADY_HAS_MISSION":
		case "PLAYER_ALREADY_HAS_HQ_ROOM":
		case "PLAYER_ALREADY_HAS_QUEST":
		case "PLAYER_ALREADY_HAS_BADGE":
		case "PLAYER_ALREADY_HAS_SIDEKICK":
		case "PLAYER_ALREADY_HAS_TITLE":
		case "PLAYER_ALREADY_HAS_MEDALLION":
		case "PLAYER_ALREADY_HAS_BUNDLE":
			return "#SHOP_ERROR_OWNED";
		default:
			return "#SHOP_ERROR_GENERAL";
		}
	}

	private void PurchaseHasFailed(string nonSucessMessage, string result)
	{
		CspUtils.DebugLog("#purchase_fail");
		CspUtils.DebugLog(nonSucessMessage);
		CspUtils.DebugLog(result);
		GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, translatePurchaseErrorCode(result), new GUIDialogNotificationSink(null), ModalLevelEnum.Default);
		base.AnimationPieceManager.RemoveIfUnfinished(_timeoutCounter);
		if (!_purchaseAcknowledgeMessageRecieved)
		{
			AppShell.Instance.EventMgr.RemoveListener<ShoppingPurchaseAcknowledgeMessage>(ShoppingPurchaseAcknowledge);
		}
		if (!_purchaseCompletedMessageRecieved)
		{
			AppShell.Instance.EventMgr.RemoveListener<ShoppingPurchaseCompletedMessage>(ShoppingPurchaseCompleted);
		}
		ShoppingWindow.instance.displayError("#PURCHASE_FAIL", translatePurchaseErrorCode(result));
		CspUtils.DebugLog("enable failure ");
		_actualGoldButton.IsEnabled = true;
		_actualFractalButton.IsEnabled = true;
		_purchaseInProgress = false;
	}
}
