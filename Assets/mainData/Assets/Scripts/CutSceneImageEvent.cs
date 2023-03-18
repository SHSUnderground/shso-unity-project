using UnityEngine;

[AddComponentMenu("Cut Scene Clips/Image")]
public class CutSceneImageEvent : CutSceneEvent
{
	public string textureSource;

	public bool useColor;

	public Color textureColor;

	protected GUIImage eventImage;

	public CutSceneImageEvent()
	{
		textureSource = string.Empty;
		textureColor.r = 0f;
		textureColor.g = 0f;
		textureColor.b = 0f;
		textureColor.a = 0f;
		eventImage = null;
		useColor = false;
	}

	public void Start()
	{
		if (eventImage == null)
		{
			eventImage = new GUIImage();
		}
	}

	public override void StartEvent()
	{
		base.StartEvent();
		eventImage.Clear();
		eventImage.SetPositionAndSize(GUIControl.QuickSizingHint.ParentSize);
		eventImage.IsVisible = true;
		eventImage.TextureSource = textureSource;
		if (useColor)
		{
			eventImage.Color = textureColor;
		}
		GUIWindow gUIWindow = (GUIWindow)GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow"];
		if (gUIWindow != null)
		{
			gUIWindow.Add(eventImage);
		}
		else
		{
			LogEventError("Failed to obtain main window for image event");
		}
	}

	public override void EndEvent()
	{
		base.EndEvent();
		GUIWindow gUIWindow = (GUIWindow)GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow"];
		if (gUIWindow != null)
		{
			gUIWindow.Remove(eventImage);
		}
		else
		{
			LogEventError("Failed to obtain main window for image event");
		}
	}
}
