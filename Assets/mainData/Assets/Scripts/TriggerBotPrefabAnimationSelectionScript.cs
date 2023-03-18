using System.Collections;
using System.Linq;
using UnityEngine;

public class TriggerBotPrefabAnimationSelectionScript : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string[] animNames = new string[2]
	{
		"movement_idle",
		"movement_idle_2"
	};

	public Animation[] animationExclusions = new Animation[0];

	private string animName;

	private float delayRange = 2.5f;

	private float delay;

	private void Start()
	{
		delay = Random.Range(0f, delayRange);
		StartCoroutine(StartAnimationCoRoutine());
	}

	private IEnumerator StartAnimationCoRoutine()
	{
		yield return new WaitForSeconds(delay);
		Animation animComp = null;
		Animation[] attachedAnims = Utils.GetComponents<Animation>(this, Utils.SearchChildren);
		Animation[] array = attachedAnims;
		foreach (Animation attachedAnim in array)
		{
			if (!Enumerable.Contains(animationExclusions, attachedAnim))
			{
				animComp = attachedAnim;
				break;
			}
		}
		if (animComp != null)
		{
			animName = animNames[Random.Range(0, animNames.Length)];
			AnimationState anim = animComp.animation[animName];
			anim.wrapMode = WrapMode.Loop;
			animComp.Play(animName);
		}
	}
}
