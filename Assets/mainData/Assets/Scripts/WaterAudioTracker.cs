using System.Runtime.CompilerServices;
using UnityEngine;

public class WaterAudioTracker : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private const string kSplashSequenceName = "Splash sequence";

	private const string kWadeSequenceName = "Wade sequence";

	private const string kIdleAnimation = "movement_idle";

	private EffectSequenceList effectsList;

	private bool moving;

	private bool crossfadingIn;

	private bool crossfadingOut;

	private float stepStartTime = -1f;

	private float stepDuration = -1f;

	private readonly string[] kMoveAnimations = new string[3]
	{
		"movement_run",
		"pickup_run",
		"attack_charge"
	};

	[CompilerGenerated]
	private bool _003CUseVO_003Ek__BackingField;

	public bool UseVO
	{
		[CompilerGenerated]
		get
		{
			return _003CUseVO_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CUseVO_003Ek__BackingField = value;
		}
	}

	public static WaterAudioTracker AddTracker(Collider collider)
	{
		WaterAudioTracker waterAudioTracker = collider.gameObject.GetComponent<WaterAudioTracker>();
		if (waterAudioTracker == null)
		{
			waterAudioTracker = collider.gameObject.AddComponent<WaterAudioTracker>();
		}
		return waterAudioTracker;
	}

	public static void RemoveTracker(Collider collider)
	{
		WaterAudioTracker component = collider.gameObject.GetComponent<WaterAudioTracker>();
		if (component != null)
		{
			Object.Destroy(component);
		}
	}

	public static void ManualPlaySplash(GameObject character)
	{
		WaterAudioTracker component = character.GetComponent<WaterAudioTracker>();
		if (component != null)
		{
			component.PlaySplash();
		}
	}

	private void Start()
	{
		effectsList = base.gameObject.GetComponent<EffectSequenceList>();
		PlayVO();
		PlaySplash();
	}

	private void Update()
	{
		AnimationState moveAnim = GetMoveAnim();
		bool flag = moveAnim != null;
		if (!moving && flag)
		{
			moving = true;
			stepStartTime = Time.time;
			stepDuration = CalculateStepDuration(moveAnim);
			crossfadingIn = true;
		}
		else if (moving && !flag)
		{
			moving = false;
			crossfadingIn = false;
			crossfadingOut = false;
		}
		else if (moving)
		{
			bool flag2 = IsIdling();
			if (!flag2)
			{
				crossfadingIn = false;
				crossfadingOut = false;
			}
			else if (!crossfadingIn && flag2)
			{
				crossfadingOut = true;
			}
			if (!crossfadingOut && Time.time > stepStartTime + stepDuration)
			{
				PlayWade();
				stepDuration = CalculateStepDuration(moveAnim);
				stepStartTime += stepDuration;
			}
		}
	}

	private bool IsIdling()
	{
		return base.animation["movement_idle"].enabled;
	}

	private AnimationState GetMoveAnim()
	{
		string[] array = kMoveAnimations;
		foreach (string name in array)
		{
			AnimationState animationState = base.animation[name];
			if (animationState != null && animationState.enabled)
			{
				return animationState;
			}
		}
		return null;
	}

	private void PlaySplash()
	{
		if (effectsList != null)
		{
			effectsList.TryOneShot("Splash sequence", base.gameObject);
		}
	}

	private void PlayWade()
	{
		if (effectsList != null)
		{
			effectsList.TryOneShot("Wade sequence", base.gameObject);
		}
	}

	private void PlayVO()
	{
		BehaviorManager component = base.gameObject.GetComponent<BehaviorManager>();
		if (UseVO && (component == null || !(component.getBehavior() is BehaviorSpline)) && WaterVOCooldown.OkToPlay(base.gameObject))
		{
			VOManager.Instance.PlayVO("walk_in_water", base.gameObject);
			WaterVOCooldown.StartCooldown(base.gameObject);
		}
	}

	private float CalculateStepDuration(AnimationState movementAnimationState)
	{
		float num = movementAnimationState.length / 2f;
		return num / ((movementAnimationState.speed != 0f) ? movementAnimationState.speed : 1f);
	}
}
