using UnityEngine;

public class TutorialVideoWindow : GUIDynamicWindow
{
	private string targetURL = string.Empty;

	private GUIImage bgImage;

	private GUIImage movieTexture;

	private GUIButton okButton;

	private GUIButton playButton;

	private WWW movieRequest;

	private WWW audioRequest;

	private GameObject audioSource;

	private ShsAudioSource audioComponent;

	private GUIStrokeTextLabel loadingLabel;

	private static TutorialVideoWindow _instance;

	private bool firstFrame = true;

	private bool playRequested = true;

	public static bool alreadyPlaying
	{
		get
		{
			return _instance != null;
		}
	}

	public TutorialVideoWindow(string url)
	{
		_instance = this;
		targetURL = url;
		movieRequest = new WWW(url);
		audioRequest = new WWW(url.Replace(".ogv", ".ogg"));
	}

	public override bool InitializeResources(bool reload)
	{
		CspUtils.DebugLog("tut InitializeResources");
		SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -30f), new Vector2(648f, 503f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		bgImage = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(648f, 463f), new Vector2(0f, 0f));
		bgImage.TextureSource = "persistent_bundle|tutorial_video_bg2";
		Add(bgImage);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-10f, -205f), new Vector2(240f, 25f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 30, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 43, 68), GUILabel.GenColor(0, 43, 68), new Vector2(3f, 4f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel.BackColorAlpha = 1f;
		gUIStrokeTextLabel.StrokeColorAlpha = 1f;
		gUIStrokeTextLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUIStrokeTextLabel.Text = "#TUTORIAL_VIDEO_TITLE";
		gUIStrokeTextLabel.IsVisible = true;
		Add(gUIStrokeTextLabel);
		loadingLabel = new GUIStrokeTextLabel();
		loadingLabel.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -40f), new Vector2(550f, 100f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		loadingLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 30, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 43, 68), GUILabel.GenColor(0, 43, 68), new Vector2(3f, 4f), TextAnchor.MiddleCenter);
		loadingLabel.BackColorAlpha = 1f;
		loadingLabel.StrokeColorAlpha = 1f;
		loadingLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		loadingLabel.Text = "#TUTORIAL_VIDEO_LOADING";
		loadingLabel.IsVisible = true;
		Add(loadingLabel);
		movieTexture = new GUIImage();
		movieTexture.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(70f, 102f), new Vector2(512f, 320f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		movieTexture.Texture = movieRequest.movie;
		movieTexture.IsVisible = false;
		movieTexture.Texture = movieRequest.movie;
		Add(movieTexture);
		playButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(0f, -300f));
		playButton.Click += delegate
		{
			playRequested = true;
		};
		playButton.HitTestSize = new Vector2(0.5f, 0.3f);
		playButton.HitTestType = HitTestTypeEnum.Circular;
		playButton.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|L_play_button_welcome_gadget");
		playButton.IsVisible = false;
		Add(playButton);
		okButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(0f, 216f));
		okButton.Click += delegate
		{
			Okay();
		};
		okButton.HitTestSize = new Vector2(0.5f, 0.5f);
		okButton.HitTestType = HitTestTypeEnum.Circular;
		okButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_mshs_button_ok");
		okButton.IsVisible = false;
		Add(okButton);
		audioSource = new GameObject("Tutorial Video Audio");
		audioSource.AddComponent<AudioSource>();
		audioComponent = audioSource.AddComponent<ShsAudioSource>();
		audioComponent.PresetBundle = AppShell.Instance.AudioManager.PresetBundleDefinitions.PresetBundles["HUD_UI"];
		audioComponent.AudioCategory = AudioCategoryEnum.SfxHud;
		audioComponent.Loop = false;
		return base.InitializeResources(reload);
	}

	public override void Update()
	{
		if (firstFrame)
		{
			firstFrame = false;
			okButton.IsVisible = true;
			playButton.IsVisible = true;
			CspUtils.DebugLog("scale complete");
			PlayVideo();
		}
		if (movieRequest.isDone && playRequested && playButton.IsVisible)
		{
			PlayVideo();
		}
		else if (!playButton.IsVisible && !(movieTexture.Texture as MovieTexture).isPlaying)
		{
			playButton.IsVisible = true;
			playButton.SetPosition(new Vector2(195f, 278f));
			playRequested = false;
			movieTexture.IsVisible = false;
		}
		base.Update();
	}

	public override void OnActive()
	{
		PlayVideo();
	}

	protected void Okay()
	{
		if (targetURL == AchievementManager.tutorialURL)
		{
			AppShell.Instance.EventReporter.ReportAchievementEvent(string.Empty, "view_tutorial", string.Empty, 1, string.Empty);
		}
		Object.Destroy(audioSource);
		Hide();
		SetInactive();
		movieTexture.Dispose();
		Dispose();
		_instance = null;
	}

	protected void PlayVideo()
	{
		CspUtils.DebugLog("Movie progress: " + movieRequest.progress + "  " + movieRequest.isDone + ". Audio progress: " + audioRequest.progress + " " + audioRequest.isDone);
		if (movieRequest.isDone && audioRequest.isDone)
		{
			if (!string.IsNullOrEmpty(movieRequest.error))
			{
				CspUtils.DebugLog("error loading movie: " + targetURL + " error was: " + movieRequest.error);
				movieRequest = new WWW(targetURL);
				return;
			}
			loadingLabel.IsVisible = false;
			playButton.IsVisible = false;
			movieTexture.IsVisible = true;
			movieTexture.Texture = movieRequest.movie;
			(movieTexture.Texture as MovieTexture).loop = false;
			(movieTexture.Texture as MovieTexture).Play();
			audioComponent.Clips = new ShsAudioBase.AudioClipReference[1];
			ShsAudioBase.AudioClipReference audioClipReference = new ShsAudioBase.AudioClipReference();
			audioClipReference.Clip = audioRequest.GetAudioClip(false);
			audioComponent.Clips[0] = audioClipReference;
			audioComponent.Play();
		}
	}

	protected override void dispose(bool disposing)
	{
		movieTexture = null;
		okButton = null;
		audioComponent = null;
		base.dispose(disposing);
	}
}
