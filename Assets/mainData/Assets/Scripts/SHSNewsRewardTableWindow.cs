using System.Collections.Generic;
using UnityEngine;

public class SHSNewsRewardTableWindow : GUISimpleControlWindow
{
	private class RewardIconLoc
	{
		public Vector2 size = Vector2.zero;

		public Vector2 offset = Vector2.zero;
	}

	public static readonly Vector2 WindowSize;

	public static readonly string MainPanelTextureSource;

	public static readonly string CollectedLabelKey;

	public static readonly string CountdownLabelKey;

	public static readonly float[] RewardColPos;

	public static readonly float[] RewardRowPos;

	public static readonly int RewardRows;

	public static readonly int RewardCols;

	public static readonly int TotalRewards;

	private List<SHSNewsRewardDayWindow> _rewardDays;

	private GUILabel _countdownClock;

	private int _today;

	private Dictionary<string, int> _rewardOccurrences;

	private static Dictionary<string, RewardIconLoc[]> _rewardIconLocMap;

	public SHSNewsRewardTableWindow()
	{
		_rewardOccurrences = new Dictionary<string, int>();
	}

	static SHSNewsRewardTableWindow()
	{
		WindowSize = new Vector2(361f, 292f);
		MainPanelTextureSource = "gameworld_bundle|L_dailyreward_mainpanel";
		CollectedLabelKey = "#DAILY_REWARD_COLLECTED";
		CountdownLabelKey = "#DAILY_REWARD_COUNTDOWN";
		RewardColPos = new float[4]
		{
			0f,
			88f,
			177f,
			265f
		};
		RewardRowPos = new float[2]
		{
			48f,
			147f
		};
		RewardRows = RewardRowPos.Length;
		RewardCols = RewardColPos.Length;
		TotalRewards = RewardRows * RewardCols;
		_rewardIconLocMap = new Dictionary<string, RewardIconLoc[]>();
		Dictionary<string, RewardIconLoc[]> rewardIconLocMap = _rewardIconLocMap;
		RewardIconLoc[] array = new RewardIconLoc[3];
		RewardIconLoc rewardIconLoc = new RewardIconLoc();
		rewardIconLoc.size = new Vector2(64f, 64f);
		rewardIconLoc.offset = Vector2.zero;
		array[0] = rewardIconLoc;
		rewardIconLoc = new RewardIconLoc();
		rewardIconLoc.size = new Vector2(78f, 66f);
		rewardIconLoc.offset = new Vector2(-3f, 1f);
		array[1] = rewardIconLoc;
		rewardIconLoc = new RewardIconLoc();
		rewardIconLoc.size = new Vector2(86f, 66f);
		rewardIconLoc.offset = new Vector2(-3f, -2f);
		array[2] = rewardIconLoc;
		rewardIconLocMap.Add("s", array);
		Dictionary<string, RewardIconLoc[]> rewardIconLocMap2 = _rewardIconLocMap;
		RewardIconLoc[] array2 = new RewardIconLoc[3];
		rewardIconLoc = new RewardIconLoc();
		rewardIconLoc.size = new Vector2(62f, 62f);
		rewardIconLoc.offset = new Vector2(0f, -4f);
		array2[0] = rewardIconLoc;
		rewardIconLoc = new RewardIconLoc();
		rewardIconLoc.size = new Vector2(76f, 65f);
		rewardIconLoc.offset = new Vector2(-4f, -2f);
		array2[1] = rewardIconLoc;
		rewardIconLoc = new RewardIconLoc();
		rewardIconLoc.size = new Vector2(74f, 70f);
		rewardIconLoc.offset = new Vector2(-2f, -6f);
		array2[2] = rewardIconLoc;
		rewardIconLocMap2.Add("g", array2);
		Dictionary<string, RewardIconLoc[]> rewardIconLocMap3 = _rewardIconLocMap;
		RewardIconLoc[] array3 = new RewardIconLoc[4];
		rewardIconLoc = new RewardIconLoc();
		rewardIconLoc.size = new Vector2(76f, 64f);
		rewardIconLoc.offset = new Vector2(0f, -1f);
		array3[0] = rewardIconLoc;
		rewardIconLoc = new RewardIconLoc();
		rewardIconLoc.size = new Vector2(72f, 62f);
		rewardIconLoc.offset = new Vector2(-1f, -1f);
		array3[1] = rewardIconLoc;
		rewardIconLoc = new RewardIconLoc();
		rewardIconLoc.size = new Vector2(80f, 58f);
		rewardIconLoc.offset = new Vector2(-2f, 0f);
		array3[2] = rewardIconLoc;
		rewardIconLoc = new RewardIconLoc();
		rewardIconLoc.size = new Vector2(78f, 60f);
		rewardIconLoc.offset = new Vector2(-3f, 0f);
		array3[3] = rewardIconLoc;
		rewardIconLocMap3.Add("t", array3);
	}

	public override bool InitializeResources(bool reload)
	{
		if (!base.InitializeResources(reload))
		{
			return false;
		}
		GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(WindowSize, Vector2.zero);
		gUIImage.TextureSource = MainPanelTextureSource;
		Add(gUIImage);
		_rewardDays = new List<SHSNewsRewardDayWindow>(TotalRewards);
		for (int i = 0; i < RewardRows; i++)
		{
			for (int j = 0; j < RewardCols; j++)
			{
				SHSNewsRewardDayWindow sHSNewsRewardDayWindow = GUIControl.CreateControlAbsolute<SHSNewsRewardDayWindow>(SHSNewsRewardDayWindow.WindowSize, new Vector2(RewardColPos[j], RewardRowPos[i]));
				_rewardDays.Add(sHSNewsRewardDayWindow);
				Add(sHSNewsRewardDayWindow);
			}
		}
		GUILabel gUILabel = GUIControl.CreateControlBottomFrame<GUILabel>(new Vector2(242f, 24f), new Vector2(-54f, -14f));
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(168, 214, 247), TextAnchor.MiddleCenter);
		gUILabel.Text = CountdownLabelKey;
		Add(gUILabel);
		_countdownClock = GUIControl.CreateControlBottomFrame<GUILabel>(new Vector2(100f, 50f), new Vector2(137f, 3f));
		_countdownClock.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 24, GUILabel.GenColor(180, 226, 19), TextAnchor.MiddleCenter);
		Add(_countdownClock);
		LocalPlayerProfile localPlayerProfile = (LocalPlayerProfile)AppShell.Instance.Profile;
		if (localPlayerProfile != null)
		{
			string text = localPlayerProfile.TimeUntilMidnight.ToString();
			int num = text.LastIndexOf(':');
			if (num < 0)
			{
				num = text.Length;
			}
			text = text.Substring(0, num);
			InitializeRewardTable(localPlayerProfile.ConsecutiveDaysPlayed, localPlayerProfile.ReceivedDailyReward, text);
		}
		return true;
	}

	public override void OnActive()
	{
		base.OnActive();
		AppShell.Instance.EventMgr.AddListener<DailyRewardMessage>(OnDailyRewardMessage);
	}

	public override void OnInactive()
	{
		base.OnInactive();
		AppShell.Instance.EventMgr.RemoveListener<DailyRewardMessage>(OnDailyRewardMessage);
	}

	public void InitializeRewardTable(int consecutiveDays, bool rewardReceived, string countdown)
	{
		SetToday(consecutiveDays);
		if (SHSNewsRewardJson.Instance.HasDailyRewardData())
		{
			CreateRewards(SHSNewsRewardJson.Instance.GetDailyRewardData());
		}
		CollectReward(rewardReceived);
		SetCountdown(countdown);
	}

	public void SetCountdown(string countdown)
	{
		if (_countdownClock != null)
		{
			_countdownClock.Text = countdown;
		}
	}

	public bool IsToday(int day)
	{
		return day == _today;
	}

	public void StartRewardDayAnimations()
	{
		if (_rewardDays != null)
		{
			foreach (SHSNewsRewardDayWindow rewardDay in _rewardDays)
			{
				rewardDay.StartAnimations();
			}
		}
	}

	private void CreateRewards(List<SHSNewsReward> rewards)
	{
		if (rewards != null)
		{
			_rewardOccurrences.Clear();
			foreach (SHSNewsReward reward in rewards)
			{
				bool flag = Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldPlayAllow);
				if (flag && reward.track_id == 2)
				{
					CreateReward(reward.day_id, reward.reward_category, reward.reward_quantity);
				}
				else if (!flag && reward.track_id == 1)
				{
					CreateReward(reward.day_id, reward.reward_category, reward.reward_quantity);
				}
			}
		}
	}

	private void CreateReward(int rewardDay, string rewardCategory, int rewardCount)
	{
		if (_rewardDays != null && rewardDay <= _rewardDays.Count && rewardDay > 0)
		{
			_rewardDays[rewardDay - 1].ClearReward();
			_rewardDays[rewardDay - 1].ClearRewardState();
			if (_rewardOccurrences.ContainsKey(rewardCategory))
			{
				Dictionary<string, int> rewardOccurrences;
				Dictionary<string, int> dictionary = rewardOccurrences = _rewardOccurrences;
				string key;
				string key2 = key = rewardCategory;
				int num = rewardOccurrences[key];
				dictionary[key2] = num + 1;
			}
			else
			{
				_rewardOccurrences.Add(rewardCategory, 1);
			}
			RewardIconLoc rewardIconLocation = GetRewardIconLocation(rewardCategory, _rewardOccurrences[rewardCategory]);
			_rewardDays[rewardDay - 1].CreateReward((!IsToday(rewardDay)) ? GetRewardDayTextureSource(rewardDay) : GetRewardTodayTextureSource(), GetRewardCategoryTextureSource(rewardCategory, _rewardOccurrences[rewardCategory]), GetRewardTextLabel(rewardCategory, rewardCount), ref rewardIconLocation.size, ref rewardIconLocation.offset);
		}
	}

	private void SetToday(int today)
	{
		_today = Mathf.Clamp(today, 0, TotalRewards);
	}

	private void CollectReward(bool rewardReceived)
	{
		if (_rewardDays != null && _today > 0)
		{
			int num = _today - 1;
			for (int i = 0; i < num; i++)
			{
				_rewardDays[i].SetRewardState(SHSNewsRewardDayWindow.SHSNewsRewardDayState.Collected);
			}
			if (rewardReceived)
			{
				_rewardDays[num].SetRewardState(SHSNewsRewardDayWindow.SHSNewsRewardDayState.CollectNow);
			}
			else
			{
				_rewardDays[num].SetRewardState(SHSNewsRewardDayWindow.SHSNewsRewardDayState.CollectedToday);
			}
			for (int j = num + 1; j < TotalRewards; j++)
			{
				_rewardDays[j].SetRewardState(SHSNewsRewardDayWindow.SHSNewsRewardDayState.UnCollected);
			}
		}
	}

	private string GetRewardTextLabel(string rewardCategory, int rewardCount)
	{
		string text = string.Empty;
		switch (rewardCategory)
		{
		case "s":
			text = "#DAILY_REWARD_SILVER";
			break;
		case "g":
			text = "#DAILY_REWARDS_GOLD";
			break;
		case "t":
			text = "#DAILY_REWARD_TICKETS";
			break;
		}
		if (AppShell.Instance != null && AppShell.Instance.stringTable != null)
		{
			return string.Format(AppShell.Instance.stringTable[text], rewardCount);
		}
		return text;
	}

	private string GetRewardCategoryTextureSource(string rewardCategory, int rewardOccurrence)
	{
		string arg = string.Empty;
		switch (rewardCategory)
		{
		case "s":
			arg = "silver";
			break;
		case "g":
			arg = "gold";
			break;
		case "t":
			arg = "tickets";
			break;
		}
		return string.Format(SHSNewsRewardDayWindow.RewardIconTextureSource, arg, rewardOccurrence.ToString());
	}

	private string GetRewardDayTextureSource(int day)
	{
		return string.Format(SHSNewsRewardDayWindow.DayLabelTextureSource, "day" + day.ToString());
	}

	private string GetRewardTodayTextureSource()
	{
		return string.Format(SHSNewsRewardDayWindow.DayLabelTextureSource, "today");
	}

	private void OnDailyRewardMessage(DailyRewardMessage msg)
	{
		InitializeRewardTable(msg.consecutiveDays, msg.rewardReceived, msg.countdown);
		StartRewardDayAnimations();
	}

	private RewardIconLoc GetRewardIconLocation(string rewardCategory, int rewardOccurrence)
	{
		if (_rewardIconLocMap.ContainsKey(rewardCategory) && rewardOccurrence - 1 < _rewardIconLocMap[rewardCategory].Length)
		{
			return _rewardIconLocMap[rewardCategory][rewardOccurrence - 1];
		}
		return new RewardIconLoc();
	}
}
