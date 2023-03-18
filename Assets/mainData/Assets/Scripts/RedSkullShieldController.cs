using UnityEngine;

[AddComponentMenu("Character/Red Skull/Red Skull Shield Controller")]
public class RedSkullShieldController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum ShieldStatus
	{
		Inactive,
		Activate,
		Active,
		Deactivate
	}

	public string shieldName = string.Empty;

	public float shieldTransitionTime = 0.5f;

	public ShieldStatus shieldManualStatus;

	private SkinnedMeshRenderer _shieldRenderer;

	private float _shieldTransitionStartTime;

	private ShieldStatus _shieldCurrentStatus;

	public void ActivateShield()
	{
		SetShieldStatus(ShieldStatus.Activate);
	}

	public void DeactivateShield()
	{
		SetShieldStatus(ShieldStatus.Deactivate);
	}

	private void SetShieldStatus(ShieldStatus status)
	{
		switch (status)
		{
		case ShieldStatus.Inactive:
			SetShieldTransition(1f);
			base.enabled = false;
			break;
		case ShieldStatus.Active:
			SetShieldTransition(0f);
			base.enabled = false;
			break;
		case ShieldStatus.Activate:
			SetShieldTransition(1f);
			_shieldTransitionStartTime = Time.time;
			base.enabled = true;
			break;
		case ShieldStatus.Deactivate:
			SetShieldTransition(0f);
			_shieldTransitionStartTime = Time.time;
			base.enabled = true;
			break;
		}
		_shieldCurrentStatus = status;
		shieldManualStatus = status;
	}

	private void SetShieldTransition(float transition)
	{
		if (_shieldRenderer != null && _shieldRenderer.material != null)
		{
			_shieldRenderer.material.SetFloat("_Transition", transition);
		}
	}

	private void Start()
	{
		Transform transform = Utils.FindNodeInChildren(base.transform, shieldName);
		if (transform == null)
		{
			CspUtils.DebugLog("shield mesh not found as child of " + base.gameObject.name + ": unable to use shield mesh controller");
			return;
		}
		_shieldRenderer = transform.GetComponent<SkinnedMeshRenderer>();
		if (_shieldRenderer == null)
		{
			CspUtils.DebugLog("shield mesh does not have a skinned mesh renderer in " + transform.gameObject.name + ": unable to use shield mesh controller");
		}
		else
		{
			SetShieldStatus(ShieldStatus.Inactive);
		}
	}

	private void Update()
	{
		if (shieldManualStatus != _shieldCurrentStatus)
		{
			SetShieldStatus(shieldManualStatus);
		}
		float num = Time.time - _shieldTransitionStartTime;
		if (num >= shieldTransitionTime)
		{
			SetShieldStatus((_shieldCurrentStatus == ShieldStatus.Activate) ? ShieldStatus.Active : ShieldStatus.Inactive);
			return;
		}
		float num2 = Mathf.Min(num / shieldTransitionTime, 1f);
		if (_shieldCurrentStatus == ShieldStatus.Activate)
		{
			num2 = 1f - num2;
		}
		SetShieldTransition(num2);
	}
}
