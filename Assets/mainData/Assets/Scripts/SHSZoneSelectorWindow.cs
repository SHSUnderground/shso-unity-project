using UnityEngine;

public class SHSZoneSelectorWindow : SHSGadget.GadgetCenterWindow
{
	public SHSZoneSelectorSliderWindow sliderWindow;

	public GUIButton goButton;

	private MouseClickDelegate goButtonClickDelegate;

	public SHSZoneSelectorWindow()
	{
		sliderWindow = new SHSZoneSelectorSliderWindow();
		sliderWindow.SelectedItemChanged += sliderWindow_SelectedItemChanged;
		Add(sliderWindow);
		SetControlFlag(ControlFlagSetting.AlphaCascade, false, true);
		goButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(512f, 512f), new Vector2(0f, 240f));
		goButton.HitTestType = HitTestTypeEnum.Alpha;
		goButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|L_gadget_gobutton");
		Add(goButton);
		SetGoButtonClickEvent(delegate
		{
			AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "switch_zone", OwnableDefinition.simpleZoneName(sliderWindow.SelectedItem.Zone.launchKey), 3f);
			AppShell.Instance.SharedHashTable["SocialSpaceLevel"] = sliderWindow.SelectedItem.Zone.launchKey;
			AppShell.Instance.Transition(GameController.ControllerType.SocialSpace);
		});
	}

	public void SetGoButtonClickEvent(MouseClickDelegate del)
	{
		if (goButtonClickDelegate != null)
		{
			goButton.Click -= goButtonClickDelegate;
		}
		goButtonClickDelegate = del;
		goButton.Click += del;
	}

	private void sliderWindow_SelectedItemChanged()
	{
		IContentDependency clickHotSpotButton = sliderWindow.SelectedItem.ClickHotSpotButton;
		goButton.IsEnabled = (!clickHotSpotButton.IsContentDependent || clickHotSpotButton.IsContentLoaded);
	}
}
