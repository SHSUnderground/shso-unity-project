using UnityEngine;

public class SHSStagedDownloadListWindow : GUIControlWindow
{
	private readonly GUIImage completeCheckbox;

	private readonly GUILabel percentLabel;

	private readonly GUIImage stageIcon;

	private readonly GUILabel stageName;

	private SHSStagedDownloadWindow.StageState state;

	public SHSStagedDownloadListWindow(SHSStagedDownloadWindow.StageState stageState)
	{
		state = stageState;
		stageName = new GUILabel();
		stageName.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft);
		stageName.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, Color.black, TextAnchor.MiddleLeft);
		stageName.Offset = new Vector2(90f, 0f);
		stageName.SetSize(300f, 18f);
		stageName.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		string arg = AppShell.Instance.stringTable[stageState.stage];
		arg = string.Format("{0} .......................................................................................", arg);
		stageName.Text = arg.Substring(0, 50);
		Add(stageName);
		stageIcon = new GUIImage();
		stageIcon.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft);
		stageIcon.SetSize(100f, 100f);
		stageIcon.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		stageIcon.TextureSource = stageState.imagePath;
		Add(stageIcon);
		completeCheckbox = new GUIImage();
		completeCheckbox.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
		completeCheckbox.SetSize(new Vector2(20f, 18f));
		completeCheckbox.Offset = new Vector2(-10f, 0f);
		completeCheckbox.TextureSource = "common_bundle|download_checkmark";
		completeCheckbox.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		completeCheckbox.IsVisible = stageState.completed;
		if (stageState.completed)
		{
			Click += SHSStagedDownloadListWindow_Click;
		}
		Add(completeCheckbox);
		percentLabel = new GUILabel();
		percentLabel.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
		percentLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, Color.black, TextAnchor.MiddleLeft);
		percentLabel.SetSize(40f, 30f);
		percentLabel.Offset = new Vector2(-10f, 0f);
		percentLabel.TextAlignment = TextAnchor.MiddleRight;
		GUILabel gUILabel = percentLabel;
		string text = string.Format("{0:#%}", stageState.percent);
		percentLabel.Text = text;
		gUILabel.Text = text;
		percentLabel.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		percentLabel.IsVisible = !stageState.completed;
		Add(percentLabel);
		MouseOver += SHSStagedDownloadListWindow_MouseOver;
		MouseOut += SHSStagedDownloadListWindow_MouseOut;
		SetSize(new Vector2(460f, 40f));
	}

	private void SHSStagedDownloadListWindow_MouseOut(GUIControl sender, GUIMouseEvent EventData)
	{
		SetBackground(false);
	}

	private void SHSStagedDownloadListWindow_MouseOver(GUIControl sender, GUIMouseEvent EventData)
	{
		SHSStagedDownloadListWindow sHSStagedDownloadListWindow = sender as SHSStagedDownloadListWindow;
		if (sHSStagedDownloadListWindow != null && sHSStagedDownloadListWindow.state.completed)
		{
			SetBackground(ColorUtil.FromRGB255(161, 172, 46, 0.6f));
		}
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (state.completed)
		{
			return;
		}
		AssetBundleLoader bundleLoader = AppShell.Instance.BundleLoader;
		if (bundleLoader.GetBundleGroupDependenciesDone(state.assetBundleGroup))
		{
			state.completed = true;
			completeCheckbox.IsVisible = true;
			percentLabel.IsVisible = false;
			Click += SHSStagedDownloadListWindow_Click;
			return;
		}
		float num = AppShell.Instance.BundleLoader.GetBundleGroupPercentageDone(state.assetBundleGroup);
		if (num >= 0.99f)
		{
			num = 0.99f;
		}
		state.percent = num;
		if (state.percent == 0f)
		{
			percentLabel.Text = "-";
		}
		else
		{
			percentLabel.Text = string.Format("{0:0#%;#%;#%}", num);
		}
	}

	private void SHSStagedDownloadListWindow_Click(GUIControl sender, GUIClickEvent EventData)
	{
		if (state.completeDelegate != null)
		{
			state.completeDelegate(this);
		}
	}
}
