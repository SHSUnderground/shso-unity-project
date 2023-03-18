public class GUIModalChangeMessage : ShsEventMessage
{
	public readonly IGUIControl ModalControl;

	public readonly bool On;

	public GUIModalChangeMessage(IGUIControl ModalControl, bool On)
	{
		this.ModalControl = ModalControl;
		this.On = On;
	}
}
