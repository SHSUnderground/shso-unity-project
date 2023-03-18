public class GUIRootWindow : GUIWindow
{
	public override bool IsRoot
	{
		get
		{
			return true;
		}
	}

	public GUIRootWindow()
	{
		Traits = new ControlTraits(ControlTraits.ActivationTraitEnum.Manual, ControlTraits.VisibilityTraitEnum.Manual, ControlTraits.DeactivationTraitEnum.DeactivateOnHide, ControlTraits.VisibilityAncestryTraitEnum.EnsureAncestorsVisible, ControlTraits.UpdateTraitEnum.ActiveAndEnabled, ControlTraits.LifeSpanTraitEnum.KeepAlive, HitTestTypeEnum.Transparent, BlockTestTypeEnum.Transparent, ControlTraits.EventHandlingEnum.Ignore, ControlTraits.ResourceLoadingTraitEnum.Async, ControlTraits.ResourceLoadingPhaseTraitEnum.Show, ControlTraits.EventListenerRegistrationTraitEnum.Register, ControlTraits.DeactiveAlphaBlendTraitEnum.FadeSyncInternal, ControlTraits.FullScreenOpaqueBackgroundTraitEnum.DoesNotHaveFullScreenOpaqueBackground, ControlTraits.RespectDisabledAlphaTraitEnum.RespectDisabledAlpha, ControlTraits.ContentDependentDisableTraitEnum.DisableOnContentDependency);
		AppShell.Instance.EventMgr.AddListener<GUIResizeMessage>(HandleResize);
		isVisible = true;
		FontInfo = new GUIFontInfo(GUIFontManager.SupportedFontEnum.Komica, 10);
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		DrawPreprocess();
		base.Draw(drawFlags);
		DrawFinalize();
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.rect = GUIManager.ScreenRect;
		base.HandleResize(message);
	}

	public override bool CanDrop(DragDropInfo DragDropInfo)
	{
		return false;
	}
}
