using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Audio/Mission Music")]
public class ShsMissionMusic : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	[Serializable]
	public class Track
	{
		public ShsAudioBase music;

		public string eventName = string.Empty;

		private ShsMissionMusic owner;

		private ShsAudioBase musicInst;

		public void Initialize(ShsMissionMusic owner)
		{
			this.owner = owner;
			InstantiateMusic();
			RegisterEvent();
		}

		public void RegisterEvent()
		{
			if (!string.IsNullOrEmpty(eventName))
			{
				ScenarioEventManager.Instance.SubscribeScenarioEvent(eventName, OnEvent);
			}
		}

		public void UnregisterEvent()
		{
			if (!string.IsNullOrEmpty(eventName))
			{
				ScenarioEventManager.Instance.UnsubscribeScenarioEvent(eventName, OnEvent);
			}
		}

		private void OnEvent(string eventName)
		{
			InstantiateMusic();
			if (!owner.IsPlaying || owner.CurrentPlaying != musicInst)
			{
				owner.TransitionTo(musicInst, true);
			}
		}

		private void InstantiateMusic()
		{
			if (music != null && musicInst == null)
			{
				musicInst = Utils.GetComponent<ShsAudioBase>(UnityEngine.Object.Instantiate(music.gameObject) as GameObject);
				musicInst.gameObject.transform.parent = owner.gameObject.transform;
			}
		}
	}

	public ShsAudioBase levelMusic;

	public ShsAudioBase bossMusic;

	public ShsAudioBase missionCompleteMusic;

	public EffectSequence missionCompleteSequence;

	public float fadeTime = 1f;

	public string bossMusicStartEvent = string.Empty;

	public string levelMusicStartEvent = string.Empty;

	public bool onlyStartLevelMusicOnEvent;

	public Track[] additionalTracks;

	protected ShsAudioBase levelMusicInst;

	protected ShsAudioBase bossMusicInst;

	protected ShsAudioBase missionCompleteMusicInst;

	protected ShsAudioBase currentPlaying;

	protected ShsAudioBase nextMusic;

	protected PersistentCrossfadeController crossfadeController;

	public bool IsPlaying
	{
		get
		{
			return currentPlaying != null;
		}
	}

	public ShsAudioBase CurrentPlaying
	{
		get
		{
			return currentPlaying;
		}
	}

	private void Start()
	{
		RegisterForLevelEvent();
		RegisterForBossEvent();
		AppShell.Instance.EventMgr.AddListener<MusicDuckMessage>(OnMusicDuck);
		AppShell.Instance.EventMgr.AddListener<BrawlerMissionCompleteMessage>(OnMissionComplete);
		InstantiateLevelMusic();
		InstantiateBossMusic();
		InstantiateMissionCompleteMusic();
		Track[] array = additionalTracks;
		foreach (Track track in array)
		{
			track.Initialize(this);
		}
	}

	private void OnDisable()
	{
		Track[] array = additionalTracks;
		foreach (Track track in array)
		{
			track.UnregisterEvent();
		}
		UnregisterForLevelEvent();
		UnregisterForBossEvent();
		AppShell.Instance.EventMgr.RemoveListener<MusicDuckMessage>(OnMusicDuck);
		AppShell.Instance.EventMgr.RemoveListener<BrawlerMissionCompleteMessage>(OnMissionComplete);
		if (crossfadeController != null)
		{
			AppShell.Instance.AudioManager.UnregisterCrossfadeController(crossfadeController);
		}
	}

	private void InstantiateLevelMusic()
	{
		if (levelMusic != null && levelMusicInst == null)
		{
			levelMusicInst = Utils.GetComponent<ShsAudioBase>(UnityEngine.Object.Instantiate(levelMusic.gameObject) as GameObject);
			levelMusicInst.gameObject.transform.parent = base.gameObject.transform;
		}
	}

	private void InstantiateBossMusic()
	{
		if (bossMusic != null && bossMusicInst == null)
		{
			bossMusicInst = Utils.GetComponent<ShsAudioBase>(UnityEngine.Object.Instantiate(bossMusic.gameObject) as GameObject);
			bossMusicInst.gameObject.transform.parent = base.gameObject.transform;
		}
	}

	private void InstantiateMissionCompleteMusic()
	{
		if (missionCompleteMusic != null && missionCompleteMusicInst == null)
		{
			// CSP - temporarily comment out this block because of crashing on 2nd time playing mayhem mission.
			//missionCompleteMusicInst = Utils.GetComponent<ShsAudioBase>(UnityEngine.Object.Instantiate(missionCompleteMusic.gameObject) as GameObject);
			//missionCompleteMusicInst.gameObject.transform.parent = base.gameObject.transform;
		}
	}

	private void RegisterForLevelEvent()
	{
		AppShell.Instance.EventMgr.AddListener<BrawlerStageBegin>(OnLevelBegin);
		if (!string.IsNullOrEmpty(levelMusicStartEvent))
		{
			ScenarioEventManager.Instance.SubscribeScenarioEvent(levelMusicStartEvent, OnLevelBeginEvent);
		}
	}

	private void UnregisterForLevelEvent()
	{
		if (string.IsNullOrEmpty(levelMusicStartEvent) || !onlyStartLevelMusicOnEvent)
		{
			AppShell.Instance.EventMgr.RemoveListener<BrawlerStageBegin>(OnLevelBegin);
		}
		if (!string.IsNullOrEmpty(levelMusicStartEvent))
		{
			ScenarioEventManager.Instance.UnsubscribeScenarioEvent(levelMusicStartEvent, OnLevelBeginEvent);
		}
	}

	private void RegisterForBossEvent()
	{
		if (string.IsNullOrEmpty(bossMusicStartEvent))
		{
			AppShell.Instance.EventMgr.AddListener<BossAIControllerBrawler.BossBattleBeginEvent>(OnBossBattleBegin);
		}
		else
		{
			ScenarioEventManager.Instance.SubscribeScenarioEvent(bossMusicStartEvent, OnBossBattleBeginEvent);
		}
	}

	private void UnregisterForBossEvent()
	{
		if (string.IsNullOrEmpty(bossMusicStartEvent))
		{
			AppShell.Instance.EventMgr.RemoveListener<BossAIControllerBrawler.BossBattleBeginEvent>(OnBossBattleBegin);
		}
		else if (ScenarioEventManager.Instance != null)
		{
			ScenarioEventManager.Instance.UnsubscribeScenarioEvent(bossMusicStartEvent, OnBossBattleBeginEvent);
		}
	}

	private void OnLevelBegin(BrawlerStageBegin e)
	{
		if (string.IsNullOrEmpty(levelMusicStartEvent) || !onlyStartLevelMusicOnEvent)
		{
			PlayLevelMusic();
		}
		else if (onlyStartLevelMusicOnEvent)
		{
			AppShell.Instance.AudioManager.RequestCrossfade(null);
		}
	}

	private void OnLevelBeginEvent(string eventName)
	{
		PlayLevelMusic();
	}

	public void PlayLevelMusic()
	{
		InstantiateLevelMusic();
		if (levelMusicInst != null)
		{
			TransitionTo(levelMusicInst, true);
		}
		else
		{
			AppShell.Instance.AudioManager.RequestCrossfade(null);
		}
	}

	private void OnBossBattleBegin(BossAIControllerBrawler.BossBattleBeginEvent e)
	{
		PlayBossMusic();
	}

	private void OnBossBattleBeginEvent(string eventName)
	{
		PlayBossMusic();
	}

	public void PlayBossMusic()
	{
		InstantiateBossMusic();
		if (!IsPlaying || currentPlaying != bossMusicInst)
		{
			TransitionTo(bossMusicInst, true);
		}
	}

	private void OnMissionComplete(BrawlerMissionCompleteMessage e)
	{
		if (crossfadeController != null)
		{
			AppShell.Instance.AudioManager.UnregisterCrossfadeController(crossfadeController);
			crossfadeController = null;
			if (AppShell.Instance.AudioManager.CurrentMusic != currentPlaying)
			{
				currentPlaying.Stop();
			}
		}
		TransitionTo(missionCompleteMusicInst, false);
	}

	private void OnMusicDuck(MusicDuckMessage e)
	{
		StartCoroutine(DuckMusic(e.duration));
	}

	protected IEnumerator DuckMusic(float duration)
	{
		float originalVolume = currentPlaying.Volume;
		for (float remaining2 = 0.5f; remaining2 > 0f; remaining2 -= Time.deltaTime)
		{
			currentPlaying.Volume = Mathf.SmoothStep(originalVolume, 0f, (0.5f - remaining2) / 0.5f);
			yield return 0;
		}
		yield return new WaitForSeconds(duration - 0.5f - 1.5f);
		for (float remaining = 3f; remaining > 0f; remaining -= Time.deltaTime)
		{
			currentPlaying.Volume = Mathf.SmoothStep(0f, originalVolume, (3f - remaining) / 3f);
			yield return 0;
		}
		currentPlaying.Volume = originalVolume;
	}

	public void TransitionTo(ShsAudioBase music, bool registerAsPersistent)
	{
		if (registerAsPersistent)
		{
			if (crossfadeController == null)
			{
				crossfadeController = new PersistentCrossfadeController(music);
				AppShell.Instance.AudioManager.RegisterCrossfadeController(crossfadeController);
			}
			else
			{
				crossfadeController.PersistentSource = music;
			}
		}
		nextMusic = music;
		StartMusic();
	}

	protected void StartMusic()
	{
		if (!(nextMusic != null))
		{
			return;
		}
		if (nextMusic == missionCompleteMusicInst && missionCompleteSequence != null)
		{
			GameObject effect = UnityEngine.Object.Instantiate(missionCompleteSequence.gameObject) as GameObject;
			EffectSequence component = Utils.GetComponent<EffectSequence>(effect);
			if (component != null)
			{
				component.Initialize(base.gameObject, delegate
				{
					UnityEngine.Object.Destroy(effect);
				}, null);
				component.StartSequence();
			}
		}
		nextMusic.Play();
		nextMusic.gameObject.AddComponent<SuicideOnStop>();
		currentPlaying = nextMusic;
		nextMusic = null;
	}
}
