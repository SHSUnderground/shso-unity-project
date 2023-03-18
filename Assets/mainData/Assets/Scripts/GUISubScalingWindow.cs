using System.Collections.Generic;
using UnityEngine;

public class GUISubScalingWindow : GUISimpleControlWindow
{
	public class FullSizeData
	{
		public Vector2 MaxSize;

		public Vector2 MaxOffset;

		public float MaxAlpha;

		public FullSizeData(Vector2 MaxSize, Vector2 MaxOffset, float MaxAlpha)
		{
			this.MaxSize = MaxSize;
			this.MaxOffset = MaxOffset;
			this.MaxAlpha = MaxAlpha;
		}

		public FullSizeData(GUIControl ctrl)
		{
			MaxSize = ctrl.Size;
			MaxOffset = ctrl.Offset;
			MaxAlpha = ctrl.Alpha;
		}
	}

	public class FullSizeLabelData : FullSizeData
	{
		public float MaxTextSize;

		public FullSizeLabelData(GUILabel ctrl)
			: base(ctrl)
		{
			MaxTextSize = ctrl.FontSize;
		}
	}

	public FullSizeData WindowData;

	public Dictionary<GUIControl, FullSizeData> MaxData;

	public override float Alpha
	{
		get
		{
			return base.Alpha;
		}
		set
		{
			base.Alpha = value;
			foreach (GUIControl key in MaxData.Keys)
			{
				SetAlphaForCtrl(key);
			}
		}
	}

	public GUISubScalingWindow(float a, float b)
		: this(new Vector2(a, b), 1f)
	{
	}

	public GUISubScalingWindow(Vector2 MaxSize)
		: this(MaxSize, 1f)
	{
	}

	public GUISubScalingWindow(Vector2 MaxSize, float MaxAlpha)
	{
		WindowData = new FullSizeData(MaxSize, Vector2.zero, MaxAlpha);
		MaxData = new Dictionary<GUIControl, FullSizeData>();
		SetSize(MaxSize);
	}

	public void AddLabel(GUILabel label)
	{
		FullSizeLabelData value = new FullSizeLabelData(label);
		MaxData.Add(label, value);
		Add(label);
	}

	public void AddItem(GUIControl ctrl)
	{
		FullSizeData value = new FullSizeData(ctrl);
		MaxData.Add(ctrl, value);
		Add(ctrl);
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		if (position != POSITION_UNDEFINED)
		{
			CalculateRect();
		}
		foreach (GUIControl key in MaxData.Keys)
		{
			SetSizeAndOffsetForCtrl(key);
		}
	}

	private void SetSizeAndOffsetForCtrl(GUIControl ctrl)
	{
		FullSizeData fullSizeData = MaxData[ctrl];
		if (ctrl is GUILabel && fullSizeData is FullSizeLabelData)
		{
			SetTextSize(ctrl as GUILabel, fullSizeData as FullSizeLabelData);
		}
		ctrl.SetSize(GetCurrentCtrlValue(WindowData.MaxSize, fullSizeData.MaxSize, Size));
		ctrl.Offset = GetCurrentCtrlValue(WindowData.MaxSize, fullSizeData.MaxOffset, Size);
	}

	private void SetTextSize(GUILabel label, FullSizeLabelData ctrlMaxData)
	{
		Vector2 size = Size;
		float a = size.x / WindowData.MaxSize.x;
		Vector2 size2 = Size;
		label.FontSize = Mathf.RoundToInt(Mathf.Min(a, size2.y / WindowData.MaxSize.y) * ctrlMaxData.MaxTextSize);
	}

	private void SetAlphaForCtrl(GUIControl ctrl)
	{
		FullSizeData fullSizeData = MaxData[ctrl];
		ctrl.Alpha = GetCurrentCtrlValue(WindowData.MaxAlpha, fullSizeData.MaxAlpha, Alpha);
	}

	private Vector2 GetCurrentCtrlValue(Vector2 MaxWinValue, Vector2 MaxCtrlValue, Vector2 CurrentWindowValue)
	{
		return new Vector2(GetCurrentCtrlValue(MaxWinValue.x, MaxCtrlValue.x, CurrentWindowValue.x), GetCurrentCtrlValue(MaxWinValue.y, MaxCtrlValue.y, CurrentWindowValue.y));
	}

	private float GetCurrentCtrlValue(float MaxWinValue, float MaxCtrlValue, float CurrentWindowValue)
	{
		return CurrentWindowValue / MaxWinValue * MaxCtrlValue;
	}
}
