using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SHSMySquadChallengeCheckboxContainer : GUISimpleControlWindow
{
	public class CheckboxInfo
	{
		[CompilerGenerated]
		private string _003CLabel_003Ek__BackingField;

		[CompilerGenerated]
		private bool _003CChecked_003Ek__BackingField;

		public string Label
		{
			[CompilerGenerated]
			get
			{
				return _003CLabel_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CLabel_003Ek__BackingField = value;
			}
		}

		public bool Checked
		{
			[CompilerGenerated]
			get
			{
				return _003CChecked_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CChecked_003Ek__BackingField = value;
			}
		}

		public CheckboxInfo(string label, bool check)
		{
			Label = label;
			Checked = check;
		}
	}

	protected const float HORZ_SPACING_WITH_LABELS = 130f;

	protected const float HORZ_SPACING_WITHOUT_LABELS = 50f;

	protected const float VERT_SPACING = 40f;

	protected const int ROWS = 3;

	protected const int COLS = 2;

	protected SHSMySquadChallengeCheckbox[] checkBoxes;

	public SHSMySquadChallengeCheckboxContainer(CheckboxInfo[] checkboxes, Vector2 size, bool withLabels)
	{
		SetSize(size);
		float num = (!withLabels) ? 50f : 130f;
		int num2 = Convert.ToInt32(Mathf.Floor(size.x / num));
		int num3 = Convert.ToInt32(Mathf.Floor(size.y / 40f));
		float num4 = 0f;
		if (!withLabels)
		{
			num4 = (float)(num2 - checkboxes.Length) * 25f;
			Offset = new Vector2(num4, 0f);
		}
		checkBoxes = new SHSMySquadChallengeCheckbox[checkboxes.Length];
		int num5 = 0;
		for (int i = 0; i < num3; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				num5 = i * num2 + j;
				if (num5 >= checkBoxes.Length)
				{
					break;
				}
				checkBoxes[num5] = new SHSMySquadChallengeCheckbox((!withLabels) ? null : checkboxes[num5].Label, checkboxes[num5].Label);
				if (checkBoxes.Length == 1 && withLabels)
				{
					checkBoxes[num5].ConfigureForSingleDisplay();
				}
				checkBoxes[num5].Checked = checkboxes[num5].Checked;
				checkBoxes[num5].SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(num * (float)j, 40f * (float)i));
				Add(checkBoxes[num5]);
			}
		}
	}
}
