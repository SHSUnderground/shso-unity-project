using UnityEngine;

public class SHSInventoryHeroItem : SHSInventorySelectionItem
{
	public HeroPersisted hero;

	public override GUIControl.ToolTipInfo ItemHoverHelpInfo
	{
		get
		{
			return new GUIControl.HeroHoverHelpInfo(hero);
		}
	}

	public override SHSButtonStyleInfo ButtonStyleInfo
	{
		get
		{
			return new SHSButtonStyleInfo("characters_bundle|inventory_character_" + hero.Name, SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		}
	}

	public override ShsCollectionItem CollectionItem
	{
		get
		{
			return hero;
		}
	}

	public override int CollectionItemCount
	{
		get
		{
			return 1;
		}
	}

	public SHSInventoryHeroItem(HeroPersisted hero, SHSInventoryAnimatedWindow headWindow)
		: base(headWindow)
	{
		this.hero = hero;
		SetupWindow();
		InventoryItemIcon.SetSize(new Vector2(103f, 103f));
		InventoryItemIcon.HitTestSize = new Vector2(0.6f, 0.6f);
		ItemCount.Text = "/";
		UpdateHeroPlaced();
		AppShell.Instance.DataManager.LoadGameData("Characters/" + hero.Name, OnCharacterDataLoaded, item);
	}

	private static void OnCharacterDataLoaded(GameDataLoadResponse response, object extraData)
	{
		CharacterDefinition characterDefinition = new CharacterDefinition();
		characterDefinition.InitializeFromData(response.Data);
		SHSItemLoadingWindow sHSItemLoadingWindow = (SHSItemLoadingWindow)extraData;
		if (sHSItemLoadingWindow == null)
		{
			CspUtils.DebugLog("Unable to cast loading window for item. Can't display character selection item.");
		}
		else
		{
			sHSItemLoadingWindow.ConfigureRequiredContent(new ContentReference(ContentTypeEnum.Bundle, characterDefinition.AssetBundle));
		}
	}

	public void UpdateHeroPlaced()
	{
		if (hero.Placed)
		{
			NumberOfItems.IsVisible = true;
			ItemCount.IsVisible = true;
			NumberOfItems.CachedVisible = true;
			ItemCount.CachedVisible = true;
		}
		else
		{
			NumberOfItems.IsVisible = false;
			ItemCount.IsVisible = false;
			NumberOfItems.CachedVisible = false;
			ItemCount.CachedVisible = false;
		}
	}

	public override GUIButton GetSetupInventoryDragDropButton()
	{
		item.Id = hero.Name;
		return new SHSInventoryHeroButton(headWindow, hero.Name, "characters_bundle|inventory_character_" + hero.Name + "_normal", DragDropInfo.CollectionType.Heroes, "Heroes", delegate
		{
			return ButtonUseable;
		});
	}

	public override int CompareTo(SHSInventorySelectionItem other)
	{
		int num = base.CompareTo(other);
		if (num != 0)
		{
			return num;
		}
		return hero.Name.CompareTo(((SHSInventoryHeroItem)other).hero.Name);
	}

	public override bool CompareTo(string id)
	{
		return hero.Name == id;
	}
}
