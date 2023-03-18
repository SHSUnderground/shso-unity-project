using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Splines/Spline Controller")]
public class SplineController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum SplineType
	{
		Linear,
		BSpline,
		Catmull
	}

	private class ControlPointCompare : IComparer<SplineControlPoint>
	{
		public int Compare(SplineControlPoint x, SplineControlPoint y)
		{
			return x.name.CompareTo(y.name);
		}
	}

	public static bool drawSplines = true;

	public bool DrawOnSelectionOnly = true;

	public bool DrawCurves;

	public bool AutoClose;

	public SplineType splineType = SplineType.BSpline;

	public float traversalTime = 5f;

	public SplineEffectSequence[] effects;

	public SplineAnimation[] animations;

	public SplineSpeed[] speeds;

	public SplineRotation[] rotations;

	protected SplineInterpolator splineEval;

	protected List<SplineEffectSequence> effectsSorted;

	protected List<SplineAnimation> animationsSorted;

	protected List<SplineSpeed> speedsSorted;

	protected List<SplineRotation> rotationsSorted;

	private void Start()
	{
		Initialize();
	}

	public Vector3 EvalPosition(float time)
	{
		return splineEval.GetVector(time);
	}

	public Quaternion EvalRotation(float time)
	{
		if (rotationsSorted == null || rotationsSorted.Count < 4)
		{
			return Quaternion.identity;
		}
		time *= traversalTime;
		if (time <= rotationsSorted[0].time)
		{
			return rotationsSorted[0].rot;
		}
		int index = rotationsSorted.Count - 1;
		if (time >= rotationsSorted[index].time)
		{
			return rotationsSorted[index].rot;
		}
		index = rotationsSorted.Count - 3;
		int i;
		for (i = 1; i < index && !(time < rotationsSorted[i + 1].time); i++)
		{
		}
		float t = (time - rotationsSorted[i].time) / (rotationsSorted[i + 1].time - rotationsSorted[i].time);
		Quaternion rot = rotationsSorted[i - 1].rot;
		Quaternion rot2 = rotationsSorted[i].rot;
		Quaternion rot3 = rotationsSorted[i + 1].rot;
		Quaternion rot4 = rotationsSorted[i + 2].rot;
		Quaternion squadIntermediate = QuaternionEx.GetSquadIntermediate(rot, rot2, rot3);
		Quaternion squadIntermediate2 = QuaternionEx.GetSquadIntermediate(rot2, rot3, rot4);
		return QuaternionEx.GetQuatSquad(t, rot2, rot3, squadIntermediate, squadIntermediate2);
	}

	public void GetFirstPoint(out Vector3 pos, out Quaternion rot)
	{
		pos = EvalPosition(0f);
		rot = EvalRotation(0f);
	}

	public float TimeForDistance(float t, float s)
	{
		return splineEval.TimeFromDistance(t, s);
	}

	public float ArcLength()
	{
		return splineEval.ArcLength();
	}

	public float ArcLength(float t0, float t1)
	{
		return splineEval.ArcLength(t0, t1);
	}

	public List<SplineAnimation> GetAnimations()
	{
		return animationsSorted;
	}

	public List<SplineEffectSequence> GetEffects()
	{
		return effectsSorted;
	}

	public List<SplineSpeed> GetSpeeds()
	{
		return speedsSorted;
	}

	public float GetDesiredSpeed(float t)
	{
		int num = speedsSorted.Count - 1;
		if ((double)t <= 0.0)
		{
			return speedsSorted[0].speed;
		}
		if (t >= speedsSorted[num].time)
		{
			return speedsSorted[num].speed;
		}
		int i;
		for (i = 0; i < num && !(t <= speedsSorted[i + 1].time); i++)
		{
		}
		SplineSpeed splineSpeed = speedsSorted[i];
		SplineSpeed splineSpeed2 = speedsSorted[i + 1];
		float t2 = (t - splineSpeed.time) / (splineSpeed2.time - splineSpeed.time);
		return Mathf.Lerp(splineSpeed.speed, splineSpeed2.speed, t2);
	}

	private void Initialize()
	{
		List<SplineControlPoint> list = new List<SplineControlPoint>();
		Component[] componentsInChildren = GetComponentsInChildren(typeof(SplineControlPoint));
		foreach (Component component in componentsInChildren)
		{
			SplineControlPoint splineControlPoint = component as SplineControlPoint;
			if (splineControlPoint != null)
			{
				list.Add(splineControlPoint);
			}
		}
		list.Sort(new ControlPointCompare());
		if (list.Count < 2)
		{
			throw new ArgumentOutOfRangeException("Need at least 2 control points");
		}
		List<SplinePoint> list2 = new List<SplinePoint>();
		for (int j = 0; j < list.Count; j++)
		{
			list2.Add(new SplinePoint(list[j]));
		}
		if (AutoClose)
		{
			list2.Add(new SplinePoint(list[0]));
		}
		splineEval = SplineInterpolatorFactory();
		if (!splineEval.Initialize(list2))
		{
			splineEval = null;
			return;
		}
		if (rotations == null || rotations.Length <= 0)
		{
			rotationsSorted = new List<SplineRotation>(2);
			rotationsSorted.Add(new SplineRotation(list[0].transform.rotation, 0f));
			rotationsSorted.Add(new SplineRotation(list[0].transform.rotation, traversalTime));
		}
		else if (rotations.Length == 1)
		{
			if ((double)rotations[0].time <= 0.0)
			{
				rotationsSorted = new List<SplineRotation>(2);
				rotationsSorted.Add(new SplineRotation(rotations[0].rot, 0f));
				rotationsSorted.Add(new SplineRotation(rotations[0].rot, traversalTime));
			}
			else if (rotations[0].time < traversalTime)
			{
				rotationsSorted = new List<SplineRotation>(3);
				rotationsSorted.Add(new SplineRotation(list[0].transform.rotation, 0f));
				rotationsSorted.Add(new SplineRotation(rotations[0].rot, rotations[0].time));
				rotationsSorted.Add(new SplineRotation(rotations[0].rot, traversalTime));
			}
			else if (rotations[0].time >= traversalTime)
			{
				rotationsSorted = new List<SplineRotation>(2);
				rotationsSorted.Add(new SplineRotation(list[0].transform.rotation, 0f));
				rotationsSorted.Add(new SplineRotation(rotations[0].rot, traversalTime));
			}
		}
		else
		{
			rotationsSorted = new List<SplineRotation>(rotations.Length);
			SplineRotation[] array = rotations;
			foreach (SplineRotation splineRotation in array)
			{
				if (splineRotation.time < 0f)
				{
					splineRotation.time = 0f;
				}
				else if (splineRotation.time > traversalTime)
				{
					splineRotation.time = traversalTime;
				}
				rotationsSorted.Add(splineRotation);
			}
		}
		rotationsSorted.Sort(delegate(SplineRotation a, SplineRotation b)
		{
			return a.time.CompareTo(b.time);
		});
		rotationsSorted.Insert(0, new SplineRotation(rotationsSorted[0]));
		rotationsSorted.Add(new SplineRotation(rotationsSorted[rotationsSorted.Count - 1]));
		effectsSorted = new List<SplineEffectSequence>(effects.Length);
		SplineEffectSequence[] array2 = effects;
		foreach (SplineEffectSequence splineEffectSequence in array2)
		{
			if (splineEffectSequence.time < 0f)
			{
				splineEffectSequence.time = 0f;
			}
			else if (splineEffectSequence.time > traversalTime)
			{
				splineEffectSequence.time = traversalTime;
			}
			effectsSorted.Add(splineEffectSequence);
		}
		effectsSorted.Sort(delegate(SplineEffectSequence a, SplineEffectSequence b)
		{
			return a.time.CompareTo(b.time);
		});
		animationsSorted = new List<SplineAnimation>(animations.Length);
		SplineAnimation[] array3 = animations;
		foreach (SplineAnimation splineAnimation in array3)
		{
			if (splineAnimation.time < 0f)
			{
				splineAnimation.time = 0f;
			}
			else if (splineAnimation.time > traversalTime)
			{
				splineAnimation.time = traversalTime;
			}
			animationsSorted.Add(splineAnimation);
		}
		animationsSorted.Sort(delegate(SplineAnimation a, SplineAnimation b)
		{
			return a.time.CompareTo(b.time);
		});
		for (int n = 1; n < rotationsSorted.Count; n++)
		{
			if (Quaternion.Dot(rotationsSorted[n].rot, rotationsSorted[n - 1].rot) < 0f)
			{
				SplineRotation splineRotation2 = rotationsSorted[n];
				splineRotation2.rot.x = 0f - splineRotation2.rot.x;
				splineRotation2.rot.y = 0f - splineRotation2.rot.y;
				splineRotation2.rot.z = 0f - splineRotation2.rot.z;
				splineRotation2.rot.w = 0f - splineRotation2.rot.w;
			}
		}
		speedsSorted = new List<SplineSpeed>(speeds.Length);
		bool flag = false;
		SplineSpeed[] array4 = speeds;
		foreach (SplineSpeed splineSpeed in array4)
		{
			if (splineSpeed.time < 0f)
			{
				splineSpeed.time = 0f;
			}
			else if (splineSpeed.time > traversalTime)
			{
				splineSpeed.time = traversalTime;
			}
			if (splineSpeed.time == 0f)
			{
				flag = true;
			}
			speedsSorted.Add(splineSpeed);
		}
		if (!flag)
		{
			SplineSpeed splineSpeed2 = new SplineSpeed();
			splineSpeed2.time = 0f;
			splineSpeed2.speed = splineEval.ArcLength() / traversalTime;
			speedsSorted.Add(splineSpeed2);
		}
		speedsSorted.Sort(delegate(SplineSpeed a, SplineSpeed b)
		{
			return a.time.CompareTo(b.time);
		});
	}

	protected SplineInterpolator SplineInterpolatorFactory()
	{
		switch (splineType)
		{
		case SplineType.Linear:
			return new InterpolatorLinear();
		case SplineType.BSpline:
			return new InterpolatorBSpline();
		case SplineType.Catmull:
			return new InterpolatorCatmull();
		default:
			return null;
		}
	}

	private void OnDrawGizmos()
	{
		if (drawSplines && !DrawOnSelectionOnly)
		{
			DrawGizmos();
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (drawSplines && DrawOnSelectionOnly)
		{
			DrawGizmos();
		}
	}

	protected void DrawGizmos()
	{
		Initialize();
		if (splineEval == null)
		{
			return;
		}
		splineEval.DrawGizmos();
		if (DrawCurves)
		{
			for (float num = 0f; num <= 1f; num += 0.01f)
			{
				Vector3 from = EvalPosition(num);
				Quaternion rotation = EvalRotation(num);
				Gizmos.color = Color.blue;
				Gizmos.DrawRay(from, rotation * Vector3.forward);
				Gizmos.color = Color.red;
				Gizmos.DrawRay(from, rotation * Vector3.right);
				Gizmos.color = Color.green;
				Gizmos.DrawRay(from, rotation * Vector3.up);
			}
		}
		for (int i = 0; i < animationsSorted.Count; i++)
		{
			Vector3 vector = splineEval.GetVector(animationsSorted[i].time / traversalTime);
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(vector, 0.3f);
		}
		for (int j = 0; j < effectsSorted.Count; j++)
		{
			Vector3 vector2 = splineEval.GetVector(effectsSorted[j].time / traversalTime);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(vector2, 0.3f);
		}
		for (int k = 0; k < speeds.Length; k++)
		{
			Vector3 vector3 = splineEval.GetVector(speeds[k].time / traversalTime);
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(vector3, 0.2f);
		}
	}
}
