using UnityEngine;

public class SHSNewsGadget : SHSGadget
{
	private class NewsTopWindow : GadgetTopWindow
	{
		public NewsTopWindow()
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(592f, 141f), new Vector2(0f, 10f));
			gUIImage.TextureSource = "persistent_bundle|gadget_topmodule";
			Add(gUIImage);
			GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(341f, 54f), new Vector2(0f, 0f));
			gUIImage2.TextureSource = "gameworld_bundle|L_mshs_newspaper_title";
			Add(gUIImage2);
		}
	}

	private SHSNewsMainWindow mainWindow;

	private bool newsClosedMessageFired;

	public SHSNewsGadget()
	{
		mainWindow = new SHSNewsMainWindow(this);
		SetBackgroundSize(new Vector2(1020f, 644f));
		SetupOpeningWindow(BackgroundType.OnePanel, mainWindow);
		SetupOpeningTopWindow(new NewsTopWindow());
		SetBackgroundImage("gameworld_bundle|mshs_newspaper_frame");
		newsClosedMessageFired = false;
		AppShell.Instance.EventMgr.AddListener<NewsClosedMessage>(OnNewsClosedMessage);
		CloseButton.Offset = new Vector2(456f, -555f);
		base.AnimationOpenFinished += mainWindow.OnOpenAnimationComplete;
	}

	static SHSNewsGadget()
	{
	}

	public override void CloseGadget()
	{
		AppShell.Instance.EventMgr.RemoveListener<NewsClosedMessage>(OnNewsClosedMessage);
		if (!newsClosedMessageFired)
		{
			AppShell.Instance.EventMgr.Fire(this, new NewsClosedMessage());
		}
		base.CloseGadget();
	}

	private void OnNewsClosedMessage(NewsClosedMessage msg)
	{
		newsClosedMessageFired = true;
		CloseGadget();
	}
}
