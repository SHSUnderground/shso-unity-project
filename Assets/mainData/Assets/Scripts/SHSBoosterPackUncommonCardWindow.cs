using System;
using UnityEngine;

public class SHSBoosterPackUncommonCardWindow : SHSBoosterPackCardWindow
{
	public SHSBoosterPackUncommonCardWindow(Texture2D faceSource, string backSource)
		: base(faceSource, backSource)
	{
	}

	public override void DisplayCard(int cardIndex, float initialDelay, float modifier)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		AnimClip animClip = SHSAnimations.Generic.Wait(initialDelay);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Expected O, but got Unknown
			GUIControl gUIControl = this;
			gUIControl.BringToFront();
			((SHSBoosterPackOpeningWindow)parent).boosterImg.BringToFront();
			float time = animationSpeed * modifier;
			Vector2[] obj = new Vector2[4]
			{
				gUIControl.Offset,
				default(Vector2),
				default(Vector2),
				default(Vector2)
			};
			//ref Vector2 reference = ref obj[1];
			Vector2 offset = gUIControl.Offset;
			float x = offset.x;
			Vector2 offset2 = gUIControl.Offset;
			obj[1] = new Vector2(x, offset2.y - 340f);
			//ref Vector2 reference2 = ref obj[2];
			float x2 = xStartOffset + xIncrement * cardIndex;
			Vector2 offset3 = gUIControl.Offset;
			obj[2] = new Vector2(x2, offset3.y - 200f);
			obj[3] = new Vector2(xStartOffset + xIncrement * cardIndex, yStartOffset);
			AnimClip pieceOne = SHSAnimations.Spline.BSplineCurveOffsetXY(time, gUIControl, obj);
			Vector2 size = gUIControl.Size;
			float x3 = size.x;
			Vector2 size2 = gUIControl.Size;
			AnimClip pieceOne2 = pieceOne ^ AnimClipBuilder.Absolute.SizeX(SHSAnimations.GenericPaths.LinearWithWiggle(x3, size2.x * targetSize, animationSpeed * modifier, 10f, 0.25f), gUIControl);
			Vector2 size3 = gUIControl.Size;
			float y = size3.y;
			Vector2 size4 = gUIControl.Size;
			AnimClip toAdd = pieceOne2 ^ AnimClipBuilder.Absolute.SizeY(SHSAnimations.GenericPaths.LinearWithWiggle(y, size4.y * targetSize, animationSpeed * modifier, 10f, 0.25f), gUIControl) ^ AnimClipBuilder.Delta.Rotation(AnimClipBuilder.Path.Linear(gUIControl.Rotation, 0f, animationSpeed * modifier), gUIControl);
			AnimClip animClip2 = SHSAnimations.Generic.Wait(animationSpeed * modifier * flipTriggerTime);
			animClip2.OnFinished += (Action)(object)(Action)delegate
			{
				Flip(flipDuration);
			};
			base.AnimationPieceManager.Add(toAdd);
			base.AnimationPieceManager.Add(animClip2);
		};
		base.AnimationPieceManager.Add(animClip);
	}
}
