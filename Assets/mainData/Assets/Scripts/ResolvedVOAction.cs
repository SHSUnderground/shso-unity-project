using ShsAudio;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ResolvedVOAction
{
	public delegate void OnVOFinished(IVOMixerItem vo);

	private GameObject _emitter;

	private VOAction _voAction;

	[CompilerGenerated]
	private bool _003CIsNetworked_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CIsMutedByUI_003Ek__BackingField;

	[CompilerGenerated]
	private IResolvedVOActionHandler _003CCustomHandler_003Ek__BackingField;

	[CompilerGenerated]
	private VORoutingInfo _003CCustomRouting_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CCustomSubmixerName_003Ek__BackingField;

	[CompilerGenerated]
	private OnVOFinished _003COnFinished_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CIsResolved_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CShouldHaveEmitter_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CAudioBundle_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CAudioAsset_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CTextID_003Ek__BackingField;

	[CompilerGenerated]
	private string[] _003CResolvedInputs_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CIsTutorialVO_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CIsLocallyOwned_003Ek__BackingField;

	public bool IsNetworked
	{
		[CompilerGenerated]
		get
		{
			return _003CIsNetworked_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CIsNetworked_003Ek__BackingField = value;
		}
	}

	public bool IsMutedByUI
	{
		[CompilerGenerated]
		get
		{
			return _003CIsMutedByUI_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CIsMutedByUI_003Ek__BackingField = value;
		}
	}

	public IResolvedVOActionHandler CustomHandler
	{
		[CompilerGenerated]
		get
		{
			return _003CCustomHandler_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CCustomHandler_003Ek__BackingField = value;
		}
	}

	public VORoutingInfo CustomRouting
	{
		[CompilerGenerated]
		get
		{
			return _003CCustomRouting_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CCustomRouting_003Ek__BackingField = value;
		}
	}

	public string CustomSubmixerName
	{
		[CompilerGenerated]
		get
		{
			return _003CCustomSubmixerName_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CCustomSubmixerName_003Ek__BackingField = value;
		}
	}

	public OnVOFinished OnFinished
	{
		[CompilerGenerated]
		get
		{
			return _003COnFinished_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003COnFinished_003Ek__BackingField = value;
		}
	}

	public bool IsResolved
	{
		[CompilerGenerated]
		get
		{
			return _003CIsResolved_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CIsResolved_003Ek__BackingField = value;
		}
	}

	public bool ShouldHaveEmitter
	{
		[CompilerGenerated]
		get
		{
			return _003CShouldHaveEmitter_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CShouldHaveEmitter_003Ek__BackingField = value;
		}
	}

	public string AudioBundle
	{
		[CompilerGenerated]
		get
		{
			return _003CAudioBundle_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CAudioBundle_003Ek__BackingField = value;
		}
	}

	public string AudioAsset
	{
		[CompilerGenerated]
		get
		{
			return _003CAudioAsset_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CAudioAsset_003Ek__BackingField = value;
		}
	}

	public string TextID
	{
		[CompilerGenerated]
		get
		{
			return _003CTextID_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CTextID_003Ek__BackingField = value;
		}
	}

	public string[] ResolvedInputs
	{
		[CompilerGenerated]
		get
		{
			return _003CResolvedInputs_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CResolvedInputs_003Ek__BackingField = value;
		}
	}

	public bool IsTutorialVO
	{
		[CompilerGenerated]
		get
		{
			return _003CIsTutorialVO_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CIsTutorialVO_003Ek__BackingField = value;
		}
	}

	public bool IsLocallyOwned
	{
		[CompilerGenerated]
		get
		{
			return _003CIsLocallyOwned_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CIsLocallyOwned_003Ek__BackingField = value;
		}
	}

	public GameObject Emitter
	{
		get
		{
			return _emitter;
		}
		protected set
		{
			_emitter = value;
			ShouldHaveEmitter = (value != null);
			IsLocallyOwned = IsEmitterOwned(value);
		}
	}

	public VOAction VOAction
	{
		get
		{
			return _voAction;
		}
		protected set
		{
			_voAction = value;
			IsTutorialVO = (value != null && value.Name == "tutorial");
		}
	}

	public bool HasAudio
	{
		get
		{
			return !string.IsNullOrEmpty(AudioBundle) && !string.IsNullOrEmpty(AudioAsset);
		}
	}

	public bool HasText
	{
		get
		{
			return !string.IsNullOrEmpty(TextID);
		}
	}

	public VORoutingInfo Routing
	{
		get
		{
			return (CustomRouting == null) ? VOAction.Routing : CustomRouting;
		}
	}

	public LocalizationStrategy LocalizationStrategy
	{
		get
		{
			AudioAssetType assetType = (!IsTutorialVO) ? AudioAssetType.FlavorVO : AudioAssetType.TutorialVO;
			return Helpers.GetLocalizationStrategy(assetType);
		}
	}

	public bool AssumeDownloaded
	{
		get
		{
			return IsTutorialVO;
		}
	}

	public bool IsPlayable
	{
		get
		{
			if (VOAction.Disabled || !IsResolved || (Emitter == null && ShouldHaveEmitter) || (Emitter != null && SuppressVO.IsEmitterSuppressed(Emitter)) || (Emitter != null && AppShell.Instance.Profile != null && AppShell.Instance.Profile.AvailableFriends.IsPlayerBlocked(Emitter)) || (IsTutorialVO && !Configuration.TutorialVOEnabled) || (!IsTutorialVO && !Configuration.FlavorVOEnabled))
			{
				return false;
			}
			return true;
		}
	}

	public ResolvedVOAction()
	{
		IsNetworked = true;
		IsMutedByUI = true;
		IsResolved = false;
		Emitter = null;
		AudioBundle = null;
		AudioAsset = null;
		TextID = null;
		CustomHandler = null;
		CustomRouting = null;
		CustomSubmixerName = string.Empty;
	}

	public ResolvedVOAction(VOAction voAction, GameObject emitter, IEnumerable<IVOInputResolver> inputs)
	{
		Resolve(voAction, emitter, inputs);
	}

	public ResolvedVOAction(VOAction voAction, GameObject emitter, string[] resolvedInputs)
	{
		Resolve(voAction, emitter, resolvedInputs);
	}

	public void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		writer.Write(VOAction.Name);
		writer.Write(IsNetworked);
		writer.Write(IsMutedByUI);
		writer.Write(Emitter);
		writer.Write(AudioBundle);
		writer.Write(AudioAsset);
		writer.Write(TextID);
		writer.Write(CustomSubmixerName ?? string.Empty);
		Routing.SerializeToBinary(writer);
	}

	public void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		VOAction = Singleton<VOActionDataManager>.instance.VOActions[reader.ReadString()];
		IsNetworked = reader.ReadBoolean();
		IsMutedByUI = reader.ReadBoolean();
		Emitter = reader.ReadGameObject();
		AudioBundle = reader.ReadString();
		AudioAsset = reader.ReadString();
		TextID = reader.ReadString();
		CustomSubmixerName = reader.ReadString();
		CustomRouting = new VORoutingInfo();
		CustomRouting.DeserializeFromBinary(reader);
		IsResolved = true;
	}

	public VORoutingInfo.ComparisonResolution CompareTo(ResolvedVOAction older)
	{
		return VORoutingInfo.Compare(older.Routing, Routing, VOAction == older.VOAction);
	}

	protected void Resolve(VOAction voAction, GameObject emitter, IEnumerable<IVOInputResolver> inputs)
	{
		List<string> list = new List<string>();
		if (voAction.Inputs != null)
		{
			object obj;
			if (inputs != null)
			{
				IEnumerator<IVOInputResolver> enumerator = inputs.GetEnumerator();
				obj = enumerator;
			}
			else
			{
				obj = null;
			}
			IEnumerator<IVOInputResolver> enumerator2 = (IEnumerator<IVOInputResolver>)obj;
			VOAction.VOActionInput[] inputs2 = voAction.Inputs;
			foreach (VOAction.VOActionInput vOActionInput in inputs2)
			{
				if (vOActionInput == null || string.IsNullOrEmpty(vOActionInput.Class))
				{
					if (enumerator2.MoveNext())
					{
						string item = enumerator2.Current.ResolveVOInput(emitter, list);
						list.Add(item);
					}
					continue;
				}
				IVOInputResolver iVOInputResolver = InstantiateInputResolver(vOActionInput.Class, vOActionInput.Parameters);
				if (iVOInputResolver != null)
				{
					string item2 = iVOInputResolver.ResolveVOInput(emitter, list);
					list.Add(item2);
				}
			}
		}
		Resolve(voAction, emitter, list.ToArray());
	}

	protected void Resolve(VOAction voAction, GameObject emitter, string[] resolvedInputs)
	{
		VOAction = voAction;
		IsNetworked = !voAction.LocalOnly;
		IsMutedByUI = !voAction.PlayOverUI;
		ResolvedInputs = resolvedInputs;
		Emitter = emitter;
		if (!voAction.Disabled && (voAction.PlayOnUnowned || IsLocallyOwned))
		{
			VOAction.VOActionClip vOActionClip = SelectClip(voAction, resolvedInputs);
			if (vOActionClip != null)
			{
				TextID = string.Format(vOActionClip.TextID, resolvedInputs).ToUpper();
				IsResolved = true;
			}
		}
	}

	protected VOAction.VOActionClip SelectClip(VOAction voAction, string[] resolvedInputs)
	{
		List<VOAction.VOActionClip> list = new List<VOAction.VOActionClip>(voAction.Clips);
		while (list.Count > 0)
		{
			VOAction.VOActionClip vOActionClip = list[UnityEngine.Random.Range(0, list.Count)];
			string bundle = string.Format(vOActionClip.Bundle, resolvedInputs);
			string asset = string.Format(vOActionClip.Asset, resolvedInputs);
			BundledAsset vOAsset = Singleton<VOAssetManifest>.instance.GetVOAsset(bundle, asset);
			if (vOAsset != null)
			{
				AudioBundle = vOAsset.Bundle;
				AudioAsset = vOAsset.Asset;
				return vOActionClip;
			}
			list.Remove(vOActionClip);
		}
		return null;
	}

	protected IVOInputResolver InstantiateInputResolver(string className, string[] parameters)
	{
		Type type = Assembly.GetExecutingAssembly().GetType(className);
		if (type == null || !typeof(IVOInputResolver).IsAssignableFrom(type))
		{
			return null;
		}
		IVOInputResolver iVOInputResolver = Activator.CreateInstance(type) as IVOInputResolver;
		if (iVOInputResolver != null)
		{
			iVOInputResolver.SetVOParams(parameters);
		}
		return iVOInputResolver;
	}

	protected bool IsEmitterOwned(GameObject emitter)
	{
		if (emitter == null)
		{
			return true;
		}
		NetworkComponent component = emitter.GetComponent<NetworkComponent>();
		return component == null || component.IsOwner();
	}
}
