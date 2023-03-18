namespace MySquadChallenge
{
	public class SHSHeroSelectionWindow : SHSSelectionWindow<SHSHeroSelectionItem, SHSItemLoadingWindow>
	{
		public SHSHeroSelectionWindow(GUISlider slider)
			: base(slider, SelectionWindowType.ThreeAcross)
		{
			SetSize(340f, 365f);
			SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		}

		public static string GetSearchName(string name)
		{
			name = name.ToLowerInvariant();
			name = name.Replace("_", string.Empty);
			name = name.Replace("'", string.Empty);
			name = name.Replace("-", string.Empty);
			name = name.Replace(" ", string.Empty);
			name = name.Replace(".", string.Empty);
			name = name.Replace("!", string.Empty);
			return name;
		}

		public void Sort(string sort)
		{
			string text = GetSearchName(sort);
			bool flag = false;
			while (!flag)
			{
				foreach (SHSHeroSelectionItem item in items)
				{
					item.active = item.NameMatch(text);
					if (item.active)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					if (text == string.Empty)
					{
						return;
					}
					text = text.Substring(0, text.Length - 1);
				}
			}
			RequestARefresh();
		}

		public SHSHeroSelectionItem FindHeroSelectionItem(string heroName)
		{
			return Find(delegate(SHSHeroSelectionItem item)
			{
				return item.HeroName == heroName;
			});
		}
	}
}
