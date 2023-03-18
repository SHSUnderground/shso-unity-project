using UnityEngine;

public class PlacedEffect : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string EffectBank = "assign bank file";

	public string Fx = "effect picked from Effect Bank";

	public bool StartPlaying = true;

	public bool RandomizeStartTime;

	public bool AutoRestart = true;

	public float MaximumRestartDelay;

	public float MinimumRestartDelay;

	private void Start()
	{
	}

	private void Update()
	{
	}
}
