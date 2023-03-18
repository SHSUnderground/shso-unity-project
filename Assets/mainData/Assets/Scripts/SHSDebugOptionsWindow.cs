using UnityEngine;

public class SHSDebugOptionsWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private GUILayoutControlWindow mainWindow;

	public SHSDebugOptionsWindow(string PanelName, SHSDebugConsoleWindow console)
		: base(PanelName, null)
	{
		SetBackground(new Color(0.3f, 0.5f, 0.8f, 0.2f));
		mainWindow = new GUILayoutControlWindow();
		mainWindow.Id = "MainWindow";
		mainWindow.SetBackground(new Color(1f, 0.5f, 0.5f, 1f));
		mainWindow.Margin = new Rect(20f, 20f, 20f, 20f);
		Add(mainWindow);
		mainWindow.Add(new GUILayoutBeginSection(GUILayoutBeginSection.OrientationEnum.Vertical));
		GUIToggleButton gUIToggleButton = addCustomToggleButton("See Through", mainWindow);
		gUIToggleButton.Id = "FadeOption";
		gUIToggleButton.Margin = new Rect(5f, 5f, 5f, 5f);
		gUIToggleButton.Changed += toggleFadeButton_Click;
		gUIToggleButton.Tag = 1;
		gUIToggleButton.Value = (PlayerPrefs.GetInt("FadeOption", 1) == 1);
		mainWindow.Add(new GUILayoutEndSection(GUILayoutEndSection.OrientationEnum.Vertical));
		mainWindow.Add(new GUILayoutBeginSection(GUILayoutBeginSection.OrientationEnum.Vertical));
		mainWindow.Add(new GUILayoutSpace(30));
		mainWindow.Add(new GUILayoutBeginSection(GUILayoutBeginSection.OrientationEnum.Horizontal, 300));
		GUILabel gUILabel = new GUILabel();
		gUILabel.Size = new Vector2(200f, 20f);
		gUILabel.Text = "Startup Scene";
		gUILabel.Rotation = 0f;
		gUILabel.Color = new Color(1f, 1f, 1f, 1f);
		gUILabel.IsVisible = true;
		gUILabel.TooltipKey = string.Empty;
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		gUILabel.Margin = new Rect(5f, 5f, 5f, 5f);
		mainWindow.Add(gUILabel);
		mainWindow.Add(new GUILayoutEndSection(GUILayoutEndSection.OrientationEnum.Horizontal));
		GUIToggleButton StartSocial = addCustomToggleButton("Game World", mainWindow);
		GUIToggleButton StartFallback = addCustomToggleButton("Fallback", mainWindow);
		StartSocial.Changed += delegate
		{
			PlayerPrefs.SetInt("startupscene", 8);
			StartFallback.Value = false;
		};
		StartFallback.Changed += delegate
		{
			PlayerPrefs.SetInt("startupscene", 16);
			StartSocial.Value = false;
		};
		switch (PlayerPrefs.GetInt("startupscene", 8))
		{
		case 8:
			StartSocial.Value = true;
			break;
		case 16:
			StartFallback.Value = true;
			break;
		}
		mainWindow.Add(new GUILayoutEndSection(GUILayoutEndSection.OrientationEnum.Vertical));
	}

	private GUIToggleButton addCustomToggleButton(string name, GUIWindow theWindowToAddItToo)
	{
		GUIToggleButton gUIToggleButton = new GUIToggleButton();
		gUIToggleButton.Spacing = 35f;
		gUIToggleButton.Text = name;
		gUIToggleButton.SetButtonSize(new Vector2(25f, 25f));
		gUIToggleButton.SetSize(240f, 25f);
		gUIToggleButton.Value = false;
		gUIToggleButton.Margin = new Rect(5f, 5f, 5f, 5f);
		theWindowToAddItToo.Add(gUIToggleButton);
		return gUIToggleButton;
	}

	private void toggleFadeButton_Click(GUIControl sender, GUIChangedEvent EventData)
	{
		GUIToggleButton gUIToggleButton = (GUIToggleButton)sender;
		PlayerPrefs.SetInt("FadeOption", gUIToggleButton.Value ? 1 : 0);
		modifyFade();
	}

	private void modifyFade()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
		if (gameObject != null)
		{
			SeeThrough seeThrough = gameObject.GetComponentInChildren(typeof(SeeThrough)) as SeeThrough;
			if (seeThrough != null)
			{
				seeThrough.enabled = (PlayerPrefs.GetInt("FadeOption", 1) == 1);
			}
		}
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		if (mainWindow != null)
		{
			mainWindow.SetPositionAndSize(new Vector2(10f, 10f), DockingAlignmentEnum.None, AnchorAlignmentEnum.None, OffsetType.Absolute, Vector2.zero, new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		}
	}
}
