public class GUIControlWindow : GUIWindow
{
	public GUIControlWindow()
	{
		Traits = ControlTraits.ControlDefault;
		Traits.EventListenerRegistrationTrait = ControlTraits.EventListenerRegistrationTraitEnum.Register;
	}

	protected override void AttachInspector()
	{
		inspector = guiTreeNode.AddComponent(typeof(GUIControlWindowInspector));
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
