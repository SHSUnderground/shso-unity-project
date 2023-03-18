public class GUISimpleControlWindow : GUIControlWindow
{
	public GUISimpleControlWindow()
	{
		Traits.HitTestType = HitTestTypeEnum.Transparent;
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		Traits.EventListenerRegistrationTrait = ControlTraits.EventListenerRegistrationTraitEnum.Ignore;
	}

	protected override void AttachInspector()
	{
		inspector = guiTreeNode.AddComponent(typeof(GUISimpleControlWindowInspector));
	}

	protected override void ReflectToInspector()
	{
		base.ReflectToInspector();
	}

	protected override void ReflectFromInspector()
	{
		base.ReflectFromInspector();
	}
}
