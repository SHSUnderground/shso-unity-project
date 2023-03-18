using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Splines/Follow")]
public class SplineFollow : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum Mode
	{
		Stop,
		Loop
	}

	public SplineController spline;

	public Mode mode = Mode.Loop;

	public bool useRootMotion;

	public bool orientToMotion;

	public bool followRotations = true;

	public float timeStartOffset;

	protected float motionExportVelocity;

	protected Transform motionExportTransform;

	protected Vector3 motionExportPosition = Vector3.zero;

	protected bool advanceSpeed(ref float time, float dt, out bool reset)
	{
		if (useRootMotion)
		{
			time = spline.TimeForDistance(time, motionExportVelocity * dt);
		}
		else
		{
			float desiredSpeed = spline.GetDesiredSpeed(time * spline.traversalTime);
			time = spline.TimeForDistance(time, desiredSpeed * dt);
		}
		reset = false;
		if (time >= 1f)
		{
			if (mode != Mode.Loop)
			{
				return false;
			}
			reset = true;
			time = 0f;
		}
		return true;
	}

	protected IEnumerator MoveByVelocity()
	{
		yield return 0;
		if (spline == null)
		{
			CspUtils.DebugLog("Spline is null in <" + base.gameObject.name + ">");
			yield break;
		}
		float time = timeStartOffset;
		int idxEffect = 0;
		int idxAnim = 0;
		Animation animationComponent = GetComponentInChildren(typeof(Animation)) as Animation;
		EffectSequenceList effectsList = GetComponentInChildren(typeof(EffectSequenceList)) as EffectSequenceList;
		List<SplineEffectSequence> effects = spline.GetEffects();
		List<SplineAnimation> anims = spline.GetAnimations();
		Dictionary<string, EffectSequence> activeEffects = new Dictionary<string, EffectSequence>();
		while (true)
		{
			bool reset = false;
			if (!advanceSpeed(ref time, Time.deltaTime, out reset))
			{
				break;
			}
			if (reset)
			{
				idxEffect = 0;
				idxAnim = 0;
				foreach (EffectSequence seq3 in activeEffects.Values)
				{
					seq3.Cancel();
				}
				activeEffects.Clear();
			}
			if (effects != null && effectsList != null)
			{
				for (; idxEffect < effects.Count; idxEffect++)
				{
					SplineEffectSequence j = effects[idxEffect];
					if (!(time * spline.traversalTime >= j.time))
					{
						break;
					}
					if (!j.turnOff)
					{
						EffectSequence seq2 = effectsList.GetLogicalEffectSequence(j.name);
						if (seq2 != null)
						{
							seq2.Initialize(null, null, null);
							seq2.StartSequence();
							activeEffects.Add(j.name, seq2);
						}
					}
					else
					{
						EffectSequence seq = null;
						if (activeEffects.TryGetValue(j.name, out seq))
						{
							activeEffects.Remove(j.name);
							seq.Cancel();
						}
					}
				}
			}
			if (anims != null && animationComponent != null)
			{
				for (; idxAnim < anims.Count; idxAnim++)
				{
					SplineAnimation i = anims[idxAnim];
					if (!(time * spline.traversalTime >= i.time))
					{
						break;
					}
					AnimationState state = animationComponent[i.animation];
					if (state != null)
					{
						if (i.looping)
						{
							state.wrapMode = WrapMode.Loop;
						}
						else
						{
							state.wrapMode = WrapMode.ClampForever;
						}
						animationComponent.Rewind(i.animation);
						animationComponent.CrossFade(i.animation, i.blendTime);
					}
				}
			}
			Vector3 oldPosition = base.transform.position;
			base.transform.position = spline.EvalPosition(time);
			if (orientToMotion)
			{
				Vector3 dir = (base.transform.position - oldPosition).normalized;
				if (dir.sqrMagnitude > 0.5f)
				{
					base.transform.rotation = Quaternion.LookRotation(dir);
				}
			}
			else if (followRotations)
			{
				base.transform.rotation = spline.EvalRotation(time);
			}
			yield return 0;
		}
	}

	public void Start()
	{
		motionExportVelocity = 0f;
		motionExportTransform = Utils.FindNodeInChildren(base.transform, "motion_export");
		if (motionExportTransform != null)
		{
			motionExportPosition = motionExportTransform.localPosition;
		}
		else
		{
			motionExportPosition = Vector3.zero;
		}
		StartCoroutine(MoveByVelocity());
	}

	public void LateUpdate()
	{
		if (motionExportTransform != null)
		{
			float y = motionExportPosition.y;
			Vector3 localPosition = motionExportTransform.localPosition;
			motionExportVelocity = (y - localPosition.y) / Time.deltaTime;
			if (motionExportVelocity < 0f)
			{
				motionExportVelocity = 0f;
			}
			motionExportPosition = motionExportTransform.localPosition;
		}
	}
}
