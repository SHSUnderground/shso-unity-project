using System;
using UnityEngine;

public class GUITBTimer : SHSTimerEx
{
	private GUIImage bgd;

	private GUIImage bgdMotion;

	private GUIImage bgd2;

	private GUILabel numbers;

	private GUILabel numberShadow;

	private GUIImage bgdHaze;

	private string timeText;

	private float lastTime;

	private float degreesPerSecond = 360f;

	public float StartingTime
	{
		get
		{
			return base.Duration;
		}
		set
		{
			base.Duration = value;
		}
	}

	public GUITBTimer()
	{
		SetSize(93f, 93f);
		bgd = new GUIImage();
		bgd.Rotation = 0f;
		bgd.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, Vector2.zero);
		bgd.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		bgd.Color = new Color(1f, 1f, 1f, 1f);
		bgd.IsVisible = true;
		bgd.Texture = (Texture2D)Resources.Load("toolbox_bundle|timer_1");
		bgdMotion = new GUIImage();
		bgdMotion.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, Vector2.zero);
		bgdMotion.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		bgdMotion.Rotation = 0f;
		bgdMotion.Color = new Color(1f, 1f, 1f, 1f);
		bgdMotion.IsVisible = true;
		bgdMotion.Texture = (Texture2D)Resources.Load("toolbox_bundle|timer_2");
		bgd2 = new GUIImage();
		bgd2.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, Vector2.zero);
		bgd2.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		bgd2.Rotation = 0f;
		bgd2.Color = new Color(1f, 1f, 1f, 1f);
		bgd2.IsVisible = true;
		bgd2.Texture = (Texture2D)Resources.Load("toolbox_bundle|timer_3");
		bgdHaze = new GUIImage();
		bgdHaze.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, Vector2.zero);
		bgdHaze.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		bgdHaze.Rotation = 0f;
		bgdHaze.Color = new Color(1f, 1f, 1f, 1f);
		bgdHaze.IsVisible = true;
		bgdHaze.Texture = (Texture2D)Resources.Load("toolbox_bundle|timer_4");
		numbers = new GUILabel();
		numbers.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, Vector2.zero);
		numbers.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		numbers.Text = "4";
		numbers.Rotation = 0f;
		numbers.IsVisible = true;
		numbers.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 52, GUILabel.GenColor(58, 71, 94), TextAnchor.MiddleCenter);
		numberShadow = new GUILabel();
		numberShadow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(2f, 2f));
		numberShadow.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		numberShadow.IsVisible = true;
		numberShadow.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 21, Color.white, TextAnchor.UpperLeft);
		Add(bgd);
		Add(bgdMotion);
		Add(bgd2);
		Add(numberShadow);
		Add(numbers);
		Add(bgdHaze);
	}

	public override void DrawPreprocess()
	{
		base.DrawPreprocess();
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		timeText = Convert.ToInt32(base.TimeLeft).ToString();
		if (base.TimerState == TimerStateEnum.Idle)
		{
			timeText = Convert.ToInt32(base.Duration).ToString();
		}
		numbers.Text = timeText;
		numberShadow.Text = timeText;
		if (base.TimerState == TimerStateEnum.Running)
		{
			float time = Time.time;
			float num = time - lastTime;
			lastTime = time;
			bgdMotion.Rotation += degreesPerSecond * num;
		}
	}
}
