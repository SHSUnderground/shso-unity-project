using System.Collections;
using UnityEngine;

[AddComponentMenu("Interactive Object/Animation")]
public class InteractiveObjectAnimation : MonoBehaviour, IInteractiveObjectChild
{
	public int loops = -1;

	public string clipName = "Take 001";

	public bool resetAnimationsOnEnable;

	public bool disableProximityHighlight;

	protected InteractiveObject owner;

	protected bool originalProximityHighlightState;

	protected bool unloading;

	public Animation AnimationOwner;

	public void OnEnable()
	{
		if (owner != null && disableProximityHighlight)
		{
			originalProximityHighlightState = owner.highlightOnProximity;
			owner.highlightOnProximity = false;
		}
		GameObject gameObject = new GameObject("IOA_Proxy");
		gameObject.transform.parent = base.gameObject.transform.parent.parent;
		Utils.AddComponent<CoroutineContainer>(gameObject).StartCoroutine(PlayAnimation(gameObject));
	}

	public void OnUnload()
	{
		unloading = true;
	}

	public void OnDisable()
	{
		if (AnimationOwner != null && !unloading)
		{
			AnimationOwner.Rewind(clipName);
			AnimationOwner.Sample();
			AnimationOwner.Stop(clipName);
		}
	}

	protected IEnumerator PlayAnimation(GameObject proxyOwner)
	{
		if (AnimationOwner != null && AnimationOwner[clipName] != null)
		{
			if (resetAnimationsOnEnable)
			{
				AnimationOwner.Rewind();
				AnimationOwner.Sample();
			}
			AnimationOwner[clipName].wrapMode = WrapMode.Loop;
			AnimationOwner.Rewind(clipName);
			AnimationOwner.Play(clipName);
			yield return new WaitForSeconds(AnimationOwner[clipName].length);
		}
		if (owner != null && disableProximityHighlight)
		{
			owner.highlightOnProximity = originalProximityHighlightState;
			owner.GotoBestState();
		}
		Object.Destroy(proxyOwner);
	}

	public void Initialize(InteractiveObject owner, GameObject model)
	{
		this.owner = owner;
		if (AnimationOwner == null)
		{
			if (model != null)
			{
				AnimationOwner = Utils.GetComponent<Animation>(model, Utils.SearchChildren);
			}
			if (AnimationOwner == null)
			{
				AnimationOwner = Utils.GetComponent<Animation>(owner.gameObject, Utils.SearchChildren);
			}
		}
	}

	public float GetLength()
	{
		if (AnimationOwner != null)
		{
			float length = AnimationOwner[clipName].length;
			if (loops > 0)
			{
				return (float)loops * length;
			}
			return length;
		}
		return AnimationOwner.clip.length;
	}
}
