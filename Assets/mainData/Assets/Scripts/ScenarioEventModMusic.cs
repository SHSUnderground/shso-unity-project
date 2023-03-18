using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("ScenarioEvent/Modulate Music")]
public class ScenarioEventModMusic : ScenarioEventHandlerEnableBase
{
	private enum ModulationState
	{
		Idle,
		Modulating,
		Returning
	}

	public float amplitude = 0.5f;

	public float cyclesPerSecond = 1f;

	private ShsAudioSource currentMusic;

	private float originalPitch = 1f;

	private ModulationState state;

	public void Reset()
	{
		if (currentMusic != null)
		{
			currentMusic.Pitch = originalPitch;
		}
		state = ModulationState.Idle;
	}

	protected override void OnEnableEvent(string eventName)
	{
		if (currentMusic == null)
		{
			currentMusic = GetMusic();
		}
		if (currentMusic == null)
		{
			CspUtils.DebugLog("Could not modulate audio using event <" + eventName + ">: No music currently playing");
		}
		else
		{
			StartCoroutine(ModulateAudio());
		}
	}

	protected override void OnDisableEvent(string eventName)
	{
		if (state != 0)
		{
			state = ModulationState.Returning;
		}
	}

	protected void OnDestroy()
	{
		Reset();
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

	protected IEnumerator ModulateAudio()
	{
		if (state == ModulationState.Modulating)
		{
			yield break;
		}
		if (state == ModulationState.Idle)
		{
			originalPitch = currentMusic.Pitch;
		}
		state = ModulationState.Modulating;
		float startTime = Time.time;
		while (state == ModulationState.Modulating && currentMusic != null)
		{
			currentMusic.Pitch = PitchValue(Time.time - startTime);
			yield return 0;
		}
		if (state == ModulationState.Returning && !(currentMusic == null))
		{
			float elapsed = Time.time - startTime;
			float secondsPerHalfCycle = 0.5f / cyclesPerSecond;
			float finishTime = (Mathf.Floor(elapsed / secondsPerHalfCycle) + 1f) * secondsPerHalfCycle + startTime;
			while (Time.time < finishTime && currentMusic != null)
			{
				currentMusic.Pitch = PitchValue(Time.time - startTime);
				yield return 0;
			}
			Reset();
		}
	}

	protected float PitchValue(float elapsed)
	{
		return amplitude * Mathf.Sin(cyclesPerSecond * elapsed * ((float)Math.PI * 2f)) + originalPitch;
	}
}
