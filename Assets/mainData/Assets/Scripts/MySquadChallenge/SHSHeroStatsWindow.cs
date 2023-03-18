using System;
using System.Collections.Generic;

namespace MySquadChallenge
{
	public class SHSHeroStatsWindow : GUISimpleControlWindow
	{
		private MySquadDataManager _dataManager;

		private SHSStatBar[] _statBars;

		private static readonly Type[] _statBarTypes = new Type[7]
		{
			typeof(SHSXpBar),
			typeof(SHSHealthBar),
			typeof(SHSPowerAttackBar),
			typeof(SHSHeroUpBar),
			typeof(SHSPowerEmoteBar),
			typeof(SHSMedalBar),
			typeof(SHSBadgeBar)
		};

		private static readonly HashSet<Type> _InfoStatBars;

		private static readonly HashSet<Type> _PowersStatBars;

		private static readonly HashSet<Type> _MedalsStatBars;

		private GUIButton InfoButton;

		private GUIButton PowersButton;

		private GUIButton MedalsButton;

		private GUIStrokeTextLabel InfoLabel;

		private GUIStrokeTextLabel PowersLabel;

		private GUIStrokeTextLabel MedalsLabel;

		public SHSHeroStatsWindow(MySquadDataManager dataManager)
		{
			_dataManager = dataManager;
			SetSize(386f, 419f);
			SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
			_statBars = new SHSStatBar[_statBarTypes.Length];
			for (int i = 0; i < _statBars.Length; i++)
			{
				SHSStatBar sHSStatBar = Activator.CreateInstance(_statBarTypes[i]) as SHSStatBar;
				if (sHSStatBar != null)
				{
					sHSStatBar.SetSize(sHSStatBar.GetBarSize());
					sHSStatBar.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, sHSStatBar.GetBarOffset());
					Add(sHSStatBar);
					_statBars[i] = sHSStatBar;
				}
			}
		}

		static SHSHeroStatsWindow()
		{
			HashSet<Type> hashSet = new HashSet<Type>();
			hashSet.Add(typeof(SHSXpBar));
			hashSet.Add(typeof(SHSHealthBar));
			hashSet.Add(typeof(SHSPowerEmoteBar));
			_InfoStatBars = hashSet;
			hashSet = new HashSet<Type>();
			hashSet.Add(typeof(SHSPowerAttackBar));
			hashSet.Add(typeof(SHSHeroUpBar));
			hashSet.Add(typeof(SHSBadgeBar));
			_PowersStatBars = hashSet;
			hashSet = new HashSet<Type>();
			hashSet.Add(typeof(SHSMedalBar));
			_MedalsStatBars = hashSet;
		}

		public void SelectHero(string hero)
		{
			int heroLevel = 0;
			if (_dataManager.Profile.AvailableCostumes.ContainsKey(hero))
			{
				heroLevel = _dataManager.Profile.AvailableCostumes[hero].Level;
			}
			SHSBadgeBar statBar = GetStatBar<SHSBadgeBar>();
			SetBadgeBar(hero);
			SetXpBar(hero);
			SetHealthBar(hero, heroLevel, !statBar.LocksVisible());
			SetPowerAttackBar(hero, heroLevel, !statBar.LocksVisible());
			SetHeroUpBar(heroLevel, !statBar.LocksVisible());
			SetPowerEmoteBar(hero, _dataManager.Profile);
			SetMedalBar(hero, _dataManager.Profile.CounterBank);
		}

		public T GetStatBar<T>() where T : SHSStatBar
		{
			if (_statBars == null)
			{
				return (T)null;
			}
			SHSStatBar[] statBars = _statBars;
			foreach (SHSStatBar sHSStatBar in statBars)
			{
				if (sHSStatBar.GetType() == typeof(T))
				{
					return sHSStatBar as T;
				}
			}
			return (T)null;
		}

		private void SetBadgeBar(string hero)
		{
			SHSBadgeBar statBar = GetStatBar<SHSBadgeBar>();
			statBar.SetHero(hero);
		}

		private void SetXpBar(string hero)
		{
			SHSXpBar statBar = GetStatBar<SHSXpBar>();
			statBar.SetExperience(MySquadDataManager.GetPercToNextLevel(_dataManager.Profile, hero), MySquadDataManager.GetExpTextRaw(_dataManager.Profile, hero));
		}

		private void SetHealthBar(string hero, int heroLevel, bool badgeOwned)
		{
			int numberOfHealthRanksForLevel = StatLevelReqsDefinition.Instance.GetNumberOfHealthRanksForLevel(heroLevel);
			SHSHealthBar statBar = GetStatBar<SHSHealthBar>();
			statBar.SetHealthBar(hero, numberOfHealthRanksForLevel, MySquadDataManager.GetHealthLevelText(numberOfHealthRanksForLevel), badgeOwned);
		}

		private void SetPowerAttackBar(string hero, int heroLevel, bool badgeOwned)
		{
			SHSPowerAttackBar statBar = GetStatBar<SHSPowerAttackBar>();
			statBar.SetPowerAttacks(hero, heroLevel, badgeOwned);
		}

		private void SetHeroUpBar(int heroLevel, bool badgeOwned)
		{
			SHSHeroUpBar statBar = GetStatBar<SHSHeroUpBar>();
			statBar.SetHeroUp(heroLevel, badgeOwned);
		}

		private void SetPowerEmoteBar(string hero, UserProfile profile)
		{
			SHSPowerEmoteBar statBar = GetStatBar<SHSPowerEmoteBar>();
			statBar.SetPowerEmotes(hero, profile);
		}

		private void SetHeroItemBar(string hero, int heroLevel)
		{
			SHSHeroItemBar statBar = GetStatBar<SHSHeroItemBar>();
			statBar.SetHeroItems(hero, heroLevel);
		}

		private void SetMedalBar(string hero, SHSCounterBank achievementBank)
		{
			SHSMedalBar statBar = GetStatBar<SHSMedalBar>();
			statBar.SetMedals(hero, achievementBank);
		}

		private void InfoButtonClicked(GUIControl sender, GUIClickEvent EventData)
		{
			SHSStatBar[] statBars = _statBars;
			foreach (SHSStatBar sHSStatBar in statBars)
			{
				if (_InfoStatBars.Contains(sHSStatBar.GetType()))
				{
					sHSStatBar.IsVisible = true;
				}
				else
				{
					sHSStatBar.IsVisible = false;
				}
			}
		}

		private void PowersButtonClicked(GUIControl sender, GUIClickEvent EventData)
		{
			SHSStatBar[] statBars = _statBars;
			foreach (SHSStatBar sHSStatBar in statBars)
			{
				if (_PowersStatBars.Contains(sHSStatBar.GetType()))
				{
					sHSStatBar.IsVisible = true;
				}
				else
				{
					sHSStatBar.IsVisible = false;
				}
			}
		}

		private void MedalsButtonClicked(GUIControl sender, GUIClickEvent EventData)
		{
			SHSStatBar[] statBars = _statBars;
			foreach (SHSStatBar sHSStatBar in statBars)
			{
				if (_MedalsStatBars.Contains(sHSStatBar.GetType()))
				{
					sHSStatBar.IsVisible = true;
				}
				else
				{
					sHSStatBar.IsVisible = false;
				}
			}
		}
	}
}
