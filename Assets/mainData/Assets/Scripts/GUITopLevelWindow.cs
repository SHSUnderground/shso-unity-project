using UnityEngine;

public class GUITopLevelWindow : GUIWindow
{
	protected string windowName;

	public override string Id
	{
		get
		{
			return windowName;
		}
	}

	public GUITopLevelWindow(string name)
	{
		Id = (windowName = ((name != null) ? name : GetType().ToString()));
		HitTestType = HitTestTypeEnum.Transparent;
		Traits.VisibleAncestryTrait = ControlTraits.VisibilityAncestryTraitEnum.EnsureAncestorsVisible;
	}

	public override void DrawPreprocess()
	{
		base.DrawPreprocess();
	}

	public override void ConfigureKeyBanks()
	{
		base.ConfigureKeyBanks();
		keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.BackQuote, false, false, false), OnDebugMinimized);
		keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.BackQuote, true, false, false), OnDebug);
		keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.Return, false, true, false), OnAltEnter);
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.rect = GUIManager.ScreenRect;
		base.HandleResize(message);
	}

	protected virtual void OnDebugMinimized(SHSKeyCode code)
	{
		AppShell.Instance.EventMgr.Fire(this, new DebugWindowMessage(DebugWindowMessage.SizeTypeEnum.HalfScreen));
	}

	protected virtual void OnDebug(SHSKeyCode code)
	{
		AppShell.Instance.EventMgr.Fire(this, new DebugWindowMessage());
	}

	protected virtual void OnAltEnter(SHSKeyCode code)
	{
		if (Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
		{
			AppShell.Instance.AutoFullScreenToggle();
		}
	}

	public override void OnAdded(IGUIContainer addedTo)
	{
		if (Parent.GetType() == typeof(GUITopLevelWindow))
		{
			CspUtils.DebugLog("You should never be adding a TopLevelWindow to another TopLevelWindow. Boo for you! ... (hint: if you see this message, that means you ARE adding a TopLevelWindow to another TopLevelWindow, so fix it!)");
		}
		base.OnAdded(addedTo);
	}

	protected override void AttachInspector()
	{
		inspector = guiTreeNode.AddComponent(typeof(GUITopLevelWindowInspector));
	}

	protected override void ReflectToInspector()
	{
		base.ReflectToInspector();
		if (inspector != null)
		{
			((GUITopLevelWindowInspector)inspector).windowName = windowName;
		}
	}

	protected override void ReflectFromInspector()
	{
		base.ReflectFromInspector();
		if (inspector != null)
		{
			windowName = ((GUITopLevelWindowInspector)inspector).windowName;
		}
	}
}
