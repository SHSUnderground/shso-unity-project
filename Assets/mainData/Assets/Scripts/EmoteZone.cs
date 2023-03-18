using System.Collections;
using UnityEngine;

public class EmoteZone : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private static string[] kDefaultEmotes = new string[1]
	{
		"dance"
	};

	public EmoteList emoteList = new EmoteList(kDefaultEmotes, EmoteList.SelectionType.Sequential);

	public FRange initialWaitTime = new FRange(2f, 3f);

	public FRange subsequentWaitTime = new FRange(2f, 3f);

	public FRange emoteTime = new FRange(4f, 5f);

	public bool infiniteEmoteTime;

	public bool disableAFK = true;

	private GameObject player;

	private BehaviorMovement behavior;

	public void OnTriggerEnter(Collider other)
	{
		if (!(GameController.GetController().LocalPlayer == other.gameObject))
		{
			return;
		}
		player = other.gameObject;
		AppShell.Instance.EventMgr.AddListener<LocalPlayerChangedMessage>(OnLocalPlayerChanged);
		StartCoroutine(AssignIdleEmote(Utils.GetComponent<CharacterGlobals>(player)));
		if (disableAFK)
		{
			AFKWatcher component = Utils.GetComponent<AFKWatcher>(player);
			if (component != null)
			{
				component.Disabled = true;
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (player != null && other.gameObject == player)
		{
			AppShell.Instance.EventMgr.RemoveListener<LocalPlayerChangedMessage>(OnLocalPlayerChanged);
			StopAllCoroutines();
			if (behavior != null)
			{
				behavior.idleEmote = null;
				behavior = null;
			}
			AFKWatcher component = Utils.GetComponent<AFKWatcher>(player);
			if (component != null)
			{
				component.Disabled = false;
			}
		}
	}

	private void OnLocalPlayerChanged(LocalPlayerChangedMessage e)
	{
		AppShell.Instance.EventMgr.RemoveListener<LocalPlayerChangedMessage>(OnLocalPlayerChanged);
		StopAllCoroutines();
		behavior = null;
	}

	private IEnumerator AssignIdleEmote(CharacterGlobals character)
	{
		if (!(character != null))
		{
			yield break;
		}
		while (behavior == null)
		{
			BehaviorBase currentBehavior = character.behaviorManager.getBehavior();
			if (currentBehavior is BehaviorMovement && !(currentBehavior is BehaviorApproach))
			{
				behavior = (currentBehavior as BehaviorMovement);
				behavior.idleEmote = new IdleEmote(emoteList, (!infiniteEmoteTime) ? emoteTime : null, initialWaitTime, subsequentWaitTime);
			}
			yield return 0;
		}
	}
}
