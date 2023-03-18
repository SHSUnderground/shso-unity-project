using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSGlowOutlineWindow : GUISimpleControlWindow
{
	private class GlowBead : GUIImage
	{
		private List<Vector2> SelectedGlowPath;

		private float SelectedGlowPathLength;

		private float glowOffset;

		public GlowBead(List<Vector2> SelectedGlowPath, float glowPathLength, float glowOffsetPerc, float glowOffsetLength, float sizePerc, string beadTextureSource)
		{
			Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			IsVisible = false;
			SetSize(new Vector2(24f, 24f) * sizePerc);
			TextureSource = beadTextureSource;
			this.SelectedGlowPath = SelectedGlowPath;
			glowOffset = glowOffsetPerc + glowOffsetLength / glowPathLength;
			SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
			SelectedGlowPathLength = glowPathLength;
		}

		public void SetupGlow(float nonAdj)
		{
			float x = (nonAdj + glowOffset) % 1f;
			Offset = GetOffset(x);
		}

		private Vector2 GetOffset(float x)
		{
			float num = SelectedGlowPathLength * x;
			float num2 = 0f;
			for (int i = 0; i < SelectedGlowPath.Count; i++)
			{
				Vector2 a = (i == SelectedGlowPath.Count - 1) ? SelectedGlowPath[0] : SelectedGlowPath[i + 1];
				Vector2 vector = SelectedGlowPath[i];
				Vector2 a2 = a - vector;
				float magnitude = a2.magnitude;
				if (num2 + magnitude >= num)
				{
					float num3 = num - num2;
					float d = num3 / magnitude;
					Vector2 b = a2 * d;
					return vector + b;
				}
				num2 += magnitude;
			}
			CspUtils.DebugLog("warning, no vector found, you did something screwy");
			return Vector2.zero;
		}
	}

	private List<Vector2> SelectedGlowPath;

	private List<GlowBead> glowBeads = new List<GlowBead>();

	private bool highlightOn;

	private float GlowPathLength;

	private string beadTextureSource;

	public float HighlightSpeed = 200f;

	private AnimClip curAnim;

	public string BeadTextureSource
	{
		get
		{
			return beadTextureSource;
		}
		set
		{
			beadTextureSource = value;
			foreach (GlowBead glowBead in glowBeads)
			{
				glowBead.TextureSource = beadTextureSource;
			}
		}
	}

	public SHSGlowOutlineWindow(params Vector2[] vectors)
		: this(new List<Vector2>(vectors))
	{
	}

	public SHSGlowOutlineWindow(List<Vector2> SelectedGlowPath)
		: this(SelectedGlowPath, "cardgamegadget_bundle|cardlaunch_glowydot", 2, 1f, 1f)
	{
	}

	public SHSGlowOutlineWindow(List<Vector2> SelectedGlowPath, string beadTextureSource)
		: this(SelectedGlowPath, beadTextureSource, 2, 1f, 1f)
	{
	}

	public SHSGlowOutlineWindow(List<Vector2> SelectedGlowPath, int beadCount)
		: this(SelectedGlowPath, "cardgamegadget_bundle|cardlaunch_glowydot", beadCount, 1f, 1f)
	{
	}

	public SHSGlowOutlineWindow(List<Vector2> SelectedGlowPath, string beadTextureSource, int beadCount)
		: this(SelectedGlowPath, beadTextureSource, beadCount, 1f, 1f)
	{
	}

	public SHSGlowOutlineWindow(List<Vector2> SelectedGlowPath, string beadTextureSource, int beadCount, float beadScale)
		: this(SelectedGlowPath, beadTextureSource, beadCount, beadScale, 1f)
	{
	}

	public SHSGlowOutlineWindow(List<Vector2> SelectedGlowPath, string beadTextureSource, int beadCount, float beadScale, float beadSpacingScale)
	{
		this.SelectedGlowPath = SelectedGlowPath;
		this.beadTextureSource = beadTextureSource;
		GlowPathLength = CalculateSelectedGlowPathLength();
		for (int i = 0; i < beadCount; i++)
		{
			float glowOffsetPerc = (float)i / (float)beadCount;
			glowBeads.Add(new GlowBead(SelectedGlowPath, GlowPathLength, glowOffsetPerc, 20f * beadSpacingScale, 1f * beadScale, beadTextureSource));
			glowBeads.Add(new GlowBead(SelectedGlowPath, GlowPathLength, glowOffsetPerc, 10f * beadSpacingScale, 0.85f * beadScale, beadTextureSource));
			glowBeads.Add(new GlowBead(SelectedGlowPath, GlowPathLength, glowOffsetPerc, 0f, 0.6f * beadScale, beadTextureSource));
		}
		foreach (GlowBead glowBead in glowBeads)
		{
			Add(glowBead);
		}
	}

	public void Highlight(bool highlightOn)
	{
		this.highlightOn = highlightOn;
		if (IsVisible)
		{
			if (highlightOn)
			{
				HighlightOn();
			}
			else
			{
				HighlightOff();
			}
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		Highlight(highlightOn);
	}

	private void HighlightOn()
	{
		foreach (GlowBead glowBead in glowBeads)
		{
			ControlList.Remove(glowBead);
			ControlList.Add(glowBead);
			glowBead.IsVisible = true;
			glowBead.SetupGlow(0f);
		}
		BeginAnimation();
	}

	private void BeginAnimation()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		base.AnimationPieceManager.SwapOut(ref curAnim, AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(0f, 1f, GlowPathLength / HighlightSpeed), AnimateGlowBeads));
		curAnim.OnFinished += (Action)(object)(Action)delegate
		{
			BeginAnimation();
		};
	}

	private void AnimateGlowBeads(float x)
	{
		foreach (GlowBead glowBead in glowBeads)
		{
			glowBead.SetupGlow(x);
		}
	}

	private void HighlightOff()
	{
		foreach (GlowBead glowBead in glowBeads)
		{
			glowBead.IsVisible = false;
		}
		base.AnimationPieceManager.RemoveIfUnfinished(curAnim);
	}

	public static List<Vector2> GetGlowPath(params Vector2[] vectors)
	{
		return new List<Vector2>(vectors);
	}

	public static List<Vector2> GenerateCircularPath(float radius, int resolution)
	{
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < resolution; i++)
		{
			float f = (float)Math.PI * 2f * ((float)i / (float)resolution);
			float x = Mathf.Cos(f) * radius;
			float y = Mathf.Sin(f) * radius;
			Vector2 item = new Vector2(x, y);
			list.Add(item);
		}
		return list;
	}

	private float CalculateSelectedGlowPathLength()
	{
		float num = 0f;
		for (int i = 0; i < SelectedGlowPath.Count; i++)
		{
			Vector2 a = (i == SelectedGlowPath.Count - 1) ? SelectedGlowPath[0] : SelectedGlowPath[i + 1];
			Vector2 b = SelectedGlowPath[i];
			num += (a - b).magnitude;
		}
		return num;
	}
}
