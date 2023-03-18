using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class HqInputDebug : HqInput
{
	private List<KeyCodeEntry> debugKeys;

	private static bool themeRequestPending;

	public override void Start()
	{
		base.Start();
		RegisterEvents();
		RegisterDebugKeys();
	}

	public override void OnDisable()
	{
		UnregisterDebugKeys();
		UnregisterEvents();
		base.OnDisable();
	}

	private void RegisterEvents()
	{
		AppShell.Instance.EventMgr.AddListener<TogglePhysicsMessage>(OnTogglePhysicsMessage);
		AppShell.Instance.EventMgr.AddListener<NextThemeMessage>(OnNextThemeMessage);
		AppShell.Instance.EventMgr.AddListener<SpawnBrickMessage>(OnSpawnBrickMessage);
	}

	private void UnregisterEvents()
	{
		AppShell.Instance.EventMgr.RemoveListener<TogglePhysicsMessage>(OnTogglePhysicsMessage);
		AppShell.Instance.EventMgr.RemoveListener<NextThemeMessage>(OnNextThemeMessage);
		AppShell.Instance.EventMgr.RemoveListener<SpawnBrickMessage>(OnSpawnBrickMessage);
	}

	private void RegisterDebugKeys()
	{
		debugKeys = new List<KeyCodeEntry>();
		KeyCodeEntry keyCodeEntry = new KeyCodeEntry(KeyCode.P, false, false, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, OnChangeMode);
		keyCodeEntry = new KeyCodeEntry(KeyCode.B, false, false, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, NewBrick);
		keyCodeEntry = new KeyCodeEntry(KeyCode.R, false, false, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, NextRoom);
		keyCodeEntry = new KeyCodeEntry(KeyCode.T, false, false, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, NextTheme);
	}

	private void UnregisterDebugKeys()
	{
		foreach (KeyCodeEntry debugKey in debugKeys)
		{
			SHSDebugInput.Inst.RemoveKeyListener(debugKey);
		}
	}

	private void OnTogglePhysicsMessage(TogglePhysicsMessage msg)
	{
		OnChangeMode(new SHSKeyCode(KeyCode.P));
	}

	[Description("Changes HQ mode")]
	private void OnChangeMode(SHSKeyCode code)
	{
		if (fsm.GetCurrentState() == typeof(ViewMode))
		{
			HqController2.Instance.GotoFlingaMode();
		}
		else if (fsm.GetCurrentState() == typeof(FlingaMode))
		{
			HqController2.Instance.GotoPlacementMode();
		}
	}

	private void OnSpawnBrickMessage(SpawnBrickMessage msg)
	{
		NewBrick(new SHSKeyCode(KeyCode.Space));
	}

	[Description("Spawns a new brick")]
	private void NewBrick(SHSKeyCode code)
	{
		if (fsm.GetCurrentState() == typeof(ViewMode))
		{
			GameObject tempObjectPrefab = HqController2.Instance.GetTempObjectPrefab(0);
			if (tempObjectPrefab == null)
			{
				CspUtils.DebugLog("Prefab was null");
			}
			block = (Object.Instantiate(tempObjectPrefab, Vector3.zero, Quaternion.identity) as GameObject);
			if (block == null)
			{
				CspUtils.DebugLog("Block was null");
			}
			Utils.GetComponent<HqObject2>(block).GotoPlacementSelectedMode();
			blockOld = false;
			blockOldPos = Vector3.zero;
			blockOldRot = Quaternion.identity;
			DragDropInfo dragDropInfo = new DragDropInfo(block);
			dragDropInfo.Result = DragDropResult.Pending;
			AppShell.Instance.EventMgr.Fire(this, new GUIDragBeginMessage(dragDropInfo));
		}
	}

	[Description("Advance to next room in HQ")]
	private void NextRoom(SHSKeyCode code)
	{
		AppShell.Instance.EventMgr.Fire(null, new HQRoomChangeRequestMessage(HQRoomChangeRequestMessage.RoomCycleDirection.Next));
	}

	private void OnNextThemeMessage(NextThemeMessage msg)
	{
		NextTheme(new SHSKeyCode(KeyCode.T));
	}

	[Description("Advance to next theme in HQ")]
	private void NextTheme(SHSKeyCode code)
	{
		if (!themeRequestPending)
		{
			themeRequestPending = true;
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, "Are you sure you want to switch themes?", delegate(string Id, GUIDialogWindow.DialogState state)
			{
				if (state == GUIDialogWindow.DialogState.Ok)
				{
					HqController2.Instance.GoToNextTheme();
				}
				themeRequestPending = false;
			}, GUIControl.ModalLevelEnum.Default);
		}
	}
}
