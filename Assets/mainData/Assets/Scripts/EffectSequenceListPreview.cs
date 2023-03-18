using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EffectSequenceListPreview : EffectSequenceList
{
	public interface IEffectPrefabProvider
	{
		Object GetEffectPrefab(string path);
	}

	[CompilerGenerated]
	private IEffectPrefabProvider _003CPrefabProvider_003Ek__BackingField;

	[CompilerGenerated]
	private Dictionary<string, string> _003CEffectPaths_003Ek__BackingField;

	public IEffectPrefabProvider PrefabProvider
	{
		[CompilerGenerated]
		get
		{
			return _003CPrefabProvider_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CPrefabProvider_003Ek__BackingField = value;
		}
	}

	public Dictionary<string, string> EffectPaths
	{
		[CompilerGenerated]
		get
		{
			return _003CEffectPaths_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CEffectPaths_003Ek__BackingField = value;
		}
	}

	public EffectSequenceListPreview()
	{
		EffectPaths = new Dictionary<string, string>();
	}

	public void SetEffectPath(string name, string prefabPath)
	{
		EffectPaths[name] = prefabPath;
	}

	public void ClearEffectPaths()
	{
		EffectPaths.Clear();
	}

	public List<string> GetBundleNames(bool includeShared)
	{
		List<string> list = new List<string>();
		if (!string.IsNullOrEmpty(characterPrefabsBundleName))
		{
			list.Add(characterPrefabsBundleName);
		}
		if (!string.IsNullOrEmpty(characterFxBundleName))
		{
			list.Add(characterFxBundleName);
		}
		if (includeShared)
		{
			foreach (string sharedFxBundleName in sharedFxBundleNames)
			{
				list.Add(sharedFxBundleName);
			}
			return list;
		}
		return list;
	}

	protected override Object GetEffectSequencePrefabByNameInternal(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			CspUtils.DebugLog("Null/empty effect sequence name passed to GetEffectSequencePrefabByNameInternal");
			return null;
		}
		string value;
		if (PrefabProvider != null && EffectPaths.TryGetValue(name, out value))
		{
			return PrefabProvider.GetEffectPrefab(value);
		}
		return base.GetEffectSequencePrefabByNameInternal(name);
	}

	public override bool TryGetLogicalEffectSequencePrefab(string name, out GameObject prefab)
	{
		EffectInfo value = null;
		string value2;
		if (LogicalEffects.TryGetValue(name, out value) && PrefabProvider != null && EffectPaths.TryGetValue(value.name, out value2))
		{
			prefab = (PrefabProvider.GetEffectPrefab(value2) as GameObject);
			return true;
		}
		return base.TryGetLogicalEffectSequencePrefab(name, out prefab);
	}
}
