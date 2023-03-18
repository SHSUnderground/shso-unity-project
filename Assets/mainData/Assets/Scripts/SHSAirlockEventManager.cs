using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHSAirlockEventManager
{
	System.Security.Cryptography.MD5CryptoServiceProvider md = null;
        
	public enum ScreenContext
	{
		Selection,
		Summary
	}

	public enum AirlockStateEnum
	{
		Idle,
		Choosing,
		AutoChoosing,
		Locked
	}

	public class AirlockSlot
	{
		public enum PlayerSlotState
		{
			Empty,
			Choosing,
			Ready
		}

		public const int NO_VALUE = -1;

		public const string NO_VALUE_S = "";

		private int slotNum;

		private int userId;

		private string userName;

		private string characterId;

		private int level;

		private int powerNumber;

		private bool isReady;

		private PlayerSlotState state;

		private SHSBrawlerAirlockGadget.AirlockSlotManager data;

		public int SlotNum
		{
			get
			{
				return slotNum;
			}
			set
			{
				slotNum = value;
			}
		}

		public int UserId
		{
			get
			{
				return userId;
			}
			set
			{
				userId = value;
			}
		}

		public string UserName
		{
			get
			{
				return userName;
			}
			set
			{
				userName = value;
				data.DisplayData(this);
			}
		}

		public string CharacterId
		{
			get
			{
				return characterId;
			}
			set
			{
				characterId = value;
				data.DisplayData(this);
			}
		}

		public int Level
		{
			get
			{
				return level;
			}
			set
			{
				level = value;
				data.DisplayData(this);
			}
		}

		public int PowerNumber
		{
			get
			{
				return powerNumber;
			}
			set
			{
				powerNumber = value;
				data.DisplayData(this);
			}
		}

		public bool IsReady
		{
			get
			{
				return isReady;
			}
			set
			{
				isReady = value;
			}
		}

		public PlayerSlotState State
		{
			get
			{
				return state;
			}
			set
			{
				state = value;
				data.DisplayData(this);
			}
		}

		public AirlockSlot(SHSBrawlerAirlockGadget.AirlockSlotManager data)
		{
			this.data = data;
			userId = -1;
			level = -1;
			powerNumber = -1;
			userName = string.Empty;
			characterId = string.Empty;
		}

		public void Reset()
		{
			State = PlayerSlotState.Empty;
			data.ClearPlayer(this);
			userId = -1;
			level = -1;
			powerNumber = -1;
			userName = string.Empty;
			characterId = string.Empty;
		}
	}

	public delegate void OnRoomUserDrop();

	public const string XP_KEY = "xp";

	public const string LEVEL_KEY = "level";

	public const string R2_KEY = "r2Attack";

	public const string READY_KEY = "ready";

	private AirlockStateEnum airlockState;

	private CharacterSelectionBlock pendingCharacter;

	private int roomCapacity = 4;

	private int currentCapacity;

	private int playerReadyCount;

	private ScreenContext context;

	private Dictionary<string, PowerAttackData> characterR2Info;

	private List<AirlockSlot> slots;

	private OnRoomUserDrop onUserDrop;

	private Hashtable lockedHeroes = new Hashtable();

	private SHSBrawlerAirlockGadget.AirlockSlotManager data;

	private bool hostIsReady;

	public AirlockStateEnum AirlockState
	{
		get
		{
			return airlockState;
		}
		set
		{
			airlockState = value;
		}
	}

	public CharacterSelectionBlock PendingCharacter
	{
		get
		{
			return pendingCharacter;
		}
		set
		{
			pendingCharacter = value;
		}
	}

	public int RoomCapacity
	{
		get
		{
			return roomCapacity;
		}
		set
		{
			roomCapacity = value;
		}
	}

	public int CurrentCapacity
	{
		get
		{
			return currentCapacity;
		}
	}

	public int PlayerReadyCount
	{
		get
		{
			return playerReadyCount;
		}
	}

	public ScreenContext Context
	{
		get
		{
			return context;
		}
		set
		{
			context = value;
		}
	}

	public Dictionary<string, PowerAttackData> CharacterR2Info
	{
		get
		{
			return characterR2Info;
		}
		set
		{
			characterR2Info = value;
		}
	}

	public Hashtable LockedHeroes
	{
		get
		{
			return lockedHeroes;
		}
	}

	public SHSAirlockEventManager(SHSBrawlerAirlockGadget.AirlockSlotManager data, Dictionary<string, PowerAttackData> characterR2Info, OnRoomUserDrop onDrop)
	{
		CharacterR2Info = characterR2Info;
		onUserDrop = onDrop;
		slots = new List<AirlockSlot>();
		this.data = data;
		for (int i = 0; i < 4; i++)
		{
			AirlockSlot airlockSlot = new AirlockSlot(data);
			airlockSlot.SlotNum = i;
			slots.Add(airlockSlot);
		}
	}

	public void OnShow()
	{
		OnRoomUserListChanged(null);
		AppShell.Instance.EventMgr.AddListener<RoomUserListChangeMessage>(OnRoomUserListChanged);
		AppShell.Instance.EventMgr.AddListener<OwnershipStringMessage>(OnOwnershipChange);
		AppShell.Instance.EventMgr.AddListener<RoomVariableChangeMessage>(OnRoomVarChange);
		AppShell.Instance.EventMgr.AddListener<UserVariableChangeMessage>(OnUserVarChange);
		AppShell.Instance.EventMgr.AddListener<CharacterRequestedMessage>(OnCharRequested);
		AppShell.Instance.EventMgr.AddListener<CharacterSelectedMessage>(OnCharSelected);
		AppShell.Instance.EventMgr.AddListener<BrawlerAutoChooseMessage>(OnAutoChoose);
		AppShell.Instance.ServerConnection.QueryAllOwnership();
		if ((AppShell.Instance.ServerConnection.State & NetworkManager.ConnectionState.ConnectedToGame) == 0)
		{
			CspUtils.DebugLog("Character Selection Panel displayed before being seated.");
		}
		airlockState = AirlockStateEnum.Choosing;
	}

	public void OnHide()
	{
		AppShell.Instance.EventMgr.RemoveListener<OwnershipStringMessage>(OnOwnershipChange);
		AppShell.Instance.EventMgr.RemoveListener<RoomUserListChangeMessage>(OnRoomUserListChanged);
		AppShell.Instance.EventMgr.RemoveListener<RoomVariableChangeMessage>(OnRoomVarChange);
		AppShell.Instance.EventMgr.RemoveListener<UserVariableChangeMessage>(OnUserVarChange);
		AppShell.Instance.EventMgr.RemoveListener<CharacterRequestedMessage>(OnCharRequested);
		AppShell.Instance.EventMgr.RemoveListener<CharacterSelectedMessage>(OnCharSelected);
		AppShell.Instance.EventMgr.RemoveListener<BrawlerAutoChooseMessage>(OnAutoChoose);
		foreach (AirlockSlot slot in slots)
		{
			slot.Reset();
		}
		playerReadyCount = 0;
		currentCapacity = 0;
	}

	private void OnRoomVarChange(RoomVariableChangeMessage message)
	{
	}

	private void OnUserVarChange(UserVariableChangeMessage message)
	{
		AirlockSlot airlockSlot = slots.Find(delegate(AirlockSlot c)
		{
			return c.UserId == message.userId;
		});
		if (airlockSlot == null)
		{
			CspUtils.DebugLog("User variable changed for unknown user: " + message.userId);
			return;
		}
		if (message.changedVars.Contains("xp"))
		{
			IServerConnection serverConnection = AppShell.Instance.ServerConnection;
			int result = 0;
			if (!int.TryParse(serverConnection.GetUserVariable(message.userId, "xp").ToString(), out result))
			{
				CspUtils.DebugLog("Cant' retrieve xp value: " + serverConnection.GetUserVariable(message.userId, "xp"));
			}
			int num = airlockSlot.Level = XpToLevelDefinition.Instance.GetLevelForXp(result);
		}
		if (message.changedVars.Contains("r2Attack") && characterR2Info != null)
		{
			IServerConnection serverConnection2 = AppShell.Instance.ServerConnection;
			int result2 = 1;
			if (!int.TryParse(serverConnection2.GetUserVariable(message.userId, "r2Attack").ToString(), out result2))
			{
				CspUtils.DebugLog("Cant' retrieve r2 value: " + serverConnection2.GetUserVariable(message.userId, "r2Attack"));
			}
			if (result2 < 1 || result2 > 3)
			{
				result2 = 1;
			}
			airlockSlot.PowerNumber = result2;
		}
		if (message.changedVars.Contains("ready") && (bool)AppShell.Instance.ServerConnection.GetUserVariable(message.userId, "ready"))
		{
			airlockSlot.State = AirlockSlot.PlayerSlotState.Ready;
			if (airlockSlot.UserId == AppShell.Instance.ServerConnection.GetGameUserId())
			{
				AppShell.Instance.EventMgr.Fire(null, new CharacterReadyMessage(new CharacterSelectionBlock(airlockSlot.CharacterId, airlockSlot.PowerNumber)));
			}
			if (++playerReadyCount == currentCapacity)
			{
			}
			if (airlockSlot.UserId == AppShell.Instance.ServerConnection.GetGameHostId())
			{
				hostIsReady = true;
				LockRoom();
			}
		}
	}

	private void OnAutoChoose(BrawlerAutoChooseMessage message)
	{
		airlockState = AirlockStateEnum.AutoChoosing;
	}

	private void OnCharRequested(CharacterRequestedMessage message)
	{
		pendingCharacter = message.data;
	}

	private void OnCharSelected(CharacterSelectedMessage message)
	{
		CspUtils.DebugLog("OnCharSelected has secondary attack of: " + message.R2Attack.ToString());
		pendingCharacter = null;
	}

	private List<AirlockSlot> getAirlockSlots()
	{
		return slots;
	}

	public AirlockSlot GetAirlockSlot(int userId)
	{
		foreach (AirlockSlot slot in slots)
		{
			if (slot.UserId == userId)
			{
				return slot;
			}
		}
		return null;
	}

	private void LockRoom()
	{
		if (AppShell.Instance.ServerConnection.IsGameHost())
		{
			AppShell.Instance.ServerConnection.LockRoom();
		}
	}

	private void OnRoomUserListChanged(RoomUserListChangeMessage message)
	{
		List<int> list = getAirlockSlots().ConvertAll(delegate(AirlockSlot control)
		{
			return control.UserId;
		});
		List<NetworkManager.UserInfo> gameAllUsers = AppShell.Instance.ServerConnection.GetGameAllUsers();
		using (List<NetworkManager.UserInfo>.Enumerator enumerator = gameAllUsers.GetEnumerator())
		{
			NetworkManager.UserInfo user;
			while (enumerator.MoveNext())
			{
				user = enumerator.Current;
				CspUtils.DebugLog("OnRoomUserListChanged user=" + user.userName);
				AirlockSlot airlockSlot = slots.Find(delegate(AirlockSlot c)
				{
					return c.UserId == user.userId;
				});
				if (airlockSlot != null)
				{	
					CspUtils.DebugLog("removing From list user=" + airlockSlot.UserName);
					list.Remove(airlockSlot.UserId);
				}
				else if (currentCapacity == roomCapacity)
				{	
					CspUtils.DebugLog("Capacity would be exceeded by adding: " + user.userId + ":" + user.userName + " to the room.");
				}
				else
				{
					bool flag = false;
					foreach (AirlockSlot slot in slots)
					{
						if (slot.State == AirlockSlot.PlayerSlotState.Empty)
						{
							slot.State = AirlockSlot.PlayerSlotState.Choosing;
							slot.UserId = user.userId;
							slot.UserName = user.userName;
							flag = true;
							currentCapacity++;
							if (user.userId == AppShell.Instance.ServerConnection.GetGameHostId())
							{
								SHSBrawlerAirlockGadget.Player player = data.Airlock.BaseInfo.airlockSlotManager.FindPlayer(user.userId);
								data.Airlock.SetVisualHostStatus(player);
								data.Airlock.SetIsHost(player, true);
							}
							if (user.userName == "<unknown>" && !data.Airlock.BaseInfo.AttemptingSquadNameFetch)
							{
								// block temporarily commented out by CSP, as it was causing problems when trying to have more than 2 players in a mission.
								//user.userName = string.Empty;
								//data.Airlock.BaseInfo.FetchSquadNames(slots);
							}
							break;
						}
					}
					if (!flag)
					{
						CspUtils.DebugLog("All slots filled for this brawl. How did this happen?");
					}
				}
			}
		}
		bool flag2 = false;
		using (List<int>.Enumerator enumerator3 = list.GetEnumerator())
		{
			int userId;
			while (enumerator3.MoveNext())
			{
				userId = enumerator3.Current;
				if (userId != -1)
				{
					AirlockSlot airlockSlot2 = slots.Find(delegate(AirlockSlot c)
					{
						return c.UserId == userId;
					});
					if (airlockSlot2.State == AirlockSlot.PlayerSlotState.Ready)
					{
						playerReadyCount--;
					}
					int gameHostId = AppShell.Instance.ServerConnection.GetGameHostId();
					if (gameHostId == userId && !hostIsReady)
					{
						data.Airlock.BaseInfo.ReappointNewHost(userId, airlockSlot2.UserName);
						LockRoom();
					}
					flag2 = true;
					airlockSlot2.State = AirlockSlot.PlayerSlotState.Empty;
					airlockSlot2.Reset();
				}
			}
		}
		lockedHeroes.Clear();
		data.refreshBlacklistedHeroes();
		foreach (AirlockSlot slot2 in slots)
		{
			if (slot2.CharacterId != string.Empty)
			{
				CspUtils.DebugLog("adding " + slot2.CharacterId + " to the ban list");
				lockedHeroes.Add(slot2.CharacterId, true);
				CspUtils.DebugLog("ban list is now ");
				foreach (string key in lockedHeroes.Keys)
				{
					CspUtils.DebugLog("   " + key);
				}
			}
		}
		if (flag2 && onUserDrop != null)
		{
			onUserDrop();
		}
	}

	private void OnOwnershipChange(OwnershipStringMessage message)
	{
		List<AirlockSlot> airlockSlots = getAirlockSlots();
		if (message.ownerId != -2 && message.ownerId != -3)
		{
			AirlockSlot airlockSlot = airlockSlots.Find(delegate(AirlockSlot s)
			{
				return s.UserId == message.ownerId;
			});
			if (airlockSlot != null)
			{
				airlockSlot.CharacterId = message.strings[0];
				if (Context != 0)
				{
					return;
				}
				if (pendingCharacter != null && message.strings[0] == pendingCharacter.name)
				{
					if (message.ownerId == AppShell.Instance.ServerConnection.GetGameUserId())
					{
						UserProfile profile = AppShell.Instance.Profile;
						if (profile != null)
						{
							if (!profile.AvailableCostumes.ContainsKey(profile.LastSelectedCostume))
							{
								CspUtils.DebugLog(profile.LastSelectedCostume + " is not in the list of Available Costumes.");
							}
							else
							{
								CspUtils.DebugLog("Setting user variables now. " + profile.LastSelectedCostume + " " + profile.AvailableCostumes[profile.LastSelectedCostume].Xp + " " + pendingCharacter.r2Attack);
								AppShell.Instance.ServerConnection.SetUserVariable("xp", profile.AvailableCostumes[profile.LastSelectedCostume].Xp);
								AppShell.Instance.ServerConnection.SetUserVariable("r2Attack", pendingCharacter.r2Attack);
								Hashtable hashtable = new Hashtable();
								hashtable["xp"] = true;
								hashtable["r2Attack"] = true;
								if (!lockedHeroes.ContainsKey(message.strings[0]))
								{
									hashtable["ready"] = true;
									AppShell.Instance.ServerConnection.SetUserVariable("ready", true);
								}
								UserVariableChangeMessage msg = new UserVariableChangeMessage(message.ownerId, hashtable);
								AppShell.Instance.EventMgr.Fire(null, msg);
							}
						}
						else
						{
							CspUtils.DebugLog("Cant obtain costume to get xp and level from. No profile available.");
						}
						CharacterSelectedMessage characterSelectedMessage = new CharacterSelectedMessage(pendingCharacter);
						characterSelectedMessage.sender = this;
						AppShell.Instance.EventMgr.Fire(null, characterSelectedMessage);
					}
					else if (airlockState == AirlockStateEnum.AutoChoosing)
					{
						CspUtils.DebugLog("Auto choose failed for: " + pendingCharacter + ". Re-choosing");
						if (!lockedHeroes.ContainsKey(message.strings[0]))
						{
							lockedHeroes.Add(message.strings[0], true);
							CspUtils.DebugLog("adding " + message.strings[0] + " to the ban list");
							CspUtils.DebugLog("ban list is now ");
							foreach (string key in lockedHeroes.Keys)
							{
								CspUtils.DebugLog("   " + key);
							}
						}
						data.Airlock.BaseInfo.p1SelectedHero = string.Empty;
						AppShell.Instance.EventMgr.Fire(this, new BrawlerAutoChooseMessage());
					}
				}
				else
				{
					bool flag = false;
					Hashtable hashtable2 = new Hashtable();
					object userVariable = AppShell.Instance.ServerConnection.GetUserVariable(message.ownerId, "xp");
					if (userVariable != null)
					{
						flag = true;
						hashtable2["xp"] = true;
					}
					object userVariable2 = AppShell.Instance.ServerConnection.GetUserVariable(message.ownerId, "r2Attack");
					if (userVariable2 != null)
					{
						flag = true;
						hashtable2["r2Attack"] = true;
					}
					object userVariable3 = AppShell.Instance.ServerConnection.GetUserVariable(message.ownerId, "ready");
					if (userVariable3 != null)
					{
						flag = true;
						hashtable2["ready"] = true;
					}
					if (flag)
					{
						UserVariableChangeMessage msg2 = new UserVariableChangeMessage(message.ownerId, hashtable2);
						AppShell.Instance.EventMgr.Fire(null, msg2);
					}
				}
				lockedHeroes.Clear();
				data.refreshBlacklistedHeroes();
				foreach (AirlockSlot slot in slots)
				{
					if (slot.CharacterId != string.Empty && !lockedHeroes.ContainsKey(slot.CharacterId))
					{
						lockedHeroes.Add(slot.CharacterId, true);
					}
				}
			}
			else
			{
				CspUtils.DebugLog("Ownership change message for unknown owner: " + message.ownerId.ToString());
			}
		}
		else
		{
			CspUtils.DebugLog("It got here, because the character was already taken up from the server.");
		}
	}
}
