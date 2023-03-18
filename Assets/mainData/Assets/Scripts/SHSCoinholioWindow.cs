using UnityEngine;

public class SHSCoinholioWindow : GUINotificationWindow
{
	public enum CoinAwardAmount
	{
		GoldOne,
		GoldFive,
		GoldTen,
		GoldFifty,
		GoldEagle,
		GoldDouble,
		SilverOne,
		SilverFive,
		SilverTen,
		SilverFifty,
		SilverEagle
	}

	private GUIImage coinImage;

	private float fadeStartTime = 0.1f;

	private float fadeDurationTime = 1f;

	private Vector2 startLocation;

	private Vector2 endLocation;

	private float rotationDir = 1f;

	private float rotationDirRange = 4f;

	private float scaleReductionBase = 0.5f;

	private float alphaReducationBase = 1.6f;

	private static readonly string[] CoinImages = new string[11]
	{
		"L_coin_gold_1",
		"L_coin_gold_5",
		"L_coin_gold_10",
		"L_coin_gold_50",
		"L_coin_gold_eagle",
		"coins",
		"L_coin_silver_1",
		"L_coin_silver_5",
		"L_coin_silver_10",
		"L_coin_silver_50",
		"coin_silver_eagle"
	};

	public SHSCoinholioWindow()
		: this(CoinAwardAmount.SilverOne)
	{
	}

	public SHSCoinholioWindow(Vector2 endLocation, int coinAmount, bool silver)
		: this(GetAwardFromCount(coinAmount, silver))
	{
		this.endLocation = endLocation;
	}

	public SHSCoinholioWindow(CoinAwardAmount awardType)
	{
		rotationDir = Random.Range(0f - rotationDirRange, rotationDirRange);
		coinImage = new GUIImage();
		coinImage.SetPositionAndSize(QuickSizingHint.ParentSize);
		coinImage.TextureSource = "notification_bundle|" + CoinImages[(int)awardType];
		Add(coinImage, DrawOrder.DrawLast, DrawPhaseHintEnum.PostDraw);
	}

	public static CoinAwardAmount GetAwardFromCount(int coinCount, bool silver)
	{
		if (!silver)
		{
			if (coinCount < 5)
			{
				return CoinAwardAmount.GoldOne;
			}
			if (coinCount < 10)
			{
				return CoinAwardAmount.GoldFive;
			}
			if (coinCount < 50)
			{
				return CoinAwardAmount.GoldTen;
			}
			if (coinCount < 100)
			{
				return CoinAwardAmount.GoldFifty;
			}
			if (coinCount < 200)
			{
				return CoinAwardAmount.GoldEagle;
			}
			return CoinAwardAmount.GoldDouble;
		}
		if (coinCount < 5)
		{
			return CoinAwardAmount.SilverOne;
		}
		if (coinCount < 10)
		{
			return CoinAwardAmount.SilverFive;
		}
		if (coinCount < 50)
		{
			return CoinAwardAmount.SilverTen;
		}
		if (coinCount < 100)
		{
			return CoinAwardAmount.SilverFifty;
		}
		return CoinAwardAmount.SilverEagle;
	}

	public override void OnShow()
	{
		base.OnShow();
		Vector2 vector = new Vector2(GUIManager.ScreenRect.width / 2.1f, GUIManager.ScreenRect.height / 2.1f);
		startLocation = new Vector2(vector.x - Random.Range(5f, vector.x * 0.25f), GUIManager.ScreenRect.height - vector.y + Random.Range(15f, vector.y * 0.25f));
		SetPositionAndSize(AnchorAlignmentEnum.Middle, startLocation, new Vector2(73f, 74f));
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("earn_coins"));
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		float num = Time.time - timeStarted;
		float num2 = num - fadeStartTime;
		if (num > fadeStartTime)
		{
			coinImage.Alpha = 1f - num2 / fadeDurationTime * alphaReducationBase;
			coinImage.Scale = 1f - num2 / fadeDurationTime * scaleReductionBase;
			rotation += rotationDir;
			Vector2 position = new Vector2(startLocation.x + (endLocation.x - startLocation.x) * (num2 / fadeDurationTime), startLocation.y + (endLocation.y - startLocation.y) * num2 / fadeDurationTime);
			SetPositionAndSize(AnchorAlignmentEnum.Middle, position, new Vector2(73f, 74f));
		}
		if (coinImage.Alpha <= 0f)
		{
			Hide();
		}
	}
}
