using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("ScenarioEvent/SynchronousTransferBox")]
[RequireComponent(typeof(NetworkComponent))]
public class ScenarioEventSynchronousTransferBox : ScenarioEventTransferBox
{
	public double serverTimeDelay;

	protected NetworkComponent netComponent;

	protected List<double> serverFireTimes;

	protected List<double> hostServerFireTimes;

	protected bool isHost;

	public void SynchronizeToHost(double serverTime)
	{
		if (hostServerFireTimes != null)
		{
			hostServerFireTimes.Add(serverTime);
		}
		else
		{
			CspUtils.DebugLog("ScenarioEventSynchronousTransferBox::SynchronizeToHost() - host server fire time list is null and cannot add server time for event fire");
		}
	}

	protected void Synchronize(double serverTime)
	{
		if (!(AppShell.Instance == null) && AppShell.Instance.ServerConnection != null)
		{
			if (netComponent == null)
			{
				CspUtils.DebugLog("ScenarioEventSynchronousTransferBox::Synchronize() - network component not found on game object and event will not be synchronized!");
			}
			else if (AppShell.Instance.ServerConnection.IsGameHost())
			{
				ScenarioEventServerTimeMessage msg = new ScenarioEventServerTimeMessage(netComponent.goNetId, serverTime);
				AppShell.Instance.ServerConnection.SendGameMsg(msg);
				SynchronizeToHost(serverTime);
			}
			else if (serverFireTimes != null)
			{
				serverFireTimes.Add(serverTime);
			}
			else
			{
				CspUtils.DebugLog("ScenarioEventSynchronousTransferBox::Synchronize() - server fire time list is null and will be unable to send event on any host transfer");
			}
		}
	}

	protected void OnBecomeHost()
	{
		isHost = true;
		if (serverFireTimes != null && serverFireTimes.Count > 0)
		{
			if (hostServerFireTimes != null && hostServerFireTimes.Count <= 0)
			{
				foreach (double serverFireTime in serverFireTimes)
				{
					double serverTime = serverFireTime;
					Synchronize(serverTime);
				}
			}
			serverFireTimes.Clear();
		}
	}

	protected override void subscribeEvents()
	{
		if (!subscribed)
		{
			base.subscribeEvents();
			serverFireTimes = new List<double>();
			hostServerFireTimes = new List<double>();
		}
	}

	protected override void fireScenarioEvent()
	{
		Synchronize(ServerTime.time + serverTimeDelay);
	}

	protected override void Start()
	{
		base.Start();
		netComponent = base.gameObject.GetComponent<NetworkComponent>();
	}

	protected override void Update()
	{
		base.Update();
		if (AppShell.Instance != null && AppShell.Instance.ServerConnection != null && !isHost && AppShell.Instance.ServerConnection.IsGameHost())
		{
			OnBecomeHost();
		}
		if (hostServerFireTimes == null)
		{
			return;
		}
		double serverTime = ServerTime.time;
		bool flag = false;
		foreach (double hostServerFireTime in hostServerFireTimes)
		{
			double num = hostServerFireTime;
			if (num <= serverTime)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			base.fireScenarioEvent();
			hostServerFireTimes.RemoveAll(delegate(double time)
			{
				return time <= serverTime;
			});
			if (serverFireTimes != null)
			{
				serverFireTimes.RemoveAll(delegate(double time)
				{
					return time <= serverTime;
				});
			}
		}
	}

	protected override void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "ScenarioEventSynchronousTransferBoxIcon.png");
	}
}
