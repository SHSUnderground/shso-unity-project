using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("ai_hq_controller")]
public class AIControllerHQData
{
	public enum RelationshipType
	{
		like,
		dislike
	}

	public enum Affiliation
	{
		Hero,
		Villain
	}

	public class emote
	{
		public sbyte id;

		public emote()
		{
		}

		public emote(sbyte id)
		{
			this.id = id;
		}
	}

	public string name;

	public SerializableDictionary<int, int> item_keyword_affinities = new SerializableDictionary<int, int>();

	public SerializableDictionary<string, int> room_affinities = new SerializableDictionary<string, int>();

	public int marvel_squad;

	public Affiliation affiliation;

	public sbyte destroy_sequence_id;

	public int strength;

	public float hunger_max;

	public float hunger_rate = 1.75f;

	public int anger_frequency;

	public int anger_duration;

	public string flinga_placeholder;

	public SerializableDictionary<RelationshipType, List<emote>> relationship_emotes = new SerializableDictionary<RelationshipType, List<emote>>();

	public SerializableDictionary<AIControllerHQ.Mood, List<emote>> mood_emotes = new SerializableDictionary<AIControllerHQ.Mood, List<emote>>();

	public SerializableDictionary<AIControllerHQ.Mood, string> mood_icons = new SerializableDictionary<AIControllerHQ.Mood, string>();

	public void InitializeDefaults()
	{
		if (!mood_emotes.ContainsKey(AIControllerHQ.Mood.Happy))
		{
			mood_emotes[AIControllerHQ.Mood.Happy] = new List<emote>();
			mood_emotes[AIControllerHQ.Mood.Happy].Add(new emote(54));
			mood_emotes[AIControllerHQ.Mood.Happy].Add(new emote(55));
		}
		if (!mood_emotes.ContainsKey(AIControllerHQ.Mood.Pleasant))
		{
			mood_emotes[AIControllerHQ.Mood.Pleasant] = new List<emote>();
			mood_emotes[AIControllerHQ.Mood.Pleasant].Add(new emote(69));
			mood_emotes[AIControllerHQ.Mood.Pleasant].Add(new emote(56));
		}
		if (!mood_emotes.ContainsKey(AIControllerHQ.Mood.Content))
		{
			mood_emotes[AIControllerHQ.Mood.Content] = new List<emote>();
			mood_emotes[AIControllerHQ.Mood.Content].Add(new emote(61));
		}
		if (!mood_emotes.ContainsKey(AIControllerHQ.Mood.Indifferent))
		{
			mood_emotes[AIControllerHQ.Mood.Indifferent] = new List<emote>();
			mood_emotes[AIControllerHQ.Mood.Indifferent].Add(new emote(72));
			mood_emotes[AIControllerHQ.Mood.Indifferent].Add(new emote(59));
		}
		if (!mood_emotes.ContainsKey(AIControllerHQ.Mood.Disgruntled))
		{
			mood_emotes[AIControllerHQ.Mood.Disgruntled] = new List<emote>();
			mood_emotes[AIControllerHQ.Mood.Disgruntled].Add(new emote(52));
			mood_emotes[AIControllerHQ.Mood.Disgruntled].Add(new emote(60));
		}
		if (!mood_emotes.ContainsKey(AIControllerHQ.Mood.Enraged))
		{
			mood_emotes[AIControllerHQ.Mood.Enraged] = new List<emote>();
			mood_emotes[AIControllerHQ.Mood.Enraged].Add(new emote(70));
			mood_emotes[AIControllerHQ.Mood.Enraged].Add(new emote(53));
			mood_emotes[AIControllerHQ.Mood.Enraged].Add(new emote(51));
		}
		if (!mood_icons.ContainsKey(AIControllerHQ.Mood.Happy))
		{
			mood_icons[AIControllerHQ.Mood.Happy] = "hq_bundle|Happy";
		}
		if (!mood_icons.ContainsKey(AIControllerHQ.Mood.Pleasant))
		{
			mood_icons[AIControllerHQ.Mood.Pleasant] = "hq_bundle|Pleasant";
		}
		if (!mood_icons.ContainsKey(AIControllerHQ.Mood.Content))
		{
			mood_icons[AIControllerHQ.Mood.Content] = "hq_bundle|Content";
		}
		if (!mood_icons.ContainsKey(AIControllerHQ.Mood.Indifferent))
		{
			mood_icons[AIControllerHQ.Mood.Indifferent] = "hq_bundle|Indifferent";
		}
		if (!mood_icons.ContainsKey(AIControllerHQ.Mood.Disgruntled))
		{
			mood_icons[AIControllerHQ.Mood.Disgruntled] = "hq_bundle|Disgruntled";
		}
		if (!mood_icons.ContainsKey(AIControllerHQ.Mood.Enraged))
		{
			mood_icons[AIControllerHQ.Mood.Enraged] = "hq_bundle|Enraged";
		}
	}
}
