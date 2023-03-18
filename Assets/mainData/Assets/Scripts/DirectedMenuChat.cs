using UnityEngine;

public class DirectedMenuChat : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private const float emoteOffset = 0.25f;

	protected const float HEIGHT_ABOVE_HEAD = 0f;

	private GameObject source;

	private GameObject target;

	private MenuChatGroup group;

	private CharacterGlobals charGlobals;

	private AnimClipManager apm = new AnimClipManager();

	private bool isLocal;

	private void Awake()
	{
		isLocal = (GameController.GetController().LocalPlayer == base.gameObject);
	}

	public static void DirectMenuChatEmote(GameObject emotingPlayer, MenuChatGroup group, GameObject targetPlayer)
	{
		if (!(emotingPlayer == null) && group != null)
		{
			DirectedMenuChat directedMenuChat = Utils.GetComponent<DirectedMenuChat>(emotingPlayer);
			if (directedMenuChat == null)
			{
				directedMenuChat = Utils.AddComponent<DirectedMenuChat>(emotingPlayer);
			}
			HairTrafficController component = emotingPlayer.GetComponent<HairTrafficController>();
			if (component != null)
			{
			}
			directedMenuChat.PerformDirectedMenuChatEmote(group, targetPlayer);
		}
	}

	public void PerformDirectedMenuChatEmote(MenuChatGroup group, GameObject targetPlayer)
	{
		if (group == null)
		{
			return;
		}
		this.group = group;
		target = targetPlayer;
		charGlobals = Utils.GetComponent<CharacterGlobals>(base.gameObject);
		if (charGlobals == null || charGlobals.behaviorManager == null || !charGlobals.behaviorManager.allowUserInput())
		{
			return;
		}
		if (isLocal && charGlobals.networkComponent != null)
		{
			charGlobals.networkComponent.QueueNetAction(new NetActionDirectedMenuChat(base.gameObject, target, group));
		}
		if (targetPlayer != null)
		{
			BehaviorTurnTo behaviorTurnTo = charGlobals.behaviorManager.requestChangeBehavior<BehaviorTurnTo>(true);
			if (behaviorTurnTo != null)
			{
				behaviorTurnTo.Initialize(target.transform.position, OnEmoteChatTurnToFinished);
			}
		}
		else
		{
			OnEmoteChatTurnToFinished(base.gameObject);
		}
	}

	private void OnEmoteChatTurnToFinished(GameObject turnedPlayer)
	{
		AppShell.Instance.EventMgr.Fire(this, new GameWorldMenuChatMessage(turnedPlayer, group));
		if (group.EmoteId == null)
		{
			return;
		}
		BehaviorEmote behaviorEmote = charGlobals.behaviorManager.requestChangeBehavior<BehaviorEmote>(true);
		if (behaviorEmote != null)
		{
			EmotesDefinition.EmoteDefinition emoteByCommand = EmotesDefinition.Instance.GetEmoteByCommand(group.EmoteId);
			if (emoteByCommand != null)
			{
				behaviorEmote.Initialize(emoteByCommand.id, true);
			}
		}
	}

	public void HideSpeechBubble()
	{
	}

	public void Update()
	{
		apm.Update(Time.deltaTime);
	}
}
