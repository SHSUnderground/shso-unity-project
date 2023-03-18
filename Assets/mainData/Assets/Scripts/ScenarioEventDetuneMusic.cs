using System.Collections;
using UnityEngine;

[AddComponentMenu("ScenarioEvent/Detune Music")]
public class ScenarioEventDetuneMusic : ScenarioEventHandlerEnableBase
{
	protected enum DetuneState
	{
		Idle,
		Detuning,
		Detuned,
		Returning
	}

	public float cents = 50f;

	public float transitionDurationSeconds = 1f;

	private ShsAudioSource currentMusic;

	private float originalPitch = 1f;

	private float destinationPitch = 1f;

	private DetuneState state;

	protected override void OnEnableEvent(string eventName)
	{
		if (currentMusic == null)
		{
			currentMusic = GetMusic();
		}
		if (currentMusic == null)
		{
			CspUtils.DebugLog("Could not detune audio using event <" + eventName + ">: No music currently playing");
		}
		else if (state == DetuneState.Idle || state == DetuneState.Returning)
		{
			StopAllCoroutines();
			if (state == DetuneState.Idle)
			{
				originalPitch = currentMusic.Pitch;
				destinationPitch = GetDestinationPitch();
			}
			state = DetuneState.Detuning;
			StartCoroutine(SlideTo(originalPitch, destinationPitch, DetuneState.Detuned));
		}
	}

	protected override void OnDisableEvent(string eventName)
	{
		if (currentMusic == null)
		{
			CspUtils.DebugLog("Could not restore original music pitch using event <" + eventName + ">: Detuned source lost or destroyed");
		}
		else if (state == DetuneState.Detuning || state == DetuneState.Detuned)
		{
			StopAllCoroutines();
			state = DetuneState.Returning;
			StartCoroutine(SlideTo(destinationPitch, originalPitch, DetuneState.Idle));
		}
	}

	protected void OnDestroy()
	{
		if (currentMusic != null)
		{
			currentMusic.Pitch = originalPitch;
		}
		state = DetuneState.Idle;
	}

	protected ShsAudioSource GetMusic()
	{
		ShsAudioSource[] array = Utils.FindObjectsOfType<ShsAudioSource>();
		foreach (ShsAudioSource shsAudioSource in array)
		{
			if (shsAudioSource.AudioCategory == AudioCategoryEnum.MusicScore && shsAudioSource.IsPlaying)
			{
				return shsAudioSource;
			}
		}
		return null;
	}

	protected IEnumerator SlideTo(float startPitch, float endPitch, DetuneState endState)
	{
		float startTime = Time.time;
		while (currentMusic != null && Time.time < startTime + transitionDurationSeconds)
		{
			float interpolant = (Time.time - startTime) / transitionDurationSeconds;
			currentMusic.Pitch = Mathf.SmoothStep(startPitch, endPitch, interpolant);
			yield return 0;
		}
		if (currentMusic != null)
		{
			currentMusic.Pitch = endPitch;
		}
		state = endState;
	}

	private float GetDestinationPitch()
	{
		return originalPitch * Mathf.Pow(2f, cents / 1200f);
	}
}
