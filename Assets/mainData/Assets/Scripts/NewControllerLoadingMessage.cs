public class NewControllerLoadingMessage : ShsEventMessage
{
	public GameController controller;

	public NewControllerLoadingMessage(GameController controller)
	{
		this.controller = controller;
	}
}
