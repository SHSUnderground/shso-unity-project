using UnityEngine;

[AddComponentMenu("Camera/Camera Shake")]
public class CameraShake : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float Strength = 5f;

	public bool FalloffOverTime = true;

	public float Lifetime = 2f;

	public bool EnableShaking = true;

	public bool FalloffOverDistance = true;

	public bool DoNotShakeIfNoPlayer;

	public float MaxDistance = 15f;

	protected float accumulatedTime;

	protected float effectDistance;

	protected GameObject localPlayer;

	protected Vector3 shakeOffset;

	public Vector3 ShakeOffset
	{
		get
		{
			return shakeOffset;
		}
	}

	public void Start()
	{
		CameraShakeManager.CreateSingleton();
		CameraShakeManager.Instance.AddShake(this);
		GameController controller = GameController.GetController();
		if (controller != null)
		{
			localPlayer = controller.LocalPlayer;
		}
		if (Lifetime == 0f)
		{
			Lifetime = 1f;
		}
		AppShell.Instance.EventMgr.AddListener<LocalPlayerChangedMessage>(OnCharacterChanged);
	}

	public void OnEnable()
	{
	}

	public void OnDisable()
	{
		CameraShakeManager.Instance.RemoveShake(this);
		AppShell.Instance.EventMgr.RemoveListener<LocalPlayerChangedMessage>(OnCharacterChanged);
	}

	protected void OnCharacterChanged(LocalPlayerChangedMessage message)
	{
		localPlayer = message.localPlayer;
	}

	public void Update()
	{
		float num = Time.deltaTime;
		if (num < 0.0166666675f)
		{
			num = 0.0166666675f;
		}
		accumulatedTime += num;
		if (!EnableShaking)
		{
			shakeOffset = Vector3.zero;
			return;
		}
		float num2 = Strength;
		if (FalloffOverTime)
		{
			num2 *= 1f - accumulatedTime / Lifetime;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
		}
		if (FalloffOverDistance)
		{
			if (localPlayer != null)
			{
				effectDistance = (localPlayer.transform.position - base.transform.position).magnitude;
			}
			else if (DoNotShakeIfNoPlayer)
			{
				effectDistance = MaxDistance;
			}
			else
			{
				effectDistance = 0f;
			}
			num2 *= 1f - effectDistance / MaxDistance;
			if (num2 <= 0f)
			{
				shakeOffset = Vector3.zero;
				return;
			}
		}
		float num3 = num2 * num;
		shakeOffset = new Vector3(Random.Range(0f - num3, num3), Random.Range(0f - num3, num3), Random.Range(0f - num3, num3));
	}

	public void LateUpdate()
	{
		CameraShakeManager.Instance.Update();
	}
}
