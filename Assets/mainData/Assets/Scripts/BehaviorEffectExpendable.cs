using System;
using UnityEngine;

public class BehaviorEffectExpendable : BehaviorBase
{
	protected OnBehaviorDone onBehaviorDone;

	protected bool exited;

	protected EffectSequence sequenceLoop;

	protected string[] sequenceChain;

	protected EffectSequence sequenceLink;

	protected int sequenceLinkIndex = -1;

	protected string[] sequencePersistent;

	protected bool endLoopOnChainComplete;

	public void Initialize(ExpendableDefinition def, OnBehaviorDone onDone)
	{
		string text = null;
		float result = 1f;
		string metadata = string.Empty;
		if (def.Parameters != null)
		{
			foreach (ExpendHandlerParameters parameter in def.Parameters)
			{
				if (parameter.Key == "effect_sequence_loop")
				{
					text = parameter.Value[0];
				}
				else if (parameter.Key == "effect_sequence_chain")
				{
					sequenceChain = parameter.Value;
				}
				else if (parameter.Key == "effect_sequence_persistent")
				{
					sequencePersistent = parameter.Value;
				}
				else if (parameter.Key == "end_loop_on_chain_complete")
				{
					if (!bool.TryParse(parameter.Value[0], out endLoopOnChainComplete))
					{
						CspUtils.DebugLog("end_loop_on_chain_complete value <" + parameter.Value[0] + "> could not be parsed as a Boolean");
					}
				}
				else if (parameter.Key == "scaleplayer")
				{
					if (!float.TryParse(parameter.Value[0], out result))
					{
						CspUtils.DebugLog("scaleplayer value <" + parameter.Value[0] + "> could not be parsed as a float");
					}
				}
				else if (parameter.Key == "metadata")
				{
					metadata = parameter.Value[0];
				}
				else if (parameter.Key == "aoe_emote_id")
				{
					CspUtils.DebugLog("Expendable has an emote AoE " + parameter.Value[0]);
					ForceEmoteMessage forceEmoteMessage = new ForceEmoteMessage(new GoNetId(GoNetId.PLAYER_ID_FLAG, AppShell.Instance.ServerConnection.GetGameUserId()));
					forceEmoteMessage.emoteID = int.Parse(parameter.Value[0]);
					AppShell.Instance.ServerConnection.SendGameMsg(forceEmoteMessage);
				}
			}
		}
		if (result != 1f)
		{
			Transform transform = Utils.FindNodeInChildren(charGlobals.gameObject.transform, "export_node");
			if (transform != null)
			{
				transform.localScale = Vector3.one * result;
				Transform transform2 = charGlobals.gameObject.transform;
				Vector3 localPosition = charGlobals.gameObject.transform.localPosition;
				float x = localPosition.x;
				Vector3 localPosition2 = charGlobals.gameObject.transform.localPosition;
				float y = localPosition2.y + 0.6f;
				Vector3 localPosition3 = charGlobals.gameObject.transform.localPosition;
				transform2.localPosition = new Vector3(x, y, localPosition3.z);
			}
		}
		bool flag = false;
		if (!string.IsNullOrEmpty(text))
		{
			StartSequenceLoop(text);
			flag = true;
		}
		if (sequenceChain != null && sequenceChain.Length > 0)
		{
			StartSequenceChain();
			flag = true;
		}
		if (sequencePersistent != null)
		{
			string[] array = sequencePersistent;
			foreach (string sequenceName in array)
			{
				CreateSequence(sequenceName, null, 600, metadata);
			}
		}
		if (def.CombatEffects.Count > 0)
		{
			foreach (ExpendCombatEffect combatEffect in def.CombatEffects)
			{
				if (combatEffect.name == "BuffPotionSpeedIncrease")
				{
					CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(charGlobals.gameObject);
					component.addSpeedMultiplier(1.5f);
					if (charGlobals.activeSidekick != null)
					{
						component = charGlobals.activeSidekick.motionController;
						component.addSpeedMultiplier(1.5f);
					}
				}
			}
		}
		onBehaviorDone = onDone;
		exited = false;
		if (!flag)
		{
			ExitBehavior();
		}
	}

	public override bool allowUserInput()
	{
		return false;
	}

	public override bool useMotionController()
	{
		return false;
	}

	protected void OnSequenceLoopDone(EffectSequence seq)
	{
		EndSequenceLoop();
	}

	protected void OnSequenceLinkDone(EffectSequence seq)
	{
		NextSequenceInChain();
	}

	protected void StartSequenceLoop(string sequenceName)
	{
		sequenceLoop = CreateSequence(sequenceName, OnSequenceLoopDone, 0, string.Empty);
	}

	protected void StartSequenceChain()
	{
		sequenceLink = null;
		sequenceLinkIndex = -1;
		if (sequenceChain != null && sequenceChain.Length > 0)
		{
			sequenceLink = CreateSequence(sequenceChain[0], OnSequenceLinkDone, 0, string.Empty);
			if (sequenceLink != null)
			{
				sequenceLinkIndex = 0;
				return;
			}
			CspUtils.DebugLog("BehaviorEffectExpendable::StartSequenceChain() - failed to create first sequence <" + sequenceChain[0] + "> in chain");
			EndSequenceChain();
		}
	}

	protected void NextSequenceInChain()
	{
		if (sequenceChain == null)
		{
			return;
		}
		sequenceLinkIndex++;
		if (sequenceLinkIndex >= sequenceChain.Length)
		{
			EndSequenceChain();
			return;
		}
		sequenceLink = CreateSequence(sequenceChain[sequenceLinkIndex], OnSequenceLinkDone, 0, string.Empty);
		if (sequenceLink == null)
		{
			CspUtils.DebugLog("BehaviorEffectExpendable::NextSequenceInChain() - failed to create next sequence <" + sequenceChain[sequenceLinkIndex] + "> in chain");
			EndSequenceChain();
		}
	}

	protected void EndSequenceLoop()
	{
		if (sequenceLoop != null)
		{
			sequenceLoop.StopSequence(false);
		}
		sequenceLoop = null;
		ExitBehavior();
	}

	protected void EndSequenceChain()
	{
		sequenceLink = null;
		sequenceLinkIndex = -1;
		if (endLoopOnChainComplete)
		{
			EndSequenceLoop();
		}
		ExitBehavior();
	}

	private void ExitBehavior()
	{
		if (CanExitBehavior() && !exited)
		{
			if (onBehaviorDone != null)
			{
				onBehaviorDone(owningObject);
				onBehaviorDone = null;
			}
			if (charGlobals.behaviorManager.getBehavior() == this)
			{
				charGlobals.behaviorManager.endBehavior();
			}
			exited = true;
		}
	}

	private bool CanExitBehavior()
	{
		return sequenceLoop == null && sequenceLink == null;
	}

	private EffectSequence CreateSequence(string sequenceName, EffectSequence.OnSequenceDone onDone, int forcedDuration = 0, string metadata = "")
	{
		EffectSequence effectSequence = null;
		if (!string.IsNullOrEmpty(sequenceName) && charGlobals != null && charGlobals.effectsList != null)
		{
			if (sequenceName == "InvisibleWomanFadeController")
			{
				InvisibleWomanFadeController invisibleWomanFadeController = Utils.GetComponent<InvisibleWomanFadeController>(charGlobals.gameObject);
				if (invisibleWomanFadeController == null)
				{
					invisibleWomanFadeController = charGlobals.gameObject.AddComponent<InvisibleWomanFadeController>();
				}
				invisibleWomanFadeController.autoFadeInDelay = 600f;
				invisibleWomanFadeController.Start();
				invisibleWomanFadeController.Fade(false);
			}
			else if (sequenceName == "SimpleColorMaterialController")
			{
				if (metadata == string.Empty)
				{
					CspUtils.DebugLog("BehaviorEffectExpendable - got a call for SimpleColorMaterialController but it had no metadata");
					return null;
				}
				string[] array = metadata.Split(',');
				float r = float.Parse(array[0]);
				float g = float.Parse(array[1]);
				float b = float.Parse(array[2]);
				float a = float.Parse(array[3]);
				Color colorShift = new Color(r, g, b, a);
				Type type = Type.GetType(sequenceName);
				SimpleColorMaterialController simpleColorMaterialController = charGlobals.gameObject.GetComponent(type) as SimpleColorMaterialController;
				if (simpleColorMaterialController == null)
				{
					simpleColorMaterialController = (charGlobals.gameObject.AddComponent(type) as SimpleColorMaterialController);
				}
				simpleColorMaterialController.colorShift = colorShift;
				IComponentTimeInit componentTimeInit = simpleColorMaterialController;
				if (componentTimeInit != null)
				{
					componentTimeInit.SetDelay(0f);
					componentTimeInit.SetDuration(900f);
				}
			}
			else if (sequenceName.IndexOf("Controller") >= 0)
			{
				Type type2 = Type.GetType(sequenceName);
				Component component = charGlobals.gameObject.GetComponent(type2);
				if (component == null)
				{
					component = charGlobals.gameObject.AddComponent(type2);
				}
				IComponentTimeInit componentTimeInit2 = component as IComponentTimeInit;
				if (componentTimeInit2 != null)
				{
					componentTimeInit2.SetDelay(0f);
					componentTimeInit2.SetDuration(600f);
				}
			}
			else
			{
				GameObject gameObject = charGlobals.effectsList.GetEffectSequencePrefabByName(sequenceName) as GameObject;
				if (gameObject != null)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
					effectSequence = (gameObject2.GetComponent(typeof(EffectSequence)) as EffectSequence);
					if (effectSequence != null)
					{
						if (forcedDuration != 0)
						{
							effectSequence.Lifetime = forcedDuration;
							effectSequence.TotalLifetime = forcedDuration;
							if (effectSequence.Particles.Length > 0)
							{
								effectSequence.Particles[0].Lifetime = forcedDuration;
							}
						}
						effectSequence.SetParent(charGlobals.gameObject);
						effectSequence.AssignCreator(charGlobals);
						effectSequence.Initialize(null, onDone, null);
						effectSequence.StartSequence();
					}
				}
				else
				{
					effectSequence = GUIManager.Instance.LoadEffectSequence(sequenceName);
					if (effectSequence != null)
					{
						effectSequence.Lifetime = 900f;
						if (effectSequence.Particles.Length > 0)
						{
							effectSequence.Particles[0].Lifetime = 600f;
						}
						effectSequence.SetParent(charGlobals.gameObject);
						effectSequence.AssignCreator(charGlobals);
						effectSequence.Initialize(null, onDone, null);
						effectSequence.StartSequence();
					}
					else
					{
						CspUtils.DebugLog("failed to load sequence");
					}
				}
			}
		}
		return effectSequence;
	}
}
