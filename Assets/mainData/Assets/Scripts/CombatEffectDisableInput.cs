public class CombatEffectDisableInput : CombatEffectBase
{
	private bool originalInputEnabledState;

	private PlayerInputController _cachedPIC;

	private PlayerCombatController _cachedPCC;

	protected PlayerInputController PlayerInputController
	{
		get
		{
			if (_cachedPIC == null)
			{
				_cachedPIC = charGlobals.GetComponent<PlayerInputController>();
			}
			return _cachedPIC;
		}
	}

	protected PlayerCombatController PlayerCombatController
	{
		get
		{
			if (_cachedPCC == null)
			{
				_cachedPCC = charGlobals.GetComponent<PlayerCombatController>();
			}
			return _cachedPCC;
		}
	}

	protected bool InputEnabled
	{
		get
		{
			return PlayerInputController == null || PlayerInputController.AllowInput;
		}
		set
		{
			if (PlayerInputController != null)
			{
				PlayerInputController.AllowInput = value;
			}
			if (PlayerCombatController != null)
			{
				PlayerCombatController.SuppressHeroUpEffect(!value);
			}
			if (Utils.IsLocalPlayer(charGlobals))
			{
				SHSBrawlerMainWindow sHSBrawlerMainWindow = (SHSBrawlerMainWindow)GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow"];
				if (sHSBrawlerMainWindow != null)
				{
					sHSBrawlerMainWindow.EnablePowerButton(value);
					sHSBrawlerMainWindow.SuppressPowerStateChange(!value);
				}
			}
		}
	}

	public override void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		originalInputEnabledState = InputEnabled;
		InputEnabled = !newCombatEffectData.disableInput;
	}

	protected override void ReleaseEffect()
	{
		base.ReleaseEffect();
		InputEnabled = originalInputEnabledState;
	}
}
