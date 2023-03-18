using UnityEngine;

public class ChallengeZoneEvent : ChallengeEvent
{
	private ChallengeZone _challengeZone;

	public override void Initialize(ChallengeManager manager, ChallengeInfo info, ISHSCounterType counter, ChallengeManager.ChallengeCompleteDelegate onChallengeComplete)
	{
		base.Initialize(manager, info, counter, onChallengeComplete);
		_challengeZone = null;
		foreach (ChallengeInfoParameters parameter in info.Parameters)
		{
			if (parameter.key == "challenge_zone_name")
			{
				GameObject gameObject = GameObject.Find(parameter.value);
				if (gameObject != null)
				{
					_challengeZone = gameObject.GetComponent<ChallengeZone>();
					break;
				}
			}
		}
	}

	public override void Ready()
	{
		base.Ready();
		if (_challengeZone != null && _challengeZone.IsZoneChallengeMet())
		{
			NotifyOnClientChallengeMet();
		}
	}
}
