using UnityEngine;

namespace MySquadChallenge
{
	public class SHSHotspots : SHSStatBar
	{
		private GUIImage _healthBar;

		private GUIStrokeTextLabel _healthBarText;

		private GUIDrawTexture[] _healthBarLocks;

		private static int _MaxHealthBarLocks = 3;

		public SHSHotspots()
		{
			_MaxHealthBarLocks = StatLevelReqsDefinition.HEALTH_RANK_COUNT;
			_healthBar = GUIControl.CreateControlAbsolute<GUIImage>(Vector2.zero, new Vector2(117f, 18f));
			Add(_healthBar);
			_healthBarText = GUIControl.CreateControlAbsolute<GUIStrokeTextLabel>(Vector2.zero, new Vector2(117f, 16f));
			_healthBarText.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(29, 82, 9), GUILabel.GenColor(29, 82, 9), new Vector2(-2f, 2f), TextAnchor.MiddleCenter);
			_healthBarText.Bold = true;
			Add(_healthBarText);
			_healthBarLocks = new GUIDrawTexture[_MaxHealthBarLocks];
			float num = 159f;
			float y = 10f;
			int num2 = 0;
			while (num2 < _MaxHealthBarLocks)
			{
				string text = string.Empty;
				if (StatLevelReqsDefinition.Instance != null)
				{
					text = MySquadDataManager.GetUnlockAtLevelText(StatLevelReqsDefinition.Instance.GetLevelHealthRankIsUnlockedAt(num2 + 1));
				}
				CspUtils.DebugLog("SHSHealthBar " + (num2 + 1) + " " + text);
				_healthBarLocks[num2] = CreateLockIcon(SHSStatBar.LockIconSize, new Vector2(num, y), text);
				num2++;
				num += 25f;
			}
		}

		public void SetHealthBar(int healthBonus, string healthText, bool badgeOwned)
		{
			CspUtils.DebugLog("SetHealthBar " + healthBonus + " " + healthText + " " + badgeOwned);
			_healthBar.TextureSource = "mysquadgadget_bundle|mshs_mysquad_heroes_health_lv6";
			_healthBar.SetSize(230f * (float)healthBonus / (float)StatLevelReqsDefinition.HEALTH_RANK_COUNT, _healthBar.Texture.height);
			_healthBarText.Text = healthText;
			Vector2 size = _healthBar.Size;
			if (size.x < 40f)
			{
				GUIStrokeTextLabel healthBarText = _healthBarText;
				Vector2 size2 = _healthBar.Size;
				healthBarText.Size = new Vector2(40f, size2.y);
			}
			else
			{
				_healthBarText.Size = _healthBar.Size;
			}
			for (int i = 0; i < _MaxHealthBarLocks; i++)
			{
				if (i >= 3 && !badgeOwned)
				{
					_healthBarLocks[i].IsVisible = false;
				}
				else
				{
					_healthBarLocks[i].IsVisible = (i >= healthBonus);
				}
			}
		}

		public override Vector2 GetBarOffset()
		{
			return new Vector2(15f, 86f);
		}
	}
}
