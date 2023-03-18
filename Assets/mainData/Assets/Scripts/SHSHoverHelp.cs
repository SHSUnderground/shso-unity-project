using System;
using UnityEngine;

public class SHSHoverHelp : GUIWindow
{
	private abstract class HoverHelpWindow : GUISimpleControlWindow
	{
		protected NineSliceTexture nineSliceTexture;

		public HoverHelpWindow()
		{
			Traits.VisibleAncestryTrait = ControlTraits.VisibilityAncestryTraitEnum.EnsureAncestorsVisible;
			SetSize(HOVER_HELP_SIZE);
			SetPosition(0f, 0f);
		}

		public override void OnHide()
		{
			base.OnHide();
			if (nineSliceTexture != null)
			{
				nineSliceTexture.ReleaseNineSliceTexture();
			}
		}
	}

	private class GenericHoverHelpWindow : HoverHelpWindow
	{
		private const float maxWidth = 175f;

		private const float itemNameCushion = 25f;

		private const int itemNameKerning = 18;

		private const int hoverTextKerning = 14;

		private const int hoverPadding = 10;

		private GUIImage ItemPortrait;

		private GUIImage genericBgImage;

		private GUIImage itemFlash;

		private GUIDropShadowTextLabel name;

		private GUILabel text;

		public GenericHoverHelpWindow()
		{
			nineSliceTexture = new NineSliceTexture();
			nineSliceTexture.AttachmentHOffset = 0;
			nineSliceTexture.AttachmentVOffset = -15;
			genericBgImage = GUIControl.CreateControlAbsolute<GUIImage>(Vector2.zero, Vector2.zero);
			Add(genericBgImage);
			itemFlash = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
			Add(itemFlash);
			ItemPortrait = GUIControl.CreateControl<GUIImage>(new Vector2(160f, 160f), Vector2.zero, DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
			GUIImage itemPortrait = ItemPortrait;
			Vector2 size = ItemPortrait.Size;
			float x = size.x * 0.7f;
			Vector2 size2 = ItemPortrait.Size;
			itemPortrait.SetSize(new Vector2(x, size2.y * 0.7f));
			Add(ItemPortrait);
			name = GUIControl.CreateControlAbsolute<GUIDropShadowTextLabel>(new Vector2(230f, 44f), new Vector2(0f, 0f));
			name.Rotation = -3f;
			name.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 24, Color.black, TextAnchor.UpperLeft);
			name.FrontColor = new Color(0.8f, 59f / 255f, 0.117647059f, 1f);
			name.BackColor = new Color(0f, 28f / 255f, 14f / 51f, 0.1f);
			name.TextOffset = new Vector2(2f, 2f);
			Add(name);
			text = GUIControl.CreateControlAbsolute<GUILabel>(new Vector2(180f, 145f), new Vector2(40f, 90f));
			text.Rotation = -3f;
			text.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(58, 71, 94), TextAnchor.UpperLeft);
			Add(text);
		}

		public void Setup(GenericHoverHelpInfo info, SHSHoverHelp helpWindow)
		{
			bool flag = false;
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			ItemPortrait.SetSize(info.IconSize);
			name.InvalidateKerning();
			text.InvalidateKerning();
			name.Text = info.name;
			text.Text = info.description;
			base.nineSliceTexture.VerticalFlip = true;
			GUIImage gUIImage = itemFlash;
			Vector2 burstTextureSize = BurstTextureSize;
			float x = burstTextureSize.x;
			Vector2 burstTextureSize2 = BurstTextureSize;
			gUIImage.Size = new Vector2(x, burstTextureSize2.y);
			itemFlash.Texture = helpWindow.BurstBgTexture;
			GUIImage gUIImage2 = itemFlash;
			Vector2 size = itemFlash.Size;
			float x2 = size.x * 0.7f;
			Vector2 size2 = itemFlash.Size;
			gUIImage2.SetSize(new Vector2(x2, size2.y * 0.7f));
			GUIImage itemPortrait = ItemPortrait;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle = helpWindow.ResourceBundle;
			itemPortrait.Offset = new Vector2(0f, resourceBundle.attachmentTexture.height / 2);
			GUIImage gUIImage3 = itemFlash;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle2 = helpWindow.ResourceBundle;
			gUIImage3.Offset = new Vector2(0f, resourceBundle2.attachmentTexture.height / 2);
			ItemPortrait.TextureSource = info.IconLocation;
			ItemPortrait.SetSize(info.IconSize);
			vector = name.Style.UnityStyle.CalcSize(new GUIContent(name.Text));
			vector2 = text.Style.UnityStyle.CalcSize(new GUIContent(text.Text));
			if (vector.x > 200f)
			{
				name.Style.UnityStyle.wordWrap = true;
				vector.x = 175f;
				name.VerticalKerning = 18;
				name.NoLineLimit = true;
				flag = true;
			}
			else
			{
				name.Style.UnityStyle.wordWrap = false;
				name.NoLineLimit = false;
			}
			if (flag)
			{
				name.CalculateTextLayout();
			}
			GUIDropShadowTextLabel gUIDropShadowTextLabel = name;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle3 = helpWindow.ResourceBundle;
			float x3 = 10 + resourceBundle3.cornerTextures[0].width;
			int num;
			if (base.nineSliceTexture.VerticalFlip)
			{
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle4 = helpWindow.ResourceBundle;
				num = resourceBundle4.attachmentTexture.height;
			}
			else
			{
				num = 0;
			}
			int num2 = 10 + num;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle5 = helpWindow.ResourceBundle;
			gUIDropShadowTextLabel.Position = new Vector2(x3, num2 + resourceBundle5.borderTextures[0].height);
			name.SetSize(new Vector2(vector.x, 125f));
			flag = false;
			if (vector2.x > 175f)
			{
				text.Style.UnityStyle.wordWrap = true;
				vector2.x = 175f;
				text.VerticalKerning = 14;
				text.NoLineLimit = true;
				flag = true;
			}
			else
			{
				text.Style.UnityStyle.wordWrap = false;
				text.NoLineLimit = false;
			}
			GUILabel gUILabel = text;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle6 = helpWindow.ResourceBundle;
			float x4 = resourceBundle6.cornerTextures[0].width + 10;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle7 = helpWindow.ResourceBundle;
			int num3 = resourceBundle7.borderTextures[0].height + 10 + name.TotalTextHeight;
			int num4;
			if (base.nineSliceTexture.VerticalFlip)
			{
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle8 = helpWindow.ResourceBundle;
				num4 = resourceBundle8.attachmentTexture.height;
			}
			else
			{
				num4 = 0;
			}
			gUILabel.SetPosition(new Vector2(x4, num3 + num4));
			text.SetSize(new Vector2(vector2.x, 125f));
			if (flag)
			{
				text.CalculateTextLayout();
			}
			int longestLine = name.LongestLine;
			if (longestLine < text.LongestLine)
			{
				longestLine = text.LongestLine;
			}
			float num5 = text.TotalTextHeight + name.TotalTextHeight;
			float num6 = 0f;
			Vector2 size3 = itemFlash.Size;
			if (num5 > size3.y)
			{
				Vector2 size4 = itemFlash.Size;
				num6 = num5 - size4.y;
			}
			base.nineSliceTexture.HorizontalFlip = (info.horizontalFlipOverride == HoverHelpInfo.FlipOverride.On);
			NineSliceTexture nineSliceTexture = base.nineSliceTexture;
			int num7 = longestLine;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle9 = helpWindow.ResourceBundle;
			float num8 = num7 + resourceBundle9.cornerTextures[0].width;
			Vector2 size5 = itemFlash.Size;
			float x5 = num8 + size5.x / 2f + 10f;
			Vector2 size6 = itemFlash.Size;
			float y = size6.y;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle10 = helpWindow.ResourceBundle;
			Vector2 dwellingSize = new Vector2(x5, y + (float)resourceBundle10.borderTextures[0].height + 10f + num6);
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle11 = helpWindow.ResourceBundle;
			Texture2D bodyTexture = resourceBundle11.bodyTexture;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle12 = helpWindow.ResourceBundle;
			Texture2D[] cornerTextures = resourceBundle12.cornerTextures;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle13 = helpWindow.ResourceBundle;
			Texture2D[] borderTextures = resourceBundle13.borderTextures;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle14 = helpWindow.ResourceBundle;
			nineSliceTexture.CreateNineSliceTexture(dwellingSize, bodyTexture, cornerTextures, borderTextures, resourceBundle14.attachmentTexture, NineSliceTexture.Side.Bottom, NineSliceTexture.SideAlignment.Left);
			genericBgImage.SetSize(new Vector2(base.nineSliceTexture.Texture.width, base.nineSliceTexture.Texture.height));
			genericBgImage.Rotation = -3f;
			genericBgImage.Texture = base.nineSliceTexture.Texture;
			genericBgImage.Alpha = 0.95f;
			float num9 = base.nineSliceTexture.Texture.width;
			Vector2 size7 = itemFlash.Size;
			SetSize(new Vector2(num9 + size7.x / 2f, base.nineSliceTexture.Texture.height));
			float num10 = base.nineSliceTexture.Texture.width;
			Vector2 size8 = itemFlash.Size;
			helpWindow.SetSize(new Vector2(num10 + size8.x / 2f, base.nineSliceTexture.Texture.height - 1));
		}

		public override void OnHide()
		{
			base.OnHide();
			genericBgImage.Texture = null;
			ItemPortrait.Texture = null;
			itemFlash.Texture = null;
		}
	}

	private class InventoryHoverHelpWindow : HoverHelpWindow
	{
		private const float maxWidth = 175f;

		private const float itemNameCushion = 25f;

		private const int itemNameKerning = 18;

		private const int hoverTextKerning = 14;

		private const int hoverPadding = 10;

		private GUIDropShadowTextLabel itemName;

		private GUIImage itemPortrait;

		private GUIImage itemFlash;

		private GUIImage inventoryBgImage;

		private GUILabel text;

		private GUILabel amount;

		public InventoryHoverHelpWindow()
		{
			nineSliceTexture = new NineSliceTexture();
			nineSliceTexture.AttachmentHOffset = 0;
			nineSliceTexture.AttachmentVOffset = -15;
			inventoryBgImage = GUIControl.CreateControlAbsolute<GUIImage>(Vector2.zero, Vector2.zero);
			Add(inventoryBgImage);
			itemFlash = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
			Add(itemFlash);
			itemName = GUIControl.CreateControlAbsolute<GUIDropShadowTextLabel>(new Vector2(230f, 44f), new Vector2(0f, 0f));
			itemName.Rotation = -3f;
			itemName.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 24, Color.black, TextAnchor.UpperLeft);
			itemName.FrontColor = new Color(0.8f, 59f / 255f, 0.117647059f, 1f);
			itemName.BackColor = new Color(0f, 28f / 255f, 14f / 51f, 0.1f);
			itemName.TextOffset = new Vector2(2f, 2f);
			Add(itemName);
			itemPortrait = GUIControl.CreateControl<GUIImage>(new Vector2(160f, 160f), Vector2.zero, DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
			GUIImage gUIImage = itemPortrait;
			Vector2 size = itemPortrait.Size;
			float x = size.x * 0.7f;
			Vector2 size2 = itemPortrait.Size;
			gUIImage.SetSize(new Vector2(x, size2.y * 0.7f));
			itemPortrait.Rotation = -3f;
			Add(itemPortrait);
			text = GUIControl.CreateControlAbsolute<GUILabel>(new Vector2(189f, 110f), new Vector2(0f, 45f));
			text.Rotation = -3f;
			text.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, new Color(16f / 255f, 44f / 255f, 19f / 85f), TextAnchor.UpperLeft);
			Add(text);
			amount = GUIControl.CreateControlAbsolute<GUILabel>(new Vector2(150f, 30f), Vector2.zero);
			amount.Rotation = -3f;
			amount.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, new Color(16f / 255f, 44f / 255f, 19f / 85f), TextAnchor.UpperLeft);
			Add(amount);
		}

		public void Setup(InventoryHoverHelpInfo info, SHSHoverHelp helpWindow)
		{
			bool flag = false;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			GUIImage gUIImage = itemFlash;
			Vector2 burstTextureSize = BurstTextureSize;
			float x = burstTextureSize.x;
			Vector2 burstTextureSize2 = BurstTextureSize;
			gUIImage.Size = new Vector2(x, burstTextureSize2.y);
			itemFlash.Texture = helpWindow.BurstBgTexture;
			GUIImage gUIImage2 = itemFlash;
			Vector2 size = itemFlash.Size;
			float x2 = size.x * 0.7f;
			Vector2 size2 = itemFlash.Size;
			gUIImage2.SetSize(new Vector2(x2, size2.y * 0.7f));
			if (info.item is Item)
			{
				Item item = info.item as Item;
				text.Text = item.Description;
				itemName.Text = item.Name;
				amount.Text = item.Quantity - item.Placed + "/" + item.Quantity;
			}
			else if (info.item is Expendable)
			{
				Expendable expendable = info.item as Expendable;
				text.Text = ((!string.IsNullOrEmpty(info.text)) ? info.text : expendable.Definition.Description);
				itemName.Text = expendable.Definition.Name;
				amount.Text = string.Empty;
			}
			vector2 = itemName.Style.UnityStyle.CalcSize(new GUIContent(itemName.Text));
			vector = text.Style.UnityStyle.CalcSize(new GUIContent(text.Text));
			GUIImage gUIImage3 = itemFlash;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle = helpWindow.ResourceBundle;
			gUIImage3.Offset = new Vector2(0f, resourceBundle.attachmentTexture.height / 2);
			Vector2 offset = itemFlash.Offset;
			float x3 = offset.x;
			Vector2 size3 = itemFlash.Size;
			float x4 = size3.x;
			Vector2 size4 = itemPortrait.Size;
			float x5 = x3 - (x4 - size4.x) / 2f;
			GUIImage gUIImage4 = itemPortrait;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle2 = helpWindow.ResourceBundle;
			gUIImage4.Offset = new Vector2(x5, resourceBundle2.attachmentTexture.height / 2);
			itemName.InvalidateKerning();
			text.InvalidateKerning();
			amount.InvalidateKerning();
			if (vector2.x > 200f)
			{
				itemName.Style.UnityStyle.wordWrap = true;
				vector2.x = 175f;
				itemName.VerticalKerning = 18;
				itemName.NoLineLimit = true;
				flag = true;
			}
			else
			{
				itemName.Style.UnityStyle.wordWrap = false;
				itemName.NoLineLimit = false;
			}
			GUIDropShadowTextLabel gUIDropShadowTextLabel = itemName;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle3 = helpWindow.ResourceBundle;
			float x6 = 10 + resourceBundle3.cornerTextures[0].width;
			int num4;
			if (base.nineSliceTexture.VerticalFlip)
			{
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle4 = helpWindow.ResourceBundle;
				num4 = resourceBundle4.attachmentTexture.height;
			}
			else
			{
				num4 = 0;
			}
			int num5 = 10 + num4;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle5 = helpWindow.ResourceBundle;
			gUIDropShadowTextLabel.Position = new Vector2(x6, num5 + resourceBundle5.borderTextures[0].height);
			itemName.SetSize(new Vector2(vector2.x, 125f));
			if (flag)
			{
				itemName.CalculateTextLayout();
			}
			GUILabel gUILabel = amount;
			Vector2 position = itemName.Position;
			float x7 = position.x;
			Vector2 position2 = itemName.Position;
			gUILabel.Position = new Vector2(x7, position2.y + (float)itemName.TotalTextHeight);
			if (vector.x > 175f)
			{
				text.Style.UnityStyle.wordWrap = true;
				vector.x = 175f;
				text.VerticalKerning = 14;
				text.NoLineLimit = true;
				flag = true;
			}
			else
			{
				text.Style.UnityStyle.wordWrap = false;
				text.NoLineLimit = false;
			}
			GUILabel gUILabel2 = text;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle6 = helpWindow.ResourceBundle;
			float x8 = resourceBundle6.cornerTextures[0].width + 10;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle7 = helpWindow.ResourceBundle;
			int num6 = resourceBundle7.borderTextures[0].height + 10 + itemName.TotalTextHeight;
			int num7;
			if (base.nineSliceTexture.VerticalFlip)
			{
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle8 = helpWindow.ResourceBundle;
				num7 = resourceBundle8.attachmentTexture.height;
			}
			else
			{
				num7 = 0;
			}
			gUILabel2.SetPosition(new Vector2(x8, num6 + num7 + amount.TotalTextHeight));
			text.SetSize(new Vector2(vector.x, 125f));
			if (flag)
			{
				text.CalculateTextLayout();
			}
			num3 = text.TotalTextHeight + itemName.TotalTextHeight + 10;
			float num8 = num3;
			Vector2 size5 = itemFlash.Size;
			if (num8 > size5.y)
			{
				float num9 = num3;
				Vector2 size6 = itemFlash.Size;
				num = num9 - size6.y;
			}
			if (vector2.x > vector.x)
			{
				num2 = vector2.x - vector.x;
			}
			base.nineSliceTexture.HorizontalFlip = (info.horizontalFlipOverride == HoverHelpInfo.FlipOverride.On);
			base.nineSliceTexture.VerticalFlip = true;
			NineSliceTexture nineSliceTexture = base.nineSliceTexture;
			float num10 = (!flag) ? vector.x : ((float)text.LongestLine);
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle9 = helpWindow.ResourceBundle;
			float num11 = num10 + (float)resourceBundle9.cornerTextures[0].width;
			Vector2 size7 = itemFlash.Size;
			float x9 = num11 + size7.x / 2f + num2 + 10f;
			Vector2 size8 = itemFlash.Size;
			float num12 = size8.y + num;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle10 = helpWindow.ResourceBundle;
			Vector2 dwellingSize = new Vector2(x9, num12 + (float)resourceBundle10.borderTextures[0].height + 10f);
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle11 = helpWindow.ResourceBundle;
			Texture2D bodyTexture = resourceBundle11.bodyTexture;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle12 = helpWindow.ResourceBundle;
			Texture2D[] cornerTextures = resourceBundle12.cornerTextures;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle13 = helpWindow.ResourceBundle;
			Texture2D[] borderTextures = resourceBundle13.borderTextures;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle14 = helpWindow.ResourceBundle;
			nineSliceTexture.CreateNineSliceTexture(dwellingSize, bodyTexture, cornerTextures, borderTextures, resourceBundle14.attachmentTexture, NineSliceTexture.Side.Bottom, NineSliceTexture.SideAlignment.Left);
			inventoryBgImage.SetSize(new Vector2(base.nineSliceTexture.Texture.width, base.nineSliceTexture.Texture.height));
			inventoryBgImage.Rotation = -3f;
			inventoryBgImage.Texture = base.nineSliceTexture.Texture;
			inventoryBgImage.Alpha = 0.95f;
			float num13 = base.nineSliceTexture.Texture.width;
			Vector2 size9 = itemFlash.Size;
			SetSize(new Vector2(num13 + size9.x / 2f, base.nineSliceTexture.Texture.height));
			float num14 = base.nineSliceTexture.Texture.width;
			Vector2 size10 = itemFlash.Size;
			helpWindow.SetSize(new Vector2(num14 + size10.x / 2f, base.nineSliceTexture.Texture.height - 1));
			if (info.item is Item)
			{
				itemPortrait.TextureSource = "items_bundle|" + ((Item)info.item).Definition.Icon;
			}
			else if (info.item is Expendable)
			{
				itemPortrait.TextureSource = ((Expendable)info.item).Definition.HoverHelpIcon;
			}
		}

		public override void OnHide()
		{
			base.OnHide();
			inventoryBgImage.Texture = null;
			itemPortrait.Texture = null;
			inventoryBgImage.Texture = null;
		}
	}

	private class HeroHoverHelpWindow : HoverHelpWindow
	{
		private const float maxWidth = 175f;

		private const float heroTitleHeight = 40f;

		private const int hoverTextKerning = 14;

		private const int hoverPadding = 10;

		private GUIImage heroPortrait;

		private GUIImage background;

		private GUIImage heroFlash;

		private GUILabel text;

		private GUIDropShadowTextLabel heroNameText;

		private SHSStyle textStyle;

		public HeroHoverHelpWindow()
		{
			nineSliceTexture = new NineSliceTexture();
			nineSliceTexture.AttachmentHOffset = 0;
			nineSliceTexture.AttachmentVOffset = -15;
			background = GUIControl.CreateControlAbsolute<GUIImage>(Vector2.zero, Vector2.zero);
			Add(background);
			heroFlash = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
			Add(heroFlash);
			heroPortrait = GUIControl.CreateControl<GUIImage>(new Vector2(183f, 183f), new Vector2(0f, 0f), DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
			Add(heroPortrait);
			heroNameText = GUIControl.CreateControlAbsolute<GUIDropShadowTextLabel>(Vector2.zero, Vector2.zero);
			heroNameText.Rotation = -3f;
			heroNameText.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 24, Color.black, TextAnchor.UpperLeft);
			heroNameText.FrontColor = new Color(0.8f, 59f / 255f, 0.117647059f, 1f);
			heroNameText.BackColor = new Color(0f, 28f / 255f, 14f / 51f, 0.1f);
			heroNameText.TextOffset = new Vector2(2f, 2f);
			Add(heroNameText);
			text = GUIControl.CreateControlAbsolute<GUILabel>(new Vector2(130f, 125f), Vector2.zero);
			text.Rotation = -3f;
			text.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, new Color(16f / 255f, 44f / 255f, 19f / 85f), TextAnchor.UpperLeft);
			Add(text);
			textStyle = text.Style;
		}

		public void Setup(HeroHoverHelpInfo info, SHSHoverHelp helpWindow, IGUIControl control)
		{
			string a = AppShell.Instance.CharacterDescriptionManager[info.item.Name].ShortDescription;
			string characterName = AppShell.Instance.CharacterDescriptionManager[info.item.Name].CharacterName;
			bool flag = false;
			float num = 0f;
			float num2 = 0f;
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			if (a == string.Empty)
			{
				a = "Description Not Available";
			}
			GUIImage gUIImage = heroFlash;
			Vector2 burstTextureSize = BurstTextureSize;
			float x = burstTextureSize.x;
			Vector2 burstTextureSize2 = BurstTextureSize;
			gUIImage.Size = new Vector2(x, burstTextureSize2.y);
			heroFlash.Texture = helpWindow.BurstBgTexture;
			heroPortrait.Texture = GUIManager.Instance.LoadTexture("characters_bundle|expandedtooltip_render_" + info.item.Name);
			heroPortrait.SetSize(heroPortrait.Texture.width, heroPortrait.Texture.height);
			GUIImage gUIImage2 = heroFlash;
			Vector2 size = heroFlash.Size;
			float width = size.x * 0.7f;
			Vector2 size2 = heroFlash.Size;
			gUIImage2.SetSize(width, size2.y * 0.7f);
			GUIImage gUIImage3 = heroPortrait;
			Vector2 size3 = heroPortrait.Size;
			float width2 = size3.x * 0.7f;
			Vector2 size4 = heroPortrait.Size;
			gUIImage3.SetSize(width2, size4.y * 0.7f);
			text.InvalidateKerning();
			heroNameText.InvalidateKerning();
			text.Text = a;
			heroNameText.Text = characterName;
			base.nineSliceTexture.VerticalFlip = true;
			vector = textStyle.UnityStyle.CalcSize(new GUIContent(a));
			vector2 = heroNameText.Style.UnityStyle.CalcSize(new GUIContent(heroNameText.Text));
			if (vector2.x > 175f)
			{
				heroNameText.Style.UnityStyle.wordWrap = true;
				vector2.x = 175f;
				heroNameText.VerticalKerning = 14;
				heroNameText.NoLineLimit = true;
				flag = true;
			}
			else
			{
				heroNameText.Style.UnityStyle.wordWrap = false;
				heroNameText.NoLineLimit = false;
			}
			GUIDropShadowTextLabel gUIDropShadowTextLabel = heroNameText;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle = helpWindow.ResourceBundle;
			float x2 = resourceBundle.cornerTextures[0].width + 10;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle2 = helpWindow.ResourceBundle;
			int num3 = resourceBundle2.borderTextures[0].height + 10;
			int num4;
			if (base.nineSliceTexture.VerticalFlip)
			{
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle3 = helpWindow.ResourceBundle;
				num4 = resourceBundle3.attachmentTexture.height;
			}
			else
			{
				num4 = 0;
			}
			gUIDropShadowTextLabel.SetPosition(new Vector2(x2, num3 + num4));
			heroNameText.SetSize(new Vector2(vector2.x, 125f));
			if (flag)
			{
				heroNameText.CalculateTextLayout();
			}
			flag = false;
			if (vector.x > 175f)
			{
				textStyle.UnityStyle.wordWrap = true;
				text.Style = textStyle;
				vector.x = 175f;
				text.VerticalKerning = 14;
				text.NoLineLimit = true;
				flag = true;
			}
			else
			{
				textStyle.UnityStyle.wordWrap = false;
				text.Style = textStyle;
				text.NoLineLimit = false;
			}
			GUILabel gUILabel = text;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle4 = helpWindow.ResourceBundle;
			float x3 = resourceBundle4.cornerTextures[0].width + 10;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle5 = helpWindow.ResourceBundle;
			int num5 = resourceBundle5.borderTextures[0].height + 10 + heroNameText.TotalTextHeight;
			int num6;
			if (base.nineSliceTexture.VerticalFlip)
			{
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle6 = helpWindow.ResourceBundle;
				num6 = resourceBundle6.attachmentTexture.height;
			}
			else
			{
				num6 = 0;
			}
			gUILabel.SetPosition(new Vector2(x3, num5 + num6));
			text.SetSize(new Vector2(vector.x, 125f));
			if (flag)
			{
				text.CalculateTextLayout();
			}
			num = 0f;
			num2 = text.TotalTextHeight + heroNameText.TotalTextHeight;
			float num7 = num2;
			Vector2 size5 = heroFlash.Size;
			if (num7 > size5.y)
			{
				float num8 = num2;
				Vector2 size6 = heroFlash.Size;
				num = num8 - size6.y;
			}
			Vector2 size7 = heroFlash.Size;
			float num9 = size7.y + num + 10f;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle7 = helpWindow.ResourceBundle;
			float num10 = num9 + (float)resourceBundle7.borderTextures[0].height;
			if (control.ScreenRect.y + control.ScreenRect.height + num10 > GUIManager.ScreenRect.height)
			{
				base.nineSliceTexture.VerticalFlip = false;
				GUIDropShadowTextLabel gUIDropShadowTextLabel2 = heroNameText;
				Vector2 position = heroNameText.Position;
				float x4 = position.x;
				Vector2 position2 = heroNameText.Position;
				float y = position2.y;
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle8 = helpWindow.ResourceBundle;
				gUIDropShadowTextLabel2.SetPosition(x4, y - (float)resourceBundle8.attachmentTexture.height);
				GUILabel gUILabel2 = text;
				Vector2 position3 = text.Position;
				float x5 = position3.x;
				Vector2 position4 = text.Position;
				float y2 = position4.y;
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle9 = helpWindow.ResourceBundle;
				gUILabel2.SetPosition(x5, y2 - (float)resourceBundle9.attachmentTexture.height);
				GUIImage gUIImage4 = heroPortrait;
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle10 = helpWindow.ResourceBundle;
				gUIImage4.Offset = new Vector2(0f, -resourceBundle10.attachmentTexture.height / 2);
				GUIImage gUIImage5 = heroFlash;
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle11 = helpWindow.ResourceBundle;
				gUIImage5.Offset = new Vector2(0f, -resourceBundle11.attachmentTexture.height / 2);
			}
			else
			{
				base.nineSliceTexture.VerticalFlip = true;
				GUIImage gUIImage6 = heroPortrait;
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle12 = helpWindow.ResourceBundle;
				gUIImage6.Offset = new Vector2(0f, resourceBundle12.attachmentTexture.height / 2);
				GUIImage gUIImage7 = heroFlash;
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle13 = helpWindow.ResourceBundle;
				gUIImage7.Offset = new Vector2(0f, resourceBundle13.attachmentTexture.height / 2);
			}
			NineSliceTexture nineSliceTexture = base.nineSliceTexture;
			float num11 = (!flag) ? vector.x : ((float)text.LongestLine);
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle14 = helpWindow.ResourceBundle;
			float num12 = num11 + (float)resourceBundle14.cornerTextures[0].width;
			Vector2 size8 = heroFlash.Size;
			Vector2 dwellingSize = new Vector2(num12 + size8.x / 2f + 10f, num10);
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle15 = helpWindow.ResourceBundle;
			Texture2D bodyTexture = resourceBundle15.bodyTexture;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle16 = helpWindow.ResourceBundle;
			Texture2D[] cornerTextures = resourceBundle16.cornerTextures;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle17 = helpWindow.ResourceBundle;
			Texture2D[] borderTextures = resourceBundle17.borderTextures;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle18 = helpWindow.ResourceBundle;
			nineSliceTexture.CreateNineSliceTexture(dwellingSize, bodyTexture, cornerTextures, borderTextures, resourceBundle18.attachmentTexture, NineSliceTexture.Side.Bottom, NineSliceTexture.SideAlignment.Left);
			background.SetSize(new Vector2(base.nineSliceTexture.Texture.width, base.nineSliceTexture.Texture.height));
			background.Rotation = -3f;
			background.Texture = base.nineSliceTexture.Texture;
			background.Alpha = 0.95f;
			float num13 = base.nineSliceTexture.Texture.width;
			Vector2 size9 = heroFlash.Size;
			SetSize(new Vector2(num13 + size9.x / 2f, base.nineSliceTexture.Texture.height));
			float num14 = base.nineSliceTexture.Texture.width;
			Vector2 size10 = heroFlash.Size;
			helpWindow.SetSize(new Vector2(num14 + size10.x / 2f, base.nineSliceTexture.Texture.height - 1));
		}

		public override void OnHide()
		{
			base.OnHide();
			background.Texture = null;
		}
	}

	private class AchievementHoverHelpWindow : HoverHelpWindow
	{
		private const float maxWidth = 175f;

		private const int hoverTextKerning = 14;

		private const int hoverPadding = 10;

		private const int progressTextOffset = 10;

		private GUIImage ItemPortrait;

		private GUIImage achievementFlash;

		private GUIImage background;

		private GUIDropShadowTextLabel name;

		private GUILabel progress;

		private GUILabel text;

		public AchievementHoverHelpWindow()
		{
			nineSliceTexture = new NineSliceTexture();
			nineSliceTexture.AttachmentHOffset = 0;
			nineSliceTexture.AttachmentVOffset = -15;
			background = GUIControl.CreateControlAbsolute<GUIImage>(Vector2.zero, Vector2.zero);
			Add(background);
			achievementFlash = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
			Add(achievementFlash);
			ItemPortrait = GUIControl.CreateControl<GUIImage>(new Vector2(117f, 107f), Vector2.zero, DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
			GUIImage itemPortrait = ItemPortrait;
			Vector2 size = ItemPortrait.Size;
			float x = size.x * 0.7f;
			Vector2 size2 = ItemPortrait.Size;
			itemPortrait.SetSize(new Vector2(x, size2.y * 0.7f));
			Add(ItemPortrait);
			name = GUIControl.CreateControlAbsolute<GUIDropShadowTextLabel>(Vector2.zero, Vector2.zero);
			name.Rotation = -3f;
			name.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 24, Color.black, TextAnchor.UpperLeft);
			name.FrontColor = new Color(0.8f, 59f / 255f, 0.117647059f, 1f);
			name.BackColor = new Color(0f, 28f / 255f, 14f / 51f, 0.1f);
			name.TextOffset = new Vector2(2f, 2f);
			Add(name);
			progress = GUIControl.CreateControlAbsolute<GUILabel>(new Vector2(349f, 202f), new Vector2(227f, 103f));
			progress.Rotation = -3f;
			progress.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, new Color(16f / 255f, 44f / 255f, 19f / 85f), TextAnchor.UpperLeft);
			Add(progress);
			text = GUIControl.CreateControlAbsolute<GUILabel>(new Vector2(180f, 145f), new Vector2(40f, 90f));
			text.Rotation = -3f;
			text.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, new Color(16f / 255f, 44f / 255f, 19f / 85f), TextAnchor.UpperLeft);
			Add(text);
		}

		public void Setup(AchievementHoverHelpInfo info, SHSHoverHelp helpWindow, IGUIControl control)
		{
			string textureSource = string.Empty;
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			bool flag = false;
			if (info.level != Achievement.AchievementLevelEnum.NotAchieved && info.level != Achievement.AchievementLevelEnum.Unknown)
			{
				textureSource = "notification_bundle|" + info.achievement.Id + "_" + info.level.ToString();
			}
			base.nineSliceTexture.VerticalFlip = true;
			GUIImage gUIImage = achievementFlash;
			Vector2 burstTextureSize = BurstTextureSize;
			float x = burstTextureSize.x;
			Vector2 burstTextureSize2 = BurstTextureSize;
			gUIImage.Size = new Vector2(x, burstTextureSize2.y);
			achievementFlash.Texture = helpWindow.BurstBgTexture;
			GUIImage gUIImage2 = achievementFlash;
			Vector2 size = achievementFlash.Size;
			float x2 = size.x * 0.7f;
			Vector2 size2 = achievementFlash.Size;
			gUIImage2.SetSize(new Vector2(x2, size2.y * 0.7f));
			ItemPortrait.TextureSource = textureSource;
			GUIImage itemPortrait = ItemPortrait;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle = helpWindow.ResourceBundle;
			itemPortrait.Offset = new Vector2(0f, resourceBundle.attachmentTexture.height / 2);
			GUIImage gUIImage3 = achievementFlash;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle2 = helpWindow.ResourceBundle;
			gUIImage3.Offset = new Vector2(0f, resourceBundle2.attachmentTexture.height / 2);
			name.InvalidateKerning();
			progress.InvalidateKerning();
			text.InvalidateKerning();
			name.Text = info.achievement.ShortDescription;
			progress.Text = Math.Max(info.achievement.GetCharacterLevelValue(info.heroName, info.bank), 0L) + " / " + info.achievement.GetLevelThreshold(info.level);
			text.Text = info.achievement.GetDescriptionForLevel(info.level);
			vector = name.Style.UnityStyle.CalcSize(new GUIContent(name.Text));
			vector2 = text.Style.UnityStyle.CalcSize(new GUIContent(text.Text));
			if (vector.x > 175f)
			{
				name.Style.UnityStyle.wordWrap = true;
				vector.x = 175f;
				name.VerticalKerning = 14;
				name.NoLineLimit = true;
				flag = true;
			}
			else
			{
				name.Style.UnityStyle.wordWrap = false;
				name.NoLineLimit = false;
			}
			GUIDropShadowTextLabel gUIDropShadowTextLabel = name;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle3 = helpWindow.ResourceBundle;
			float x3 = resourceBundle3.cornerTextures[0].width + 10;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle4 = helpWindow.ResourceBundle;
			int num = resourceBundle4.borderTextures[0].height + 10;
			int num2;
			if (base.nineSliceTexture.VerticalFlip)
			{
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle5 = helpWindow.ResourceBundle;
				num2 = resourceBundle5.attachmentTexture.height;
			}
			else
			{
				num2 = 0;
			}
			gUIDropShadowTextLabel.SetPosition(new Vector2(x3, num + num2));
			name.SetSize(new Vector2(vector.x, 125f));
			if (flag)
			{
				name.CalculateTextLayout();
			}
			GUILabel gUILabel = progress;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle6 = helpWindow.ResourceBundle;
			float x4 = resourceBundle6.cornerTextures[0].width + 10;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle7 = helpWindow.ResourceBundle;
			int num3 = resourceBundle7.borderTextures[0].height + 10;
			int num4;
			if (base.nineSliceTexture.VerticalFlip)
			{
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle8 = helpWindow.ResourceBundle;
				num4 = resourceBundle8.attachmentTexture.height;
			}
			else
			{
				num4 = 0;
			}
			gUILabel.Position = new Vector2(x4, num3 + num4 + name.TotalTextHeight);
			flag = false;
			if (vector2.x > 175f)
			{
				text.Style.UnityStyle.wordWrap = true;
				vector2.x = 175f;
				text.VerticalKerning = 14;
				text.NoLineLimit = true;
				flag = true;
			}
			else
			{
				text.Style.UnityStyle.wordWrap = false;
				text.NoLineLimit = false;
			}
			GUILabel gUILabel2 = text;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle9 = helpWindow.ResourceBundle;
			float x5 = resourceBundle9.cornerTextures[0].width + 10;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle10 = helpWindow.ResourceBundle;
			int num5 = 10 + resourceBundle10.borderTextures[0].height + 10 + name.TotalTextHeight;
			int num6;
			if (base.nineSliceTexture.VerticalFlip)
			{
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle11 = helpWindow.ResourceBundle;
				num6 = resourceBundle11.attachmentTexture.height;
			}
			else
			{
				num6 = 0;
			}
			float num7 = num5 + num6;
			Vector2 vector3 = progress.Style.UnityStyle.CalcSize(new GUIContent(progress.Text));
			gUILabel2.SetPosition(new Vector2(x5, num7 + vector3.y));
			text.SetSize(new Vector2(vector2.x, 125f));
			if (flag)
			{
				text.CalculateTextLayout();
			}
			float num8 = 0f;
			float num9 = text.TotalTextHeight + name.TotalTextHeight;
			Vector2 size3 = achievementFlash.Size;
			if (num9 > size3.y)
			{
				Vector2 size4 = achievementFlash.Size;
				num8 = num9 - size4.y;
			}
			base.nineSliceTexture.HorizontalFlip = (info.horizontalFlipOverride == HoverHelpInfo.FlipOverride.On);
			Vector2 size5 = achievementFlash.Size;
			float num10 = size5.y + num8 + 10f;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle12 = helpWindow.ResourceBundle;
			float num11 = num10 + (float)resourceBundle12.borderTextures[0].height;
			if (info.verticalFlipOverride == HoverHelpInfo.FlipOverride.Off || (info.verticalFlipOverride == HoverHelpInfo.FlipOverride.Auto && control.ScreenRect.y + control.ScreenRect.height + num11 > GUIManager.ScreenRect.height))
			{
				base.nineSliceTexture.VerticalFlip = false;
				GUIDropShadowTextLabel gUIDropShadowTextLabel2 = name;
				Vector2 position = name.Position;
				float x6 = position.x;
				Vector2 position2 = name.Position;
				float y = position2.y;
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle13 = helpWindow.ResourceBundle;
				gUIDropShadowTextLabel2.SetPosition(x6, y - (float)resourceBundle13.attachmentTexture.height);
				GUILabel gUILabel3 = progress;
				Vector2 position3 = progress.Position;
				float x7 = position3.x;
				Vector2 position4 = progress.Position;
				float y2 = position4.y;
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle14 = helpWindow.ResourceBundle;
				gUILabel3.SetPosition(x7, y2 - (float)resourceBundle14.attachmentTexture.height);
				GUILabel gUILabel4 = text;
				Vector2 position5 = text.Position;
				float x8 = position5.x;
				Vector2 position6 = text.Position;
				float y3 = position6.y;
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle15 = helpWindow.ResourceBundle;
				gUILabel4.SetPosition(x8, y3 - (float)resourceBundle15.attachmentTexture.height);
				GUIImage itemPortrait2 = ItemPortrait;
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle16 = helpWindow.ResourceBundle;
				itemPortrait2.Offset = new Vector2(-26f, -resourceBundle16.attachmentTexture.height / 2);
				GUIImage gUIImage4 = achievementFlash;
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle17 = helpWindow.ResourceBundle;
				gUIImage4.Offset = new Vector2(0f, -resourceBundle17.attachmentTexture.height / 2);
			}
			else
			{
				base.nineSliceTexture.VerticalFlip = true;
				GUIImage itemPortrait3 = ItemPortrait;
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle18 = helpWindow.ResourceBundle;
				itemPortrait3.Offset = new Vector2(-26f, resourceBundle18.attachmentTexture.height / 2);
				GUIImage gUIImage5 = achievementFlash;
				GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle19 = helpWindow.ResourceBundle;
				gUIImage5.Offset = new Vector2(0f, resourceBundle19.attachmentTexture.height / 2);
			}
			NineSliceTexture nineSliceTexture = base.nineSliceTexture;
			float x9 = vector2.x;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle20 = helpWindow.ResourceBundle;
			float num12 = x9 + (float)resourceBundle20.cornerTextures[0].width;
			Vector2 size6 = achievementFlash.Size;
			float x10 = num12 + size6.x / 2f + 10f;
			Vector2 size7 = achievementFlash.Size;
			float num13 = size7.y + num8 + 10f;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle21 = helpWindow.ResourceBundle;
			Vector2 dwellingSize = new Vector2(x10, num13 + (float)resourceBundle21.borderTextures[0].height);
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle22 = helpWindow.ResourceBundle;
			Texture2D bodyTexture = resourceBundle22.bodyTexture;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle23 = helpWindow.ResourceBundle;
			Texture2D[] cornerTextures = resourceBundle23.cornerTextures;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle24 = helpWindow.ResourceBundle;
			Texture2D[] borderTextures = resourceBundle24.borderTextures;
			GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle25 = helpWindow.ResourceBundle;
			nineSliceTexture.CreateNineSliceTexture(dwellingSize, bodyTexture, cornerTextures, borderTextures, resourceBundle25.attachmentTexture, NineSliceTexture.Side.Bottom, NineSliceTexture.SideAlignment.Left);
			background.SetSize(new Vector2(base.nineSliceTexture.Texture.width, base.nineSliceTexture.Texture.height));
			background.Rotation = -3f;
			background.Texture = base.nineSliceTexture.Texture;
			float num14 = base.nineSliceTexture.Texture.width;
			Vector2 size8 = achievementFlash.Size;
			SetSize(new Vector2(num14 + size8.x / 2f, base.nineSliceTexture.Texture.height));
			float num15 = base.nineSliceTexture.Texture.width;
			Vector2 size9 = achievementFlash.Size;
			helpWindow.SetSize(new Vector2(num15 + size9.x / 2f, base.nineSliceTexture.Texture.height - 1));
			ItemPortrait.Rotation = -3f;
		}

		public override void OnHide()
		{
			base.OnHide();
			background.Texture = null;
		}
	}

	private const float attachmentOffset = 20f;

	private const float attachmentRightOffset = 90f;

	public GUIImage background;

	private GenericHoverHelpWindow GenericHoverHelp;

	private InventoryHoverHelpWindow InventoryHoverHelp;

	private HeroHoverHelpWindow HeroHoverHelp;

	private AchievementHoverHelpWindow AchievementHoverHelp;

	private bool resourceBundleAcquired;

	private GUIToolTipManager.ToolTipResources.ResourceBundle resourceBundle;

	private Texture2D burstBgTexture;

	private GUIToolTipManager.ToolTipResources toolTipResources;

	public static readonly Vector2 HOVER_HELP_SIZE = new Vector2(339f, 202f);

	public static readonly Vector2 BurstTextureSize = new Vector2(192f, 192f);

	public GUIToolTipManager.ToolTipResources.ResourceBundle ResourceBundle
	{
		get
		{
			return resourceBundle;
		}
	}

	public Texture2D BurstBgTexture
	{
		get
		{
			return burstBgTexture;
		}
	}

	public GUIToolTipManager.ToolTipResources ToolTipResource
	{
		set
		{
			toolTipResources = value;
		}
	}

	public SHSHoverHelp()
	{
		Traits.HitTestType = HitTestTypeEnum.Transparent;
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		Traits.VisibleAncestryTrait = ControlTraits.VisibilityAncestryTraitEnum.EnsureAncestorsVisible;
		SetSize(HOVER_HELP_SIZE);
		SetPosition(1f, 1f);
		background = GUIControl.CreateControlAbsolute<GUIImage>(HOVER_HELP_SIZE, new Vector2(0f, 0f));
		background.TextureSource = "common_bundle|mshs_expandedtooltip_container";
		GenericHoverHelp = new GenericHoverHelpWindow();
		Add(GenericHoverHelp);
		InventoryHoverHelp = new InventoryHoverHelpWindow();
		Add(InventoryHoverHelp);
		HeroHoverHelp = new HeroHoverHelpWindow();
		Add(HeroHoverHelp);
		AchievementHoverHelp = new AchievementHoverHelpWindow();
		Add(AchievementHoverHelp);
	}

	public void DisplayHoverHelp(IGUIControl Control, HoverHelpInfo info)
	{
		if (burstBgTexture == null)
		{
			burstBgTexture = GUIManager.Instance.LoadTexture("GUI/ToolTipTextures/mshs_expanded_tooltip_flash");
		}
		if (!resourceBundleAcquired)
		{
			resourceBundleAcquired = true;
			resourceBundle = toolTipResources.GetBundle("default");
		}
		configureWindow(info, Control);
		configurePosition(info, Control);
	}

	private void HideAndShow(HoverHelpWindow window)
	{
		window.Show();
		if (window != GenericHoverHelp)
		{
			GenericHoverHelp.Hide();
		}
		if (window != InventoryHoverHelp)
		{
			InventoryHoverHelp.Hide();
		}
		if (window != HeroHoverHelp)
		{
			HeroHoverHelp.Hide();
		}
		if (window != AchievementHoverHelp)
		{
			AchievementHoverHelp.Hide();
		}
	}

	private void configureWindow(HoverHelpInfo info, IGUIControl control)
	{
		if (info is GenericHoverHelpInfo)
		{
			GenericHoverHelp.Setup(info as GenericHoverHelpInfo, this);
			HideAndShow(GenericHoverHelp);
		}
		else if (info is InventoryHoverHelpInfo)
		{
			InventoryHoverHelp.Setup(info as InventoryHoverHelpInfo, this);
			HideAndShow(InventoryHoverHelp);
		}
		else if (info is HeroHoverHelpInfo)
		{
			HeroHoverHelp.Setup(info as HeroHoverHelpInfo, this, control);
			HideAndShow(HeroHoverHelp);
		}
		else if (info is AchievementHoverHelpInfo)
		{
			AchievementHoverHelp.Setup(info as AchievementHoverHelpInfo, this, control);
			HideAndShow(AchievementHoverHelp);
		}
	}

	private void configurePosition(HoverHelpInfo info, IGUIControl ctrl)
	{
		Rect screenRect = ScreenRect;
		Rect screenRect2 = GUIManager.ScreenRect;
		float num = 0f;
		float num2 = 0f;
		num = ctrl.ScreenRect.x;
		num2 = ctrl.ScreenRect.y + ctrl.Rect.height - 20f;
		if (info.extendLeft)
		{
			num += ctrl.Rect.width - base.rect.width + 90f;
		}
		if (num2 + screenRect.height > screenRect2.height)
		{
			float y = ctrl.ScreenRect.y;
			Vector2 size = Size;
			num2 = y - size.y + 20f;
		}
		float num3 = num;
		Vector2 size2 = Size;
		if (num3 + size2.x > screenRect2.width)
		{
			float num4 = num;
			float num5 = num;
			Vector2 size3 = Size;
			num = num4 - (num5 + size3.x - screenRect2.width);
		}
		SetPosition(new Vector2(num, num2));
	}

	private void backgroundFlip(bool flip)
	{
		if (flip)
		{
			background.TextureSource = "common_bundle|mshs_expandedtooltip_container";
		}
		else
		{
			background.TextureSource = "common_bundle|mshs_expandedtooltip_container_flipped";
		}
	}
}
