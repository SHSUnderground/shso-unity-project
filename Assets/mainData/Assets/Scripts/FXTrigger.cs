using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Triggers/FX Trigger")]
public class FXTrigger : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public EffectSequence effectSequence;

	public ShsAudioSource audioEffect_DEPRECATED;

	public bool playOnPlayer;

	public bool attachToPlayer;

	public Transform spawnLocation;

	public Vector3 offset = Vector3.zero;

	public bool stopOnExit;

	public bool limitToOneInstance;

	public bool localPlayerOnly;

	protected List<GameObject> fxInstances = new List<GameObject>();

	public void OnTriggerEnter(Collider other)
	{
		fxInstances.RemoveAll(delegate(GameObject arg)
		{
			return arg == null;
		});
		if ((!limitToOneInstance || fxInstances.Count == 0) && DoesTrigger(other))
		{
			Play(other);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (stopOnExit && DoesTrigger(other))
		{
			Stop();
		}
	}

	public bool DoesTrigger(Collider other)
	{
		return (localPlayerOnly && Utils.IsLocalPlayer(other.gameObject)) || (!localPlayerOnly && Utils.IsPlayer(other.gameObject));
	}

	protected void Play(Collider player)
	{
		Transform transform = (!playOnPlayer) ? base.gameObject.transform : player.transform;
		if (effectSequence != null)
		{
			PlayEffectSequence(transform);
		}
		if (audioEffect_DEPRECATED != null)
		{
			PlayAudioEffect(transform);
		}
	}

	protected void PlayEffectSequence(Transform transform)
	{
		Transform transform2 = transform;
		if (spawnLocation != null)
		{
			transform2 = spawnLocation;
		}
		GameObject gameObject = Object.Instantiate(effectSequence.gameObject, transform2.position + offset, transform2.rotation) as GameObject;
		if (attachToPlayer)
		{
			gameObject.transform.parent = transform;
		}
		fxInstances.Add(gameObject);
		EffectSequence component = Utils.GetComponent<EffectSequence>(gameObject);
		component.Initialize(null, KillSequence, null);
		component.StartSequence();
	}

	protected void PlayAudioEffect(Transform transform)
	{
		GameObject gameObject = Object.Instantiate(audioEffect_DEPRECATED.gameObject, transform.position + offset, transform.rotation) as GameObject;
		Utils.AddComponent<SuicideOnStop>(gameObject);
		if (stopOnExit)
		{
			fxInstances.Add(gameObject);
		}
	}

	protected void Stop()
	{
		foreach (GameObject fxInstance in fxInstances)
		{
			if (fxInstance != null)
			{
				Object.Destroy(fxInstance);
			}
		}
		fxInstances.Clear();
	}

	public void KillSequence(EffectSequence seq)
	{
		fxInstances.Remove(seq.gameObject);
		Object.Destroy(seq.gameObject);
	}
}
