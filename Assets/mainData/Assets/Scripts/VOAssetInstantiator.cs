using ShsAudio;
using System.Runtime.CompilerServices;
using UnityEngine;

public class VOAssetInstantiator
{
	public enum InstantiationResult
	{
		Unknown,
		Success,
		Invalid,
		Missing,
		NotDownloaded
	}

	protected class InstantiationContext
	{
		private ResolvedVOAction voAction;

		private OnVOAssetInstantiated onInstantiated;

		public InstantiationContext(ResolvedVOAction voAction, OnVOAssetInstantiated onInstantiated)
		{
			this.voAction = voAction;
			this.onInstantiated = onInstantiated;
		}

		public void Instantiate()
		{
			if (onInstantiated != null)
			{
				if (voAction == null)
				{
					onInstantiated(null, InstantiationResult.Invalid);
				}
				else if (!Helpers.VOBundlesDownloaded && !voAction.AssumeDownloaded)
				{
					onInstantiated(null, InstantiationResult.NotDownloaded);
				}
				else if (voAction.AudioAsset.EndsWith("_audio"))
				{
					LoadAudioPrefab();
				}
				else
				{
					LoadRawClip();
				}
			}
		}

		private void LoadRawClip()
		{
			ShsAudioSource shsAudioSource = InstantiateAudioTemplate();
			ShsAudioBase.AudioClipReference audioClipReference = new ShsAudioBase.AudioClipReference();
			audioClipReference.AssetName = voAction.AudioBundle + "|" + voAction.AudioAsset;
			shsAudioSource.Clips = new ShsAudioBase.AudioClipReference[1]
			{
				audioClipReference
			};
			Finalize(shsAudioSource);
		}

		private ShsAudioSource InstantiateAudioTemplate()
		{
			string str = "vo_audio_template";
			if (!string.IsNullOrEmpty(voAction.VOAction.TemplatePrefab))
			{
				str = voAction.VOAction.TemplatePrefab;
			}
			else if (!voAction.IsMutedByUI || voAction.AudioAsset.StartsWith("VO_UI"))
			{
				str = ((!voAction.AudioAsset.Contains("BossName")) ? "vo_ui_audio_template" : "vo_ui_boss_audio_template");
			}
			else if (voAction.AudioAsset.StartsWith("VO_Boss"))
			{
				str = "vo_boss_audio_template";
			}
			GameObject gameObject = Object.Instantiate(Resources.Load("Audio/VO/" + str)) as GameObject;
			ShsAudioSource component = gameObject.GetComponent<ShsAudioSource>();
			component.LocalizationStrategy = voAction.LocalizationStrategy;
			string text2 = component.PrefabName = (gameObject.name = voAction.AudioAsset + "_audio");
			return component;
		}

		private void LoadAudioPrefab()
		{
			SABundledAsset bundledAsset = Helpers.ResolveBundledAsset(voAction.AudioBundle, voAction.AudioAsset, voAction.LocalizationStrategy);
			AppShell.Instance.BundleLoader.LoadAsset(bundledAsset.bundle, bundledAsset.asset, null, OnAudioPrefabLoaded);
		}

		private void OnAudioPrefabLoaded(Object obj, AssetBundle bundle, object extraData)
		{
			GameObject gameObject = obj as GameObject;
			if (gameObject == null)
			{
				CspUtils.DebugLog("Failed to load audio prefab asset <" + voAction.AudioAsset + "> from asset bundle <" + voAction.AudioBundle + ">.");
			}
			else
			{
				GameObject gameObject2 = Object.Instantiate(gameObject) as GameObject;
				gameObject2.name = voAction.AudioAsset;
				ShsAudioSource component = gameObject2.GetComponent<ShsAudioSource>();
				if (!(component == null))
				{
					component.PlayOnAwake = false;
					Finalize(component);
					return;
				}
				CspUtils.DebugLog("No ShsAudioSource attached to loaded audio prefab: <" + voAction.AudioAsset + "> from asset bundle <" + voAction.AudioBundle + ">.");
			}
			onInstantiated(null, InstantiationResult.Missing);
		}

		private new void Finalize(ShsAudioSource audioSrc)
		{
			if (voAction.IsLocallyOwned)
			{
				LocallyOwnedVO locallyOwnedVO = audioSrc.gameObject.AddComponent<LocallyOwnedVO>();
				locallyOwnedVO.VOAction = voAction;
				locallyOwnedVO.AudioSource = audioSrc;
			}
			onInstantiated(audioSrc, InstantiationResult.Success);
		}
	}

	public delegate void OnVOAssetInstantiated(ShsAudioSource audioSrc, InstantiationResult result);

	public const string AUDIO_TEMPLATES_PATH = "Audio/VO/";

	[CompilerGenerated]
	private ResolvedVOAction _003CVOAction_003Ek__BackingField;

	public ResolvedVOAction VOAction
	{
		[CompilerGenerated]
		get
		{
			return _003CVOAction_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CVOAction_003Ek__BackingField = value;
		}
	}

	public VOAssetInstantiator(ResolvedVOAction voAction)
	{
		VOAction = voAction;
	}

	public void Instantiate(OnVOAssetInstantiated onInstantiated)
	{
		InstantiationContext instantiationContext = new InstantiationContext(VOAction, onInstantiated);
		instantiationContext.Instantiate();
	}
}
