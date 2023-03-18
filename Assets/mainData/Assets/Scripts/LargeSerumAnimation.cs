using System;
using UnityEngine;

public class LargeSerumAnimation : GUISimpleControlWindow
{
	protected GUIImage serumImage;

	protected AnimClip serumAnim;

	protected int pathNumber;

	public LargeSerumAnimation()
	{
		serumImage = new GUIImage();
		serumImage.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(200f, 200f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		serumImage.TextureSource = "mysquadgadget_bundle|serum_icon_01";
		Add(serumImage);
	}

	public void BeginSerumDrip()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		AnimClip animClip = AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(1f, 11f, 2f), Flip);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			BeginSerumDrip();
		};
		base.AnimationPieceManager.SwapOut(ref serumAnim, animClip);
	}

	protected void Flip(float i)
	{
		int num = Mathf.RoundToInt(i);
		if (num > 11)
		{
			num = 1;
		}
		SetPath(num);
	}

	protected void SetPath(int i)
	{
		if (i != pathNumber)
		{
			pathNumber = i;
			serumImage.TextureSource = string.Format("mysquadgadget_bundle|serum_icon_{0:00}", i);
		}
	}
}
