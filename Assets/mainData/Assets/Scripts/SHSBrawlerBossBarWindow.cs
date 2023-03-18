using System.Collections.Generic;
using UnityEngine;

public class SHSBrawlerBossBarWindow : GUIControlWindow
{
	public enum BossBarStyle
	{
		Stacked,
		Horizontal,
		HorizontalScaled
	}

	public BossBarStyle bossBarStyle;

	private bool _bossActive;

	private List<SHSBrawlerBossBar> _usedBars;

	private List<SHSBrawlerBossBar> _unusedBars;

	private float _healthBarScale;

	public bool BossActive
	{
		get
		{
			return _bossActive;
		}
		set
		{
			_bossActive = value;
		}
	}

	public SHSBrawlerBossBarWindow()
	{
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		isVisible = false;
		_bossActive = false;
		_healthBarScale = 1f;
		Traits.ResourceLoadingPhaseTrait = ControlTraits.ResourceLoadingPhaseTraitEnum.Active;
		bossBarStyle = BossBarStyle.Horizontal;
		_usedBars = new List<SHSBrawlerBossBar>();
		_unusedBars = new List<SHSBrawlerBossBar>();
	}

	public override bool InitializeResources(bool reload)
	{
		for (int i = 0; i < 2; i++)
		{
			SHSBrawlerBossBar sHSBrawlerBossBar = CreateBossBar();
			if (sHSBrawlerBossBar != null)
			{
				_unusedBars.Add(sHSBrawlerBossBar);
			}
		}
		return base.InitializeResources(reload);
	}

	public void RestartControl()
	{
		_bossActive = (_usedBars.Count > 0);
	}

	public override void OnActive()
	{
		base.OnActive();
		if (AppShell.Instance != null)
		{
			AppShell.Instance.EventMgr.AddListener<BossAIControllerBrawler.BossBattleBeginEvent>(OnBossBattleBegin);
			AppShell.Instance.EventMgr.AddListener<BossAIControllerBrawler.BossBattleEndEvent>(OnBossBattleEnd);
			AppShell.Instance.EventMgr.AddListener<BossAIControllerBrawler.BossDieEvent>(OnBossDie);
		}
	}

	public override void OnInactive()
	{
		base.OnInactive();
	}

	public override void OnShow()
	{
		base.OnShow();
		UpdateBossBars();
	}

	public override void OnHide()
	{
		base.OnHide();
		foreach (SHSBrawlerBossBar usedBar in _usedBars)
		{
			usedBar.IsVisible = false;
		}
	}

	public void PushBossBar(GameObject boss)
	{
		SHSBrawlerBossBar sHSBrawlerBossBar = null;
		if (_unusedBars.Count > 0)
		{
			sHSBrawlerBossBar = _unusedBars[0];
			_unusedBars.RemoveAt(0);
		}
		else
		{
			sHSBrawlerBossBar = CreateBossBar();
		}
		if (sHSBrawlerBossBar != null)
		{
			Add(sHSBrawlerBossBar);
			sHSBrawlerBossBar.SetBoss(boss);
			_usedBars.Add(sHSBrawlerBossBar);
		}
		UpdateBossBars();
	}

	public void PopBossBar()
	{
		SHSBrawlerBossBar bar = null;
		if (_usedBars.Count > 0)
		{
			bar = _usedBars[0];
		}
		PopBossBar(bar);
	}

	public void PopBossBar(SHSBrawlerBossBar bar)
	{
		_usedBars.Remove(bar);
		if (bar != null)
		{
			Remove(bar);
			_unusedBars.Add(bar);
		}
		UpdateBossBars();
	}

	public void PopAllBossBars()
	{
		_unusedBars.AddRange(_usedBars);
		_usedBars.Clear();
		foreach (SHSBrawlerBossBar unusedBar in _unusedBars)
		{
			Remove(unusedBar);
		}
		UpdateBossBars();
	}

	public void UpdateBossBars()
	{
		Vector2 offset = new Vector2(-30f, 0f);
		foreach (SHSBrawlerBossBar usedBar in _usedBars)
		{
			usedBar.IsVisible = true;
			usedBar.ScaleHealthBar(_healthBarScale);
			switch (bossBarStyle)
			{
			case BossBarStyle.Stacked:
				usedBar.SetPosition(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, OffsetType.Absolute, offset);
				offset.y -= usedBar.OffsetY;
				break;
			case BossBarStyle.Horizontal:
				usedBar.SetPosition(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, OffsetType.Absolute, offset);
				offset.x -= (size.x - 30f) / (float)(_usedBars.Count + 1);
				break;
			}
		}
		for (int num = _usedBars.Count - 1; num > 0; num--)
		{
			_usedBars[num].BringToFront();
		}
	}

	private SHSBrawlerBossBar CreateBossBar()
	{
		SHSBrawlerBossBar sHSBrawlerBossBar = new SHSBrawlerBossBar();
		sHSBrawlerBossBar.SetPosition(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, OffsetType.Absolute, new Vector2(-30f, 0f));
		sHSBrawlerBossBar.SetSize(SHSBrawlerBossBar.bossBarSize);
		sHSBrawlerBossBar.HitTestType = HitTestTypeEnum.Transparent;
		return sHSBrawlerBossBar;
	}

	private void OnBossBattleBegin(BossAIControllerBrawler.BossBattleBeginEvent e)
	{
		PopAllBossBars();
		_healthBarScale = 1f / (float)e.Bosses.Count;
		foreach (GameObject boss in e.Bosses)
		{
			PushBossBar(boss);
		}
		_bossActive = true;
		Show();
	}

	private void OnBossBattleEnd(BossAIControllerBrawler.BossBattleEndEvent e)
	{
		PopAllBossBars();
		_bossActive = false;
		Hide();
	}

	private void OnBossDie(BossAIControllerBrawler.BossDieEvent e)
	{
		_healthBarScale = 1f / (float)_usedBars.Count;
		SHSBrawlerBossBar bar = null;
		foreach (SHSBrawlerBossBar usedBar in _usedBars)
		{
			if (usedBar.BossId == e.Boss.GetInstanceID())
			{
				bar = usedBar;
				break;
			}
		}
		PopBossBar(bar);
	}
}
