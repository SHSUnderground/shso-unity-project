using UnityEngine;

public class SHSBrawlerIndicatorArrow : SHSIndicatorArrow
{
	private GUIImage icon;

	public readonly string defaultIndicatorIcon = "brawler_hud_indicator_defeat";

	protected override bool ArrowVisible
	{
		get
		{
			return base.ArrowVisible;
		}
		set
		{
			icon.IsVisible = value;
			base.ArrowVisible = value;
		}
	}

	public SHSBrawlerIndicatorArrow()
	{
		icon = new GUIImage();
		icon.TextureSource = "brawler_bundle|" + defaultIndicatorIcon;
		icon.SetPositionAndSize(Vector2.zero, new Vector2(32f, 32f));
		icon.IsVisible = true;
	}

	public void SetIndicatorIcon(string iconName)
	{
		icon.TextureSource = "brawler_bundle|" + iconName;
	}

	protected override void SetArrowSize(float width, float height)
	{
		base.SetArrowSize(width, height);
		icon.SetSize(width / 2f, height / 2f);
	}

	protected override void SetArrowPosition(Vector2 screenPos)
	{
		base.SetArrowPosition(screenPos);
		icon.SetPosition(screenPos + (arrow.Size - icon.Size) / 2f);
	}
}
