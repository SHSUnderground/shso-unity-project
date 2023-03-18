using System;
using System.Collections.Generic;
using UnityEngine;

public class SidekickSpecialAbilityCooldown : SidekickSpecialAbility
{
	public string effect = string.Empty;

	public int refresh;

	private static DateTime epoch = new DateTime(2013, 10, 20, 0, 0, 0, DateTimeKind.Utc);

	private static Dictionary<string, float> _cooldowns = new Dictionary<string, float>();

	public SidekickSpecialAbilityCooldown(PetUpgradeXMLDefinitionCooldown def)
		: base(def)
	{
		effect = def.effect;
		refresh = def.refresh;
		uses = 1;
		switch (effect)
		{
		case "megacollect":
			name = "#ABILITY_NAME_MEGACOLLECT";
			icon = "shopping_bundle|megacollect";
			break;
		}
		float num = PlayerPrefs.GetInt("SpecialAbility." + effect);
		if (num < 1f)
		{
			num = -1000000f;
		}
		else
		{
			int num2 = (int)((double)num - (DateTime.Now - epoch).TotalSeconds);
			num = ((num2 > 0) ? (Time.time + (float)num2) : (-1000000f));
		}
		if (!_cooldowns.ContainsKey(effect))
		{
			_cooldowns.Add(effect, num);
		}
		else
		{
			_cooldowns[effect] = num;
		}
	}

	public override void execute()
	{
		if (_cooldowns[effect] > Time.time)
		{
			CspUtils.DebugLog("Can't activate cooldown effect " + effect + " because it has not refreshed");
			return;
		}
		_cooldowns[effect] = Time.time + (float)refresh;
		PlayerPrefs.SetInt("SpecialAbility." + effect, (int)((DateTime.Now - epoch).TotalSeconds + (double)refresh));
		switch (effect)
		{
		case "megacollect":
		{
			UnityEngine.Object[] array = UnityEngine.Object.FindSceneObjectsOfType(typeof(ActivityObject));
			UnityEngine.Object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				ActivityObject activityObject = (ActivityObject)array2[i];
				CspUtils.DebugLog("megacollect " + activityObject);
				activityObject.gameObject.BroadcastMessage("Triggered", null, SendMessageOptions.DontRequireReceiver);
			}
			break;
		}
		}
	}

	public virtual float cooldownRemaining()
	{
		if (_cooldowns[effect] < Time.time)
		{
			return 0f;
		}
		return _cooldowns[effect] - Time.time;
	}
}
