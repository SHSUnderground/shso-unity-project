using System.Collections.Generic;
using UnityEngine;

public class CharacterLocatorManager
{
	protected SquadBattlePlayerEnum playerEnum;

	protected SquadBattleCharacterLocator spawnLocator;

	protected SquadBattleCharacterLocator spawnSecondLocator;

	protected SquadBattleCharacterLocator[] keeperLocators;

	protected Dictionary<string, GameObject> charactersByName;

	protected Dictionary<string, SquadBattleCharacterLocator> locatorsByCharacter;

	protected Dictionary<string, int> keeperCountByCharacter;

	protected int nextAvailableLocator;

	protected GameObject[] temporaryCharacters;

	protected int currentRewardCharacter;

	public int NextAvailableLocator
	{
		get
		{
			return nextAvailableLocator;
		}
	}

	public CharacterLocatorManager()
	{
	}

	public CharacterLocatorManager(SquadBattlePlayerEnum newPlayerEnum)
	{
		nextAvailableLocator = 0;
		playerEnum = newPlayerEnum;
		locatorsByCharacter = new Dictionary<string, SquadBattleCharacterLocator>();
		keeperCountByCharacter = new Dictionary<string, int>();
		charactersByName = new Dictionary<string, GameObject>();
		temporaryCharacters = new GameObject[2];
		List<SquadBattleCharacterLocator> list = new List<SquadBattleCharacterLocator>();
		GameObject[] array = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			SquadBattleCharacterLocator squadBattleCharacterLocator = gameObject.GetComponent(typeof(SquadBattleCharacterLocator)) as SquadBattleCharacterLocator;
			if (!(squadBattleCharacterLocator != null) || squadBattleCharacterLocator.playerIndex != playerEnum)
			{
				continue;
			}
			if (squadBattleCharacterLocator.positionIndex < 0)
			{
				if (squadBattleCharacterLocator.positionIndex == -1)
				{
					spawnLocator = squadBattleCharacterLocator;
				}
				else if (squadBattleCharacterLocator.positionIndex == -2)
				{
					spawnSecondLocator = squadBattleCharacterLocator;
				}
				else
				{
					CspUtils.DebugLog("Invalid position index " + squadBattleCharacterLocator.positionIndex + " for player " + playerEnum + " - ignoring locator");
				}
			}
			else
			{
				list.Add(squadBattleCharacterLocator);
			}
		}
		keeperLocators = new SquadBattleCharacterLocator[list.Count];
		foreach (SquadBattleCharacterLocator item in list)
		{
			if (keeperLocators[item.positionIndex] != null)
			{
				CspUtils.DebugLog("Duplicate CharacterLocator position index " + item.positionIndex + " for player " + item.playerIndex + " - locator ignored");
			}
			else
			{
				keeperLocators[item.positionIndex] = item;
			}
		}
		for (int j = 0; j < keeperLocators.Length; j++)
		{
			if (keeperLocators[j] == null)
			{
				CspUtils.DebugLog("Missing CharacterLocator position index " + j + " for player " + playerEnum + " - system will not work properly");
			}
			MeshRenderer meshRenderer = keeperLocators[j].GetComponentInChildren(typeof(MeshRenderer)) as MeshRenderer;
			meshRenderer.enabled = false;
		}
		if (spawnLocator == null)
		{
			CspUtils.DebugLog("Missing spawn locator (index -1) for player " + playerEnum + " - system will not work properly");
		}
		else
		{
			MeshRenderer meshRenderer2 = spawnLocator.GetComponentInChildren(typeof(MeshRenderer)) as MeshRenderer;
			meshRenderer2.enabled = false;
		}
		if (spawnSecondLocator == null)
		{
			CspUtils.DebugLog("Missing second spawn locator (index -2) for player " + playerEnum + " - system will not work properly");
			return;
		}
		MeshRenderer meshRenderer3 = spawnSecondLocator.GetComponentInChildren(typeof(MeshRenderer)) as MeshRenderer;
		meshRenderer3.enabled = false;
	}

	public GameObject getCharacter(string characterName)
	{
		if (charactersByName.ContainsKey(characterName))
		{
			return charactersByName[characterName];
		}
		return null;
	}

	public GameObject getAvatar()
	{
		if (keeperLocators[0] != null)
		{
			if (keeperLocators[0].keeperCharacter == null)
			{
				CspUtils.DebugLog("No avatar character found!");
				return null;
			}
			return keeperLocators[0].keeperCharacter;
		}
		CspUtils.DebugLog("No avatar character locator found!");
		return null;
	}

	public GameObject getNextCharacter()
	{
		GameObject keeperCharacter = keeperLocators[currentRewardCharacter].keeperCharacter;
		currentRewardCharacter++;
		if (currentRewardCharacter == nextAvailableLocator)
		{
			currentRewardCharacter = 0;
		}
		return keeperCharacter;
	}

	public void spawnKeeper(string characterName, SquadBattleAction action)
	{
		if (keeperCountByCharacter.ContainsKey(characterName))
		{
			Dictionary<string, int> dictionary;
			Dictionary<string, int> dictionary2 = dictionary = keeperCountByCharacter;
			string key;
			string key2 = key = characterName;
			int num = dictionary[key];
			dictionary2[key2] = num + 1;
			return;
		}
		if (nextAvailableLocator >= keeperLocators.Length)
		{
			CspUtils.DebugLog("No more locators are available for another keeper");
			return;
		}
		keeperCountByCharacter.Add(characterName, 1);
		SquadBattleCharacterLocator homeLocator = keeperLocators[nextAvailableLocator];
		if (nextAvailableLocator == 0)
		{
			spawnCharacter(characterName, action, null, homeLocator, homeLocator, false, OnSpawnedAvatar);
		}
		else
		{
			spawnCharacter(characterName, action, null, spawnLocator, homeLocator, false, null);
		}
		nextAvailableLocator++;
	}

	protected void OnSpawnedAvatar(GameObject spawnedCharacter)
	{
		CharacterGlobals characterGlobals = spawnedCharacter.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
		characterGlobals.squadBattleCharacterAI.Intro();
		if (playerEnum == SquadBattlePlayerEnum.Left)
		{
			CardGameController.Instance.StartTransaction.CompleteStep("spawnedAvatars");
		}
	}

	public void removeKeeper(string characterName)
	{
		removeKeeper(characterName, false);
	}

	public void removeKeeper(string characterName, bool temporary)
	{
		CspUtils.DebugLog(playerEnum + " removeKeeper " + characterName + " temporary = " + temporary);
		if (!keeperCountByCharacter.ContainsKey(characterName))
		{
			CspUtils.DebugLog("Tried to remove nonexisting keeper " + characterName);
			return;
		}
		Dictionary<string, int> dictionary;
		Dictionary<string, int> dictionary2 = dictionary = keeperCountByCharacter;
		string key;
		string key2 = key = characterName;
		int num = dictionary[key];
		dictionary2[key2] = num - 1;
		if (keeperCountByCharacter[characterName] == 0)
		{
			if (charactersByName.ContainsKey(characterName))
			{
				clearLocator(locatorsByCharacter[characterName].positionIndex);
				locatorsByCharacter.Remove(characterName);
				SquadBattleCharacterAI squadBattleCharacterAI = charactersByName[characterName].GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
				squadBattleCharacterAI.homeLocator = null;
				charactersByName.Remove(characterName);
			}
			if (!temporary)
			{
				keeperCountByCharacter.Remove(characterName);
			}
		}
		if (temporary)
		{
			Dictionary<string, int> dictionary3;
			Dictionary<string, int> dictionary4 = dictionary3 = keeperCountByCharacter;
			string key3 = key = characterName;
			num = dictionary3[key];
			dictionary4[key3] = num + 1;
		}
	}

	protected void clearLocator(int locatorIndex)
	{
		for (int i = locatorIndex; i < keeperLocators.Length && keeperLocators[i].keeperCharacter != null; i++)
		{
			GameObject gameObject = null;
			if (i + 1 < keeperLocators.Length)
			{
				gameObject = keeperLocators[i + 1].keeperCharacter;
			}
			keeperLocators[i].keeperCharacter = gameObject;
			if (gameObject != null)
			{
				locatorsByCharacter[gameObject.name] = keeperLocators[i];
				SquadBattleCharacterAI squadBattleCharacterAI = gameObject.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
				squadBattleCharacterAI.SetHomeLocator(keeperLocators[i]);
			}
		}
		nextAvailableLocator--;
	}

	public void spawnTemporaryCharacter(string characterName, SquadBattleAction action, GameObject approachTarget, bool isSecondaryCharacter, SquadBattleCharacterSpawnData.TemporaryCharacterSpawned OnSpawnedEvent)
	{
		SquadBattleCharacterLocator squadBattleCharacterLocator = spawnLocator;
		if (isSecondaryCharacter)
		{
			squadBattleCharacterLocator = spawnSecondLocator;
		}
		spawnCharacter(characterName, action, approachTarget, squadBattleCharacterLocator, null, isSecondaryCharacter, OnSpawnedEvent);
	}

	protected void spawnCharacter(string characterName, SquadBattleAction action, GameObject approachTarget, SquadBattleCharacterLocator spawnLocator, SquadBattleCharacterLocator homeLocator, bool isSecondaryCharacter, SquadBattleCharacterSpawnData.TemporaryCharacterSpawned OnSpawnedEvent)
	{
		CspUtils.DebugLog("spawnCharacter " + characterName);
		SquadBattleCharacterSpawnData squadBattleCharacterSpawnData = new SquadBattleCharacterSpawnData();
		squadBattleCharacterSpawnData.action = action;
		squadBattleCharacterSpawnData.spawnLocator = spawnLocator;
		squadBattleCharacterSpawnData.homeLocator = homeLocator;
		squadBattleCharacterSpawnData.characterName = characterName;
		squadBattleCharacterSpawnData.approachTarget = approachTarget;
		squadBattleCharacterSpawnData.onSpawned = OnSpawnedEvent;
		if (temporaryCharacters[0] != null && temporaryCharacters[0].name == characterName)
		{
			squadBattleCharacterSpawnData.obj = temporaryCharacters[0];
			characterSpawned(squadBattleCharacterSpawnData);
		}
		else if (temporaryCharacters[1] != null && temporaryCharacters[1].name == characterName)
		{
			squadBattleCharacterSpawnData.obj = temporaryCharacters[1];
			characterSpawned(squadBattleCharacterSpawnData);
		}
		else
		{
			AppShell.Instance.DataManager.LoadGameData("Characters/" + characterName, OnCharacterDataLoaded, squadBattleCharacterSpawnData);
		}
	}

	protected void OnCharacterDataLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			return;
		}
		SquadBattleCharacterSpawnData squadBattleCharacterSpawnData = extraData as SquadBattleCharacterSpawnData;
		squadBattleCharacterSpawnData.spawnData = response.Data;
		string @string = squadBattleCharacterSpawnData.spawnData.GetString("//asset_bundle");
		string string2 = squadBattleCharacterSpawnData.spawnData.GetString("//character_model/model_name");
		if (@string != null && string2 != null)
		{
			squadBattleCharacterSpawnData.assetBundleName = @string;
			AppShell.Instance.BundleLoader.LoadAsset(@string, string2, squadBattleCharacterSpawnData, OnCharacterModelLoaded);
		}
		else
		{
			CspUtils.DebugLog("No asset bundle name found in character data <" + response.Path + ">.  Cannot spawn character.");
		}
	}

	protected void OnCharacterModelLoaded(Object modelPrefab, AssetBundle bundle, object extraData)
	{
		SquadBattleCharacterSpawnData squadBattleCharacterSpawnData = extraData as SquadBattleCharacterSpawnData;
		if (squadBattleCharacterSpawnData == null)
		{
			CspUtils.DebugLog("Spawn data missing in callback from AssetBundleLoader for unknown character.");
			return;
		}
		DataWarehouse spawnData = squadBattleCharacterSpawnData.spawnData;
		string @string = spawnData.GetString("//name");
		if (modelPrefab == null)
		{
			CspUtils.DebugLog("No character model prefab found for character " + @string + ".  Cannot spawn character.");
			return;
		}
		if (CardGameController.Instance.spawnEffectPrefab != null && squadBattleCharacterSpawnData.homeLocator != keeperLocators[0])
		{
			GameObject gameObject = Object.Instantiate(CardGameController.Instance.spawnEffectPrefab, squadBattleCharacterSpawnData.spawnLocator.transform.position, squadBattleCharacterSpawnData.spawnLocator.transform.rotation) as GameObject;
			if (gameObject != null)
			{
				EffectSequence effectSequence = gameObject.GetComponent(typeof(EffectSequence)) as EffectSequence;
				if (effectSequence != null)
				{
					effectSequence.Initialize(null, null, null);
					effectSequence.StartSequence();
				}
			}
		}
		GameObject gameObject2 = Object.Instantiate(modelPrefab) as GameObject;
		gameObject2.name = @string;
		squadBattleCharacterSpawnData.obj = gameObject2;
		gameObject2.transform.position = squadBattleCharacterSpawnData.spawnLocator.transform.position;
		gameObject2.transform.rotation = squadBattleCharacterSpawnData.spawnLocator.transform.rotation;
		gameObject2.layer = 12;
		Vector3 vector = spawnData.TryGetVector("//character_model/scale", Vector3.one);
		if (vector != Vector3.one)
		{
			gameObject2.transform.localScale = vector;
		}
		Object[] componentsInChildren = gameObject2.GetComponentsInChildren(typeof(SkinnedMeshRenderer));
		Object[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)array[i];
			skinnedMeshRenderer.updateWhenOffscreen = spawnData.TryGetBool("//lod/animate_offscreen", false);
		}
		if (gameObject2.GetComponentInChildren<SkinnedMeshRenderer>() != null && gameObject2.GetComponentInChildren<LodCharacter>() == null && spawnData.TryGetBool("//lod/enabled", true))
		{
			gameObject2.AddComponent<LodCharacter>();
		}
		Utils.SetLayerTree(gameObject2, 12);
		GraphicsOptions.AddPlayerShadow(gameObject2);
		SkinnedMeshRenderer skinnedMeshRenderer2 = gameObject2.GetComponentInChildren(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
		skinnedMeshRenderer2.gameObject.AddComponent(typeof(DynamicShadowTarget));
		CharacterController characterController = gameObject2.AddComponent(typeof(CharacterController)) as CharacterController;
		characterController.height = spawnData.TryGetFloat("//character_controller/height", 2f);
		characterController.radius = spawnData.TryGetFloat("//character_controller/radius", 0.5f);
		characterController.slopeLimit = spawnData.TryGetFloat("//character_controller/slope_limit", 90f);
		characterController.stepOffset = spawnData.TryGetFloat("//character_controller/step_offset", 0.3f);
		characterController.center = spawnData.TryGetVector("//character_controller/center", new Vector3(0f, 1f, 0f));
		RaycastHit hitInfo;
		if (ShsCharacterController.FindGround(gameObject2.transform.position + Vector3.up, 10f, out hitInfo))
		{
			gameObject2.transform.position = hitInfo.point;
		}
		else
		{
			CspUtils.DebugLog("Failed to find ground when spawning " + @string + " at " + gameObject2.transform.position);
		}
		SquadBattleCharacterAI squadBattleCharacterAI = gameObject2.AddComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
		if (squadBattleCharacterSpawnData.homeLocator != null)
		{
			squadBattleCharacterAI.homeLocator = squadBattleCharacterSpawnData.homeLocator;
		}
		else
		{
			squadBattleCharacterAI.homeLocator = squadBattleCharacterSpawnData.spawnLocator;
		}
		squadBattleCharacterAI.locatorManager = this;
		gameObject2.AddComponent(typeof(ShsCharacterController));
		BehaviorManager behaviorManager = gameObject2.AddComponent(typeof(BehaviorManager)) as BehaviorManager;
		behaviorManager.defaultBehaviorType = "BehaviorMovementSquadBattle";
		CharacterMotionController characterMotionController = gameObject2.AddComponent(typeof(CharacterMotionController)) as CharacterMotionController;
		DataWarehouse data = spawnData.GetData("//character_motion_controller");
		characterMotionController.InitializeFromData(data);
		Animation component = Utils.GetComponent<Animation>(gameObject2, Utils.SearchChildren);
		if (component != null)
		{
			FacialAnimation facialAnimation = component.gameObject.AddComponent(typeof(FacialAnimation)) as FacialAnimation;
			facialAnimation.facialExpression = FacialAnimation.Expression.Normal;
		}
		CharacterSpawn.AddExtraComponents(gameObject2, spawnData, bundle);
		CombatController combatController;
		if (spawnData.GetCount("//player_combat_controller") > 0)
		{
			DataWarehouse data2 = spawnData.GetData("//player_combat_controller");
			DataWarehouse attackData = null;
			if (spawnData.GetCount("//attack_data") > 0)
			{
				attackData = spawnData.GetData("//attack_data");
			}
			PlayerCombatController playerCombatController = gameObject2.AddComponent(typeof(PlayerCombatController)) as PlayerCombatController;
			playerCombatController.InitializeFromData(data2, attackData);
			combatController = playerCombatController;
			squadBattleCharacterAI.isVillain = false;
		}
		else
		{
			squadBattleCharacterAI.isVillain = true;
			DataWarehouse data3 = spawnData.GetData("//combat_controller");
			DataWarehouse attackData2 = null;
			if (spawnData.GetCount("//attack_data") > 0)
			{
				attackData2 = spawnData.GetData("//attack_data");
			}
			combatController = (gameObject2.AddComponent(typeof(CombatController)) as CombatController);
			combatController.InitializeFromData(data3, attackData2);
			combatController.recoilResistance = 0;
		}
		if (playerEnum == SquadBattlePlayerEnum.Left)
		{
			combatController.faction = CombatController.Faction.Player;
		}
		else
		{
			combatController.faction = CombatController.Faction.Enemy;
		}
		EffectSequenceList effectSequenceList = gameObject2.AddComponent(typeof(EffectSequenceList)) as EffectSequenceList;
		DataWarehouse data4 = spawnData.GetData("//effect_sequence_list");
		effectSequenceList.InitializeFromData(data4, bundle, squadBattleCharacterSpawnData.assetBundleName);
		effectSequenceList.RequestLoadedCallback(EffectsLoadedCallback, squadBattleCharacterSpawnData);
		gameObject2.AddComponent(typeof(CharacterGlobals));
	}

	public void EffectsLoadedCallback(EffectSequenceList fxList, object returnObject)
	{
		SquadBattleCharacterSpawnData squadBattleCharacterSpawnData = returnObject as SquadBattleCharacterSpawnData;
		DataWarehouse spawnData = squadBattleCharacterSpawnData.spawnData;
		string text = spawnData.TryGetString("//character_model/effect_name", null);
		if (text != null)
		{
			Object effectSequencePrefabByName = fxList.GetEffectSequencePrefabByName(text);
			if (effectSequencePrefabByName == null)
			{
				CspUtils.DebugLog("No effect prefab found in character assets for <" + text + ">.  Cannot spawn effect.");
			}
			else
			{
				GameObject child = Object.Instantiate(effectSequencePrefabByName) as GameObject;
				Utils.AttachGameObject(squadBattleCharacterSpawnData.obj, child);
			}
		}
		CreateColliders(squadBattleCharacterSpawnData);
	}

	public void CreateColliders(SquadBattleCharacterSpawnData characterSpawnData)
	{
		CombatController combatController = characterSpawnData.obj.GetComponent(typeof(CombatController)) as CombatController;
		if (combatController == null)
		{
			CspUtils.DebugLog("Cannot find Combat Controller in CreateColliders on <" + characterSpawnData.obj.name + ">.");
			return;
		}
		foreach (DataWarehouse item in characterSpawnData.spawnData.GetIterator("//character_model/collider"))
		{
			string @string = item.GetString("name");
			string colliderPrefabName = item.TryGetString("collider_prefab", "_AttackCollider");
			string string2 = item.GetString("node");
			Vector3 offset = item.TryGetVector("offset", new Vector3(0f, 0f, 0f));
			float scale = item.TryGetFloat("scale", 1f);
			combatController.addCollider(colliderPrefabName, string2, @string, offset, scale);
		}
		characterSpawned(characterSpawnData);
	}

	protected void characterSpawned(SquadBattleCharacterSpawnData characterSpawnData)
	{
		GameObject obj = characterSpawnData.obj;
		SquadBattleCharacterAI squadBattleCharacterAI = obj.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
		squadBattleCharacterAI.owningPlayer = playerEnum;
		foreach (KeyValuePair<string, GameObject> item in charactersByName)
		{
			Physics.IgnoreCollision(obj.collider, item.Value.collider);
		}
		bool flag = characterSpawnData.spawnLocator != spawnLocator;
		if (characterSpawnData.homeLocator != null)
		{
			charactersByName.Add(characterSpawnData.characterName, obj);
			characterSpawnData.homeLocator.keeperCharacter = obj;
			if (characterSpawnData.homeLocator == keeperLocators[0])
			{
				AppShell.Instance.EventMgr.Fire(this, new CardGameEvent.AvatarSpawned(characterSpawnData, playerEnum));
			}
			locatorsByCharacter.Add(characterSpawnData.characterName, characterSpawnData.homeLocator);
			squadBattleCharacterAI.homeLocator = characterSpawnData.homeLocator;
		}
		else
		{
			if (!flag)
			{
				temporaryCharacters[0] = obj;
			}
			else
			{
				temporaryCharacters[1] = obj;
			}
			squadBattleCharacterAI.homeLocator = null;
		}
		if (characterSpawnData.onSpawned != null)
		{
			characterSpawnData.onSpawned(obj);
		}
		if (characterSpawnData.action != null)
		{
			SquadBattleCharacterController.Instance.BeginAction(obj, characterSpawnData.action);
		}
		if (characterSpawnData.approachTarget != null)
		{
			squadBattleCharacterAI.ApproachTarget(characterSpawnData.approachTarget, !flag);
		}
	}

	public bool DoSomethingHappy(GameObject notThisGuy, GameObject norThisGuy)
	{
		bool result = false;
		foreach (KeyValuePair<string, GameObject> item in charactersByName)
		{
			if (item.Value != notThisGuy && item.Value != norThisGuy)
			{
				result = true;
				SquadBattleCharacterAI squadBattleCharacterAI = item.Value.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
				squadBattleCharacterAI.DoSomethingHappy();
			}
		}
		return result;
	}

	public bool DoSomethingAngry(GameObject notThisGuy)
	{
		bool result = false;
		foreach (KeyValuePair<string, GameObject> item in charactersByName)
		{
			if (item.Value != notThisGuy)
			{
				result = true;
				SquadBattleCharacterAI squadBattleCharacterAI = item.Value.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
				squadBattleCharacterAI.DoSomethingAngry();
			}
		}
		return result;
	}

	public void KillCharacters(GameObject killer)
	{
		foreach (KeyValuePair<string, GameObject> item in charactersByName)
		{
			SquadBattleCharacterAI squadBattleCharacterAI = item.Value.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
			squadBattleCharacterAI.Defeat();
			if (item.Value == getAvatar())
			{
				squadBattleCharacterAI.Killed(killer);
			}
		}
		GameObject[] array = temporaryCharacters;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				SquadBattleCharacterAI squadBattleCharacterAI2 = gameObject.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
				squadBattleCharacterAI2.Defeat();
			}
		}
	}

	public void VictoryCharacters()
	{
		foreach (KeyValuePair<string, GameObject> item in charactersByName)
		{
			SquadBattleCharacterAI squadBattleCharacterAI = item.Value.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
			squadBattleCharacterAI.Victory();
		}
		GameObject[] array = temporaryCharacters;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				SquadBattleCharacterAI squadBattleCharacterAI2 = gameObject.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
				squadBattleCharacterAI2.Victory();
			}
		}
	}
}
