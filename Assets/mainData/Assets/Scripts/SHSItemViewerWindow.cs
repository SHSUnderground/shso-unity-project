using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SHSItemViewerWindow : GUINotificationWindow
{
	private struct ItemViewerProperties
	{
		public string bgTextureSource;

		public string labelText;

		public string numText;

		public int fakeOfflineValue;

		public Type itemSpawner;

		public Type[] itemSpawnerArgTypes;

		public object[] itemSpawnerArgs;

		public CustomConfiguration configurationDelegate;
	}

	private delegate void CustomConfiguration(SHSItemViewerWindow windowToConfigure, object[] windowParameters);

	private const float fadeStartTime = 3f;

	private const float fadeDurationTime = 0.5f;

	private const int logoOffset = 20;

	private const int textBlockOffset = 20;

	private const int textLineOffset = 10;

	private static Hashtable propertiesLookup;

	private GUIImage background;

	private GUILabel label;

	private GUILabel numLabel;

	private GUINotificationManager.GUINotificationStyleEnum windowStyle;

	private List<GUIControl> postAddList = new List<GUIControl>();

	private List<GUIControl> preAddList = new List<GUIControl>();

	private object[] windowParameters;

	public SHSItemViewerWindow(object[] windowParameters)
	{
		windowStyle = (GUINotificationManager.GUINotificationStyleEnum)(int)windowParameters[0];
		this.windowParameters = windowParameters;
	}

	private static void SetupAllItemViewerProperties()
	{
		propertiesLookup = new Hashtable();
		ItemViewerProperties itemViewerProperties = default(ItemViewerProperties);
		propertiesLookup.Add(GUINotificationManager.GUINotificationStyleEnum.TicketNotify, itemViewerProperties);
		itemViewerProperties.configurationDelegate = delegate
		{
			UserProfile profile3 = AppShell.Instance.Profile;
			ItemViewerProperties itemViewerProperties4 = (ItemViewerProperties)propertiesLookup[GUINotificationManager.GUINotificationStyleEnum.TicketNotify];
			itemViewerProperties4.bgTextureSource = "GUI/Notifications/gameworld_pickup_toast_tickets";
			itemViewerProperties4.labelText = "#TICKET_COUNT_TOAST";
			string numText3;
			if (profile3 != null)
			{
				numText3 = profile3.Tickets.ToString();
			}
			else
			{
				int num3 = ++itemViewerProperties4.fakeOfflineValue;
				numText3 = num3.ToString();
			}
			itemViewerProperties4.numText = numText3;
			itemViewerProperties4.itemSpawner = typeof(SHSTicketholioWindow);
			itemViewerProperties4.itemSpawnerArgs = new object[2]
			{
				0,
				0
			};
			itemViewerProperties4.itemSpawnerArgTypes = new Type[2]
			{
				typeof(Vector2),
				typeof(int)
			};
			propertiesLookup[GUINotificationManager.GUINotificationStyleEnum.TicketNotify] = itemViewerProperties4;
		};
		propertiesLookup[GUINotificationManager.GUINotificationStyleEnum.TicketNotify] = itemViewerProperties;
		itemViewerProperties = default(ItemViewerProperties);
		propertiesLookup.Add(GUINotificationManager.GUINotificationStyleEnum.CoinUpNotify, itemViewerProperties);
		itemViewerProperties.configurationDelegate = delegate(SHSItemViewerWindow windowToConfigure, object[] windowParameters)
		{
			UserProfile profile2 = AppShell.Instance.Profile;
			ItemViewerProperties itemViewerProperties3 = (ItemViewerProperties)propertiesLookup[GUINotificationManager.GUINotificationStyleEnum.CoinUpNotify];
			itemViewerProperties3.bgTextureSource = "GUI/Notifications/gameworld_pickup_toast_silver";
			itemViewerProperties3.labelText = "#SILVER_COUNT_TOAST";
			string numText2;
			if (profile2 != null)
			{
				numText2 = profile2.Silver.ToString();
			}
			else
			{
				int num2 = ++itemViewerProperties3.fakeOfflineValue;
				numText2 = num2.ToString();
			}
			itemViewerProperties3.numText = numText2;
			itemViewerProperties3.itemSpawner = typeof(SHSCoinholioWindow);
			itemViewerProperties3.itemSpawnerArgs = new object[3]
			{
				0,
				(int)windowParameters[2],
				(bool)windowParameters[3]
			};
			itemViewerProperties3.itemSpawnerArgTypes = new Type[3]
			{
				typeof(Vector2),
				typeof(int),
				typeof(bool)
			};
			propertiesLookup[GUINotificationManager.GUINotificationStyleEnum.CoinUpNotify] = itemViewerProperties3;
		};
		propertiesLookup[GUINotificationManager.GUINotificationStyleEnum.CoinUpNotify] = itemViewerProperties;
		itemViewerProperties = default(ItemViewerProperties);
		propertiesLookup.Add(GUINotificationManager.GUINotificationStyleEnum.FractalNotify, itemViewerProperties);
		itemViewerProperties.configurationDelegate = delegate(SHSItemViewerWindow windowToConfigure, object[] windowParameters)
		{
			UserProfile profile = AppShell.Instance.Profile;
			ItemViewerProperties itemViewerProperties2 = (ItemViewerProperties)propertiesLookup[GUINotificationManager.GUINotificationStyleEnum.FractalNotify];
			itemViewerProperties2.bgTextureSource = "GUI/Notifications/gameworld_pickup_toast_herotokens";
			itemViewerProperties2.labelText = "#FRACTAL_COUNT_TOAST";
			string numText;
			if (profile != null)
			{
				numText = profile.Shards.ToString();
			}
			else
			{
				int num = ++itemViewerProperties2.fakeOfflineValue;
				numText = num.ToString();
			}
			itemViewerProperties2.numText = numText;
			itemViewerProperties2.itemSpawner = typeof(SHSFractalholioWindow);
			itemViewerProperties2.itemSpawnerArgs = new object[3]
			{
				0,
				(int)windowParameters[2],
				(bool)windowParameters[3]
			};
			itemViewerProperties2.itemSpawnerArgTypes = new Type[3]
			{
				typeof(Vector2),
				typeof(int),
				typeof(bool)
			};
			propertiesLookup[GUINotificationManager.GUINotificationStyleEnum.FractalNotify] = itemViewerProperties2;
		};
		propertiesLookup[GUINotificationManager.GUINotificationStyleEnum.FractalNotify] = itemViewerProperties;
	}

	public override bool InitializeResources(bool reload)
	{
		if (reload)
		{
			return base.InitializeResources(reload);
		}
		if (propertiesLookup == null)
		{
			SetupAllItemViewerProperties();
		}
		GUIContent gUIContent = new GUIContent();
		Vector2 vector = default(Vector2);
		ItemViewerProperties itemViewerProperties = (ItemViewerProperties)propertiesLookup[windowStyle];
		float num = 0f;
		itemViewerProperties.configurationDelegate(this, windowParameters);
		itemViewerProperties = (ItemViewerProperties)propertiesLookup[windowStyle];
		background = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(225f, 94f), Vector2.zero);
		background.Position = Vector2.zero;
		background.TextureSource = itemViewerProperties.bgTextureSource;
		numLabel = new GUILabel();
		numLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 26, GUILabel.GenColor(255, 249, 157), TextAnchor.UpperLeft);
		numLabel.Text = itemViewerProperties.numText;
		gUIContent.text = numLabel.Text;
		vector = numLabel.Style.UnityStyle.CalcSize(gUIContent);
		num = vector.y;
		numLabel.Size = new Vector2(vector.x, vector.y);
		GUILabel gUILabel = numLabel;
		Vector2 size = background.Size;
		gUILabel.Position = new Vector2(size.x * 0.5f - vector.x * 0.5f + 20f, 20f);
		label = new GUILabel();
		label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 249, 157), TextAnchor.UpperLeft);
		label.Text = itemViewerProperties.labelText;
		gUIContent.text = label.Text;
		vector = label.Style.UnityStyle.CalcSize(gUIContent);
		label.Size = new Vector2(vector.x, vector.y);
		GUILabel gUILabel2 = label;
		Vector2 size2 = background.Size;
		float x = size2.x * 0.5f - vector.x * 0.5f + 20f;
		Vector2 position = numLabel.Position;
		gUILabel2.Position = new Vector2(x, position.y + num - 10f);
		foreach (GUIControl preAdd in preAddList)
		{
			Add(preAdd);
		}
		Add(background);
		Add(label);
		Add(numLabel);
		foreach (GUIControl postAdd in postAddList)
		{
			Add(postAdd);
		}
		base.OccupiedSlot = GUINotificationWindow.SlotsManager.AssignSlot(windowStyle, this);
		SlotManager slotsManager = GUINotificationWindow.SlotsManager;
		int occupiedSlot = base.OccupiedSlot;
		Vector2 size3 = background.Size;
		slotsManager.AddOffset(occupiedSlot, size3.y);
		Vector2 offset = new Vector2(0f, 0f - GUINotificationWindow.SlotsManager.GetCurrentOffset(base.OccupiedSlot));
		Vector2 size4 = background.Size;
		float x2 = size4.x;
		Vector2 size5 = background.Size;
		SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, offset, new Vector2(x2, size5.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		preAddList.Clear();
		postAddList.Clear();
		if (itemViewerProperties.itemSpawner != null)
		{
			ConstructorInfo constructor = itemViewerProperties.itemSpawner.GetConstructor(itemViewerProperties.itemSpawnerArgTypes);
			itemViewerProperties.itemSpawnerArgs[0] = new Vector2(0f, GUIManager.ScreenRect.height - GUINotificationWindow.SlotsManager.GetCurrentOffset(base.OccupiedSlot));
			((IGUIContainer)windowParameters[1]).Add((GUIControl)constructor.Invoke(itemViewerProperties.itemSpawnerArgs));
		}
		return base.InitializeResources(reload);
	}

	public override void OnUpdate()
	{
		float num = Time.time - timeStarted;
		float num2 = num - 3f;
		if (num > 3f)
		{
			Alpha = 1f - num2 / 0.5f;
		}
		if (Alpha <= 0f)
		{
			Hide();
		}
	}

	public override void Hide()
	{
		if (!base.WindowOverwrite)
		{
			GUINotificationWindow.WindowHandler.ClearWindowOfType(windowStyle);
			GUINotificationWindow.SlotsManager.UnassignSlot(base.OccupiedSlot);
		}
		base.Hide();
	}

	public void AddToPostDrawList(GUIControl control)
	{
		postAddList.Add(control);
	}

	public void AddToPreDrawList(GUIControl control)
	{
		preAddList.Add(control);
	}
}
