using System.Collections.Generic;
using UnityEngine;

public class BrawlerAttackSelector : GUISimpleControlWindow
{
	private GUIImage _background;

	private List<AttackSelectorButton> attackButtons = new List<AttackSelectorButton>();

	public BrawlerAttackSelector()
	{
		_background = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(45f, 46f), new Vector2(0f, 3f));
		_background.TextureSource = "brawlergadget_bundle|brawler_gadget_small_power_selection";
		Add(_background);
		AppShell.Instance.EventMgr.AddListener<PlayerChangedSecondaryAttackMessage>(OnChangedSecondaryAttack);
	}

	public void AddAttacks()
	{
		addAttack(1);
		addAttack(2);
		addAttack(3);
	}

	private void OnChangedSecondaryAttack(PlayerChangedSecondaryAttackMessage msg)
	{
		int whatAttack = msg.whatAttack;
		if (whatAttack >= 1 && whatAttack <= 3)
		{
			_background.SetPosition(new Vector2(40 * (whatAttack - 1) + 3, 4f));
		}
	}

	private void iconClicked(GUIControl sender, GUIClickEvent eventArgs)
	{
		int attackID = (sender as AttackSelectorButton).attackID;
		CspUtils.DebugLog("iconClicked " + attackID);
		GameObject localPlayer = BrawlerController.Instance.LocalPlayer;
		CombatController component = localPlayer.GetComponent<CombatController>();
		if (component != null)
		{
			CspUtils.DebugLog("setting secondary ");
			component.SetSecondaryAttack(attackID);
		}
	}

	private void addAttack(int whatAttack)
	{
		if (AppShell.Instance == null || AppShell.Instance.Profile == null)
		{
			CspUtils.DebugLog("BrawlerAttackSelector - AppShell.Instance is null, you must be running a scene directly in Unity?");
			return;
		}
		string lastSelectedCostume = AppShell.Instance.Profile.LastSelectedCostume;
		HeroPersisted value = null;
		AppShell.Instance.Profile.AvailableCostumes.TryGetValue(lastSelectedCostume, out value);
		int level = value.Level;
		int maxPowerAttackRankUnlockedAt = StatLevelReqsDefinition.Instance.GetMaxPowerAttackRankUnlockedAt(whatAttack, level);
		string buttonSourceRoot = "brawlergadget_bundle|super_power_" + whatAttack;
		GUIHotSpotButton gUIHotSpotButton = GUIControl.CreateControlTopLeftFrame<GUIHotSpotButton>(new Vector2(48f, 48f), new Vector2(40 * (whatAttack - 1), 5f));
		gUIHotSpotButton.HitTestType = HitTestTypeEnum.Circular;
		gUIHotSpotButton.BlockTestType = BlockTestTypeEnum.Circular;
		gUIHotSpotButton.BlockTestSize = new Vector2(1f, 1f);
		gUIHotSpotButton.IsVisible = false;
		Add(gUIHotSpotButton);
		gUIHotSpotButton.IsVisible = true;
		AttackSelectorButton attackSelectorButton = GUIControl.CreateControlTopLeftFrame<AttackSelectorButton>(new Vector2(48f, 48f), new Vector2(40 * (whatAttack - 1), 5f));
		attackSelectorButton.StyleInfo = new SHSButtonStyleInfo(buttonSourceRoot);
		attackSelectorButton.Traits.ActivationTrait = ControlTraits.ActivationTraitEnum.Manual;
		Add(attackSelectorButton);
		attackSelectorButton.attackID = whatAttack;
		attackSelectorButton.Traits.HitTestType = HitTestTypeEnum.Circular;
		attackSelectorButton.Traits.BlockTestType = BlockTestTypeEnum.Circular;
		if (maxPowerAttackRankUnlockedAt > 0)
		{
			attackSelectorButton.Click += iconClicked;
		}
		attackButtons.Add(attackSelectorButton);
		if (maxPowerAttackRankUnlockedAt > 0)
		{
			attackSelectorButton.ToolTip = new NamedToolTipInfo(MySquadDataManager.GetPowerAttackName(lastSelectedCostume, whatAttack, maxPowerAttackRankUnlockedAt), new Vector2(-40f, 0f));
		}
		else
		{
			string unlockAtLevelText = MySquadDataManager.GetUnlockAtLevelText(StatLevelReqsDefinition.Instance.GetLevelPowerAttackRankIsUnlockedAt(whatAttack, 1));
			GUIDrawTexture gUIDrawTexture = GUIControl.CreateControlAbsolute<GUIDrawTexture>(new Vector2(24f, 24f), new Vector2(40 * (whatAttack - 1) + 20, 24f));
			gUIDrawTexture.Traits.ActivationTrait = ControlTraits.ActivationTraitEnum.Manual;
			gUIDrawTexture.HitTestType = HitTestTypeEnum.Circular;
			gUIDrawTexture.TextureSource = "mysquadgadget_bundle|mysquad_gadget_charactericon_lock";
			gUIDrawTexture.ToolTip = new NamedToolTipInfo(unlockAtLevelText, new Vector2(-20f, 0f));
			Add(gUIDrawTexture);
		}
		if (whatAttack == 1)
		{
			GUIImage background = _background;
			Vector2 position = attackSelectorButton.Position;
			float x = position.x + 3f;
			Vector2 position2 = attackSelectorButton.Position;
			background.SetPosition(new Vector2(x, position2.y - 1f));
		}
	}
}
