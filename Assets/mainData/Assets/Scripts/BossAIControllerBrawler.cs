using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAIControllerBrawler : AIControllerBrawler
{
	public class BossCreatedEvent : ShsEventMessage
	{
		public List<GameObject> Bosses;

		public BossCreatedEvent(List<GameObject> bosses)
		{
			Bosses = bosses;
		}
	}

	public class BossDieEvent : ShsEventMessage
	{
		public GameObject Boss;

		public BossDieEvent(GameObject boss)
		{
			Boss = boss;
		}
	}

	public class BossBattleBeginEvent : ShsEventMessage
	{
		public List<GameObject> Bosses;

		public BossBattleBeginEvent(List<GameObject> bosses)
		{
			Bosses = bosses;
		}
	}

	public class BossBattleEndEvent : ShsEventMessage
	{
		public GameObject BossObject;

		public BossBattleEndEvent(GameObject bossObject)
		{
			BossObject = bossObject;
		}
	}

	public float deathCamDistance = 7f;

	public Vector3 deathCamOffset;

	private static List<GameObject> _bossList;

	private bool isListeningForSplash;

	private bool playedStartVO;

	private bool triedPlayingStartVO;

	private bool bossNameVOFinished;

	public static int BossCount
	{
		get
		{
			return (_bossList != null) ? _bossList.Count : 0;
		}
	}

	public override bool CanPlayAttackVO
	{
		get
		{
			return playedStartVO && base.CanPlayAttackVO;
		}
	}

	public override void InitializeFromData(DataWarehouse bossAiMeleeData)
	{
		deathCamDistance = bossAiMeleeData.TryGetFloat("death_camera_distance", 7f);
		deathCamOffset = bossAiMeleeData.TryGetVector("death_camera_offset", Vector3.zero);
		if (bossAiMeleeData.TryGetBool("skips_boss_name_vo", false))
		{
			DontWaitForBossName();
		}
		base.InitializeFromData(bossAiMeleeData.TryGetData("ai_melee_controller", new EmptyDataWarehouse()));
	}

	public override void InitializeFromCopy(AIControllerBrawler source)
	{
		BossAIControllerBrawler bossAIControllerBrawler = source as BossAIControllerBrawler;
		if (bossAIControllerBrawler != null)
		{
			deathCamDistance = bossAIControllerBrawler.deathCamDistance;
			deathCamOffset = bossAIControllerBrawler.deathCamOffset;
		}
		base.InitializeFromCopy(source);
	}

	public override void InitializeFromSpawner(CharacterSpawn source)
	{
		base.InitializeFromSpawner(source);
		if (source.canPlayVOWithoutCutscene)
		{
			playedStartVO = true;
		}
	}

	private void OnEnable()
	{
		AppShell.Instance.EventMgr.AddListener<BossDieEvent>(OnBossDie);
	}

	public override void OnDisable()
	{
		base.OnDisable();
		if (_bossList != null)
		{
			_bossList.Remove(base.gameObject);
		}
		AppShell.Instance.EventMgr.RemoveListener<BossDieEvent>(OnBossDie);
	}

	public override void Start()
	{
		base.Start();
		if (_bossList == null)
		{
			_bossList = new List<GameObject>();
		}
		if (!_bossList.Contains(base.gameObject))
		{
			_bossList.Add(base.gameObject);
			_bossList.Sort(BossListSorter);
		}
		AppShell.Instance.EventMgr.Fire(null, new BossCreatedEvent(_bossList));
	}

	public override void OnCutSceneStart()
	{
		base.OnCutSceneStart();
		SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
		if (componentsInChildren != null)
		{
			SkinnedMeshRenderer[] array = componentsInChildren;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
			{
				skinnedMeshRenderer.updateWhenOffscreen = true;
			}
		}
		if (!isListeningForSplash)
		{
			AppShell.Instance.EventMgr.AddListener<CutSceneImageFadeEventMessage>(OnCutSceneImageFadeEventMessage);
			isListeningForSplash = true;
		}
	}

	public override void OnCutSceneEnd()
	{
		base.OnCutSceneEnd();
		SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
		if (componentsInChildren != null)
		{
			SkinnedMeshRenderer[] array = componentsInChildren;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
			{
				skinnedMeshRenderer.updateWhenOffscreen = false;
			}
		}
		if (isListeningForSplash)
		{
			AppShell.Instance.EventMgr.RemoveListener<CutSceneImageFadeEventMessage>(OnCutSceneImageFadeEventMessage);
			isListeningForSplash = false;
		}
		PlayStartVO();
	}

	public void DontWaitForBossName()
	{
		bossNameVOFinished = true;
	}

	private int BossListSorter(GameObject x, GameObject y)
	{
		return x.name.CompareTo(y.name);
	}

	private void OnBossDie(BossDieEvent msg)
	{
		if (msg.Boss == base.gameObject && _bossList != null)
		{
			_bossList.Remove(msg.Boss);
		}
	}

	protected void OnCutSceneImageFadeEventMessage(CutSceneImageFadeEventMessage e)
	{
		if (e.ImageFadeEvent.fadeIn && base.name.Contains("_boss"))
		{
			string value = base.name.Remove(base.name.LastIndexOf("_boss"));
			if (e.ImageFadeEvent.textureSource.Contains(value))
			{
				StartCoroutine(WaitForFadeIn(e.ImageFadeEvent.fadeEnd));
			}
			else if (e.ImageFadeEvent.textureSource.Contains("doc_ock_01"))
			{
				StartCoroutine(WaitForFadeIn(e.ImageFadeEvent.fadeEnd));
			}
		}
	}

	protected IEnumerator WaitForFadeIn(float duration)
	{
		yield return new WaitForSeconds(Mathf.Max(duration - 0.5f, 0f));
		OnBossSplashScreenShown();
	}

	protected void OnBossSplashScreenShown()
	{
		ResolvedVOAction vO = VOManager.Instance.GetVO("boss_name", base.gameObject);
		if (vO != null && vO.IsResolved)
		{
			vO.OnFinished = (ResolvedVOAction.OnVOFinished)Delegate.Combine(vO.OnFinished, new ResolvedVOAction.OnVOFinished(OnBossNameVOFinished));
			VOManager.Instance.PlayResolvedVO(vO);
		}
	}

	protected void OnBossNameVOFinished(IVOMixerItem vo)
	{
		bossNameVOFinished = true;
		if (triedPlayingStartVO)
		{
			PlayStartVO();
		}
	}

	protected void PlayStartVO()
	{
		if (!bossNameVOFinished)
		{
			triedPlayingStartVO = true;
		}
		else
		{
			if (playedStartVO)
			{
				return;
			}
			playedStartVO = true;
			List<ResolvedVOAction> list = new List<ResolvedVOAction>();
			PlayerCombatController[] array = Utils.FindObjectsOfType<PlayerCombatController>();
			PlayerCombatController[] array2 = array;
			foreach (PlayerCombatController playerCombatController in array2)
			{
				ResolvedVOAction vO = VOManager.Instance.GetVO("relationships", base.gameObject, new VOInputString(playerCombatController.name));
				if (vO != null && vO.IsResolved)
				{
					list.Add(vO);
				}
			}
			if (list.Count > 0)
			{
				VOManager.Instance.PlayResolvedVO(list[UnityEngine.Random.Range(0, list.Count)]);
			}
		}
	}

	private static int GetBossCapacity()
	{
		CharacterSpawn[] array = Utils.FindObjectsOfType<CharacterSpawn>();
		if (array == null)
		{
			return 0;
		}
		int num = 0;
		CharacterSpawn[] array2 = array;
		foreach (CharacterSpawn characterSpawn in array2)
		{
			if (characterSpawn.IsBoss)
			{
				num++;
			}
		}
		return num;
	}
}
