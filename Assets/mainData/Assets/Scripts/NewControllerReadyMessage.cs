public class NewControllerReadyMessage : ShsEventMessage
{
	public GameController controller;

	public NewControllerReadyMessage(GameController controller)
	{
		this.controller = controller;
	}
}
