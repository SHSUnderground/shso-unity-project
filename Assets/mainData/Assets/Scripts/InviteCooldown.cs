using UnityEngine;

public class InviteCooldown : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float startTime;

	public float cooldownDuration;

	public string inviteKey;

	public bool CooldownFinished
	{
		get
		{
			return Time.time > startTime + cooldownDuration;
		}
	}

	public float StartTime
	{
		get
		{
			return startTime;
		}
		set
		{
			startTime = value;
		}
	}

	public float Duration
	{
		get
		{
			return cooldownDuration;
		}
		set
		{
			cooldownDuration = value;
		}
	}
}
