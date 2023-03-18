using System.Xml.Serialization;

[XmlRoot(ElementName = "preset")]
public class AudioPresetVoice : AudioPreset
{
	public enum AudioStealBehavior
	{
		fail,
		steal_oldest,
		steal_quietest
	}

	protected int priority;

	protected int maxPlaybacks;

	protected AudioStealBehavior stealBehavior;

	[XmlElement(ElementName = "audio_priority")]
	public int Priority
	{
		get
		{
			return priority;
		}
		set
		{
			priority = value;
		}
	}

	[XmlElement(ElementName = "audio_max_playbacks")]
	public int MaxPlaybacks
	{
		get
		{
			return maxPlaybacks;
		}
		set
		{
			maxPlaybacks = value;
		}
	}

	[XmlElement(ElementName = "audio_steal_behavior")]
	public AudioStealBehavior StealBehavior
	{
		get
		{
			return stealBehavior;
		}
		set
		{
			stealBehavior = value;
		}
	}
}
