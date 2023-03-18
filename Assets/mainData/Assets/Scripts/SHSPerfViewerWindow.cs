using UnityEngine;

public class SHSPerfViewerWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	public GUILayoutControlWindow leftWindow;

	private GameObject npcRoot;

	private GameObject tbRoot;

	private GameObject pigRoot;

	public SHSPerfViewerWindow(string PanelName)
		: base(PanelName, null)
	{
		SetBackground(new Color(0.3f, 0.5f, 0.8f, 0.2f));
		leftWindow = new GUILayoutControlWindow();
		leftWindow.SetBackground(new Color(1f, 0.5f, 0.5f, 1f));
		leftWindow.Margin = new Rect(20f, 20f, 20f, 20f);
		Add(leftWindow);
		leftWindow.Add(new GUILayoutBeginSection(GUILayoutBeginSection.OrientationEnum.Vertical, 250));
		GUIToggleButton gUIToggleButton = addCustomToggleButton("UI TOGGLE", leftWindow);
		gUIToggleButton.Value = true;
		gUIToggleButton.Changed += delegate(GUIControl sender, GUIChangedEvent eventData)
		{
			GUIManager.Instance.DrawingEnabled = (eventData.NewValue == 1f);
		};
		GUIToggleButton gUIToggleButton2 = addCustomToggleButton("NPC TOGGLE", leftWindow);
		gUIToggleButton2.Value = true;
		gUIToggleButton2.Changed += delegate(GUIControl sender, GUIChangedEvent eventData)
		{
			if (npcRoot == null)
			{
				npcRoot = GameObject.Find("NPC_RT");
			}
			if (npcRoot != null)
			{
				Utils.ActivateTree(npcRoot, eventData.NewValue == 1f);
				CspUtils.DebugLog("NPCs set to: " + eventData.NewValue);
			}
		};
		GUIToggleButton gUIToggleButton3 = addCustomToggleButton("TROUBLE BOT TOGGLE", leftWindow);
		gUIToggleButton3.Value = true;
		gUIToggleButton3.Changed += delegate(GUIControl sender, GUIChangedEvent eventData)
		{
			if (tbRoot == null)
			{
				tbRoot = GameObject.Find("TroubleBots");
			}
			if (tbRoot != null)
			{
				Utils.ActivateTree(tbRoot, eventData.NewValue == 1f);
				CspUtils.DebugLog("TroubleBots set to: " + eventData.NewValue);
			}
		};
		GUIToggleButton gUIToggleButton4 = addCustomToggleButton("PIGEONS TOGGLE", leftWindow);
		gUIToggleButton4.Value = true;
		gUIToggleButton4.Changed += delegate(GUIControl sender, GUIChangedEvent eventData)
		{
			if (pigRoot == null)
			{
				pigRoot = GameObject.Find("Pigeons");
			}
			if (pigRoot != null)
			{
				Utils.ActivateTree(pigRoot, eventData.NewValue == 1f);
				CspUtils.DebugLog("Pigeons set to: " + eventData.NewValue);
			}
		};
		GUIToggleButton gUIToggleButton5 = addCustomToggleButton("CAMERA TOGGLE", leftWindow);
		gUIToggleButton5.Value = true;
		gUIToggleButton5.Changed += delegate(GUIControl sender, GUIChangedEvent eventData)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
			if (gameObject != null)
			{
				gameObject.camera.enabled = (eventData.NewValue == 1f);
				CspUtils.DebugLog("Camera set to: " + eventData.NewValue);
			}
		};
		leftWindow.Add(new GUILayoutEndSection(GUILayoutEndSection.OrientationEnum.Vertical));
	}

	private GUIToggleButton addCustomToggleButton(string name, GUIWindow theWindowToAddItToo)
	{
		GUIToggleButton gUIToggleButton = new GUIToggleButton();
		gUIToggleButton.Text = name;
		gUIToggleButton.Spacing = 35f;
		gUIToggleButton.SetButtonSize(new Vector2(25f, 25f));
		gUIToggleButton.SetSize(240f, 25f);
		gUIToggleButton.Value = false;
		gUIToggleButton.Margin = new Rect(5f, 5f, 5f, 5f);
		theWindowToAddItToo.Add(gUIToggleButton);
		return gUIToggleButton;
	}

	public override void OnActive()
	{
		base.OnActive();
	}

	public override void DrawPreprocess()
	{
		base.DrawPreprocess();
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		if (leftWindow != null)
		{
			leftWindow.SetPositionAndSize(new Vector2(10f, 0f), DockingAlignmentEnum.None, AnchorAlignmentEnum.None, OffsetType.Absolute, Vector2.zero, new Vector2(0.3f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		}
	}

	public override void DrawFinalize()
	{
		base.DrawFinalize();
	}
}
