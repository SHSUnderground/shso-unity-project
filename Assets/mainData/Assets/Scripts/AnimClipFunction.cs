using System;

public class AnimClipFunction : AnimClip
{
	private AnimPath path;

	private Action<float> action;

	public override float TotalTime
	{
		get
		{
			return path.TotalTime;
		}
	}

	public AnimClipFunction(AnimPath path, Action<float> action)
	{
		this.path = path;
		this.action = action;
	}

	public AnimClipFunction(float time, Action<float> action)
	{
		path = AnimPath.Linear(0f, time, time);
		this.action = action;
	}

	public override void Update(float deltaTime)
	{
		if (CurrentState == State.UnStarted)
		{
			CurrentState = State.Running;
		}
		elapsedTime += deltaTime;
		if (SHSDebugAnimClipManagerInfoWindow.RecordingAnim)
		{
			SHSDebugAnimClipManagerInfoWindow.Publish(false, Name, elapsedTime.ToString(), path.GetValue(elapsedTime).ToString());
		}
		action(path.GetValue(elapsedTime));
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
		
		try {  // CSP added try catch
			action(path.GetValue(elapsedTime)); 
		} 
		catch (Exception e) {
			CspUtils.DebugLog(e);
		} 
		FireOnFinished();
	}
}
