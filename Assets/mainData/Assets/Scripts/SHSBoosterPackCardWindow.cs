using System;
using UnityEngine;

public class SHSBoosterPackCardWindow : GUIControlWindow
{
	public delegate void AnimFinished();

	private readonly GUIImage cardBack;

	private readonly GUIImage cardFace;

	private GUIImage currentFlipPiece;

	public AnimFinished OnAnimFinished;

	protected int xStartOffset = -84;

	protected int xIncrement = 81;

	protected int yStartOffset = -69;

	protected float targetSize = 0.35f;

	protected float animationSpeed = 0.75f;

	protected float flipTriggerTime = 0.3f;

	protected float flipDuration = 0.5f;

	public GUIImage CardFace
	{
		get
		{
			return cardFace;
		}
	}

	public SHSBoosterPackCardWindow(Texture2D faceSource, string backSource)
	{
		cardFace = new GUIImage();
		CardFace.Texture = faceSource;
		CardFace.SetPositionAndSize(QuickSizingHint.ParentSize);
		CardFace.IsVisible = false;
		Add(CardFace);
		cardBack = new GUIImage();
		cardBack.TextureSource = backSource;
		cardBack.SetPositionAndSize(QuickSizingHint.ParentSize);
		cardBack.IsVisible = true;
		Add(cardBack);
		currentFlipPiece = cardBack;
	}

	protected void Flip(float duration)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		CardFace.SetPosition(QuickSizingHint.Centered);
		cardBack.SetPosition(QuickSizingHint.Centered);
		AnimClip animClip = AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(1f, 0f, duration / 2f), FlipSetSize);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Expected O, but got Unknown
			currentFlipPiece.IsVisible = false;
			currentFlipPiece = CardFace;
			currentFlipPiece.IsVisible = true;
			currentFlipPiece.SetSize(0f, 0f);
			AnimClip animClip2 = AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(0f, 1f, duration / 2f), FlipSetSize);
			animClip2.OnFinished += (Action)(object)(Action)delegate
			{
				CardFace.SetPositionAndSize(QuickSizingHint.ParentSize);
				cardBack.SetPositionAndSize(QuickSizingHint.ParentSize);
				if (OnAnimFinished != null)
				{
					OnAnimFinished();
				}
			};
			base.AnimationPieceManager.Add(animClip2);
		};
		base.AnimationPieceManager.Add(animClip);
	}

	private void FlipSetSize(float i)
	{
		Vector2 size = Size;
		float x = size.x * i;
		Vector2 size2 = Size;
		Vector2 size3 = new Vector2(x, size2.y);
		currentFlipPiece.SetSize(size3);
	}

	public void SetDisplayParameters(int xstartOffset, int xincrement, int ystartoffset, float targetsize, float animationspeed, float fliptriggertime, float flipduration)
	{
		xStartOffset = xstartOffset;
		xIncrement = xincrement;
		yStartOffset = ystartoffset;
		targetSize = targetsize;
		animationSpeed = animationspeed;
		flipTriggerTime = fliptriggertime;
		flipDuration = flipduration;
	}

	public virtual void DisplayCard(int cardIndex, float initialDelay, float modifier)
	{
		throw new NotImplementedException();
	}
}
