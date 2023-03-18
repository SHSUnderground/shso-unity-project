public class SHSHQWindow : GUITopLevelWindow
{
	public class DisplayHQUIMessage : ShsEventMessage
	{
	}

	private SHSHQHUD HQHUD;

	public SHSHQWindow()
		: base("SHSHQWindow")
	{
		HQHUD = new SHSHQHUD();
		Add(HQHUD);
	}

	public override void OnShow()
	{
		base.OnShow();
		AppShell.Instance.EventMgr.AddListener<DisplayHQUIMessage>(OnDisplayHQUI);
	}

	public override void OnHide()
	{
		base.OnHide();
		AppShell.Instance.EventMgr.RemoveListener<DisplayHQUIMessage>(OnDisplayHQUI);
		HQHUD.IsVisible = false;
	}

	public void OnDisplayHQUI(DisplayHQUIMessage msg)
	{
		HQHUD.IsVisible = true;
	}

	protected override void InitializeBundleList()
	{
		base.InitializeBundleList();
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("hq_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
	}
}
