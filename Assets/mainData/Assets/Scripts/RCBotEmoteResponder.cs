using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RCBotEmoteResponder : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public delegate void EmoteDelegate(sbyte emoteId);

	private const float emoteCooldown = 1f;

	public string emote = "pemote1";

	private float lastEmoteTime = -1f;

	[CompilerGenerated]
	private Dictionary<sbyte, EmoteDelegate> _003CEmoteDelegates_003Ek__BackingField;

	public Dictionary<sbyte, EmoteDelegate> EmoteDelegates
	{
		[CompilerGenerated]
		get
		{
			return _003CEmoteDelegates_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CEmoteDelegates_003Ek__BackingField = value;
		}
	}

	public RCBotEmoteResponder()
	{
		EmoteDelegates = new Dictionary<sbyte, EmoteDelegate>();
	}

	private void OnEnable()
	{
		if (AppShell.Instance != null)
		{
			AppShell.Instance.EventMgr.AddListener<EmoteMessage>(OnEmote);
		}
	}

	private void OnDisable()
	{
		if (AppShell.Instance != null)
		{
			AppShell.Instance.EventMgr.RemoveListener<EmoteMessage>(OnEmote);
		}
	}

	private void OnEmote(EmoteMessage e)
	{
		if (!IsOwner(e.sender))
		{
			return;
		}
		EmoteDelegate value;
		if (EmoteDelegates.TryGetValue(e.emote, out value))
		{
			value(e.emote);
		}
		else if (Time.time >= lastEmoteTime + 1f)
		{
			BehaviorManager component = base.gameObject.GetComponent<BehaviorManager>();
			BehaviorEmote behaviorEmote = component.requestChangeBehavior<BehaviorEmote>(false);
			if (behaviorEmote != null)
			{
				behaviorEmote.Initialize(EmotesDefinition.Instance.GetEmoteByCommand(emote).id);
				lastEmoteTime = Time.time;
			}
		}
	}

	private bool IsOwner(GameObject potentialOwner)
	{
		return potentialOwner != null && potentialOwner.GetComponent<NetworkComponent>().IsOwner();
	}
}
