using System;
using UnityEngine;

public class SHSBaseLogicBar : GUIChildControl
{
	public enum BarDirection
	{
		AscendLeft,
		AscendRight
	}

	public delegate void SHSBarEventHandler(object sender, SHSBarEventArgs args);

	protected float min;

	protected float max;

	protected float value;

	protected float updateSpeed;

	protected float currentValue;

	protected BarDirection direction;

	protected bool valueChangedFlag;

	public float Min
	{
		get
		{
			return min;
		}
		set
		{
			min = value;
			ensureBounds();
		}
	}

	public float Max
	{
		get
		{
			return max;
		}
		set
		{
			max = value;
			ensureBounds();
		}
	}

	public virtual float Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
			valueChangedFlag = true;
			ensureBounds();
		}
	}

	public float UpdateSpeed
	{
		get
		{
			return updateSpeed;
		}
		set
		{
			updateSpeed = value;
		}
	}

	public float CurrentValue
	{
		get
		{
			return currentValue;
		}
	}

	public BarDirection Direction
	{
		get
		{
			return direction;
		}
		set
		{
			direction = value;
		}
	}

	public event SHSBarEventHandler SHSBarEvent;

	public SHSBaseLogicBar()
	{
		Min = 0f;
		Max = 100f;
		Value = 0f;
		currentValue = 0f;
		updateSpeed = 10f;
		Direction = BarDirection.AscendLeft;
		valueChangedFlag = false;
	}

	protected void ensureBounds()
	{
		if (value < min)
		{
			value = min;
		}
		if (value > max)
		{
			value = max;
		}
		if (currentValue < min)
		{
			currentValue = min;
		}
		if (currentValue > max)
		{
			currentValue = max;
		}
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		float deltaTime = Time.deltaTime;
		if (currentValue == Value && valueChangedFlag)
		{
			return;
		}
		int num = Math.Sign(Value - currentValue);
		if (num != 0)
		{
			currentValue += deltaTime * updateSpeed * (float)num;
			switch (num)
			{
			case 1:
				if (CurrentValue > Value)
				{
					currentValue = Value;
				}
				break;
			case -1:
				if (CurrentValue < Value)
				{
					currentValue = Value;
				}
				break;
			}
		}
		if (valueChangedFlag)
		{
			valueChangedFlag = false;
			FireSHSBarEvent(this, SHSBarEventArgs.SHSBarEventType.Start);
		}
		if (currentValue == Value)
		{
			FireSHSBarEvent(this, SHSBarEventArgs.SHSBarEventType.End);
		}
		else
		{
			FireSHSBarEvent(this, SHSBarEventArgs.SHSBarEventType.Progress);
		}
	}

	protected void FireSHSBarEvent(object sender, SHSBarEventArgs.SHSBarEventType PassedEventType)
	{
		if (this.SHSBarEvent != null)
		{
			this.SHSBarEvent(sender, new SHSBarEventArgs(PassedEventType, Min, Max, Value, CurrentValue));
		}
	}

	public void InitializeValue(float value)
	{
		this.value = value;
		currentValue = value;
		ensureBounds();
	}

	public float GetPercentage()
	{
		return (CurrentValue - Min) / (Max - Min);
	}
}
