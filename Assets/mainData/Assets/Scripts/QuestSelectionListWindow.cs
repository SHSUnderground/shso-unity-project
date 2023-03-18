using UnityEngine;

public class QuestSelectionListWindow : SHSSelectionWindow<QuestItem, GUISimpleControlWindow>
{
	public QuestSelectionListWindow(GUISlider slider)
		: base(slider, 220f, new Vector2(220f, 78f), 10)
	{
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		slider.FireChanged();
	}

	public void Sort(string sort)
	{
		bool flag = false;
		while (!flag)
		{
			foreach (QuestItem item in items)
			{
				item.active = item.QuestName.Contains(sort);
				if (item.active)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				if (sort == string.Empty)
				{
					return;
				}
				sort = sort.Substring(0, sort.Length - 1);
			}
		}
		RequestARefresh();
	}
}
