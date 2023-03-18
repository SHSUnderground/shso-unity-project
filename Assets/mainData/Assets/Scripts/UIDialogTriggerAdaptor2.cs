using System;
using System.Reflection;

public class UIDialogTriggerAdaptor2 : UITriggerAdaptor2
{
	public string WindowSpawnLocation;

	public string DialogTypeName;

	public string WindowText = string.Empty;

	public void OnEnable()
	{
		if (DialogTypeName == "SHSPrizeWheelDialog")
		{
			Utils.ActivateTree(base.transform.parent.transform.parent.gameObject, false);
		}
	}

	protected override void OnConfirmed()
	{
		//Discarded unreachable code: IL_00e2
		DialogInUse = true;
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		Type type = executingAssembly.GetType(DialogTypeName);
		if (type == null)
		{
			CspUtils.DebugLog("Can't get type: " + DialogTypeName + " from executing assembly.");
			DialogInUse = false;
		}
		else
		{
			try
			{
				GUIManager.Instance.ShowDialog(type, WindowText, WindowSpawnLocation, new GUIDialogNotificationSink(delegate(string Id, IGUIDialogWindow window)
				{
					window.LinkedObject = base.gameObject;
				}, delegate
				{
				}, delegate
				{
				}, delegate
				{
				}, delegate(string Id, GUIDialogWindow.DialogState state)
				{
					DialogInUse = false;
					if (state != GUIDialogWindow.DialogState.Ok)
					{
					}
				}), GUIControl.ModalLevelEnum.Default);
			}
			catch (Exception ex)
			{
				DialogInUse = false;
				throw ex;
			}
		}
	}
}
