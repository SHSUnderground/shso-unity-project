using System;
using UnityEngine;

public class SHSDebugWindow : GUITopLevelWindow
{
	private class FPSData
	{
		public float updateInternal = 0.5f;

		public float accumulationBucket;

		public int frames;

		public float timeLeft = -100f;

		public float lastIntervalStart;

		public int frames2;

		public float lowFps2;

		public float globalMinFps;

		public float lastFrameStart;
	}

	private GUIDropShadowTextLabel FrameRateLabel;

	private GUIDropShadowTextLabel FrameRateLabel2;

	private bool isFrameRateLogging;

	private SHSDebugConsoleWindow console;

	private GUIDropShadowTextLabel PlayerPositionLabel;

	private GUIDropShadowTextLabel PlayerRotationLabel;

	private bool firstResize = true;

	public bool IsFrameRateLogging
	{
		get
		{
			return isFrameRateLogging;
		}
		set
		{
			isFrameRateLogging = value;
		}
	}

	public SHSDebugConsoleWindow Console
	{
		get
		{
			return console;
		}
	}

	public override SHSInput.InputRequestorType InputRequestorType
	{
		get
		{
			return SHSInput.InputRequestorType.Debug;
		}
	}

	public SHSDebugWindow()
		: base("SHSDebugWindow")
	{
		console = new SHSDebugConsoleWindow();
		console.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		console.SetPositionAndSize(QuickSizingHint.ParentSize);
		if (ShsPlayerPrefs.HasKey(ShsPlayerPrefs.Keys.DebugWindowSizeX))
		{
			console.SetSize(new Vector2(ShsPlayerPrefs.GetFloat(ShsPlayerPrefs.Keys.DebugWindowSizeX), ShsPlayerPrefs.GetFloat(ShsPlayerPrefs.Keys.DebugWindowSizeY)));
		}
		console.LogWindow.LogEntryStyle = GUIManager.Instance.StyleManager.GetStyle("DebugConsoleLogEntry");
		Add(console);
		Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Ignore;
		AppShell.Instance.EventMgr.AddListener<DebugWindowMessage>(OnDebugWindowToggle);
		GUIDropShadowTextLabel gUIDropShadowTextLabel = new GUIDropShadowTextLabel(new Color(1f, 1f, 1f, 1f), new Color(0f, 0f, 0f, 1f));
		gUIDropShadowTextLabel.Id = "ServerName";
		gUIDropShadowTextLabel.SetPositionAndSize(0f, 5f, 100f, 10f);
		gUIDropShadowTextLabel.Text = AppShell.Instance.NetworkEnvironment;
		gUIDropShadowTextLabel.StyleInfo = new SHSNamedStyleInfo("LoginText");
		gUIDropShadowTextLabel.IsVisible = (PlayerPrefs.GetInt("serverNameDisplay") == 1);
		gUIDropShadowTextLabel.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		Add(gUIDropShadowTextLabel);
		IsVisible = true;
		FrameRateLabel = new GUIDropShadowTextLabel(new Color(1f, 1f, 1f, 1f), new Color(0f, 0f, 0f, 1f));
		FrameRateLabel.Id = "FrameRateLabel";
		FrameRateLabel.SetPositionAndSize(0f, 15f, 100f, 10f);
		FrameRateLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, Color.white, TextAnchor.UpperLeft);
		FrameRateLabel.BackColor = Color.black;
		FrameRateLabel.FrontColor = Color.white;
		FrameRateLabel.IsVisible = (PlayerPrefs.GetInt("frameRateLabelDisplay") == 1);
		FrameRateLabel.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		FrameRateLabel.Tag = new FPSData();
		FrameRateLabel.Overflow = true;
		Add(FrameRateLabel);
		FrameRateLabel2 = new GUIDropShadowTextLabel(new Color(1f, 1f, 1f, 1f), new Color(0f, 0f, 0f, 1f));
		FrameRateLabel2.Id = "FrameRateLabel2";
		FrameRateLabel2.SetPositionAndSize(0f, 30f, 100f, 10f);
		FrameRateLabel2.Text = "Frame RateLabel:";
		FrameRateLabel2.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, Color.white, TextAnchor.UpperLeft);
		FrameRateLabel2.BackColor = Color.black;
		FrameRateLabel2.FrontColor = Color.white;
		FrameRateLabel2.IsVisible = (PlayerPrefs.GetInt("frameRateLabelDisplay") == 1);
		FrameRateLabel2.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		FrameRateLabel2.Overflow = true;
		Add(FrameRateLabel2);
		PlayerPositionLabel = new GUIDropShadowTextLabel(new Color(1f, 1f, 1f, 1f), new Color(0f, 0f, 0f, 1f));
		PlayerPositionLabel.Id = "PlayerPositionLabel";
		PlayerPositionLabel.SetPositionAndSize(10f, 195f, 200f, 10f);
		PlayerPositionLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, Color.white, TextAnchor.UpperLeft);
		PlayerPositionLabel.BackColor = Color.black;
		PlayerPositionLabel.FrontColor = Color.white;
		PlayerPositionLabel.IsVisible = (PlayerPrefs.GetInt("playerPosAndRotLabelDisplay") == 1);
		PlayerPositionLabel.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		PlayerPositionLabel.Overflow = true;
		Add(PlayerPositionLabel);
		PlayerRotationLabel = new GUIDropShadowTextLabel(new Color(1f, 1f, 1f, 1f), new Color(0f, 0f, 0f, 1f));
		PlayerRotationLabel.Id = "PlayerRotationLabel";
		PlayerRotationLabel.SetPositionAndSize(10f, 210f, 200f, 10f);
		PlayerRotationLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, Color.white, TextAnchor.UpperLeft);
		PlayerRotationLabel.BackColor = Color.black;
		PlayerRotationLabel.FrontColor = Color.white;
		PlayerRotationLabel.IsVisible = (PlayerPrefs.GetInt("playerPosAndRotLabelDisplay") == 1);
		PlayerRotationLabel.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		PlayerRotationLabel.Overflow = true;
		Add(PlayerRotationLabel);
		isFrameRateLogging = false;
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		((GUIDropShadowTextLabel)this["ServerName"]).Text = AppShell.Instance.NetworkEnvironment;
		if (FrameRateLabel.IsVisible || IsFrameRateLogging)
		{
			FPSData fPSData = FrameRateLabel.Tag as FPSData;
			if (fPSData != null)
			{
				if (fPSData.timeLeft == -100f)
				{
					fPSData.timeLeft = fPSData.updateInternal;
					fPSData.lastIntervalStart = Time.realtimeSinceStartup;
					fPSData.frames2 = 0;
					fPSData.lowFps2 = 0f;
					fPSData.globalMinFps = 0f;
					fPSData.lastFrameStart = fPSData.lastIntervalStart;
				}
				else
				{
					fPSData.timeLeft -= Time.deltaTime;
					fPSData.accumulationBucket += Time.timeScale / Time.deltaTime;
					fPSData.frames++;
					if (fPSData.timeLeft <= 0f)
					{
						fPSData.timeLeft = fPSData.updateInternal;
						fPSData.accumulationBucket = 0f;
						fPSData.frames = 0;
					}
					fPSData.frames2++;
					float realtimeSinceStartup = Time.realtimeSinceStartup;
					fPSData.lowFps2 = Math.Max(fPSData.lowFps2, realtimeSinceStartup - fPSData.lastFrameStart);
					fPSData.globalMinFps = Math.Max(fPSData.globalMinFps, realtimeSinceStartup - fPSData.lastFrameStart);
					fPSData.lastFrameStart = realtimeSinceStartup;
					if (fPSData.updateInternal < realtimeSinceStartup - fPSData.lastIntervalStart)
					{
						string text = "Frame Rate: " + ((float)fPSData.frames2 / (realtimeSinceStartup - fPSData.lastIntervalStart)).ToString("f2") + " RT FPS (IntMin: " + (1f / fPSData.lowFps2).ToString("f1") + ", GlobMin: " + (1f / fPSData.globalMinFps).ToString("f1") + ")";
						if (FrameRateLabel.IsVisible)
						{
							FrameRateLabel2.Text = text;
						}
						if (IsFrameRateLogging)
						{
							CspUtils.DebugLog(text);
						}
						fPSData.frames2 = 0;
						fPSData.lowFps2 = 0f;
						fPSData.lastIntervalStart = realtimeSinceStartup;
					}
				}
			}
		}
		if (PlayerPositionLabel.IsVisible)
		{
			GameObject localPlayer = GameController.GetController().LocalPlayer;
			if (localPlayer != null)
			{
				PlayerPositionLabel.Text = "Position: " + localPlayer.transform.position.ToString();
			}
			else
			{
				PlayerPositionLabel.Text = "No local player";
			}
		}
		if (PlayerRotationLabel.IsVisible)
		{
			GameObject localPlayer2 = GameController.GetController().LocalPlayer;
			if (localPlayer2 != null)
			{
				PlayerRotationLabel.Text = "Rotation: " + localPlayer2.transform.rotation.ToString();
			}
			else
			{
				PlayerPositionLabel.Text = "No local player";
			}
		}
	}

	private void OnDebugWindowToggle(DebugWindowMessage message)
	{
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ClientConsoleAllow))
		{
			showDebugWindowConsole(message);
		}
	}

	private void showDebugWindowConsole(DebugWindowMessage message)
	{
		SHSDebugConsoleWindow sHSDebugConsoleWindow = GetControl("SHSDebugConsoleWindow") as SHSDebugConsoleWindow;
		if (sHSDebugConsoleWindow.IsVisible)
		{
			sHSDebugConsoleWindow.Hide();
			return;
		}
		if (ShsPlayerPrefs.HasKey(ShsPlayerPrefs.Keys.DebugWindowSizeX))
		{
			Vector2 vector = new Vector2(ShsPlayerPrefs.GetFloat(ShsPlayerPrefs.Keys.DebugWindowSizeX), ShsPlayerPrefs.GetFloat(ShsPlayerPrefs.Keys.DebugWindowSizeY));
			if (vector.magnitude < 32f)
			{
				vector = new Vector2(Screen.width, (float)Screen.height / 2f);
			}
			sHSDebugConsoleWindow.SetPositionAndSize(0f, 0f, vector.x, vector.y);
		}
		else if (message.SizeType == DebugWindowMessage.SizeTypeEnum.HalfScreen)
		{
			sHSDebugConsoleWindow.SetPositionAndSize(0f, 0f, Screen.width, (float)Screen.height / 2f);
		}
		sHSDebugConsoleWindow.Show();
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		if (firstResize)
		{
			firstResize = !firstResize;
		}
	}

	public void OnGlobalMinFpsReset()
	{
		FPSData fPSData = FrameRateLabel.Tag as FPSData;
		fPSData.globalMinFps = 0f;
	}
}
