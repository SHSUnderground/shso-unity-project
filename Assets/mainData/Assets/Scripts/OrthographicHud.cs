using OrthographicHudEvents;
using UnityEngine;

public class OrthographicHud : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Camera Camera;

	public int minHeight = 644;

	public static readonly float kAspectScalar = 0.002419f;

	protected AnimClipManager AnimationClipManager;

	private float screenWidth;

	private float screenHeight;

	private float aspectRatio;

	public void Awake()
	{
		AnimationClipManager = new AnimClipManager();
	}

	public void Update()
	{
		AnimationClipManager.Update(Time.deltaTime);
		if (Camera.aspect != aspectRatio || (float)Screen.width != screenWidth || (float)Screen.height != screenHeight)
		{
			UpdateScreenSize();
		}
	}

	public void UpdateScreenSize()
	{
		aspectRatio = Camera.aspect;
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		float num = (!(screenHeight >= (float)minHeight)) ? ((float)minHeight) : screenHeight;
		Camera.orthographicSize = num * kAspectScalar / 2f;
		AppShell.Instance.EventMgr.Fire(this, new ScreenSizeChanged(Screen.width, Screen.height, Camera.aspect, Camera.orthographicSize));
	}

	public void AttachToHud(GameObject obj)
	{
		if (obj == null)
		{
			CspUtils.DebugLog("OrthographicHud::AttachToHud() - trying to attach null object");
			return;
		}
		Utils.AttachGameObject(Camera.gameObject, obj);
		PlayerPanelProperties component = Utils.GetComponent<PlayerPanelProperties>(obj);
		if (component != null)
		{
			component.Initialize(Camera);
			component.InitializePosition(Camera);
		}
		else
		{
			CspUtils.DebugLog("OrthographicHud::AttachToHud() - attached object <" + obj.name + "> does not have player panel properties and will not have proper positioning");
		}
	}
}
