using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimClipBuilder
{
	public class Delta
	{
		private delegate void DeltaModDel(GUIControl control, float value);

		public static AnimClip SizeX(AnimPath path, params GUIControl[] controls)
		{
			return DeltaMod("SizeX", path, controls, delegate(GUIControl control, float value)
			{
				Vector2 size = control.Size;
				float x = size.x + value;
				Vector2 size2 = control.Size;
				control.SetSize(new Vector2(x, size2.y), control.HorizontalSizeHint, control.VerticalSizeHint);
			});
		}

		public static AnimClip SizeY(AnimPath path, params GUIControl[] controls)
		{
			return DeltaMod("SizeY", path, controls, delegate(GUIControl control, float value)
			{
				Vector2 size = control.Size;
				float x = size.x;
				Vector2 size2 = control.Size;
				control.SetSize(new Vector2(x, size2.y + value), control.HorizontalSizeHint, control.VerticalSizeHint);
			});
		}

		public static AnimClip SizeXY(AnimPath path, params GUIControl[] controls)
		{
			return DeltaMod("SizeXY", path, controls, delegate(GUIControl control, float value)
			{
				Vector2 size = control.Size;
				float x = size.x + value;
				Vector2 size2 = control.Size;
				control.SetSize(new Vector2(x, size2.y + value), control.HorizontalSizeHint, control.VerticalSizeHint);
			});
		}

		public static AnimClip PositionX(AnimPath path, params GUIControl[] controls)
		{
			return DeltaMod("PositionX", path, controls, delegate(GUIControl control, float value)
			{
				Vector2 position = control.Position;
				float x = position.x + value;
				Vector2 position2 = control.Position;
				control.SetPosition(new Vector2(x, position2.y), control.Docking, control.Anchor, control.OffsetStyle, control.Offset);
			});
		}

		public static AnimClip PositionY(AnimPath path, params GUIControl[] controls)
		{
			return DeltaMod("PositionY", path, controls, delegate(GUIControl control, float value)
			{
				Vector2 position = control.Position;
				float x = position.x;
				Vector2 position2 = control.Position;
				control.SetPosition(new Vector2(x, position2.y + value), control.Docking, control.Anchor, control.OffsetStyle, control.Offset);
			});
		}

		public static AnimClip Alpha(AnimPath path, params GUIControl[] controls)
		{
			return DeltaMod("Alpha", path, controls, delegate(GUIControl control, float value)
			{
				control.Alpha += value;
			});
		}

		public static AnimClip AnimationAlpha(AnimPath path, params GUIControl[] controls)
		{
			return DeltaMod("AnimationAlpha", path, controls, delegate(GUIControl control, float value)
			{
				control.AnimationAlpha += value;
			});
		}

		public static AnimClip OffsetX(AnimPath path, params GUIControl[] controls)
		{
			return DeltaMod("OffsetX", path, controls, delegate(GUIControl control, float value)
			{
				Vector2 offset = control.Offset;
				float x = offset.x + value;
				Vector2 offset2 = control.Offset;
				control.Offset = new Vector2(x, offset2.y);
			});
		}

		public static AnimClip OffsetY(AnimPath path, params GUIControl[] controls)
		{
			return DeltaMod("OffsetY", path, controls, delegate(GUIControl control, float value)
			{
				Vector2 offset = control.Offset;
				float x = offset.x;
				Vector2 offset2 = control.Offset;
				control.Offset = new Vector2(x, offset2.y + value);
			});
		}

		public static AnimClip Rotation(AnimPath path, params GUIControl[] controls)
		{
			return DeltaMod("Rotation", path, controls, delegate(GUIControl control, float value)
			{
				control.Rotation += value;
			});
		}

		private static AnimClip DeltaMod(string name, AnimPath path, GUIControl[] controls, Action<GUIControl, float> del)
		{
			if (controls.Length == 0)
			{
				CspUtils.DebugLog("Warning: GUI Animation requested without any target controls");
			}
			float lastValue = path.GetValue(0f);
			AnimClipFunction animClipFunction = new AnimClipFunction(path, delegate(float curValue)
			{
				float num = curValue - lastValue;
				lastValue = curValue;
				GUIControl[] array = controls;
				foreach (GUIControl gUIControl in array)
				{
					del.Invoke(gUIControl, num);
				}
			});
			if (controls.Length == 1)
			{
				animClipFunction.Name = controls[0].Id + " " + name;
			}
			else if (controls.Length > 1)
			{
				animClipFunction.Name = controls[0].Id + " " + name + " + others";
			}
			return animClipFunction;
		}
	}

	public class Absolute
	{
		private delegate void AbsoluteModDel(GUIControl control, float value);

		public static AnimClip SizeX(AnimPath path, params GUIControl[] controls)
		{
			return AbsoluteMod("SizeX", path, controls, delegate(GUIControl control, float value)
			{
				Vector2 size = control.Size;
				control.SetSize(new Vector2(value, size.y), control.HorizontalSizeHint, control.VerticalSizeHint);
			});
		}

		public static AnimClip SizeY(AnimPath path, params GUIControl[] controls)
		{
			return AbsoluteMod("SizeY", path, controls, delegate(GUIControl control, float value)
			{
				Vector2 size = control.Size;
				control.SetSize(new Vector2(size.x, value), control.HorizontalSizeHint, control.VerticalSizeHint);
			});
		}

		public static AnimClip SizeXY(AnimPath path, params GUIControl[] controls)
		{
			return AbsoluteMod("SizeXY", path, controls, delegate(GUIControl control, float value)
			{
				control.SetSize(new Vector2(value, value), control.HorizontalSizeHint, control.VerticalSizeHint);
			});
		}

		public static AnimClip PositionX(AnimPath path, params GUIControl[] controls)
		{
			return AbsoluteMod("PositionX", path, controls, delegate(GUIControl control, float value)
			{
				Vector2 position = control.Position;
				control.SetPosition(new Vector2(value, position.y), control.Docking, control.Anchor, control.OffsetStyle, control.Offset);
			});
		}

		public static AnimClip PositionY(AnimPath path, params GUIControl[] controls)
		{
			return AbsoluteMod("PositionY", path, controls, delegate(GUIControl control, float value)
			{
				Vector2 position = control.Position;
				control.SetPosition(new Vector2(position.x, value), control.Docking, control.Anchor, control.OffsetStyle, control.Offset);
			});
		}

		public static AnimClip Alpha(AnimPath path, params GUIControl[] controls)
		{
			return AbsoluteMod("Alpha", path, controls, delegate(GUIControl control, float value)
			{
				control.Alpha = value;
			});
		}

		public static AnimClip AnimationAlpha(AnimPath path, params GUIControl[] controls)
		{
			return AbsoluteMod("AnimationAlpha", path, controls, delegate(GUIControl control, float value)
			{
				control.AnimationAlpha = value;
			});
		}

		public static AnimClip OffsetX(AnimPath path, params GUIControl[] controls)
		{
			return AbsoluteMod("OffsetX", path, controls, delegate(GUIControl control, float value)
			{
				Vector2 offset = control.Offset;
				control.Offset = new Vector2(value, offset.y);
			});
		}

		public static AnimClip OffsetY(AnimPath path, params GUIControl[] controls)
		{
			return AbsoluteMod("OffsetY", path, controls, delegate(GUIControl control, float value)
			{
				Vector2 offset = control.Offset;
				control.Offset = new Vector2(offset.x, value);
			});
		}

		public static AnimClip Rotation(AnimPath path, params GUIControl[] controls)
		{
			return AbsoluteMod("Rotation", path, controls, delegate(GUIControl control, float value)
			{
				control.Rotation = value;
			});
		}

		public static AnimClip Nothing(AnimPath path, params GUIControl[] controls)
		{
			return AbsoluteMod("Nothing", path, controls, delegate
			{
			});
		}

		private static AnimClip AbsoluteMod(string name, AnimPath path, GUIControl[] controls, AbsoluteModDel del)
		{
			if (controls.Length == 0)
			{
				CspUtils.DebugLog("Warning: GUI Animation requested without any target controls");
			}
			AnimClipFunction animClipFunction = new AnimClipFunction(path, delegate(float value)
			{
				GUIControl[] array = controls;
				foreach (GUIControl control in array)
				{
					del(control, value);
				}
			});
			if (controls.Length == 1)
			{
				animClipFunction.Name = controls[0].Id + " " + name;
			}
			else if (controls.Length > 1)
			{
				animClipFunction.Name = controls[0].Id + " " + name + " + others";
			}
			return animClipFunction;
		}
	}

	public class Custom
	{
		public static AnimClip Function(AnimPath path, Action<float> function)
		{
			return new AnimClipFunction(path, function);
		}
	}

	public class Path
	{
		public static AnimPathOld CurveEditor(GameObject AnimationObject, string ClipName)
		{
			GUIPathAnimation path = AnimationObject.GetComponent<GUIPathAnimation>();
			Animation animation = AnimationObject.GetComponent<Animation>();
			if (path == null)
			{
				CspUtils.DebugLog("GameObject does not have GUIPathAnimation");
				return null;
			}
			if (animation == null)
			{
				CspUtils.DebugLog("GameObject does not have Animation");
				return null;
			}
			if (animation[ClipName] == null)
			{
				CspUtils.DebugLog("GameObject does not have clip named " + ClipName + " as part of Animation");
				return null;
			}
			float animationLength = animation[ClipName].length;
			animation.Play();
			animation.Stop();
			return new AnimPathOld(delegate(AnimClip a, float totalTimeElapsed, out bool done)
			{
				animation[ClipName].enabled = true;
				if (totalTimeElapsed < animationLength)
				{
					animation[ClipName].time = totalTimeElapsed;
					done = false;
				}
				else
				{
					animation[ClipName].time = animationLength;
					done = true;
				}
				animation.Sample();
				animation[ClipName].enabled = false;
				return path.Path;
			}, animationLength);
		}

		public static AnimPath Sin(float periodOffset, float numberOfRevolutions, float totalTime)
		{
			return AnimPath.Sin(periodOffset, numberOfRevolutions, totalTime);
		}

		public static AnimPath Cos(float periodOffset, float numberOfRevolutions, float totalTime)
		{
			return AnimPath.Cos(periodOffset, numberOfRevolutions, totalTime);
		}

		public static AnimPath Linear(float start, float finish, float time)
		{
			return AnimPath.Linear(start, finish, time);
		}

		public static AnimPath Quadratic(float start, float finish, float bend, float time)
		{
			return AnimPath.Quadratic(start, finish, bend, time);
		}

		public static AnimPath Constant(float value, float time)
		{
			return AnimPath.Constant(value, time);
		}

		public static AnimPath Composite(params AnimPath[] animationPaths)
		{
			if (animationPaths.Length == 0)
			{
				return AnimPath.Constant(0f, 0f);
			}
			AnimPath result = animationPaths[0];
			for (int i = 1; i < animationPaths.Length; i++)
			{
				result *= animationPaths[i];
			}
			return result;
		}

		public static AnimPath Pow(AnimPath baseNum, AnimPath power)
		{
			return AnimPath.Pow(baseNum, power);
		}

		public static AnimPath Additive(params AnimPath[] animationPaths)
		{
			if (animationPaths.Length == 0)
			{
				return AnimPath.Constant(0f, 0f);
			}
			AnimPath result = animationPaths[0];
			for (int i = 1; i < animationPaths.Length; i++)
			{
				result += animationPaths[i];
			}
			return result;
		}

		public static AnimPath Chained(params AnimPath[] animationPaths)
		{
			if (animationPaths.Length == 0)
			{
				return AnimPath.Constant(0f, 0f);
			}
			AnimPath result = animationPaths[0];
			for (int i = 1; i < animationPaths.Length; i++)
			{
				result |= animationPaths[i];
			}
			return result;
		}

		public static AnimPath Chained(params AnimPathOld[] animationPaths)
		{
			if (animationPaths.Length == 0)
			{
				return AnimPath.Constant(0f, 0f);
			}
			AnimPath result = animationPaths[0];
			for (int i = 1; i < animationPaths.Length; i++)
			{
				AnimPath animPath = animationPaths[i];
				result |= animPath;
			}
			return result;
		}

		public static AnimPath SmoothChainedWithCut(float cutTime, params AnimPathOld[] animationPaths)
		{
			float num = 0f;
			foreach (AnimPathOld animPathOld in animationPaths)
			{
				num += animPathOld.animationLength;
			}
			List<AnimPathOld> list = new List<AnimPathOld>(animationPaths);
			List<AnimPathOld> list2 = new List<AnimPathOld>();
			AnimPathOld animPathOld2 = list[0];
			list.Remove(animPathOld2);
			AnimClip a = new AnimClipFunction(0f, delegate
			{
			});
			while (list.Count > 0)
			{
				AnimPathOld animPathOld3 = list[0];
				list.Remove(animPathOld3);
				float num2 = Mathf.Min(cutTime, animPathOld2.animationLength / 2f);
				float num3 = num2;
				float totalTimeElapsed = animPathOld2.animationLength - num2;
				bool done;
				float num4 = animPathOld2.path(a, totalTimeElapsed, out done);
				float num5 = Mathf.Min(0.01f, animPathOld2.animationLength / 15f);
				totalTimeElapsed = animPathOld2.animationLength - (num2 + num5);
				float startDelta = (num4 - animPathOld2.path(a, totalTimeElapsed, out done)) / num5;
				num2 = Mathf.Min(cutTime, animPathOld3.animationLength / 2f);
				num3 += num2;
				totalTimeElapsed = num2;
				float num6 = animPathOld3.path(a, totalTimeElapsed, out done);
				num5 = Mathf.Min(0.01f, animPathOld3.animationLength / 15f);
				totalTimeElapsed = num2 + num5;
				float endDelta = (animPathOld3.path(a, totalTimeElapsed, out done) - num6) / num5;
				list2.Add(animPathOld2);
				list2.Add(SmoothTransition(num4, startDelta, num6, endDelta, num3));
				animPathOld2 = animPathOld3;
			}
			list2.Add(animPathOld2);
			for (int j = 0; j < animationPaths.Length; j++)
			{
				float num2 = Mathf.Min(cutTime, animationPaths[j].animationLength / 2f);
				if (j == 0 || j == animationPaths.Length - 1)
				{
					animationPaths[j].animationLength -= num2;
					if (j == animationPaths.Length - 1)
					{
						animationPaths[j].pathOffset = num2;
					}
				}
				else
				{
					animationPaths[j].animationLength -= num2 * 2f;
					animationPaths[j].pathOffset = num2;
				}
			}
			return Chained(list2.ToArray());
		}

		public static AnimPath SmoothChainedAdditive(float additiveTime, params AnimPathOld[] animationPaths)
		{
			List<AnimPathOld> list = new List<AnimPathOld>(animationPaths);
			List<AnimPathOld> list2 = new List<AnimPathOld>();
			AnimPathOld animPathOld = list[0];
			list.Remove(animPathOld);
			AnimClip a = new AnimClipFunction(0f, delegate
			{
			});
			while (list.Count > 0)
			{
				AnimPathOld animPathOld2 = list[0];
				list.Remove(animPathOld2);
				float animationLength = animPathOld.animationLength;
				bool done;
				float num = animPathOld.path(a, animationLength, out done);
				float num2 = Mathf.Min(0.01f, animPathOld.animationLength / 15f);
				animationLength = animPathOld.animationLength - num2;
				float startDelta = (num - animPathOld.path(a, animationLength, out done)) / num2;
				animationLength = 0f;
				float num3 = animPathOld2.path(a, animationLength, out done);
				num2 = Mathf.Min(0.01f, animPathOld2.animationLength / 15f);
				animationLength = num2;
				float endDelta = (animPathOld2.path(a, animationLength, out done) - num3) / num2;
				list2.Add(animPathOld);
				list2.Add(SmoothTransition(num, startDelta, num3, endDelta, additiveTime));
				animPathOld = animPathOld2;
			}
			list2.Add(animPathOld);
			return Chained(list2.ToArray());
		}

		public static AnimPathOld SmoothTransition(float start, float startDelta, float end, float endDelta, float time)
		{
			float animationLength = time;
			float period;
			float amp;
			if (Mathf.Sign(startDelta) == Mathf.Sign(endDelta) && startDelta != 0f && endDelta != 0f)
			{
				period = (float)Math.PI * 2f;
				float num = start + startDelta * time * 0.25f - ((end - start) * time * 0.25f + start);
				float num2 = end + endDelta * (0f - time) + endDelta * 0.75f - ((end - start) * time * 0.75f + start);
				amp = num * 0.5f + num2 * 0.5f;
			}
			else
			{
				period = (float)Math.PI;
				amp = (start + startDelta * time / 2f) * 0.5f + (end + endDelta * (0f - time) + endDelta * time / 2f) * 0.5f - ((end - start) * time / 2f + start);
			}
			amp *= 0.8f;
			return new AnimPathOld(delegate(AnimClip a, float totalTimeElapsed, out bool done)
			{
				if (totalTimeElapsed > time)
				{
					totalTimeElapsed = time;
				}
				float num3 = totalTimeElapsed / time;
				float num4 = start + startDelta * totalTimeElapsed;
				float num5 = end + endDelta * (0f - time) + endDelta * totalTimeElapsed;
				float num6 = Mathf.Sin(period * num3) * amp + (end - start) * totalTimeElapsed + start;
				float num7 = (0f - Mathf.Cos((float)Math.PI * num3) + 1f) * 0.5f;
				float num8 = Mathf.Pow(1f - num7, 3f);
				float num9 = Mathf.Pow(num7, 3f);
				float num10 = 1f - (num8 + num9);
				float result = num4 * num8 + num6 * num10 + num5 * num9;
				done = (totalTimeElapsed >= time);
				return result;
			}, animationLength);
		}
	}

	public class AnimPathOld
	{
		public float pathOffset;

		private AnimationPathDelegate aPath;

		public float animationLength;

		public AnimPathOld(AnimationPathDelegate aPath, float animationLength)
		{
			this.aPath = aPath;
			this.animationLength = animationLength;
		}

		public float path(AnimClip a, float totalTimeElapsed, out bool done)
		{
			return aPath(a, totalTimeElapsed + pathOffset, out done);
		}

		public static AnimPath operator +(AnimPathOld a, AnimPathOld b)
		{
			return Path.Additive(a, b);
		}

		public static AnimPath operator -(AnimPathOld a, AnimPathOld b)
		{
			return Path.Additive(a, Path.Composite(Path.Constant(-1f, 0f), b));
		}

		public static AnimPath operator *(AnimPathOld a, AnimPathOld b)
		{
			return Path.Composite(a, b);
		}

		public static AnimPath operator |(AnimPathOld a, AnimPathOld b)
		{
			return Path.Chained(a, b);
		}

		public static implicit operator AnimPathOld(float a)
		{
			return Path.Constant(a, 0f);
		}

		public static implicit operator AnimPathOld(AnimPath animPath)
		{
			return new AnimPathOld(delegate(AnimClip a, float totalTimeElapsed, out bool done)
			{
				done = !(totalTimeElapsed < animPath.TotalTime);
				return animPath.GetValue(totalTimeElapsed);
			}, animPath.TotalTime);
		}
	}

	public delegate float AnimationPathDelegate(AnimClip a, float totalTimeElapsed, out bool done);

	public static GUIChildControl IgnoreWarningControl = new GUIChildControl();

	public static AnimClip CurveEditor(GameObject AnimationObject, string ClipName, params GUIControl[] ctrls)
	{
		GUICurveAnimation curve = AnimationObject.GetComponent<GUICurveAnimation>();
		Animation component = AnimationObject.GetComponent<Animation>();
		if (curve == null)
		{
			CspUtils.DebugLog("GameObject does not have GUICurveAnimation");
			return null;
		}
		if (component == null)
		{
			CspUtils.DebugLog("GameObject does not have Animation");
			return null;
		}
		if (component[ClipName] == null)
		{
			CspUtils.DebugLog("GameObject does not have clip named " + ClipName + " as part of Animation");
			return null;
		}
		Animation animation = component;
		float animationLength = animation[ClipName].length;
		animation.Play();
		animation.Stop();
		return new AnimClipFunction(animationLength, delegate(float totalTime)
		{
			animation[ClipName].enabled = true;
			if (totalTime < animationLength)
			{
				animation[ClipName].time = totalTime;
			}
			else
			{
				animation[ClipName].time = animationLength;
			}
			animation.Sample();
			animation[ClipName].enabled = false;
			GUIControl[] array = ctrls;
			foreach (GUIControl gUIControl in array)
			{
				curve.SetGUIControl(gUIControl);
			}
		});
	}

	public static void FullDebugPrint(AnimPathOld path, float step, Vector3 orgin, bool keepLast)
	{
		if (step == 0f)
		{
			CspUtils.DebugLogError("step = 0 means you get stuck in an infinate loop, setting step to 1");
			step = 1f;
		}
		List<float> list = new List<float>();
		AnimClip a = new AnimClipFunction(0f, delegate
		{
		});
		bool done = false;
		float num = 0f;
		float num2 = 0f;
		while (!done)
		{
			float num3 = path.path(a, num, out done);
			if (done && !keepLast)
			{
				break;
			}
			num2 = Mathf.Max(num2, Mathf.Abs(num3));
			list.Add(num3);
			num += step;
		}
		if (list.Count <= 3)
		{
			CspUtils.DebugLogError("not enough data points. lower your step. Returning");
			return;
		}
		List<float> list2 = new List<float>();
		float num4 = 0f;
		for (int i = 1; i < list.Count; i++)
		{
			float num5 = list[i] - list[i - 1];
			num4 = Mathf.Max(num4, Mathf.Abs(num5));
			list2.Add(num5);
		}
		List<float> list3 = new List<float>();
		float num6 = 0f;
		for (int j = 1; j < list2.Count; j++)
		{
			float num7 = list2[j] - list2[j - 1];
			num6 = Mathf.Max(num6, Mathf.Abs(num7));
			list3.Add(num7);
		}
		DrawGraph(list, num2, step, new Vector3(0f, 0f, 0f) + orgin);
		DrawGraph(list2, num4, step, new Vector3(0f, -2.2f, 0f) + orgin);
		DrawGraph(list3, num6, step, new Vector3(0f, -4.4f, 0f) + orgin);
	}

	private static void DrawGraph(List<float> pos, float maxVal, float step, Vector3 orgin)
	{
		if (maxVal == 0f)
		{
			maxVal = 1f;
		}
		if (maxVal >= 1f)
		{
			Debug.DrawLine(new Vector3(0f, 1f / maxVal, 0f) + orgin, new Vector3(step * (float)pos.Count, 1f / maxVal, 0f) + orgin, Color.gray);
			Debug.DrawLine(new Vector3(0f, -1f / maxVal, 0f) + orgin, new Vector3(step * (float)pos.Count, -1f / maxVal, 0f) + orgin, Color.gray);
		}
		Debug.DrawLine(new Vector3(0f, 1f, 0f) + orgin, new Vector3(0f, -1f, 0f) + orgin, Color.black);
		Debug.DrawLine(new Vector3(0f, 0f, 0f) + orgin, new Vector3(step * (float)pos.Count, 0f, 0f) + orgin, Color.black);
		Vector3 start = new Vector3(0f, pos[0] / maxVal, 0f) + orgin;
		float num = step;
		for (int i = 1; i < pos.Count; i++)
		{
			float num2 = pos[i] / maxVal;
			Vector3 vector = new Vector3(num, num2, 0f) + orgin;
			Debug.DrawLine(start, vector, new Color(Mathf.Abs(num2), 0f, Mathf.Abs(1f - num2)));
			start = vector;
			num += step;
		}
	}
}
