using System.Collections.Generic;
using UnityEngine;

public class TitleManager
{
	private static Dictionary<int, TitleData> _allTitles = new Dictionary<int, TitleData>();

	private static Dictionary<int, MedallionData> _allMedallions = new Dictionary<int, MedallionData>();

	public static Dictionary<int, bool> _ownedTitles = new Dictionary<int, bool>();

	public static Dictionary<int, bool> _ownedMedallions = new Dictionary<int, bool>();

	private static int _currentTitleID = -1;

	private static int _currentMedallionID = -1;

	public static Dictionary<int, PlayerTitleData> playerTitles = new Dictionary<int, PlayerTitleData>();

	public static int currentTitleID
	{
		get
		{
			return _currentTitleID;
		}
		set
		{
			if (!_ownedTitles.ContainsKey(value))
			{
				value = -1;
			}
			_currentTitleID = value;
			AppShell.Instance.Profile.titleID = value;
			ShsPlayerPrefs.SetInt(ShsPlayerPrefs.Keys.TitleID, value);
			sendUpdate();
			requestTitle(value, AppShell.Instance.ServerConnection.GetGameUserId());
		}
	}

	public static int currentMedallionID
	{
		get
		{
			return _currentMedallionID;
		}
		set
		{
			if (!_ownedMedallions.ContainsKey(value))
			{
				value = -1;
			}
			_currentMedallionID = value;
			AppShell.Instance.Profile.medallionID = value;
			ShsPlayerPrefs.SetInt(ShsPlayerPrefs.Keys.MedallionID, value);
			sendUpdate();
			requestMedallion(value, AppShell.Instance.ServerConnection.GetGameUserId());
		}
	}

	public static Dictionary<int, bool> getOwnedTitles()
	{
		return _ownedTitles;
	}

	public static Dictionary<int, bool> getOwnedMedallions()
	{
		return _ownedMedallions;
	}

	public static void addTitle(int ownableTypeID, string name)
	{
		_allTitles[ownableTypeID] = new TitleData(ownableTypeID, name);
	}

	public static void addMedallion(int ownableTypeID, string name, string textureSource)
	{
		_allMedallions[ownableTypeID] = new MedallionData(ownableTypeID, name, textureSource);
	}

	public static bool ownsTitle(int ownableTypeID)
	{
		return _ownedTitles.ContainsKey(ownableTypeID);
	}

	public static void awardTitle(int ownableTypeID, bool announce)
	{
		_ownedTitles[ownableTypeID] = true;
		if (announce)
		{
			AppShell.Instance.EventMgr.Fire(null, new TitlePurchasedEvent(ownableTypeID));
		}
	}

	public static void awardMedallion(int ownableTypeID, bool announce)
	{
		_ownedMedallions[ownableTypeID] = true;
		if (announce)
		{
			AppShell.Instance.EventMgr.Fire(null, new MedallionPurchasedEvent(ownableTypeID));
		}
	}

	public static TitleData getTitle(int id)
	{
		TitleData value;
		if (_allTitles.TryGetValue(id, out value))
		{
			return value;
		}
		return null;
	}

	public static MedallionData getMedallion(int id)
	{
		MedallionData value;
		if (_allMedallions.TryGetValue(id, out value))
		{
			return value;
		}
		return null;
	}

	public static void init(int playerGoNetID)
	{
		_currentTitleID = AppShell.Instance.Profile.titleID;
		_currentMedallionID = AppShell.Instance.Profile.medallionID;
		if (!_ownedTitles.ContainsKey(_currentTitleID))
		{
			CspUtils.DebugLog("Warning:  Cached Title ID is not one you own, must have switched accounts?");
			_currentTitleID = -1;
		}
		if (!_ownedMedallions.ContainsKey(_currentMedallionID))
		{
			CspUtils.DebugLog("Warning:  Cached Medallion ID is not one you own, must have switched accounts?");
			_currentMedallionID = -1;
		}
		requestTitleAndMedallion(_currentTitleID, _currentMedallionID, playerGoNetID);
		sendUpdate();
	}

	public static void refresh()
	{
		if (playerTitles.ContainsKey(AppShell.Instance.ServerConnection.GetGameUserId()))
		{
			broadcastChange(playerTitles[AppShell.Instance.ServerConnection.GetGameUserId()]);
		}
	}

	private static void sendUpdate()
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("title_id", currentTitleID);
		wWWForm.AddField("medallion_id", currentMedallionID);
		AppShell.Instance.WebService.StartRequest("resources$users/" + AppShell.Instance.Profile.UserId + "/set_title_info", delegate(ShsWebResponse response)
		{
			if (response.Status != 200)
			{
				CspUtils.DebugLog("TitleManager Request failure: " + response.Status + ":" + response.Body);
			}
			else
			{
				SetTitleMessage setTitleMessage = new SetTitleMessage(new GoNetId(GoNetId.PLAYER_ID_FLAG, AppShell.Instance.ServerConnection.GetGameUserId()));
				setTitleMessage.titleID = currentTitleID;
				setTitleMessage.medallionID = currentMedallionID;
				AppShell.Instance.ServerConnection.SendGameMsg(setTitleMessage);
			}
		}, wWWForm.data);
	}

	public static void requestTitleAndMedallion(int titleID, int medallionID, int parentGoNetID)
	{
		requestTitle(titleID, parentGoNetID);
		requestMedallion(medallionID, parentGoNetID);
	}

	public static void requestMedallion(int medallionID, int parentGoNetID)
	{
		if (playerTitles.ContainsKey(parentGoNetID))
		{
			playerTitles[parentGoNetID].medallionID = medallionID;
		}
		else
		{
			playerTitles[parentGoNetID] = new PlayerTitleData(-1, medallionID, parentGoNetID);
		}
		broadcastChange(playerTitles[parentGoNetID]);
	}

	public static void setPrestige(int userID, string hero)
	{
		if (!playerTitles.ContainsKey(userID))
		{
			playerTitles[userID] = new PlayerTitleData(-1, -1, userID);
		}
		playerTitles[userID].prestigeHeroes.Add(hero, true);
	}

	public static bool checkHeroPrestige(int userID, string hero)
	{
		CspUtils.DebugLog("checkHeroPrestige " + userID + " " + hero);
		if (!playerTitles.ContainsKey(userID))
		{
			return false;
		}
		return playerTitles[userID].checkHeroPrestige(hero);
	}

	public static void requestTitle(int titleID, int parentGoNetID)
	{
		if (playerTitles.ContainsKey(parentGoNetID))
		{
			playerTitles[parentGoNetID].titleID = titleID;
		}
		else
		{
			playerTitles[parentGoNetID] = new PlayerTitleData(titleID, -1, parentGoNetID);
		}
		broadcastChange(playerTitles[parentGoNetID]);
	}

	public static void heroCreated(int parentGoNetID)
	{
		if (playerTitles.ContainsKey(parentGoNetID))
		{
			GameController controller = GameController.GetController();
			if (controller is SocialSpaceController)
			{
				broadcastChange(playerTitles[parentGoNetID]);
			}
		}
	}

	public static void broadcastChange(PlayerTitleData data)
	{
		string newTitle = string.Empty;
		string newMedallionSource = string.Empty;
		if (data.titleID > 0)
		{
			OwnableDefinition def = OwnableDefinition.getDef(data.titleID);
			if (def != null)
			{
				newTitle = def.name;
			}
		}
		if (data.medallionID > 0)
		{
			OwnableDefinition def = OwnableDefinition.getDef(data.medallionID);
			if (def != null)
			{
				newMedallionSource = def.metadata;
			}
		}
		AppShell.Instance.EventMgr.Fire(AppShell.Instance, new PlayerChangedSquadInfoMessage(data.parentPlayerID, newTitle, newMedallionSource));
	}
}
