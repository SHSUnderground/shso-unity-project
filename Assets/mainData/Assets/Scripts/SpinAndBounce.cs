using UnityEngine;

public class SpinAndBounce : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public delegate void BouncedCallback();

	private const float gravity = -40f;

	public float initialVelocity = 20f;

	public int numberOfBounces = 4;

	public float bounceDecayRate = 0.6f;

	public float initialRotationsPerSecond = 5f;

	public float rotationDecayPerSecond = 1f;

	public Vector3 rotationAxis = Vector3.up;

	public bool startAutomatically;

	[HideInInspector]
	public BouncedCallback OnBounced;

	private int bounces;

	private float initialY;

	private float velocity;

	private bool atBounceRest = true;

	private bool atRotateRest = true;

	private float degreesPerSecond;

	private float degreeDecayPerSecond;

	public int Bounces
	{
		get
		{
			return bounces;
		}
	}

	private void Start()
	{
		Vector3 position = base.transform.position;
		initialY = position.y;
		velocity = initialVelocity;
		degreesPerSecond = 360f * initialRotationsPerSecond;
		degreeDecayPerSecond = 360f * rotationDecayPerSecond;
		if (startAutomatically)
		{
			Go();
		}
	}

	public void Go()
	{
		atBounceRest = false;
		atRotateRest = false;
	}

	private void Update()
	{
		if (!atBounceRest)
		{
			Vector3 position = base.transform.position;
			position.y += velocity * Time.deltaTime;
			base.transform.position = position;
			velocity += -40f * Time.deltaTime;
			Vector3 position2 = base.transform.position;
			if (position2.y <= initialY)
			{
				bounces++;
				atBounceRest = (bounces >= numberOfBounces);
				position.y = initialY;
				base.transform.position = position;
				velocity = initialVelocity * Mathf.Pow(bounceDecayRate, bounces);
				if (OnBounced != null)
				{
					OnBounced();
				}
			}
		}
		if (!atRotateRest)
		{
			base.transform.Rotate(rotationAxis, degreesPerSecond * Time.deltaTime);
			degreesPerSecond -= degreeDecayPerSecond * Time.deltaTime;
			if (degreesPerSecond <= 0f)
			{
				atRotateRest = true;
			}
		}
	}
}
