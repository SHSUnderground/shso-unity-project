using UnityEngine;

public class SHSBrawlerComboWindow : GUIControlWindow
{
	protected class TriLabel : GUISimpleControlWindow
	{
		protected GUILabel top;

		protected GUILabel middle;

		protected GUILabel bottom;

		public TriLabel()
		{
			bottom = new GUILabel();
			bottom.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(2f, 2f));
			bottom.Alpha = 0f;
			middle = new GUILabel();
			middle.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(1f, 1f));
			middle.Alpha = 0f;
			top = new GUILabel();
			top.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f));
			top.Alpha = 0f;
			Add(bottom);
			Add(middle);
			Add(top);
		}

		public override void SetSize(float width, float height)
		{
			base.SetSize(width + 2f, height + 2f);
			bottom.SetSize(width, height);
			middle.SetSize(width, height);
			top.SetSize(width, height);
		}

		public void SetFontData(GUIFontManager.SupportedFontEnum font, int fontSize)
		{
			bottom.SetupText(font, fontSize, Utils.ColorFromBytes(178, 46, 0, byte.MaxValue), TextAnchor.UpperLeft);
			middle.SetupText(font, fontSize, Utils.ColorFromBytes(155, 94, 8, byte.MaxValue), TextAnchor.UpperLeft);
			top.SetupText(font, fontSize, Utils.ColorFromBytes(byte.MaxValue, 242, 191, byte.MaxValue), TextAnchor.UpperLeft);
		}

		public void SetText(string label)
		{
			bottom.Text = label;
			middle.Text = label;
			top.Text = label;
		}

		public void FadeIn(float fadeTime)
		{
			base.AnimationPieceManager.ClearAll();
			base.AnimationPieceManager.Add(AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(bottom.Alpha, 1f, fadeTime), bottom, middle, top));
		}

		public void FadeOut(float fadeTime)
		{
			base.AnimationPieceManager.ClearAll();
			base.AnimationPieceManager.Add(AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(bottom.Alpha, 0f, fadeTime), bottom, middle, top));
		}
	}

	private TriLabel doubleText;

	private TriLabel doubleLabel;

	private TriLabel tripleText;

	private TriLabel tripleLabel;

	private int lastCombo;

	private float doubleLabelY = 6f;

	private float tripleLabelY = 10f;

	public SHSBrawlerComboWindow()
	{
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		SetSize(176f, 176f);
		doubleText = new TriLabel();
		doubleText.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(26f, 100f));
		doubleText.SetSize(18f, 40f);
		doubleText.SetFontData(GUIFontManager.SupportedFontEnum.Grobold, 24);
		doubleText.SetText("2");
		doubleText.IsVisible = true;
		Add(doubleText);
		doubleLabel = new TriLabel();
		doubleLabel.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(44f, 106f));
		doubleLabel.SetSize(130f, 40f);
		doubleLabel.SetFontData(GUIFontManager.SupportedFontEnum.Grobold, 18);
		doubleLabel.SetText("#COMBO_POPUP");
		doubleLabel.IsVisible = true;
		Add(doubleLabel);
		tripleText = new TriLabel();
		tripleText.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(16f, 100f));
		tripleText.SetSize(30f, 40f);
		tripleText.SetFontData(GUIFontManager.SupportedFontEnum.Grobold, 30);
		tripleText.SetText("3");
		tripleText.IsVisible = true;
		Add(tripleText);
		tripleLabel = new TriLabel();
		tripleLabel.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(36f, 110f));
		tripleLabel.SetSize(130f, 40f);
		tripleLabel.SetFontData(GUIFontManager.SupportedFontEnum.Grobold, 20);
		tripleLabel.SetText("#COMBO_POPUP");
		tripleLabel.IsVisible = true;
		Add(tripleLabel);
	}

	public void UpdateCombo(float newComboHeat)
	{
		int num = Mathf.FloorToInt(newComboHeat);
		if (lastCombo == num)
		{
			return;
		}
		if (lastCombo < num)
		{
			if (num == 1)
			{
				base.AnimationPieceManager.ClearAll();
				BounceUpElement(doubleText, doubleLabel, doubleLabelY);
				doubleText.FadeIn(1f);
				doubleLabel.FadeIn(1f);
			}
			if (num == 2)
			{
				base.AnimationPieceManager.ClearAll();
				doubleText.FadeOut(0.5f);
				doubleLabel.FadeOut(0.5f);
				BounceUpElement(tripleText, tripleLabel, tripleLabelY);
				tripleText.FadeIn(1f);
				tripleLabel.FadeIn(1f);
			}
		}
		if (num < lastCombo)
		{
			if (num == 1)
			{
				TriLabel triLabel = doubleText;
				Vector2 offset = doubleText.Offset;
				triLabel.Offset = new Vector2(offset.x, 0f);
				TriLabel triLabel2 = doubleLabel;
				Vector2 offset2 = doubleLabel.Offset;
				triLabel2.Offset = new Vector2(offset2.x, doubleLabelY);
				doubleText.FadeIn(1f);
				doubleLabel.FadeIn(1f);
			}
			else
			{
				BounceDownElement(doubleText, doubleLabel, doubleLabelY);
				doubleText.FadeOut(1f);
				doubleLabel.FadeOut(1f);
			}
			BounceDownElement(tripleText, tripleLabel, tripleLabelY);
			tripleText.FadeOut(1f);
			tripleLabel.FadeOut(1f);
		}
		lastCombo = num;
	}

	protected void BounceUpElement(TriLabel valueLabel, TriLabel textLabel, float labelOffset)
	{
		AnimClipManager animationPieceManager = base.AnimationPieceManager;
		Vector2 offset = valueLabel.Offset;
		AnimClip pieceOne = AnimClipBuilder.Absolute.OffsetY(AnimClipBuilder.Path.Quadratic(offset.y, 0f, 1f, 1f), valueLabel);
		Vector2 offset2 = textLabel.Offset;
		animationPieceManager.Add(pieceOne ^ AnimClipBuilder.Absolute.OffsetY(AnimClipBuilder.Path.Quadratic(offset2.y, labelOffset, 1f, 1f), textLabel));
	}

	protected void BounceDownElement(TriLabel valueLabel, TriLabel textLabel, float labelOffset)
	{
		AnimClipManager animationPieceManager = base.AnimationPieceManager;
		Vector2 offset = valueLabel.Offset;
		AnimClip pieceOne = AnimClipBuilder.Absolute.OffsetY(AnimClipBuilder.Path.Quadratic(offset.y, 100f, 0f, 1f), valueLabel);
		Vector2 offset2 = textLabel.Offset;
		animationPieceManager.Add(pieceOne ^ AnimClipBuilder.Absolute.OffsetY(AnimClipBuilder.Path.Quadratic(offset2.y, 100f + labelOffset, 0f, 1f), textLabel));
	}
}
