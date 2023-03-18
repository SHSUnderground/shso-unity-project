using System.Collections;
using UnityEngine;

public class GUINotificationWindow : GUIChildWindow
{
	protected class NotificationBackground
	{
		public enum PieceType
		{
			LeftPiece,
			MiddlePiece,
			RightPiece
		}

		private const int pieceCount = 3;

		private GUIImage[] backgroundPieces = new GUIImage[3];

		public void Build(float width)
		{
			backgroundPieces[0] = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(64f, 128f), Vector2.zero);
			backgroundPieces[0].SetPosition(new Vector2(0f, 0f));
			backgroundPieces[0].TextureSource = "notification_bundle|achievement_frame_left";
			backgroundPieces[1] = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(width, 128f), Vector2.zero);
			GUIImage obj = backgroundPieces[1];
			Vector2 position = backgroundPieces[0].Position;
			float x = position.x;
			Vector2 size = backgroundPieces[0].Size;
			float x2 = x + size.x;
			Vector2 position2 = backgroundPieces[0].Position;
			obj.SetPosition(new Vector2(x2, position2.y));
			backgroundPieces[1].TextureSource = "notification_bundle|achievement_frame_center";
			backgroundPieces[2] = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(32f, 128f), Vector2.zero);
			GUIImage obj2 = backgroundPieces[2];
			Vector2 position3 = backgroundPieces[1].Position;
			float x3 = position3.x;
			Vector2 size2 = backgroundPieces[1].Size;
			float x4 = x3 + size2.x;
			Vector2 position4 = backgroundPieces[1].Position;
			obj2.SetPosition(new Vector2(x4, position4.y));
			backgroundPieces[2].TextureSource = "notification_bundle|achievement_frame_right";
		}

		public GUIImage GetBackgroundPiece(PieceType type)
		{
			return backgroundPieces[(int)type];
		}

		public void Dispose()
		{
			backgroundPieces[0] = null;
			backgroundPieces[1] = null;
			backgroundPieces[2] = null;
			backgroundPieces = null;
		}
	}

	protected class NotificationWindowHandler
	{
		private Hashtable activeWindowsLookup = new Hashtable();

		public void AddWindowToHandle(GUINotificationWindow window, GUINotificationManager.GUINotificationStyleEnum windowStyle)
		{
			if (activeWindowsLookup.ContainsKey(windowStyle))
			{
				activeWindowsLookup[windowStyle] = window;
			}
			else
			{
				activeWindowsLookup.Add(windowStyle, window);
			}
		}

		public void ClearWindowOfType(GUINotificationManager.GUINotificationStyleEnum windowStyle)
		{
			activeWindowsLookup.Remove(windowStyle);
		}

		public bool IsTypeActive(GUINotificationManager.GUINotificationStyleEnum windowStyle)
		{
			if (activeWindowsLookup.ContainsKey(windowStyle))
			{
				GUINotificationWindow gUINotificationWindow = (GUINotificationWindow)activeWindowsLookup[windowStyle];
				if (gUINotificationWindow.IsActive)
				{
					return true;
				}
			}
			return false;
		}

		public GUINotificationWindow GetWindow(GUINotificationManager.GUINotificationStyleEnum windowStyle)
		{
			if (activeWindowsLookup.ContainsKey(windowStyle))
			{
				return (GUINotificationWindow)activeWindowsLookup[windowStyle];
			}
			return null;
		}
	}

	protected class SlotManager
	{
		private const int MaxSlots = 2;

		private const float itemSectorOffset = 200f;

		private bool[] occupiedSlots = new bool[2];

		private float[] occupiedOffsets = new float[2];

		private GUINotificationWindow[] windowRefs = new GUINotificationWindow[2];

		public int AssignSlot(GUINotificationManager.GUINotificationStyleEnum windowStyle, GUINotificationWindow window)
		{
			int num = 0;
			if (WindowHandler.IsTypeActive(windowStyle))
			{
				GUINotificationWindow window2;
				if ((window2 = WindowHandler.GetWindow(windowStyle)) != null)
				{
					num = window2.OccupiedSlot;
					window2.Hide();
					windowRefs[num] = window;
					occupiedSlots[num] = true;
				}
			}
			else
			{
				bool flag = false;
				for (int i = 0; i < occupiedSlots.Length; i++)
				{
					if (!occupiedSlots[i])
					{
						occupiedSlots[i] = true;
						num = i;
						windowRefs[num] = window;
						flag = true;
						i = occupiedSlots.Length;
					}
				}
				if (!flag)
				{
					num = 0;
					GUINotificationWindow window2;
					if ((window2 = windowRefs[num]) != null)
					{
						window2.Hide();
					}
					occupiedSlots[num] = true;
					windowRefs[num] = window;
				}
			}
			WindowHandler.AddWindowToHandle(window, windowStyle);
			return num;
		}

		public void UnassignSlot(int slot)
		{
			occupiedSlots[slot] = false;
			windowRefs[slot] = null;
		}

		public void AddOffset(int slot, float offset)
		{
			occupiedOffsets[slot] = offset;
			ChainUpdateWindowOffsets(slot);
		}

		public float GetCurrentOffset(int slot)
		{
			float num = 0f;
			if (slot != 0)
			{
				for (int i = 0; i < slot; i++)
				{
					num += occupiedOffsets[i];
				}
			}
			return 200f + num;
		}

		private void ChainUpdateWindowOffsets(int slot)
		{
			int num = slot + 1;
			if (num > occupiedSlots.Length - 1 || num > occupiedOffsets.Length - 1 || !occupiedSlots[num])
			{
				return;
			}
			for (int i = num; i < occupiedSlots.Length; i++)
			{
				if (occupiedSlots[i])
				{
					float num2 = 0f;
					GUINotificationWindow gUINotificationWindow = windowRefs[i];
					for (int j = 0; j < i; j++)
					{
						num2 += occupiedOffsets[j];
					}
					Vector2 offset = gUINotificationWindow.Offset;
					gUINotificationWindow.SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(offset.x, 0f - (num2 + 200f)), gUINotificationWindow.Size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				}
			}
		}
	}

	public enum BlockSizeType
	{
		Width,
		Height
	}

	private static NotificationWindowHandler viewerWindowHandler;

	private static SlotManager slotsManager;

	protected float timeStarted;

	private bool windowOverwrite;

	private int occupiedSlot;

	protected static NotificationWindowHandler WindowHandler
	{
		get
		{
			if (viewerWindowHandler == null)
			{
				viewerWindowHandler = new NotificationWindowHandler();
			}
			return viewerWindowHandler;
		}
	}

	protected static SlotManager SlotsManager
	{
		get
		{
			if (slotsManager == null)
			{
				slotsManager = new SlotManager();
			}
			return slotsManager;
		}
	}

	public bool WindowOverwrite
	{
		get
		{
			return windowOverwrite;
		}
		set
		{
			windowOverwrite = value;
		}
	}

	public int OccupiedSlot
	{
		get
		{
			return occupiedSlot;
		}
		set
		{
			occupiedSlot = value;
		}
	}

	public GUINotificationWindow()
	{
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		Traits.HitTestType = HitTestTypeEnum.Transparent;
		SetControlFlag(ControlFlagSetting.HitTestIgnore, true, false);
		Traits.LifeSpan = ControlTraits.LifeSpanTraitEnum.DestroyOnHide;
		Traits.UpdateTrait = ControlTraits.UpdateTraitEnum.AlwaysUpdate;
		timeStarted = Time.time;
	}

	public static float GetTextBlockSize(GUILabel[] textLabels, BlockSizeType type)
	{
		float num = 0f;
		if (type == BlockSizeType.Width)
		{
			for (int i = 0; i < textLabels.Length; i++)
			{
				Vector2 vector = textLabels[i].Style.UnityStyle.CalcSize(new GUIContent(textLabels[i].Text));
				if (num < vector.x)
				{
					num = vector.x;
				}
			}
		}
		if (type == BlockSizeType.Height)
		{
			for (int j = 0; j < textLabels.Length; j++)
			{
				GUIStyle unityStyle = textLabels[j].Style.UnityStyle;
				GUIContent content = new GUIContent(textLabels[j].Text);
				Vector2 size = textLabels[j].Size;
				float num2 = unityStyle.CalcHeight(content, size.x);
				if (num < num2)
				{
					num = num2;
				}
			}
		}
		return num;
	}

	public static Vector2 GetTextBlockSize(GUILabel[] textLabels)
	{
		return new Vector2(GetTextBlockSize(textLabels, BlockSizeType.Width), GetTextBlockSize(textLabels, BlockSizeType.Height));
	}
}
