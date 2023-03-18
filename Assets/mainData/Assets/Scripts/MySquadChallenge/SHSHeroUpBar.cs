using UnityEngine;

namespace MySquadChallenge
{
	public class SHSHeroUpBar : SHSStatBar
	{
		private GUIDrawTexture _advHeroUp;

		private GUIDrawTexture _lock;

		private GUIDrawTexture _advHeroUp2;

		private GUIDrawTexture _lock2;

		private static readonly Vector2 _HeroUpIconSize = new Vector2(58f, 54f);

		public SHSHeroUpBar()
		{
			CreateBarTitle("#HEROLIST_HEROUP");
			Vector2 size = _HeroUpIconSize * 0.91f;
			GUIDrawTexture gUIDrawTexture = CreateIcon(size, new Vector2(112f, 0f), "mysquad_heroinfo_upgrades_hero_up_1");
			gUIDrawTexture.ToolTip = new NamedToolTipInfo("#HeroUp");
			_advHeroUp = CreateIcon(size, new Vector2(165f, 0f), "mysquad_heroinfo_upgrades_hero_up_2");
			_advHeroUp.ToolTip = new NamedToolTipInfo("#UPGRADEDHEROUP", new Vector2(18f, 0f));
			_lock = CreateLockIcon(SHSStatBar.LockIconSize * 0.91f, new Vector2(190f, 18f), MySquadDataManager.GetUnlockAtLevelText(StatLevelReqsDefinition.Instance.GetLevelHeroupRankIsUnlockedAt(2)));
			_advHeroUp2 = CreateIcon(size, new Vector2(218f, 0f), "mysquad_heroinfo_upgrades_hero_up_3");
			_advHeroUp2.ToolTip = new NamedToolTipInfo("#UPGRADEDHEROUP2", new Vector2(18f, 0f));
			_lock2 = CreateLockIcon(SHSStatBar.LockIconSize * 0.91f, new Vector2(243f, 18f), MySquadDataManager.GetUnlockAtLevelText(StatLevelReqsDefinition.Instance.GetLevelHeroupRankIsUnlockedAt(3)));
		}

		public void SetHeroUp(int level, bool badgeOwned)
		{
			int numberOfHeroupRanksForLevel = StatLevelReqsDefinition.Instance.GetNumberOfHeroupRanksForLevel(level);
			if (numberOfHeroupRanksForLevel >= 2)
			{
				_advHeroUp.IsEnabled = true;
				_lock.IsVisible = false;
			}
			else
			{
				_advHeroUp.IsEnabled = false;
				_lock.IsVisible = true;
			}
			if (numberOfHeroupRanksForLevel >= 3)
			{
				_advHeroUp2.IsEnabled = true;
				_lock2.IsVisible = false;
			}
			else if (badgeOwned)
			{
				_advHeroUp2.IsVisible = true;
				_advHeroUp2.IsEnabled = false;
				_lock2.IsVisible = true;
			}
			else
			{
				_advHeroUp2.IsVisible = false;
				_lock2.IsVisible = false;
			}
		}

		public override Vector2 GetBarOffset()
		{
			return new Vector2(15f, 209f);
		}
	}
}
