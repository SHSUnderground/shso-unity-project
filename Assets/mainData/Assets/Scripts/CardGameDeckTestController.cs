using CardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[AddComponentMenu("CardGame/Controller")]
public class CardGameDeckTestController : GameController
{
	protected static CardGameDeckTestController instance;

	public List<string> player0Decks;

	public List<string> player1Decks;

	public int trials = 1;

	public bool finished;

	private bool gameStarted;

	private CardGamePlayer[] players = new CardGamePlayer[2];

	public static CardGameDeckTestController Instance
	{
		get
		{
			return instance;
		}
	}

	public bool GameStarted
	{
		get
		{
			return gameStarted;
		}
		set
		{
			gameStarted = value;
		}
	}

	public override void Awake()
	{
		base.Awake();
		instance = this;
		bCallControllerReadyFromStart = false;
		if (player0Decks == null || player0Decks.Count < 1 || player1Decks == null || player1Decks.Count < 1)
		{
			finished = true;
		}
		else
		{
			StartCoroutine(Initialize());
		}
	}

	public override void OnEnable()
	{
		base.OnEnable();
		instance = this;
	}

	public override void OnDisable()
	{
		base.OnDisable();
		instance = null;
	}

	public override void OnOldControllerUnloading(AppShell.GameControllerTypeData currentGameData, AppShell.GameControllerTypeData newGameData)
	{
		base.OnOldControllerUnloading(currentGameData, newGameData);
		RoomAgent.Disconnect();
	}

	private IEnumerator Initialize()
	{
		StartInfo start = new StartInfo();
		gameStarted = false;
		players = CreatePlayers(start.Players);
		CspUtils.DebugLog("Players created: " + players[0].Hero + " vs " + players[1].Hero);
		MultiCoroutine deckLoader = new MultiCoroutine();
		CardGamePlayer[] array = players;
		foreach (CardGamePlayer p in array)
		{
			deckLoader.Add(p.LoadDeck());
		}
		yield return StartCoroutine(deckLoader);
		try
		{
			deckLoader.Throw();
		}
		catch (Exception ex2)
		{
			CspUtils.DebugLog(ex2.Message);
			Shutdown();
			yield break;
		}
		CspUtils.DebugLog("Decks loaded");
		MultiCoroutine startupHandler = new MultiCoroutine(ConnectToRoom(start));
		yield return StartCoroutine(startupHandler);
		try
		{
			startupHandler.Throw();
		}
		catch (Exception ex)
		{
			CspUtils.DebugLog(ex.Message);
			Shutdown();
			yield break;
		}
		CardGamePlayer[] array2 = players;
		foreach (CardGamePlayer p2 in array2)
		{
			if (p2.Info.Type != PlayerType.Network)
			{
				p2.SendReady(-1, string.Empty, null);
			}
		}
		ControllerReady();
	}

	private void Shutdown()
	{
		RoomAgent.Disconnect();
	}

	private IEnumerator ConnectToRoom(StartInfo start)
	{
		while (AppShell.Instance.SharedHashTable["CardGameTicket"] == null)
		{
			yield return new WaitForEndOfFrame();
		}
		Matchmaker2.Ticket ticket = (Matchmaker2.Ticket)AppShell.Instance.SharedHashTable["CardGameTicket"];
		AppShell.Instance.SharedHashTable["CardGameTicket"] = null;
		if (ticket.status == Matchmaker2.Ticket.Status.SUCCESS)
		{
			if (AppShell.Instance.ServerConfig.TryGetBool("//custom_smartfox_server", false))
			{
				string whichEnv = AppShell.Instance.ServerConfig.TryGetString("//environment", "DEV");
				string server = ticket.server = AppShell.Instance.ServerConfig.TryGetString("//" + whichEnv + "/smartfox/customserver", null);
			}
			CspUtils.DebugLog("Card game server " + ticket.server + " | ticket: " + ticket.ticket);
			start.RoomTicket = ticket;
			MultiCoroutine connector = new MultiCoroutine(RoomAgent.Connect(start));
			yield return StartCoroutine(connector);
			try
			{
				connector.Throw();
				CspUtils.DebugLog("Connected to room");
			}
			catch (Exception ex)
			{
				CspUtils.DebugLog("Card game failed connecting: " + ex.Message);
			}
			RoomAgent.SetPlayers(players);
			CspUtils.DebugLog("Players selected");
			yield break;
		}
		throw new Exception("Failed to acquire a ticket");
	}

	private IEnumerator PreloadHeroes()
	{
		Dictionary<string, bool> heroSet = new Dictionary<string, bool>();
		CardGamePlayer[] array = players;
		foreach (CardGamePlayer player in array)
		{
			heroSet[player.HeroPrefab] = true;
			foreach (BattleCard card in player.Deck)
			{
				string[] internalHeroName = card.InternalHeroName;
				foreach (string hero in internalHeroName)
				{
					heroSet[Util.GetPrefabHeroName(hero)] = true;
				}
			}
		}
		GameDataManager dataManager = AppShell.Instance.DataManager;
		Dictionary<string, GameDataLoadResponse> dataLoadResponses = new Dictionary<string, GameDataLoadResponse>();
		foreach (string hero2 in heroSet.Keys)
		{
			dataManager.LoadGameData("Characters/" + hero2, delegate(GameDataLoadResponse response, object extraData)
			{
				string key = (string)extraData;
				dataLoadResponses[key] = response;
			}, hero2);
		}
		while (dataLoadResponses.Count < heroSet.Count)
		{
			yield return new WaitForEndOfFrame();
		}
		AssetBundleLoader loader = AppShell.Instance.BundleLoader;
		int remainingHeroes = heroSet.Keys.Count;
		foreach (KeyValuePair<string, GameDataLoadResponse> response2 in dataLoadResponses)
		{
			if (!string.IsNullOrEmpty(response2.Value.Error))
			{
				throw new FileLoadException(string.Format("Error loading hero prefab {0}: {1}", response2.Key, response2.Value.Error));
			}
			DataWarehouse data = response2.Value.Data;
			string bundleName = data.GetString("//asset_bundle");
			string modelName = data.GetString("//character_model/model_name");
			if (!string.IsNullOrEmpty(bundleName) && !string.IsNullOrEmpty(modelName))
			{
				loader.LoadAsset(bundleName, modelName, null, delegate
				{
					remainingHeroes--;
				});
				loader.PreLoadAsset(bundleName, "movement_run");
				DataWarehouse fxData = data.GetData("//effect_sequence_list");
				string characterFxBundleName = fxData.TryGetString("character_fx", string.Empty);
				foreach (DataWarehouse dw in fxData.GetIterator("logical_effect"))
				{
					string fxPrefabName = dw.TryGetString("prefab_name", string.Empty);
					if (!string.IsNullOrEmpty(fxPrefabName) && !string.IsNullOrEmpty(characterFxBundleName))
					{
						loader.PreLoadAsset(characterFxBundleName, fxPrefabName);
					}
				}
			}
		}
		while (remainingHeroes != 0)
		{
			yield return new WaitForEndOfFrame();
		}
	}

	private CardGamePlayer[] CreatePlayers(PlayerInfo[] players)
	{
		CardGamePlayer[] array = new CardGamePlayer[players.Length];
		for (int i = 0; i < array.Length; i++)
		{
			CardGamePlayer cardGamePlayer = null;
			switch (players[i].Type)
			{
			case PlayerType.Human:
				cardGamePlayer = CreatePlayer<CardGameHumanPlayer>();
				break;
			case PlayerType.AI:
				cardGamePlayer = CreatePlayer<CardGameAIPlayer>();
				break;
			case PlayerType.Network:
				cardGamePlayer = CreatePlayer<CardGameNetworkPlayer>();
				break;
			default:
				throw new Exception("Unknown player type encountered");
			}
			cardGamePlayer.Info = players[i];
			array[i] = cardGamePlayer;
		}
		array[0].opponent = array[1];
		array[1].opponent = array[0];
		CspUtils.DebugLog("Players " + array[0].Info.PlayerID + " and " + array[1].Info.PlayerID + " created");
		return array;
	}

	private CardGamePlayer CreatePlayer<T>() where T : CardGamePlayer
	{
		GameObject g = new GameObject();
		return Utils.AddComponent<T>(g);
	}
}
