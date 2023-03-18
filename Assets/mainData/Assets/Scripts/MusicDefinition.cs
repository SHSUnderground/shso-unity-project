using System;
using UnityEngine;

[Serializable]
public class MusicDefinition
{
	public enum MusicType
	{
		Level,
		Boss,
		MissionComplete
	}

	public MusicType Type;

	public AudioClip Clip;

	public float Volume = 1f;

	public bool Loop;

	public float FadeTime = 1f;

	public MusicDefinition()
	{
		Volume = 1f;
	}
}
