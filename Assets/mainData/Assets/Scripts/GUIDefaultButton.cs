public class GUIDefaultButton : GUIButton, IGUIDefaultInvoke
{
	public bool isDefault
	{
		get
		{
			return true;
		}
		set
		{
		}
	}

	protected override void AttachInspector()
	{
		inspector = guiTreeNode.AddComponent(typeof(GUIDefaultButtonInspector));
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
