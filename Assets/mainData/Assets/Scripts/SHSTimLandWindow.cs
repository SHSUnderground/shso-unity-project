using UnityEngine;

public class SHSTimLandWindow : GUITopLevelWindow
{
	private GUILabel testLabel;

	private GUITextArea testTA;

	private GUIMovieTexture movieTest;

	private GUIMovieTexture streamTest;

	private static float drawTime = Time.time;

	private static int fps = 0;

	public SHSTimLandWindow()
		: base("SHSTimLandWindow")
	{
	}

	public override bool InitializeResources(bool reload)
	{
		testLabel = new GUILabel();
		testLabel.SetPositionAndSize(20f, 20f, 200f, 200f);
		testLabel.Text = "Cheese";
		testLabel.TextColor = new Color(1f, 1f, 1f, 1f);
		testTA = new GUITextArea();
		testTA.SetPositionAndSize(120f, 120f, 200f, 200f);
		testTA.Text = "Cheese is pleasing, more so than phlegm.";
		testTA.TextColor = new Color(0.5f, 1f, 1f, 1f);
		testTA.FontSize = 45;
		testTA.TextColor = Color.red;
		testTA.Italicized = true;
		Add(testTA);
		Add(testLabel);
		GUIDrawTexture gUIDrawTexture = new GUIDrawTexture();
		gUIDrawTexture.SetPositionAndSize(0f, 300f, 300f, 300f);
		gUIDrawTexture.Traits.ResourceLoadingTrait = ControlTraits.ResourceLoadingTraitEnum.Async;
		gUIDrawTexture.TextureSource = "toolbox_bundle|contentField_435x293";
		Add(gUIDrawTexture);
		GUIControlWindow gUIControlWindow = new GUIControlWindow();
		gUIControlWindow.SetPositionAndSize(200f, 200f, 600f, 600f);
		gUIControlWindow.SetBackground(Color.red);
		Add(gUIControlWindow);
		movieTest = new GUIMovieTexture();
		movieTest.SetPositionAndSize(10f, 20f, 240f, 200f);
		gUIControlWindow.Add(movieTest);
		streamTest = new GUIMovieTexture();
		streamTest.SetPositionAndSize(300f, 20f, 240f, 200f);
		gUIControlWindow.Add(streamTest);
		GUIButton gUIButton = new GUIButton();
		gUIButton.SetPositionAndSize(400f, 240f, 70f, 40f);
		gUIButton.Text = "PLAY";
		gUIButton.Click += play_Click;
		Add(gUIButton);
		GUIButton gUIButton2 = new GUIButton();
		gUIButton2.SetPositionAndSize(480f, 240f, 70f, 40f);
		gUIButton2.Text = "STOP";
		gUIButton2.Click += stop_Click;
		Add(gUIButton2);
		movieTest.AutoPlay = true;
		movieTest.TextureSource = "tutorial_bundle|howtojumpMovie";
		streamTest.MovieUri = "http://www.unity3d.com/webplayers/Movie/sample.ogg";
		streamTest.AutoPlay = true;
		return base.InitializeResources(reload);
	}

	private void stop_Click(GUIControl sender, GUIClickEvent EventData)
	{
		movieTest.Stop();
		streamTest.Stop();
	}

	private void play_Click(GUIControl sender, GUIClickEvent EventData)
	{
		movieTest.Play();
		streamTest.Play();
	}

	protected override void InitializeBundleList()
	{
		base.InitializeBundleList();
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("persistent_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("tutorial_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("toolbox_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
	}

	private void spinButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
	}

	public void OnPrizeWheelWebResponse(ShsWebResponse response)
	{
		CspUtils.DebugLog(response.Body);
		PrizeWheelManager prizeWheelManager = new PrizeWheelManager();
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		prizeWheelManager.InitializeFromData(dataWarehouse);
		SocialSpaceController.PrizeWheelData = prizeWheelManager;
		AppShell.Instance.EventMgr.Fire(this, new PrizeWheelLoadedMessage(prizeWheelManager));
	}

	private void alphaButton_MouseUp(GUIControl sender, GUIMouseEvent EventData)
	{
		CspUtils.DebugLog("ALPHA UP");
	}

	private void alphaButton_MouseDown(GUIControl sender, GUIMouseEvent EventData)
	{
		CspUtils.DebugLog("ALPHA DOWN");
	}

	private void alphaButton_MouseOut(GUIControl sender, GUIMouseEvent EventData)
	{
		CspUtils.DebugLog("ALPHA OUT");
	}

	private void alphaButton_MouseOver(GUIControl sender, GUIMouseEvent EventData)
	{
		CspUtils.DebugLog("ALPHA OVER");
	}

	private void maskTest_MouseOut(GUIControl sender, GUIMouseEvent EventData)
	{
	}

	private void maskTest_MouseOver(GUIControl sender, GUIMouseEvent EventData)
	{
	}

	private void sliderMask_Changed(GUIControl sender, GUIChangedEvent eventData)
	{
	}

	private void b_Click(GUIControl sender, GUIClickEvent EventData)
	{
		AppShell.Instance.Transition(GameController.ControllerType.Fallback);
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		if (Event.current.type == EventType.Repaint)
		{
			fps++;
		}
		if (Time.time - drawTime > 1f)
		{
			drawTime = Time.time;
			fps = 0;
		}
		base.Draw(drawFlags);
	}

	public override void OnUpdate()
	{
	}

	private void OnTempPrizeDataLoaded(GameDataLoadResponse response, object extraData)
	{
		PrizeWheelManager prizeWheelManager = new PrizeWheelManager();
		DataWarehouse data = response.Data;
		prizeWheelManager.InitializeFromData(data);
		SocialSpaceController.PrizeWheelData = prizeWheelManager;
		AppShell.Instance.EventMgr.Fire(this, new PrizeWheelLoadedMessage(prizeWheelManager));
	}

	public override void OnShow()
	{
	}

	public override void OnSceneEnter(AppShell.GameControllerTypeData currentGameData)
	{
	}

	public override void OnSceneLoaded(AppShell.GameControllerTypeData currentGameData)
	{
	}

	private void configureMask(float scale)
	{
	}

	public override void OnSceneLeave(AppShell.GameControllerTypeData lastGameData, AppShell.GameControllerTypeData currentGameData)
	{
	}
}
