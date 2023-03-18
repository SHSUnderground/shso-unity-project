using UnityEngine;

//namespace MySquadChallenge
//{
	public class MSCBadgeGUI : GUISubScalingWindow
	{
		private GUIImage background;

		private GUIImage item;

		private GUIHotSpotButton hotspot;

		public int ownableTypeID;

		protected static readonly Vector2 SLIDE_ITEM_SIZE = new Vector2(150f, 150f);

		public MSCBadgeGUI()
			: base(SLIDE_ITEM_SIZE)
		{
			SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, 0f));
			Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			Traits.VisibleAncestryTrait = ControlTraits.VisibilityAncestryTraitEnum.DetachedVisibility;
			background = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(150f, 150f), new Vector2(0f, 0f));
			background.TextureSource = "shopping_bundle|badge";
			AddItem(background);
			item = GUIControl.CreateControlFrameCentered<GUIImage>(SLIDE_ITEM_SIZE, Vector2.zero);
			AddItem(item);
			hotspot = GUIControl.CreateControlBottomFrame<GUIHotSpotButton>(SLIDE_ITEM_SIZE, new Vector2(0f, 0f));
			hotspot.MouseDown += OnMouseDown;
			hotspot.ToolTip = new NamedToolTipInfo("Inner tool", new Vector2(18f, 0f));
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
			CspUtils.DebugLog("setHero characters_bundle|inventory_character_" + hero + "_normal");
			item.TextureSource = "characters_bundle|inventory_character_" + hero + "_normal";
		}

		private void OnMouseDown(GUIControl sender, GUIMouseEvent EventData)
		{
			if (IsEnabled)
			{
				CspUtils.DebugLog("CLICKED Badge " + ownableTypeID);
				ShoppingWindow shoppingWindow = new ShoppingWindow(ownableTypeID);
				shoppingWindow.launch();
			}
		}
	}
//}
