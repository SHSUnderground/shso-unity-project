using UnityEngine;

public class GUI9SliceImage : GUIControlWindow
{
	private GUIImage[] Image9Slices = new GUIImage[9];

	protected string textureSource;

	public string TextureSource
	{
		get
		{
			return textureSource;
		}
		set
		{
			textureSource = value;
			for (int i = 0; i < Image9Slices.Length; i++)
			{
				Image9Slices[i].TextureSource = textureSource + "0" + (i + 1);
				Image9Slices[i].SetSize(Image9Slices[i].Rect.width, Image9Slices[i].Rect.height);
			}
			if (Image9Slices[0].Rect.width != Image9Slices[6].Rect.width || Image9Slices[0].Rect.height != Image9Slices[2].Rect.height || Image9Slices[6].Rect.height != Image9Slices[8].Rect.height || Image9Slices[2].Rect.width != Image9Slices[8].Rect.width)
			{
				CspUtils.DebugLog("Images not of correct size for 9 slice formatting");
			}
			Setup9Slices();
		}
	}

	public GUI9SliceImage()
	{
		Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Ignore;
		for (int i = 0; i < Image9Slices.Length; i++)
		{
			Image9Slices[i] = new GUIImage();
			Add(Image9Slices[i]);
		}
		Image9Slices[0].SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		Image9Slices[2].SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		Image9Slices[6].SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft);
		Image9Slices[8].SetPosition(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight);
		Image9Slices[1].SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		Image9Slices[3].SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		Image9Slices[5].SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		Image9Slices[7].SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft);
		Image9Slices[4].SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
	}

	private void Setup9Slices()
	{
		float width = Rect.width - Image9Slices[0].Rect.width - Image9Slices[2].Rect.width;
		float width2 = Image9Slices[0].Rect.width;
		float width3 = Image9Slices[2].Rect.width;
		float height = Rect.height - Image9Slices[0].Rect.height - Image9Slices[6].Rect.height;
		float height2 = Image9Slices[0].Rect.height;
		float height3 = Image9Slices[6].Rect.height;
		Image9Slices[1].SetSize(width, height2);
		Image9Slices[1].Offset = new Vector2(width2, 0f);
		Image9Slices[3].SetSize(width2, height);
		Image9Slices[3].Offset = new Vector2(0f, height2);
		Image9Slices[5].SetSize(width3, height);
		Image9Slices[5].Offset = new Vector2(0f, height2);
		Image9Slices[7].SetSize(width, height3);
		Image9Slices[7].Offset = new Vector2(width2, 0f);
		Image9Slices[4].SetSize(width, height);
		Image9Slices[4].Offset = new Vector2(width2, height2);
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		Setup9Slices();
	}
}
