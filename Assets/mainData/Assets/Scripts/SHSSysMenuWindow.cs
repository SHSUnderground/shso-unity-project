using UnityEngine;

public class SHSSysMenuWindow : GUIDialogWindow
{
	private GUIButton optionsButton;

	private GUIButton resumeButton;

	private GUIButton quitButton;

	public SHSSysMenuWindow()
	{
		optionsButton = new GUIButton();
		optionsButton.Rect = new Rect(10f, 10f, 110f, 20f);
		optionsButton.Text = "#options";
		optionsButton.Rotation = 0f;
		optionsButton.Color = new Color(1f, 1f, 1f, 1f);
		optionsButton.IsVisible = true;
		optionsButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|stdbutton");
		optionsButton.EntitlementFlag = Entitlements.EntitlementFlagEnum.DemoLimitsOn;
		optionsButton.Click += delegate
		{
			GUIManager.Instance.Root["SHSMainWindow/SHSSystemMainWindow/SHSSysOptionsWindow"].Show(ModalLevelEnum.Default);
		};
		Add(optionsButton);
		resumeButton = new GUIButton();
		resumeButton.Rect = new Rect(10f, 35f, 110f, 20f);
		resumeButton.Text = "#resume";
		resumeButton.Rotation = 0f;
		resumeButton.Color = new Color(1f, 1f, 1f, 1f);
		resumeButton.IsVisible = true;
		resumeButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|stdbutton");
		resumeButton.Click += delegate
		{
			Hide();
		};
		Add(resumeButton);
		quitButton = new GUIButton();
		quitButton.Rect = new Rect(10f, 60f, 110f, 20f);
		quitButton.Text = "#quit";
		quitButton.Rotation = 0f;
		quitButton.Color = new Color(1f, 1f, 1f, 1f);
		quitButton.IsVisible = true;
		quitButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|stdbutton");
		quitButton.EntitlementFlag = Entitlements.EntitlementFlagEnum.DemoLimitsOn;
		quitButton.Click += delegate
		{
			AppShell.Instance.PromptQuit();
		};
		Add(quitButton);
		SetPosition(QuickSizingHint.Centered);
		SetSize(300f, 190f);
		Traits.LifeSpan = ControlTraits.LifeSpanTraitEnum.KeepAlive;
	}

	public override void DrawPreprocess()
	{
		base.DrawPreprocess();
		GUI.color = new Color(1f, 1f, 1f, 0.5f);
		GUI.Box(new Rect(0f, 0f, base.rect.width, base.rect.height), string.Empty);
		GUI.color = new Color(1f, 1f, 1f, 1f);
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
	}

	public override void DrawFinalize()
	{
		base.DrawFinalize();
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		optionsButton.Rect = new Rect(10f, 10f, base.rect.width - 10f, 50f);
		resumeButton.Rect = new Rect(10f, 70f, base.rect.width - 10f, 50f);
		quitButton.Rect = new Rect(10f, 130f, base.rect.width - 10f, 50f);
	}

	protected override void OnEscape(SHSKeyCode code)
	{
		Hide();
	}
}
