using OrthographicHudEvents;
using UnityEngine;

public class PlayerPanelProperties : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum PanelHorizontalAlignment
	{
		Left,
		Center,
		Right
	}

	public enum PanelVerticalAlignment
	{
		Top,
		Center,
		Bottom
	}

	public const float kDepth = 2f;

	public PanelHorizontalAlignment horizontalAlignment = PanelHorizontalAlignment.Center;

	public PanelVerticalAlignment verticalAlignment = PanelVerticalAlignment.Center;

	public PlayerPanelProperties()
	{
		horizontalAlignment = PanelHorizontalAlignment.Center;
		verticalAlignment = PanelVerticalAlignment.Center;
	}

	public void Initialize(Camera cam)
	{
	}

	public void InitializePosition(Camera cam)
	{
		if (cam != null)
		{
			ChangeScreenPosition(cam.orthographicSize, cam.aspect);
		}
	}

	private void Start()
	{
		AppShell.Instance.EventMgr.AddListener<ScreenSizeChanged>(OnScreenSizeChanged);
	}

	private void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<ScreenSizeChanged>(OnScreenSizeChanged);
	}

	private void Update()
	{
	}

	private void ChangeScreenPosition(float orthographicSize, float aspectRatio)
	{
		float num = orthographicSize * 2f;
		float num2 = num * aspectRatio;
		Vector3 localPosition = new Vector3(0f, 0f, 2f);
		switch (horizontalAlignment)
		{
		case PanelHorizontalAlignment.Left:
			localPosition.x = (0f - num2) / 2f;
			break;
		case PanelHorizontalAlignment.Center:
			localPosition.x = 0f;
			break;
		case PanelHorizontalAlignment.Right:
			localPosition.x = num2 / 2f;
			break;
		}
		switch (verticalAlignment)
		{
		case PanelVerticalAlignment.Top:
			localPosition.y = num / 2f;
			break;
		case PanelVerticalAlignment.Center:
			localPosition.y = 0f;
			break;
		case PanelVerticalAlignment.Bottom:
			localPosition.y = (0f - num) / 2f;
			break;
		}
		base.transform.localPosition = localPosition;
	}

	private void OnScreenSizeChanged(ScreenSizeChanged evt)
	{
		ChangeScreenPosition(evt.orthoSize, evt.aspectRatio);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "CardPanelIcon.png");
	}
}
