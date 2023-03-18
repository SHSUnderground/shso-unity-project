using UnityEngine;

public class SHSTimerEx : GUIControlWindow
{
	public enum TimerStateEnum
	{
		Idle,
		Running
	}

	public enum TimerEventType
	{
		Started,
		Update,
		Completed
	}

	public delegate void TimerEvent(TimerEventType Type, int data);

	private float timeCurrent;

	private float speed = 1f;

	private float timeStart;

	private float timeEnd;

	private TimerStateEnum timerState;

	private float duration;

	public float TimeCurrent
	{
		get
		{
			return timeCurrent;
		}
	}

	public float Speed
	{
		get
		{
			return speed;
		}
		set
		{
			speed = value;
		}
	}

	public float TimeStart
	{
		get
		{
			return timeStart;
		}
	}

	public float TimeEnd
	{
		get
		{
			return timeEnd;
		}
	}

	public TimerStateEnum TimerState
	{
		get
		{
			return timerState;
		}
		set
		{
			timerState = value;
		}
	}

	public float Duration
	{
		get
		{
			return duration;
		}
		set
		{
			duration = value;
		}
	}

	public float TimeElapsed
	{
		get
		{
			return timeCurrent - timeStart;
		}
	}

	public float TimeLeft
	{
		get
		{
			return timeEnd - timeCurrent;
		}
	}

	public event TimerEvent OnTimerEvent;

	public void Start()
	{
		timeStart = Time.time;
		timeEnd = timeStart + duration;
		timeCurrent = Time.time;
		timerState = TimerStateEnum.Running;
		if (this.OnTimerEvent != null)
		{
			this.OnTimerEvent(TimerEventType.Started, 0);
		}
	}

	public void Stop()
	{
		Abort();
		if (this.OnTimerEvent != null)
		{
			this.OnTimerEvent(TimerEventType.Completed, 0);
		}
	}

	public void Abort()
	{
		timeEnd = Time.time;
		timeCurrent = Time.time;
		timerState = TimerStateEnum.Idle;
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (timerState != 0)
		{
			timeCurrent = Time.time * speed;
			if (timeCurrent > timeEnd)
			{
				Stop();
			}
			else if (this.OnTimerEvent != null)
			{
				this.OnTimerEvent(TimerEventType.Update, 0);
			}
		}
	}

	public override void OnInactive()
	{
		if (timerState == TimerStateEnum.Running)
		{
			Stop();
		}
		base.OnInactive();
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
	}
}
