using ShsAudio;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class ShsAudioBase : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	[Serializable]
	public class AudioClipReference
	{
		public AudioClip Clip;

		public SABundle BundleName;

		public string AssetName;

		public int ClipRepeatCount = 1;

		[CompilerGenerated]
		private bool _003CWaitingForAssetLoad_003Ek__BackingField;

		public bool WaitingForAssetLoad
		{
			[CompilerGenerated]
			get
			{
				return _003CWaitingForAssetLoad_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CWaitingForAssetLoad_003Ek__BackingField = value;
			}
		}

		public AudioClipReference()
		{
			WaitingForAssetLoad = true;
		}

		public bool HasAsset()
		{
			return Clip != null || !string.IsNullOrEmpty(AssetName);
		}

		public bool IsBundleNameInAssetName()
		{
			return AssetName != null && AssetName.Contains("|");
		}

		public bool IsNullBundle()
		{
			return BundleName == SABundle.None && !IsBundleNameInAssetName();
		}

		public bool IsNullBundleReference()
		{
			return string.IsNullOrEmpty(AssetName) || IsNullBundle();
		}
	}

	protected const float MAX_TIME_TO_WAIT_TO_PLAY = 2f;

	protected const float MAX_TIME_TO_WAIT_TO_PLAY_MUSIC = -1f;

	protected float volume = 1f;

	protected AudioMixerValues mixerSettings;

	protected bool hasRequestedClips;

	[CompilerGenerated]
	private bool _003CIsRegistered_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CPaused_003Ek__BackingField;

	[CompilerGenerated]
	private LocalizationStrategy _003CLocalizationStrategy_003Ek__BackingField;

	public bool IsRegistered
	{
		[CompilerGenerated]
		get
		{
			return _003CIsRegistered_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CIsRegistered_003Ek__BackingField = value;
		}
	}

	public bool Paused
	{
		[CompilerGenerated]
		get
		{
			return _003CPaused_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CPaused_003Ek__BackingField = value;
		}
	}

	public virtual float Volume
	{
		get
		{
			return volume;
		}
		set
		{
			volume = value;
		}
	}

	public LocalizationStrategy LocalizationStrategy
	{
		[CompilerGenerated]
		get
		{
			return _003CLocalizationStrategy_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CLocalizationStrategy_003Ek__BackingField = value;
		}
	}

	public ShsAudioBase()
	{
		IsRegistered = false;
		LocalizationStrategy = LocalizationStrategy.UseLocale;
	}

	public virtual void Play()
	{
		StartCoroutine(DelayedPlay());
	}

	public abstract void Stop();

	protected virtual IEnumerator DelayedPlay()
	{
		float startTime = Time.time;
		while (!hasRequestedClips && Time.time - startTime < 30f)
		{
			yield return 0;
			RequestClipsFromAssetBundles();
		}
		while (!IsRegistered && Time.time - startTime < 30f)
		{
			RegisterWithAudioManager();
			yield return 0;
		}
		if (!IsRegistered)
		{
			CspUtils.DebugLog("ShsAudioBase <" + base.gameObject.name + "> failed to play after 30 seconds!");
		}
		else
		{
			PlayImmediate();
		}
	}

	public abstract bool Is3D();

	protected abstract void PlayImmediate();

	public void Triggered()
	{
		Play();
	}

	protected virtual void RegisterWithAudioManager()
	{
		if (AppShell.Instance != null && AppShell.Instance.AudioManager != null)
		{
			mixerSettings = AppShell.Instance.AudioManager.RegisterAudioSource(this);
			IsRegistered = true;
		}
	}

	protected void UnregisterWithAudioManager()
	{
		AppShell.Instance.AudioManager.UnregisterAudioSource(this);
		IsRegistered = false;
	}

	public virtual float GetMixedVolume()
	{
		return volume * GetMixerVolume();
	}

	protected abstract float GetMixerVolume();

	public void OnAudioMixerChanged(AudioMixerMessage e)
	{
		Volume = volume;
	}

	protected virtual void RequestClipsFromAssetBundles()
	{
		hasRequestedClips = true;
	}

	protected void RequestClipsFromAssetBundles(IEnumerable clips)
	{
		if (Definitions.TaggedAudioReferences != null && !hasRequestedClips)
		{
			bool flag = false;
			foreach (AudioClipReference clip in clips)
			{
				flag = false;
				if (clip.Clip == null)
				{
					if (!string.IsNullOrEmpty(clip.AssetName))
					{
						bool flag2 = clip.AssetName.Contains("|");
						if (clip.BundleName != 0 || flag2)
						{
							clip.WaitingForAssetLoad = true;
							SABundledAsset bundledAsset;
							if (flag2)
							{
								int num = clip.AssetName.IndexOf("|");
								string bundle = clip.AssetName.Substring(0, num);
								string prefabName = clip.AssetName.Substring(num + 1);
								bundledAsset = Helpers.ResolveBundledAsset(bundle, prefabName, LocalizationStrategy);
							}
							else
							{
								bundledAsset = Helpers.ResolveBundledAsset(clip.BundleName, clip.AssetName, LocalizationStrategy);
							}
							if (bundledAsset == null)
							{
								CspUtils.DebugLog("Could not resolve audio asset with bundle <" + clip.BundleName + "> and asset <" + clip.AssetName + ">");
								clip.WaitingForAssetLoad = false;
								continue;
							}
							CachedAssetBundle value;
							UnityEngine.Object value2;
							if (AppShell.Instance.BundleLoader.CachedBundles.TryGetValue(bundledAsset.bundle, out value) && value.PreloadedAssets.TryGetValue(bundledAsset.asset, out value2))
							{
								OnClipLoadedFromAssetBundle(value2, value.Bundle, clip);
								value.MarkAssetsAsUsed();
							}
							if (clip.WaitingForAssetLoad)
							{
								AppShell.Instance.BundleLoader.LoadAsset(bundledAsset.bundle, bundledAsset.asset, clip, OnClipLoadedFromAssetBundle);
							}
						}
						else
						{
							flag = true;
							CspUtils.DebugLog("FLAGCSP1");
						}
					}
					else
					{
						flag = true;
						CspUtils.DebugLog("FLAGCSP2");
					}
				}
				else if ((clip.BundleName != 0) || ((clip.AssetName != null) && (clip.AssetName != string.Empty)))
				{
					// CSP temporarily commented out next 2 lines.
					//flag = true;
					//CspUtils.DebugLog("FLAGCSP3: clip.BundleName=" + clip.BundleName + " " + (clip.BundleName != 0) + " " + (clip.AssetName != null) + " " +  (clip.AssetName != string.Empty) );
				}
				if (flag)
				{
					CspUtils.DebugLog("Inconsistent data in audio clip reference: source=<" + base.gameObject.name + ">, Clip=<" + ((!(clip.Clip != null)) ? null : clip.Clip.name) + ">, Bundle=<" + clip.BundleName + ">, Asset=<" + clip.AssetName + ">.");
				}
			}
			hasRequestedClips = true;
		}
	}

	protected void OnClipLoadedFromAssetBundle(UnityEngine.Object obj, AssetBundle bundle, object extraData)
	{
		AudioClipReference audioClipReference = extraData as AudioClipReference;
		if (audioClipReference == null)
		{
			CspUtils.DebugLog("Invalid clip reference <" + extraData + "> recieved on source=<" + base.gameObject.name + ">.  This audio clip will likely not be able to play. (bundle=<" + ((!(bundle != null)) ? null : bundle.name) + "> and object=<" + obj + ">).");
		}
		AudioClip audioClip = obj as AudioClip;
		if (audioClip == null)
		{
			CspUtils.DebugLog("Failed to load AudioClip asset <" + audioClipReference.AssetName + "> from asset bundle <" + audioClipReference.BundleName + "> for source=<" + base.gameObject.name + ">.");
		}
		audioClipReference.Clip = audioClip;
		audioClipReference.WaitingForAssetLoad = false;
	}
}
