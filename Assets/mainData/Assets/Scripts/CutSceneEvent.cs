using UnityEngine;

public class CutSceneEvent : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public delegate void OnCutSceneEventEnd();

	public float eventTime;

	private float mStartTime;

	private bool mPlayed;

	protected OnCutSceneEventEnd mOnEventEnd;

	public float EventTime
	{
		get
		{
			return eventTime;
		}
	}

	public float StartTime
	{
		get
		{
			return mStartTime;
		}
	}

	public float ElapsedTime
	{
		get
		{
			return Time.time - mStartTime;
		}
	}

	public CutSceneEvent()
	{
		eventTime = 0f;
		mStartTime = -1f;
		mPlayed = false;
		mOnEventEnd = null;
	}

	public void InitializeEvent(OnCutSceneEventEnd onEventEnd)
	{
		mOnEventEnd = onEventEnd;
	}

	public virtual void StartEvent()
	{
		mStartTime = Time.time;
	}

	public virtual void UpdateEvent()
	{
	}

	public virtual void EndEvent()
	{
		mStartTime = -1f;
		mPlayed = true;
		if (mOnEventEnd != null)
		{
			mOnEventEnd();
		}
	}

	public bool EventStarted()
	{
		return StartTime >= 0f;
	}

	public bool EventTimeElapsed()
	{
		return EventTime < ElapsedTime;
	}

	public bool EventEnded()
	{
		return mPlayed;
	}

	public void Update()
	{
		if (EventStarted())
		{
			if (EventTimeElapsed())
			{
				EndEvent();
			}
			else
			{
				UpdateEvent();
			}
		}
	}

	public void LogEventError(string error)
	{
		CspUtils.DebugLog("Cut Scene Event Error: " + error);
	}

	public void DrawGizmoToTargetTransform(Transform target)
	{
		Gizmos.color = Color.grey;
		Gizmos.DrawLine(base.transform.position, target.position);
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(target.position, new Vector3(0.25f, 0.25f, 0.25f));
	}
}
