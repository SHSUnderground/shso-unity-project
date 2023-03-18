using UnityEngine;

public class MyHeroesPowerAttackBar : IconBar
{
	private GUIDrawTexture[] _icons;

	private GUIDrawTexture[] _badgeIcons;

	private GUIDrawTexture[] _lockIcons;

	private int _maxIcons = 3;

	private static readonly Vector2 _iconSize = new Vector2(64f, 64f);

	public MyHeroesPowerAttackBar()
	{
		float num = 40f;
		float num2 = num;
		float num3 = 54f;
		_maxIcons = StatLevelReqsDefinition.POWER_ATTACKS_RANK_COUNT;
		_icons = new GUIDrawTexture[_maxIcons];
		_badgeIcons = new GUIDrawTexture[_maxIcons];
		int num4 = 0;
		while (num4 < _maxIcons)
		{
			_icons[num4] = CreateIcon(_iconSize * 0.91f, new Vector2(num2, 1f), "mysquad_heroinfo_upgrades_power_attack_" + (num4 + 1));
			_icons[num4].HitTestSize = new Vector2(0.925f, 0.925f);
			num4++;
			num2 += num3;
		}
		float num5 = num + 30f;
		Vector2 size = IconBar.LockIconSize * 0.91f;
		_lockIcons = new GUIDrawTexture[_maxIcons];
		int num6 = 0;
		while (num6 < _maxIcons)
		{
			_badgeIcons[num6] = CreateIcon(IconBar.LockIconSize, new Vector2(num5 - 4f, 16f));
			_badgeIcons[num6].TextureSource = "shopping_bundle|badge";
			_lockIcons[num6] = CreateLockIcon(size, new Vector2(num5, 18f), string.Empty);
			num6++;
			num5 += num3;
		}
	}

	public void config(string hero, int whatPowerAttack, int heroLevel)
	{
		for (int i = 1; i <= _maxIcons; i++)
		{
			string empty = string.Empty;
			int num = -1;
			if (whatPowerAttack == 0)
			{
				empty = "mysquad_heroinfo_upgrades_hero_up_" + i;
				num = StatLevelReqsDefinition.Instance.GetLevelHeroupRankIsUnlockedAt(i);
			}
			else
			{
				empty = "mysquad_heroinfo_upgrades_power_attack_" + whatPowerAttack;
				if (i >= 2)
				{
					empty += "plus";
				}
				if (i >= 3)
				{
					empty += "2";
				}
				num = StatLevelReqsDefinition.Instance.GetLevelPowerAttackRankIsUnlockedAt(whatPowerAttack, i);
			}
			_icons[i - 1].TextureSource = "mysquadgadget_bundle|" + empty;
			_icons[i - 1].ToolTip = new NamedToolTipInfo(MySquadDataManager.GetPowerAttackName(hero, whatPowerAttack, i), new Vector2(45f, 0f));
			if (num == -1)
			{
				CspUtils.DebugLog("got a -1 for power attack " + whatPowerAttack + "  at rank " + 3 + " (power attack 0 is the hero up)");
			}
			if (num <= heroLevel)
			{
				_lockIcons[i - 1].IsVisible = false;
				_badgeIcons[i - 1].IsVisible = false;
				_icons[i - 1].IsEnabled = true;
				continue;
			}
			_lockIcons[i - 1].IsVisible = true;
			_icons[i - 1].IsEnabled = false;
			if (num > 11)
			{
				_badgeIcons[i - 1].IsVisible = true;
				_lockIcons[i - 1].ToolTip = new NamedToolTipInfo("#TT_HERO_BADGE_NOT_OWNED", new Vector2(45f, 0f));
			}
			else
			{
				_badgeIcons[i - 1].IsVisible = false;
				_lockIcons[i - 1].ToolTip = new NamedToolTipInfo(MySquadDataManager.GetUnlockAtLevelText(num), new Vector2(45f, 0f));
			}
		}
	}
}
