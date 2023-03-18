using System.Runtime.CompilerServices;
using UnityEngine;

public class GUIControlInspector : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool ReflectButton;

	public bool DebugDraw;

	public string type;

	public string id;

	public GUIControl.DockingAlignmentEnum dockingAlignment;

	public GUIControl.AnchorAlignmentEnum anchorAlignment;

	public GUIControl.OffsetType offsetType;

	public Vector2 offset;

	public Vector2 position;

	public Rect padding;

	public Rect margin;

	public Vector2 size;

	public Vector2 toolTipOffset;

	public bool hover;

	public Rect contentLoadingRect;

	public bool IsVisible;

	public float Alpha;

	public float AnimationAlpha;

	public GUIControl.ControlTraits.VisibilityTraitEnum VisibilityTrait;

	public GUIControl.ControlTraits.EventHandlingEnum EventHandling;

	public GUIControl.ControlTraits.RespectDisabledAlphaTraitEnum RespectDisabledAlphaTrait;

	public GUIControl.HitTestTypeEnum HitTestType;

	public GUIControl.BlockTestTypeEnum BlockTestType;

	protected int uniqueId;

	private bool oldReflectButton;

	private GUIControl guiControl;

	[CompilerGenerated]
	private bool _003CLastIsVisible_003Ek__BackingField;

	public bool LastIsVisible
	{
		[CompilerGenerated]
		get
		{
			return _003CLastIsVisible_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CLastIsVisible_003Ek__BackingField = value;
		}
	}

	public int UniqueId
	{
		set
		{
			uniqueId = value;
		}
	}

	public GUIControl GUIControl
	{
		get
		{
			return guiControl;
		}
		set
		{
			guiControl = value;
		}
	}

	public void Awake()
	{
		if (guiControl != null)
		{
			guiControl.FromObjectToInspector();
		}
	}

	public void Update()
	{
		if (guiControl == null)
		{
			return;
		}
		if (ReflectButton)
		{
			if (!oldReflectButton)
			{
				CspUtils.DebugLog("Element <" + type + "> " + id + " is now active for editing.");
				oldReflectButton = true;
			}
			guiControl.FromInspectorToObject();
		}
		else if (oldReflectButton)
		{
			CspUtils.DebugLog("Element <" + type + "> " + id + " is no longer being edited.");
			oldReflectButton = false;
			guiControl.FromObjectToInspector();
		}
	}
}
