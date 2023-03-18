using System;
using UnityEngine;

public class GUITBTimerCounter : SHSTimerEx
{
	private GUIImage bgd;

	private GUIImage bgdMotion;

	private GUIImage bgd2;

	private GUILabel numbers;

	private GUILabel numberShadow;

	private GUILabel counter;

	private GUILabel counterShadow;

	private GUIImage bgdHaze;

	private string timeText;

	private int count;

	private int countTotal;

	private float lastTime;

	private float degreesPerSecond = 360f;

	public int Count
	{
		get
		{
			return count;
		}
		set
		{
			count = value;
		}
	}

	public int CountTotal
	{
		get
		{
			return countTotal;
		}
		set
		{
			countTotal = value;
		}
	}

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

	public GUITBTimerCounter()
	{
		SetSize(93f, 93f);
		bgd = new GUIImage();
		bgd.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, Vector2.zero);
		bgd.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		bgd.Texture = (Texture2D)Resources.Load("toolbox_bundle|timer_1");
		bgdMotion = new GUIImage();
		bgdMotion.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, Vector2.zero);
		bgdMotion.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		bgdMotion.Texture = (Texture2D)Resources.Load("toolbox_bundle|timer_2");
		bgd2 = new GUIImage();
		bgd2.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, Vector2.zero);
		bgd2.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		bgd2.Texture = (Texture2D)Resources.Load("toolbox_bundle|timer_3");
		bgdHaze = new GUIImage();
		bgdHaze.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, Vector2.zero);
		bgdHaze.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		bgdHaze.Texture = (Texture2D)Resources.Load("toolbox_bundle|timer_4");
		numbers = new GUILabel();
		numbers.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, -10f));
		numbers.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		numbers.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 21, GUILabel.GenColor(61, 61, 61), TextAnchor.MiddleCenter);
		numberShadow = new GUILabel();
		numberShadow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(2f, -9f));
		numberShadow.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		numberShadow.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 21, GUILabel.GenColor(255, 255, 255), TextAnchor.MiddleCenter);
		counter = new GUILabel();
		counter.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 10f));
		counter.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		counter.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 21, GUILabel.GenColor(100, 100, 100), TextAnchor.MiddleCenter);
		counterShadow = new GUILabel();
		counterShadow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(2f, 11f));
		counterShadow.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		counterShadow.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 21, GUILabel.GenColor(255, 255, 255), TextAnchor.MiddleCenter);
		Add(bgd);
		Add(bgdMotion);
		Add(bgd2);
		Add(numberShadow);
		Add(numbers);
		Add(counterShadow);
		Add(counter);
		Add(bgdHaze);
	}

	public override void DrawPreprocess()
	{
		base.DrawPreprocess();
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		timeText = convertToTime(Convert.ToInt32(base.TimeLeft));
		if (base.TimerState == TimerStateEnum.Idle)
		{
			timeText = convertToTime(Convert.ToInt32(base.Duration));
		}
		numbers.Text = timeText;
		numberShadow.Text = timeText;
		string text = count + "/" + countTotal;
		counter.Text = text;
		counterShadow.Text = text;
		if (base.TimerState == TimerStateEnum.Running)
		{
			float time = Time.time;
			float num = time - lastTime;
			lastTime = time;
			bgdMotion.Rotation += degreesPerSecond * num;
		}
	}

	private string convertToTime(int sec)
	{
		return sec / 60 + ":" + keepTensPlace(sec % 60);
	}

	private string keepTensPlace(int sec)
	{
		if (sec < 10)
		{
			return "0" + sec;
		}
		return string.Empty + sec;
	}
}
