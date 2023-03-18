using UnityEngine;

internal class GUIHubMenuDialogActivateMessage : ShsEventMessage
{
	public readonly IGUIControl sourceControl;

	public readonly IGUIControl targetControl;

	public readonly Vector2 sourceLocationOffset;

	public GUIHubMenuDialogActivateMessage(IGUIControl SourceControl, IGUIControl TargetControl, Vector2 LocationOffset)
	{
		sourceControl = SourceControl;
		targetControl = TargetControl;
		sourceLocationOffset = LocationOffset;
	}
}
