using UnityEngine;

public class DiamondFormController : CharacterMaterialController
{
	public string diamondPrefabName;

	public float autoRemoveFormDelay = 20f;

	public bool overrideImpactMatrix = true;

	private GameObject _diamondForm;

	private ImpactMatrix.Type _originalIMType;

	public void ToggleDiamondForm(bool apply)
	{
		if (apply)
		{
			Connect(autoRemoveFormDelay);
		}
		else
		{
			Disconnect();
		}
	}

	public override bool Connect()
	{
		return Connect(autoRemoveFormDelay);
	}

	public bool Connect(float duration)
	{
		if (!base.Connect())
		{
			return false;
		}
		if (HasDiamondForm())
		{
			return true;
		}
		EffectSequenceList component = base.gameObject.GetComponent<EffectSequenceList>();
		if (component == null)
		{
			CspUtils.DebugLog("<" + base.gameObject.name + "> cannot apply diamond form because they do not have an effect sequence list");
			return false;
		}
		EffectSequence effectSequence = null;
		GameObject gameObject = component.TryGetEffectSequencePrefabByName(diamondPrefabName) as GameObject;
		if (gameObject != null)
		{
			_diamondForm = (Object.Instantiate(gameObject) as GameObject);
			if (_diamondForm != null)
			{
				effectSequence = _diamondForm.GetComponent<EffectSequence>();
			}
			if (overrideImpactMatrix)
			{
				CombatController component2 = base.gameObject.GetComponent<CombatController>();
				_originalIMType = component2.ImpactMatrixType;
				component2.ImpactMatrixType = ImpactMatrix.Type.Metal;
			}
		}
		else
		{
			CspUtils.DebugLog("failed to find prefab <" + diamondPrefabName + "> in <" + base.gameObject.name + "> character's bundle");
		}
		if (effectSequence != null)
		{
			if (duration > 0f)
			{
				effectSequence.TotalLifetime = duration;
			}
			effectSequence.Initialize(base.gameObject, null, null);
		}
		else
		{
			CspUtils.DebugLog("failed to start effect sequence for prefab <" + diamondPrefabName + "> with character <" + base.gameObject.name + ">");
		}
		return true;
	}

	public override bool Disconnect()
	{
		if (!base.Disconnect())
		{
			return false;
		}
		if (_diamondForm != null)
		{
			EffectSequence component = _diamondForm.GetComponent<EffectSequence>();
			if (component != null)
			{
				component.Cancel();
			}
			if (overrideImpactMatrix)
			{
				base.gameObject.GetComponent<CombatController>().ImpactMatrixType = _originalIMType;
			}
		}
		_diamondForm = null;
		return true;
	}

	public bool HasDiamondForm()
	{
		return _diamondForm != null;
	}

	[AnimTag("diamond")]
	private void OnDiamondAnimTag(AnimationEvent e)
	{
		string[] array = e.stringParameter.Split(':');
		string a = array[0];
		float num = 0f;
		if (array.Length > 1)
		{
			num = float.Parse(array[1]);
		}
		if (num > 0f)
		{
			Connect(num);
		}
		else if (a == "toggle")
		{
			ToggleDiamondForm(!HasDiamondForm());
		}
		else
		{
			ToggleDiamondForm(a == "out");
		}
	}
}
