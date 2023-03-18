using UnityEngine;

public class GUI3DDrawTexture : GUIDrawTexture
{
	private GameObject linkedObject;

	private bool isLinked;

	public GameObject LinkedObject
	{
		get
		{
			return linkedObject;
		}
		set
		{
			linkedObject = value;
			if (value == null)
			{
				isLinked = false;
				texture = null;
			}
			else
			{
				configureLinkedObject();
			}
		}
	}

	public bool IsLinked
	{
		get
		{
			return isLinked;
		}
	}

	private void configureLinkedObject()
	{
		if (!isLinked && !(linkedObject == null))
		{
			Camera camera = linkedObject.GetComponent(typeof(Camera)) as Camera;
			if (camera == null)
			{
				CspUtils.DebugLog("Linked Object for " + Id + " doesn't have a camera component. This is madness!");
			}
			else
			{
				texture = camera.targetTexture;
			}
		}
	}

	public override void ConfigureKeyBanks()
	{
		base.ConfigureKeyBanks();
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
	}
}
