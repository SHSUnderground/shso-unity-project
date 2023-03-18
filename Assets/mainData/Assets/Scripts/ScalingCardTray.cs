using UnityEngine;

public class ScalingCardTray : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject LeftEdge;

	public GameObject Center;

	public GameObject RightEdge;

	public GameObject Label;

	public static readonly float kEdgeSpacing = 0.2f;

	public static readonly float kEdgeWidth = 1.0256f;

	public float width = 1f;

	private float width_ = 1f;

	public GUIControl.AnchorAlignmentEnum anchor = GUIControl.AnchorAlignmentEnum.Middle;

	private GUIControl.AnchorAlignmentEnum anchor_ = GUIControl.AnchorAlignmentEnum.Middle;

	public GameObject neighborPanel;

	public Vector2 neighborOffset;

	private AnimClipManager AnimationPieceManager = new AnimClipManager();

	private AnimClip trayResizeAnimation;

	private float xOffset;

	private float yOffset;

	private static float kTrayHeight = 3.45f;

	public ScalingCardTray()
	{
		width_ = (width = 1f);
	}

	public void Awake()
	{
		Center.transform.localScale = new Vector3(width_, kTrayHeight, 1f);
		LeftEdge.transform.localScale = new Vector3(kEdgeWidth, kTrayHeight, 1f);
		RightEdge.transform.localScale = new Vector3(0f - kEdgeWidth, kTrayHeight, 1f);
	}

	public void Update()
	{
		AnimationPieceManager.Update(Time.deltaTime);
		if (width_ != width || anchor_ != anchor)
		{
			width_ = width;
			anchor_ = anchor;
			float num = (width_ + kEdgeWidth) / 2f;
			float num2 = width_ / 2f + kEdgeWidth;
			float num3 = kTrayHeight / 2f;
			xOffset = 0f;
			yOffset = 0f;
			switch (anchor_)
			{
			case GUIControl.AnchorAlignmentEnum.TopLeft:
				xOffset = num2;
				yOffset = 0f - num3;
				break;
			case GUIControl.AnchorAlignmentEnum.MiddleLeft:
				xOffset = num2;
				break;
			case GUIControl.AnchorAlignmentEnum.BottomLeft:
				xOffset = num2;
				yOffset = num3;
				break;
			case GUIControl.AnchorAlignmentEnum.TopMiddle:
				yOffset = 0f - num3;
				break;
			case GUIControl.AnchorAlignmentEnum.BottomMiddle:
				yOffset = num3;
				break;
			case GUIControl.AnchorAlignmentEnum.TopRight:
				xOffset = 0f - num2;
				yOffset = 0f - num3;
				break;
			case GUIControl.AnchorAlignmentEnum.MiddleRight:
				xOffset = 0f - num2;
				break;
			case GUIControl.AnchorAlignmentEnum.BottomRight:
				xOffset = 0f - num2;
				yOffset = num3;
				break;
			}
			Vector3 localScale = base.transform.localScale;
			Center.transform.localScale = new Vector3(width_, kTrayHeight, 1f);
			Center.transform.localPosition = new Vector3(xOffset, yOffset, 0f);
			LeftEdge.transform.localPosition = new Vector3(0f - num + xOffset, yOffset, 0f);
			RightEdge.transform.localPosition = new Vector3(num + xOffset, yOffset, 0f);
			if (Label != null)
			{
				Vector3 localPosition = Label.transform.localPosition;
				Label.transform.localPosition = new Vector3(xOffset * localScale.x, localPosition.y, localPosition.z);
			}
			if (neighborPanel != null)
			{
				Vector3 localPosition2 = base.transform.localPosition;
				Vector3 localPosition3 = neighborPanel.transform.localPosition;
				neighborPanel.transform.localPosition = new Vector3(localPosition2.x + xOffset * 2f * localScale.x + neighborOffset.x, localPosition2.y + neighborOffset.y, localPosition3.z);
			}
		}
	}

	public void Resize(float newWidth, float duration)
	{
		AnimPath path = AnimClipBuilder.Path.Linear(width, newWidth, duration);
		AnimClip newPiece = AnimClipBuilder.Custom.Function(path, delegate(float value)
		{
			width = value;
		});
		AnimationPieceManager.SwapOut(ref trayResizeAnimation, newPiece);
	}
}
