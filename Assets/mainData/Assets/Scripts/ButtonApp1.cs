using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ButtonApp1 : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Texture2D rollOverTexture;

	public Texture2D normalTexture;

	public AudioClip beep;

	private void OnMouseEnter()
	{
		base.guiTexture.texture = rollOverTexture;
	}

	private void OnMouseExit()
	{
		base.guiTexture.texture = normalTexture;
	}

	private void OnMouseUp()
	{
		base.audio.PlayOneShot(beep);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
