using UnityEngine;

public class CardZoomComponent
{
	public class ZoomData
	{
		public Vector2 normalCardSize = new Vector2(244f, 340f);

		public Vector2 zoomCardSize = new Vector2(366f, 512f);

		public Vector2 normalOffset = new Vector2(0f, 0f);

		public Vector2 zoomOffset = new Vector2(0f, 0f);

		public float zoomInDelay = 0.5f;

		public float zoomOutDelay = 0.5f;

		public ZoomData(Vector2 normalCardSize, Vector2 normalOffset, Vector2 zoomCardSize, Vector2 zoomOffset, float zoomInDelay, float zoomOutDelay)
		{
			this.normalCardSize = normalCardSize;
			this.normalOffset = normalOffset;
			this.zoomCardSize = zoomCardSize;
			this.zoomOffset = zoomOffset;
			this.zoomInDelay = zoomInDelay;
			this.zoomOutDelay = zoomOutDelay;
		}
	}

	protected class GrowAndShrinkCard : SHSAnimations
	{
		public static AnimClip GrowCard(GUIDrawTexture prizeCard, ZoomData data)
		{
			float x = data.normalCardSize.x;
			float x2 = data.zoomCardSize.x;
			Vector2 size = prizeCard.Size;
			float time = GenericFunctions.FrationalTime(x, x2, size.x, 0.3f);
			return Absolute.SizeX(GenericPaths.LinearWithWiggle(data.normalCardSize.x, data.zoomCardSize.x, time), prizeCard) ^ Absolute.SizeY(GenericPaths.LinearWithWiggle(data.normalCardSize.y, data.zoomCardSize.y, time), prizeCard) ^ Absolute.OffsetX(GenericPaths.LinearWithWiggle(data.normalOffset.x, data.zoomOffset.x, time), prizeCard);
		}

		public static AnimClip ShrinkCard(GUIDrawTexture prizeCard, ZoomData data)
		{
			float x = data.zoomCardSize.x;
			float x2 = data.normalCardSize.x;
			Vector2 size = prizeCard.Size;
			float time = GenericFunctions.FrationalTime(x, x2, size.x, 0.3f);
			return Absolute.SizeX(Path.Linear(data.zoomCardSize.x, data.normalCardSize.x, time), prizeCard) ^ Absolute.SizeY(Path.Linear(data.zoomCardSize.y, data.normalCardSize.y, time), prizeCard) ^ Absolute.OffsetX(Path.Linear(data.zoomOffset.x, data.normalOffset.x, time), prizeCard);
		}
	}

	private const float CARD_ZOOM_TIME = 0.5f;

	private bool zooming;

	private AnimClip CardZoomAnimation;

	private bool mouseIsOverCard;

	private float lastMouseOverTime;

	private bool explicitZoom;

	private AnimClipManager animClipManager;

	private GUIDrawTexture cardImage;

	private ZoomData zoomData;

	private AnimClip growAndShrink;

	public bool IsMouseOver
	{
		set
		{
			if (mouseIsOverCard != value)
			{
				mouseIsOverCard = !mouseIsOverCard;
				lastMouseOverTime = Time.time;
			}
		}
	}

	public bool IsZooming
	{
		get
		{
			return zooming;
		}
	}

	public CardZoomComponent(AnimClipManager animClipManager, GUIDrawTexture cardImage, ZoomData zoomData)
	{
		this.animClipManager = animClipManager;
		this.cardImage = cardImage;
		this.zoomData = zoomData;
	}

	public void Update()
	{
		if (!mouseIsOverCard && !explicitZoom && Time.time - lastMouseOverTime > zoomData.zoomOutDelay)
		{
			ZoomOutOnCard();
		}
		if (mouseIsOverCard && Time.time - lastMouseOverTime > zoomData.zoomInDelay)
		{
			ZoomInOnCard(false);
		}
	}

	public void ZoomOutOnCard()
	{
		if (zooming && cardImage != null)
		{
			zooming = false;
			animClipManager.SwapOut(ref growAndShrink, GrowAndShrinkCard.ShrinkCard(cardImage, zoomData));
		}
	}

	public void ZoomInOnCard(bool explicitZoom)
	{
		this.explicitZoom = explicitZoom;
		if (!zooming && cardImage != null)
		{
			zooming = true;
			animClipManager.SwapOut(ref growAndShrink, GrowAndShrinkCard.GrowCard(cardImage, zoomData));
		}
	}
}
