using UnityEngine;

[AddComponentMenu("Interactive Object/Cooldown")]
public class InteractiveObjectCooldown : MonoBehaviour, IInteractiveObjectChild
{
	public float cooldown = 5f;

	public void Initialize(InteractiveObject owner, GameObject model)
	{
	}

	public float GetLength()
	{
		return cooldown;
	}
}
