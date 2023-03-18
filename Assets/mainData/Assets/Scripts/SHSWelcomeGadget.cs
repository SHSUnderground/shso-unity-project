using UnityEngine;

public class SHSWelcomeGadget : SHSGadget
{
	private class WelcomeTopWindow : GadgetTopWindow
	{
		public WelcomeTopWindow()
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(148f, 36f), new Vector2(0f, -8f));
			gUIImage.TextureSource = "gameworld_bundle|L_welcome_gadget_title";
			Add(gUIImage);
		}
	}

	private SHSWelcomeMainWindow mainWindow;

	private bool welcomeClosedMessageFired;

	public SHSWelcomeGadget()
	{
		Traits.FullScreenOpaqueBackgroundTrait = ControlTraits.FullScreenOpaqueBackgroundTraitEnum.DoesNotHaveFullScreenOpaqueBackground;
		mainWindow = new SHSWelcomeMainWindow(this);
		SetBackgroundSize(new Vector2(461f, 639f));
		SetupOpeningWindow(BackgroundType.OnePanel, mainWindow);
		SetupOpeningTopWindow(new WelcomeTopWindow());
		SetBackgroundImage("gameworld_bundle|welcome_gadget_main_background");
		welcomeClosedMessageFired = false;
		CloseButton.Offset = new Vector2(186f, -587f);
		CloseButton.SetSize(new Vector2(32f, 32f));
		CloseButton.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|x_button_welcome_gadget");
		base.AnimationOpenFinished += mainWindow.OnOpenAnimationComplete;
	}

	public override void CloseGadget()
	{
		AppShell.Instance.EventMgr.RemoveListener<NewsClosedMessage>(OnWelcomeClosedMessage);
		if (!welcomeClosedMessageFired)
		{
			AppShell.Instance.EventMgr.Fire(this, new NewsClosedMessage());
		}
		base.CloseGadget();
	}

	private void OnWelcomeClosedMessage(NewsClosedMessage msg)
	{
		welcomeClosedMessageFired = true;
		CloseGadget();
	}
}
