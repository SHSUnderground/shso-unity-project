using UnityEngine;

public class SidekickHUDItem : GUIControlWindow
{
	protected GUIImage background;

	protected GUIImage effectIcon;

	protected GUIButton CloseButton;

	public bool open;

	private int cachedSidekickID = -2;

	public SidekickHUDItem()
	{
		SetSize(new Vector2(90f, 70f));
		background = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(64f, 64f), new Vector2(3f, 0f));
		background.SetSize(64f, 64f);
		background.HitTestType = HitTestTypeEnum.Rect;
		background.BlockTestType = BlockTestTypeEnum.Rect;
		background.TextureSource = "gameworld_bundle|gameworld_token_background";
		background.HitTestSize = new Vector2(1f, 1f);
		Add(background);
		background.IsVisible = false;
		CloseButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(48f, 48f), new Vector2(25f, -20f));
		CloseButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|button_mysquad_close");
		CloseButton.Click += delegate
		{
			PetDataManager.changeCurrentPet(-1);
			PetDataManager.submitSidekickInfo();
		};
		CloseButton.ToolTip = new NamedToolTipInfo("#EFFECTSHUD_HIDE_SIDEKICK");
		Add(CloseButton);
		CloseButton.IsVisible = false;
		AppShell.Instance.EventMgr.AddListener<CurrentPetChangeEvent>(OnPetChange);
		MouseOver += delegate
		{
			if (!open && cachedSidekickID > 0)
			{
				CloseButton.IsVisible = true;
				open = true;
				SetPosition(new Vector2(position.x - 3f, position.y));
			}
		};
		MouseOut += delegate
		{
			if (open)
			{
				open = false;
				CloseButton.IsVisible = false;
				SetPosition(new Vector2(position.x + 3f, position.y));
			}
		};
		SetVisible(false);
		OnPetChange(null);
	}

	private void OnPetChange(CurrentPetChangeEvent evt)
	{
		cachedSidekickID = PetDataManager.getCurrentPet();
		if (cachedSidekickID < 0)
		{
			SetVisible(false);
			background.IsVisible = false;
			return;
		}
		PetData data = PetDataManager.getData(PetDataManager.getCurrentPet());
		if (data == null)
		{
			SetVisible(false);
			background.IsVisible = false;
			return;
		}
		SetVisible(true);
		background.IsVisible = true;
		if (effectIcon != null)
		{
			Remove(effectIcon);
			effectIcon = null;
		}
		effectIcon = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(70f, 70f), new Vector2(3f, 0f));
		effectIcon.TextureSource = "shopping_bundle|" + data.inventoryIconBase + "_normal";
		Add(effectIcon);
		Add(CloseButton);
	}
}
