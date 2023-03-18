using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AFKWatcher : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float afkTime = 30f;

	private bool isAFK;

	private PlayerStatusDefinition.Status statusPriorToAFK;

	[CompilerGenerated]
	private bool _003CDisabled_003Ek__BackingField;

	public bool Disabled
	{
		[CompilerGenerated]
		get
		{
			return _003CDisabled_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CDisabled_003Ek__BackingField = value;
		}
	}

	private void Start()
	{
		Disabled = false;
		statusPriorToAFK = PlayerStatusDefinition.Instance.GetStatus(PlayerStatusDefinition.DefaultStatusName);
		StartCoroutine(CheckForAFK());
	}

	private IEnumerator CheckForAFK()
	{
		while (true)
		{
			if (isAFK)
			{
				while (isAFK)
				{
					float lastIdleTime = SHSInput.IdleTime;
					yield return 0;
					if (SHSInput.IdleTime < lastIdleTime)
					{
						isAFK = false;
						PlayerStatus.SetLocalStatus(statusPriorToAFK);
					}
				}
			}
			else if (!Disabled && SHSInput.IdleTime > SHSInput.maxAFK)
			{
				isAFK = true;
				statusPriorToAFK = GetCurrentStatus();
				PlayerStatus.SetLocalStatus(PlayerStatusDefinition.Instance.GetStatus("AFK"));
				Debug.Log ("Idle for too long! Quitting Application!");
				Application.Quit();
			}
			else
			{
				yield return new WaitForSeconds(1f);
			}
		}
	}

	private PlayerStatusDefinition.Status GetCurrentStatus()
	{
		PlayerStatus component = Utils.GetComponent<PlayerStatus>(base.gameObject);
		if (component == null)
		{
			return statusPriorToAFK;
		}
		return component.Status;
	}
}
