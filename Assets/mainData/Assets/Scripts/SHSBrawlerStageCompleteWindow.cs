using UnityEngine;

public class SHSBrawlerStageCompleteWindow : GUIWindow
{
	private const float DISPLAY_TIME = 8f;

	private const float RAY_ROTATION_RATE = 45f;

	private const float RAY_SCALE_START = 1f;

	private const float RAY_SCALE_END = 1.0625f;

	private const float BACK_STAR_SCALE_START = 1f;

	private const float BACK_STAR_SCALE_END = 1.25f;

	private const float FRONT_STAR_SCALE_START = 1f;

	private const float FRONT_STAR_SCALE_END = 1.125f;

	private const float SCALE_PERIOD = 2f;

	private GUIDrawTexture raysTop;

	private GUIDrawTexture raysBottom;

	private GUIDrawTexture backStar;

	private GUIDrawTexture frontStar;

	private GUIDrawTexture text;

	private float elapsedTime;

	private bool timerActive;

	private float currentScaleTime;

	public SHSBrawlerStageCompleteWindow()
	{
		raysTop = new GUIDrawTexture();
		raysTop.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero, new Vector2(454f, 454f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		raysTop.Rotation = 180f;
		raysTop.IsVisible = true;
		raysTop.TextureSource = "brawler_bundle|raysTop";
		Add(raysTop);
		raysBottom = new GUIDrawTexture();
		raysBottom.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero, new Vector2(454f, 454f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		raysBottom.Rotation = 0f;
		raysBottom.IsVisible = true;
		raysBottom.TextureSource = "brawler_bundle|raysBottom";
		Add(raysBottom);
		backStar = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(new Vector2(230f, 233f), new Vector2(-3f, 0f));
		backStar.Rotation = 0f;
		backStar.IsVisible = true;
		backStar.TextureSource = "brawler_bundle|backstar";
		Add(backStar);
		frontStar = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(new Vector2(202f, 195f), new Vector2(0f, -8f));
		frontStar.Rotation = 0f;
		frontStar.IsVisible = true;
		frontStar.TextureSource = "brawler_bundle|frontstar";
		Add(frontStar);
		text = new GUIDrawTexture();
		text.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero, new Vector2(364f, 182f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		text.Rotation = 0f;
		text.IsVisible = true;
		text.TextureSource = "brawler_bundle|L_text";
		Add(text);
		elapsedTime = 0f;
	}

	private void DoSummaryComplete()
	{
		AppShell.Instance.EventMgr.Fire(this, new BrawlerSummaryCompleteMessage());
		Hide();
	}

	private void onTimerEvent(SHSTimerEx.TimerEventType Type, int data)
	{
		if (Type == SHSTimerEx.TimerEventType.Completed)
		{
			DoSummaryComplete();
		}
	}

	public override void DrawPreprocess()
	{
		base.DrawPreprocess();
	}

	public override void DrawFinalize()
	{
		base.DrawFinalize();
	}

	public override void Show()
	{
		base.Show();
		SHSBrawlerMainWindow sHSBrawlerMainWindow = (SHSBrawlerMainWindow)GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow"];
		sHSBrawlerMainWindow.SetPowerBarVisibility(false);
		elapsedTime = 0f;
		timerActive = true;
		GUIManager.Instance.TogglePersistentGUI(GUIManager.PersistentGUIToggleOption.Visibility, true);

		
	}

	public override void Hide()
	{
		base.Hide();
		timerActive = false;
		GUIManager.Instance.TogglePersistentGUI(GUIManager.PersistentGUIToggleOption.Visibility, false);
	}

	public override void Update()
	{
		base.Update();
		if (timerActive)
		{
			elapsedTime += Time.deltaTime;
			if (elapsedTime > 8f)
			{
				onTimerEvent(SHSTimerEx.TimerEventType.Completed, 0);
			}
		}
		if (isVisible)
		{
			raysTop.Rotation += 45f * Time.deltaTime;
			raysBottom.Rotation += 45f * Time.deltaTime;
			currentScaleTime = (currentScaleTime + Time.deltaTime) % 2f;
			float num = currentScaleTime * 2f / 2f;
			if (num > 1f)
			{
				num = 1f - (num - 1f);
			}
			num = 3f * (num * num) - 2f * (num * num * num);
			raysTop.Scale = 1f * (1f - num) + 1.0625f * num;
			raysBottom.Scale = 1f * (1f - num) + 1.0625f * num;
			backStar.Scale = 1f * (1f - num) + 1.25f * num;
			frontStar.Scale = 1f * (1f - num) + 1.125f * num;
		}
	}
}
