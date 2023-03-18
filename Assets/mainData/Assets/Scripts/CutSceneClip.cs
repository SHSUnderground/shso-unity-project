using System;
using UnityEngine;

[Serializable]
public class CutSceneClip
{
	public delegate void OnClipEnd();

	public GameObject clipPrefab;

	public float timeOffset;

	private bool mPlayed;

	private bool mIsPlaying;

	private CutSceneEvent[] mEvents;

	private OnClipEnd mOnClipEnd;

	public bool Played
	{
		get
		{
			return mPlayed;
		}
	}

	public bool IsPlaying
	{
		get
		{
			return mIsPlaying;
		}
	}

	public CutSceneClip()
	{
		clipPrefab = null;
		timeOffset = 0f;
		mPlayed = false;
		mIsPlaying = false;
		mEvents = null;
		mOnClipEnd = null;
	}

	public void InitializeClip(OnClipEnd onClipEnd)
	{
		mOnClipEnd = onClipEnd;
		mEvents = clipPrefab.GetComponents<CutSceneEvent>();
		CutSceneEvent[] array = mEvents;
		foreach (CutSceneEvent cutSceneEvent in array)
		{
			cutSceneEvent.InitializeEvent(OnClipEventEnd);
		}
	}

	public void StartClip()
	{
		CutSceneEvent[] array = mEvents;
		foreach (CutSceneEvent cutSceneEvent in array)
		{
			cutSceneEvent.StartEvent();
		}
		mIsPlaying = true;
		mPlayed = false;
	}

	public void OnClipEventEnd()
	{
		CutSceneEvent[] array = mEvents;
		foreach (CutSceneEvent cutSceneEvent in array)
		{
			if (!cutSceneEvent.EventEnded())
			{
				return;
			}
		}
		mIsPlaying = false;
		mPlayed = true;
		if (mOnClipEnd != null)
		{
			mOnClipEnd();
		}
	}
}
