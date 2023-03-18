using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Character/Invisible Woman/InvisibleWomanFadeController")]
public class InvisibleWomanFadeController : CharacterMaterialController
{
	public enum FadeState
	{
		Inactive,
		FadeIn,
		FadeOut
	}

	public float transitionMax = 1f;

	protected int delayedFadeInCount;

	private SkinnedMeshRenderer[] _skinnedMeshes;

	private FadeState _currentFadeState;

	private int FadeOutCount;

	private float _fadeStart;

	private bool connected;

	private List<Material[]> _oldMaterials;

	public float fadeTime = 0.5f;

	public Material overrideMaterial;

	public FadeState manualFadeControl;

	public float autoFadeInDelay = 10f;

	private bool useWipe;

	public float wipeBase;

	public float wipeHeight = 2.4f;

	private bool disabled;

	private void OnEnable()
	{
		if (_skinnedMeshes == null)
		{
			_skinnedMeshes = Utils.GetComponents<SkinnedMeshRenderer>(base.gameObject, Utils.SearchChildren);
			if (_skinnedMeshes == null || _skinnedMeshes.Length == 0)
			{
				CspUtils.DebugLog("Unable to connect fade controller to " + base.gameObject.name + ".  SkinnedMeshRenderer component not found.");
			}
		}
		if (_oldMaterials == null)
		{
			_oldMaterials = new List<Material[]>();
		}
	}

	public void Start()
	{
		if (overrideMaterial == null)
		{
			overrideMaterial = new Material(Shader.Find("Marvel/Characters/Invisible Woman/Fade"));
		}
		Animation component = Utils.GetComponent<Animation>(base.gameObject, Utils.SearchChildren);
		if (component == null)
		{
			CspUtils.DebugLog("Unable to find animation component.  Animation event listening disabled.");
			return;
		}
		InvisibleWomanFadeListener invisibleWomanFadeListener = Utils.AddComponent<InvisibleWomanFadeListener>(component.gameObject);
		invisibleWomanFadeListener.controller = this;
		autoFadeInDelay = ((!(GameController.GetController() is SocialSpaceController) && !(GameController.GetController() is CardGameController)) ? (-1f) : autoFadeInDelay);
	}

	private void Update()
	{
		if (disabled)
		{
			return;
		}
		if (manualFadeControl != 0 && _currentFadeState != manualFadeControl)
		{
			Fade(manualFadeControl == FadeState.FadeIn);
		}
		if (_currentFadeState == FadeState.Inactive)
		{
			base.enabled = false;
			return;
		}
		float num = Mathf.Min((Time.time - _fadeStart) / fadeTime, transitionMax);
		float num2 = num;
		if (_currentFadeState == FadeState.FadeIn)
		{
			num2 = transitionMax - num2;
		}
		SkinnedMeshRenderer[] skinnedMeshes = _skinnedMeshes;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshes)
		{
			for (int j = 0; j < skinnedMeshRenderer.materials.Length; j++)
			{
				skinnedMeshRenderer.materials[j].SetFloat("_Transition", num2);
			}
		}
		if (num >= transitionMax)
		{
			if (_currentFadeState == FadeState.FadeIn)
			{
				Disconnect();
			}
			if (AppShell.Instance != null && AppShell.Instance.EventMgr != null)
			{
				AppShell.Instance.EventMgr.Fire(base.gameObject, new EntityFadeMessage(base.gameObject, _currentFadeState == FadeState.FadeOut));
			}
			base.enabled = false;
			_currentFadeState = FadeState.Inactive;
		}
	}

	public void Disable(bool disable)
	{
		disabled = disable;
		if (disabled)
		{
			StopAllCoroutines();
			delayedFadeInCount = 0;
			FadeOutCount = 0;
			manualFadeControl = FadeState.Inactive;
			_currentFadeState = FadeState.Inactive;
			if (connected)
			{
				Disconnect();
			}
		}
	}

	public bool IsInvisible()
	{
		return FadeOutCount != 0;
	}

	public void Fade(bool fadeIn)
	{
		if (!disabled)
		{
			if (fadeIn)
			{
				OnFadeIn();
			}
			else
			{
				OnFadeOut();
			}
		}
	}

	public void Fade(float duration)
	{
		if (!disabled)
		{
			OnFadeOut();
			StartCoroutine(DelayedFadeIn(duration));
		}
	}

	protected void OnFadeIn()
	{
		delayedFadeInCount++;
		if (FadeOutCount != 0 && Connect())
		{
			FadeOutCount = 0;
			fadeChange();
			GetComponent<CharacterGlobals>().effectsList.TryOneShot("invisible_fade_in_sequence", base.gameObject);
		}
	}

	protected void OnFadeOut()
	{
		delayedFadeInCount++;
		if (FadeOutCount != 1)
		{
			if (!Connect())
			{
				return;
			}
			FadeOutCount = 1;
			fadeChange();
			GetComponent<CharacterGlobals>().effectsList.TryOneShot("invisible_fade_out_sequence", base.gameObject);
		}
		if (autoFadeInDelay > 0f)
		{
			StartCoroutine(DelayedFadeIn(autoFadeInDelay));
		}
	}

	protected void fadeChange()
	{
		StopAllCoroutines();
		_currentFadeState = ((FadeOutCount <= 0) ? FadeState.FadeIn : FadeState.FadeOut);
		_fadeStart = Time.time;
		base.enabled = true;
	}

	protected IEnumerator DelayedFadeIn(float delay)
	{
		int currentCount = delayedFadeInCount;
		yield return new WaitForSeconds(delay);
		if (currentCount == delayedFadeInCount)
		{
			OnFadeIn();
		}
	}

	public override bool Connect()
	{
		if (!base.Connect())
		{
			return false;
		}
		if (connected)
		{
			return true;
		}
		_oldMaterials.Clear();
		if (overrideMaterial.shader.name == "Marvel/Characters/SharedFX/WipeFade")
		{
			useWipe = true;
		}
		for (int i = 0; i < _skinnedMeshes.Length; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = _skinnedMeshes[i];
			int num = 0;
			Material[] array = new Material[skinnedMeshRenderer.materials.Length];
			Material[] materials = skinnedMeshRenderer.materials;
			foreach (Material material in materials)
			{
				array[num] = new Material(overrideMaterial);
				Texture texture = material.GetTexture("_MainTex");
				array[num].SetTexture("_MainTex", texture);
				array[num].color = material.color;
				if (useWipe)
				{
					array[num].SetFloat("_BaseY", wipeBase);
					array[num].SetFloat("_Height", wipeHeight);
				}
				num++;
			}
			_oldMaterials.Add(skinnedMeshRenderer.materials);
			skinnedMeshRenderer.materials = array;
		}
		connected = true;
		return true;
	}

	public override bool Disconnect()
	{
		if (!base.Disconnect())
		{
			return false;
		}
		if (_oldMaterials.Count > 0)
		{
			for (int i = 0; i < _skinnedMeshes.Length; i++)
			{
				_skinnedMeshes[i].materials = _oldMaterials[i];
			}
			_oldMaterials.Clear();
		}
		connected = false;
		StopAllCoroutines();
		delayedFadeInCount = 0;
		FadeOutCount = 0;
		manualFadeControl = FadeState.Inactive;
		_currentFadeState = FadeState.Inactive;
		return true;
	}
}
