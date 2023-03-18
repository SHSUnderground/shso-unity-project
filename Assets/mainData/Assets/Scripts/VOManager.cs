using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class VOManager : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public class VODataNotLoadedException : Exception
	{
	}

	protected class RoutingSystem
	{
		protected VOGlobalMixer globalMixer;

		protected VOCustomSubmixer customSubmixes;

		protected VOMixerItemQueuer mixerItemQueuer;

		protected LinkedList<VOEmitterMixer> emitterMixers;

		protected VOEmitterMixer nullEmitterMixer;

		public VOCustomSubmixer CustomSubmixes
		{
			get
			{
				return customSubmixes;
			}
		}

		public RoutingSystem()
		{
			globalMixer = new VOGlobalMixer();
			customSubmixes = new VOCustomSubmixer();
			customSubmixes.SetOutput(globalMixer);
			customSubmixes.InitializeGlobalCustomSubmixers();
			mixerItemQueuer = new VOMixerItemQueuer();
			mixerItemQueuer.SetOutput(customSubmixes);
			nullEmitterMixer = new VOEmitterMixer(null);
			nullEmitterMixer.SetOutput(mixerItemQueuer);
			emitterMixers = new LinkedList<VOEmitterMixer>();
		}

		public void Update()
		{
			foreach (LinkedListNode<VOEmitterMixer> item in Utils.RemovableEnumerate(emitterMixers))
			{
				if (item.Value.Emitter == null)
				{
					emitterMixers.Remove(item);
				}
				else
				{
					item.Value.Update();
				}
			}
			nullEmitterMixer.Update();
			mixerItemQueuer.Update();
			customSubmixes.Update();
			globalMixer.Update();
		}

		public void RouteVOAction(ResolvedVOAction action, IResolvedVOActionHandler handler)
		{
			IVOMixerItem item = new VOMixerItem(action, handler);
			if (action.Emitter == null)
			{
				nullEmitterMixer.SendVO(item);
				return;
			}
			foreach (VOEmitterMixer emitterMixer in emitterMixers)
			{
				if (emitterMixer.Emitter == action.Emitter)
				{
					emitterMixer.SendVO(item);
					return;
				}
			}
			VOEmitterMixer vOEmitterMixer = new VOEmitterMixer(action.Emitter);
			vOEmitterMixer.SetOutput(mixerItemQueuer);
			emitterMixers.AddLast(vOEmitterMixer);
			vOEmitterMixer.SendVO(item);
		}

		public void Cancel(ResolvedVOAction action)
		{
			nullEmitterMixer.Cancel(action);
			foreach (VOEmitterMixer emitterMixer in emitterMixers)
			{
				emitterMixer.Cancel(action);
			}
		}

		public void CancelAll()
		{
			nullEmitterMixer.Cancel();
			foreach (VOEmitterMixer emitterMixer in emitterMixers)
			{
				emitterMixer.Cancel();
			}
		}

		public bool IsEmitterInUse(GameObject emitter)
		{
			if (emitter == null)
			{
				return nullEmitterMixer.InUse();
			}
			foreach (VOEmitterMixer emitterMixer in emitterMixers)
			{
				if (emitterMixer.Emitter == emitter)
				{
					return emitterMixer.InUse();
				}
			}
			return false;
		}
	}

	protected VOHooks hooks = new VOHooks();

	protected RoutingSystem routingSystem = new RoutingSystem();

	protected Dictionary<string, IResolvedVOActionHandler> localeSpecificHandlers;

	[CompilerGenerated]
	private static VOManager _003CInstance_003Ek__BackingField;

	public static VOManager Instance
	{
		[CompilerGenerated]
		get
		{
			return _003CInstance_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CInstance_003Ek__BackingField = value;
		}
	}

	public VOCustomSubmixer CustomSubmixes
	{
		get
		{
			return routingSystem.CustomSubmixes;
		}
	}

	public VOHooks Hooks
	{
		get
		{
			return hooks;
		}
	}

	public ResolvedVOAction GetVO(string action, GameObject emitter, IEnumerable<IVOInputResolver> inputs)
	{
		return ResolveVOAction(action, emitter, inputs);
	}

	public ResolvedVOAction GetVO(string action, GameObject emitter)
	{
		return ResolveVOAction(action, emitter, null);
	}

	public ResolvedVOAction GetVO(string action, IEnumerable<IVOInputResolver> inputs)
	{
		return GetVO(action, null, inputs);
	}

	public ResolvedVOAction PlayResolvedVO(ResolvedVOAction action)
	{
		LocalPlayResolvedVO(action);
		DispatchVOAction(action);
		return action;
	}

	public ResolvedVOAction LocalPlayResolvedVO(ResolvedVOAction action)
	{
		if (action != null && action.IsPlayable)
		{
			hooks.ResolvedVOPlayed(action);
			routingSystem.RouteVOAction(action, GetResolvedVOActionHandler(action));
		}
		return action;
	}

	public ResolvedVOAction PlayVO(string action, GameObject emitter, IEnumerable<IVOInputResolver> inputs)
	{
		return PlayResolvedVO(GetVO(action, emitter, inputs));
	}

	public ResolvedVOAction PlayVO(string action, GameObject emitter)
	{
		return PlayVO(action, emitter, null);
	}

	public ResolvedVOAction PlayVO(string action, IEnumerable<IVOInputResolver> inputs)
	{
		return PlayVO(action, null, inputs);
	}

	public ResolvedVOAction PlayVO(string action)
	{
		return PlayVO(action, null, null);
	}

	public ResolvedVOAction PlayNetVO(NetActionVO netVO)
	{
		if (netVO != null && Singleton<VOAssetManifest>.instance.DoesVOAssetExist(netVO.action.AudioBundle, netVO.action.AudioAsset))
		{
			return LocalPlayResolvedVO(netVO.action);
		}
		return netVO.action;
	}

	public void Stop(ResolvedVOAction vo)
	{
		routingSystem.Cancel(vo);
	}

	public void StopAll()
	{
		routingSystem.CancelAll();
	}

	public bool IsEmitterInUse(GameObject emitter)
	{
		return routingSystem.IsEmitterInUse(emitter);
	}

	public IResolvedVOActionHandler GetLocaleSpecificVOActionHandler()
	{
		IResolvedVOActionHandler value;
		if (!localeSpecificHandlers.TryGetValue(LocaleMapper.GetCurrentLocale(), out value))
		{
			return localeSpecificHandlers["FALLBACK"];
		}
		return value;
	}

	protected void Update()
	{
		routingSystem.Update();
	}

	protected void Awake()
	{
		Instance = this;
		InitializeLocaleSpecificHandlers();
		InitializeSystemHooks();
	}

	protected ResolvedVOAction ResolveVOAction(string action, GameObject emitter, IEnumerable<IVOInputResolver> inputs)
	{
		if (!Singleton<VOActionDataManager>.instance.DataIsLoaded)
		{
			throw new VODataNotLoadedException();
		}
		ResolvedVOAction resolvedVOAction = new ResolvedVOAction(Singleton<VOActionDataManager>.instance.VOActions[action], emitter, inputs);
		return (!resolvedVOAction.IsResolved) ? null : resolvedVOAction;
	}

	protected void DispatchVOAction(ResolvedVOAction voAction)
	{
		if (voAction != null && voAction.IsPlayable && voAction.Emitter != null && voAction.IsNetworked)
		{
			NetworkComponent component = voAction.Emitter.GetComponent<NetworkComponent>();
			if (component != null)
			{
				component.QueueNetActionIgnoringOwnership(new NetActionVO(voAction));
			}
		}
	}

	protected IResolvedVOActionHandler GetResolvedVOActionHandler(ResolvedVOAction action)
	{
		if (action.CustomHandler != null)
		{
			return action.CustomHandler;
		}
		return GetLocaleSpecificVOActionHandler();
	}

	protected void InitializeLocaleSpecificHandlers()
	{
		localeSpecificHandlers = new Dictionary<string, IResolvedVOActionHandler>();
		localeSpecificHandlers["FALLBACK"] = new GenericVOActionHandler();
		localeSpecificHandlers["en_us"] = new GenericVOActionHandler();
	}

	protected void InitializeSystemHooks()
	{
		AppShell.Instance.OnOldControllerUnloading += OnControllerUnloading;
	}

	private void OnControllerUnloading(AppShell.GameControllerTypeData currentGameTypeData, AppShell.GameControllerTypeData newGameTypeData)
	{
		hooks.ClearDelegates();
	}
}
