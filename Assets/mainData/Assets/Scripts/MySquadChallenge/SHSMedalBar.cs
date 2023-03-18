using System.Collections.Generic;
using UnityEngine;

namespace MySquadChallenge
{
	public class SHSMedalBar : SHSStatBar
	{
		private GUIDrawTexture[] _medalIcons;

		private List<SHSCounterAchievement> _medals;

		private static readonly int _MedalsPerRow = 5;

		private static readonly Vector2 _MedalIconSize = new Vector2(117f, 107f);

		public SHSMedalBar()
		{
			CreateBarTitle("#HEROLIST_MEDALS", new Vector2(13f, -19f));
			_medals = new List<SHSCounterAchievement>();
			foreach (Achievement value in AppShell.Instance.AchievementsManager.Achievements.Values)
			{
				if (value is SHSCounterAchievement)
				{
					_medals.Add(value as SHSCounterAchievement);
				}
			}
			_medals.Sort(delegate(SHSCounterAchievement a, SHSCounterAchievement b)
			{
				return a.ShortDescription.CompareTo(b.ShortDescription);
			});
			int num = _medals.Count / _MedalsPerRow;
			float num2 = 15f;
			float num3 = 49f;
			float num4 = 45f;
			Vector2 size = _MedalIconSize * 0.4f;
			_medalIcons = new GUIDrawTexture[_medals.Count];
			int num5 = 0;
			while (num5 < num)
			{
				float num6 = 110f;
				int num7 = 0;
				while (num7 < _MedalsPerRow)
				{
					int num8 = num5 * _MedalsPerRow + num7;
					if (num8 >= _medalIcons.Length)
					{
						break;
					}
					_medalIcons[num8] = CreateIcon(size, new Vector2(num6, num2));
					SetIconTexture(_medalIcons[num8], "notification_bundle", _medals[num8].Id + "_" + Achievement.AchievementLevelEnum.Adamantium.ToString());
					num7++;
					num6 += num3;
				}
				num5++;
				num2 += num4;
			}
		}

		public void SetMedals(string hero, SHSCounterBank achievementBank)
		{
			for (int i = 0; i < _medals.Count; i++)
			{
				SHSCounterAchievement sHSCounterAchievement = _medals[i];
				Achievement.AchievementLevelEnum level = sHSCounterAchievement.GetLevel(hero, achievementBank);
				string source = string.Empty;
				switch (level)
				{
				case Achievement.AchievementLevelEnum.NotAchieved:
					source = sHSCounterAchievement.Id + "_" + Achievement.AchievementLevelEnum.Bronze.ToString();
					break;
				default:
					source = sHSCounterAchievement.Id + "_" + level.ToString();
					break;
				case Achievement.AchievementLevelEnum.Unknown:
					break;
				}
				SetIconTexture(_medalIcons[i], "notification_bundle", source);
				_medalIcons[i].IsEnabled = (level != Achievement.AchievementLevelEnum.NotAchieved);
				HoverHelpInfo hoverHelpInfo = new AchievementHoverHelpInfo(sHSCounterAchievement, achievementBank, hero, Achievement.GetNextHighestLevel(level));
				hoverHelpInfo.extendLeft = true;
				hoverHelpInfo.horizontalFlipOverride = HoverHelpInfo.FlipOverride.On;
				hoverHelpInfo.verticalFlipOverride = HoverHelpInfo.FlipOverride.Off;
				_medalIcons[i].ToolTip = hoverHelpInfo;
			}
		}

		public override Vector2 GetBarOffset()
		{
			return new Vector2(15f, 292f);
		}

		public override Vector2 GetBarSize()
		{
			return new Vector2(360f, 98f);
		}
	}
}
