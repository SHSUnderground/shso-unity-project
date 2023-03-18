public class AnimClipSequence : AnimClip
{
	private AnimClip a;

	private AnimClip b;

	public override float TotalTime
	{
		get
		{
			return a.TotalTime + b.TotalTime;
		}
	}

	public AnimClipSequence(AnimClip a, AnimClip b)
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
		float num = 0f;
		if (!a.IsTimeUp())
		{
			if (a.TotalTime - a.ElapsedTime < deltaTime)
			{
				num = deltaTime - (a.TotalTime - a.ElapsedTime);
			}
			a.Update(deltaTime);
		}
		else if (!b.IsTimeUp())
		{
			b.Update(deltaTime);
		}
		if (num != 0f)
		{
			b.Update(num);
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
