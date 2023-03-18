using System.Collections.Generic;
using UnityEngine;

public class CharacterTestEmote : CharacterTestBase
{
	public bool Positive = true;

	public bool Aggressive = true;

	public bool Reactive = true;

	public bool PowerEmote = true;

	public bool Internal = true;

	public bool BubbleEmotions = true;

	public bool BubbleExclamations = true;

	public bool BubbleGreetings = true;

	public bool BubbleHeroic = true;

	public bool BubbleInstructions = true;

	public bool BubblePlay = true;

	public float timeBetweenEmotes = 1f;

	protected bool activated;

	protected float nextEmoteTime;

	protected List<sbyte> emoteList;

	public void Start()
	{
		emoteList = new List<sbyte>();
	}

	public void Update()
	{
		if (!activated)
		{
			return;
		}
		if (nextEmoteTime == 0f)
		{
			bool flag = true;
			foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
			{
				if (!(character.behaviorManager.getBehavior() is BehaviorMovement))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				nextEmoteTime = Time.time + timeBetweenEmotes;
			}
		}
		else if (Time.time > nextEmoteTime)
		{
			nextEmoteTime = 0f;
			playNextEmote();
		}
	}

	protected void playNextEmote()
	{
		if (emoteList.Count == 0)
		{
			activated = false;
			CharacterTest.Instance.TestDone();
		}
		else
		{
			sbyte b = emoteList[0];
			emoteList.RemoveAt(0);
			foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
			{
				BehaviorEmote behaviorEmote = character.behaviorManager.requestChangeBehavior(typeof(BehaviorEmote), false) as BehaviorEmote;
				if (behaviorEmote != null)
				{
					if (EmotesDefinition.Instance.GetEmoteById(b).isLooping)
					{
						behaviorEmote.Initialize(b, false, 5f);
					}
					else
					{
						behaviorEmote.Initialize(b);
					}
				}
			}
		}
	}

	public override void Activate()
	{
		nextEmoteTime = 0f;
		emoteList.Clear();
		if (Positive)
		{
			addEmotes(EmotesDefinition.Instance.GetEmotesByCategory(EmotesDefinition.EmoteCategoriesEnum.Positive));
		}
		if (Aggressive)
		{
			addEmotes(EmotesDefinition.Instance.GetEmotesByCategory(EmotesDefinition.EmoteCategoriesEnum.Aggressive));
		}
		if (Reactive)
		{
			addEmotes(EmotesDefinition.Instance.GetEmotesByCategory(EmotesDefinition.EmoteCategoriesEnum.Reactive));
		}
		if (PowerEmote)
		{
			addEmotes(EmotesDefinition.Instance.GetEmotesByCategory(EmotesDefinition.EmoteCategoriesEnum.PowerEmote));
		}
		if (Internal)
		{
			addEmotes(EmotesDefinition.Instance.GetEmotesByCategory(EmotesDefinition.EmoteCategoriesEnum.Internal));
		}
		if (BubbleEmotions)
		{
			addEmotes(EmotesDefinition.Instance.GetEmotesByCategory(EmotesDefinition.EmoteCategoriesEnum.BubbleEmotions));
		}
		if (BubbleExclamations)
		{
			addEmotes(EmotesDefinition.Instance.GetEmotesByCategory(EmotesDefinition.EmoteCategoriesEnum.BubbleExclamations));
		}
		if (BubbleGreetings)
		{
			addEmotes(EmotesDefinition.Instance.GetEmotesByCategory(EmotesDefinition.EmoteCategoriesEnum.BubbleGreetings));
		}
		if (BubbleHeroic)
		{
			addEmotes(EmotesDefinition.Instance.GetEmotesByCategory(EmotesDefinition.EmoteCategoriesEnum.BubbleHeroic));
		}
		if (BubbleInstructions)
		{
			addEmotes(EmotesDefinition.Instance.GetEmotesByCategory(EmotesDefinition.EmoteCategoriesEnum.BubbleInstructions));
		}
		if (BubblePlay)
		{
			addEmotes(EmotesDefinition.Instance.GetEmotesByCategory(EmotesDefinition.EmoteCategoriesEnum.BubblePlay));
		}
		activated = true;
	}

	protected void addEmotes(List<EmotesDefinition.EmoteDefinition> emotes)
	{
		foreach (EmotesDefinition.EmoteDefinition emote in emotes)
		{
			emoteList.Add(emote.id);
		}
	}
}
