using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorEmote : BehaviorBase
{
	protected sbyte emoteId = -1;

	protected bool localOnly = true;

	protected EffectSequence effect;

	protected bool done;

	protected bool looping;

	protected float emoteBroadcastRange = 15f;

	protected bool allowInput = true;

	protected EmotesDefinition.EmoteDefinition emoteDef;

	protected EffectSequenceList list;

	protected GameObject spawnedEffect;

	protected float timeout = -1f;

	protected OnBehaviorDone doneCallback;

	protected EffectSequence.OnSequenceEvent onEventCallback;

	protected EffectSequence emoteSequence;

	protected Dictionary<SkinnedMeshRenderer, Material[]> originialMaterials = new Dictionary<SkinnedMeshRenderer, Material[]>();

	private bool _playerBillboardVisible = true;

	public sbyte EmoteID
	{
		get
		{
			return emoteId;
		}
	}

	public override bool Paused
	{
		get
		{
			return base.Paused;
		}
		set
		{
			base.Paused = value;
			if (effect != null)
			{
				effect.Paused = value;
			}
		}
	}

	public bool Looping
	{
		get
		{
			return looping;
		}
	}

	protected bool PlayerBillboardVisible
	{
		set
		{
			if (value != _playerBillboardVisible)
			{
				_playerBillboardVisible = value;
				HairTrafficController component = owningObject.GetComponent<HairTrafficController>();
				if (component != null)
				{
					component.ToggleBillboard(value);
				}
			}
		}
	}

	public bool Initialize(sbyte emoteId, OnBehaviorDone onEmoteDone)
	{
		doneCallback = onEmoteDone;
		return Initialize(emoteId);
	}

	public bool Initialize(sbyte emoteId)
	{
		return Initialize(emoteId, false);
	}

	public bool Initialize(sbyte emoteId, bool localOnly)
	{
		effect = null;
		list = (owningObject.GetComponent(typeof(EffectSequenceList)) as EffectSequenceList);
		if (list == null)
		{
			CspUtils.DebugLog("BehaviorEmote: no EffectSequenceList for character " + owningObject.name);
			return false;
		}
		emoteDef = EmotesDefinition.Instance.GetEmoteById(emoteId);
		if (emoteDef == null)
		{
			CspUtils.DebugLog("BehaviorEmote: Could not find emote for emoteId " + emoteId.ToString());
			return false;
		}
		this.emoteId = emoteId;
		this.localOnly = localOnly;
		spawnedEffect = null;
		if (!startSequence())
		{
			return false;
		}
		localBroadcastEmote(emoteId);
		looping = (emoteDef.isLooping && CardGameController.Instance == null);
		charGlobals.motionController.stopGently(false);
		if (AppShell.Instance != null && AppShell.Instance.EventMgr != null)
		{
			AppShell.Instance.EventMgr.Fire(owningObject, new EmoteSequenceMessage(owningObject, emoteId));
		}
		return true;
	}

	public bool Initialize(sbyte emoteId, bool localOnly, float timeout)
	{
		this.timeout = timeout;
		return Initialize(emoteId, localOnly);
	}

	public bool Initialize(sbyte emoteId, bool localOnly, bool inputEnabled, float timeout)
	{
		this.timeout = timeout;
		allowInput = inputEnabled;
		return Initialize(emoteId, localOnly);
	}

	public bool Initialize(sbyte emoteId, bool localOnly, bool inputEnabled)
	{
		allowInput = inputEnabled;
		return Initialize(emoteId, localOnly);
	}

	public bool Initialize(sbyte emoteId, bool inputEnabled, OnBehaviorDone onEmoteDone)
	{
		allowInput = inputEnabled;
		return Initialize(emoteId, onEmoteDone);
	}

	public bool Initialize(sbyte emoteId, bool localOnly, bool inputEnabled, OnBehaviorDone onEmoteDone)
	{
		allowInput = inputEnabled;
		doneCallback = onEmoteDone;
		return Initialize(emoteId, localOnly);
	}

	public bool Initialize(sbyte emoteId, bool localOnly, bool inputEnabled, EffectSequence.OnSequenceEvent onEvent)
	{
		onEventCallback = onEvent;
		return Initialize(emoteId, localOnly, inputEnabled);
	}

	protected bool startSequence()
	{
		effect = null;
		if (emoteDef.isLogicalSequence)
		{
			if (BrawlerController.Instance != null && !BrawlerController.Instance.isTestScene)
			{
				CspUtils.DebugLog("Blocked Power emote in Mission.");
				return false;
			}
			if (charGlobals.spawnData != null && (BrawlerController.Instance == null || !BrawlerController.Instance.isTestScene) && Utils.IsLocalPlayer(charGlobals) && AppShell.Instance.Profile != null)
			{
				HeroPersisted heroPersisted = AppShell.Instance.Profile.AvailableCostumes[owningObject.name];
				if (heroPersisted == null)
				{
					CspUtils.DebugLog("Oi! No hero profile for <" + owningObject.name + ">!");
					return false;
				}
				int num = heroPersisted.Level;
				PlayerCombatController playerCombatController = owningObject.GetComponent(typeof(PlayerCombatController)) as PlayerCombatController;
				if (playerCombatController != null)
				{
					num = Math.Max(num, playerCombatController.characterLevel);
				}
				if (emoteDef.id == 2 && num < 5)
				{
					CspUtils.DebugLog("Power emote 2 will be available at level 5.");
					return false;
				}
				if (emoteDef.id == 3 && num < 10)
				{
					CspUtils.DebugLog("Power emote 3 will be available at level MAX (10?)");
					return false;
				}
			}
			emoteSequence = list.GetLogicalEffectSequence(emoteDef.sequenceName);
			if (emoteSequence == null)
			{
				CspUtils.DebugLog("BehaviorEmote: Failed to find emote sequence for " + emoteDef.sequenceName);
			}
		}
		else
		{
			GameObject gameObject = list.GetEffectSequencePrefabByName(emoteDef.sequenceName) as GameObject;
			if (gameObject != null)
			{
				spawnedEffect = (UnityEngine.Object.Instantiate(gameObject) as GameObject);
				emoteSequence = (spawnedEffect.GetComponent(typeof(EffectSequence)) as EffectSequence);
				emoteSequence.SetParent(charGlobals.gameObject);
			}
		}
		if (emoteSequence == null)
		{
			return false;
		}
		effect = emoteSequence;
		emoteSequence.AssignCreator(charGlobals);
		emoteSequence.Initialize(null, OnEffectDone, OnEffectEvent);
		if (emoteSequence.AutoStart)
		{
			emoteSequence.AutoStart = false;
			CspUtils.DebugLog("Emote sequence <" + emoteDef.sequenceName + "> attached to <" + owningObject.name + "> is set to play automatically; suppressing AutoStart.");
		}
		emoteSequence.StartSequence();
		PlayerBillboardVisible = false;
		if (!localOnly && networkComponent != null && networkComponent.IsOwner())
		{
			GameObject targetedPlayer = null;
			SocialSpaceController socialSpaceController = GameController.GetController() as SocialSpaceController;
			if (socialSpaceController != null)
			{
				targetedPlayer = socialSpaceController.SelectedPlayer;
			}
			NetActionEmote action = new NetActionEmote(owningObject, emoteId, targetedPlayer);
			networkComponent.QueueNetAction(action);
			charGlobals.motionController.positionSent(true);
		}
		return true;
	}

	private void localBroadcastEmote(sbyte emoteId)
	{
		EmoteListener[] array = Utils.FindObjectsOfType<EmoteListener>();
		EmoteListener[] array2 = array;
		foreach (EmoteListener emoteListener in array2)
		{
			if ((emoteListener.transform.position - charGlobals.gameObject.transform.position).magnitude < emoteBroadcastRange)
			{
				emoteListener.OnEmoteBroadcast(emoteId, charGlobals.gameObject);
			}
		}
	}

	public override void behaviorBegin()
	{
		base.behaviorBegin();
	}

	public override void behaviorUpdate()
	{
		if (Paused)
		{
			return;
		}
		base.behaviorUpdate();
		if (done || (timeout > 0f && elapsedTime >= timeout))
		{
			charGlobals.behaviorManager.endBehavior();
			if (doneCallback != null)
			{
				doneCallback(owningObject);
			}
		}
	}

	public override void behaviorLateUpdate()
	{
		base.behaviorLateUpdate();
		charGlobals.motionController.performRootMotion();
	}

	public override void behaviorEnd()
	{
		if (!done && spawnedEffect != null)
		{
			UnityEngine.Object.Destroy(spawnedEffect);
		}
		done = true;
		if (effect != null)
		{
			foreach (KeyValuePair<SkinnedMeshRenderer, Material[]> originialMaterial in originialMaterials)
			{
				originialMaterial.Key.materials = originialMaterial.Value;
			}
			originialMaterials.Clear();
			effect.Cancel();
		}
		PlayerBillboardVisible = true;
	}

	public override bool allowUserInput()
	{
		return allowInput;
	}

	public override void destinationChanged()
	{
		base.destinationChanged();
		if (networkComponent != null && networkComponent.IsOwner())
		{
			NetActionCancel action = new NetActionCancel();
			networkComponent.QueueNetAction(action);
			charGlobals.behaviorManager.endBehavior();
		}
	}

	public override void motionJumped()
	{
		base.motionJumped();
		charGlobals.behaviorManager.endBehavior();
	}

	protected void OnEffectDone(EffectSequence seq)
	{
		if (!done && looping)
		{
			startSequence();
		}
		else
		{
			done = true;
		}
	}

	protected void OnEffectEvent(EffectSequence seq, EventEffect effect)
	{
		switch (effect.EventName)
		{
		case "behavior end":
			done = true;
			break;
		case "power emote aoe":
		{
			Collider[] array2 = Physics.OverlapSphere(owningObject.transform.position, effect.EventValue);
			if (array2.Length <= 0)
			{
				break;
			}
			Dictionary<InteractiveObject, bool> dictionary = new Dictionary<InteractiveObject, bool>();
			Collider[] array3 = array2;
			foreach (Collider collider in array3)
			{
				InteractiveObject interactiveObject = GetInteractiveObject(collider.gameObject);
				if (interactiveObject != null)
				{
					if (!dictionary.ContainsKey(interactiveObject))
					{
						dictionary.Add(interactiveObject, true);
					}
				}
				else
				{
					collider.gameObject.SendMessage("OnPowerEmote", owningObject, SendMessageOptions.DontRequireReceiver);
				}
				InteractableObject component = Utils.GetComponent<InteractableObject>(collider);
				if (component != null)
				{
					component.OnPowerEmote(owningObject);
				}
			}
			foreach (InteractiveObject key in dictionary.Keys)
			{
				key.OnPowerEmote(owningObject);
			}
			break;
		}
		case "material swap":
		{
			Transform transform = Utils.FindNodeInChildren(owningObject.transform, "export_node");
			Component[] componentsInChildren = transform.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			Component[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)array[i];
				if (skinnedMeshRenderer.materials.Length > (int)effect.EventValue)
				{
					if (!originialMaterials.ContainsKey(skinnedMeshRenderer))
					{
						originialMaterials[skinnedMeshRenderer] = skinnedMeshRenderer.materials;
					}
					Material[] materials = skinnedMeshRenderer.materials;
					materials[(int)effect.EventValue] = effect.material;
					skinnedMeshRenderer.materials = materials;
				}
			}
			break;
		}
		}
		if (onEventCallback != null)
		{
			onEventCallback(seq, effect);
		}
	}

	protected InteractiveObject GetInteractiveObject(GameObject o)
	{
		while (o != null)
		{
			InteractiveObject component = Utils.GetComponent<InteractiveObject>(o);
			if (component != null)
			{
				return component;
			}
			if (o.transform.parent == null)
			{
				break;
			}
			o = o.transform.parent.gameObject;
		}
		return null;
	}

	public override float GetBehaviorDuration()
	{
		if (emoteSequence == null)
		{
			return 0f;
		}
		return emoteSequence.Lifetime - elapsedTime;
	}
}
