using UnityEngine;

public class AnimationSpeedController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private float speed_ = 1f;

	public float speed = 1f;

	public GameObject targetObject;

	private void Awake()
	{
	}

	private void OnEnable()
	{
		UpdateSpeed();
	}

	private void Update()
	{
		if (speed_ != speed)
		{
			speed_ = speed;
			UpdateSpeed();
		}
	}

	private void UpdateSpeed()
	{
		Animation[] array = null;
		array = ((!(targetObject != null)) ? GetComponentsInChildren<Animation>() : targetObject.GetComponentsInChildren<Animation>());
		Animation[] array2 = array;
		foreach (Animation animation in array2)
		{
			foreach (AnimationState item in animation)
			{
				item.speed = speed_;
			}
		}
	}
}
