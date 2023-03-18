using System.Collections;
using UnityEngine;

public class GUIToolTipManager
{
	public enum State
	{
		Inactive,
		Activating,
		Active,
		Deactivating
	}

	public class ToolTipResources
	{
		public struct ResourceBundle
		{
			public Texture2D[] borderTextures;

			public Texture2D[] cornerTextures;

			public Texture2D bodyTexture;

			public Texture2D attachmentTexture;

			public static ResourceBundle EmptyBundle
			{
				get
				{
					return default(ResourceBundle);
				}
			}

			public void ReleaseTextureRefs()
			{
				if (borderTextures != null)
				{
					for (int i = 0; i < borderTextures.Length; i++)
					{
						borderTextures[i] = null;
					}
					borderTextures = null;
				}
				if (cornerTextures != null)
				{
					for (int j = 0; j < cornerTextures.Length; j++)
					{
						cornerTextures[j] = null;
					}
					cornerTextures = null;
				}
				bodyTexture = null;
				attachmentTexture = null;
			}
		}

		public struct ToolTipPathData
		{
			public string[] cornerPaths;

			public string[] borderPaths;

			public string bodyPath;

			public string attachmentPath;
		}

		public const int BorderAndCornerCount = 4;

		private Hashtable bundleTable = new Hashtable();

		public void CreateBundle(string key, ToolTipPathData pathData)
		{
			ResourceBundle resourceBundle = default(ResourceBundle);
			resourceBundle.cornerTextures = new Texture2D[4];
			resourceBundle.borderTextures = new Texture2D[4];
			for (int i = 0; i < 4; i++)
			{
				resourceBundle.borderTextures[i] = GUIManager.Instance.LoadTexture(pathData.borderPaths[i]);
				resourceBundle.cornerTextures[i] = GUIManager.Instance.LoadTexture(pathData.cornerPaths[i]);
			}
			resourceBundle.bodyTexture = GUIManager.Instance.LoadTexture(pathData.bodyPath);
			if (pathData.attachmentPath != string.Empty)
			{
				resourceBundle.attachmentTexture = GUIManager.Instance.LoadTexture(pathData.attachmentPath);
			}
			bundleTable.Add(key, resourceBundle);
		}

		public ResourceBundle GetBundle(string key)
		{
			if (bundleTable.ContainsKey(key))
			{
				return (ResourceBundle)bundleTable[key];
			}
			return ResourceBundle.EmptyBundle;
		}
	}

	private float time;

	protected State currentState;

	private IGUIControl lastOverControl;

	private IGUIControl currentOverControl;

	private float currentshowDelay;

	private float showDelay = 0.5f;

	private float hideDelay = 5f;

	private IGUIControl toolTipControl;

	private SHSHoverHelp hoverHelpLayer;

	private ToolTipResources toolTipResources;

	public State CurrentState
	{
		get
		{
			return currentState;
		}
		protected set
		{
			currentState = value;
		}
	}

	public IGUIControl CurrentOverControl
	{
		get
		{
			return currentOverControl;
		}
		set
		{
			currentOverControl = value;
		}
	}

	public float ShowDelay
	{
		get
		{
			return showDelay;
		}
		set
		{
			showDelay = value;
		}
	}

	public float HideDelay
	{
		get
		{
			return hideDelay;
		}
		set
		{
			hideDelay = value;
		}
	}

	public IGUIControl ToolTipControl
	{
		get
		{
			return toolTipControl;
		}
		set
		{
			toolTipControl = value;
		}
	}

	public SHSHoverHelp HoverHelpLayer
	{
		get
		{
			return hoverHelpLayer;
		}
		set
		{
			hoverHelpLayer = value;
		}
	}

	public ToolTipResources ToolTipResource
	{
		get
		{
			return toolTipResources;
		}
	}

	public GUIToolTipManager(GUIManager Manager)
	{
		toolTipResources = new ToolTipResources();
		time = Time.time;
		currentOverControl = null;
		currentshowDelay = showDelay;
		lastOverControl = null;
		ToolTipResources.ToolTipPathData pathData = default(ToolTipResources.ToolTipPathData);
		pathData.bodyPath = "GUI/ToolTipTextures/body_small";
		pathData.attachmentPath = "GUI/ToolTipTextures/attachmentPiece";
		pathData.cornerPaths = new string[4];
		pathData.borderPaths = new string[4];
		for (int i = 0; i < 4; i++)
		{
			pathData.cornerPaths[i] = "GUI/ToolTipTextures/corner" + (i + 1).ToString() + "_small";
			pathData.borderPaths[i] = "GUI/ToolTipTextures/border" + (i + 1).ToString() + "_small";
		}
		toolTipResources.CreateBundle("default", pathData);
		AppShell.Instance.OnOldControllerUnloading += delegate
		{
			currentState = State.Deactivating;
		};
	}

	public void Update()
	{
		if (SHSDiagnosticsWindow.tooltipDebug && currentOverControl != null)
		{
			if (toolTipControl != null && currentOverControl == null)
			{
			}
			return;
		}
		if (currentOverControl == null && (currentState == State.Activating || currentState == State.Active))
		{
			currentState = State.Deactivating;
		}
		if (currentOverControl != null && !currentOverControl.IsVisible && (currentState == State.Activating || currentState == State.Active))
		{
			currentState = State.Deactivating;
		}
		if (lastOverControl != currentOverControl)
		{
			time = Time.time;
			lastOverControl = currentOverControl;
			CurrentState = ((lastOverControl != null && lastOverControl.ToolTip != GUIControl.NoToolTipInfo.Instance) ? State.Activating : State.Deactivating);
			if (currentOverControl != null && GUIManager.Instance.CurrentState == GUIManager.ModalStateEnum.Modal && !currentOverControl.GetControlFlag(GUIControl.ControlFlagSetting.IsModal))
			{
				CurrentState = State.Deactivating;
			}
			return;
		}
		switch (currentState)
		{
		case State.Inactive:
			time = Time.time;
			currentshowDelay += 0.01f;
			if (currentshowDelay > showDelay)
			{
				currentshowDelay = showDelay;
			}
			break;
		case State.Activating:
			if (Time.time > time + currentshowDelay)
			{
				updateToolTip();
				CurrentState = State.Active;
				currentshowDelay = 0f;
				time = Time.time;
			}
			break;
		case State.Active:
			if (lastOverControl != currentOverControl)
			{
				CurrentState = State.Inactive;
			}
			else if (Time.time > time + hideDelay && currentOverControl != null && currentOverControl.ToolTip.TooltipType != GUIControl.ToolTipInfo.ToolTipTypeEnum.HoverHelp)
			{
				CurrentState = State.Deactivating;
			}
			break;
		case State.Deactivating:
			if (toolTipControl != null)
			{
				deactivateTooltip();
			}
			CurrentState = State.Inactive;
			break;
		}
	}

	public void RefreshToolTip()
	{
		if (currentOverControl != null && currentOverControl.ToolTip != GUIControl.NoToolTipInfo.Instance)
		{
			currentState = State.Activating;
		}
	}

	private void updateToolTip()
	{
		if (toolTipControl != null && currentOverControl != null)
		{
			updateToolTip(currentOverControl.ToolTipOffset);
		}
	}

	private void updateToolTip(Vector2 positionOffset)
	{
		if (toolTipControl != null && currentOverControl != null)
		{
			if (currentOverControl.ToolTip.TooltipType == GUIControl.ToolTipInfo.ToolTipTypeEnum.HoverHelp)
			{
				hoverHelpLayer.DisplayHoverHelp(currentOverControl, currentOverControl.ToolTip as GUIControl.HoverHelpInfo);
				((SHSTooltip)toolTipControl).Hide();
			}
			else
			{
				((SHSTooltip)toolTipControl).Configure(currentOverControl.ToolTip, currentOverControl, positionOffset);
				hoverHelpLayer.Hide();
			}
		}
	}

	private void deactivateTooltip()
	{
		((SHSTooltip)toolTipControl).Hide();
		hoverHelpLayer.Hide();
	}
}
