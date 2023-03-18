using UnityEngine;

public class InvisibilityRemover : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public void Start()
	{
		InvisibleWomanFadeController component = Utils.GetComponent<InvisibleWomanFadeController>(base.transform.root.gameObject);
		if (component.IsInvisible())
		{
			component.Fade(true);
		}
	}
}
