using SHSConsole;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public class SHSCommandWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	public class CommandItemData : GUIListItem, IComparable<CommandItemData>
	{
		private GUIButton box;

		private GUILabel nameLabel;

		private GUITextField argLocation;

		private GUITextField editTarget;

		private string nameWOParams;

		public CommandItemData(string name, string discription, GUITextField editT, string nameWOParams, GUITextField argLoc)
		{
			argLocation = argLoc;
			editTarget = editT;
			this.nameWOParams = nameWOParams;
			box = new GUIButton();
			box.SetPosition(0f, 0f);
			box.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
			Add(box);
			nameLabel = new GUILabel();
			nameLabel.SetSize(new Vector2(1f, 0.5f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
			nameLabel.SetPosition(5f, 5f);
			nameLabel.Text = name;
			nameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
			Add(nameLabel);
			GUILabel gUILabel = new GUILabel();
			gUILabel.SetSize(new Vector2(1f, 0.5f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
			gUILabel.SetPosition(5f, 25f);
			gUILabel.Text = discription;
			gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
			Add(gUILabel);
		}

		private void targetSelected()
		{
			editTarget.Text = nameWOParams + " " + argLocation.Text;
			GUIManager.Instance.FocusManager.getFocus(editTarget);
		}

		public override void Draw(DrawModeSetting drawFlags)
		{
			base.Draw(drawFlags);
		}

		public override void DrawFinalize()
		{
			if (isSelected)
			{
				targetSelected();
				isSelected = false;
			}
			base.DrawFinalize();
		}

		public int CompareTo(CommandItemData other)
		{
			return nameLabel.Text.CompareTo(other.nameLabel.Text);
		}
	}

	private GUIListViewWindow<GUITextListItem> outputWindow;

	private GUITextField commandField;

	private List<string> commandHistory = new List<string>();

	private int commandHistoryIndex;

	private GUIListViewWindow<CommandItemData> commandList;

	private GUIControlWindow commandInputWindow;

	public SHSCommandWindow(string WindowName)
		: base(WindowName, null)
	{
		SetBackground(new Color(1f, 0.4f, 1f, 0.2f));
		commandInputWindow = new GUIControlWindow();
		commandInputWindow.SetSize(new Vector2(0.7f, 0.9f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		commandInputWindow.SetPosition(0f, 0f);
		Add(commandInputWindow);
		outputWindow = new GUIListViewWindow<GUITextListItem>();
		outputWindow.SetBackground(new Color(0.1f, 0.1f, 0.1f, 0.7f));
		outputWindow.SetPosition(10f, 10f);
		outputWindow.ListItemHeight = 20;
		outputWindow.SetSize(new Vector2(0.9f, 0.9f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		outputWindow.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Inherit;
		commandInputWindow.Add(outputWindow);
		slider = new GUISlider();
		slider.SetPositionAndSize(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, new Vector2(-20f, 25f), new Vector2(50f, 0.8f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Percentage);
		slider.Orientation = GUISlider.SliderOrientationEnum.Vertical;
		slider.Rotation = 0f;
		slider.Color = new Color(1f, 1f, 1f, 1f);
		slider.IsVisible = true;
		slider.IsEnabled = true;
		slider.Value = 0f;
		slider.Min = 0f;
		slider.Max = 100f;
		slider.StyleInfo = SHSInheritedStyleInfo.Instance;
		commandInputWindow.Add(slider);
		outputWindow.Slider = slider;
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(15f, -5f), new Vector2(0.7f, 20f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Absolute);
		gUIImage.TextureSource = "GUI/white2x2";
		gUIImage.Color = new Color(0f, 0.63f, 0.8f);
		gUIImage.Alpha = 0.4f;
		commandInputWindow.Add(gUIImage);
		commandField = new GUITextField();
		commandField.SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(15f, -5f), new Vector2(0.7f, 20f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Absolute);
		commandField.TextColor = Color.white;
		commandField.StyleInfo = SHSInheritedStyleInfo.Instance;
		commandField.ControlName = "CommandConsoleInputField";
		commandField.ClipboardCopyEnabled = true;
		commandInputWindow.Add(commandField);
		GUIButton gUIButton = new GUIButton();
		gUIButton.SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Percentage, new Vector2(0.7f, -0.01f), new Vector2(0.2f, 20f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Absolute);
		gUIButton.Text = "Enter";
		gUIButton.Click += enterButton_Click;
		gUIButton.Alpha = 1f;
		gUIButton.SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		commandInputWindow.Add(gUIButton);
		outputWindow.AddItem(new GUITextListItem("-----------------------------------------------------------------------------------------"));
		outputWindow.AddItem(new GUITextListItem("Welcome to the Super Hero Squad console window!"));
		outputWindow.AddItem(new GUITextListItem("Type '?' or 'help' for a list of commands."));
		outputWindow.AddItem(new GUITextListItem("-----------------------------------------------------------------------------------------"));
		commandList = new GUIListViewWindow<CommandItemData>();
		commandList.SetBackground(new Color(0.1f, 0.1f, 0.1f, 0.7f));
		commandList.ListItemHeight = 20;
		commandList.SetSize(new Vector2(0.3f, 0.9f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		commandList.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Inherit;
		Add(commandList);
		commandList.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, new Vector2(-20f, 25f));
		commandList.Orientation = GUIListViewWindow<CommandItemData>.ListViewOrientationEnum.Vertical;
		commandList.ListItemHeight = 55;
		commandList.ListItemWidth = 230;
		commandList.Padding = new Rect(12f, 0f, 0f, 0f);
		GUITextField gUITextField = new GUITextField();
		gUITextField.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, new Vector2(-60f, 0f));
		gUITextField.SetSize(new Vector2(0.2f, 0.1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		Add(gUITextField);
		List<CommandItemData> list = new List<CommandItemData>();
		foreach (List<MethodInfo> value in SHSConsole.Console.Inst.ConsoleMethods.Values)
		{
			foreach (MethodInfo item in value)
			{
				string empty = string.Empty;
				string text = string.Empty;
				empty = item.Name;
				string nameWOParams = empty.ToString();
				ParameterInfo[] parameters = item.GetParameters();
				if (parameters.Length == 0)
				{
					empty += "()";
				}
				else
				{
					empty += " (";
					bool flag = true;
					ParameterInfo[] array = parameters;
					foreach (ParameterInfo parameterInfo in array)
					{
						if (!flag)
						{
							empty += ", ";
						}
						empty = empty + parameterInfo.ParameterType.Name + " " + parameterInfo.Name;
						flag = false;
					}
					empty += ")";
				}
				DescriptionAttribute[] array2 = (DescriptionAttribute[])item.GetCustomAttributes(typeof(DescriptionAttribute), false);
				if (array2.Length > 0)
				{
					text += array2[0].Description;
				}
				list.Add(new CommandItemData(empty, text, commandField, nameWOParams, gUITextField));
			}
		}
		list.Sort();
		foreach (CommandItemData item2 in list)
		{
			commandList.AddItem(item2);
		}
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
		commandList.Slider = gUISlider;
	}

	private void OnScrollCmdHistory(SHSKeyCode code)
	{
		if (code.code == KeyCode.UpArrow && commandHistoryIndex > 0)
		{
			commandHistoryIndex--;
		}
		string text = (commandHistoryIndex >= commandHistory.Count) ? string.Empty : commandHistory[commandHistoryIndex];
		commandField.Text = text;
	}

	private void OnEnter(SHSKeyCode code)
	{
		onExecute();
	}

	private void enterButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		onExecute();
	}

	public override void OnShow()
	{
		base.OnShow();
		GUIManager.Instance.SetKeyboardFocus(commandField.ControlName);
	}

	public override void ConfigureKeyBanks()
	{
		base.ConfigureKeyBanks();
		keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.Return, false, false, false), OnEnter);
		keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.UpArrow, false, false, false), OnScrollCmdHistory);
		keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.DownArrow, false, false, false), OnScrollCmdHistory);
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		if (outputWindow != null)
		{
			outputWindow.ListItemWidth = (int)outputWindow.Rect.width;
		}
	}

	private void onExecute()
	{
		onExecute(false);
	}

	private void onExecute(bool UpdateHistory)
	{
		string text = commandField.Text.Trim();
		string text2 = SHSConsole.Console.Inst.ExecuteCommand(text);
		outputWindow.AddItem(new GUITextListItem("> " + text));
		string[] array = text2.Split('\n');
		string[] array2 = array;
		foreach (string text3 in array2)
		{
			outputWindow.AddItem(new GUITextListItem(text3));
		}
		commandField.Text = string.Empty;
		if (commandHistoryIndex == commandHistory.Count || text != commandHistory[commandHistoryIndex])
		{
			commandHistoryIndex = commandHistory.Count;
			commandHistory.Add(text);
		}
		commandHistoryIndex++;
		outputWindow.Slider.Value = outputWindow.Slider.Max;
	}
}
