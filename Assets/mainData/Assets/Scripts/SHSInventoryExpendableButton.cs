public class SHSInventoryExpendableButton : GUIButton
{
	private string _expendableId;

	private string _expendableName;

	private bool _expendableConfirmUse;

	public SHSInventoryExpendableButton(Expendable expandable)
	{
		_expendableId = expandable.Definition.OwnableTypeId;
		_expendableName = expandable.Definition.Name;
		_expendableConfirmUse = expandable.Definition.ConfirmUse;
		Click += InventoryExpendableButton_Click;
	}

	private void StartToExpend()
	{
		AppShell.Instance.ExpendablesManager.UseExpendable(_expendableId);
	}

	private void InventoryExpendableButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		if (_expendableConfirmUse)
		{
			GUIManager.Instance.ShowDynamicWindow(new SHSExpendableUseConfirmationWindow(_expendableName, StartToExpend), ModalLevelEnum.Full);
		}
		else
		{
			StartToExpend();
		}
	}
}
