using UnityEngine;

public class SHSZoneSelectorGadget : SHSGadget
{
	private class TopWindow : GadgetTopWindow
	{
		public TopWindow()
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(592f, 141f), new Vector2(0f, 10f));
			gUIImage.TextureSource = "persistent_bundle|gadget_topmodule";
			Add(gUIImage);
			GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(203f, 42f), Vector2.zero);
			gUIImage2.TextureSource = "zonechooser_bundle|L_title_select_zone";
			Add(gUIImage2);
		}
	}

	private TopWindow topWindow;

	public SHSZoneSelectorWindow ZoneSelectorWindow;

	public SHSZoneSelectorGadget()
	{
		topWindow = new TopWindow();
		ZoneSelectorWindow = new SHSZoneSelectorWindow();
		SetupOpeningTopWindow(topWindow);
		SetupOpeningWindow(BackgroundType.OnePanel, ZoneSelectorWindow);
		SetBackgroundImage("persistent_bundle|gadget_mainwindow_frame");
		PlayerStatus.SetLocalStatus(PlayerStatusDefinition.Instance.GetStatus("ZoneChooser"));
	}

	public override void OnHide()
		{
			if (PlayerStatus.GetLocalStatus() == PlayerStatusDefinition.Instance.GetStatus("ZoneChooser"))
			{
				PlayerStatus.ClearLocalStatus();
			}
			base.OnHide();
		}
}
