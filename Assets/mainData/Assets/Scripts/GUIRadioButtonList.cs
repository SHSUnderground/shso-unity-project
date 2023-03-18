using System.Collections.Generic;

public class GUIRadioButtonList : GUIControlWindow
{
	private List<GUIToggleButton> radioButtonList;

	public GUIToggleButton this[int index]
	{
		get
		{
			return radioButtonList[index];
		}
	}

	public GUIToggleButton SelectedButton
	{
		get
		{
			foreach (GUIToggleButton radioButton in radioButtonList)
			{
				if (radioButton.Value)
				{
					return radioButton;
				}
			}
			return null;
		}
	}

	public GUIRadioButtonList()
	{
		radioButtonList = new List<GUIToggleButton>();
	}

	public void AddButton(GUIToggleButton button)
	{
		radioButtonList.Add(button);
		Add(button);
		button.toggleButton.Click -= button.OnClickDelegate;
		button.toggleButton.Click += delegate
		{
			if (!button.Value)
			{
				foreach (GUIToggleButton radioButton in radioButtonList)
				{
					if (radioButton != button)
					{
						radioButton.Value = false;
					}
				}
				button.Value = true;
			}
		};
		if (radioButtonList.Count == 1)
		{
			button.Value = true;
		}
	}

	public void Select(int index)
	{
		UnselectAll();
		if (radioButtonList[index] != null)
		{
			radioButtonList[index].Value = true;
		}
	}

	public void UnselectAll()
	{
		foreach (GUIToggleButton radioButton in radioButtonList)
		{
			radioButton.Value = false;
		}
	}
}
