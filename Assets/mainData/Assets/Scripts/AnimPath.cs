using System;
using UnityEngine;

public class AnimPath
{
	public float timeOffset;

	private Func<float, float> func;

	private float totalTime;

	public float TotalTime
	{
		get
		{
			return totalTime;
		}
	}

	public AnimPath(Func<float, float> func, float totalTime)
	{
		this.func = func;
		this.totalTime = totalTime;
	}

	public float GetValue(float time)
	{
		time += timeOffset;
		time = Mathf.Clamp(time, 0f, TotalTime);
		return func.Invoke(time);
	}

	public static AnimPath Sin(float periodOffset, float numberOfRevolutions, float totalTime)
	{
		if (totalTime == 0f)
		{
			return Constant(Mathf.Sin(periodOffset + (float)Math.PI * 2f * numberOfRevolutions), totalTime);
		}
		return new AnimPath(delegate(float t)
		{
			return Mathf.Sin(periodOffset + (float)Math.PI * 2f * (t / totalTime) * numberOfRevolutions);
		}, totalTime);
	}

	public static AnimPath Cos(float periodOffset, float numberOfRevolutions, float totalTime)
	{
		if (totalTime == 0f)
		{
			return Constant(Mathf.Cos(periodOffset + (float)Math.PI * 2f * numberOfRevolutions), totalTime);
		}
		return new AnimPath(delegate(float t)
		{
			return Mathf.Cos(periodOffset + (float)Math.PI * 2f * (t / totalTime) * numberOfRevolutions);
		}, totalTime);
	}

	public static AnimPath Linear(float start, float finish, float time)
	{
		if (time == 0f)
		{
			return Constant(finish, time);
		}
		return new AnimPath(delegate(float t)
		{
			return start + (finish - start) * (t / time);
		}, time);
	}

	public static AnimPath Quadratic(float start, float finish, float bend, float time)
	{
		if (time == 0f)
		{
			return Constant(finish, time);
		}
		return new AnimPath(delegate(float t)
		{
			float num = t / time;
			num = num * num * (1f - bend) + (1f - (1f - num) * (1f - num)) * bend;
			return start + (finish - start) * num;
		}, time);
	}

	public static AnimPath Constant(float value, float time)
	{
		return new AnimPath(delegate
		{
			return value;
		}, time);
	}

	public static AnimPath Pow(AnimPath baseNum, AnimPath power)
	{
		return new AnimPath(delegate(float t)
		{
			return Mathf.Pow(baseNum.GetValue(t), power.GetValue(t));
		}, Mathf.Max(baseNum.TotalTime, power.TotalTime));
	}

	public static AnimPath Convert(AnimClipBuilder.AnimPathOld oldPath)
	{
		AnimClip oldAnimPiece = new AnimClipFunction(0f, delegate
		{
		});
		return new AnimPath(delegate(float t)
		{
			bool done;
			return oldPath.path(oldAnimPiece, t, out done);
		}, oldPath.animationLength);
	}

	public static AnimPath operator +(AnimPath a, AnimPath b)
	{
		return new AnimPath(delegate(float x)
		{
			return a.GetValue(x) + b.GetValue(x);
		}, Mathf.Max(a.TotalTime, b.TotalTime));
	}

	public static AnimPath operator -(AnimPath a, AnimPath b)
	{
		return new AnimPath(delegate(float x)
		{
			return a.GetValue(x) - b.GetValue(x);
		}, Mathf.Max(a.TotalTime, b.TotalTime));
	}

	public static AnimPath operator *(AnimPath a, AnimPath b)
	{
		return new AnimPath(delegate(float x)
		{
			return a.GetValue(x) * b.GetValue(x);
		}, Mathf.Max(a.TotalTime, b.TotalTime));
	}

	public static AnimPath operator |(AnimPath a, AnimPath b)
	{
		Func<float, float> val = delegate(float x)
		{
			return (x < a.TotalTime) ? a.GetValue(x) : b.GetValue(x - a.TotalTime);
		};
		return new AnimPath(val, a.TotalTime + b.TotalTime);
	}

	public static implicit operator AnimPath(float a)
	{
		return new AnimPath(delegate
		{
			return a;
		}, 0f);
	}

	public static implicit operator AnimPath(AnimClipBuilder.AnimPathOld oldPath)
	{
		return Convert(oldPath);
	}
}
