using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class KeyInputTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float acceleration = 50f;

	public float speed = 5f;

	public float jump = 10f;

	protected CharacterController controller;

	protected Vector3 velocity = Vector3.zero;

	protected CameraManagerTest cameraMgr;

	private void Start()
	{
		controller = (GetComponent(typeof(CharacterController)) as CharacterController);
		cameraMgr = (Object.FindObjectOfType(typeof(CameraManagerTest)) as CameraManagerTest);
	}

	private void Update()
	{
		Vector3 a = Vector3.zero;
		if (controller.isGrounded)
		{
			velocity.y = 0f;
		}
		Vector3 vector = new Vector3(SHSInput.GetAxis("Horizontal"), 0f, SHSInput.GetAxis("Vertical"));
		if (vector.x == 0f && vector.z == 0f)
		{
			velocity.x = 0f;
			velocity.z = 0f;
		}
		else
		{
			Camera camera = Camera.main;
			if (cameraMgr != null)
			{
				camera = cameraMgr.activeCamera;
			}
			vector = camera.transform.TransformDirection(vector);
			vector.y = 0f;
			vector.Normalize();
			a = vector * acceleration;
			a *= acceleration;
		}
		a.y = -9.8f;
		if (controller.isGrounded && SHSInput.GetKeyDown(KeyCode.Space))
		{
			a.y += jump / Time.deltaTime;
		}
		velocity += a * Time.deltaTime;
		Vector3 vector2 = velocity;
		vector2.y = 0f;
		if (vector2.magnitude > speed)
		{
			vector2.Normalize();
			vector2 *= speed;
			velocity.x = vector2.x;
			velocity.z = vector2.z;
		}
		controller.Move(velocity * Time.deltaTime);
	}
}
