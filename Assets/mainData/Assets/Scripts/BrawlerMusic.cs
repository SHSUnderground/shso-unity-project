using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrawlerMusic : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public MusicDefinition[] MusicList;

	protected List<MusicDefinition> levelMusic;

	protected List<MusicDefinition> bossMusic;

	protected List<MusicDefinition> missionCompleteMusic;

	protected MusicDefinition currentPlaying;

	protected MusicDefinition nextMusic;

	protected bool isTransitioning;

	public bool IsPlaying
	{
		get
		{
			return currentPlaying != null;
		}
	}

	private void Awake()
	{
		currentPlaying = null;
		base.audio.playOnAwake = false;
		levelMusic = new List<MusicDefinition>();
		bossMusic = new List<MusicDefinition>();
		missionCompleteMusic = new List<MusicDefinition>();
		MusicDefinition[] musicList = MusicList;
		foreach (MusicDefinition musicDefinition in musicList)
		{
			switch (musicDefinition.Type)
			{
			case MusicDefinition.MusicType.Boss:
				bossMusic.Add(musicDefinition);
				break;
			case MusicDefinition.MusicType.Level:
				levelMusic.Add(musicDefinition);
				break;
			case MusicDefinition.MusicType.MissionComplete:
				missionCompleteMusic.Add(musicDefinition);
				break;
			default:
				CspUtils.DebugLog("Found an unknow music type <" + musicDefinition.Type + "> for clip <" + musicDefinition.Clip.name + "> while initializing the brawler music list in <" + base.gameObject.name + ">.");
				break;
			}
		}
	}

	private void Start()
	{
		AppShell.Instance.EventMgr.AddListener<BrawlerStageBegin>(OnLevelBegin);
		AppShell.Instance.EventMgr.AddListener<BossAIControllerBrawler.BossBattleBeginEvent>(OnBossBattleBegin);
		AppShell.Instance.EventMgr.AddListener<BossAIControllerBrawler.BossBattleEndEvent>(OnBossBattleEnd);
		AppShell.Instance.EventMgr.AddListener<MusicDuckMessage>(OnMusicDuck);
		AppShell.Instance.EventMgr.AddListener<AudioMixerMessage>(OnAudioMixerChanged);
		AppShell.Instance.EventMgr.AddListener<BrawlerMissionCompleteMessage>(OnMissionComplete);
	}

	private void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<BrawlerStageBegin>(OnLevelBegin);
		AppShell.Instance.EventMgr.RemoveListener<BossAIControllerBrawler.BossBattleEndEvent>(OnBossBattleEnd);
		AppShell.Instance.EventMgr.RemoveListener<BossAIControllerBrawler.BossBattleBeginEvent>(OnBossBattleBegin);
		AppShell.Instance.EventMgr.RemoveListener<MusicDuckMessage>(OnMusicDuck);
		AppShell.Instance.EventMgr.RemoveListener<AudioMixerMessage>(OnAudioMixerChanged);
		AppShell.Instance.EventMgr.RemoveListener<BrawlerMissionCompleteMessage>(OnMissionComplete);
	}

	private void OnLevelBegin(BrawlerStageBegin e)
	{
		TransitionTo(MusicDefinition.MusicType.Level);
	}

	private void OnBossBattleBegin(BossAIControllerBrawler.BossBattleBeginEvent e)
	{
		if (!IsPlaying || currentPlaying.Type != MusicDefinition.MusicType.Boss)
		{
			TransitionTo(MusicDefinition.MusicType.Boss);
		}
	}

	private void OnBossBattleEnd(BossAIControllerBrawler.BossBattleEndEvent e)
	{
		if (IsPlaying && currentPlaying.Type != MusicDefinition.MusicType.Boss)
		{
		}
	}

	private void OnMissionComplete(BrawlerMissionCompleteMessage e)
	{
		TransitionTo(MusicDefinition.MusicType.MissionComplete);
	}

	private void OnMusicDuck(MusicDuckMessage e)
	{
		StartCoroutine(DuckMusic(e.duration));
	}

	private void OnAudioMixerChanged(AudioMixerMessage e)
	{
		if (IsPlaying)
		{
			base.audio.volume = e.MixerSettings.MusicVolume * currentPlaying.Volume;
		}
	}

	protected IEnumerator DuckMusic(float duration)
	{
		for (float remaining2 = 0.5f; remaining2 > 0f; remaining2 -= Time.deltaTime)
		{
			base.audio.volume = Mathf.SmoothStep(0f, AppShell.Instance.AudioManager.MixerSettings.MusicVolume * currentPlaying.Volume, remaining2 / 0.5f);
			yield return 0;
		}
		yield return new WaitForSeconds(duration - 0.5f - 1.5f);
		for (float remaining = 3f; remaining > 0f; remaining -= Time.deltaTime)
		{
			base.audio.volume = Mathf.SmoothStep(AppShell.Instance.AudioManager.MixerSettings.MusicVolume * currentPlaying.Volume, 0f, remaining / 3f);
			yield return 0;
		}
	}

	private void Triggered(GameObject triggeringObject)
	{
		TransitionTo(currentPlaying.Type);
	}

	private void TriggerLevelMusic(GameObject triggeringObject)
	{
		TransitionTo(MusicDefinition.MusicType.Level);
	}

	private void TriggerBossMusic(GameObject triggeringObject)
	{
		TransitionTo(MusicDefinition.MusicType.Boss);
	}

	public void TransitionTo(MusicDefinition.MusicType musicType)
	{
		MusicDefinition musicDefinition = null;
		switch (musicType)
		{
		case MusicDefinition.MusicType.Boss:
			musicDefinition = ChooseMusicFromList(bossMusic);
			break;
		case MusicDefinition.MusicType.Level:
			musicDefinition = ChooseMusicFromList(levelMusic);
			break;
		case MusicDefinition.MusicType.MissionComplete:
			musicDefinition = ChooseMusicFromList(missionCompleteMusic);
			break;
		}
		nextMusic = musicDefinition;
		if (!IsPlaying)
		{
			StartMusic();
		}
		else if (!isTransitioning)
		{
			StartCoroutine(TransitionOutAndPlayNext());
			isTransitioning = true;
		}
	}

	protected IEnumerator TransitionOutAndPlayNext()
	{
		if (currentPlaying != null)
		{
			float originalVolume = base.audio.volume;
			float remaining = currentPlaying.FadeTime;
			while (remaining > 0f)
			{
				remaining -= Time.deltaTime;
				base.audio.volume = Mathf.SmoothStep(0f, originalVolume, remaining / currentPlaying.FadeTime);
				yield return 0;
			}
			base.audio.Stop();
		}
		StartMusic();
		isTransitioning = false;
	}

	protected void StartMusic()
	{
		if (nextMusic != null)
		{
			base.audio.Stop();
			base.audio.loop = nextMusic.Loop;
			base.audio.volume = AppShell.Instance.AudioManager.MixerSettings.MusicVolume * nextMusic.Volume;
			base.audio.clip = nextMusic.Clip;
			base.audio.Play();
			currentPlaying = nextMusic;
			nextMusic = null;
		}
	}

	protected MusicDefinition ChooseMusicFromList(List<MusicDefinition> musicList)
	{
		CspUtils.DebugLog("choosing music from <" + musicList.Count + "> choices.");
		if (musicList.Count == 0)
		{
			return null;
		}
		if (musicList.Count == 1)
		{
			return musicList[0];
		}
		int index = Random.Range(0, musicList.Count);
		return musicList[index];
	}
}
