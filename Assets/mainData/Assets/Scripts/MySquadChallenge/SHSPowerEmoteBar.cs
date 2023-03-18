using System.Collections.Generic;
using UnityEngine;

namespace MySquadChallenge
{
	public class SHSPowerEmoteBar : SHSStatBar
	{
		private GUIDrawTexture[] _powerEmotes;

		private GUIDrawTexture[] _lockIcons;

		private static readonly int _MaxPowerEmotes = 3;

		private static readonly Vector2[] _PowerEmoteSize = new Vector2[3]
		{
			new Vector2(60f, 52f),
			new Vector2(61f, 54f),
			new Vector2(67f, 57f)
		};

		public SHSPowerEmoteBar()
		{
			CreateBarTitle("#HEROLIST_POWEREMOTES");
			float num = 112f;
			float num2 = 54f;
			_powerEmotes = new GUIDrawTexture[_MaxPowerEmotes];
			int num3 = 0;
			while (num3 < _MaxPowerEmotes)
			{
				_powerEmotes[num3] = CreateIcon(_PowerEmoteSize[num3] * 0.91f, new Vector2(num, 3f), "mysquad_heroinfo_upgrades_power_emote_" + (num3 + 1));
				_powerEmotes[num3].ToolTip = new NamedToolTipInfo("#emotechat_pemote" + (num3 + 1), new Vector2(45f, 0f));
				_powerEmotes[num3].HitTestSize = new Vector2(0.925f, 0.925f);
				num3++;
				num += num2;
			}
			float num4 = 191f;
			Vector2 size = SHSStatBar.LockIconSize * 0.91f;
			_lockIcons = new GUIDrawTexture[_MaxPowerEmotes - 1];
			int num5 = 0;
			while (num5 < _MaxPowerEmotes - 1)
			{
				_lockIcons[num5] = CreateLockIcon(size, new Vector2(num4, 18f), string.Empty);
				num5++;
				num4 += num2;
			}
		}

		public void SetPowerEmotes(string hero, UserProfile profile)
		{
			List<EmotesDefinition.EmoteDefinition> emotesByCategory = EmotesDefinition.Instance.GetEmotesByCategory(EmotesDefinition.EmoteCategoriesEnum.PowerEmote);
			for (int i = 0; i < _MaxPowerEmotes; i++)
			{
				string failReason = string.Empty;
				bool flag = true;
				if (emotesByCategory.Count > i)
				{
					flag = EmotesDefinition.Instance.RequirementsCheck(emotesByCategory[i].id, hero, profile, out failReason);
				}
				_powerEmotes[i].IsEnabled = flag;
				if (i > 0)
				{
					_lockIcons[i - 1].IsVisible = !flag;
					CreateLockToolTip(_lockIcons[i - 1], failReason);
				}
			}
		}

		public override Vector2 GetBarOffset()
		{
			return new Vector2(15f, 251f);
		}
	}
}
