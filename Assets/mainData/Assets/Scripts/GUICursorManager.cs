using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUICursorManager
{
	public enum CursorType
	{
		Normal,
		Attack,
		Interactable,
		Uninteractable,
		Click,
		None,
		DragDrop,
		Native
	}

	protected class Cursor
	{
		public GUICursor graphic;

		public GameObject sfxOnEnter;

		public GameObject sfxOnExit;

		public Cursor(GUICursor graphic)
		{
			this.graphic = graphic;
		}

		public Cursor(GUICursor graphic, string sfxOnEnter, string sfxOnExit)
		{
			this.graphic = graphic;
			AsyncLoadSFX(sfxOnEnter, sfxOnExit);
		}

		private void AsyncLoadSFX(string sfxOnEnter, string sfxOnExit)
		{
			GUIBundleManager bundleManager = GUIManager.Instance.BundleManager;
			if (bundleManager.IsBundleLoaded("cursor_bundle"))
			{
				if (sfxOnEnter != null)
				{
					bundleManager.LoadAsset("cursor_bundle", sfxOnEnter, null, delegate(UnityEngine.Object obj, AssetBundle bundle, object extraData)
					{
						this.sfxOnEnter = (obj as GameObject);
					});
				}
				if (sfxOnExit != null)
				{
					bundleManager.LoadAsset("cursor_bundle", sfxOnExit, null, delegate(UnityEngine.Object obj, AssetBundle bundle, object extraData)
					{
						this.sfxOnExit = (obj as GameObject);
					});
				}
			}
		}

		public void Enter(Cursor oldCursor)
		{
			if (oldCursor != this)
			{
				if (oldCursor != null && oldCursor.sfxOnExit != null)
				{
					ShsAudioSource.PlayAutoSound(oldCursor.sfxOnExit);
				}
				if (sfxOnEnter != null)
				{
					ShsAudioSource.PlayAutoSound(sfxOnEnter);
				}
			}
		}
	}

	private GUIManager manager;

	private bool initialized;

	private Hashtable customCursorMetrics = new Hashtable();

	protected Dictionary<CursorType, Cursor> cursors;

	protected Dictionary<CursorType, Cursor> cursorsDefault;

	protected GUICursor cursorCurrent;

	protected bool cursorEnabled = true;

	protected Vector2 cursorScale = new Vector2(1f, 1f);

	protected Vector2 cursorSize = new Vector2(128f, 128f);

	protected Vector2 dragDropCursorSize = new Vector2(-1f, -1f);

	private CursorType cursorCurrentType;

	private CursorType cachedCursorType;

	private bool haveACachedCursor;

	public GUICursor CursorCurrent
	{
		get
		{
			return cursorCurrent;
		}
	}

	public bool CursorEnabled
	{
		get
		{
			return cursorEnabled;
		}
		set
		{
			cursorEnabled = value;
		}
	}

	public Vector2 CursorScale
	{
		get
		{
			return cursorScale;
		}
		set
		{
			cursorScale = value;
		}
	}

	public Vector2 CursorSize
	{
		get
		{
			return cursorSize;
		}
		set
		{
			cursorSize = value;
		}
	}

	public Vector2 DragDropCursorSize
	{
		get
		{
			return dragDropCursorSize;
		}
		set
		{
			dragDropCursorSize = value;
		}
	}

	public CursorType CursorCurrentType
	{
		get
		{
			return cursorCurrentType;
		}
	}

	public GUICursorManager(GUIManager manager)
	{
		this.manager = manager;
	}

	public Vector2 CustomCursorMetrics(CursorType type)
	{
		if (customCursorMetrics.ContainsKey(type))
		{
			return (Vector2)customCursorMetrics[type];
		}
		return Vector2.zero;
	}

	public void InitializeCursor()
	{
		cursors = new Dictionary<CursorType, Cursor>();
		cursors[CursorType.Normal] = new Cursor(new GUICursor("cursor_bundle|Cursor0005", new Vector2(20f, 12f)));
		cursors[CursorType.Attack] = new Cursor(new GUICursor("cursor_bundle|Cursor0004", new Vector2(30f, 22f)));
		cursors[CursorType.Interactable] = new Cursor(new GUICursor("cursor_bundle|Cursor0001", new Vector2(18f, 12f)));
		cursors[CursorType.Uninteractable] = new Cursor(new GUICursor("cursor_bundle|Cursor0002", new Vector2(9f, 12f)));
		cursors[CursorType.Click] = new Cursor(new GUICursor("cursor_bundle|Cursor0003", new Vector2(23f, 26f)));
		cursors[CursorType.Native] = new Cursor(null);
		cursors[CursorType.None] = new Cursor(null);
		CursorScale = new Vector2(1f, 1f);
		CursorSize = new Vector2(128f, 128f);
		customCursorMetrics[CursorType.Normal] = new Vector2(48f, 43f);
		customCursorMetrics[CursorType.Interactable] = new Vector2(48f, 45f);
		customCursorMetrics[CursorType.Click] = new Vector2(48f, 45f);
		customCursorMetrics[CursorType.Uninteractable] = new Vector2(50f, 48f);
		cursorsDefault = new Dictionary<CursorType, Cursor>();
		foreach (KeyValuePair<CursorType, Cursor> cursor in cursors)
		{
			cursorsDefault[cursor.Key] = cursor.Value;
		}
		initialized = true;
		SetCursorType(CursorType.Normal);
	}

	public void UpdateCursor()
	{
		if (!cursorEnabled)
		{
			return;
		}
		CursorType cursorType = CursorCurrentType;
		if (IsOverClickableControl())
		{
			if (!haveACachedCursor)
			{
				cachedCursorType = CursorCurrentType;
				haveACachedCursor = true;
			}
			cursorType = CursorType.Interactable;
			if (IsOverDisabledControl())
			{
				cursorType = CursorType.Uninteractable;
			}
		}
		bool mouseButton = Input.GetMouseButton(0);
		if (cursorType == CursorType.Interactable && mouseButton)
		{
			cursorType = CursorType.Click;
		}
		if (cursorType == CursorType.Click && !mouseButton)
		{
			cursorType = CursorType.Interactable;
		}
		if (haveACachedCursor && !IsOverClickableControl())
		{
			haveACachedCursor = false;
			cursorType = cachedCursorType;
		}
		if (cursorType != CursorCurrentType)
		{
			internalSetCursorType(cursorType);
		}
		if (cursorCurrent == null)
		{
			return;
		}
		Vector3 point = SHSInput.mouseScreenPosition;
		if (!GUIManager.ScreenRect.Contains(point))
		{
			return;
		}
		if (GetCursorType() == CursorType.DragDrop)
		{
			Texture cursorImage = cursorCurrent.CursorImage;
			Vector2 vector = DragDropCursorSize;
			float num;
			if (vector.x != -1f)
			{
				Vector2 vector2 = DragDropCursorSize;
				num = vector2.x;
			}
			else
			{
				num = cursorImage.width;
			}
			float num2 = num;
			Vector2 vector3 = DragDropCursorSize;
			float num3;
			if (vector3.y != -1f)
			{
				Vector2 vector4 = DragDropCursorSize;
				num3 = vector4.y;
			}
			else
			{
				num3 = cursorImage.height;
			}
			float num4 = num3;
			Rect screenRect = new Rect(point.x - num2 / 2f, point.y - num4 / 2f, num2, num4);
			if (Event.current.type == EventType.Repaint)
			{
				Graphics.DrawTexture(screenRect, cursorImage);
			}
			return;
		}
		Texture cursorImage2 = cursorCurrent.CursorImage;
		if (!(cursorImage2 == null))
		{
			float x = point.x;
			float x2 = cursorCurrent.clickOffset.x;
			Vector2 vector5 = CursorScale;
			float left = x - x2 * vector5.x;
			float y = point.y;
			float y2 = cursorCurrent.clickOffset.y;
			Vector2 vector6 = CursorScale;
			float top = y - y2 * vector6.y;
			Vector2 vector7 = CursorSize;
			float x3 = vector7.x;
			Vector2 vector8 = CursorScale;
			float width = x3 * vector8.x;
			Vector2 vector9 = CursorSize;
			float y3 = vector9.y;
			Vector2 vector10 = CursorScale;
			Rect screenRect2 = new Rect(left, top, width, y3 * vector10.y);
			if (Event.current.type == EventType.Repaint)
			{
				Graphics.DrawTexture(screenRect2, cursorImage2);
			}
		}
	}

	private bool IsOverClickableControl()
	{
		return manager.CurrentOverControl != null && manager.CurrentOverControl is GUIControl && ((GUIControl)manager.CurrentOverControl).HasAClickOrMouseDownEvent() && GetCursorType() != CursorType.DragDrop;
	}

	private bool IsOverDisabledControl()
	{
		return manager.CurrentOverControl != null && manager.CurrentOverControl is GUIControl && !((GUIControl)manager.CurrentOverControl).IsEnabled;
	}

	private void SetCursorTypeCursorOver()
	{
		if (!haveACachedCursor)
		{
			cachedCursorType = CursorCurrentType;
		}
		internalSetCursorType(CursorType.Interactable);
		haveACachedCursor = true;
	}

	private void SetCursorTypeCursorDisabled()
	{
		if (!haveACachedCursor)
		{
			cachedCursorType = CursorCurrentType;
		}
		internalSetCursorType(CursorType.Uninteractable);
		haveACachedCursor = true;
	}

	private void RevertCursor()
	{
		haveACachedCursor = false;
		internalSetCursorType(cachedCursorType);
	}

	public void SetCursorType(CursorType type)
	{
		haveACachedCursor = false;
		internalSetCursorType(type);
	}

	private void internalSetCursorType(CursorType type)
	{
		//Discarded unreachable code: IL_008a
		if (initialized)
		{
			if (!AppShell.Instance.HasFocus)
			{
				type = CursorType.Native;
			}
			try
			{
				Cursor value = null;
				try
				{
					cursors.TryGetValue(cursorCurrentType, out value);
				}
				catch (Exception ex)
				{
					CspUtils.DebugLog("Can't get old cursor by type.");
					CspUtils.DebugLog("Cursors object, type, and oldCursor object: ");
					CspUtils.DebugLog(cursors);
					CspUtils.DebugLog(cursorCurrentType);
					CspUtils.DebugLog(value);
					throw ex;
				}
				cursorCurrentType = type;
				Screen.showCursor = (type == CursorType.Native);
				Cursor value2;
				if (cursors.TryGetValue(type, out value2))
				{
					if (value2 != null)
					{
						value2.Enter(value);
						cursorCurrent = value2.graphic;
					}
					else
					{
						CspUtils.DebugLog("New Cursor doesn't exist for application.");
					}
				}
				else
				{
					CspUtils.DebugLog("Missing cursor for type: " + type.ToString());
				}
			}
			catch (Exception ex2)
			{
				CspUtils.DebugLog("Cursor not configured yet for type setting. Error: " + ex2.Message);
			}
		}
	}

	public CursorType GetCursorType()
	{
		return cursorCurrentType;
	}

	public void SetCursorForDragDropMode(GUICursor cursor)
	{
		Cursor value;
		if (cursors.TryGetValue(CursorType.DragDrop, out value))
		{
			value.graphic = cursor;
		}
		else
		{
			cursors[CursorType.DragDrop] = new Cursor(cursor);
		}
	}
}
