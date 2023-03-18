using UnityEngine;

public class BadgeGUI : GUISubScalingWindow
{
	private GUIImage background;

	private GUIImage item;

	private GUIHotSpotButton hotspot;

	public int ownableTypeID;

	public int tier = 1;

	protected static readonly Vector2 SLIDE_ITEM_SIZE = new Vector2(150f, 150f);

	public BadgeGUI(int tier)
		: base(SLIDE_ITEM_SIZE)
	{
		SetSize(SLIDE_ITEM_SIZE);
		this.tier = tier;
		background = GUIControl.CreateControlTopLeftFrame<GUIImage>(SLIDE_ITEM_SIZE, new Vector2(0f, 0f));
		if (tier == 1)
		{
			background.TextureSource = "shopping_bundle|badge_silver";
		}
		else
		{
			background.TextureSource = "shopping_bundle|badge";
		}
		background.IsVisible = true;
		AddItem(background);
		item = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(103f, 103f), new Vector2(23f, 23f));
		item.IsVisible = true;
		AddItem(item);
		hotspot = GUIControl.CreateControlTopLeftFrame<GUIHotSpotButton>(SLIDE_ITEM_SIZE, new Vector2(0f, 0f));
		hotspot.MouseDown += OnMouseDown;
		AddItem(hotspot);
	}

	public void setOwned(bool owned)
	{
		if (owned)
		{
			hotspot.ToolTip = new NamedToolTipInfo("#TT_HERO_BADGE_ALREADY_OWNED", new Vector2(18f, 0f));
		}
		else
		{
			hotspot.ToolTip = new NamedToolTipInfo("#TT_HERO_BADGE_NOT_OWNED", new Vector2(18f, 0f));
		}
	}

	public void setHero(string hero)
	{
		item.TextureSource = "characters_bundle|inventory_character_" + hero + "_normal";
	}

	private void OnMouseDown(GUIControl sender, GUIMouseEvent EventData)
	{
		if (IsEnabled)
		{
			OwnableDefinition.goToOwnableLocation(ownableTypeID);
		}
	}
}
