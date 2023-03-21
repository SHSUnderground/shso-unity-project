using LitJson;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml.XPath;
using UnityEngine;

public class PetDataManager
{
	private static Dictionary<int, PetData> allPetDataDict = new Dictionary<int, PetData>();

	private static Dictionary<int, bool> ownedPetDict = new Dictionary<int, bool>();

	public static void InitializeFromData(DataWarehouse xml)
	{
		XPathNavigator value = xml.GetValue("pets");
		XPathNodeIterator xPathNodeIterator = value.SelectChildren("pet", string.Empty);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(PetXMLDefinition));
		while (xPathNodeIterator.MoveNext())
		{
			string outerXml = xPathNodeIterator.Current.OuterXml;
			PetXMLDefinition petXMLDefinition = xmlSerializer.Deserialize(new StringReader(outerXml)) as PetXMLDefinition;
			if (petXMLDefinition != null)
			{
				allPetDataDict[petXMLDefinition.ownableID] = new PetData(petXMLDefinition);
			}
		}
	}

	public static void parsePetDefinition(int ownableID, string petDataJson)
	{
		PetDataJson petDataJson2 = JsonMapper.ToObject<PetDataJson>(petDataJson);
		if (petDataJson2 != null)
		{
			allPetDataDict[ownableID] = new PetData(ownableID, petDataJson2);
		}
	}

	public static PetData getData(int id)
	{
		PetData value;
		if (allPetDataDict.TryGetValue(id, out value))
		{
			return value;
		}
		return null;
	}

	public static bool ownsPet(int petOwnableTypeID)
	{
		return ownedPetDict.ContainsKey(petOwnableTypeID);
	}

	public static void purchasedPetByID(int ownableID, bool announce = false)
	{
		ownedPetDict[ownableID] = true;
		if (announce)
		{
			AppShell.Instance.EventMgr.Fire(null, new PetPurchasedEvent(ownableID));
		}
	}

	public static void respawnPet()
	{
		changeCurrentPet(getCurrentPet(), true);
	}

	public static void submitSidekickInfo()
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("user_id", AppShell.Instance.Profile.UserId.ToString());
		wWWForm.AddField("sidekick_id", ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.PetName, -1));
		wWWForm.AddField("sidekick_tier", 0);
		AppShell.Instance.WebService.StartRequest("resources$users/set_sidekick_info.py", delegate(ShsWebResponse response)
		{
			if (response.Status != 200)
			{
				CspUtils.DebugLog("set_sidekick_info Request failure: " + response.Status + ":" + response.Body);
			}
			else
			{
				CspUtils.DebugLog("set_sidekick_info Request success!: " + response.Status + ":" + response.Body);
			}
		}, wWWForm.data);
	}

	public static void changeCurrentPet(int id, bool forceChange = false)
	{
		if (!forceChange && id == ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.PetName, -1))
		{
			return;
		}
		if (!ownedPetDict.ContainsKey(id))
		{
			id = -1;
		}
		PetData data = getData(id);
		if (data == null)
		{
			id = -1;
		}
		if (id == -1)
		{
			ShsPlayerPrefs.SetInt(ShsPlayerPrefs.Keys.PetName, -1);
			AppShell.Instance.EventMgr.Fire(AppShell.Instance, new CurrentPetChangeEvent(-1));
			PetSpawner.requestPet(-1);
		}
		else if (data != null)
		{
			ShsPlayerPrefs.SetInt(ShsPlayerPrefs.Keys.PetName, data.id);
			AppShell.Instance.EventMgr.Fire(AppShell.Instance, new CurrentPetChangeEvent(data.id));
			PetSpawner.requestPet(data.id);
		}
		if (AppShell.Instance.Profile != null)
		{
			int num = 0;
			while (num < AppShell.Instance.Profile.socialAbilities.Count)
			{
				if (AppShell.Instance.Profile.socialAbilities[num] is SidekickSpecialAbility)
				{
					AppShell.Instance.Profile.socialAbilities.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}
		if (data != null)
		{
			foreach (SidekickSpecialAbility ability in data.abilities)
			{
				if (ability.displaySpace == "social" && ability.isUnlocked())
				{
					AppShell.Instance.Profile.socialAbilities.Add(ability);
					ability.begin();
				}
			}
		}
		if (AppShell.Instance.Profile != null)
		{
			AppShell.Instance.Profile.refreshSocialAbilities();
		}
		Object[] array = Object.FindSceneObjectsOfType(typeof(InteractiveObject));
		Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			InteractiveObject interactiveObject = (InteractiveObject)array2[i];
			interactiveObject.ResetForNewPlayer();
		}
		if (id == -1)
		{
			Object[] array3 = Object.FindSceneObjectsOfType(typeof(ActivityObject));
			Object[] array4 = array3;
			for (int j = 0; j < array4.Length; j++)
			{
				ActivityObject activityObject = (ActivityObject)array4[j];
				activityObject.ResetForNewPet(null);
			}
		}
	}

	public static int getCurrentPet()
	{
		int @int = ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.PetName, -1);
		if (!getOwnedPets().ContainsKey(@int))
		{
			return -1;
		}
		return @int;
	}

	public static Dictionary<int, bool> getOwnedPets()
	{
		return ownedPetDict;
	}
}
