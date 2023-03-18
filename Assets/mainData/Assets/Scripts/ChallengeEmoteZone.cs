using UnityEngine;

public class ChallengeEmoteZone : ChallengeZone
{
	public string challengeEmote;

	private bool _isListeningForEmote;

	public bool IsChallengeEmote(sbyte emote)
	{
		if (EmotesDefinition.Instance == null)
		{
			return false;
		}
		EmotesDefinition.EmoteDefinition emoteById = EmotesDefinition.Instance.GetEmoteById(emote);
		if (emoteById == null)
		{
			return false;
		}
		return emoteById.command == challengeEmote;
	}

	private void OnEmote(EmoteSequenceMessage msg)
	{
		if (Utils.IsLocalPlayer(msg.sender) && base.IsZoneChallengeMet() && IsChallengeEmote(msg.emote))
		{
			FireChallengeZoneEvent();
		}
	}

	private void AddEmoteListener()
	{
		if (base.LocalPlayerInZone && !_isListeningForEmote && AppShell.Instance != null && AppShell.Instance.EventMgr != null)
		{
			AppShell.Instance.EventMgr.AddListener<EmoteSequenceMessage>(OnEmote);
			_isListeningForEmote = true;
		}
	}

	private void RemoveEmoteListener()
	{
		if (!base.LocalPlayerInZone && _isListeningForEmote && AppShell.Instance != null && AppShell.Instance.EventMgr != null)
		{
			AppShell.Instance.EventMgr.RemoveListener<EmoteSequenceMessage>(OnEmote);
			_isListeningForEmote = false;
		}
	}

	public override bool IsZoneChallengeMet()
	{
		if (!base.IsZoneChallengeMet())
		{
			return false;
		}
		if (GameController.GetController() == null || GameController.GetController().LocalPlayer == null)
		{
			return false;
		}
		BehaviorManager component = GameController.GetController().LocalPlayer.GetComponent<BehaviorManager>();
		if (component == null)
		{
			return false;
		}
		BehaviorEmote behaviorEmote = component.getBehavior() as BehaviorEmote;
		if (behaviorEmote == null)
		{
			return false;
		}
		return IsChallengeEmote(behaviorEmote.EmoteID);
	}

	protected override void OnChallengeZoneEnter(GameObject go)
	{
		base.OnChallengeZoneEnter(go);
		AddEmoteListener();
	}

	protected override void OnChallengeZoneExit(GameObject go)
	{
		base.OnChallengeZoneExit(go);
		RemoveEmoteListener();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		RemoveEmoteListener();
	}
}
