using System;
using System.Runtime.CompilerServices;

public abstract class AnimClip
{
	public enum State
	{
		UnStarted,
		Running,
		Complete,
		ForceComplete
	}

	public State CurrentState;

	protected float elapsedTime;

	public string Name;

	private Action onFinished;

	public abstract float TotalTime
	{
		get;
	}

	public bool Done
	{
		get
		{
			return CurrentState == State.Complete || CurrentState == State.ForceComplete;
		}
	}

	public float ElapsedTime
	{
		get
		{
			return elapsedTime;
		}
		set
		{
			elapsedTime = value;
		}
	}

	public float TimeOver
	{
		get
		{
			return ElapsedTime - TotalTime;
		}
	}

	public event Action OnFinished
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.onFinished += value;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.onFinished -= value;
		}
	}

	public abstract void Update(float deltaTime);

	public abstract void ForceComplete();

	protected void FireOnFinished()
	{
		if ((object)this.onFinished != null)
		{
			this.onFinished.Invoke();
		}
	}

	public bool IsTimeUp()
	{
		return elapsedTime >= TotalTime && CurrentState != State.UnStarted;
	}

	public static AnimClip operator ^(AnimClip PieceOne, AnimClip PieceTwo)
	{
		return new AnimClipUnion(PieceOne, PieceTwo);
	}

	public static AnimClip operator |(AnimClip PieceOne, AnimClip PieceTwo)
	{
		return new AnimClipSequence(PieceOne, PieceTwo);
	}
}
