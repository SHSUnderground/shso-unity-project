using System.Collections;
using UnityEngine;

public class Crossfader : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum CrossfadeDirection
	{
		FadeIn,
		FadeOut
	}

	public enum FadeState
	{
		None,
		FadingIn,
		FadingOut
	}

	public const float DEFAULT_AUDIO_CROSSFADE_TIME = 1.5f;

	public float fadeInDuration = -1f;

	public float fadeOutDuration = -1f;

	public CrossfadeDirection crossfadeDirection;

	public bool stopOnFadeOut = true;

	private ShsAudioBase audioSource;

	private FadeState fadeState;

	private bool fadeOutOnEnable;

	private void OnEnable()
	{
		if (fadeOutOnEnable)
		{
			fadeOutOnEnable = false;
			StartCoroutine(FadeOut());
		}
	}

	public static Crossfader CrossfadeIn(ShsAudioBase source)
	{
		return Crossfade(source, CrossfadeDirection.FadeIn);
	}

	public static Crossfader CrossfadeOut(ShsAudioBase source)
	{
		return Crossfade(source, CrossfadeDirection.FadeOut);
	}

	public static Crossfader CrossfadeOut(ShsAudioBase source, bool stopOnFinish)
	{
		Crossfader crossfader = GetCrossfader(source);
		crossfader.StopAllCoroutines();
		crossfader.stopOnFadeOut = stopOnFinish;
		return CrossfadeOut(source);
	}

	public static Crossfader Crossfade(ShsAudioBase source, CrossfadeDirection direction)
	{
		Crossfader crossfader = GetCrossfader(source);
		crossfader.StopAllCoroutines();
		crossfader.crossfadeDirection = direction;
		crossfader.BeginCrossfade();
		return crossfader;
	}

	public void BeginCrossfade()
	{
		audioSource = Utils.GetComponent<ShsAudioBase>(base.gameObject);
		if (audioSource == null)
		{
			return;
		}
		if (fadeState != 0)
		{
			StopAllCoroutines();
			fadeState = FadeState.None;
		}
		switch (crossfadeDirection)
		{
		case CrossfadeDirection.FadeIn:
			StartCoroutine(FadeIn());
			break;
		case CrossfadeDirection.FadeOut:
			if (base.gameObject.active)
			{
				StartCoroutine(FadeOut());
				break;
			}
			CspUtils.DebugLog(base.gameObject.name + " is disabled, and cannot be faded out -- waiting to be re-enabled before fading.");
			fadeOutOnEnable = true;
			break;
		}
	}

	public static Crossfader GetCrossfader(ShsAudioBase source)
	{
		Crossfader crossfader = Utils.GetComponent<Crossfader>(source.gameObject);
		if (crossfader == null)
		{
			crossfader = Utils.AddComponent<Crossfader>(source.gameObject);
		}
		return crossfader;
	}

	private IEnumerator FadeIn()
	{
		if (fadeState == FadeState.FadingIn)
		{
			yield break;
		}
		fadeState = FadeState.FadingIn;
		if (fadeInDuration != 0f)
		{
			if (fadeInDuration < 0f)
			{
				fadeInDuration = 1.5f;
			}
			float startTime = Time.time;
			float targetVolume = audioSource.Volume;
			audioSource.Volume = 0f;
			while (audioSource.Volume < targetVolume)
			{
				audioSource.Volume = Mathf.SmoothStep(0f, targetVolume, (Time.time - startTime) / fadeInDuration);
				yield return 0;
			}
			fadeState = FadeState.None;
		}
	}

	private IEnumerator FadeOut()
	{
		if (fadeState == FadeState.FadingOut)
		{
			yield break;
		}
		fadeState = FadeState.FadingOut;
		if (fadeOutDuration == 0f)
		{
			FinishFadeOut();
			yield break;
		}
		if (fadeOutDuration < 0f)
		{
			fadeOutDuration = 1.5f;
		}
		float startTime = Time.time;
		float originalVolume = audioSource.Volume;
		while (audioSource != null && audioSource.Volume > 0f)
		{
			audioSource.Volume = Mathf.SmoothStep(originalVolume, 0f, (Time.time - startTime) / fadeOutDuration);
			yield return 0;
		}
		FinishFadeOut();
	}

	private void FinishFadeOut()
	{
		if (audioSource != null && stopOnFadeOut)
		{
			audioSource.Stop();
		}
		fadeState = FadeState.None;
	}
}
