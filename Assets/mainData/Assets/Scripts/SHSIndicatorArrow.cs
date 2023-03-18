using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SHSIndicatorArrow : GUIControlWindow, IDisposable
{
	private const float arrowSizeBig = 1f;

	private const float arrowSizeSmall = 0.25f;

	private const float arrowDistanceMax = 2f;

	protected GUIImage arrow;

	public GameObject arrowTarget;

	protected GameObject anchorObject;

	[CompilerGenerated]
	private bool _003CShowOnScreen_003Ek__BackingField;

	public bool ShowOnScreen
	{
		[CompilerGenerated]
		get
		{
			return _003CShowOnScreen_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CShowOnScreen_003Ek__BackingField = value;
		}
	}

	protected virtual bool ArrowVisible
	{
		get
		{
			return arrow.IsVisible;
		}
		set
		{
			arrow.IsVisible = value;
		}
	}

	public SHSIndicatorArrow()
	{
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		Traits.HitTestType = HitTestTypeEnum.Transparent;
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		IsEnabled = true;
		IsVisible = false;
		SetPositionAndSize(QuickSizingHint.ParentSize);
		arrow = GUIControl.CreateControlCenter<GUIImage>(new Vector2(64f, 64f), new Vector2(0f, 0f));
		arrow.TextureSource = "common_bundle|hud_indicator_arrow";
		arrow.Color = new Color(1f, 1f, 1f, 1f);
		arrow.IsVisible = true;
		Add(arrow);
		ShowOnScreen = true;
		AppShell.Instance.EventMgr.AddListener<LocalPlayerChangedMessage>(OnCharacterChanged);
	}

	void IDisposable.Dispose()
	{
		AppShell.Instance.EventMgr.RemoveListener<LocalPlayerChangedMessage>(OnCharacterChanged);
	}

	private void OnCharacterChanged(LocalPlayerChangedMessage message)
	{
		SetIndicatorAnchor(message.localPlayer);
	}

	public void SetIndicatorAnchor(GameObject newAnchor)
	{
		anchorObject = newAnchor;
	}

	public override void OnUpdate()
	{
		if (arrowTarget == null || anchorObject == null)
		{
			ArrowVisible = false;
		}
		else
		{
			Vector3 v = Camera.main.WorldToScreenPoint(arrowTarget.transform.position);
			if (v.x < 0f || v.y < 0f || v.x > Camera.main.pixelWidth || v.y > Camera.main.pixelHeight)
			{
				if (!ArrowVisible)
				{
					ArrowVisible = true;
				}
				Vector3 vector = Camera.main.WorldToViewportPoint(anchorObject.transform.position);
				vector.x = 0.5f;
				vector.y = 0.5f;
				Vector3 vector2 = Camera.main.ViewportToWorldPoint(vector);
				Vector3 b = arrowTarget.transform.position - vector2;
				b.Normalize();
				b = Camera.main.WorldToViewportPoint(vector2 + b) - vector;
				b.z = 0f;
				b.Normalize();
				b.y = 0f - b.y;
				arrow.Rotation = (Mathf.Atan2(b.y, b.x) * 57.29578f + 360f) % 360f;
				float num = 64f;
				float num2 = Camera.main.pixelWidth - num;
				float num3 = Camera.main.pixelHeight - num;
				if (arrow.Rotation < 45f || arrow.Rotation >= 315f)
				{
					float num4 = (arrow.Rotation + 45f) % 360f;
					v.x = num2;
					v.y = num4 / 90f * num3;
				}
				if (arrow.Rotation >= 45f && arrow.Rotation < 135f)
				{
					float num5 = arrow.Rotation - 45f;
					v.x = (1f - num5 / 90f) * num2;
					v.y = num3;
				}
				if (arrow.Rotation >= 135f && arrow.Rotation < 225f)
				{
					float num6 = arrow.Rotation - 135f;
					v.x = num;
					v.y = (1f - num6 / 90f) * num3;
				}
				if (arrow.Rotation >= 225f && arrow.Rotation < 315f)
				{
					float num7 = arrow.Rotation - 225f;
					v.x = num7 / 90f * num2;
					v.y = num;
				}
				SetArrowSize(64f, 64f);
				SetArrowPosition(v);
			}
			else if (ShowOnScreen)
			{
				if (!ArrowVisible)
				{
					ArrowVisible = true;
				}
				float num8 = Mathf.Max(v.z, 1f);
				num8 = Mathf.Min(16f / num8, 1f) * 64f;
				SetArrowSize(num8, num8);
				Vector2 arrowPosition = new Vector2(v.x, Camera.main.pixelHeight - v.y);
				arrowPosition.x -= num8 * 0.5f;
				arrowPosition.y -= 160f * num8 / 64f;
				arrowPosition.x = Mathf.Max(arrowPosition.x, num8 * 0.5f);
				arrowPosition.x = Mathf.Min(arrowPosition.x, Camera.main.pixelWidth - num8 * 0.5f);
				arrowPosition.y = Mathf.Max(arrowPosition.y, num8 * 0.5f);
				arrowPosition.y = Mathf.Min(arrowPosition.y, Camera.main.pixelHeight - num8 * 0.5f);
				arrow.Rotation = 90f;
				SetArrowPosition(arrowPosition);
			}
			else if (ArrowVisible)
			{
				ArrowVisible = false;
			}
		}
		base.OnUpdate();
	}

	protected virtual void SetArrowSize(float width, float height)
	{
		arrow.SetSize(width, height);
	}

	protected virtual void SetArrowPosition(Vector2 screenPos)
	{
		arrow.Offset = screenPos;
	}
}
