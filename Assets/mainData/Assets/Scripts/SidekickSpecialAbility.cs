using System;
using UnityEngine;

public class SidekickSpecialAbility : SpecialAbility
{
	public SidekickSpecialAbility(PetUpgradeXMLDefinition def)
	{
		specialAbilityID = SpecialAbility.nextSpecialAbilityID++;
		icon = def.icon;
		name = def.name;
		uses = def.uses;
		requiredOwnable = def.requiredOwnable;
		displaySpace = def.displaySpace;
	}

	public static SidekickSpecialAbility parse(PetUpgradeXMLDefinition def)
	{
		if (def is PetUpgradeXMLDefinitionSmartbomb)
		{
			return new SidekickSpecialAbilitySmartBomb(def as PetUpgradeXMLDefinitionSmartbomb);
		}
		if (def is PetUpgradeXMLDefinitionAlly)
		{
			return new SidekickSpecialAbilityAlly(def as PetUpgradeXMLDefinitionAlly);
		}
		if (def is PetUpgradeXMLDefinitionSpawnTickets)
		{
			return new SidekickSpecialAbilitySpawnTickets(def as PetUpgradeXMLDefinitionSpawnTickets);
		}
		if (def is PetUpgradeXMLDefinitionSpawnXP)
		{
			return new SidekickSpecialAbilitySpawnXP(def as PetUpgradeXMLDefinitionSpawnXP);
		}
		if (def is PetUpgradeXMLDefinitionSpawnSilver)
		{
			return new SidekickSpecialAbilitySpawnSilver(def as PetUpgradeXMLDefinitionSpawnSilver);
		}
		if (def is PetUpgradeXMLDefinitionSpawnFractals)
		{
			return new SidekickSpecialAbilitySpawnFractals(def as PetUpgradeXMLDefinitionSpawnFractals);
		}
		if (def is PetUpgradeXMLDefinitionSpawnScavenger)
		{
			return new SidekickSpecialAbilitySpawnScavenger(def as PetUpgradeXMLDefinitionSpawnScavenger);
		}
		if (def is PetUpgradeXMLDefinitionStunPigeon)
		{
			return new SidekickSpecialAbilityStunPigeon(def as PetUpgradeXMLDefinitionStunPigeon);
		}
		if (def is PetUpgradeXMLDefinitionKillTroublebot)
		{
			return new SidekickSpecialAbilityKillTroublebot(def as PetUpgradeXMLDefinitionKillTroublebot);
		}
		if (def is PetUpgradeXMLDefinitionGrab)
		{
			return new SidekickSpecialAbilityGrab(def as PetUpgradeXMLDefinitionGrab);
		}
		if (def is PetUpgradeXMLDefinitionHotspot)
		{
			return new SidekickSpecialAbilityHotspot(def as PetUpgradeXMLDefinitionHotspot);
		}
		if (def is PetUpgradeXMLDefinitionMove)
		{
			return new SidekickSpecialAbilityMove(def as PetUpgradeXMLDefinitionMove);
		}
		if (def is PetUpgradeXMLDefinitionBrawlerPassiveBuff)
		{
			return new SidekickSpecialAbilityBrawlerPassiveBuff(def as PetUpgradeXMLDefinitionBrawlerPassiveBuff);
		}
		if (def is PetUpgradeXMLDefinitionBrawlerBuff)
		{
			return new SidekickSpecialAbilityBrawlerBuff(def as PetUpgradeXMLDefinitionBrawlerBuff);
		}
		if (def is PetUpgradeXMLDefinitionCooldown)
		{
			return new SidekickSpecialAbilityCooldown(def as PetUpgradeXMLDefinitionCooldown);
		}
		if (def is PetUpgradeXMLDefinitionPickupStrength)
		{
			return new SidekickSpecialAbilityPickupStrength(def as PetUpgradeXMLDefinitionPickupStrength);
		}
		if (def is PetUpgradeXMLDefinitionPokeImpossible)
		{
			return new SidekickSpecialAbilityPokeImpossible(def as PetUpgradeXMLDefinitionPokeImpossible);
		}
		if (def is PetUpgradeXMLDefinitionPokeStar)
		{
			return new SidekickSpecialAbilityPokeStar(def as PetUpgradeXMLDefinitionPokeStar);
		}
		CspUtils.DebugLog("SidekickSpecialAbility parse failed for " + def);
		throw new Exception("Invalid Special Ability XML detected, did you forget to add to the list? " + def);
	}

	public virtual void attachToPetObject(GameObject petObject)
	{
	}
}
