using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatEffectBase : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public CombatEffectData combatEffectData;

	protected bool removed;

	public Dictionary<string, CombatEffectBase> storedLocation;

	protected CharacterGlobals charGlobals;

	protected int buffID = -1;

	protected CombatController _sourceCombatController;

	public CombatController getSourceCombatController()
	{
		return _sourceCombatController;
	}

	public virtual void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		_sourceCombatController = sourceCombatController;
		combatEffectData = newCombatEffectData;
		removed = false;
		GameObject gameObject = base.transform.root.gameObject;
		charGlobals = (gameObject.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals);
		if (charGlobals == null)
		{
			CspUtils.DebugLog("CharacterGlobals not found in CombatEffectBase.");
		}
	}

	public void RegisterIcon()
	{
		if (combatEffectData.icon != string.Empty && BrawlerStatManager.Active)
		{
			buffID = BrawlerStatManager.instance.ReportBuffAdded(base.transform.root.gameObject, "brawler_bundle|" + combatEffectData.icon, combatEffectData.toolTip, combatEffectData.alertTexture);
		}
	}

	public virtual void Start()
	{
	}

	public void CountdownCombatEffect()
	{
		if (buffID != -1)
		{
			BrawlerStatManager.instance.ReportBuffCountdown(base.transform.root.gameObject, buffID);
		}
	}

	public void removeCombatEffect(bool doRemoveEffect)
	{
		if (!removed)
		{
			if (doRemoveEffect && !string.IsNullOrEmpty(combatEffectData.effectRemovePrefabName))
			{
				charGlobals.combatController.createEffect(combatEffectData.effectRemovePrefabName, base.transform.parent.gameObject);
			}
			SendMessage("OnRemove", doRemoveEffect, SendMessageOptions.DontRequireReceiver);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	protected void OnRemove(bool doRemoveEffect)
	{
		removed = true;
		ReleaseEffect();
	}

	protected virtual void ReleaseEffect()
	{
		if (storedLocation != null && combatEffectData != null)
		{
			storedLocation.Remove(combatEffectData.combatEffectName);
			storedLocation = null;
		}
		if (buffID != -1)
		{
			BrawlerStatManager.instance.ReportBuffRemoved(base.transform.root.gameObject, buffID);
			buffID = -1;
		}
	}

	protected BehaviorBase changeBehavior(string behaviorName)
	{
		Type type = Type.GetType(behaviorName);
		if (type == null)
		{
			CspUtils.DebugLog("Unknown behavior type " + behaviorName + " in CombatEffectBase.changeBehavior");
			return null;
		}
		return charGlobals.behaviorManager.forceChangeBehavior(type);
	}
}
