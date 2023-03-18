using System.Collections.Generic;
using UnityEngine;

public class ChallengeManagerServerStub
{
	public enum ServerMode
	{
		ClientSimulation,
		Regression,
		Passthrough
	}

	private static ServerMode currentMode = ServerMode.Passthrough;

	private readonly List<IChallenge> challengesPending;

	private readonly List<ChallengeInfo> sortedChallengeInfoList;

	protected float latencyRange;

	protected float latencyCurrent;

	protected ChallengeManager manager;

	public static ServerMode CurrentMode
	{
		get
		{
			return currentMode;
		}
		set
		{
			currentMode = value;
		}
	}

	public List<IChallenge> ChallengesPending
	{
		get
		{
			return challengesPending;
		}
	}

	public float LatencyRange
	{
		get
		{
			return latencyRange;
		}
		set
		{
			latencyRange = value;
		}
	}

	public float LatencyCurrent
	{
		get
		{
			return latencyCurrent;
		}
	}

	public ChallengeManagerServerStub(ChallengeManager manager)
	{
		challengesPending = new List<IChallenge>();
		sortedChallengeInfoList = new List<ChallengeInfo>();
		latencyRange = 0.5f;
		this.manager = manager;
		currentMode = ((PlayerPrefs.GetInt("challengesimulated", 0) != 1) ? ServerMode.Passthrough : ServerMode.ClientSimulation);
	}

	public void Initialize()
	{
		foreach (KeyValuePair<int, ChallengeInfo> item in manager.ChallengeDictionary)
		{
			sortedChallengeInfoList.Add(item.Value);
		}
	}

	public void NotifyServerChallengeCompleted(IChallenge challenge)
	{
		if (manager.Enabled)
		{
			if (currentMode == ServerMode.ClientSimulation)
			{
				challengesPending.Add(challenge);
				latencyCurrent = latencyRange;
			}
			else if (currentMode == ServerMode.Passthrough)
			{
				string uri = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/", "resources$", "users/", AppShell.Instance.Profile.UserId, "challenge", "met", challenge.Id);
				AppShell.Instance.WebService.StartRequest(uri, delegate(ShsWebResponse response)
				{
					if (response.Status != 200)
					{
						CspUtils.DebugLog("Challenge Reporting error: " + response.Status + ":" + response.Body);
					}
				}, null, ShsWebService.ShsWebServiceType.RASP);
			}
		}
	}

	public void Update()
	{
		if (!(latencyCurrent > 0f))
		{
			return;
		}
		latencyCurrent -= Time.deltaTime;
		if (latencyCurrent <= 0f)
		{
			if (challengesPending.Count > 0)
			{
				foreach (IChallenge item in challengesPending)
				{
					int nextChallengeId = 0;
					IChallenge localChallenge = item;
					int num = sortedChallengeInfoList.FindIndex(delegate(ChallengeInfo entry)
					{
						return entry.ChallengeId == localChallenge.Id;
					});
					if (num + 1 < sortedChallengeInfoList.Count)
					{
						nextChallengeId = sortedChallengeInfoList[num + 1].ChallengeId;
					}
					ChallengeServerMessage message = new ChallengeServerMessage(item.Id, nextChallengeId, ChallengeRewardType.Unknown, string.Empty, "true");
					manager.ProcessServerChallengeAckMessage(message);
				}
				challengesPending.Clear();
			}
			else
			{
				CspUtils.DebugLog("Server expecting challenge updates to pass through to the server, but the queue is empty.");
			}
		}
	}
}
