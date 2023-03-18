using UnityEngine;

public interface IGUIDialogWindow
{
	GUIDialogWindow.DialogState CurrentState
	{
		get;
	}

	GameObject LinkedObject
	{
		get;
		set;
	}

	IGUIDialogNotification NotificationSink
	{
		get;
		set;
	}

	string Text
	{
		get;
		set;
	}

	string TitleText
	{
		get;
		set;
	}
}
