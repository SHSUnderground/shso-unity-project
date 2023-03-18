using UnityEngine;

public class SHSCurrencyWindow : GUISimpleControlWindow
{
	private GUIImage currencyBgdTexture;

	private GUIImage currencyIconTexture;

	private GUIImage currencyLabelTexture;

	private GUILabel currency;

	public string BgdTextureSource
	{
		get
		{
			return currencyBgdTexture.TextureSource;
		}
		set
		{
			currencyBgdTexture.TextureSource = value;
		}
	}

	public string IconTextureSource
	{
		get
		{
			return currencyIconTexture.TextureSource;
		}
		set
		{
			currencyIconTexture.TextureSource = value;
		}
	}

	public string LabelTextureSource
	{
		get
		{
			return currencyLabelTexture.TextureSource;
		}
		set
		{
			currencyLabelTexture.TextureSource = value;
		}
	}

	public GUILabel CurrencyLabel
	{
		get
		{
			return currency;
		}
	}

	public SHSCurrencyWindow()
	{
		currencyBgdTexture = new GUIImage();
		currencyBgdTexture.SetPositionAndSize(QuickSizingHint.ParentSize);
		Add(currencyBgdTexture);
		currencyIconTexture = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(82f, 80f), new Vector2(62f, -6f));
		currencyIconTexture.AutoSizeToTexture = true;
		Add(currencyIconTexture);
		currencyLabelTexture = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(82f, 80f), new Vector2(-29f, -31f));
		currencyLabelTexture.AutoSizeToTexture = true;
		Add(currencyLabelTexture);
		currency = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(120f, 30f), new Vector2(-31f, -7f));
		Add(currency);
	}
}
