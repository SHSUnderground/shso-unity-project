public class SHSWelcomeMainWindow : SHSGadget.GadgetCenterWindow
{
	public static string WebPromoOverrideImageName = "content$ingame_welcome_promo.png";

	private SHSNewsRewardTableWindow rewardTable;

	private SHSWelcomeGadget welcomeGadget;

	private GUIImage heroRenderImage;

	private GUIStrokeTextLabel heroTitle;

	private GUIDropShadowTextLabel heroDescription;

	private GUIStrokeTextLabel sellLabel;

	private bool HasWebPromoOverrideImage;

	public SHSWelcomeMainWindow(SHSWelcomeGadget gadget)
	{
	}

	public void OnOpenAnimationComplete()
	{
		AppShell.Instance.SharedHashTable["NewsPaperHasBeenShown"] = true;
		if (rewardTable != null)
		{
			rewardTable.StartRewardDayAnimations();
		}
	}
}
