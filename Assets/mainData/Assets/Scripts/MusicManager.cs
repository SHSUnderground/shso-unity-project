using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum MusicTransitionType
	{
		CrossFade,
		Abrupt
	}

	protected Dictionary<MusicSource.MusicSourceType, Dictionary<string, List<MusicSource>>> musicByTypeAndTag;

	protected Dictionary<int, TrackedMusicSource> registeredMusicSources;

	private void Awake()
	{
		registeredMusicSources = new Dictionary<int, TrackedMusicSource>();
		musicByTypeAndTag = new Dictionary<MusicSource.MusicSourceType, Dictionary<string, List<MusicSource>>>();
		foreach (int value in Enum.GetValues(typeof(MusicSource.MusicSourceType)))
		{
			musicByTypeAndTag.Add((MusicSource.MusicSourceType)value, new Dictionary<string, List<MusicSource>>());
		}
	}

	public void RegisterMusicSource(MusicSource musicSource, bool sceneSpecific)
	{
		CspUtils.DebugLog("--- Registering music source from game object <" + musicSource.gameObject.name + "> with ID <" + musicSource.gameObject.GetInstanceID() + ">.");
		registeredMusicSources[musicSource.gameObject.GetInstanceID()] = new TrackedMusicSource(musicSource, sceneSpecific);
		List<MusicSource> value;
		if (!musicByTypeAndTag[musicSource.MusicType].TryGetValue(musicSource.MusicTag, out value))
		{
			value = new List<MusicSource>();
			musicByTypeAndTag[musicSource.MusicType].Add(musicSource.MusicTag, value);
		}
		value.Add(musicSource);
	}

	public void UnregisterMusicSource(MusicSource musicSource)
	{
		CspUtils.DebugLog("--- Unregistering music source from game object <" + musicSource.gameObject.name + "> with ID <" + musicSource.gameObject.GetInstanceID() + ">.");
		registeredMusicSources.Remove(musicSource.gameObject.GetInstanceID());
		List<MusicSource> value;
		if (!musicByTypeAndTag[musicSource.MusicType].TryGetValue(musicSource.MusicTag, out value))
		{
			CspUtils.DebugLog("Requested to unregister a music source <" + musicSource.gameObject.name + ">, but was unable to find any music with that type <" + Enum.GetName(typeof(MusicSource.MusicSourceType), musicSource) + " and tag <" + musicSource.MusicTag + ">.");
		}
		else
		{
			value.Remove(musicSource);
		}
	}

	public void OnSceneTransition()
	{
		List<MusicSource> list = new List<MusicSource>();
		foreach (TrackedMusicSource value in registeredMusicSources.Values)
		{
			if (value.DiscardOnSceneTransition)
			{
				list.Add(value.Source);
			}
		}
		foreach (MusicSource item in list)
		{
			UnregisterMusicSource(item);
		}
		list.Clear();
	}

	public void Play(MusicSource.MusicSourceType type, string tag)
	{
		Play(type, tag, MusicTransitionType.CrossFade);
	}

	public void Play(MusicSource.MusicSourceType type, string tag, MusicTransitionType transitionType)
	{
		MusicSource x = SelectMusic(type, tag);
		if (!(x != null))
		{
		}
	}

	protected MusicSource SelectMusic(MusicSource.MusicSourceType type, string tag)
	{
		MusicSource result = null;
		List<MusicSource> value;
		if (musicByTypeAndTag[type].TryGetValue(tag, out value))
		{
			int index = UnityEngine.Random.Range(0, value.Count);
			result = value[index];
		}
		else
		{
			CspUtils.DebugLog("Asked to play music with type <" + type + "> and tag <" + tag + ">, but no matching music was available!");
		}
		return result;
	}
}
