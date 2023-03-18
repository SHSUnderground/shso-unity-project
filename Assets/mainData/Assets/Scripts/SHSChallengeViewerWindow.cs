using System;
using UnityEngine;

public class SHSChallengeViewerWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private Vector2 scrollPos;

	private SHSStyle headerStyle;

	private ChallengeManager mgr;

	private ISHSCounterType counter;

	private string currentCounterVal;

	private GUILabel managerState;

	private GUILabel fanfareState;

	private GUIToggleButton ChallengeSystemSimulationMode;

	private string overrideCounterValue = "-1";

	public SHSChallengeViewerWindow(string name)
		: base(name, null)
	{
		managerState = new GUILabel();
		managerState.SetupText(GUIFontManager.SupportedFontEnum.Komica, 10, Color.yellow, TextAnchor.UpperLeft);
		managerState.SetPositionAndSize(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, new Vector2(200f, 300f));
		managerState.Offset = new Vector2(0f, 0f);
		fanfareState = new GUILabel();
		fanfareState.SetupText(GUIFontManager.SupportedFontEnum.Komica, 10, Color.yellow, TextAnchor.UpperLeft);
		fanfareState.SetPositionAndSize(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, new Vector2(200f, 300f));
		fanfareState.Offset = new Vector2(0f, 200f);
		ChallengeSystemSimulationMode = new GUIToggleButton();
		ChallengeSystemSimulationMode.Value = (PlayerPrefs.GetInt("challengesimulated", 0) == 1);
		ChallengeSystemSimulationMode.Text = "Challenge System Client Simulation";
		ChallengeSystemSimulationMode.Spacing = 35f;
		ChallengeSystemSimulationMode.SetButtonSize(new Vector2(25f, 25f));
		ChallengeSystemSimulationMode.SetSize(240f, 25f);
		ChallengeSystemSimulationMode.SetPosition(300f, 40f);
		ChallengeSystemSimulationMode.Margin = new Rect(5f, 5f, 5f, 5f);
		Add(ChallengeSystemSimulationMode);
		ChallengeSystemSimulationMode.Changed += delegate
		{
			bool value = ChallengeSystemSimulationMode.Value;
			PlayerPrefs.SetInt("challengesimulated", value ? 1 : 0);
			if (value)
			{
				ChallengeManagerServerStub.CurrentMode = ChallengeManagerServerStub.ServerMode.ClientSimulation;
			}
			else
			{
				ChallengeManagerServerStub.CurrentMode = ChallengeManagerServerStub.ServerMode.Passthrough;
			}
		};
		Add(fanfareState);
		Add(managerState);
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		if (!AppShell.Instance.ChallengeManager.Enabled)
		{
			GUILayout.Label("CHALLENGES (DISABLED)", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
			return;
		}
		GUILayout.BeginArea(new Rect(30f, 30f, 1000f, base.rect.height - 50f));
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		GUILayout.Label("CHALLENGES", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		if (AppShell.Instance.ChallengeManager.ChallengeDictionary.Count > 0)
		{
			mgr = AppShell.Instance.ChallengeManager;
			int lastViewedChallengeId = mgr.LastViewedChallengeId;
			int nextChallengeServerId = mgr.NextChallengeServerId;
			IChallenge challenge = null;
			ChallengeInfo challengeInfo = null;
			ChallengeManager.ChallengeManagerStateEnum currentState;
			mgr.GetChallengeManagerDisplayInfo(out currentState, out challenge, out challengeInfo);
			if (counter == null && AppShell.Instance.CounterManager.CountersLoaded)
			{
				counter = AppShell.Instance.CounterManager.GetCounter("ChallengeCounter");
			}
			currentCounterVal = ((counter == null) ? "-" : counter.GetCurrentValue().ToString());
			GUILayout.BeginVertical();
			if (challenge != null)
			{
				drawCurrentChallenge(challenge);
			}
			if (challengeInfo != null)
			{
				GUILayout.Space(10f);
				GUILayout.Label("PENDING", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
				GUILayout.BeginHorizontal(GUILayout.Width(700f));
				GUILayout.Label(string.Empty, headerStyle.UnityStyle, GUILayout.Width(60f));
				GUILayout.Label("ID", headerStyle.UnityStyle, GUILayout.Width(60f));
				GUILayout.Label("MESSAGE TYPE", headerStyle.UnityStyle, GUILayout.Width(100f));
				GUILayout.Label("NAME", headerStyle.UnityStyle, GUILayout.Width(170f));
				GUILayout.Label("DESC", headerStyle.UnityStyle, GUILayout.Width(300f));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(GUILayout.Width(700f));
				if (GUILayout.Button("MARK VIEWED", GUILayout.Width(150f)))
				{
					mgr.SetViewedChallenge(challengeInfo.ChallengeId, null);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				drawChallengeInfo(challengeInfo, nextChallengeServerId, lastViewedChallengeId);
			}
			GUILayout.Space(10f);
			GUILayout.Label("ALL", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
			GUILayout.BeginHorizontal(GUILayout.Width(700f));
			GUILayout.Label(string.Empty, headerStyle.UnityStyle, GUILayout.Width(60f));
			GUILayout.Label("ID", headerStyle.UnityStyle, GUILayout.Width(60f));
			GUILayout.Label("MESSAGE TYPE", headerStyle.UnityStyle, GUILayout.Width(100f));
			GUILayout.Label("NAME", headerStyle.UnityStyle, GUILayout.Width(170f));
			GUILayout.Label("REWARD", headerStyle.UnityStyle, GUILayout.Width(70f));
			GUILayout.Label("VALUE", headerStyle.UnityStyle, GUILayout.Width(70f));
			GUILayout.Label("DESC", headerStyle.UnityStyle, GUILayout.Width(300f));
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			foreach (ChallengeInfo value in mgr.ChallengeDictionary.Values)
			{
				drawChallengeInfo(value, nextChallengeServerId, lastViewedChallengeId);
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void drawCurrentChallenge(IChallenge challenge)
	{
		GUILayout.Space(10f);
		GUILayout.Label("CURRENT", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		GUILayout.BeginHorizontal(GUILayout.Width(700f));
		GUILayout.Label("ID", headerStyle.UnityStyle, GUILayout.Width(60f));
		GUILayout.Label("NAME", headerStyle.UnityStyle, GUILayout.Width(170f));
		GUILayout.Label("STATE", headerStyle.UnityStyle, GUILayout.Width(200f));
		GUILayout.Label("COUNTER", headerStyle.UnityStyle, GUILayout.Width(190f));
		GUILayout.Label("DESC", headerStyle.UnityStyle, GUILayout.Width(300f));
		GUILayout.EndHorizontal();
		ChallengeInfo challengeInfo = mgr.ChallengeDictionary[challenge.Id];
		Color color = GUI.color;
		GUI.color = Color.cyan;
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal(GUILayout.Width(1000f));
		GUILayout.Label(challengeInfo.ChallengeId.ToString(), headerStyle.UnityStyle, GUILayout.Width(60f));
		GUILayout.Label(AppShell.Instance.stringTable[challenge.Name], headerStyle.UnityStyle, GUILayout.Width(170f));
		GUILayout.Label(challenge.Status.ToString(), headerStyle.UnityStyle, GUILayout.Width(200f));
		string a = GUILayout.TextField((!(overrideCounterValue != "-1")) ? currentCounterVal : overrideCounterValue, headerStyle.UnityStyle, GUILayout.Width(50f));
		if (a != currentCounterVal)
		{
			overrideCounterValue = a;
		}
		if (GUILayout.Button("Set", GUILayout.Width(50f)))
		{
			counter.SetCounter(long.Parse(overrideCounterValue));
			currentCounterVal = overrideCounterValue;
			overrideCounterValue = "-1";
		}
		if (GUILayout.Button("Complete", GUILayout.Width(80f)))
		{
			mgr.ForceChallengeComplete();
		}
		GUILayout.Space(20f);
		GUILayout.Label(challengeInfo.Description, headerStyle.UnityStyle, GUILayout.Width(300f));
		GUILayout.EndHorizontal();
		foreach (ChallengeInfoParameters parameter in challengeInfo.Parameters)
		{
			GUILayout.BeginHorizontal(GUILayout.Width(1000f));
			GUILayout.Space(300f);
			GUILayout.Label(parameter.key, headerStyle.UnityStyle, GUILayout.Width(100f));
			GUILayout.Label(parameter.value, headerStyle.UnityStyle, GUILayout.Width(300f));
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
		GUI.color = color;
	}

	private void drawChallengeInfo(ChallengeInfo challengeInfo, int nextId, int lastId)
	{
		GUILayout.BeginHorizontal(GUILayout.Width(1000f));
		if (challengeInfo.ChallengeId == nextId)
		{
			GUI.color = Color.yellow;
			GUILayout.Label(string.Empty, GUILayout.Width(60f));
			GUILayout.Label(challengeInfo.ChallengeId.ToString(), GUILayout.Width(60f));
		}
		else
		{
			Color color = GUI.color;
			if (challengeInfo.ChallengeId > lastId && challengeInfo.ChallengeId < nextId)
			{
				GUI.color = Color.cyan;
			}
			if (ChallengeManagerServerStub.CurrentMode == ChallengeManagerServerStub.ServerMode.ClientSimulation)
			{
				if (GUILayout.Button("SET", GUILayout.Width(60f)))
				{
					AppShell.Instance.ChallengeManager.OverrideChallenge(challengeInfo);
				}
			}
			else
			{
				GUILayout.Label(string.Empty, GUILayout.Width(60f));
			}
			GUILayout.Label(challengeInfo.ChallengeId.ToString(), headerStyle.UnityStyle, GUILayout.Width(60f));
			GUILayout.Label(challengeInfo.MessageType, headerStyle.UnityStyle, GUILayout.Width(100f));
			GUILayout.Label(AppShell.Instance.stringTable[challengeInfo.Name], headerStyle.UnityStyle, GUILayout.Width(170f));
			GUILayout.Label(challengeInfo.Reward.rewardType.ToString(), headerStyle.UnityStyle, GUILayout.Width(70f));
			GUILayout.Label(challengeInfo.Reward.value, headerStyle.UnityStyle, GUILayout.Width(70f));
			GUILayout.Label(AppShell.Instance.stringTable[challengeInfo.Description], headerStyle.UnityStyle, GUILayout.Width(300f));
			GUI.color = color;
		}
		GUILayout.EndHorizontal();
	}

	public override void OnUpdate()
	{
		if (mgr == null || mgr.FanfareManager.FanfareQueue == null)
		{
			return;
		}
		int count = mgr.FanfareManager.FanfareQueue.Count;
		string text = "MANAGER State: " + mgr.ChallengeManagerStatus + Environment.NewLine;
		string text2 = text;
		text = text2 + "Next Challenge ID: " + mgr.NextChallengeServerId + Environment.NewLine;
		text2 = text;
		text = text2 + "Last Viewed ID: " + mgr.LastViewedChallengeId + Environment.NewLine;
		text2 = text;
		text = text2 + "Enabled: " + mgr.Enabled + Environment.NewLine;
		text += Environment.NewLine;
		managerState.Text = text;
		if (ChallengeManagerServerStub.CurrentMode == ChallengeManagerServerStub.ServerMode.ClientSimulation)
		{
			string str = "FANFARE:" + Environment.NewLine;
			str = str + "------------------" + Environment.NewLine;
			text2 = str;
			str = text2 + "State: " + mgr.FanfareManager.CurrentState + Environment.NewLine;
			if (mgr.FanfareManager.CurrentState == ChallengeManagerFanfareManager.FanfareStateEnum.Inactive)
			{
				str = str + "Reason: " + mgr.FanfareManager.InactiveReason + Environment.NewLine;
			}
			text2 = str;
			str = text2 + "Storing: " + count + Environment.NewLine;
			if (count > 0)
			{
				FanfareInstance fanfareInstance = mgr.FanfareManager.FanfareQueue.Peek();
				str = str + fanfareInstance.challengeCompleted.Name + Environment.NewLine;
				str = str + fanfareInstance.nextChallengeServerId + Environment.NewLine;
			}
			count = mgr.ChallengeServer.ChallengesPending.Count;
			str += Environment.NewLine;
			str = str + "SERVER:" + Environment.NewLine;
			str = str + "------------------" + Environment.NewLine;
			text2 = str;
			str = text2 + "Mode: " + ChallengeManagerServerStub.CurrentMode + Environment.NewLine;
			text2 = str;
			str = text2 + "Latency: " + mgr.ChallengeServer.LatencyCurrent + Environment.NewLine;
			text2 = str;
			str = text2 + "Storing: " + count + Environment.NewLine;
			if (count > 0)
			{
				foreach (IChallenge item in mgr.ChallengeServer.ChallengesPending)
				{
					str = str + item.Name + Environment.NewLine;
					str = str + item.Status + Environment.NewLine;
				}
			}
			str = str + Environment.NewLine + "Stored Challenge: " + PlayerPrefs.GetString("CM_CC", string.Empty);
			fanfareState.Text = str;
		}
		base.OnUpdate();
	}

	public override void OnShow()
	{
		base.OnShow();
		headerStyle = GUIManager.Instance.StyleManager.GetStyle("DebugConsoleEventHeader");
		scrollPos = Vector2.zero;
	}
}
