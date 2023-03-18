using UnityEngine;

public class GlobalNavButton : GUISimpleControlWindow
{
	public GlobalNav.GlobalNavType type;

	public GUIHotSpotButton background;

	private GUIImage bgl;

	private GUIImage bgc;

	private GUIImage bgr;

	private GUIImage icon;

	private GUILabel label;

	private Vector2 _cachedSize = Vector2.zero;

	public GlobalNavButton(string labelStr, string iconPath, GlobalNav.GlobalNavType type)
	{
		this.type = type;
		HitTestType = HitTestTypeEnum.Rect;
		BlockTestType = BlockTestTypeEnum.Rect;
		base.HitTestSize = new Vector2(1f, 1f);
		background = GUIControl.CreateControlTopLeftFrame<GUIHotSpotButton>(new Vector2(90f, 40f), Vector2.zero);
		background.Style = GlobalNav.GlobalNavStyle;
		background.Color = new Color(1f, 0f, 0f);
		background.HitTestType = HitTestTypeEnum.Rect;
		background.BlockTestType = BlockTestTypeEnum.Rect;
		background.HitTestSize = new Vector2(1f, 1f);
		background.MouseOut += delegate
		{
			ShsAudioSource.PlayAutoSound(GlobalNav.GlobalNavStyle.AudioOut);
			roll(false);
		};
		background.MouseOver += delegate
		{
			ShsAudioSource.PlayAutoSound(GlobalNav.GlobalNavStyle.AudioOver);
			roll(true);
		};
		Add(background);
		bgl = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(16f, 50f), Vector2.zero);
		bgl.TextureSource = "hud_bundle|globalnav_underlay3l_normal";
		bgl.IsVisible = true;
		Add(bgl);
		bgc = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(100f, 50f), Vector2.zero);
		bgc.TextureSource = "hud_bundle|globalnav_underlay3c_normal";
		bgc.IsVisible = true;
		Add(bgc);
		label = new GUILabel();
		label.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(50f, 0f), new Vector2(80f, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, GUILabel.GenColor(104, 160, 9), TextAnchor.MiddleLeft);
		label.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		label.Id = "label";
		label.Text = labelStr;
		float textBlockSize = GUINotificationWindow.GetTextBlockSize(new GUILabel[1]
		{
			label
		}, GUINotificationWindow.BlockSizeType.Width);
		bgc.SetSize(new Vector2(textBlockSize + 60f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		label.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(55f, 0f), new Vector2(textBlockSize, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetSize(bgc.Size);
		background.SetSize(bgc.Size);
		icon = new GUIImage();
		icon.SetSize(new Vector2(102f, 102f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		icon.TextureSource = iconPath;
		icon.Id = "icon";
		Add(icon);
	}

	private void roll(bool over)
	{
		if (over)
		{
			bgl.TextureSource = "hud_bundle|globalnav_underlay3l_highlight";
			bgc.TextureSource = "hud_bundle|globalnav_underlay3c_highlight";
			bgr.TextureSource = "hud_bundle|globalnav_underlay3r_highlight";
		}
		else
		{
			bgl.TextureSource = "hud_bundle|globalnav_underlay3l_normal";
			bgc.TextureSource = "hud_bundle|globalnav_underlay3c_normal";
			bgr.TextureSource = "hud_bundle|globalnav_underlay3r_normal";
		}
	}

	public void adjustWidth(int newWidth)
	{
		float x = newWidth;
		Vector2 size = Size;
		SetSize(new Vector2(x, size.y));
		background.SetSize(Size);
		_cachedSize = Size;
		bgc.SetPosition(new Vector2(16f, 0f));
		GUIImage gUIImage = bgc;
		float x2 = newWidth - 32;
		Vector2 size2 = background.Size;
		gUIImage.SetSize(new Vector2(x2, size2.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Vector2 size3 = new Vector2(16f, 50f);
		Vector2 position = bgc.Position;
		Vector2 size4 = bgc.Size;
		bgr = GUIControl.CreateControlTopLeftFrame<GUIImage>(size3, position + new Vector2(size4.x, 0f));
		bgr.TextureSource = "hud_bundle|globalnav_underlay3r_normal";
		bgr.IsVisible = true;
		Add(bgr);
		Add(label);
		icon.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, new Vector2(-22f, -26f));
		label.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(46f, 14f), new Vector2(newWidth, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
	}
}
