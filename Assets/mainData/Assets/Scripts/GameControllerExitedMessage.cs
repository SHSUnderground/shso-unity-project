public class GameControllerExitedMessage : ShsEventMessage
{
	public GameController controller;

	public GameControllerExitedMessage(GameController controller)
	{
		this.controller = controller;
	}
}
