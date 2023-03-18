using UnityEngine;

public class SHSReportPlayerDialogWindow : GUIDialogWindow
{
	private SHSReportPlayerStepOneWindow stepOneWindow;

	private SHSReportPlayerStepTwoWindow stepTwoWindow;

	private SHSReportPlayerStepThreeWindow stepThreeWindow;

	private SHSReportPlayerStepThreeBadWordWindow stepThreeBWWindow;

	private SHSReportPlayerStepThreePersonalInfoWindow stepThreePIWindow;

	private SHSReportPlayerStepThreeRudeWindow stepThreeRUWindow;

	private SHSReportPlayerStepFourWindow stepFourWindow;

	public string reportedPlayerName;

	public string reportCause;

	public string reportSubCause;

	public string reportComments;

	public bool reportPlayerConfirmed;

	public SHSReportPlayerDialogWindow()
	{
		stepOneWindow = new SHSReportPlayerStepOneWindow();
		stepOneWindow.SetPosition(QuickSizingHint.Centered);
		stepOneWindow.SetSize(new Vector2(550f, 400f));
		stepOneWindow.Id = "StepOneWindow";
		Add(stepOneWindow);
		stepTwoWindow = new SHSReportPlayerStepTwoWindow();
		stepTwoWindow.SetPosition(QuickSizingHint.Centered);
		stepTwoWindow.SetSize(new Vector2(550f, 400f));
		stepTwoWindow.Id = "StepTwoWindow";
		Add(stepTwoWindow);
		stepThreeWindow = new SHSReportPlayerStepThreeWindow();
		stepThreeWindow.SetPosition(QuickSizingHint.Centered);
		stepThreeWindow.SetSize(new Vector2(550f, 500f));
		stepThreeWindow.Id = "StepThreeWindow";
		Add(stepThreeWindow);
		stepThreeBWWindow = new SHSReportPlayerStepThreeBadWordWindow();
		stepThreeBWWindow.SetPosition(QuickSizingHint.Centered);
		stepThreeBWWindow.SetSize(new Vector2(550f, 500f));
		stepThreeBWWindow.Id = "StepThreeBWWindow";
		Add(stepThreeBWWindow);
		stepThreePIWindow = new SHSReportPlayerStepThreePersonalInfoWindow();
		stepThreePIWindow.SetPosition(QuickSizingHint.Centered);
		stepThreePIWindow.SetSize(new Vector2(550f, 500f));
		stepThreePIWindow.Id = "StepThreePIWindow";
		Add(stepThreePIWindow);
		stepThreeRUWindow = new SHSReportPlayerStepThreeRudeWindow();
		stepThreeRUWindow.SetPosition(QuickSizingHint.Centered);
		stepThreeRUWindow.SetSize(new Vector2(550f, 500f));
		stepThreeRUWindow.Id = "StepThreeRUWindow";
		Add(stepThreeRUWindow);
		stepFourWindow = new SHSReportPlayerStepFourWindow();
		stepFourWindow.SetPosition(QuickSizingHint.Centered);
		stepFourWindow.SetSize(new Vector2(550f, 500f));
		stepFourWindow.Id = "StepFourWindow";
		Add(stepFourWindow);
	}

	public override void OnShow()
	{
		SetPosition(QuickSizingHint.Centered);
		SetSize(new Vector2(800f, 600f));
		base.OnShow();
		stepOneWindow.Show();
		stepTwoWindow.Hide();
		stepThreeWindow.Hide();
		stepFourWindow.Hide();
	}

	public void ReportStepCompleted(string nextStep)
	{
		foreach (IGUIContainer control in controlList)
		{
			control.Hide();
		}
		if (nextStep != string.Empty)
		{
			this[nextStep].Show();
		}
		else
		{
			Hide();
		}
	}
}
