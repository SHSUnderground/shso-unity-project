using UnityEngine;

[RequireComponent(typeof(InteractiveObjectCooldown))]
public class RallyLauncherButtonToggle : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private float duration = 0.25f;

	private float startTime;

	private bool expired;

	private void Triggered()
	{
		expired = false;
		InteractiveObjectCooldown component = Utils.GetComponent<InteractiveObjectCooldown>(base.gameObject);
		if (component != null)
		{
			duration = component.cooldown - 0.5f;
		}
		Transform transform = base.gameObject.transform.parent.transform.FindChild("Model/Button_Stand/Button_Stand/button");
		if (transform != null)
		{
			GameObject gameObject = transform.gameObject;
			gameObject.renderer.material.color = new Color(1f, 0f, 0f, 1f);
		}
		startTime = Time.time;
	}

	private void Update()
	{
		if (!expired && Time.time - startTime > duration)
		{
			expired = true;
			Transform transform = base.gameObject.transform.parent.transform.FindChild("Model/Button_Stand/Button_Stand/button");
			if (transform != null)
			{
				GameObject gameObject = transform.gameObject;
				gameObject.renderer.material.color = new Color(0f, 1f, 0f, 1f);
			}
		}
	}
}
