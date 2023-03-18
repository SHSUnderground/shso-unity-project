using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using VO;

public class GenericVOActionHandler : IResolvedVOActionHandler
{
	protected class VOInstance
	{
		protected GenericVOActionHandler parent;

		protected ResolvedVOAction voAction;

		protected OnVOActionFinishedDelegate onFinished;

		protected ShsAudioSource audioSrc;

		public bool HasAudio
		{
			get
			{
				return parent.PlaysAudio && voAction.HasAudio;
			}
		}

		public bool HasText
		{
			get
			{
				return parent.DisplaysSubtitles && voAction.HasText;
			}
		}

		public VOInstance(GenericVOActionHandler parent, ResolvedVOAction voAction, OnVOActionFinishedDelegate onFinished)
		{
			this.parent = parent;
			this.voAction = voAction;
			this.onFinished = onFinished;
		}

		public void Go()
		{
			if (HasAudio)
			{
				PlayVOAudio(voAction);
				if (HasText)
				{
					DisplayVOText(voAction);
				}
			}
			else if (HasText)
			{
				DisplayVOText(voAction);
			}
			else
			{
				Finished();
			}
		}

		public void Stop()
		{
			CleanUp();
		}

		protected void Finished()
		{
			CleanUp();
			if (onFinished != null)
			{
				onFinished();
			}
		}

		protected void CleanUp()
		{
			if (audioSrc != null)
			{
				Object.Destroy(audioSrc.gameObject);
			}
			parent.activeInstances.Remove(voAction);
		}

		protected void PlayVOAudio(ResolvedVOAction vo)
		{
			VOAssetInstantiator vOAssetInstantiator = new VOAssetInstantiator(vo);
			vOAssetInstantiator.Instantiate(OnAudioSourceCreated);
		}

		protected void OnAudioSourceCreated(ShsAudioSource audioSrc, VOAssetInstantiator.InstantiationResult result)
		{
			this.audioSrc = audioSrc;
			if (audioSrc == null)
			{
				Finished();
				return;
			}
			if (voAction.Emitter != null)
			{
				Utils.AttachGameObject(voAction.Emitter, audioSrc.gameObject);
			}
			else if (voAction.ShouldHaveEmitter)
			{
				if (audioSrc != null)
				{
					Object.Destroy(audioSrc.gameObject);
				}
				return;
			}
			AudioFinishedCallback.AddFinishedCallback(audioSrc.gameObject, OnAudioFinished);
			audioSrc.Play();
		}

		protected void OnAudioFinished(ShsAudioSource src)
		{
			Finished();
		}

		protected void DisplayVOText(ResolvedVOAction vo)
		{
			if (!string.IsNullOrEmpty(vo.TextID))
			{
				CspUtils.DebugLog("TODO: Display this text on-screen: " + AppShell.Instance.stringTable[vo.TextID]);
			}
		}
	}

	protected Dictionary<ResolvedVOAction, VOInstance> activeInstances = new Dictionary<ResolvedVOAction, VOInstance>();

	[CompilerGenerated]
	private bool _003CPlaysAudio_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CDisplaysSubtitles_003Ek__BackingField;

	public bool PlaysAudio
	{
		[CompilerGenerated]
		get
		{
			return _003CPlaysAudio_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CPlaysAudio_003Ek__BackingField = value;
		}
	}

	public bool DisplaysSubtitles
	{
		[CompilerGenerated]
		get
		{
			return _003CDisplaysSubtitles_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CDisplaysSubtitles_003Ek__BackingField = value;
		}
	}

	public GenericVOActionHandler()
	{
		PlaysAudio = true;
		DisplaysSubtitles = false;
	}

	public GenericVOActionHandler(bool playsAudio, bool displaysSubtitles)
	{
		PlaysAudio = playsAudio;
		DisplaysSubtitles = displaysSubtitles;
	}

	public void HandleResolvedVOAction(ResolvedVOAction vo, OnVOActionFinishedDelegate onFinished)
	{
		VOInstance vOInstance = new VOInstance(this, vo, onFinished);
		activeInstances.Add(vo, vOInstance);
		vOInstance.Go();
	}

	public void CancelVOAction(ResolvedVOAction vo)
	{
		VOInstance value;
		if (activeInstances.TryGetValue(vo, out value))
		{
			value.Stop();
		}
	}
}
