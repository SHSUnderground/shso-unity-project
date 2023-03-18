using UnityEngine;

namespace MySquadChallenge
{
	public class SHSHealthBar : SHSStatBar
	{
		private GUIImage _healthBar;

		private GUIStrokeTextLabel _healthBarText;

		private GUIDrawTexture[] _healthBarLocks;

		private static int _MaxHealthBarLocks = 3;

		private int healthBonus;

		private bool badgeOwned;

		private GUIDrawTexture _heroLore;

		public SHSHealthBar()
		{
			_MaxHealthBarLocks = StatLevelReqsDefinition.HEALTH_RANK_COUNT;
			GUIStrokeTextLabel gUIStrokeTextLabel = GUIControl.CreateControlAbsolute<GUIStrokeTextLabel>(GetBarSize(), new Vector2(13f, 114f));
			gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(134, 208, 255), GUILabel.GenColor(0, 33, 101), GUILabel.GenColor(1, 33, 85), new Vector2(-2f, 2f), TextAnchor.UpperLeft);
			gUIStrokeTextLabel.Text = "#HEROLIST_HEALTH";
			gUIStrokeTextLabel.Bold = true;
			Add(gUIStrokeTextLabel);
			_healthBar = GUIControl.CreateControlAbsolute<GUIImage>(Vector2.zero, new Vector2(117f, 114f));
			Add(_healthBar);
			_healthBarText = GUIControl.CreateControlAbsolute<GUIStrokeTextLabel>(Vector2.zero, new Vector2(117f, 114f));
			_healthBarText.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(29, 82, 9), GUILabel.GenColor(29, 82, 9), new Vector2(-2f, 2f), TextAnchor.MiddleCenter);
			_healthBarText.Bold = true;
			Add(_healthBarText);
			_healthBarLocks = new GUIDrawTexture[_MaxHealthBarLocks];
			float num = 154f;
			float y = 105f;
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
			_heroLore = GUIControl.CreateControlAbsolute<GUIDrawTexture>(new Vector2(82f, 33f), new Vector2(280f, 25f));
			_heroLore.ToolTip = new NamedToolTipInfo("#HEROLIST_LOREVALUE", new Vector2(-55f, 0f));
			Add(_heroLore);
		}

		public void SetHealthBar(string hero, int healthBonus, string healthText, bool badgeOwned)
		{
			this.healthBonus = healthBonus;
			this.badgeOwned = badgeOwned;
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
			if (_heroLore != null)
			{
				int heroLore = AppShell.Instance.HeroLoreManager.GetHeroLore(hero);
				if (heroLore < 0 || heroLore > 3)
				{
					CspUtils.DebugLog("SHSHeroProfileWindow::SetHeroLore() - hiding lore icon due to unexpected lore value <" + heroLore + ">");
					_heroLore.IsVisible = false;
				}
				else
				{
					_heroLore.IsVisible = true;
					_heroLore.TextureSource = "common_bundle|marvel_lore_icon_mysquad_plus" + heroLore;
				}
			}
		}

		public override Vector2 GetBarOffset()
		{
			return new Vector2(15f, -10f);
		}

		public override Vector2 GetBarSize()
		{
			return new Vector2(360f, 398f);
		}

		public override void SetVisible(bool visible)
		{
			base.SetVisible(visible);
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
	}
}
