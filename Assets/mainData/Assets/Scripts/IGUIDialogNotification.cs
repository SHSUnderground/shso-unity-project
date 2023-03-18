public interface IGUIDialogNotification
{
	void DialogCreated(string Id, IGUIDialogWindow window);

	void DialogShown(string Id, GUIDialogWindow.DialogState state);

	void DialogCancelled(string Id, GUIDialogWindow.DialogState state);

	void DialogClosing(string Id, IGUIDialogWindow window);

	void DialogClosed(string Id, GUIDialogWindow.DialogState state);

	GUIDialogWindow.DialogUpdateState DialogGetUpdateStatus(string Id);
}
