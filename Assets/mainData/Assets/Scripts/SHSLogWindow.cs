using System.Collections.Generic;
using UnityEngine;

public class SHSLogWindow : GUITabbedDialogWindow.GUITabbedWindow, ILogListener
{
	public class LogEntryView
	{
		public SHSLogEntry Entry;

		public float StartPoint;

		public float EndPoint;

		public float CalcHeight;

		public LogEntryView(SHSLogEntry entry)
		{
			Entry = entry;
			StartPoint = float.MinValue;
			EndPoint = float.MinValue;
			CalcHeight = float.MinValue;
		}
	}

	private GUISlider logSlider;

	private GUILabel searchResults;

	private int entryIndex;

	private float lastSliderPos;

	private List<LogEntryView> logEntryViews;

	private Rect logRect;

	private Dictionary<SHSLogEntry.LogEntryType, Color> logColors;

	private Texture2D highlightedTexture;

	private SHSStyle logEntryStyle;

	private float viewWidth = float.MinValue;

	private bool autoScroll = true;

	public SHSStyle LogEntryStyle
	{
		get
		{
			return logEntryStyle;
		}
		set
		{
			logEntryStyle = value;
			if (value != null)
			{
				SHSDebug.AddListener(this);
				Recalculate();
			}
		}
	}

	public float ViewWidth
	{
		get
		{
			return viewWidth;
		}
		set
		{
			viewWidth = value;
			Recalculate();
		}
	}

	public bool AutoScroll
	{
		get
		{
			return autoScroll;
		}
		set
		{
			autoScroll = value;
		}
	}

	public SHSLogWindow(string WindowName)
		: base(WindowName, null)
	{
		SetBackground(new Color(0.5f, 0.5f, 0.8f, 0.4f));
		logEntryViews = new List<LogEntryView>();
		logSlider = new GUISlider();
		logSlider.Rotation = 0f;
		logSlider.Color = new Color(1f, 1f, 1f, 1f);
		logSlider.IsVisible = true;
		logSlider.Value = 0f;
		logSlider.Min = 0f;
		logSlider.Max = 100f;
		Add(logSlider);
		logColors = new Dictionary<SHSLogEntry.LogEntryType, Color>();
		logColors.Add(SHSLogEntry.LogEntryType.Info, new Color(1f, 1f, 1f, 1f));
		logColors.Add(SHSLogEntry.LogEntryType.Warning, new Color(1f, 0.8f, 0.3f, 1f));
		logColors.Add(SHSLogEntry.LogEntryType.Error, new Color(1f, 0.3f, 0.3f, 1f));
		logColors.Add(SHSLogEntry.LogEntryType.Critical, new Color(1f, 0f, 0f, 1f));
		logColors.Add(SHSLogEntry.LogEntryType.Highlight, new Color(0f, 0f, 0f, 1f));
		GUIImage gUIImage = GUIControl.CreateControl<GUIImage>(new Vector2(214f, 78f), new Vector2(-40f, 25f), DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		gUIImage.TextureSource = "common_bundle|white2x2";
		gUIImage.Color = new Color(0.5f, 0.5f, 0.5f, 1f);
		Add(gUIImage);
		GUILabel gUILabel = GUIControl.CreateControl<GUILabel>(new Vector2(194f, 25f), new Vector2(-50f, 30f), DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(239, 255, 119), TextAnchor.UpperLeft);
		gUILabel.Text = "Search:";
		Add(gUILabel);
		GUITextField search = GUIControl.CreateControl<GUITextField>(new Vector2(194f, 25f), new Vector2(-50f, 50f), DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		search.FontFace = GUIFontManager.SupportedFontEnum.Komica;
		search.FontSize = 15;
		search.TextColor = GUILabel.GenColor(81, 82, 81);
		search.Color = GUILabel.GenColor(0, 0, 255);
		search.BackgroundColor = GUILabel.GenColor(255, 0, 255);
		search.TextAlignment = TextAnchor.UpperLeft;
		search.Changed += delegate
		{
			HighlightMe(search.Text);
		};
		Add(search);
		searchResults = GUIControl.CreateControl<GUILabel>(new Vector2(194f, 25f), new Vector2(-50f, 80f), DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		searchResults.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(239, 255, 119), TextAnchor.UpperLeft);
		searchResults.Text = "Found:";
		Add(searchResults);
	}

	~SHSLogWindow()
	{
		SHSDebug.RemoveListener(this);
	}

	public void HighlightMe(string search)
	{
		if (highlightedTexture == null)
		{
			highlightedTexture = GUIManager.Instance.LoadTexture("common_bundle|white2x2");
		}
		search = search.ToLowerInvariant();
		int num = 0;
		for (int i = entryIndex; i < logEntryViews.Count; i++)
		{
			string text = logEntryViews[i].Entry.Message.ToLowerInvariant();
			logEntryViews[i].Entry.IsHighlighted = (text.Contains(search) && !string.IsNullOrEmpty(search));
			if (logEntryViews[i].Entry.IsHighlighted)
			{
				num++;
			}
		}
		if (string.IsNullOrEmpty(search))
		{
			searchResults.Text = "Type search text";
		}
		else
		{
			searchResults.Text = num.ToString() + " matches found.";
		}
	}

	public override void DrawPreprocess()
	{
		base.DrawPreprocess();
	}

	public override void OnUpdate()
	{
		float value = logSlider.Value;
		if (value != lastSliderPos)
		{
			LogEntryView logEntryView = logEntryViews[entryIndex];
			if (lastSliderPos < value)
			{
				if (logEntryView.EndPoint < value)
				{
					bool flag = false;
					while (!flag && entryIndex < logEntryViews.Count - 1)
					{
						entryIndex++;
						logEntryView = logEntryViews[entryIndex];
						if (logEntryView.EndPoint > value)
						{
							flag = true;
						}
					}
				}
			}
			else if (lastSliderPos > value && logEntryView.StartPoint > value)
			{
				bool flag2 = false;
				while (!flag2 && entryIndex > 0)
				{
					entryIndex--;
					logEntryView = logEntryViews[entryIndex];
					if (logEntryView.StartPoint < value)
					{
						flag2 = true;
					}
				}
			}
			lastSliderPos = logSlider.Value;
		}
		base.OnUpdate();
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		GUI.BeginGroup(logRect);
		Color color = GUI.color;
		for (int i = entryIndex; i < logEntryViews.Count; i++)
		{
			if (logEntryViews[i].Entry.IsHighlighted)
			{
				logEntryStyle.UnityStyle.normal.textColor = logColors[SHSLogEntry.LogEntryType.Highlight];
				logEntryStyle.UnityStyle.normal.background = highlightedTexture;
			}
			else
			{
				logEntryStyle.UnityStyle.normal.textColor = logColors[logEntryViews[i].Entry.EntryType];
				GUI.color = logColors[logEntryViews[i].Entry.EntryType];
				logEntryStyle.UnityStyle.normal.background = null;
			}
			GUILayout.Label(new GUIContent("[" + logEntryViews[i].Entry.TimeStamp.ToString("T") + "] " + logEntryViews[i].Entry.Message), logEntryStyle.UnityStyle, GUILayout.MaxWidth(ViewWidth));
		}
		GUI.color = color;
		GUI.EndGroup();
		base.Draw(drawFlags);
	}

	public override void DrawFinalize()
	{
		base.DrawFinalize();
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		if (logSlider != null)
		{
			logSlider.Rect = new Rect(base.rect.width - 40f, 30f, 30f, base.rect.height - 60f);
			logRect = new Rect(30f, 30f, base.rect.width - 50f, base.rect.height - 60f);
			ViewWidth = logRect.width;
		}
	}

	private void CalculateSize(LogEntryView view, int index)
	{
		float num = 0f;
		if (index > 0)
		{
			num = logEntryViews[index - 1].EndPoint;
		}
		float num2 = LogEntryStyle.UnityStyle.CalcHeight(new GUIContent(view.Entry.Message), ViewWidth);
		view.StartPoint = num;
		view.EndPoint = num + num2;
		view.CalcHeight = num2;
	}

	private void Recalculate()
	{
		for (int i = 0; i < logEntryViews.Count; i++)
		{
			CalculateSize(logEntryViews[i], i);
		}
		if (logEntryViews.Count == 0)
		{
			logSlider.Max = 0f;
		}
		else
		{
			logSlider.Max = logEntryViews[logEntryViews.Count - 1].EndPoint - logRect.height;
		}
	}

	public void OnLogEntryAdded(SHSLogEntry entry)
	{
		LogEntryView logEntryView = new LogEntryView(entry);
		CalculateSize(logEntryView, logEntryViews.Count);
		logEntryViews.Add(logEntryView);
		logSlider.Max = logEntryView.EndPoint - logRect.height;
		if (AutoScroll)
		{
			logSlider.Value = logSlider.Max;
		}
	}

	public void OnLogEntryRemoved(int logEntryIndex)
	{
		logEntryViews.RemoveAt(logEntryIndex);
		Recalculate();
	}

	public void OnLogEntryRemoved(SHSLogEntry entry)
	{
		int num = 0;
		while (true)
		{
			if (num < logEntryViews.Count)
			{
				if (logEntryViews[num].Entry == entry)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		OnLogEntryRemoved(num);
	}
}
