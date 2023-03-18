using System.Collections.Generic;
using UnityEngine;

public class DebugFlyCam2 : MonoBehaviour, ICaptureHandler, ICaptureManager, IInputHandler
{
	public static DebugFlyCam2 _current;

	public static int index = 1;

	private KeyBank? _myKeyBank;

	private Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> _activeKeyDictionary;

	private CameraLiteManager _cameraManager;

	private float _oldFOV;

	private float _oldDOFRange;

	private float _oldDOFTarget;

	private bool _paused;

	private bool _goSlow;

	private ICaptureManager manager;

	private static Vector3 queuedMoveDir;

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
			_current.HookKeys(false);
			Object.Destroy(_current.gameObject);
			_current = null;
		}
		else
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "DebugFlyCam" + index++;
			_current = Utils.AddComponent<DebugFlyCam2>(gameObject);
			_current.HookKeys(true);
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
		AOSceneCamera aOSceneCamera = Object.FindObjectOfType(typeof(AOSceneCamera)) as AOSceneCamera;
		if (aOSceneCamera != null)
		{
			aOSceneCamera.DOFRange = _oldDOFRange;
			aOSceneCamera.DOFTarget = _oldDOFTarget;
		}
		Camera.main.fov = _oldFOV;
		CspUtils.DebugLog("OnDisable");
	}

	private void Update()
	{
		if (SHSInput.GetKeyDown(KeyCode.Space, this))
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
		float num = (!_goSlow) ? 10f : 1f;
		transform.position += queuedMoveDir * (num * Time.fixedDeltaTime);
		queuedMoveDir = Vector3.zero;
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
			SHSInput.SetInputBlockingMode(this, SHSInput.InputBlockType.CaptureMode);
			SHSInput.AddCaptureHandler(this, this);
			SHSInput.RegisterListener(_myKeyBank.Value, true);
			SHSDebugInput.Inst.DeactivateDebugKeys();
		}
		else
		{
			SHSInput.RevertInputBlockingMode(this);
			SHSInput.UnregisterListener(_myKeyBank.Value);
			SHSDebugInput.Inst.ActivateDebugKeys();
		}
	}

	public CaptureHandlerResponse HandleCapture(SHSKeyCode keycode)
	{
		CaptureHandlerResponse result = (keycode.source == this) ? CaptureHandlerResponse.Passthrough : CaptureHandlerResponse.Block;
		if (keycode.source == this && keycode.code == KeyCode.W)
		{
			manager.CaptureHandlerGotInput(this);
		}
		return result;
	}

	public void OnCaptureAcquired()
	{
		CspUtils.DebugLog("Capture acquired");
	}

	public void OnCaptureUnacquired()
	{
		CspUtils.DebugLog("Capture unacquired.");
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
				KeyCode.F2
			};
			KeyCode[] array2 = array;
			foreach (KeyCode keyCode in array2)
			{
				if (keyCode == KeyCode.F2)
				{
					_activeKeyDictionary.Add(new KeyCodeEntry(keyCode, true, true, true, true), OnKeyEvent);
					continue;
				}
				_activeKeyDictionary.Add(new KeyCodeEntry(keyCode, false, false, false, true), OnKeyEvent);
				_activeKeyDictionary.Add(new KeyCodeEntry(keyCode, false, false, true, true), OnKeyEvent);
			}
		}
		return _activeKeyDictionary;
	}

	private void OnKeyEvent(SHSKeyCode code)
	{
		Transform transform = Camera.main.transform;
		if (code.code == KeyCode.W)
		{
			queuedMoveDir += transform.forward;
		}
		if (code.code == KeyCode.S)
		{
			queuedMoveDir -= transform.forward;
		}
		if (code.code == KeyCode.A)
		{
			Vector3 a = queuedMoveDir;
			Vector3 right = transform.right;
			float x = right.x;
			Vector3 right2 = transform.right;
			Vector3 vector = new Vector3(x, 0f, right2.z);
			queuedMoveDir = a - vector.normalized;
		}
		if (code.code == KeyCode.D)
		{
			Vector3 a2 = queuedMoveDir;
			Vector3 right3 = transform.right;
			float x2 = right3.x;
			Vector3 right4 = transform.right;
			Vector3 vector2 = new Vector3(x2, 0f, right4.z);
			queuedMoveDir = a2 + vector2.normalized;
		}
		if (code.code == KeyCode.Q)
		{
			queuedMoveDir -= Vector3.up;
		}
		if (code.code == KeyCode.E)
		{
			queuedMoveDir += Vector3.up;
		}
		if (code.code == KeyCode.F2)
		{
			Toggle();
		}
	}

	public void ConfigureKeyBanks()
	{
	}

	public bool IsDescendantHandler(IInputHandler handler)
	{
		return false;
	}

	public void CaptureHandlerGotInput(ICaptureHandler handler)
	{
		if (!(handler is DebugFlyCam2))
		{
		}
	}
}
