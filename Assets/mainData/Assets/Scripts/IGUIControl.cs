using System.Collections;
using UnityEngine;

public interface IGUIControl : IGUIDragDrop, IGUIDrawable, IGUIResizable, ICaptureHandler
{
	IGUIContainer Parent
	{
		get;
		set;
	}

	bool IsVisible
	{
		get;
		set;
	}

	bool IsEnabled
	{
		get;
		set;
	}

	bool IsActive
	{
		get;
	}

	bool ResourcesInitialized
	{
		get;
	}

	bool CachedVisible
	{
		get;
		set;
	}

	GUIControl.DrawPhaseHintEnum DrawPhaseHint
	{
		get;
		set;
	}

	BitArray ControlFlags
	{
		get;
	}

	Rect Rect
	{
		get;
		set;
	}

	Rect ScreenRect
	{
		get;
	}

	Rect ClientRect
	{
		get;
	}

	string Id
	{
		get;
		set;
	}

	string Path
	{
		get;
	}

	GUIManager.UILayer Layer
	{
		get;
		set;
	}

	GUIControl.ControlTraits Traits
	{
		get;
		set;
	}

	GUIControl.ToolTipInfo ToolTip
	{
		get;
	}

	Vector2 ToolTipOffset
	{
		get;
		set;
	}

	BitTexture Mask
	{
		get;
	}

	float Alpha
	{
		get;
		set;
	}

	float AnimationAlpha
	{
		get;
		set;
	}

	ContentLoadingActivateDelegate OnLoadingActivate
	{
		get;
		set;
	}

	Rect ContentLoadingCustomDrawRect
	{
		get;
		set;
	}

	void OnActive();

	void OnInactive();

	void OnShow();

	void OnHide();

	void OnUpdate();

	void OnAdded(IGUIContainer addedTo);

	void OnRemoved(IGUIContainer removedFrom);

	void OnSceneEnter(AppShell.GameControllerTypeData currentGameData);

	void OnSceneLoaded(AppShell.GameControllerTypeData currentGameData);

	void OnSceneLeave(AppShell.GameControllerTypeData lastGameData, AppShell.GameControllerTypeData currentGameData);

	void OnLocaleChange(string newLocale);

	bool InitializeResources(bool reload);

	void SetActive();

	void SetInactive();

	void Update();

	void EnsureHierarchyVisibleState();

	bool QueryHierarchyVisibleState();

	void Show();

	void Show(GUIControl.ModalLevelEnum modalLevel);

	void Hide();

	void SetVisible(bool visible, GUIControl.SetVisibleReason reason);

	void SetControlFlag(GUIControl.ControlFlagSetting Setting, bool On, bool SetChildren);

	void RemoveControlFlag(GUIControl.ControlFlagSetting Setting, bool SetChildren);

	bool GetControlFlag(GUIControl.ControlFlagSetting Setting);

	void SetControlMask(GUIControl.ControlFlagSetting Setting, bool On, bool SetChildren);

	void BringToFront();

	void SendToBack();

	void SetMask(Texture2D Texture);

	void SetMask(BitTexture Mask, int Width, int Height);
}
