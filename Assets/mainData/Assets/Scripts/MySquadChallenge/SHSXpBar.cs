using UnityEngine;

namespace MySquadChallenge
{
	public class SHSXpBar : SHSStatBar
	{
		private GUIImage _barCap;

		private GUIImage _barMid;

		private GUIStrokeTextLabel _barText;

		private static readonly float _MinMidWidth = 1f;

		private static readonly float _MaxMidWidth = 259f;

		private static readonly float _MinTextWidth = 64f;

		public SHSXpBar()
		{
			GUIImage gUIImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(63f, 53f), new Vector2(1f, -1f));
			gUIImage.TextureSource = "mysquadgadget_bundle|L_mysquad_heroinfo_xp_icon";
			Add(gUIImage);
			GUIImage gUIImage2 = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(15f, 19f), new Vector2(60f, 18f));
			gUIImage2.TextureSource = "mysquadgadget_bundle|mshs_mysquad_heroes_xpbar_left";
			Add(gUIImage2);
			_barMid = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(_MinMidWidth, 19f), new Vector2(75f, 18f));
			_barMid.TextureSource = "mysquadgadget_bundle|mshs_mysquad_heroes_xpbar_fill";
			Add(_barMid);
			_barCap = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(15f, 19f), new Vector2(0f, 18f));
			_barCap.TextureSource = "mysquadgadget_bundle|mshs_mysquad_heroes_xpbar_right";
			Add(_barCap);
			_barText = GUIControl.CreateControlAbsolute<GUIStrokeTextLabel>(Vector2.zero, new Vector2(60f, 17f));
			_barText.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(126, 50, 0), GUILabel.GenColor(134, 0, 0), new Vector2(-2f, 2f), TextAnchor.MiddleCenter);
			_barText.Bold = true;
			Add(_barText);
			SetExperience(0f, "100/10,000");
		}

		public void SetExperience(float percent, string text)
		{
			percent = Mathf.Clamp(percent, 0f, 1f);
			float num = _MinMidWidth + percent * (_MaxMidWidth - _MinMidWidth);
			if (_barMid != null)
			{
				GUIImage barMid = _barMid;
				Vector2 size = _barMid.Size;
				barMid.SetSize(num, size.y);
			}
			if (_barCap != null && _barMid != null)
			{
				GUIImage barCap = _barCap;
				Vector2 position = _barMid.Position;
				float x = position.x + num;
				Vector2 position2 = _barCap.Position;
				barCap.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(x, position2.y));
			}
			if (_barText != null && !string.IsNullOrEmpty(text))
			{
				_barText.Text = text;
				float num2 = 0f;
				if (_barCap != null)
				{
					Vector2 size2 = _barCap.Size;
					num2 = size2.x;
				}
				GUIStrokeTextLabel barText = _barText;
				float x2 = Mathf.Max(num + num2 * 2f, _MinTextWidth);
				Vector2 size3 = _barText.Size;
				barText.Size = new Vector2(x2, size3.y);
			}
		}

		public override Vector2 GetBarOffset()
		{
			return new Vector2(15f, 45f);
		}
	}
}
