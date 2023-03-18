using UnityEngine;

namespace MySquadChallenge
{
	public class SHSPowerAttackBar : SHSStatBar
	{
		private GUIDrawTexture[] _attackIcons;

		private GUIDrawTexture[] _lockIcons;

		private Vector2[] _displayInfo;

		private static readonly int _MaxAttackIcons = 9;

		private static readonly Vector2 _PowerAttackIconSize = new Vector2(64f, 64f);

		public SHSPowerAttackBar()
		{
			CreateBarTitle("#HEROLIST_POWERATTACKS");
			float num = 102f;
			float num2 = 132f;
			float num3 = 0f;
			float num4 = 40f;
			_displayInfo = new Vector2[_MaxAttackIcons];
			_displayInfo[0] = new Vector2(1f, 1f);
			_displayInfo[1] = new Vector2(2f, 1f);
			_displayInfo[2] = new Vector2(1f, 2f);
			_displayInfo[3] = new Vector2(3f, 1f);
			_displayInfo[4] = new Vector2(2f, 2f);
			_displayInfo[5] = new Vector2(3f, 2f);
			_displayInfo[6] = new Vector2(1f, 3f);
			_displayInfo[7] = new Vector2(2f, 3f);
			_displayInfo[8] = new Vector2(3f, 3f);
			Vector2 size = _PowerAttackIconSize * 0.91f;
			Vector2 size2 = SHSStatBar.LockIconSize * 0.91f;
			_attackIcons = new GUIDrawTexture[_MaxAttackIcons];
			_lockIcons = new GUIDrawTexture[_MaxAttackIcons];
			for (int i = 0; i < _MaxAttackIcons; i++)
			{
				Vector2 vector = _displayInfo[i];
				if (i != 0 && i % 3 == 0)
				{
					num = 102f + num4 * 0f;
					num2 = 132f + num4 * 0f;
					num3 += 38f;
				}
				else if (i == 6)
				{
					num = 102f + num4 * 0f;
					num2 = 132f + num4 * 0f;
					num3 += 38f;
				}
				string text = "mysquad_heroinfo_upgrades_power_attack_" + vector.x;
				if (vector.y >= 2f)
				{
					text += "plus";
				}
				if (vector.y >= 3f)
				{
					text += "2";
				}
				_attackIcons[i] = CreateIcon(size, new Vector2(num, num3), text);
				_attackIcons[i].HitTestSize = new Vector2(0.65f, 0.65f);
				int num5 = -1;
				num5 = StatLevelReqsDefinition.Instance.GetLevelPowerAttackRankIsUnlockedAt((int)vector.x, (int)vector.y);
				if (num5 == -1)
				{
					CspUtils.DebugLog("got a -1 for GetLevelPowerAttackRankIsUnlockedAt");
				}
				string unlockAtLevelText = MySquadDataManager.GetUnlockAtLevelText(num5);
				_lockIcons[i] = CreateLockIcon(size2, new Vector2(num2, num3 + 18f), unlockAtLevelText);
				num += num4;
				num2 += num4;
			}
		}

		public void SetPowerAttacks(string hero, int level, bool badgeOwned)
		{
			for (int i = 0; i < _MaxAttackIcons; i++)
			{
				Vector2 vector = _displayInfo[i];
				bool flag = level >= StatLevelReqsDefinition.Instance.GetLevelPowerAttackRankIsUnlockedAt((int)vector.x, (int)vector.y);
				if (badgeOwned || i < 6)
				{
					_attackIcons[i].IsVisible = true;
					_attackIcons[i].IsEnabled = flag;
					_lockIcons[i].IsVisible = !flag;
					_attackIcons[i].ToolTip = new NamedToolTipInfo(MySquadDataManager.GetPowerAttackName(hero, (int)vector.x, (int)vector.y));
				}
				else
				{
					_attackIcons[i].IsVisible = false;
					_lockIcons[i].IsVisible = false;
				}
			}
		}

		public override Vector2 GetBarSize()
		{
			return new Vector2(360f, 110f);
		}

		public override Vector2 GetBarOffset()
		{
			return new Vector2(15f, 127f);
		}

		private bool IsPrimaryAttack(int index)
		{
			return 4 % (index + 1) == 0;
		}
	}
}
