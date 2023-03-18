using System.Collections.Generic;

public class VoiceLimitingPlayInfo
{
	protected string groupName;

	protected int playingCount;

	protected int maxPlaybacks;

	protected List<ShsAudioSource> sources;

	public string GroupName
	{
		get
		{
			return groupName;
		}
	}

	public int PlayingCount
	{
		get
		{
			return sources.Count;
		}
	}

	public int MaxPlaybacks
	{
		get
		{
			return maxPlaybacks;
		}
	}

	public VoiceLimitingPlayInfo(string groupName, int maxPlaybacks)
	{
		this.groupName = groupName;
		this.maxPlaybacks = maxPlaybacks;
		if (maxPlaybacks > 0)
		{
			sources = new List<ShsAudioSource>(maxPlaybacks);
		}
		else
		{
			sources = new List<ShsAudioSource>();
		}
	}

	public void AddAudioSource(ShsAudioSource audioSource)
	{
		sources.Add(audioSource);
	}

	public ShsAudioSource RemoveOldestSource()
	{
		if (sources.Count <= 0)
		{
			return null;
		}
		ShsAudioSource result = sources[0];
		sources.RemoveAt(0);
		return result;
	}

	public void RemoveAudioSource(ShsAudioSource audioSource)
	{
		sources.Remove(audioSource);
	}

	public ShsAudioSource FindLowestPriorityAudioSource(AudioPresetVoice.AudioStealBehavior stealingBehavior)
	{
		if (sources.Count <= 0)
		{
			return null;
		}
		ShsAudioSource shsAudioSource = sources[0];
		foreach (ShsAudioSource source in sources)
		{
			if (source.PresetBundle.PresetVoice.Priority > shsAudioSource.PresetBundle.PresetVoice.Priority)
			{
				shsAudioSource = source;
			}
			else if (source.PresetBundle.PresetVoice.Priority == shsAudioSource.PresetBundle.PresetVoice.Priority && stealingBehavior == AudioPresetVoice.AudioStealBehavior.steal_quietest && source.Volume < shsAudioSource.Volume)
			{
				shsAudioSource = source;
			}
		}
		return shsAudioSource;
	}

	public void PruneNullSources()
	{
		List<ShsAudioSource> list = null;
		foreach (ShsAudioSource source in sources)
		{
			if (source == null)
			{
				CspUtils.DebugLog("Audio source was not properly cleaned up, and is still tracked as a voice!");
				if (list == null)
				{
					list = new List<ShsAudioSource>();
				}
				list.Add(source);
			}
		}
		if (list != null)
		{
			foreach (ShsAudioSource item in list)
			{
				sources.Remove(item);
			}
		}
	}
}
