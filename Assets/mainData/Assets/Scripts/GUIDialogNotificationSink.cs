public class GUIDialogNotificationSink : IGUIDialogNotification
{
	public delegate void DialogEventNotificationDelegate(string Id, GUIDialogWindow.DialogState state);

	public delegate void DialogCreatedNotificationDelegate(string Id, IGUIDialogWindow window);

	public delegate void DialogClosingNotificationDelegate(string Id, IGUIDialogWindow window);

	public delegate GUIDialogWindow.DialogUpdateState DialogUpdateNotificationDelegate(string Id);

	private DialogCreatedNotificationDelegate createdDelegate;

	private DialogEventNotificationDelegate shownDelegate;

	private DialogClosingNotificationDelegate closingDelegate;

	private DialogEventNotificationDelegate cancelDelegate;

	private DialogEventNotificationDelegate closedDelegate;

	private DialogUpdateNotificationDelegate updateDelegate;

	private GUIDialogNotificationSink()
	{
	}

	public GUIDialogNotificationSink(DialogCreatedNotificationDelegate CreatedDelegate, DialogEventNotificationDelegate ShownDelegate, DialogClosingNotificationDelegate ClosingDelegate, DialogEventNotificationDelegate CancelDelegate, DialogEventNotificationDelegate ClosedDelegate, DialogUpdateNotificationDelegate UpdateDelegate)
	{
		createdDelegate = CreatedDelegate;
		shownDelegate = ShownDelegate;
		closingDelegate = ClosingDelegate;
		cancelDelegate = CancelDelegate;
		closedDelegate = ClosedDelegate;
		updateDelegate = UpdateDelegate;
	}

	public GUIDialogNotificationSink(DialogCreatedNotificationDelegate CreatedDelegate, DialogEventNotificationDelegate ShownDelegate, DialogClosingNotificationDelegate ClosingDelegate, DialogEventNotificationDelegate CancelDelegate, DialogEventNotificationDelegate ClosedDelegate)
		: this(CreatedDelegate, ShownDelegate, ClosingDelegate, CancelDelegate, ClosedDelegate, null)
	{
	}

	public GUIDialogNotificationSink(DialogEventNotificationDelegate ClosedDelegate)
	{
		createdDelegate = null;
		shownDelegate = null;
		closingDelegate = null;
		cancelDelegate = null;
		closedDelegate = ClosedDelegate;
		updateDelegate = null;
	}

	public void DialogCreated(string Id, IGUIDialogWindow window)
	{
		if (createdDelegate != null)
		{
			createdDelegate(Id, window);
		}
	}

	public void DialogShown(string Id, GUIDialogWindow.DialogState state)
	{
		if (shownDelegate != null)
		{
			shownDelegate(Id, state);
		}
	}

	public void DialogClosing(string Id, IGUIDialogWindow window)
	{
		if (closingDelegate != null)
		{
			closingDelegate(Id, window);
		}
	}

	public void DialogCancelled(string Id, GUIDialogWindow.DialogState state)
	{
		if (cancelDelegate != null)
		{
			cancelDelegate(Id, state);
		}
	}

	public void DialogClosed(string Id, GUIDialogWindow.DialogState state)
	{
		if (closedDelegate != null)
		{
			closedDelegate(Id, state);
		}
	}

	public GUIDialogWindow.DialogUpdateState DialogGetUpdateStatus(string Id)
	{
		if (updateDelegate != null)
		{
			return updateDelegate(Id);
		}
		return GUIDialogWindow.DialogUpdateState.Inactive;
	}
}
