using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSAnimations : AnimClipBuilder
{
	public class WindowOpenCloseDelegates
	{
		public static GUIWindow.AnimationPieceGeneratorDelegate GetOpenAnimation(List<GUIControl> bounceIn, List<GUIControl> fadeIn, Vector2 fullSize, float startAlpha, params Action[] callbacks)
		{
			return delegate
			{
				AnimClip animClip = Generic.BatchAnimationTransitionIn(bounceIn, fadeIn, fullSize, startAlpha);
				Action[] array = callbacks;
				foreach (Action value in array)
				{
					animClip.OnFinished += value;
				}
				return animClip;
			};
		}

		public static GUIWindow.AnimationPieceGeneratorDelegate GetCloseAnimation(List<GUIControl> bounceOut, List<GUIControl> fadeOut, Vector2 fullSize, float endAlpha, params Action[] callbacks)
		{
			return delegate
			{
				AnimClip animClip = Generic.BatchAnimationTransitionOut(bounceOut, fadeOut, fullSize, endAlpha);
				Action[] array = callbacks;
				foreach (Action value in array)
				{
					animClip.OnFinished += value;
				}
				return animClip;
			};
		}

		public static GUIWindow.AnimationPieceGeneratorDelegate GetOpenAnimation(GUIWindow window, Vector2 fullSize, params GUIControl[] ctrls)
		{
			List<GUIControl> bounceIn = new List<GUIControl>(ctrls);
			List<GUIControl> fadeIn = new List<GUIControl>();
			foreach (IGUIControl control in window.ControlList)
			{
				if (control is GUIControl && !bounceIn.Contains(control as GUIControl))
				{
					fadeIn.Add(control as GUIControl);
				}
			}
			return delegate
			{
				return Generic.BatchAnimationTransitionIn(bounceIn, fadeIn, fullSize, 0f);
			};
		}

		public static GUIWindow.AnimationPieceGeneratorDelegate GetCloseAnimation(GUIWindow window, Vector2 fullSize, params GUIControl[] ctrls)
		{
			List<GUIControl> bounceOut = new List<GUIControl>(ctrls);
			List<GUIControl> fadeOut = new List<GUIControl>();
			foreach (IGUIControl control in window.ControlList)
			{
				if (control is GUIControl && !bounceOut.Contains(control as GUIControl))
				{
					fadeOut.Add(control as GUIControl);
				}
			}
			return delegate
			{
				return Generic.BatchAnimationTransitionOut(bounceOut, fadeOut, fullSize, 0f);
			};
		}

		public static GUIWindow.AnimationPieceGeneratorDelegate FadeIn(float time, params GUIControl[] ctrls)
		{
			return delegate
			{
				GUIControl[] array = ctrls;
				foreach (GUIControl gUIControl in array)
				{
					gUIControl.AnimationAlpha = 0f;
				}
				return Absolute.AnimationAlpha(Path.Linear(0f, 1f, time), ctrls);
			};
		}

		public static GUIWindow.AnimationPieceGeneratorDelegate FadeOut(float time, params GUIControl[] ctrls)
		{
			return delegate
			{
				return Absolute.AnimationAlpha(Path.Linear(1f, 0f, time), ctrls);
			};
		}
	}

	public class Spline
	{
		public static AnimClip BSplineCurveOffsetXY(float time, GUIControl control, params Vector2[] pts)
		{
			return new AnimClipFunction(time, delegate(float t)
			{
				InterpolatorBSpline interpolatorBSpline = new InterpolatorBSpline();
				List<SplinePoint> ctrlPoints = new List<SplinePoint>(Array.ConvertAll(pts, delegate(Vector2 vec)
				{
					return new SplinePoint(new Vector3(vec.x, 0f, vec.y), Quaternion.identity);
				}));
				interpolatorBSpline.Initialize(ctrlPoints);
				Vector3 vector = interpolatorBSpline.GetVector(t);
				control.Offset = new Vector2(vector.x, vector.z);
			});
		}
	}

	public class Generic
	{
		public static AnimClip Pop(Vector2 newSize, Vector2 defaultSize, bool afterBounce, params GUIControl[] popCtrls)
		{
			AnimClip result = Blank();
			for (int i = 0; i < popCtrls.Length; i++)
			{
				popCtrls[i].SetSize(defaultSize);
				float delay = Mathf.Max(0.02f * (float)i + UnityEngine.Random.Range(-0.04f, 0.04f), 0f);
				if (afterBounce)
				{
					result ^= ChangeSize(popCtrls[i], newSize, defaultSize, 0.2f, delay);
				}
				else
				{
					result ^= ChangeSizeDirect(popCtrls[i], newSize, defaultSize, 0.2f, delay);
				}
			}
			return result;
		}

		public static AnimClip Blank()
		{
			return Absolute.Nothing(Path.Constant(0f, 0f), AnimClipBuilder.IgnoreWarningControl);
		}

		public static AnimClip Wait(float waitTime)
		{
			return Absolute.Nothing(Path.Constant(0f, waitTime), AnimClipBuilder.IgnoreWarningControl);
		}

		public static AnimClip BatchAnimationTransitionIn(List<GUIControl> bounceIn, List<GUIControl> fadeIn, Vector2 fullSize, float startAlpha)
		{
			return AnimationBounceTransitionIn(fullSize, startAlpha, bounceIn.ToArray()) ^ AnimationFadeTransitionIn(fadeIn.ToArray());
		}

		public static AnimClip BatchAnimationTransitionOut(List<GUIControl> bounceOut, List<GUIControl> fadeOut, Vector2 fullSize, float endAlpha)
		{
			return AnimationBounceTransitionOut(fullSize, endAlpha, bounceOut.ToArray()) ^ AnimationFadeTransitionOut(fadeOut.ToArray());
		}

		public static AnimClip AnimationBounceTransitionIn(Vector2 fullSize, float startAlpha, params GUIControl[] bounceIn)
		{
			return AnimationBounceTransitionIn(fullSize, startAlpha, 0f, bounceIn);
		}

		public static AnimClip AnimationBounceTransitionIn(Vector2 fullSize, float startAlpha, float delay, params GUIControl[] bounceIn)
		{
			foreach (GUIControl gUIControl in bounceIn)
			{
				gUIControl.AnimationAlpha = 0f;
			}
			return Absolute.SizeX(GenericPaths.BounceTransitionInX(fullSize.x, delay), bounceIn) ^ Absolute.SizeY(GenericPaths.BounceTransitionInY(fullSize.y, delay), bounceIn) ^ Absolute.AnimationAlpha(GenericPaths.BounceTransitionInAlpha(startAlpha, delay), bounceIn);
		}

		public static AnimClip AnimationFadeTransitionIn(params GUIControl[] fadeIn)
		{
			return AnimationFadeTransitionIn(0f, 1f, 0f, fadeIn);
		}

		public static AnimClip AnimationFadeTransitionIn(float delay, params GUIControl[] fadeIn)
		{
			return AnimationFadeTransitionIn(delay, 1f, 0f, fadeIn);
		}

		public static AnimClip AnimationFadeTransitionIn(float delay, float endingAlpha, params GUIControl[] fadeIn)
		{
			return AnimationFadeTransitionIn(delay, 1f, 0f, fadeIn);
		}

		public static AnimClip AnimationFadeTransitionIn(float delay, float endingAlpha, float additonalFadeInTime, params GUIControl[] fadeIn)
		{
			foreach (GUIControl gUIControl in fadeIn)
			{
				gUIControl.AnimationAlpha = 0f;
			}
			return Absolute.AnimationAlpha(Path.Constant(0f, 0.9f + delay) | (Path.Sin(0f, 0.25f, 0.2f + additonalFadeInTime) * endingAlpha), fadeIn);
		}

		public static AnimClip AnimationBounceTransitionOut(Vector2 fullSize, float startAlpha, params GUIControl[] bounceOut)
		{
			return AnimationBounceTransitionOut(fullSize, startAlpha, 0f, bounceOut);
		}

		public static AnimClip AnimationBounceTransitionOut(Vector2 fullSize, float endAlpha, float delay, params GUIControl[] bounceOut)
		{
			return Absolute.SizeX(GenericPaths.BounceTransitionOutX(fullSize.x, delay), bounceOut) ^ Absolute.SizeY(GenericPaths.BounceTransitionOutY(fullSize.y, delay), bounceOut) ^ Absolute.AnimationAlpha(GenericPaths.BounceTransitionOutAlpha(endAlpha, delay), bounceOut);
		}

		public static AnimClip AnimationFadeTransitionOut(params GUIControl[] fadeOut)
		{
			return AnimationFadeTransitionOut(0f, 1f, 0f, fadeOut);
		}

		public static AnimClip AnimationFadeTransitionOut(float delay, params GUIControl[] fadeOut)
		{
			return AnimationFadeTransitionOut(delay, 1f, 0f, fadeOut);
		}

		public static AnimClip AnimationFadeTransitionOut(float delay, float startingAlpha, params GUIControl[] fadeOut)
		{
			return AnimationFadeTransitionOut(delay, 1f, 0f, fadeOut);
		}

		public static AnimClip AnimationFadeTransitionOut(float delay, float startingAlpha, float additonalFadeOutTime, params GUIControl[] fadeOut)
		{
			return Absolute.AnimationAlpha(Path.Constant(startingAlpha, delay) | (Path.Sin((float)Math.PI / 2f, 0.25f, 0.2f + additonalFadeOutTime) * startingAlpha), fadeOut);
		}

		public static AnimClip FullAnimationOffset(GUIControl control, float finalRotation, Vector2 finalO, Vector2 finalS, float finalAlpha, float time)
		{
			AnimClip pieceOne = Absolute.Alpha(Path.Linear(control.Alpha, finalAlpha, time), control) ^ Absolute.Rotation(Path.Linear(control.Rotation, finalRotation, time), control);
			Vector2 offset = control.Offset;
			AnimClip pieceOne2 = pieceOne ^ Absolute.OffsetX(Path.Linear(offset.x, finalO.x, time), control);
			Vector2 offset2 = control.Offset;
			AnimClip pieceOne3 = pieceOne2 ^ Absolute.OffsetY(Path.Linear(offset2.y, finalO.y, time), control);
			Vector2 size = control.Size;
			AnimClip pieceOne4 = pieceOne3 ^ Absolute.SizeX(Path.Linear(size.x, finalS.x, time), control);
			Vector2 size2 = control.Size;
			return pieceOne4 ^ Absolute.SizeY(Path.Linear(size2.y, finalS.y, time), control);
		}

		public static AnimClip SizeChangeWithEndBounce(float startSize, float finalSize, float time, float bounceTime, params GUIControl[] ctrls)
		{
			return Absolute.SizeXY(Path.Chained(Path.Linear(startSize, finalSize, time), Path.Sin(0f, 1f, bounceTime) * (finalSize * 0.12f) * Path.Linear(1f, 0f, bounceTime) + finalSize), ctrls);
		}

		public static AnimClip FadeInVis(List<GUIControl> toFade, float time)
		{
			toFade.ForEach(delegate(GUIControl ctrl)
			{
				ctrl.IsVisible = true;
			});
			AnimClip result = Blank();
			foreach (GUIControl item in toFade)
			{
				result ^= FadeIn(item, time);
			}
			return result;
		}

		public static AnimClip FadeOutVis(List<GUIControl> toFade, float time)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Expected O, but got Unknown
			AnimClip animClip = Blank();
			foreach (GUIControl item in toFade)
			{
				animClip ^= FadeOut(item, time);
			}
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				toFade.ForEach(delegate(GUIControl ctrl)
				{
					ctrl.IsVisible = false;
				});
			};
			return animClip;
		}

		public static AnimClip FadeIn(List<GUIControl> toFade, float time)
		{
			AnimClip result = Blank();
			foreach (GUIControl item in toFade)
			{
				result ^= FadeIn(item, time);
			}
			return result;
		}

		public static AnimClip FadeOut(List<GUIControl> toFade, float time)
		{
			AnimClip result = Blank();
			foreach (GUIControl item in toFade)
			{
				result ^= FadeOut(item, time);
			}
			return result;
		}

		public static AnimClip FadeInVis(GUIControl ctrl, float time)
		{
			ctrl.IsVisible = true;
			return FadeIn(ctrl, time);
		}

		public static AnimClip FadeOutVis(GUIControl ctrl, float time)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			AnimClip animClip = FadeOut(ctrl, time);
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				ctrl.IsVisible = false;
			};
			return animClip;
		}

		public static AnimClip FadeIn(GUIControl ctrl, float time)
		{
			return Absolute.Alpha(Path.Linear(ctrl.Alpha, 1f, (1f - ctrl.Alpha) * time), ctrl);
		}

		public static AnimClip FadeOut(GUIControl ctrl, float time)
		{
			return Absolute.Alpha(Path.Linear(ctrl.Alpha, 0f, ctrl.Alpha * time), ctrl);
		}

		public static AnimClip ChangeSize(GUIControl ctrl, Vector2 newSize, Vector2 defaultSize, float time)
		{
			return ChangeSize(ctrl, newSize, defaultSize, time, 0f);
		}

		public static AnimClip ChangeSize(GUIControl ctrl, Vector2 newSize, Vector2 defaultSize, float time, float delay)
		{
			float x = defaultSize.x;
			float x2 = newSize.x;
			Vector2 size = ctrl.Size;
			float time2 = GenericFunctions.FrationalTime(x, x2, size.x, time);
			float y = defaultSize.y;
			float y2 = newSize.y;
			Vector2 size2 = ctrl.Size;
			float time3 = GenericFunctions.FrationalTime(y, y2, size2.y, time);
			Vector2 size3 = ctrl.Size;
			AnimPath a = Path.Constant(size3.x, delay);
			Vector2 size4 = ctrl.Size;
			AnimClip pieceOne = Absolute.SizeX(a | GenericPaths.LinearWithWiggle(size4.x, newSize.x, time2), ctrl);
			Vector2 size5 = ctrl.Size;
			AnimPath a2 = Path.Constant(size5.y, delay);
			Vector2 size6 = ctrl.Size;
			return pieceOne ^ Absolute.SizeY(a2 | GenericPaths.LinearWithWiggle(size6.y, newSize.y, time3), ctrl);
		}

		public static AnimClip ChangeSizeDirect(GUIControl ctrl, Vector2 newSize, Vector2 defaultSize, float time, float delay)
		{
			float x = defaultSize.x;
			float x2 = newSize.x;
			Vector2 size = ctrl.Size;
			float time2 = GenericFunctions.FrationalTime(x, x2, size.x, time);
			float y = defaultSize.y;
			float y2 = newSize.y;
			Vector2 size2 = ctrl.Size;
			float time3 = GenericFunctions.FrationalTime(y, y2, size2.y, time);
			Vector2 size3 = ctrl.Size;
			AnimPath a = Path.Constant(size3.x, delay);
			Vector2 size4 = ctrl.Size;
			AnimClip pieceOne = Absolute.SizeX(a | Path.Linear(size4.x, newSize.x, time2), ctrl);
			Vector2 size5 = ctrl.Size;
			AnimPath a2 = Path.Constant(size5.y, delay);
			Vector2 size6 = ctrl.Size;
			return pieceOne ^ Absolute.SizeY(a2 | Path.Linear(size6.y, newSize.y, time3), ctrl);
		}

		public static AnimClip ChangeVisibility(bool visible, params GUIControl[] ctrls)
		{
			return new AnimClipFunction(0f, delegate
			{
				GUIControl[] array = ctrls;
				foreach (GUIControl gUIControl in array)
				{
					gUIControl.IsVisible = visible;
				}
			});
		}

		public static AnimClip WheelOfWinSpin(float firstRotationAmount, float firstRotationTime, float secondRotationAmount, float secondRotationTime, params GUIControl[] ctrls)
		{
			return Delta.Rotation(Path.Chained(Path.Linear(0f, firstRotationAmount, firstRotationTime), firstRotationAmount + (1f - Path.Pow(Path.Linear(1f, 0f, secondRotationTime), Path.Constant(2f, 0f))) * secondRotationAmount), ctrls);
		}

		public static AnimClip WheelOfWinSpin(float totalRotation, float totalTime, params GUIControl[] ctrls)
		{
			float num = totalRotation * 2f / 3f;
			float time = totalTime * 0.5f;
			float a = totalRotation * 1f / 3f;
			float time2 = totalTime * 0.5f;
			return Delta.Rotation(Path.Chained(Path.Linear(0f, num, time), num + (1f - Path.Pow(Path.Linear(1f, 0f, time2), Path.Constant(2f, 0f))) * a), ctrls);
		}
	}

	public class GenericPaths
	{
		public static AnimPath LinearWithBounce(float start, float end, float time, float bounceMag, float bounceTime)
		{
			return Path.Linear(start, end, time) | (Path.Sin(0f, 0.5f, bounceTime) * bounceMag * SignW0(start - end) + end);
		}

		public static AnimPath LinearWithBounce(float start, float end, float time)
		{
			return LinearWithBounce(start, end, time, Mathf.Abs(end - start) * 0.075f, time);
		}

		public static AnimPath LinearWithWiggle(float start, float end, float time, float bounceMag, float bounceTime)
		{
			return Path.Linear(start, end, time) | (Path.Sin(0f, 1f, bounceTime) * bounceMag * SignW0(start - end) * Path.Linear(1f, 0f, bounceTime) + end);
		}

		public static AnimPath AddBounce(AnimPath pathToAddBounceToo, float start, float end)
		{
			float a = Mathf.Abs(end - start) * 0.075f;
			float totalTime = pathToAddBounceToo.TotalTime;
			return pathToAddBounceToo | (Path.Sin(0f, 0.5f, totalTime) * a * SignW0(start - end) + end);
		}

		public static AnimPath LinearWithWiggle(float start, float end, float time)
		{
			return LinearWithWiggle(start, end, time, (0f - Mathf.Abs(end - start)) * 0.1597f, time);
		}

		public static AnimPath LinearWithSingleWiggle(float start, float end, float time)
		{
			return LinearWithMutedSingleWiggle(start, end, 1f, time);
		}

		public static AnimPath LinearWithMutedSingleWiggle(float start, float end, float mod, float time)
		{
			float a = (0f - Mathf.Abs(end - start)) * 0.1597f * mod;
			float num = time * 0.5f;
			return Path.Linear(start, end, time) | (Path.Sin(0f, 0.5f, num) * a * SignW0(start - end) * Path.Linear(1f, 0f, num) + end);
		}

		public static AnimPath LinearWith2xSingleWiggle(float start, float end, float time)
		{
			float a = 0f - Mathf.Abs(end - start);
			float num = 4.141593f;
			float time2 = time * (1f / num);
			float num2 = time * ((float)Math.PI / num);
			return Path.Linear(start, end, time2) | (Path.Sin(0f, 0.5f, num2) * a * SignW0(start - end) * Path.Linear(1f, 0f, num2) + end);
		}

		public static AnimPath BounceTransitionInX(float FullSizeX, float delay)
		{
			return Path.Constant(0f, delay) | (FullSizeX * Path.SmoothChainedWithCut(0.105f, Path.Sin(0f, 1.25f, 0.4f) * 0.3f + Path.Linear(0f, 1.1f, 0.4f), Path.Cos(0f, 1f, 0.6f) * Path.Linear(0.2f, 0f, 0.6f) + Path.Linear(1f, 1f, 0f)));
		}

		public static AnimPath BounceTransitionInY(float FullSizeY, float delay)
		{
			return Path.Constant(0f, delay) | (FullSizeY * Path.SmoothChainedWithCut(0.073f, Path.Linear(0f, 1.4f, 0.231f) + Path.Sin((float)Math.PI, 0.5f, 0.231f) * 0.45f, Path.Cos(0f, 0.75f, 0.231f) * 0.4f + 1f, Path.Linear(1f, 1f, 0f) + Path.Sin(0f, 1f, 0.538f) * Path.Linear(0.1f, 0f, 0.538f)));
		}

		public static AnimPath BounceTransitionInAlpha(float startAlpha, float delay)
		{
			return Path.Constant(startAlpha, delay) | Path.Linear(startAlpha, 1f, 1f);
		}

		public static AnimPath BounceTransitionOutX(float FullSizeX, float delay)
		{
			return Path.Constant(FullSizeX, 0.2f + delay) | (FullSizeX * Path.SmoothChainedWithCut(0.1f, 1f + Path.Sin(0f, 0.5f, 0.154f) * 0.2f, Path.Pow(Path.Linear(1f, 0f, 0.408f), Path.Constant(3f, 0f)) * 0.7f + 0.3f));
		}

		public static AnimPath BounceTransitionOutY(float FullSizeY, float delay)
		{
			return Path.Constant(FullSizeY, 0.2f + delay) | (FullSizeY * Path.SmoothChainedWithCut(0.1f, 1f - Path.Sin(0f, 0.75f, 0.281f) * Path.Linear(0f, 0.4f, 0.281f), 1.4f - Path.Linear(0f, 1.4f, 0.331f) - Path.Sin(0f, 0.5f, 0.331f) * 0.45f));
		}

		public static AnimPath BounceTransitionOutAlpha(float endAlpha, float delay)
		{
			return Path.Constant(1f, 0.2f + delay) | Path.Linear(1f, endAlpha, 0.462f);
		}

		private static float SignW0(float a)
		{
			if (a == 0f)
			{
				return 0f;
			}
			if (a > 0f)
			{
				return 1f;
			}
			return -1f;
		}
	}

	public class GenericFunctions
	{
		public static float FrationalTime(float expectedStartValue, float expectedEndValue, float actualValue, float expectedTime)
		{
			return Mathf.Abs((actualValue - expectedEndValue) / (expectedStartValue - expectedEndValue)) * expectedTime;
		}
	}
}
