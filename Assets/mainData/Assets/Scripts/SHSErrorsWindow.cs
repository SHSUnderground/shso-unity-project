using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSErrorsWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	public class ErrorItemData : GUIListItem, IComparable<ErrorItemData>
	{
		private SHSErrorCodes.Response response;

		private GUIButton box;

		private GUILabel numberLabel;

		private GUILabel descriptionLabel;

		public ErrorItemData(SHSErrorCodes.Response response)
		{
			this.response = response;
			box = new GUIButton();
			box.SetPosition(0f, 0f);
			box.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
			Add(box);
			numberLabel = new GUILabel();
			numberLabel.SetSize(new Vector2(1f, 0.5f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
			numberLabel.SetPosition(5f, 5f);
			numberLabel.Text = string.Format("{0} ({1})", response.Number.ToString(), response.Code.ToString());
			numberLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
			Add(numberLabel);
			descriptionLabel = new GUILabel();
			descriptionLabel.SetSize(new Vector2(1f, 0.5f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
			descriptionLabel.SetPosition(45f, 25f);
			descriptionLabel.Text = response.getMessage();
			descriptionLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
			Add(descriptionLabel);
		}

		public override void DrawFinalize()
		{
			if (isSelected)
			{
				CspUtils.DebugLog("Console forced error: " + descriptionLabel.Text);
				AppShell.Instance.CriticalError(response.Code, "Forced error by Console");
				isSelected = false;
			}
			base.DrawFinalize();
		}

		public int CompareTo(ErrorItemData other)
		{
			return response.Number.CompareTo(other.response.Number);
		}
	}

	private GUIListViewWindow<ErrorItemData> errorList;

	public SHSErrorsWindow(string Name)
		: this(Name, null)
	{
	}

	public SHSErrorsWindow(string Name, GUISlider slider)
		: base(Name, slider)
	{
		errorList = new GUIListViewWindow<ErrorItemData>();
		errorList.SetBackground(new Color(0.1f, 0.1f, 0.1f, 0.7f));
		errorList.ListItemHeight = 20;
		errorList.SetSize(new Vector2(470f, 0.9f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Percentage);
		errorList.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Inherit;
		Add(errorList);
		errorList.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(20f, 25f));
		errorList.Orientation = GUIListViewWindow<ErrorItemData>.ListViewOrientationEnum.Vertical;
		errorList.ListItemHeight = 55;
		errorList.ListItemWidth = 460;
		errorList.Padding = new Rect(12f, 0f, 0f, 0f);
		GUISlider gUISlider = new GUISlider();
		gUISlider.SetPositionAndSize(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, new Vector2(-20f, 25f), new Vector2(50f, 0.8f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Percentage);
		gUISlider.Orientation = GUISlider.SliderOrientationEnum.Vertical;
		gUISlider.Rotation = 0f;
		gUISlider.Color = new Color(1f, 1f, 1f, 1f);
		gUISlider.IsVisible = true;
		gUISlider.IsEnabled = true;
		gUISlider.Value = 0f;
		gUISlider.Min = 0f;
		gUISlider.Max = 100f;
		gUISlider.StyleInfo = SHSInheritedStyleInfo.Instance;
		Add(gUISlider);
		errorList.Slider = gUISlider;
	}

	~SHSErrorsWindow()
	{
	}

	public override void OnShow()
	{
		base.OnShow();
		List<ErrorItemData> list = new List<ErrorItemData>();
		foreach (int value in Enum.GetValues(typeof(SHSErrorCodes.Code)))
		{
			SHSErrorCodes.Response response = SHSErrorCodes.GetResponse((SHSErrorCodes.Code)value);
			if (response != null)
			{
				list.Add(new ErrorItemData(response));
			}
		}
		list.Sort();
		foreach (ErrorItemData item in list)
		{
			errorList.AddItem(item);
		}
	}
}
