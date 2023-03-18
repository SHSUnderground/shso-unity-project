using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("VO/VO Object")]
public class VOObject : MonoBehaviour, IGeneralEffect
{
	public enum NetUsage
	{
		UseActionDefault,
		NotNetworked,
		Networked
	}

	public string action;

	public GameObject emitter;

	public bool playOnStart;

	public bool stopOnDestroy = true;

	public bool allowMultipleInstances;

	public bool destroyWhenFinished;

	public bool warnIfUnresolved = true;

	public VOInputComponent[] inputs = new VOInputComponent[0];

	public NetUsage netUsage;

	public bool useCustomRouting;

	public VORoutingInfo customRouting = new VORoutingInfo();

	private LinkedList<ResolvedVOAction> instances = new LinkedList<ResolvedVOAction>();

	private bool hasPlayedOnStart;

	[CompilerGenerated]
	private static bool _003CAlwaysWarnIfUnresolved_003Ek__BackingField;

	public static bool AlwaysWarnIfUnresolved
	{
		[CompilerGenerated]
		get
		{
			return _003CAlwaysWarnIfUnresolved_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CAlwaysWarnIfUnresolved_003Ek__BackingField = value;
		}
	}

	public float ChanceToPlay
	{
		get
		{
			VOAction value;
			if (Singleton<VOActionDataManager>.instance.VOActions.TryGetValue(action, out value))
			{
				return value.SequenceProbability;
			}
			return -1f;
		}
	}

	public void Start()
	{
		if (playOnStart && !hasPlayedOnStart)
		{
			hasPlayedOnStart = true;
			PlayVO();
		}
	}

	public void OnCreatedAsGeneralEffect(EffectSequence sequence)
	{
		if (!hasPlayedOnStart)
		{
			hasPlayedOnStart = true;
			PlayVO();
		}
	}

	public void OnDestroy()
	{
		if (stopOnDestroy)
		{
			StopVO();
		}
	}

	public void PlayVO()
	{
		if (!allowMultipleInstances && instances.Count > 0)
		{
			return;
		}
		if (emitter == null)
		{
			emitter = GetEmitter();
		}
		if (!string.IsNullOrEmpty(action))
		{
			ResolvedVOAction vO = VOManager.Instance.GetVO(action, emitter, inputs);
			if (vO == null || !vO.IsResolved)
			{
				if (warnIfUnresolved || AlwaysWarnIfUnresolved)
				{
					CspUtils.DebugLog(string.Format("Did not resolve VO Object <{0}> with action <{1}>", base.name, action));
				}
				return;
			}
			vO.OnFinished = (ResolvedVOAction.OnVOFinished)Delegate.Combine(vO.OnFinished, new ResolvedVOAction.OnVOFinished(OnVOFinished));
			instances.AddLast(vO);
			if (netUsage != 0)
			{
				vO.IsNetworked = (netUsage == NetUsage.Networked);
			}
			if (useCustomRouting)
			{
				vO.CustomRouting = customRouting;
			}
			VOManager.Instance.PlayResolvedVO(vO);
		}
		else
		{
			CspUtils.DebugLog(string.Format("No action specified for VO object with name <{0}>", base.name));
		}
	}

	public void StopVO()
	{
		foreach (ResolvedVOAction instance in instances)
		{
			instance.OnFinished = (ResolvedVOAction.OnVOFinished)Delegate.Remove(instance.OnFinished, new ResolvedVOAction.OnVOFinished(OnVOFinished));
			VOManager.Instance.Stop(instance);
		}
		instances.Clear();
	}

	public bool IsFinished()
	{
		return instances.Count == 0;
	}

	public bool IsLooping()
	{
		return false;
	}

	private void OnVOFinished(IVOMixerItem vo)
	{
		foreach (LinkedListNode<ResolvedVOAction> item in Utils.RemovableEnumerate(instances))
		{
			if (item.Value == vo.Action)
			{
				instances.Remove(item);
			}
		}
		if (instances.Count == 0 && destroyWhenFinished)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private GameObject GetEmitter()
	{
		VOEmitter component = Utils.GetComponent<VOEmitter>(base.gameObject, Utils.SearchParents);
		if (component != null)
		{
			return component.gameObject;
		}
		return base.transform.root.gameObject;
	}
}
