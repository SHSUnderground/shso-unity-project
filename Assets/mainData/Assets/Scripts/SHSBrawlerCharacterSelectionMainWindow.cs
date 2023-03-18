public class SHSBrawlerCharacterSelectionMainWindow : GUITopLevelWindow
{
	private SHSBrawlerAirlockHostWindow charSelectWindow;

	public SHSBrawlerCharacterSelectionMainWindow()
		: base("SHSBrawlerCharacterSelectionMainWindow")
	{
		charSelectWindow = new SHSBrawlerAirlockHostWindow();
		charSelectWindow.SetPositionAndSize(0f, 0f, 0f, 0f);
		Add(charSelectWindow, DrawOrder.DrawFirst, DrawPhaseHintEnum.PostDraw);
	}
}
