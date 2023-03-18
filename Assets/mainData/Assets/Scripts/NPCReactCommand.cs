using System;
using UnityEngine;

[Serializable]
public class NPCReactCommand : NPCCommandBase
{
	public enum ReactionTypeEnum
	{
		Emote,
		PowerEmote,
		Interaction,
		TroubleBotDestruction
	}

	public const string DEFAULT_INTERACT_RESPONSE = "emote_laugh";

	public ReactionTypeEnum reactionType;

	public float reactMinTime = 0.2f;

	public float reactMaxTime = 0.8f;

	public sbyte emoteId;

	private float delay = float.MaxValue;

	private bool delayComplete;

	public NPCReactCommand()
	{
		interruptable = false;
		type = NPCCommandTypeEnum.React;
	}

	public override void Start()
	{
		Start(null);
	}

	public override void Start(params object[] initValues)
	{
		base.Start(initValues);
		if (reactionType == ReactionTypeEnum.TroubleBotDestruction)
		{
			OnTurnCompleted(gameObject);
			return;
		}
		BehaviorTurnTo behaviorTurnTo = behaviorManager.requestChangeBehavior(typeof(BehaviorTurnTo), false) as BehaviorTurnTo;
		behaviorTurnTo.Initialize(target.transform.position, OnTurnCompleted);
	}

	private void OnTurnCompleted(GameObject go)
	{
		startTime = Time.time;
		delay = UnityEngine.Random.Range(reactMinTime, reactMaxTime);
	}

	public override void Suspend()
	{
		base.Suspend();
	}

	public override void Resume()
	{
		base.Resume();
	}

	public override void Completed()
	{
		base.Completed();
	}

	public override NPCCommandResultEnum Update()
	{
		bool flag = false;
		if (Time.time - startTime > delay && !delayComplete)
		{
			delayComplete = true;
			if (reactionType == ReactionTypeEnum.Emote)
			{
				flag = reactToEmote();
			}
			else if (reactionType == ReactionTypeEnum.Interaction)
			{
				flag = reactToInteract();
			}
			else if (reactionType == ReactionTypeEnum.TroubleBotDestruction)
			{
				flag = reactToEmote();
			}
		}
		return (!isDone || flag) ? NPCCommandResultEnum.InProgress : NPCCommandResultEnum.Completed;
	}

	private bool reactToInteract()
	{
		playReaction("emote_laugh");
		return false;
	}

	private bool reactToEmote()
	{
		EmotesDefinition.EmoteDefinition emoteById = EmotesDefinition.Instance.GetEmoteById(emoteId);
		if (emoteById == null)
		{
			CspUtils.DebugLog("Cant find emote id: " + emoteId);
			return false;
		}
		string value;
		if (!aiController.SpecializedEmoteReactionList.TryGetValue(emoteById.command, out value))
		{
			value = emoteById.npcReaction;
			if (string.IsNullOrEmpty(value))
			{
				CspUtils.DebugLog("Emote: " + emoteId + " has no reaction animation");
				return false;
			}
		}
		if (emoteById.isLooping)
		{
			playLoopingReaction(value);
		}
		else
		{
			playReaction(value);
		}
		AppShell.Instance.EventMgr.Fire(this, new NPCEmoteReactMessage(target, value));
		return false;
	}

	private void playReaction(string reaction)
	{
		BehaviorAnimate behaviorAnimate = behaviorManager.requestChangeBehavior(typeof(BehaviorAnimate), false) as BehaviorAnimate;
		behaviorAnimate.Initialize(reaction, delegate
		{
			isDone = true;
		});
	}

	private void playLoopingReaction(string reaction)
	{
		BehaviorLoopAnimate behaviorLoopAnimate = behaviorManager.requestChangeBehavior<BehaviorLoopAnimate>(false);
		if (behaviorLoopAnimate != null)
		{
			behaviorLoopAnimate.Initialize(reaction, delegate
			{
				if (target == null)
				{
					return true;
				}
				BehaviorEmote behaviorEmote = Utils.GetComponent<BehaviorManager>(target).getBehavior() as BehaviorEmote;
				return (behaviorEmote == null || behaviorEmote.EmoteID != emoteId) ? true : false;
			}, delegate
			{
				isDone = true;
			});
		}
	}

	public override string ToString()
	{
		EmotesDefinition.EmoteDefinition emoteDefinition = null;
		if (emoteId != 0)
		{
			EmotesDefinition.Instance.GetEmoteById(emoteId);
		}
		return type.ToString() + ": Source(" + ((!(target != null)) ? "null" : target.gameObject.name) + " Type: " + reactionType + " Emote: " + ((emoteDefinition != null) ? emoteDefinition.command : "null");
	}
}
