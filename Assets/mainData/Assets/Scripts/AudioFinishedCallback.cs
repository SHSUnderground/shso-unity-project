using System;
using UnityEngine;

public class AudioFinishedCallback : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public delegate void FinishedDelegate(ShsAudioSource audioSource);

	public event FinishedDelegate OnAudioCompleted;

	public static AudioFinishedCallback AddFinishedCallback(GameObject obj, FinishedDelegate onAudioCompleted)
	{
		if (obj != null)
		{
			AudioFinishedCallback audioFinishedCallback = obj.GetComponent<AudioFinishedCallback>();
			if (audioFinishedCallback == null)
			{
				audioFinishedCallback = obj.AddComponent<AudioFinishedCallback>();
			}
			AudioFinishedCallback audioFinishedCallback2 = audioFinishedCallback;
			audioFinishedCallback2.OnAudioCompleted = (FinishedDelegate)Delegate.Combine(audioFinishedCallback2.OnAudioCompleted, onAudioCompleted);
			return audioFinishedCallback;
		}
		return null;
	}

	public static AudioFinishedCallback RemoveFinishedCallback(GameObject obj, FinishedDelegate onAudioCompleted)
	{
		if (obj != null)
		{
			AudioFinishedCallback component = obj.GetComponent<AudioFinishedCallback>();
			if (component != null)
			{
				component.OnAudioCompleted = (FinishedDelegate)Delegate.Remove(component.OnAudioCompleted, onAudioCompleted);
				return component;
			}
		}
		return null;
	}

	private void OnAudioFinished(ShsAudioSource audioSource)
	{
		if (this.OnAudioCompleted != null)
		{
			this.OnAudioCompleted(audioSource);
		}
	}
}
