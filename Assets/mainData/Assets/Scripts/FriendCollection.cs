using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using UnityEngine;

public class FriendCollection : ShsCollectionBase<Friend>
{
	public delegate void SentInviteStatusChecked(bool isInSentList);

	protected const string ELEMENT_NAME = "friend";

	protected const string KEY_NAME = "friends";

	private BlockedCollection availableBlocked;

	private List<Friend> availablePending;

	private float ServerInvitationDelay = 1f;

	public BlockedCollection AvailableBlocked
	{
		get
		{
			return availableBlocked;
		}
	}

	public List<Friend> AvailablePending
	{
		get
		{
			return availablePending;
		}
	}

	public FriendCollection()
	{
		collectionElementName = "friend";
		keyName = "friends";
		availableBlocked = new BlockedCollection();
		availablePending = new List<Friend>();
		AppShell.Instance.EventMgr.AddListener<FriendUpdateMessage>(OnFriendUpdate);
		AppShell.Instance.EventMgr.AddListener<FriendRequestMessage>(OnFriendRequest);
		AppShell.Instance.EventMgr.AddListener<FriendAcceptedMessage>(OnFriendAccepted);
		AppShell.Instance.EventMgr.AddListener<FriendDeclinedMessage>(OnFriendDeclined);
	}

	public FriendCollection(DataWarehouse data)
		: this()
	{
		InitializeFromData(data);
	}

	~FriendCollection()
	{
		AppShell.Instance.EventMgr.RemoveListener<FriendUpdateMessage>(OnFriendUpdate);
		AppShell.Instance.EventMgr.RemoveListener<FriendRequestMessage>(OnFriendRequest);
		AppShell.Instance.EventMgr.RemoveListener<FriendAcceptedMessage>(OnFriendAccepted);
		AppShell.Instance.EventMgr.RemoveListener<FriendDeclinedMessage>(OnFriendDeclined);
	}

	public void OnFriendsLoaded(ShsWebResponse response)
	{
		if (response.Status != 200)
		{
			CspUtils.DebugLog("Inventory request failed for <" + response.RequestUri + "> with status " + response.Status);
			return;
		}
		Clear();
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		DataWarehouse data = dataWarehouse.GetData("//friends");
		InitializeFromData(data);
		availableBlocked.OnBlockedLoaded(dataWarehouse);
		AppShell.Instance.EventMgr.Fire(this, new FriendListUpdatedMessage(FriendListUpdatedMessage.Type.Reload));
	}

	public void ReloadFriendList()
	{
		AppShell.Instance.WebService.StartRequest("resources$users/friends.py", OnFriendsLoaded);
	}

	public void OnFriendRequest(FriendRequestMessage msg)
	{
		availablePending.Add(new Friend(msg.FriendName, msg.FriendID, string.Empty, true, true));
	}

	public void OnFriendAccepted(FriendAcceptedMessage msg)
	{
		availablePending.RemoveAll(delegate(Friend f)
		{
			return f.Id == msg.FriendID;
		});
		AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_event", "friend_accepted", 1, -10000, -10000, string.Empty, string.Empty);
	}

	public void OnFriendDeclined(FriendDeclinedMessage msg)
	{
		availablePending.RemoveAll(delegate(Friend f)
		{
			return f.Id == msg.FriendID;
		});
	}

	private void OnFriendUpdate(FriendUpdateMessage message)
	{
		int friendID = message.FriendID;
		if (!ContainsKey(friendID.ToString()))
		{
			CspUtils.DebugLog("Friend Update for friend: " + message.FriendID + ", but it's not in the friend container.");
			return;
		}
		Friend friend = this[friendID.ToString()];
		friend.Online = (message.Status == "online");
		friend.AvailableForActivity = message.Available;
		friend.Location = message.Location;
		AppShell.Instance.EventMgr.Fire(this, new FriendListUpdatedMessage(FriendListUpdatedMessage.Type.Update, message));
	}

	public void RemoveFriend(int id)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("target", id);
		wWWForm.AddField("rel", "friend");
		if (AppShell.Instance.Profile != null)
		{
			AppShell.Instance.WebService.StartRequest("resources$users/friendsremove.py", OnFriendsRemoved, wWWForm.data);
		}
		Remove(id.ToString());
	}

	public void AddFriend(int id)
	{
		AddFriend(id, null);
	}

	public void AddFriend(string name)
	{
		AddFriend(-1, name);
	}

	private float GetLastFriendRequestTime()
	{
		if (AppShell.Instance.SharedHashTable["FriendListLastInviteTime"] != null)
		{
			return (float)AppShell.Instance.SharedHashTable["FriendListLastInviteTime"];
		}
		return 0f;
	}

	private void SetLastInviteTime()
	{
		AppShell.Instance.SharedHashTable["FriendListLastInviteTime"] = Time.time;
	}

	private void CancelLastInviteTime()
	{
		AppShell.Instance.SharedHashTable["FriendListLastInviteTime"] = 0;
	}

	private float GetFriendInviteTimeRemainingCooldown()
	{
		return Time.time - GetLastFriendRequestTime();
	}

	public bool IsFriendInviteCooldownReady()
	{
		return GetFriendInviteTimeRemainingCooldown() >= ServerInvitationDelay;
	}

	public void FireFriendCooldownIncompleteDialog()
	{
		float friendInviteTimeRemainingCooldown = GetFriendInviteTimeRemainingCooldown();
		string text = AppShell.Instance.stringTable["#FriendsList_FriendCooldownIncomplete"] + Environment.NewLine + Environment.NewLine + AppShell.Instance.stringTable["#FriendsList_FriendCooldownTimeRemaining"] + Math.Max(1f, Mathf.Round(ServerInvitationDelay - friendInviteTimeRemainingCooldown)).ToString();
		GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, text, delegate
		{
		}, GUIControl.ModalLevelEnum.Default);
	}

	private void AddFriend(int id, string name)
	{
		bool flag = true;
		if (availablePending.TrueForAll(delegate(Friend test)
		{
			return (!string.IsNullOrEmpty(name)) ? (test.Name != name) : (test.Id != id);
		}))
		{
			if (!string.IsNullOrEmpty(name))
			{
				foreach (Friend value in base.Values)
				{
					if (value.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					{
						GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, "#FriendsList_FriendAlready", delegate
						{
						}, GUIControl.ModalLevelEnum.Default);
						return;
					}
				}
			}
			flag = false;
			if (!IsFriendInviteCooldownReady())
			{
				FireFriendCooldownIncompleteDialog();
				return;
			}
			SetLastInviteTime();
		}
		WWWForm wWWForm = new WWWForm();
		if (string.IsNullOrEmpty(name))
		{
			wWWForm.AddField("target", id);
		}
		else
		{
			wWWForm.AddField("target_name", name);
		}
		if (AppShell.Instance.Profile != null)
		{
			ShsWebService.ShsWebServiceCallback callback = (!flag) ? new ShsWebService.ShsWebServiceCallback(OnFriendAddedInvite) : new ShsWebService.ShsWebServiceCallback(OnFriendAddedAccept);
			AppShell.Instance.WebService.StartRequest("resources$users/friendsadd.py", callback, wWWForm.data);
		}
	}

	public void DeclineFriend(int id)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("target", id);
		wWWForm.AddField("rel", "friend");
		if (AppShell.Instance.Profile != null)
		{
			AppShell.Instance.WebService.StartRequest("resources$users/friends_decline.py", OnFriendDeclined, wWWForm.data);
		}
		availablePending.RemoveAll(delegate(Friend f)
		{
			return f.Id == id;
		});
		Remove(id.ToString());
	}

	public void OnFriendDeclined(ShsWebResponse response)
	{
		if (response.Status != 200)
		{
			CspUtils.DebugLog("Decline Friend Request Failed for<" + response.RequestUri + "> with status " + response.Status);
		}
		else
		{
			ReloadFriendList();
		}
	}

	public void OnFriendAddedInvite(ShsWebResponse response)
	{
		CspUtils.DebugLog("OnFriendAddedInvite " + response);
		AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_event", "invite_friend", 1, string.Empty);
		OnFriendsAdded(response, true);
	}

	public void OnFriendAddedAccept(ShsWebResponse response)
	{
		OnFriendsAdded(response, false);
	}

	public void OnFriendsAdded(ShsWebResponse response, bool SendConfirmMessage)
	{
		if (response.Status != 200)
		{
			CspUtils.DebugLog("Add Friend Failed for<" + response.RequestUri + "> with status " + response.Status);
			if (response.Body == "ERR_SOURCE_TOO_MANY_FRIENDS")
			{
				AppShell.Instance.CriticalError(SHSErrorCodes.Code.FriendsRequestFailedTooManyFriendsSource);
			}
			else if (response.Body == "ERR_TARGET_TOO_MANY_FRIENDS")
			{
				AppShell.Instance.CriticalError(SHSErrorCodes.Code.FriendsRequestFailedTooManyFriendsTarget);
			}
			else if (response.Body == "ERR_TOO_MANY_INVITES")
			{
				AppShell.Instance.CriticalError(SHSErrorCodes.Code.FriendsRequestFailedTooManyInvites);
			}
			else if (response.Body == "ERR_NOT_ALLOWED")
			{
				AppShell.Instance.CriticalError(SHSErrorCodes.Code.FriendsRequestFailedInvitingFriendNotAllowed);
			}
			else if (response.Body == "ERR_UNKNOWN_PLAYER")
			{
				GUIManager.Instance.ShowErrorDialog(SHSErrorCodes.Code.FriendsRequestFailedNoMatchingPlayerFound, "no friend by that name.");
			}
			else if (response.Body == "ERR_NOT_ONLINE")
			{
				GUIManager.Instance.NotificationManager.Display(GUINotificationManager.GUINotificationStyleEnum.FriendOfflineNotify, "#friend_request_playername_notonline");
			}
			else
			{
				CspUtils.DebugLog(response.Body);
			}
		}
		else
		{
			if (response.Body == "OK" && SendConfirmMessage)
			{
				GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, "#FriendsList_FriendRequestSent", delegate
				{
				}, GUIControl.ModalLevelEnum.Default);
			}
			ReloadFriendList();
		}
		if (SendConfirmMessage)
		{
			ServerInvitationDelay = 5f;
		}
	}

	public void OnFriendsRemoved(ShsWebResponse response)
	{
		if (response.Status != 200)
		{
			CspUtils.DebugLog("Kick Friend Failed for<" + response.RequestUri + "> with status " + response.Status);
		}
		else
		{
			ReloadFriendList();
		}
	}

	public void OnFriendsDeclined(ShsWebResponse response)
	{
		if (response.Status != 200)
		{
			CspUtils.DebugLog("Decline Friend Failed for<" + response.RequestUri + "> with status " + response.Status);
		}
		else
		{
			ReloadFriendList();
		}
	}

	public bool IsPlayerBlocked(GameObject player)
	{
		if (player != null)
		{
			NetworkComponent component = player.GetComponent<NetworkComponent>();
			PlayerDictionary.Player value;
			if (component != null && AppShell.Instance.PlayerDictionary.TryGetValue(component.goNetId.ChildId, out value))
			{
				foreach (Friend value2 in AvailableBlocked.Values)
				{
					if (value2 != null && value2.Id == value.PlayerId)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public void RemoveBlocked(int id)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("target", id);
		wWWForm.AddField("rel", "ignore");
		if (AppShell.Instance.Profile != null)
		{
			AppShell.Instance.WebService.StartRequest("resources$users/friends_remove.py", OnBlockedRemoved, wWWForm.data);
		}
		Remove(id.ToString());
		availableBlocked.Remove(id.ToString());
	}

	public void AddBlocked(int id)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("target", id);
		if (AppShell.Instance.Profile != null)
		{
			AppShell.Instance.WebService.StartRequest("resources$users/friends_add_ignore.py", OnBlockedAdded, wWWForm.data);
		}
	}

	public void OnBlockedAdded(ShsWebResponse response)
	{
		if (response.Status != 200)
		{
			CspUtils.DebugLog("Add Friend Failed for<" + response.RequestUri + "> with status " + response.Status);
		}
		else
		{
			ReloadFriendList();
		}
	}

	public void OnBlockedRemoved(ShsWebResponse response)
	{
		if (response.Status != 200)
		{
			CspUtils.DebugLog("Kick Friend Failed for<" + response.RequestUri + "> with status " + response.Status);
		}
		else
		{
			ReloadFriendList();
		}
	}

	public void IsPlayerInSentInvites(int friendID, SentInviteStatusChecked onSentInviteStatusChecked)
	{
		AppShell.Instance.WebService.StartRequest("resources$users/friends.py", delegate(ShsWebResponse response)
		{
			if (response.Status != 200)
			{
				onSentInviteStatusChecked(false);
			}
			else
			{
				bool isInSentList = false;
				DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
				dataWarehouse.Parse();
				foreach (XPathNavigator value in dataWarehouse.GetValues("//a/f"))
				{
					DataWarehouse dataWarehouse2 = new DataWarehouse(value);
					if (dataWarehouse2.TryGetInt("id", -1) == friendID)
					{
						isInSentList = true;
						break;
					}
				}
				onSentInviteStatusChecked(isInSentList);
			}
		});
	}

	public bool IsPlayerInBlockedList(int playerId)
	{
		return availableBlocked.ContainsKey(playerId.ToString());
	}

	public bool IsPlayerInFriendList(int playerId)
	{
		return ContainsKey(playerId.ToString());
	}

	public void GoToFriend(int friendID)
	{
		AppShell.Instance.Matchmaker2.JoinGameWorldWithUser(friendID, delegate(Matchmaker2.Ticket ticket)
		{
			if (ticket.status == Matchmaker2.Ticket.Status.SUCCESS)
			{
				CspUtils.DebugLog("Ticket:\n" + ticket.ticket);
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(ticket.ticket);
				string innerText = xmlDocument.SelectSingleNode("/ticket/zone").InnerText;
				string innerText2 = xmlDocument.SelectSingleNode("/ticket/game").InnerText;
				if (innerText2 == Matchmaker2.GameTypeToString(Matchmaker2.GameType.WORLD))
				{
					string value = null;
					if (GameController.GetController() != null && GameController.GetController().LocalPlayer != null)
					{
						value = GameController.GetController().LocalPlayer.name;
					}
					AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "switch_zone", OwnableDefinition.simpleZoneName(innerText), 3f);
					AppShell.Instance.SharedHashTable["SocialSpaceLevel"] = innerText;
					AppShell.Instance.SharedHashTable["SocialSpaceSpawnPoint"] = null;
					AppShell.Instance.SharedHashTable["SocialSpaceCharacter"] = value;
					AppShell.Instance.SharedHashTable["SocialSpaceTicket"] = ticket;
					AppShell.Instance.Transition(GameController.ControllerType.SocialSpace);
				}
				else
				{
					CspUtils.DebugLog("Unsupported: Friend not in the social space");
				}
			}
			else
			{
				CspUtils.DebugLog("Error finding friend " + friendID);
			}
		});
	}
}
