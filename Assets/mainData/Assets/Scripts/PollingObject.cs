using System;
using System.Collections;
using UnityEngine;

public class PollingObject : YieldInstruction, IDisposable, IEnumerator
{
	public object Current
	{
		get
		{
			CspUtils.DebugLog("get Current");
			return 1;
		}
	}

	public PollingObject()
	{
		CspUtils.DebugLog("PollingObject");
	}

	public bool MoveNext()
	{
		CspUtils.DebugLog("MoveNext");
		return true;
	}

	public void Reset()
	{
		CspUtils.DebugLog("Reset");
	}

	public void Dispose()
	{
		CspUtils.DebugLog("Dispose");
	}
}
