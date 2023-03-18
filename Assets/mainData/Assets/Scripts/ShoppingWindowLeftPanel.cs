using System.Collections.Generic;
using UnityEngine;

public class ShoppingWindowLeftPanel : GUIDialogWindow
{
	private GUIImage _background;

	private List<ShoppingWindowLeftNavButton> _navButtons = new List<ShoppingWindowLeftNavButton>();

	private int _nextNavY = 10;

	public ShoppingWindowLeftPanel()
	{
		_background = new GUIImage();
		_background.SetSize(new Vector2(194f, 574f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_background.Position = Vector2.zero;
		_background.TextureSource = "shopping_bundle|leftpanel";
		Add(_background);
		SetSize(_background.Size);
		addButton(NewShoppingManager.ShoppingCategory.Featured);
		addButton(NewShoppingManager.ShoppingCategory.Hero);
		addButton(NewShoppingManager.ShoppingCategory.AgentOnly);
		addButton(NewShoppingManager.ShoppingCategory.Bundle);
		addButton(NewShoppingManager.ShoppingCategory.Badge);
		addButton(NewShoppingManager.ShoppingCategory.Craft);
		addButton(NewShoppingManager.ShoppingCategory.Mission);
		addButton(NewShoppingManager.ShoppingCategory.MysteryBox);
		addButton(NewShoppingManager.ShoppingCategory.Potion);
		addButton(NewShoppingManager.ShoppingCategory.Card);
		addButton(NewShoppingManager.ShoppingCategory.Sidekick);
		addButton(NewShoppingManager.ShoppingCategory.Title);
		addButton(NewShoppingManager.ShoppingCategory.Medallion);
		addButton(NewShoppingManager.ShoppingCategory.Misc);
	}

	private void addButton(NewShoppingManager.ShoppingCategory category)
	{
		ShoppingWindowLeftNavButton shoppingWindowLeftNavButton = null;
		switch (category)
		{
		case NewShoppingManager.ShoppingCategory.Featured:
			shoppingWindowLeftNavButton = new ShoppingWindowLeftNavButton("shopping_bundle|navicon_new", "#SHOPPINGNAV_NEW", 30, GUILabel.GenColor(255, 209, 81), GUILabel.GenColor(0, 0, 0), true);
			break;
		case NewShoppingManager.ShoppingCategory.Hero:
			shoppingWindowLeftNavButton = new ShoppingWindowLeftNavButton("shopping_bundle|navicon_heroes", "#SHOPPINGNAV_HEROES", 30, GUILabel.GenColor(238, 29, 29), GUILabel.GenColor(194, 194, 194));
			break;
		case NewShoppingManager.ShoppingCategory.AgentOnly:
			shoppingWindowLeftNavButton = new ShoppingWindowLeftNavButton("shopping_bundle|navicon_agent", "#SHOPPINGNAV_AGENTS", 16, GUILabel.GenColor(153, 91, 255), GUILabel.GenColor(194, 194, 194));
			break;
		case NewShoppingManager.ShoppingCategory.Badge:
			shoppingWindowLeftNavButton = new ShoppingWindowLeftNavButton("shopping_bundle|navicon_badges", "#SHOPPINGNAV_BADGES", 16, GUILabel.GenColor(0, 121, 206), GUILabel.GenColor(194, 194, 194));
			break;
		case NewShoppingManager.ShoppingCategory.Craft:
			shoppingWindowLeftNavButton = new ShoppingWindowLeftNavButton("shopping_bundle|navicon_craft", "#SHOPPINGNAV_CRAFTS", 16, GUILabel.GenColor(0, 121, 206), GUILabel.GenColor(194, 194, 194));
			break;
		case NewShoppingManager.ShoppingCategory.Mission:
			shoppingWindowLeftNavButton = new ShoppingWindowLeftNavButton("shopping_bundle|navicon_missions", "#SHOPPINGNAV_MISSIONS", 16, GUILabel.GenColor(0, 121, 206), GUILabel.GenColor(194, 194, 194));
			break;
		case NewShoppingManager.ShoppingCategory.MysteryBox:
			shoppingWindowLeftNavButton = new ShoppingWindowLeftNavButton("shopping_bundle|navicon_mystery", "#SHOPPINGNAV_MYSTERY", 16, GUILabel.GenColor(0, 121, 206), GUILabel.GenColor(194, 194, 194));
			break;
		case NewShoppingManager.ShoppingCategory.Potion:
			shoppingWindowLeftNavButton = new ShoppingWindowLeftNavButton("shopping_bundle|navicon_potions", "#SHOPPINGNAV_POTIONS", 16, GUILabel.GenColor(0, 121, 206), GUILabel.GenColor(194, 194, 194));
			break;
		case NewShoppingManager.ShoppingCategory.Card:
			shoppingWindowLeftNavButton = new ShoppingWindowLeftNavButton("shopping_bundle|navicon_cards", "#SHOPPINGNAV_CARDS", 16, GUILabel.GenColor(0, 121, 206), GUILabel.GenColor(194, 194, 194));
			break;
		case NewShoppingManager.ShoppingCategory.Sidekick:
			shoppingWindowLeftNavButton = new ShoppingWindowLeftNavButton("shopping_bundle|navicon_sidekicks", "#SHOPPINGNAV_SIDEKICKS", 16, GUILabel.GenColor(0, 121, 206), GUILabel.GenColor(194, 194, 194));
			break;
		case NewShoppingManager.ShoppingCategory.Title:
			shoppingWindowLeftNavButton = new ShoppingWindowLeftNavButton("shopping_bundle|navicon_titles", "#SHOPPINGNAV_TITLES", 16, GUILabel.GenColor(0, 121, 206), GUILabel.GenColor(194, 194, 194));
			break;
		case NewShoppingManager.ShoppingCategory.Gear:
			shoppingWindowLeftNavButton = new ShoppingWindowLeftNavButton("shopping_bundle|navicon_gear", "#SHOPPINGNAV_GEAR", 16, GUILabel.GenColor(0, 121, 206), GUILabel.GenColor(194, 194, 194));
			break;
		case NewShoppingManager.ShoppingCategory.Misc:
			shoppingWindowLeftNavButton = new ShoppingWindowLeftNavButton("shopping_bundle|navicon_misc", "#SHOPPINGNAV_MISC", 16, GUILabel.GenColor(0, 121, 206), GUILabel.GenColor(194, 194, 194));
			break;
		case NewShoppingManager.ShoppingCategory.Medallion:
			shoppingWindowLeftNavButton = new ShoppingWindowLeftNavButton("shopping_bundle|navicon_medallions", "#SHOPPINGNAV_MEDALLIONS", 16, GUILabel.GenColor(0, 121, 206), GUILabel.GenColor(194, 194, 194));
			break;
		case NewShoppingManager.ShoppingCategory.Bundle:
			shoppingWindowLeftNavButton = new ShoppingWindowLeftNavButton("shopping_bundle|navicon_bundles", "#SHOPPINGNAV_BUNDLES", 16, GUILabel.GenColor(0, 121, 206), GUILabel.GenColor(194, 194, 194));
			break;
		default:
			CspUtils.DebugLog("Invalid category!");
			return;
		}
		shoppingWindowLeftNavButton.SetPosition(8f, _nextNavY);
		shoppingWindowLeftNavButton.category = category;
		shoppingWindowLeftNavButton.bg.Click += delegate
		{
			CspUtils.DebugLog(category);
			ShoppingWindow.instance.showCategory(category);
		};
		Add(shoppingWindowLeftNavButton);
		_navButtons.Add(shoppingWindowLeftNavButton);
		int nextNavY = _nextNavY;
		Vector2 size = shoppingWindowLeftNavButton.Size;
		_nextNavY = nextNavY + ((int)size.y - 2);
		if (category != NewShoppingManager.ShoppingCategory.Misc)
		{
			GUIImage gUIImage = new GUIImage();
			gUIImage.SetSize(new Vector2(171f, 8f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIImage.Position = new Vector2(1f, _nextNavY);
			gUIImage.TextureSource = "shopping_bundle|leftnavdivider";
			gUIImage.IsVisible = true;
			Add(gUIImage);
		}
	}
}
