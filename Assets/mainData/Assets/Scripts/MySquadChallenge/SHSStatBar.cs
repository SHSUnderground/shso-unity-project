using UnityEngine;

namespace MySquadChallenge
{
	public class SHSStatBar : GUISimpleControlWindow
	{
		protected static readonly Vector2 LockIconSize = new Vector2(32f, 35f);

		public SHSStatBar()
		{
			SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		}

		public void CreateBarTitle(string title, Vector2 offset)
		{
			GUIStrokeTextLabel gUIStrokeTextLabel = GUIControl.CreateControlAbsolute<GUIStrokeTextLabel>(GetBarSize(), offset);
			gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(134, 208, 255), GUILabel.GenColor(0, 33, 101), GUILabel.GenColor(1, 33, 85), new Vector2(-2f, 2f), TextAnchor.UpperLeft);
			gUIStrokeTextLabel.SetPosition(new Vector2(15f, 18f));
			gUIStrokeTextLabel.Text = title;
			gUIStrokeTextLabel.Bold = true;
			Add(gUIStrokeTextLabel);
		}

		public void CreateBarTitle(string title)
		{
			CreateBarTitle(title, new Vector2(13f, -1f));
		}

		public GUIDrawTexture CreateIcon(Vector2 size, Vector2 offset)
		{
			GUIDrawTexture gUIDrawTexture = GUIControl.CreateControlAbsolute<GUIDrawTexture>(size, offset);
			gUIDrawTexture.Traits.ActivationTrait = ControlTraits.ActivationTraitEnum.Manual;
			gUIDrawTexture.HitTestType = HitTestTypeEnum.Circular;
			Add(gUIDrawTexture);
			return gUIDrawTexture;
		}

		public GUIDrawTexture CreateIcon(Vector2 size, Vector2 offset, string source)
		{
			GUIDrawTexture gUIDrawTexture = CreateIcon(size, offset);
			SetIconTexture(gUIDrawTexture, "mysquadgadget_bundle", source);
			return gUIDrawTexture;
		}

		public GUIDrawTexture CreateLockIcon(Vector2 size, Vector2 offset, string tooltip)
		{
			GUIDrawTexture gUIDrawTexture = CreateIcon(size, offset, "mysquad_gadget_charactericon_lock");
			gUIDrawTexture.HitTestSize = new Vector2(0.875f, 0.875f);
			gUIDrawTexture.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			gUIDrawTexture.IsVisible = false;
			CreateLockToolTip(gUIDrawTexture, tooltip);
			return gUIDrawTexture;
		}

		public void CreateLockToolTip(GUIDrawTexture lockIcon, string tooltip)
		{
			lockIcon.ToolTip = new NamedToolTipInfo(tooltip, new Vector2(20f, 0f));
		}

		public void SetIconTexture(GUIDrawTexture icon, string bundle, string source)
		{
			icon.TextureSource = bundle + "|" + source;
		}

		public virtual Vector2 GetBarOffset()
		{
			return Vector2.zero;
		}

		public virtual Vector2 GetBarSize()
		{
			return new Vector2(360f, 55f);
		}
	}
}
