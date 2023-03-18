using UnityEngine;

public class GUIVisualCueWindow : GUINotificationWindow
{
	public enum AttachedStateEnum
	{
		GameObject,
		GUIControl,
		None
	}

	private GUIControl linkedControl;

	private bool autopositionAndSizeOverriden;

	private GameObject linkedGameObject;

	private DockingAlignmentEnum linkDockingPoint;

	private AnchorAlignmentEnum linkAnchorPoint;

	private AttachedStateEnum attachedState = AttachedStateEnum.None;

	public GUIControl LinkedControl
	{
		get
		{
			return linkedControl;
		}
		set
		{
			resetLinkage();
			linkedControl = value;
			if (linkedControl == null)
			{
				attachedState = AttachedStateEnum.None;
				return;
			}
			attachedState = AttachedStateEnum.GUIControl;
			linkedControl.Resize += linkedControl_OnResize;
			linkedControl.OnVisible += linkedControl_OnVisible;
			linkedControl.OnHidden += linkedControl_OnHidden;
			linkedControl_OnResize(value, null);
		}
	}

	public GameObject LinkedGameObject
	{
		get
		{
			return linkedGameObject;
		}
		set
		{
			resetLinkage();
			linkedGameObject = value;
			if (linkedGameObject == null)
			{
				attachedState = AttachedStateEnum.None;
			}
			else
			{
				attachedState = AttachedStateEnum.GameObject;
			}
		}
	}

	public DockingAlignmentEnum LinkDockingPoint
	{
		get
		{
			return linkDockingPoint;
		}
		set
		{
			linkDockingPoint = value;
			if (attachedState == AttachedStateEnum.GUIControl)
			{
				linkedControl_OnResize(linkedControl, null);
			}
		}
	}

	public AnchorAlignmentEnum LinkAnchorPoint
	{
		get
		{
			return linkAnchorPoint;
		}
		set
		{
			linkAnchorPoint = value;
			if (attachedState == AttachedStateEnum.GUIControl)
			{
				linkedControl_OnResize(linkedControl, null);
			}
		}
	}

	public AttachedStateEnum AttachedState
	{
		get
		{
			return attachedState;
		}
	}

	public override Vector2 Offset
	{
		set
		{
			base.Offset = value;
			directRectMode = true;
			linkedControl_OnResize(linkedControl, null);
		}
	}

	public GUIVisualCueWindow()
	{
		base.Docking = DockingAlignmentEnum.None;
		base.Anchor = AnchorAlignmentEnum.None;
		position = new Vector2(0f, 0f);
		directRectMode = true;
		Traits.LifeSpan = ControlTraits.LifeSpanTraitEnum.DestroyOnHide;
	}

	private void linkedControl_OnVisible(GUIControl sender)
	{
		Show();
	}

	private void linkedControl_OnHidden(GUIControl sender)
	{
		Hide();
	}

	public void Link(GUIControl control, DockingAlignmentEnum linkDockingPoint, AnchorAlignmentEnum linkAnchorPoint, Vector2 offset)
	{
		this.linkDockingPoint = linkDockingPoint;
		this.linkAnchorPoint = linkAnchorPoint;
		base.offset = offset;
		LinkedControl = control;
	}

	public void Unlink()
	{
		LinkedControl = null;
	}

	public override void OnUpdate()
	{
		if (!autopositionAndSizeOverriden)
		{
			switch (attachedState)
			{
			case AttachedStateEnum.GameObject:
				updateGOState();
				break;
			case AttachedStateEnum.GUIControl:
				updateGUIState();
				break;
			}
			base.OnUpdate();
		}
	}

	private void updateGOState()
	{
	}

	private void updateGUIState()
	{
	}

	private void resetLinkage()
	{
		switch (attachedState)
		{
		case AttachedStateEnum.GUIControl:
			linkedControl.Resize -= linkedControl_OnResize;
			break;
		}
		autopositionAndSizeOverriden = false;
		autoPosition = false;
		autoSize = false;
		base.Docking = DockingAlignmentEnum.None;
		base.Anchor = AnchorAlignmentEnum.None;
		directRectMode = true;
	}

	private void linkedControl_OnResize(GUIControl control, GUIResizeMessage message)
	{
		if (linkedControl == null)
		{
			CspUtils.DebugLog("Asked to resize and reposition against a linked control, but no linked control exists.");
			return;
		}
		Rect screenRect = linkedControl.ScreenRect;
		setLinkedPosition(screenRect);
	}

	private void setLinkedPosition(Rect linkRect)
	{
		float x = base.rect.x;
		float y = base.rect.y;
		x = ((linkDockingPoint != 0 && linkDockingPoint != DockingAlignmentEnum.MiddleLeft && linkDockingPoint != DockingAlignmentEnum.BottomLeft) ? ((linkDockingPoint != DockingAlignmentEnum.Middle && linkDockingPoint != DockingAlignmentEnum.TopMiddle && linkDockingPoint != DockingAlignmentEnum.BottomMiddle) ? ((linkDockingPoint != DockingAlignmentEnum.TopRight && linkDockingPoint != DockingAlignmentEnum.MiddleRight && linkDockingPoint != DockingAlignmentEnum.BottomRight) ? x : (linkRect.x + linkRect.width)) : (linkRect.x + linkRect.width / 2f)) : linkRect.x);
		y = ((linkDockingPoint != 0 && linkDockingPoint != DockingAlignmentEnum.TopMiddle && linkDockingPoint != DockingAlignmentEnum.TopRight) ? ((linkDockingPoint != DockingAlignmentEnum.Middle && linkDockingPoint != DockingAlignmentEnum.MiddleLeft && linkDockingPoint != DockingAlignmentEnum.MiddleRight) ? ((linkDockingPoint != DockingAlignmentEnum.BottomRight && linkDockingPoint != DockingAlignmentEnum.BottomMiddle && linkDockingPoint != DockingAlignmentEnum.BottomLeft) ? position.y : (linkRect.y + linkRect.height)) : (linkRect.y + linkRect.height / 2f)) : linkRect.y);
		float num = x;
		float num2;
		if (linkAnchorPoint == AnchorAlignmentEnum.TopLeft || linkAnchorPoint == AnchorAlignmentEnum.MiddleLeft || linkAnchorPoint == AnchorAlignmentEnum.BottomLeft)
		{
			num2 = 0f;
		}
		else if (linkAnchorPoint == AnchorAlignmentEnum.Middle || linkAnchorPoint == AnchorAlignmentEnum.TopMiddle || linkAnchorPoint == AnchorAlignmentEnum.BottomMiddle)
		{
			Vector2 rectSize = base.RectSize;
			num2 = rectSize.x / 2f;
		}
		else if (linkAnchorPoint == AnchorAlignmentEnum.TopRight || linkAnchorPoint == AnchorAlignmentEnum.MiddleRight || linkAnchorPoint == AnchorAlignmentEnum.BottomRight)
		{
			Vector2 rectSize2 = base.RectSize;
			num2 = rectSize2.x;
		}
		else
		{
			num2 = 0f;
		}
		x = num - num2;
		float num3 = y;
		float num4;
		if (linkAnchorPoint == AnchorAlignmentEnum.TopLeft || linkAnchorPoint == AnchorAlignmentEnum.TopMiddle || linkAnchorPoint == AnchorAlignmentEnum.TopRight)
		{
			num4 = 0f;
		}
		else if (linkAnchorPoint == AnchorAlignmentEnum.Middle || linkAnchorPoint == AnchorAlignmentEnum.MiddleLeft || linkAnchorPoint == AnchorAlignmentEnum.MiddleRight)
		{
			Vector2 rectSize3 = base.RectSize;
			num4 = rectSize3.y / 2f;
		}
		else if (linkAnchorPoint == AnchorAlignmentEnum.BottomRight || linkAnchorPoint == AnchorAlignmentEnum.BottomMiddle || linkAnchorPoint == AnchorAlignmentEnum.BottomLeft)
		{
			Vector2 rectSize4 = base.RectSize;
			num4 = rectSize4.y;
		}
		else
		{
			num4 = 0f;
		}
		y = num3 - num4;
		if (base.OffsetStyle == OffsetType.Percentage)
		{
			x += linkRect.width * Offset[0];
			y += linkRect.height * Offset[1];
		}
		else if (base.OffsetStyle == OffsetType.Absolute)
		{
			x += Offset[0];
			y += Offset[1];
		}
		base.rect = new Rect(x, y, base.rect.width, base.rect.height);
	}

	public override void SetPositionAndSize(DockingAlignmentEnum Docking, AnchorAlignmentEnum Anchor, OffsetType OffsetType, Vector2 Offset, Vector2 Size, AutoSizeTypeEnum HorizontalSizeHint, AutoSizeTypeEnum VerticalSizeHint)
	{
		resetLinkage();
		base.SetPositionAndSize(Docking, Anchor, OffsetType, Offset, Size, HorizontalSizeHint, VerticalSizeHint);
		autopositionAndSizeOverriden = true;
		directRectMode = false;
	}

	public override void SetSize(float width, float height)
	{
		base.SetSize(width, height);
		directRectMode = true;
		if (attachedState == AttachedStateEnum.GUIControl)
		{
			linkedControl_OnResize(linkedControl, null);
		}
	}

	public override void SetSize(Vector2 Size)
	{
		base.SetSize(Size);
		directRectMode = true;
		if (attachedState == AttachedStateEnum.GUIControl)
		{
			linkedControl_OnResize(linkedControl, null);
		}
	}
}
