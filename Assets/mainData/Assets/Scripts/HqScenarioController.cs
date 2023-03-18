using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Hq/Switch/Scenario Controller")]
public class HqScenarioController : HqSwitchController
{
	protected class FlipAnimationData
	{
		public float currentSpeed;

		public float currentTime;

		public FlipAnimationData(float speed, float time)
		{
			currentSpeed = speed;
			currentTime = time;
		}
	}

	public List<GameObject> onObjects = new List<GameObject>();

	public List<GameObject> offObjects = new List<GameObject>();

	public List<GameObject> effectSequences = new List<GameObject>();

	public List<GameObject> flipAnimations = new List<GameObject>();

	public List<GameObject> onAnimations = new List<GameObject>();

	public List<HqTrigger> triggers = new List<HqTrigger>();

	protected List<GameObject> activeSequences;

	protected bool isOn;

	protected bool unloading;

	protected Dictionary<AnimationState, FlipAnimationData> currentAnimationData;

	protected void Update()
	{
		foreach (GameObject onObject in onObjects)
		{
			if (onObject.active && !isOn)
			{
				Utils.ActivateTree(onObject, false);
			}
			else if (!onObject.active && isOn)
			{
				Utils.ActivateTree(onObject, true);
			}
		}
		foreach (GameObject offObject in offObjects)
		{
			if (!offObject.active && !isOn)
			{
				Utils.ActivateTree(offObject, true);
			}
			else if (offObject.active && isOn)
			{
				Utils.ActivateTree(offObject, false);
			}
		}
		if (!isOn && activeSequences != null)
		{
			foreach (GameObject activeSequence in activeSequences)
			{
				EffectSequence component = Utils.GetComponent<EffectSequence>(activeSequence, Utils.SearchChildren);
				component.Cancel();
				Object.Destroy(activeSequence);
			}
			activeSequences.Clear();
			activeSequences = null;
		}
		if (isOn)
		{
			foreach (GameObject effectSequence in effectSequences)
			{
				bool flag = false;
				if (activeSequences != null)
				{
					foreach (GameObject activeSequence2 in activeSequences)
					{
						if (activeSequence2.name == effectSequence.name)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					GameObject gameObject = Object.Instantiate(effectSequence) as GameObject;
					EffectSequence component2 = Utils.GetComponent<EffectSequence>(gameObject, Utils.SearchChildren);
					if (component2 != null)
					{
						component2.Initialize(base.gameObject, null, null);
						component2.StartSequence();
						gameObject.name = effectSequence.name;
						if (activeSequences == null)
						{
							activeSequences = new List<GameObject>();
						}
						activeSequences.Add(gameObject);
					}
				}
			}
		}
		if (currentAnimationData != null)
		{
			foreach (GameObject flipAnimation in flipAnimations)
			{
				if (flipAnimation != null && flipAnimation.animation != null)
				{
					AnimationState animationState = flipAnimation.animation[flipAnimation.animation.clip.name];
					if (animationState != null)
					{
						if (animationState.time < 0f)
						{
							animationState.time = 0f;
						}
						if (animationState.time > flipAnimation.animation.clip.length)
						{
							animationState.time = flipAnimation.animation.clip.length;
						}
						if (currentAnimationData.ContainsKey(animationState))
						{
							currentAnimationData[animationState].currentSpeed = animationState.speed;
							currentAnimationData[animationState].currentTime = animationState.time;
						}
						else
						{
							currentAnimationData[animationState] = new FlipAnimationData(animationState.speed, animationState.time);
						}
						if (animationState.speed > 0f || (animationState.speed < 0f && !flipAnimation.animation.IsPlaying(flipAnimation.animation.clip.name)))
						{
							flipAnimation.animation.Play();
						}
					}
				}
			}
		}
		foreach (HqTrigger trigger in triggers)
		{
			if (!isOn && trigger.IsOn)
			{
				trigger.TurnOff();
			}
			else if (isOn && !trigger.IsOn)
			{
				trigger.TurnOn();
			}
		}
	}

	public void OnUnload()
	{
		unloading = true;
	}

	public void OnDisable()
	{
		if (activeSequences == null)
		{
			return;
		}
		if (!unloading)
		{
			for (int num = activeSequences.Count - 1; num >= 0; num--)
			{
				GameObject gameObject = activeSequences[num];
				if (gameObject != null)
				{
					Object.Destroy(gameObject);
				}
			}
		}
		activeSequences.Clear();
		activeSequences = null;
	}

	public void OnEnable()
	{
		if (currentAnimationData != null)
		{
			foreach (AnimationState key in currentAnimationData.Keys)
			{
				key.speed = currentAnimationData[key].currentSpeed;
				key.time = currentAnimationData[key].currentTime;
			}
		}
	}

	public override void Flip()
	{
		if (flipAnimations.Count > 0 && currentAnimationData == null)
		{
			currentAnimationData = new Dictionary<AnimationState, FlipAnimationData>();
		}
		foreach (GameObject flipAnimation in flipAnimations)
		{
			float speed = 1f;
			if (isOn)
			{
				speed = -1f;
			}
			AnimationState animationState = flipAnimation.animation[flipAnimation.animation.clip.name];
			if (animationState.time < 0f)
			{
				animationState.time = 0f;
			}
			if (animationState.time > flipAnimation.animation.clip.length)
			{
				animationState.time = flipAnimation.animation.clip.length;
			}
			if (!currentAnimationData.ContainsKey(animationState))
			{
				currentAnimationData[animationState] = new FlipAnimationData(speed, animationState.time);
			}
			animationState.wrapMode = WrapMode.ClampForever;
			flipAnimation.animation[flipAnimation.animation.clip.name].speed = speed;
			flipAnimation.animation.Play();
		}
		foreach (GameObject onAnimation in onAnimations)
		{
			if (!isOn)
			{
				AnimationState animationState2 = onAnimation.animation[onAnimation.animation.clip.name];
				animationState2.wrapMode = WrapMode.Loop;
				animationState2.speed = 1f;
				onAnimation.animation.Sample();
				onAnimation.animation.Play();
			}
			else
			{
				onAnimation.animation[onAnimation.animation.clip.name].speed = 0f;
			}
		}
		isOn = !isOn;
	}

	public override bool CanUse()
	{
		return HqController2.Instance.State == typeof(HqController2.HqControllerFlinga);
	}
}
