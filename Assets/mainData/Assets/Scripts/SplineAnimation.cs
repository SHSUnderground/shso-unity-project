using System;

[Serializable]
public class SplineAnimation : SplineEvent
{
	public string animation;

	public float blendTime = 0.25f;

	public bool looping;

	public bool scaleTime;
}
