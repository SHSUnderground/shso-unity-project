using System.Collections.Generic;
using UnityEngine;

public interface IGUIContainer : IGUIControl, IGUIDragDrop, IGUIDrawable, IGUIResizable, ICaptureHandler
{
	bool IsRoot
	{
		get;
	}

	List<IGUIControl> ControlList
	{
		get;
	}

	int Level
	{
		get;
	}

	Branch GraphNode
	{
		get;
		set;
	}

	Color BackgroundColor
	{
		get;
	}

	GUIFontInfo FontInfo
	{
		get;
		set;
	}

	IGUIControl this[string key]
	{
		get;
	}

	GUIWindow.LayoutTypeEnum LayoutType
	{
		get;
	}

	SHSSkin Skin
	{
		get;
	}

	void Add(IGUIControl Control);

	void Add(IGUIControl Control, GUIControl.DrawOrder DrawOrder);

	void Add(IGUIControl Control, GUIControl.DrawOrder DrawOrder, GUIControl.DrawPhaseHintEnum DrawPhaseHint);

	void Add(IGUIControl Control, IGUIControl TargetControl);

	void Remove(IGUIControl Control);

	void InternalDeferredRemove(IGUIControl Control);

	IGUIControl RemoveControlFromList(IGUIControl Control);

	IGUIControl GetControl(string Id);

	T GetControl<T>(string Id) where T : GUIControl;

	bool Transition(string WindowName);

	bool Transition(string WindowName, TransactionMonitor Transaction);

	void SetBackground(string textureSource);

	void SetBackground(Color color);

	void SetBackground(bool off);

	bool HitTestBoundsCheck();

	List<T> GetControlsOfType<T>();

	List<T> GetControlsOfType<T>(bool recursive);

	void GetBundleList(ref List<GUIWindow.SupportingAssetBundleInfo> bundleList, bool recurse);
}
