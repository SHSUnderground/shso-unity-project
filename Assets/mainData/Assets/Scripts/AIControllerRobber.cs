using UnityEngine;

public class AIControllerRobber : AIControllerNPC
{
	public enum RobberState
	{
		Inactive,
		Fleeing,
		WaitingForPlayer,
		Despawning
	}

	public static readonly float kDefaultRobberMoveSpeed = 10f;

	public NPCPath path;

	public ShsAudioSource startSFX;

	public ShsAudioSource caughtSFX;

	public EffectSequence despawnEffect;

	public bool stareAtPlayer = true;

	public bool stareAtCameraWhileInactive = true;

	private NPCPathNode startNode;

	public float robberMoveSpeed = kDefaultRobberMoveSpeed;

	public static string kActivityName;

	public string caughtAnimation = "movement_caught";

	public string gotAwayAnimation = "movement_taunt";

	public string caughtEffectName = "emote_caught_sequence";

	public string gotAwayEffectName = "emote_taunt_sequence";

	public bool consideredHuman;

	protected CharacterGlobals charGlobals;

	private EffectSequence despawnEffectInstance;

	private EffectSequence waitEffectInstance;

	private string waitEffectName;

	private RobberState state;

	private SHSRobberRallyActivity parentActivity;

	public RobberState State
	{
		get
		{
			return state;
		}
	}

	public override bool DespawnOnExtendedIdle
	{
		get
		{
			return false;
		}
	}

	public override void Start()
	{
		if (base.AssignedPath == null)
		{
			base.AssignedPath = path;
		}
		commandManager.mustHaveCommands = false;
		charGlobals = base.gameObject.GetComponent<CharacterGlobals>();
	}

	public override void OnInteract(GameObject player)
	{
	}

	public override void OnEmoteBroadcast(sbyte emoteID, GameObject broadcaster)
	{
	}

	public new void OnStartled(object value)
	{
	}

	public override bool ConfigureCommandsFromNode(NPCPathNode node, bool setNodeAsCurrent)
	{
		if (setNodeAsCurrent)
		{
			base.CurrentNode = node;
		}
		ExecuteNodeCommands(node);
		RobberPathNode robberPathNode = node as RobberPathNode;
		if (IsEndOfPath(node) || (robberPathNode != null && robberPathNode.robberWaitsForPlayer))
		{
			WaitForPlayer(robberPathNode);
		}
		else
		{
			GoToNextNode();
		}
		return true;
	}

	private void ExecuteNodeCommands(NPCPathNode node)
	{
		foreach (NPCCommandHint command2 in node.commands)
		{
			NPCCommandBase command = NPCCommandBase.CreateCommand(command2);
			commandManager.AddCommand(command);
		}
	}

	public void BeginActivity(SHSRobberRallyActivity parentActivity)
	{
		if (state != 0)
		{
			return;
		}
		this.parentActivity = parentActivity;
		if (parentActivity == null)
		{
			CspUtils.DebugLog("Invalid parent activity.  How the heck did you do that?");
			return;
		}
		if (base.AssignedPath == null)
		{
			CspUtils.DebugLog("No path associated with Robber");
			return;
		}
		base.CurrentNode = base.AssignedPath.GetStartNode();
		startNode = base.CurrentNode;
		if (startSFX != null)
		{
			ShsAudioSource.PlayAutoSound(startSFX.gameObject, base.transform);
		}
		GoToNode(base.CurrentNode);
	}

	public float OnGotAway()
	{
		charGlobals.motionController.setNewFacing(GameController.GetController().LocalPlayer.transform.position - base.transform.position);
		PlayDespawnEffect();
		state = RobberState.Despawning;
		return PlayGotAwayEffects();
	}

	protected float PlayGotAwayEffects()
	{
		EffectSequence effect;
		if (!string.IsNullOrEmpty(gotAwayEffectName) && charGlobals.effectsList.TryOneShot(gotAwayEffectName, charGlobals.gameObject, out effect))
		{
			return effect.Lifetime;
		}
		base.animation.Play(gotAwayAnimation);
		return base.animation.GetClip(gotAwayAnimation).length;
	}

	public float OnCaught()
	{
		charGlobals.motionController.setNewFacing(GameController.GetController().LocalPlayer.transform.position - base.transform.position);
		PlayDespawnEffect();
		state = RobberState.Despawning;
		stareAtPlayer = false;
		return PlayCaughtEffects();
	}

	protected float PlayCaughtEffects()
	{
		if (caughtSFX != null)
		{
			ShsAudioSource.PlayAutoSound(caughtSFX.gameObject);
		}
		EffectSequence effect;
		if (!string.IsNullOrEmpty(caughtEffectName) && charGlobals.effectsList.TryOneShot(caughtEffectName, charGlobals.gameObject, out effect))
		{
			return effect.Lifetime;
		}
		base.animation.Play(caughtAnimation);
		return base.animation.GetClip(caughtAnimation).length;
	}

	public void Despawn()
	{
		if (despawnEffectInstance != null)
		{
			Object.Destroy(despawnEffectInstance.gameObject);
			despawnEffectInstance = null;
		}
		NPCDerezCommand nPCDerezCommand = new NPCDerezCommand();
		nPCDerezCommand.fadeTime = 0f;
		nPCDerezCommand.respawn = false;
		commandManager.AddCommand(nPCDerezCommand);
	}

	protected void PlayDespawnEffect()
	{
		DestroyWaitSequence();
		if (despawnEffect != null)
		{
			despawnEffectInstance = (Object.Instantiate(despawnEffect) as EffectSequence);
			if (despawnEffectInstance != null)
			{
				despawnEffectInstance.Initialize(base.gameObject, null, null);
				despawnEffectInstance.StartSequence();
			}
		}
	}

	private void WaitForPlayer(RobberPathNode robberNode)
	{
		state = RobberState.WaitingForPlayer;
		if (!string.IsNullOrEmpty(robberNode.overrideAnimationName))
		{
			charGlobals.behaviorManager.OverrideAnimation("movement_idle", robberNode.overrideAnimationName);
		}
		PlayWaitSequence(robberNode);
		SphereCollider sphereCollider = Utils.AddComponent<SphereCollider>(base.gameObject);
		if (robberNode.overrideAlertRadius > 0f)
		{
			sphereCollider.radius = robberNode.overrideAlertRadius;
		}
		else
		{
			sphereCollider.radius = RobberPathNode.kDefaultAlertRadius;
		}
		sphereCollider.isTrigger = true;
	}

	private void PlayWaitSequence(RobberPathNode robberNode)
	{
		if (!string.IsNullOrEmpty(robberNode.effectSequenceName))
		{
			charGlobals.behaviorManager.requestChangeBehavior<BehaviorWait>(false);
			waitEffectName = robberNode.effectSequenceName;
			waitEffectInstance = charGlobals.effectsList.PlaySequence(waitEffectName, OnWaitSequenceFinished);
		}
	}

	private void OnWaitSequenceFinished(EffectSequence effect)
	{
		Object.Destroy(effect.gameObject);
		waitEffectInstance = charGlobals.effectsList.PlaySequence(waitEffectName, OnWaitSequenceFinished);
	}

	private void DestroyWaitSequence()
	{
		if (waitEffectInstance != null)
		{
			Object.Destroy(waitEffectInstance.gameObject);
			if (charGlobals.behaviorManager.getBehavior() is BehaviorWait)
			{
				charGlobals.behaviorManager.endBehavior();
			}
		}
	}

	public void Update()
	{
		if (state == RobberState.WaitingForPlayer && stareAtPlayer)
		{
			charGlobals.motionController.setNewFacing(GameController.GetController().LocalPlayer.transform.position - base.transform.position);
		}
		else if (state == RobberState.Inactive && stareAtCameraWhileInactive)
		{
			charGlobals.motionController.setNewFacing(Camera.main.transform.position - base.transform.position);
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (state == RobberState.WaitingForPlayer && Utils.IsLocalPlayer(other.gameObject))
		{
			DestroyWaitSequence();
			Object.Destroy(Utils.GetComponent<SphereCollider>(base.gameObject));
			GoToNextNode();
		}
	}

	private void GoToNextNode()
	{
		if (IsEndOfPath(base.CurrentNode))
		{
			parentActivity.Win();
		}
		else
		{
			GoToNode(base.CurrentNode.nextNodes[Random.Range(0, base.CurrentNode.nextNodes.Length)]);
		}
	}

	private void GoToNode(NPCPathNode node)
	{
		RobberMoveToNodeCommand robberMoveToNodeCommand = new RobberMoveToNodeCommand();
		robberMoveToNodeCommand.targetNode = node;
		robberMoveToNodeCommand.run = true;
		robberMoveToNodeCommand.moveSpeed = robberMoveSpeed;
		robberMoveToNodeCommand.ParentActivity = parentActivity;
		if (node != base.CurrentNode)
		{
			robberMoveToNodeCommand.NodeToLeave = base.CurrentNode;
		}
		commandManager.AddCommand(robberMoveToNodeCommand);
		state = RobberState.Fleeing;
	}

	private bool IsEndOfPath(NPCPathNode node)
	{
		bool result = node.nextNodes.Length == 0;
		NPCPathNode[] nextNodes = node.nextNodes;
		foreach (NPCPathNode x in nextNodes)
		{
			if (x == startNode)
			{
				result = true;
				break;
			}
		}
		return result;
	}
}
