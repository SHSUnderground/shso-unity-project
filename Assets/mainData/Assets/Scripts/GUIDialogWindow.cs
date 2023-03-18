using UnityEngine;

public class GUIDialogWindow : GUIDynamicWindow, IGUIDialogWindow
{
	public enum DialogState
	{
		Created,
		Shown,
		Ok,
		Cancel,
		Loading,
		InProgress
	}

	public enum DialogUpdateState
	{
		Inactive,
		InProgress,
		Cancelled,
		Complete
	}

	private DialogState currentState;

	private string text;

	private string titleText;

	private GameObject linkedObject;

	private bool isLinked;

	private bool initialized;

	private IGUIDialogNotification notificationSink;

	public DialogState CurrentState
	{
		get
		{
			return currentState;
		}
	}

	public virtual string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
		}
	}

	public virtual string TitleText
	{
		get
		{
			return titleText;
		}
		set
		{
			titleText = value;
		}
	}

	public GameObject LinkedObject
	{
		get
		{
			return linkedObject;
		}
		set
		{
			linkedObject = value;
			isLinked = (value != null);
		}
	}

	public bool IsLinked
	{
		get
		{
			return isLinked;
		}
	}

	public IGUIDialogNotification NotificationSink
	{
		get
		{
			return notificationSink;
		}
		set
		{
			if (notificationSink != null)
			{
				CspUtils.DebugLog("Cant modify notification sink once its been assigned to this instance.");
				return;
			}
			notificationSink = value;
			if (notificationSink != null)
			{
				init();
			}
		}
	}

	public GUIDialogWindow()
	{
		currentState = DialogState.Created;
	}

	protected void init()
	{
		if (!initialized)
		{
			if (notificationSink != null)
			{
				notificationSink.DialogCreated(Id, this);
			}
			initialized = true;
		}
	}

	public override void ConfigureKeyBanks()
	{
		base.ConfigureKeyBanks();
		keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.Escape, false, false, false), OnEscape);
		keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.Return, false, false, false), OnEnter);
	}

	public override void OnShow()
	{
		base.OnShow();
		if (notificationSink != null)
		{
			notificationSink.DialogShown(Id, currentState);
		}
		currentState = DialogState.Shown;
	}

	public override void OnHide()
	{
		if (notificationSink != null)
		{
			notificationSink.DialogClosing(Id, this);
		}
		base.OnHide();
		if (notificationSink != null)
		{
			notificationSink.DialogClosed(Id, currentState);
		}
	}

	public virtual void OnCancel()
	{
		currentState = DialogState.Cancel;
		if (notificationSink != null)
		{
			notificationSink.DialogCancelled(Id, currentState);
		}
		Hide();
	}

	public virtual void OnOk()
	{
		currentState = DialogState.Ok;
		Hide();
	}

	protected virtual void OnEscape(SHSKeyCode code)
	{
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (notificationSink != null)
		{
			switch (notificationSink.DialogGetUpdateStatus(Id))
			{
			case DialogUpdateState.Inactive:
				break;
			case DialogUpdateState.InProgress:
				break;
			case DialogUpdateState.Cancelled:
				OnCancel();
				break;
			case DialogUpdateState.Complete:
				OnOk();
				break;
			}
		}
	}

	private void fireQueueAction()
	{
	}

	protected virtual void OnEnter(SHSKeyCode code)
	{
		IGUIControl iGUIControl = controlList.Find(delegate(IGUIControl control)
		{
			return control is GUIDefaultButton;
		});
		if (iGUIControl != null && iGUIControl.IsEnabled)
		{
			((GUIDefaultButton)iGUIControl).FireMouseClick(new GUIClickEvent());
		}
	}
}
