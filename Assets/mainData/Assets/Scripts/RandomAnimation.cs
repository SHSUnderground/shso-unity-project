using System.Collections;
using System.Linq;
using UnityEngine;

[AddComponentMenu("Miscellaneous/Random Animation")]
[RequireComponent(typeof(Animation))]
public class RandomAnimation : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum RandomAnimationLoopType
	{
		UseTimeDelay,
		LoopOnAnimationEnd
	}

	public bool loop = true;

	public RandomAnimationLoopType loopType;

	public FRange loopDelay = new FRange(5f, 10f);

	public float crossfadeDuration = 0.3f;

	public bool allowRepeats;

	public bool useIdleAnimation;

	public string idleAnimationName = string.Empty;

	public DRange idleLoopCount = new DRange(1, 3);

	private AnimationState[] anims;

	private int idleAnimationIndex;

	private int lastSelectedAnimIndex = -1;

	private int idleCountRemaining;

	private void Start()
	{
		if (base.animation == null)
		{
			return;
		}
		anims = Enumerable.ToArray(Enumerable.Cast<AnimationState>(base.animation));
		if (useIdleAnimation)
		{
			for (int i = 0; i < anims.Length; i++)
			{
				if (anims[i].name == idleAnimationName)
				{
					idleAnimationIndex = i;
				}
			}
		}
		float delay;
		if (useIdleAnimation)
		{
			idleCountRemaining = Mathf.Max(idleLoopCount.RandomValue - 1, 0);
			delay = PlayIdleAnim();
		}
		else
		{
			delay = PlayRandomAnim();
		}
		if (loop)
		{
			StartCoroutine(CoRandomAnim(delay));
		}
	}

	private IEnumerator CoRandomAnim(float delay)
	{
		while (delay > 0f)
		{
			if (loopType == RandomAnimationLoopType.UseTimeDelay)
			{
				yield return new WaitForSeconds(loopDelay.RandomValue);
			}
			else
			{
				yield return new WaitForSeconds(delay);
			}
			if (useIdleAnimation && idleCountRemaining > 0)
			{
				idleCountRemaining--;
				delay = PlayIdleAnim();
				continue;
			}
			if (useIdleAnimation)
			{
				idleCountRemaining = idleLoopCount.RandomValue;
			}
			delay = PlayRandomAnim();
		}
	}

	private float PlayAnim(int animIndex)
	{
		base.animation.Rewind(anims[animIndex].name);
		base.animation.CrossFade(anims[animIndex].name, crossfadeDuration);
		return anims[animIndex].length;
	}

	private float PlayIdleAnim()
	{
		return PlayAnim(idleAnimationIndex);
	}

	private float PlayRandomAnim()
	{
		int randomIndex = GetRandomIndex(anims.Length);
		if (anims.Length > 0)
		{
			return PlayAnim(randomIndex);
		}
		return 0f;
	}

	private int GetRandomIndex(int upperBound)
	{
		if (useIdleAnimation)
		{
			upperBound--;
		}
		int num = 0;
		if (upperBound > 1)
		{
			if (lastSelectedAnimIndex == -1)
			{
				lastSelectedAnimIndex = Random.Range(0, upperBound);
				if (useIdleAnimation && lastSelectedAnimIndex > idleAnimationIndex)
				{
					lastSelectedAnimIndex++;
				}
			}
			if (allowRepeats)
			{
				num = Random.Range(0, upperBound);
				if (useIdleAnimation && num > idleAnimationIndex)
				{
					num++;
				}
			}
			else
			{
				int num2 = lastSelectedAnimIndex;
				if (useIdleAnimation && lastSelectedAnimIndex > idleAnimationIndex)
				{
					num2 = lastSelectedAnimIndex - 1;
				}
				num = Random.Range(0, upperBound - 1);
				if (num > num2)
				{
					num++;
				}
				if (useIdleAnimation && num > idleAnimationIndex)
				{
					num++;
				}
			}
			lastSelectedAnimIndex = num;
		}
		return num;
	}
}
