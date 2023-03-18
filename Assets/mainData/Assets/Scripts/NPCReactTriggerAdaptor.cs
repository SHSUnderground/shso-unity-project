using System.Collections;
using UnityEngine;

public class NPCReactTriggerAdaptor : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private const float interactDistance = 2f;

	private EnableProxy glowEnabler;

	public void Start()
	{
		StartCoroutine(DelayedStart());
	}

	protected IEnumerator DelayedStart()
	{
		yield return 0;
		InteractiveObject io = Utils.GetComponent<InteractiveObject>(base.gameObject, Utils.SearchParents);
		if (io != null)
		{
			GameObject highlightRoot = io.GetRoot(InteractiveObject.StateIdx.Highlight);
			if (highlightRoot != null)
			{
				glowEnabler = Utils.GetComponent<EnableProxy>(highlightRoot);
			}
		}
	}

	public void Triggered()
	{
		AIControllerNPC component = Utils.GetComponent<AIControllerNPC>(base.gameObject, Utils.SearchParents);
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		component.Wait();
		MoveTowardsNPC(component, Utils.GetComponent<BehaviorManager>(localPlayer));
	}

	protected void MoveTowardsNPC(AIControllerNPC npc, BehaviorManager playerBehaviorMan)
	{
		Vector3 vector = npc.transform.position - playerBehaviorMan.transform.position;
		Vector3 vector2 = vector;
		vector2.y = 0f;
		if (vector2.sqrMagnitude < 4f)
		{
			BehaviorTurnTo behaviorTurnTo = playerBehaviorMan.requestChangeBehavior<BehaviorTurnTo>(true);
			if (behaviorTurnTo != null)
			{
				CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(playerBehaviorMan.gameObject);
				component.stopGently();
				behaviorTurnTo.Initialize(npc.transform.position, AimedAtNPC);
			}
		}
		else
		{
			BehaviorApproach behaviorApproach = playerBehaviorMan.requestChangeBehavior(typeof(BehaviorApproach), true) as BehaviorApproach;
			if (behaviorApproach != null)
			{
				Quaternion rotation = Quaternion.LookRotation(vector);
				Vector3 newTargetPosition = npc.transform.position - vector.normalized * 2f * 0.9f;
				behaviorApproach.Initialize(newTargetPosition, rotation, true, AimedAtNPC, delegate
				{
					npc.StopWaiting();
				}, 0.15f, 2f, true, true);
			}
		}
	}

	protected void AimedAtNPC(GameObject player)
	{
		SetGlowEnabled(false);
		AIControllerNPC component = Utils.GetComponent<AIControllerNPC>(base.gameObject, Utils.SearchParents);
		component.StopWaiting();
		component.StartCoroutine(WaitForReaction(component));
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		BehaviorManager component2 = Utils.GetComponent<BehaviorManager>(localPlayer);
		BehaviorEmote behaviorEmote = component2.requestChangeBehavior(typeof(BehaviorEmote), false) as BehaviorEmote;
		if (behaviorEmote != null)
		{
			sbyte id = EmotesDefinition.Instance.GetEmoteByCommand("greet").id;
			behaviorEmote.Initialize(id, true);
			bool flag = false;
			if (component.Gender == AIControllerNPC.NPCGender.Female)
			{
				ResolvedVOAction resolvedVOAction = VOManager.Instance.PlayVO("greet_npc_female", localPlayer);
				flag = (resolvedVOAction != null && resolvedVOAction.IsResolved);
			}
			else if (component.Gender == AIControllerNPC.NPCGender.Male)
			{
				ResolvedVOAction resolvedVOAction2 = VOManager.Instance.PlayVO("greet_npc_male", localPlayer);
				flag = (resolvedVOAction2 != null && resolvedVOAction2.IsResolved);
			}
			if (!flag)
			{
				VOManager.Instance.PlayVO("greet_npc", localPlayer);
			}
		}
	}

	private IEnumerator WaitForReaction(AIControllerNPC npc)
	{
		while (!(npc.CurrentCommand is NPCReactCommand))
		{
			yield return 0;
		}
		while (npc.CurrentCommand is NPCReactCommand)
		{
			yield return 0;
		}
		OnNPCReactionFinished();
	}

	private void OnNPCReactionFinished()
	{
		SetGlowEnabled(true);
	}

	protected void SetGlowEnabled(bool enable)
	{
		if (glowEnabler != null && glowEnabler.gameObject.active)
		{
			glowEnabler.ToggleObjects(enable);
		}
		else if (glowEnabler != null && !glowEnabler.gameObject.active)
		{
			glowEnabler.ToggleObjects(false);
		}
	}
}
