using System.Collections.Generic;
using UnityEngine;

public class PetData
{
	public int id = -1;

	public string bundlePath = string.Empty;

	public string characterDataPath = string.Empty;

	public string readableName = string.Empty;

	public string displayName = string.Empty;

	public string displayDesc = string.Empty;

	public float idleTimeRange = 30f;

	public int flyer;

	public string shoppingIcon = string.Empty;

	public string inventoryIconBase = string.Empty;

	public string customRunAnim = string.Empty;

	public string customIdleAnim = string.Empty;

	public int pigeonDeathHack;

	public List<PetFidgetData> fidgets = new List<PetFidgetData>();

	public int fidgetEmoteID = -1;

	public string fidgetAnimName = string.Empty;

	public Vector3 scale = new Vector3(1f, 1f, 1f);

	public List<SpecialAbility> abilities = new List<SpecialAbility>();

	public HotSpotType.Style hotSpotType
	{
		get
		{
			HotSpotType.Style style = HotSpotType.Style.None;
			foreach (SpecialAbility ability in abilities)
			{
				if (ability.isUnlocked() && ability is SidekickSpecialAbilityHotspot)
				{
					style |= HotSpotType.GetEnumFromString((ability as SidekickSpecialAbilityHotspot).hotSpotType);
				}
			}
			return style;
		}
	}

	public PetData(int id, string bundlePath, Vector3 scale)
	{
		this.id = id;
		this.bundlePath = bundlePath;
		this.scale = scale;
	}

	public PetData(int id, PetDataJson info)
	{
		this.id = id;
		bundlePath = info.b;
		string[] array = info.s.Split('|');
		float x = float.Parse(array[0]);
		float y = float.Parse(array[1]);
		float z = float.Parse(array[2]);
		scale = new Vector3(x, y, z);
	}

	public PetData(PetXMLDefinition info)
	{
		id = info.ownableID;
		bundlePath = info.bundlePath;
		characterDataPath = info.characterDataPath;
		readableName = info.readableName;
		displayName = info.displayName;
		displayDesc = info.displayDesc;
		if (info.idleTimeRange != null)
		{
			idleTimeRange = float.Parse(info.idleTimeRange);
		}
		flyer = info.flyer;
		shoppingIcon = string.Empty + info.shoppingIcon;
		inventoryIconBase = string.Empty + info.inventoryIconBase;
		customRunAnim = string.Empty + info.customRunAnim;
		customIdleAnim = string.Empty + info.customIdleAnim;
		pigeonDeathHack = info.pigeonDeathHack;
		string[] array = info.scale.Split(',');
		float x = float.Parse(array[0]);
		float y = float.Parse(array[1]);
		float z = float.Parse(array[2]);
		scale = new Vector3(x, y, z);
		foreach (PetFidgetXMLDefinition fidget in info.fidgets)
		{
			PetFidgetData petFidgetData = new PetFidgetData();
			petFidgetData.fidgetAnimName = fidget.fidgetAnimName;
			petFidgetData.fidgetEmoteID = int.Parse("0" + fidget.fidgetEmoteID);
			petFidgetData.fidgetSoundName = fidget.fidgetSoundName;
			petFidgetData.fidgetMassEmoteID = fidget.fidgetMassEmoteID;
			petFidgetData.fidgetPlayerEmoteID = fidget.fidgetPlayerEmoteID;
			fidgets.Add(petFidgetData);
		}
		foreach (PetUpgradeXMLDefinition upgrade in info.upgrades)
		{
			abilities.Add(SidekickSpecialAbility.parse(upgrade));
		}
		abilities.Sort(delegate(SpecialAbility p1, SpecialAbility p2)
		{
			if (p1.requiredOwnable < p2.requiredOwnable)
			{
				return -1;
			}
			return (p1.requiredOwnable > p2.requiredOwnable) ? 1 : p1.name.CompareTo(p2.name);
		});
	}

	public PetFidgetData getRandomFidget()
	{
		if (fidgets.Count <= 0)
		{
			return null;
		}
		return fidgets[Random.Range(0, fidgets.Count)];
	}
}
