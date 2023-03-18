using System.Collections.Generic;
using UnityEngine;

public class DebugFlyCam : MonoBehaviour, ICaptureHandler, IInputHandler
{
	public static DebugFlyCam _current;

	private static KeyBank? _myKeyBank;

	private static Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> _activeKeyDictionary;

	private CameraLiteManager _cameraManager;

	private ICaptureManager manager;

	private float _oldFOV;

	private float _oldDOFRange;

	private float _oldDOFTarget;

	private bool _paused;

	private bool _goSlow;

	public float sensitivityX = 10f;

	public float sensitivityY = 10f;

	public float minimumX = -360f;

	public float maximumX = 360f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	private float rotationX;

	private float rotationY;

	public ICaptureManager Manager
	{
		get
		{
			return manager;
		}
		set
		{
			manager = value;
		}
	}

	public SHSInput.InputRequestorType InputRequestorType
	{
		get
		{
			return SHSInput.InputRequestorType.World;
		}
	}

	public bool CanHandleInput
	{
		get
		{
			return true;
		}
	}

	public static void Toggle()
	{
		if (_current != null)
		{
			Object.Destroy(_current.gameObject);
			_current = null;
		}
		else
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "DebugFlyCam";
			_current = Utils.AddComponent<DebugFlyCam>(gameObject);
		}
	}

	private void OnEnable()
	{
		_cameraManager = CameraLiteManager.Instance;
		if (_cameraManager != null)
		{
			if (!_cameraManager.enabled)
			{
				_cameraManager = null;
			}
			else
			{
				_cameraManager.enabled = false;
			}
		}
		KeyBank? myKeyBank = _myKeyBank;
		if (!myKeyBank.HasValue)
		{
			_myKeyBank = new KeyBank(this, GUIControl.KeyInputState.Active, KeyBank.KeyBankTypeEnum.Additive, GetKeyList(GUIControl.KeyInputState.Active));
		}
		HookKeys(true);
		Quaternion rotation = Camera.main.transform.rotation;
		Vector3 eulerAngles = rotation.eulerAngles;
		rotationX = eulerAngles.y;
		Vector3 eulerAngles2 = rotation.eulerAngles;
		rotationY = eulerAngles2.x;
		AOSceneCamera aOSceneCamera = Object.FindObjectOfType(typeof(AOSceneCamera)) as AOSceneCamera;
		if (aOSceneCamera != null)
		{
			_oldDOFRange = aOSceneCamera.DOFRange;
			_oldDOFTarget = aOSceneCamera.DOFTarget;
		}
		_oldFOV = Camera.main.fov;
	}

	private void OnDisable()
	{
		if (_cameraManager != null)
		{
			_cameraManager.enabled = true;
		}
		if (!_paused)
		{
			HookKeys(false);
		}
		AOSceneCamera aOSceneCamera = Object.FindObjectOfType(typeof(AOSceneCamera)) as AOSceneCamera;
		if (aOSceneCamera != null)
		{
			aOSceneCamera.DOFRange = _oldDOFRange;
			aOSceneCamera.DOFTarget = _oldDOFTarget;
		}
		Camera.main.fov = _oldFOV;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			_paused = !_paused;
			HookKeys(!_paused);
		}
		if (_paused)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
		{
			_goSlow = !_goSlow;
		}
		Transform transform = Camera.main.transform;
		rotationX += Input.GetAxis("Mouse X") * sensitivityX * ((!_goSlow) ? 1f : 0.1f);
		rotationY += Input.GetAxis("Mouse Y") * (0f - sensitivityY) * ((!_goSlow) ? 1f : 0.1f);
		rotationX = ClampAngle(rotationX, minimumX, maximumX);
		rotationY = ClampAngle(rotationY, minimumY, maximumY);
		transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);
		Vector3 vector = default(Vector3);
		if (Input.GetKey(KeyCode.W))
		{
			vector += transform.forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			vector -= transform.forward;
		}
		if (Input.GetKey(KeyCode.A))
		{
			Vector3 a = vector;
			Vector3 right = transform.right;
			float x = right.x;
			Vector3 right2 = transform.right;
			Vector3 vector2 = new Vector3(x, 0f, right2.z);
			vector = a - vector2.normalized;
		}
		if (Input.GetKey(KeyCode.D))
		{
			Vector3 a2 = vector;
			Vector3 right3 = transform.right;
			float x2 = right3.x;
			Vector3 right4 = transform.right;
			Vector3 vector3 = new Vector3(x2, 0f, right4.z);
			vector = a2 + vector3.normalized;
		}
		if (Input.GetKey(KeyCode.Q))
		{
			vector -= Vector3.up;
		}
		if (Input.GetKey(KeyCode.E))
		{
			vector += Vector3.up;
		}
		float num = (!_goSlow) ? 10f : 1f;
		transform.position += vector * (num * Time.deltaTime);
		float num2 = Input.GetAxis("Mouse ScrollWheel");
		if (num2 == 0f)
		{
			return;
		}
		bool flag = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		if (flag)
		{
			num2 *= -1f;
		}
		float num3 = Mathf.Abs(num2 * 10f) * 1.1f;
		float num4 = (!(num2 >= 0f)) ? (1f / num3) : num3;
		if (flag)
		{
			Camera.main.fov *= num4;
			return;
		}
		AOSceneCamera aOSceneCamera = Object.FindObjectOfType(typeof(AOSceneCamera)) as AOSceneCamera;
		if (aOSceneCamera != null)
		{
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				aOSceneCamera.DOFRange *= num4;
			}
			else
			{
				aOSceneCamera.DOFTarget *= num4;
			}
			CspUtils.DebugLog(string.Format("DOF Target: {0}    DOF Range: {1}", aOSceneCamera.DOFTarget, aOSceneCamera.DOFRange));
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	private void HookKeys(bool hook)
	{
		if (hook)
		{
			SHSInput.RegisterListener(_myKeyBank.Value, true);
			SHSDebugInput.Inst.DeactivateDebugKeys();
		}
		else
		{
			SHSInput.UnregisterListener(_myKeyBank.Value);
			SHSDebugInput.Inst.ActivateDebugKeys();
		}
	}

	public CaptureHandlerResponse HandleCapture(SHSKeyCode keycode)
	{
		if (_paused)
		{
			return (keycode.code != KeyCode.Space) ? CaptureHandlerResponse.Passthrough : CaptureHandlerResponse.Block;
		}
		return (!_activeKeyDictionary.ContainsKey(new KeyCodeEntry(keycode.code, false, false, false))) ? CaptureHandlerResponse.Passthrough : CaptureHandlerResponse.Block;
	}

	public void OnCaptureAcquired()
	{
	}

	public void OnCaptureUnacquired()
	{
	}

	public Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> GetKeyList(GUIControl.KeyInputState inputState)
	{
		if (inputState != 0)
		{
			return new Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate>();
		}
		if (_activeKeyDictionary == null)
		{
			_activeKeyDictionary = new Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate>();
			KeyCode[] array = new KeyCode[7]
			{
				KeyCode.W,
				KeyCode.S,
				KeyCode.A,
				KeyCode.D,
				KeyCode.Q,
				KeyCode.E,
				KeyCode.Space
			};
			KeyCode[] array2 = array;
			foreach (KeyCode code in array2)
			{
				_activeKeyDictionary.Add(new KeyCodeEntry(code, false, false, false), delegate
				{
				});
				_activeKeyDictionary.Add(new KeyCodeEntry(code, false, false, true), delegate
				{
				});
			}
		}
		return _activeKeyDictionary;
	}

	public void ConfigureKeyBanks()
	{
	}

	public bool IsDescendantHandler(IInputHandler handler)
	{
		return false;
	}
}
