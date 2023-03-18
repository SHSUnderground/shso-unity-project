using UnityEngine;

public class ScenarioEventTutorial : ScenarioEventHandlerEnableBase
{
	public enum BrawlerTutorialState
	{
		Lesson1,
		Lesson2,
		Lesson3,
		Lesson4,
		Complete
	}

	public BrawlerTutorialState GoToState;

	protected void CreateTutorialWindow(string lessonKey, string movieTexture)
	{
		string goal = "#m_0015_1_Tutorial_Brawler_" + lessonKey + "_goal";
		string description = "#m_0015_1_Tutorial_Brawler_" + lessonKey + "_description";
		SHSBrawlerRailsIntroGadget sHSBrawlerRailsIntroGadget = new SHSBrawlerRailsIntroGadget();
		GUIManager.Instance.ShowDynamicWindow(sHSBrawlerRailsIntroGadget, GUIControl.ModalLevelEnum.Default);
		SHSBrawlerRailsLessonWindow sHSBrawlerRailsLessonWindow = new SHSBrawlerRailsLessonWindow();
		sHSBrawlerRailsIntroGadget.SetupOpeningWindow(SHSGadget.BackgroundType.OnePanel, sHSBrawlerRailsLessonWindow);
		sHSBrawlerRailsLessonWindow.SetupLesson(goal, description, movieTexture, lessonKey);
	}

	protected override void OnEnableEvent(string eventName)
	{
		switch (GoToState)
		{
		case BrawlerTutorialState.Complete:
			break;
		case BrawlerTutorialState.Lesson1:
			CreateTutorialWindow("lesson1", "tutorial_bundle|howtojumpMovie");
			break;
		case BrawlerTutorialState.Lesson2:
			CreateTutorialWindow("lesson2", "tutorial_bundle|howtojumpMovie");
			break;
		case BrawlerTutorialState.Lesson3:
			CreateTutorialWindow("lesson3", "tutorial_bundle|howtojumpMovie");
			break;
		case BrawlerTutorialState.Lesson4:
			CreateTutorialWindow("lesson4", "tutorial_bundle|howtojumpMovie");
			break;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "NewObjectiveIcon.png");
	}
}
