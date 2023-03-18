public class DropDownItemSelectedMessage : ShsEventMessage
{
	public readonly string ItemName;

	public readonly GUIDropDownListBox Sender;

	public DropDownItemSelectedMessage(GUIDropDownListBox Sender, string ItemName)
	{
		this.ItemName = ItemName;
		this.Sender = Sender;
	}
}
