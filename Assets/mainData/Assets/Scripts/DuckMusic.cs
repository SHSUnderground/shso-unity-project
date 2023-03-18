using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ShsAudioSource))]
[AddComponentMenu("Audio/Duck Music")]
public class DuckMusic : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float fadeOutDuration = 0.5f;

	public float fadeInDuration = 2f;

	public bool fadeIn = true;

	private void Start()
	{
		StartCoroutine(Duck());
	}

	private IEnumerator Duck()
	{
		ShsAudioSource sound = GetComponent<ShsAudioSource>();

///////////// this block quick fix by CSP, because above line was not finding component. ////////////////////////////////////////////////////////////////////
		Component[] components = GetComponents<Component>() as Component[];
        if(components.Length > 0) {
                //Print all the components to console
                foreach(Component comp in components) {
                    CspUtils.DebugLog ("component="+comp);
					if (comp is ShsAudioSource)
						sound = (ShsAudioSource)comp;
				}
        }  
//////////////////////////////////////////////////////////////////////////////////

		while (!sound.IsPlaying)
		{
			yield return 0;
		}
		ShsAudioBase music = GetMusic();
		if (music == null)
		{
			CspUtils.DebugLog("Could not duck music, as no music could be found");
			yield break;
		}
		float originalVolume = music.Volume;
		float remaining2 = fadeOutDuration;
		while (remaining2 > 0f && music != null)
		{
			music.Volume = Mathf.SmoothStep(originalVolume, 0f, (fadeOutDuration - remaining2) / fadeOutDuration);
			yield return 0;
			remaining2 -= Time.deltaTime;
		}
		if (fadeIn)
		{
			AdjustAutoKill();
			while (sound.IsPlaying)
			{
				yield return 0;
			}
			float remaining = fadeInDuration;
			while (remaining > 0f && music != null)
			{
				music.Volume = Mathf.SmoothStep(0f, originalVolume, (fadeInDuration - remaining) / fadeInDuration);
				yield return 0;
				remaining -= Time.deltaTime;
			}
			if (music != null)
			{
				music.Volume = originalVolume;
			}
		}
	}

	private ShsAudioBase GetMusic()
	{
		return AppShell.Instance.AudioManager.CurrentMusic;
	}

	private void AdjustAutoKill()
	{
		SuicideOnStop component = GetComponent<SuicideOnStop>();
		if (component != null)
		{
			component.killDelay = fadeInDuration + 0.1f;
		}
	}
}
