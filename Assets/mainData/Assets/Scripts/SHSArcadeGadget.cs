using UnityEngine;

public class SHSArcadeGadget : SHSGadget
{
	private class ArcadeTopWindow : GadgetTopWindow
	{
		public ArcadeTopWindow()
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(200f, 43f), new Vector2(0f, 0f));
			gUIImage.TextureSource = "persistent_bundle|L_arcade_launcher_title";
			Add(gUIImage);
		}
	}

	private readonly SHSArcadeMainWindow mainWindow;

	public SHSArcadeGadget()
	{
		mainWindow = new SHSArcadeMainWindow(this);
		SetBackgroundSize(new Vector2(1020f, 644f));
		SetupOpeningWindow(BackgroundType.OnePanel, mainWindow);
		SetupOpeningTopWindow(new ArcadeTopWindow());
		SetBackgroundImage("persistent_bundle|arcade_launcher_frame");
		CloseButton.Offset = new Vector2(457f, -556f);
	}

	public override void OnShow()
	{
		base.OnShow();
		PlayerStatus.SetLocalStatus(PlayerStatusDefinition.Instance.GetStatus("Arcade"));
		AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "open_arcade", 1, -10000, -10000, string.Empty, string.Empty);
	}

	public override void CloseGadget()
	{
		if (PlayerStatus.GetLocalStatus() == PlayerStatusDefinition.Instance.GetStatus("Arcade"))
			{
				PlayerStatus.ClearLocalStatus();
			}
		base.CloseGadget();
	}
}
