using UnityEngine;

public class EffectHUDItem : GUIControlWindow
{
	protected GUIImage background;

	protected GUIImage effectIcon;

	protected GUIButton CloseButton;

	public ExpendableDefinition def;

	public bool open;

	public EffectHUDItem(ExpendableDefinition def)
	{
		SetSize(new Vector2(90f, 70f));
		this.def = def;
		background = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(64f, 64f), new Vector2(3f, 0f));
		background.SetSize(64f, 64f);
		background.HitTestType = HitTestTypeEnum.Rect;
		background.BlockTestType = BlockTestTypeEnum.Rect;
		background.TextureSource = "gameworld_bundle|gameworld_token_background";
		background.HitTestSize = new Vector2(1f, 1f);
		Add(background);
		effectIcon = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(70f, 70f), new Vector2(0f, 0f));
		effectIcon.TextureSource = def.InventoryIcon + "_normal";
		Add(effectIcon);
		CloseButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(48f, 48f), new Vector2(25f, -20f));
		CloseButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|button_mysquad_close");
		CloseButton.Click += delegate
		{
			AppShell.Instance.ExpendablesManager.CancelEffect(def.OwnableTypeId);
		};
		CloseButton.ToolTip = new NamedToolTipInfo("#EFFECTSHUD_CANCEL_EFFECT");
		Add(CloseButton);
		CloseButton.IsVisible = false;
		MouseOver += delegate
		{
			if (!open)
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
	}
}
