using UnityEngine;

internal class SHSBrawlerHeroUpBubble : GUIControlWindow
{
	private const float nativeX = 88f;

	private const float nativeY = 90f;

	private const float fadeTime = 1f;

	private const float flashTime = 0.25f;

	public static Vector2 minScreenOffset = new Vector2(-44f, -135f);

	public static Vector2 maxScreenOffset = new Vector2(-88f, -270f);

	public static Vector2 minScreenSize = new Vector2(44f, 45f);

	public static Vector2 maxScreenSize = new Vector2(88f, 90f);

	public static float minScaleDistance = 15f;

	public static float maxScaleDistance = 40f;

	public static float screenDist = 0f;

	protected float fadeTimer;

	private bool mFadeOut;

	protected float flashTimer;

	private GUIImage mHeroBubble;

	private GUIImage mHeroBubbleHighlight;

	private GameObject mOwner;

	public SHSBrawlerHeroUpBubble()
	{
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		Traits.HitTestType = HitTestTypeEnum.Transparent;
		SetPosition(0f, 0f);
		SetSize(88f, 90f);
		mHeroBubble = CreateHeroUpImage("brawler_bundle|mshs_brawler_hud_heroup_bubble");
		mHeroBubbleHighlight = CreateHeroUpImage("brawler_bundle|mshs_brawler_hud_heroup_bubble_flash");
		Add(mHeroBubble);
		Add(mHeroBubbleHighlight);
	}

	public override void Update()
	{
		base.Update();
		if (mOwner != null)
		{
			Vector3 viewport = Camera.main.WorldToViewportPoint(mOwner.transform.position);
			UpdateScale(viewport.z);
			UpdatePosition(viewport);
			UpdateFade();
		}
	}

	public void SetAlpha(float alpha)
	{
		mHeroBubble.Alpha = alpha;
		mHeroBubbleHighlight.Alpha = alpha;
	}

	public void AttachHeroUpBubble(GameObject obj)
	{
		if (!(obj == null))
		{
			mOwner = obj;
			IsVisible = true;
			SetAlpha(0f);
			fadeTimer = 1f;
			mFadeOut = true;
			Vector3 viewport = Camera.main.WorldToViewportPoint(mOwner.transform.position);
			UpdateScale(viewport.z);
			UpdatePosition(viewport);
		}
	}

	public void DetachHeroUpBubble()
	{
		if (!(mOwner == null))
		{
			mOwner = null;
			IsVisible = false;
		}
	}

	public void UpdatePowerPercent(float percent)
	{
		if (percent >= 1f)
		{
			if (mFadeOut)
			{
				fadeTimer = 0f;
				mFadeOut = false;
			}
		}
		else if (!mFadeOut)
		{
			fadeTimer = 0f;
			mFadeOut = true;
		}
	}

	private GUIImage CreateHeroUpImage(string heroUpImgPath)
	{
		GUIImage gUIImage = new GUIImage();
		gUIImage.TextureSource = heroUpImgPath;
		gUIImage.SetSize(88f, 90f);
		gUIImage.SetPosition(Vector2.zero);
		return gUIImage;
	}

	private void UpdatePosition(Vector3 viewport)
	{
		float num = ScreenRect.width / (2f * parent.ScreenRect.width);
		float num2 = ScreenRect.height / (2f * parent.ScreenRect.height);
		Vector2 vector = Vector2.Lerp(maxScreenOffset, minScreenOffset, screenDist);
		float num3 = vector.x / parent.ScreenRect.width;
		float num4 = vector.y / parent.ScreenRect.height;
		float x = (viewport.x - num + num3) * parent.ScreenRect.width;
		float y = (1f - viewport.y + num2 + num4) * parent.ScreenRect.height;
		SetPosition(x, y);
	}

	private void UpdateScale(float disToTarget)
	{
		float num = Mathf.Clamp(disToTarget, minScaleDistance, maxScaleDistance);
		screenDist = (num - minScaleDistance) / (maxScaleDistance - minScaleDistance);
		Vector2 size = Vector2.Lerp(maxScreenSize, minScreenSize, screenDist);
		SetSize(size);
		mHeroBubble.SetSize(size);
		mHeroBubbleHighlight.SetSize(size);
	}

	private void UpdateFade()
	{
		if (!mFadeOut)
		{
			flashTimer += Time.deltaTime;
			if (flashTimer > 0.25f)
			{
				flashTimer = 0f;
				mHeroBubbleHighlight.IsVisible = !mHeroBubbleHighlight.IsVisible;
			}
		}
		if (!(fadeTimer >= 1f))
		{
			fadeTimer += Time.deltaTime;
			float num = Mathf.Clamp01(fadeTimer / 1f);
			if (mFadeOut)
			{
				num = 1f - num;
			}
			SetAlpha(num);
		}
	}
}
