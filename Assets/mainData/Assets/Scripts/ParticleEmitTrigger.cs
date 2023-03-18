using UnityEngine;

[AddComponentMenu("Triggers/Particle Emit Trigger")]
public class ParticleEmitTrigger : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum Mode
	{
		StopEmitOnEnter,
		StartEmitOnEnter
	}

	public GameObject emitterOwner;

	public Mode mode;

	public bool localPlayerOnly = true;

	private void OnTriggerEnter(Collider other)
	{
		if (Utils.IsLocalPlayer(other.gameObject))
		{
			SetEmit(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (Utils.IsLocalPlayer(other.gameObject))
		{
			SetEmit(false);
		}
	}

	private void SetEmit(bool entering)
	{
		if (!(emitterOwner != null))
		{
			return;
		}
		ParticleEmitter[] componentsInChildren = emitterOwner.GetComponentsInChildren<ParticleEmitter>();
		foreach (ParticleEmitter particleEmitter in componentsInChildren)
		{
			switch (mode)
			{
			case Mode.StopEmitOnEnter:
				particleEmitter.emit = !entering;
				break;
			case Mode.StartEmitOnEnter:
				particleEmitter.emit = entering;
				break;
			}
		}
	}
}
