using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerTime
{
	public const float CLIENT_LATENCY_VARIANCE_THRESHOLD = 10f;

	protected static ServerTime instance;

	protected DateTime unixEpoch;

	protected DateTime clientBase;

	protected double lastServerTime;

	protected TimeSpan serverTimespan;

	protected Queue<long> latencySamples;

	protected long latencySum;

	protected bool serverPingReceived;

	public static double time
	{
		get
		{
			return Instance.GetServerTimeInSeconds();
		}
	}

	public bool ready
	{
		get
		{
			return latencySamples.Count > 0;
		}
	}

	public static ServerTime Instance
	{
		get
		{
			return instance;
		}
	}

	public DateTime UnixEpoch
	{
		get
		{
			return unixEpoch;
		}
	}

	public ServerTime()
	{
		if (instance != null)
		{
			CspUtils.DebugLog("ServerTime already created, attempt to create a second one will lead to instability.");
		}
		else
		{
			instance = this;
		}
		Reset();
	}

	public void Reset()
	{
		latencySum = 0L;
		latencySamples = new Queue<long>();
		unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		clientBase = DateTime.UtcNow;
		serverTimespan = clientBase - unixEpoch;
		lastServerTime = 0.0;
	}

	public void ProcessPingResult(string clientTxt, string serverTxt)
	{
		//CspUtils.DebugLog("clientTxt=" + clientTxt);
		//CspUtils.DebugLog("serverTxt=" + serverTxt);
		long num = long.Parse(clientTxt);
		long num2 = long.Parse(serverTxt) * 1000;   //CSP - convert from sec to miilisec (extension overflowed on millisec)
		long num3 = (GetClientTimeInMilliseconds() - num) / 2;
		latencySum += num3;
		latencySamples.Enqueue(num3);
		if (latencySamples.Count > 10)
		{
			latencySum -= latencySamples.Dequeue();
		}
		long num4 = latencySum / latencySamples.Count;
		if (serverPingReceived && Math.Abs((DateTime.UtcNow - clientBase).TotalSeconds) > 10.0)
		{
			CspUtils.DebugLog("Large client variance detected between pings of server (" + Math.Abs((DateTime.UtcNow - clientBase).TotalSeconds) + ".Using raw server ping return time only");
			lastServerTime = serverTimespan.TotalSeconds;
		}
		else
		{
			lastServerTime = GetServerTimeInSeconds();
		}
		serverTimespan = TimeSpan.FromMilliseconds(Convert.ToDouble(num2 + num4));
		clientBase = DateTime.UtcNow;
		serverPingReceived = true;
	}

	public double GetServerTimeInSeconds()
	{
		double totalSeconds = (serverTimespan + (DateTime.UtcNow - clientBase)).TotalSeconds;
		if (totalSeconds >= lastServerTime)
		{
			return totalSeconds;
		}
		return lastServerTime;
	}

	public DateTime GetServerTimeInDateTime()
	{
		return unixEpoch + (serverTimespan + (DateTime.UtcNow - clientBase));
	}

	public DateTime ComputeCachedServerTime(double time)
	{
		return unixEpoch + TimeSpan.FromSeconds(time);
	}

	public long GetClientTimeInMilliseconds()
	{
		return Convert.ToInt64((DateTime.UtcNow - unixEpoch).TotalMilliseconds);
	}

	public long GetAverageLatency()
	{
		return latencySum / latencySamples.Count;
	}
}
