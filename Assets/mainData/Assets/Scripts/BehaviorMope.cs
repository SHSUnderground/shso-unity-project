using UnityEngine;

public class BehaviorMope : BehaviorMovement
{
	protected float waitTime;

	protected float lastEmoteAttempt;

	protected OnBehaviorDone doneCallback;

	protected bool donePlayingEmote;

	protected EffectSequenceList list;

	protected GameObject spawnedEffect;

	protected AIControllerHQ aiController;

	public void Initialize(OnBehaviorDone onDone, float idleTime)
	{
		doneCallback = onDone;
		waitTime = idleTime;
		donePlayingEmote = true;
		charGlobals.motionController.setDestination(owningObject.transform.position);
		aiController = Utils.GetComponent<AIControllerHQ>(owningObject, Utils.SearchChildren);
	}

	private void DoEmote()
	{
		EmotesDefinition.EmoteDefinition emoteByCommand = EmotesDefinition.Instance.GetEmoteByCommand("sad");
		if (emoteByCommand != null)
		{
			startSequence(emoteByCommand);
		}
	}

	public override void behaviorUpdate()
	{
		if (!Paused)
		{
			base.behaviorUpdate();
			if ((!aiController.IsAngry || elapsedTime >= waitTime) && doneCallback != null)
			{
				doneCallback(owningObject);
			}
		}
	}

	protected bool startSequence(EmotesDefinition.EmoteDefinition emoteDef)
	{
		EffectSequence effectSequence = null;
		EffectSequenceList effectSequenceList = owningObject.GetComponent(typeof(EffectSequenceList)) as EffectSequenceList;
		if (emoteDef.isLogicalSequence)
		{
			effectSequence = effectSequenceList.GetLogicalEffectSequence(emoteDef.sequenceName);
		}
		else
		{
			GameObject gameObject = effectSequenceList.GetEffectSequencePrefabByName(emoteDef.sequenceName) as GameObject;
			if (gameObject != null)
			{
				spawnedEffect = (Object.Instantiate(gameObject) as GameObject);
				effectSequence = (spawnedEffect.GetComponent(typeof(EffectSequence)) as EffectSequence);
				effectSequence.SetParent(charGlobals.gameObject);
			}
		}
		if (effectSequence == null)
		{
			return false;
		}
		effectSequence.AssignCreator(charGlobals);
		effectSequence.Initialize(null, OnEffectDone, null);
		effectSequence.StartSequence();
		donePlayingEmote = false;
		return true;
	}

	public override void behaviorEnd()
	{
		if (!donePlayingEmote && spawnedEffect != null)
		{
			Object.Destroy(spawnedEffect);
		}
		base.behaviorEnd();
	}

	protected void OnEffectDone(EffectSequence seq)
	{
		donePlayingEmote = true;
	}
}
