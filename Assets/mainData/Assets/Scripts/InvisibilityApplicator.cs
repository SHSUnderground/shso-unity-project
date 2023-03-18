using UnityEngine;

public class InvisibilityApplicator : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected InvisibleWomanFadeController controller;

	protected bool ownsFadeController;

	public float duration = 3f;

	public int fadeStartCounter = -1;

	public void Start()
	{
		InvisibleWomanFadeController component = Utils.GetComponent<InvisibleWomanFadeController>(base.transform.root.gameObject);
		if (component == null)
		{
			controller = base.transform.root.gameObject.AddComponent<InvisibleWomanFadeController>();
			ownsFadeController = true;
		}
		else
		{
			controller = component;
		}
		fadeStartCounter = 3;
	}

	public void Update()
	{
		if (fadeStartCounter >= 0)
		{
			fadeStartCounter--;
		}
		if (fadeStartCounter == 0 && !controller.IsInvisible())
		{
			controller.autoFadeInDelay = duration;
			controller.Fade(duration);
		}
	}

	public void OnDisable()
	{
		if (ownsFadeController && controller != null)
		{
			Object.Destroy(controller);
			controller = null;
		}
	}
}
