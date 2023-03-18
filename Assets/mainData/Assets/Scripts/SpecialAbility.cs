using UnityEngine;

public class SpecialAbility
{
	protected static int nextSpecialAbilityID = 1;

	public static int PASSIVE_USES = -999;

	public int specialAbilityID;

	public string icon = string.Empty;

	public Vector2 iconSize = Vector2.zero;

	public string name = string.Empty;

	public int uses = PASSIVE_USES;

	public int usesLeft;

	public int requiredOwnable = -1;

	public string displaySpace = string.Empty;

	public virtual bool sameAs(SpecialAbility ab)
	{
		if (GetType() != ab.GetType())
		{
			return false;
		}
		if (icon != ab.icon || name != ab.name)
		{
			return false;
		}
		return true;
	}

	public static int SpecialAbilitySorter(SpecialAbility x, SpecialAbility y)
	{
		if (x.specialAbilityID < y.specialAbilityID)
		{
			return -1;
		}
		if (x.specialAbilityID > y.specialAbilityID)
		{
			return 1;
		}
		return 0;
	}

	public virtual void execute()
	{
	}

	public virtual void update()
	{
	}

	public virtual void begin()
	{
	}

	public virtual bool isUnlocked()
	{
		if (requiredOwnable != -1 && !OwnableDefinition.isOwned(requiredOwnable, AppShell.Instance.Profile))
		{
			return false;
		}
		return true;
	}
}
