using UnityEngine;

[AddComponentMenu("Test/Kevin/Animationizer")]
public class Animationizer : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Animation target;

	public AnimationClip clip;

	private void Update()
	{
		base.enabled = false;
		target.Play(clip.name);
	}
}
