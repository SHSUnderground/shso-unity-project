using System;
using UnityEngine;

public class SHSScanlineTransitionWindow<Key> : GUISimpleControlWindow where Key : class, IComparable<Key>
{
	private GUISimpleControlWindow growthWindow;

	private GUISimpleControlWindow crushedWindow;

	private GUIImage scanline;

	private Vector2 scanlineTransitionWindowSize;

	private Vector2 windowSize;

	private float scanTotalTime;

	private static readonly Vector2 defaultScanlineWindowSize = new Vector2(520f, 520f);

	private static readonly float defaultScanlineTime = 0.65f;

	private Func<Key, GUIWindow> getWindow;

	private Key KeySelected;

	private AnimClip runningAnimation;

	private Key QueuedKeySelect;

	public GUIWindow CurrentlySelectedWindow;

	public static Vector2 DefaultScanlineWindowSize
	{
		get
		{
			return defaultScanlineWindowSize;
		}
	}

	public static float DefaultScanlineTime
	{
		get
		{
			return defaultScanlineTime;
		}
	}

	public SHSScanlineTransitionWindow(Func<Key, GUIWindow> getWindow)
		: this(new Vector2(520f, 520f), new Vector2(520f, 520f), 0.65f, getWindow)
	{
	}

	public SHSScanlineTransitionWindow(Vector2 WindowSize, Vector2 scanlineWindowSize, float scanTotalTime, Func<Key, GUIWindow> getWindow)
	{
		scanlineTransitionWindowSize = scanlineWindowSize;
		windowSize = WindowSize;
		this.scanTotalTime = scanTotalTime;
		this.getWindow = getWindow;
		SetSize(WindowSize);
		growthWindow = new GUISimpleControlWindow();
		SetupStartGrowthWindow(growthWindow);
		Add(growthWindow);
		crushedWindow = new GUISimpleControlWindow();
		SetupStartCrushWindow(crushedWindow);
		Add(crushedWindow);
		scanline = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(459f, 30f), Vector2.zero);
		scanline.TextureSource = "persistent_bundle|gadget_scanline";
		scanline.IsVisible = false;
		Add(scanline);
	}

	public void InitialSetup(Key KeyName)
	{
		CurrentlySelectedWindow = getWindow.Invoke(KeyName);
		CurrentlySelectedWindow.SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft);
		CurrentlySelectedWindow.SetSize(windowSize);
		growthWindow.Add(CurrentlySelectedWindow);
		KeySelected = KeyName;
		Remove(crushedWindow);
		crushedWindow = growthWindow;
		growthWindow = new GUISimpleControlWindow();
		Add(growthWindow, DrawOrder.DrawLast);
		SetupStartCrushWindow(crushedWindow);
		SetupStartGrowthWindow(growthWindow);
		scanline.IsVisible = false;
	}

	public void BeginTransition(Key KeyName)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Expected O, but got Unknown
		if (KeySelected.CompareTo(KeyName) == 0)
		{
			return;
		}
		if (runningAnimation != null)
		{
			if (QueuedKeySelect == null)
			{
				QueuedKeySelect = KeyName;
				runningAnimation.OnFinished += (Action)(object)(Action)delegate
				{
					BeginTransition(QueuedKeySelect);
					QueuedKeySelect = (Key)null;
				};
			}
			else
			{
				QueuedKeySelect = KeyName;
			}
			return;
		}
		KeySelected = KeyName;
		CurrentlySelectedWindow = getWindow.Invoke(KeyName);
		CurrentlySelectedWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		CurrentlySelectedWindow.SetSize(windowSize);
		growthWindow.Add(CurrentlySelectedWindow);
		CurrentlySelectedWindow.IsVisible = true;
		scanline.Offset = new Vector2(0f, 0f);
		scanline.IsVisible = true;
		AnimClip animClip = AnimClipBuilder.Absolute.SizeY(AnimClipBuilder.Path.Linear(0f, windowSize.y, scanTotalTime), growthWindow) ^ AnimClipBuilder.Absolute.SizeY(AnimClipBuilder.Path.Linear(windowSize.y, 0f, scanTotalTime), crushedWindow) ^ AnimClipBuilder.Absolute.OffsetY(AnimClipBuilder.Path.Linear(-15f, scanlineTransitionWindowSize.y - 15f, scanTotalTime), scanline);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			Remove(crushedWindow);
			crushedWindow = growthWindow;
			growthWindow = new GUISimpleControlWindow();
			Add(growthWindow, DrawOrder.DrawLast);
			SetupStartCrushWindow(crushedWindow);
			SetupStartGrowthWindow(growthWindow);
			scanline.IsVisible = false;
			CurrentlySelectedWindow.SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft);
			runningAnimation = null;
		};
		runningAnimation = animClip;
		base.AnimationPieceManager.Add(animClip);
	}

	private void SetupStartGrowthWindow(GUIWindow growthWindow)
	{
		growthWindow.SetSize(new Vector2(windowSize.x, 0f));
		growthWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
	}

	private void SetupStartCrushWindow(GUIWindow crushWindow)
	{
		crushWindow.SetSize(windowSize);
		crushWindow.SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft);
	}
}
