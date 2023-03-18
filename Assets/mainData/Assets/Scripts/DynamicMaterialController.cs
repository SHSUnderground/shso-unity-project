using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("Character/DynamicMaterialController")]
public class DynamicMaterialController : CharacterMaterialController
{
	public bool reparentToRoot;

	public float delay;

	public float duration = -1f;

	public bool destroyWhenFinished = true;

	public bool includeParticleRenderers;

	public bool includeNonSkinnedRenderers;

	public MaterialOverride[] newMaterialInfo;

	private Dictionary<Renderer, Material[]> originalMaterials;

	private float startTime;

	[CompilerGenerated]
	private bool _003CConnected_003Ek__BackingField;

	public bool Connected
	{
		[CompilerGenerated]
		get
		{
			return _003CConnected_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CConnected_003Ek__BackingField = value;
		}
	}

	protected IEnumerable Renderers
	{
		get
		{
			return originalMaterials.Keys;
		}
		set
		{
			originalMaterials = new Dictionary<Renderer, Material[]>();
			foreach (Renderer item in value)
			{
				originalMaterials[item] = item.materials;
			}
		}
	}

	public DynamicMaterialController()
	{
		newMaterialInfo = new MaterialOverride[1]
		{
			new MaterialOverride()
		};
	}

	public void Start()
	{
		if (reparentToRoot)
		{
			base.transform.parent = GetCharacterRoot().transform;
		}
		StartCoroutine(TimedConnect(delay));
	}

	public void Update()
	{
		if (Connected)
		{
			MaterialOverride[] array = newMaterialInfo;
			foreach (MaterialOverride materialOverride in array)
			{
				materialOverride.AnimateTo(Time.time - startTime, Renderers);
			}
		}
	}

	public void OnDestroy()
	{
		Disconnect();
	}

	public override bool Connect()
	{
		if (base.Connect())
		{
			if (!Connected)
			{
				Connected = true;
				startTime = Time.time;
				ApplyMaterials();
				StartCoroutine(TimedDisconnect(duration));
			}
			return true;
		}
		return false;
	}

	public override bool Disconnect()
	{
		return Disconnect(false);
	}

	public virtual bool Disconnect(bool ignoreDestruction)
	{
		if (base.Disconnect())
		{
			if (Connected)
			{
				Connected = false;
				RestoreMaterials();
				if (!ignoreDestruction && destroyWhenFinished)
				{
					Object.Destroy(base.gameObject);
				}
			}
			return true;
		}
		return false;
	}

	protected IEnumerator TimedConnect(float delay)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		Connect();
	}

	protected IEnumerator TimedDisconnect(float delay)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
			Disconnect();
		}
	}

	protected void ApplyMaterials()
	{
		Renderers = GetRenderers();
		MaterialOverride[] array = newMaterialInfo;
		foreach (MaterialOverride materialOverride in array)
		{
			materialOverride.ApplyToRenderers(Renderers);
		}
		GetCharacterRoot().SendMessage("OnMaterialsChanged", SendMessageOptions.DontRequireReceiver);
	}

	protected void RestoreMaterials()
	{
		foreach (Renderer renderer in Renderers)
		{
			renderer.materials = originalMaterials[renderer];
		}
		MaterialOverride[] array = newMaterialInfo;
		foreach (MaterialOverride materialOverride in array)
		{
			materialOverride.ClearResources();
		}
		GetCharacterRoot().SendMessage("OnMaterialsChanged", SendMessageOptions.DontRequireReceiver);
	}

	protected IEnumerable<Renderer> GetRenderers()
	{
		Renderer[] componentsInChildren = GetCharacterRoot().GetComponentsInChildren<Renderer>(true);
		foreach (Renderer r in componentsInChildren)
		{
			bool isSkinned = r is SkinnedMeshRenderer;
			bool isParticle = r is ParticleRenderer;
			if ((isParticle && includeParticleRenderers && includeNonSkinnedRenderers) || (!isParticle && !isSkinned && includeNonSkinnedRenderers) || isSkinned)
			{
				yield return r;
			}
		}
	}
}
