using System.Runtime.CompilerServices;

public class PlayerInputControllerCreated : ShsEventMessage
{
	[CompilerGenerated]
	private PlayerInputController _003CController_003Ek__BackingField;

	public PlayerInputController Controller
	{
		[CompilerGenerated]
		get
		{
			return _003CController_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CController_003Ek__BackingField = value;
		}
	}

	public PlayerInputControllerCreated(PlayerInputController controller)
	{
		Controller = controller;
	}
}
