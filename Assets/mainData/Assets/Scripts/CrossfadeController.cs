public class CrossfadeController
{
	public virtual ShsAudioBase RequestCrossfade(ShsAudioBase currentMusic, ShsAudioBase audioSource)
	{
		if (IsLoadingMusicRequestedOverStartup(currentMusic, audioSource))
		{
			return currentMusic;
		}
		if (AreSourcesIdentical(currentMusic, audioSource))
		{
			return currentMusic;
		}
		if (currentMusic != null)
		{
			if (audioSource != null)
			{
				CrossfadeIn(audioSource);
			}
			CrossfadeOut(currentMusic);
		}
		return audioSource;
	}

	public virtual void OnUnregistered()
	{
	}

	protected bool IsLoadingMusicRequestedOverStartup(ShsAudioBase currentMusic, ShsAudioBase newMusic)
	{
		ShsAudioSource shsAudioSource = currentMusic as ShsAudioSource;
		ShsAudioSource shsAudioSource2 = newMusic as ShsAudioSource;
		ShsAudioSource loadingMusic = AppShell.Instance.loadingMusic;
		ShsAudioSource startupMusic = AppShell.Instance.startupMusic;
		return shsAudioSource != null && shsAudioSource2 != null && startupMusic != null && loadingMusic != null && shsAudioSource.PrefabName == startupMusic.PrefabName && shsAudioSource2.PrefabName == loadingMusic.PrefabName;
	}

	protected bool AreSourcesIdentical(ShsAudioBase left, ShsAudioBase right)
	{
		if (left == null || right == null)
		{
			return false;
		}
		if (left is ShsAudioSource && right is ShsAudioSource)
		{
			return (left as ShsAudioSource).PrefabName == (right as ShsAudioSource).PrefabName;
		}
		return left.GetType() == right.GetType() && left.gameObject.name == right.gameObject.name;
	}

	protected virtual Crossfader CrossfadeIn(ShsAudioBase source)
	{
		return Crossfader.CrossfadeIn(source);
	}

	protected virtual Crossfader CrossfadeOut(ShsAudioBase source)
	{
		return Crossfader.CrossfadeOut(source);
	}
}
