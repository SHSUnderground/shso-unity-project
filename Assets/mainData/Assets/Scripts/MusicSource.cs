using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicSource : ShsAudioSource
{
	public enum MusicSourceType
	{
		Intro,
		Outro,
		Explore,
		Brawl
	}

	public enum MusicFollowOnAction
	{
		Next,
		Previous,
		First,
		Last,
		Random,
		Tag
	}

	public MusicSourceType MusicType = MusicSourceType.Explore;

	public string MusicTag = string.Empty;

	public MusicFollowOnAction FollowOnAction;

	public string FollowOnTag = string.Empty;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
	}
}
