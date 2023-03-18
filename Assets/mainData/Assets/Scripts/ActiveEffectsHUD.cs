using System.Collections.Generic;
using UnityEngine;

public class ActiveEffectsHUD : GUISimpleControlWindow
{
	private List<EffectHUDItem> _effects = new List<EffectHUDItem>();

	protected GUIImage background;

	public static int OFFSET_X = 20;

	public static int DELTA_Y = 50;

	private SidekickHUDItem sidekickHUDItem;

	public ActiveEffectsHUD()
	{
		SetSize(new Vector2(529f, 600f));
		background = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(529f, 366f), new Vector2(-200f, 0f));
		background.HitTestType = HitTestTypeEnum.Rect;
		background.BlockTestType = BlockTestTypeEnum.Rect;
		background.TextureSource = "options_bundle|options_tab_panel_1";
		sidekickHUDItem = new SidekickHUDItem();
		sidekickHUDItem.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(OFFSET_X, 0f));
		sidekickHUDItem.IsVisible = false;
		Add(sidekickHUDItem);
		AppShell.Instance.EventMgr.AddListener<StartExpendableEffectMessage>(OnStartEffectMessage);
		AppShell.Instance.EventMgr.AddListener<StopExpendableEffectMessage>(OnStopEffectMessage);
	}

	public override void Update()
	{
	}

	private void OnStartEffectMessage(StartExpendableEffectMessage message)
	{
		if (AppShell.Instance.ServerConnection.GetGameUserId() != message.UserId || !AppShell.Instance.ExpendablesManager.ExpendableTypes.ContainsKey(message.ExpendableId))
		{
			return;
		}
		ExpendableDefinition expendableDefinition = AppShell.Instance.ExpendablesManager.ExpendableTypes[message.ExpendableId];
		if (expendableDefinition.ExpendEffectHandler != "persistent_handler")
		{
			CspUtils.DebugLog("ActiveEffectsHUD: Ignoring effect ID " + message.ExpendableId + " because it is not a persistant effect");
			return;
		}
		for (int i = 0; i < _effects.Count; i++)
		{
			if (_effects[i].def.OwnableTypeId == message.ExpendableId)
			{
				return;
			}
		}
		EffectHUDItem effectHUDItem = new EffectHUDItem(expendableDefinition);
		effectHUDItem.IsVisible = true;
		Add(effectHUDItem);
		_effects.Add(effectHUDItem);
		reshuffle();
	}

	private void reshuffle()
	{
		for (int i = 0; i < _effects.Count; i++)
		{
			if (_effects.Count > 7)
			{
				if (i % 2 == 0)
				{
					_effects[i].SetPosition(new Vector2(OFFSET_X, DELTA_Y + i * DELTA_Y / 2));
				}
				else
				{
					_effects[i].SetPosition(new Vector2(OFFSET_X - 45, DELTA_Y + i * DELTA_Y / 2));
				}
			}
			else
			{
				_effects[i].SetPosition(new Vector2(OFFSET_X, (i + 1) * DELTA_Y));
			}
		}
	}

	private void OnStopEffectMessage(StopExpendableEffectMessage message)
	{
		if (AppShell.Instance.ServerConnection.GetGameUserId() != message.UserId)
		{
			return;
		}
		for (int i = 0; i < _effects.Count; i++)
		{
			if (_effects[i].def.OwnableTypeId == message.ExpendableId)
			{
				Remove(_effects[i]);
				_effects.Remove(_effects[i]);
				break;
			}
		}
		reshuffle();
	}
}
