using System;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingWindowUpsellPanel : GUISimpleControlWindow
{
	private GUIImage _background;

	private List<CatalogItem> _possibleUpsells = new List<CatalogItem>();

	private CatalogItem _currentUpsell;

	private CatalogItem _nextUpsell;

	private GUISimpleControlWindow _currentUpsellWindow;

	private static Vector2 MAX_ICON_SIZE = new Vector2(110f, 110f);

	public ShoppingWindowUpsellPanel()
	{
		_background = new GUIImage();
		_background.SetSize(new Vector2(240f, 176f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_background.Position = Vector2.zero;
		_background.TextureSource = "shopping_bundle|upsellpanel";
		Add(_background);
		SetSize(_background.Size);
		foreach (CatalogItem item in NewShoppingManager.categoryContents[NewShoppingManager.ShoppingCategory.Featured])
		{
			if (item.ownableDef.category != OwnableDefinition.Category.Title)
			{
				_possibleUpsells.Add(item);
			}
		}
		nextUpsell();
	}

	private void nextUpsell()
	{
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		if (ShoppingWindow.instance == null || _possibleUpsells.Count <= 0)
		{
			return;
		}
		CatalogItem item = _currentUpsell;
		if (_possibleUpsells.Count == 1)
		{
			item = _possibleUpsells[0];
		}
		else
		{
			while (item == null || item == _currentUpsell)
			{
				item = _possibleUpsells[UnityEngine.Random.Range(0, _possibleUpsells.Count)];
			}
		}
		GUISimpleControlWindow newUpsell = formatUpsellWindow(item);
		Add(newUpsell);
		newUpsell.Alpha = 0f;
		AnimClip animClip = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 1f), newUpsell);
		AnimClip animClip2 = SHSAnimations.Generic.Wait(5f);
		AnimClip animClip3 = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(1f, 0f, 1f), newUpsell);
		AnimClip toAdd = animClip;
		toAdd |= animClip2;
		toAdd |= animClip3;
		animClip2.OnFinished += (Action)(object)(Action)delegate
		{
			_currentUpsell = item;
			_currentUpsellWindow = newUpsell;
			nextUpsell();
		};
		animClip3.OnFinished += (Action)(object)(Action)delegate
		{
			Remove(newUpsell);
			newUpsell.Dispose();
		};
		ShoppingWindow.instance.AnimationPieceManager.Add(toAdd);
	}

	private GUISimpleControlWindow formatUpsellWindow(CatalogItem item)
	{
		GUISimpleControlWindow gUISimpleControlWindow = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(new Vector2(240f, 200f), new Vector2(5f, 5f));
		GUIImageWithEvents gUIImageWithEvents = GUIControl.CreateControlTopLeftFrame<GUIImageWithEvents>(gUISimpleControlWindow.Size, Vector2.zero);
		gUISimpleControlWindow.Add(gUIImageWithEvents);
		gUIImageWithEvents.IsVisible = true;
		gUIImageWithEvents.Alpha = 0f;
		gUIImageWithEvents.Click += delegate
		{
			ShoppingWindow.instance.upsellClick(item);
		};
		GUISimpleControlWindow icon = item.ownableDef.getIcon(MAX_ICON_SIZE);
		float x = MAX_ICON_SIZE.x;
		Vector2 size = icon.Size;
		float x2 = 15f + (x - size.x) / 2f;
		float y = MAX_ICON_SIZE.y;
		Vector2 size2 = icon.Size;
		icon.Position = new Vector2(x2, 45f + (y - size2.y) / 2f);
		icon.IsVisible = true;
		gUISimpleControlWindow.Add(icon);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		Vector2 offset = new Vector2(0f, 10f);
		Vector2 size3 = gUISimpleControlWindow.Size;
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, offset, new Vector2(size3.x, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel.BackColorAlpha = 1f;
		gUIStrokeTextLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUIStrokeTextLabel.Text = item.name;
		gUISimpleControlWindow.Add(gUIStrokeTextLabel);
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetSize(new Vector2(92f, 54f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Vector2 size4 = gUISimpleControlWindow.Size;
		float x3 = size4.x;
		Vector2 size5 = gUIImage.Size;
		gUIImage.Position = new Vector2(x3 - size5.x - 8f, 117f);
		gUIImage.TextureSource = "shopping_bundle|priceoverlay";
		gUISimpleControlWindow.Add(gUIImage);
		float num = 1f;
		Entitlements.ServerValueEntitlement serverValueEntitlement = Singleton<Entitlements>.instance.EntitlementsSet[Entitlements.EntitlementFlagEnum.SubscriptionType] as Entitlements.ServerValueEntitlement;
		if (serverValueEntitlement.Value == "12")
		{
			num = 0.900001f;
		}
		Vector2 vector = gUIImage.Position + new Vector2(4f, 4f);
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
			gUISimpleControlWindow.Add(gUIImage2);
			GUIStrokeTextLabel gUIStrokeTextLabel2 = new GUIStrokeTextLabel();
			gUIStrokeTextLabel2.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, vector + new Vector2(27f, 1f), new Vector2(63f, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIStrokeTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 14, GUILabel.GenColor(255, 226, 90), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.UpperLeft);
			gUIStrokeTextLabel2.BackColorAlpha = 1f;
			gUIStrokeTextLabel2.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
			gUIStrokeTextLabel2.Text = ShoppingWindow.convertNumber((int)((float)item.goldPrice * (float)item.quantity * num));
			gUIStrokeTextLabel2.Id = "goldPriceLabel";
			gUISimpleControlWindow.Add(gUIStrokeTextLabel2);
			vector += new Vector2(0f, 23f);
		}
		if (item.shardPrice > 0)
		{
			GUIImage gUIImage3 = new GUIImage();
			gUIImage3.SetSize(new Vector2(21f, 26f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIImage3.Position = vector + new Vector2(3f, 0f);
			gUIImage3.TextureSource = "shopping_bundle|newfractal_shopping";
			gUIImage3.Id = "shardIcon";
			gUISimpleControlWindow.Add(gUIImage3);
			GUIStrokeTextLabel gUIStrokeTextLabel3 = new GUIStrokeTextLabel();
			gUIStrokeTextLabel3.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, vector + new Vector2(27f, 1f), new Vector2(63f, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIStrokeTextLabel3.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 14, GUILabel.GenColor(255, 136, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.UpperLeft);
			gUIStrokeTextLabel3.BackColorAlpha = 1f;
			gUIStrokeTextLabel3.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
			gUIStrokeTextLabel3.Text = ShoppingWindow.convertNumber((int)((float)item.shardPrice * (float)item.quantity * num));
			gUIStrokeTextLabel3.Id = "shardPriceLabel";
			gUISimpleControlWindow.Add(gUIStrokeTextLabel3);
		}
		if (item.quantity > 1)
		{
			GUIImage gUIImage4 = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(102f, 60f), new Vector2(135f, 61f));
			gUIImage4.TextureSource = "shopping_bundle|shopping_item_quantity_container";
			gUISimpleControlWindow.Add(gUIImage4);
			GUILabel gUILabel = GUIControl.CreateControlTopLeftFrame<GUILabel>(new Vector2(60f, 35f), gUIImage4.Position + new Vector2(25f, 15f));
			gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 33, GUILabel.GenColor(0, 120, 255), TextAnchor.MiddleCenter);
			gUILabel.Text = string.Format("{0:n0}", item.quantity);
			gUILabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
			gUILabel.Rotation = -5f;
			gUISimpleControlWindow.Add(gUILabel);
		}
		gUISimpleControlWindow.IsVisible = true;
		gUISimpleControlWindow.SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		gUISimpleControlWindow.Alpha = 0f;
		return gUISimpleControlWindow;
	}

	public void rotate()
	{
	}
}
