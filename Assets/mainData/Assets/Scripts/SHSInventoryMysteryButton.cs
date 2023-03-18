public class SHSInventoryMysteryButton : GUIButton
{
	private string _boxId;

	private string _boxName;

	public SHSInventoryMysteryButton(MysteryBox box)
	{
		_boxId = string.Empty + box.Definition.ownableTypeID;
		_boxName = box.Definition.name;
		Click += InventoryMysteryButton_Click;
	}

	private void StartToExpend()
	{
		AppShell.Instance.ExpendablesManager.UseExpendable(_boxId);
	}

	private void InventoryMysteryButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		StartToExpend();
	}
}
