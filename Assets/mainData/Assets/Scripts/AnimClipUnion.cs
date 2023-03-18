using UnityEngine;

public class AnimClipUnion : AnimClip
{
	private AnimClip a;

	private AnimClip b;

	public override float TotalTime
	{
		get
		{
			return Mathf.Max(a.TotalTime, b.TotalTime);
		}
	}

	public AnimClipUnion(AnimClip a, AnimClip b)
	{
		this.a = a;
		this.b = b;
	}

	public override void Update(float deltaTime)
	{
		if (CurrentState == State.UnStarted)
		{
			CurrentState = State.Running;
		}
		elapsedTime += deltaTime;
		if (!a.IsTimeUp())
		{
			a.Update(deltaTime);
		}
		if (!b.IsTimeUp())
		{
			b.Update(deltaTime);
		}
		if (IsTimeUp())
		{
			CurrentState = State.Complete;
			FireOnFinished();
		}
	}

	public override void ForceComplete()
	{
		CurrentState = State.ForceComplete;
		elapsedTime = TotalTime;
		a.ForceComplete();
		b.ForceComplete();
		FireOnFinished();
	}
}
