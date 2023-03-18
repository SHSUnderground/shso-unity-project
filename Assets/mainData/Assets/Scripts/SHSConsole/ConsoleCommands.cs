using CardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using UnityEngine;

namespace SHSConsole
{
	internal class ConsoleCommands
	{
		System.Security.Cryptography.MD5CryptoServiceProvider md = null;
        
		[Description("Tim function. Only Tim can wield the magic. ")]
		public void A(string itemId)
		{
			AppShell.Instance.ExpendablesManager.UseExpendable(itemId, delegate(IExpendHandler handler)
			{
				CspUtils.DebugLog(handler.State);
			});
		}

		public void B()
		{
			Dictionary<string, PrerequisiteCheckResult> resultDictionary = new Dictionary<string, PrerequisiteCheckResult>();
			AppShell.Instance.ExpendablesManager.CanExpend(new string[3]
			{
				"77194",
				"77196",
				"23232"
			}, ref resultDictionary);
			foreach (KeyValuePair<string, PrerequisiteCheckResult> item in resultDictionary)
			{
				SHSDebug.DebugLogger log = CspUtils.DebugLog;
				object[] obj = new object[5]
				{
					item.Key,
					":",
					null,
					null,
					null
				};
				PrerequisiteCheckResult value = item.Value;
				obj[2] = value.State;
				obj[3] = ":";
				PrerequisiteCheckResult value2 = item.Value;
				obj[4] = value2.StateExplanation;
				log(string.Concat(obj));
			}
		}

		[Description("Spawn a window of the given type. ")]
		public void ActivateDialog(string dialogTypeName, string text)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Type type = executingAssembly.GetType(dialogTypeName);
			if (type == null)
			{
				CspUtils.DebugLog("Can't get type: " + dialogTypeName + " from executing assembly.");
			}
			else
			{
				GUIManager.Instance.ShowDialog(type, text, delegate(string Id, GUIDialogWindow.DialogState state)
				{
					if (state == GUIDialogWindow.DialogState.Ok)
					{
						CspUtils.DebugLog("Ok ");
					}
				}, GUIControl.ModalLevelEnum.Default);
			}
		}

		[Description("Builds a point from two input values")]
		public Vector2 BuildPoint(float x, float y)
		{
			return new Vector2(x, y);
		}

		[Description("Detect Memory Leaks")]
		public void DetectLeaks()
		{
			AppShell.Instance.gameObject.AddComponent<DetectLeaks>();
		}

		[Description("Echoes the string parameter back to the console")]
		public string Echo(string s)
		{
			return s;
		}

		[Description("Adds a friend with the given Gazillion ID.")]
		public void AddFriend(int friendId)
		{
			AppShell.Instance.Profile.AvailableFriends.AddFriend(friendId);
		}

		[Description("Joins the game instance and zone of the given friend id")]
		public string JoinFriend(int FriendId)
		{
			AppShell.Instance.Matchmaker2.JoinGameWorldWithUser(FriendId, delegate(Matchmaker2.Ticket ticket)
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
						CspUtils.DebugLog("Connecting to friend instance...");
						string value = null;
						if (GameController.GetController() != null && GameController.GetController().LocalPlayer != null)
						{
							value = GameController.GetController().LocalPlayer.name;
						}
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
					CspUtils.DebugLog("Error finding friend " + FriendId);
				}
			});
			return string.Empty;
		}

		[Description("Warps you to the zone with the name you specify (see <name> in spaces.xml)")]
		public void GoToZone(string zone)
		{
			string value = null;
			if (GameController.GetController() != null && GameController.GetController().LocalPlayer != null)
			{
				value = GameController.GetController().LocalPlayer.name;
			}
			AppShell.Instance.SharedHashTable["SocialSpaceLevel"] = zone;
			AppShell.Instance.SharedHashTable["SocialSpaceSpawnPoint"] = null;
			AppShell.Instance.SharedHashTable["SocialSpaceCharacter"] = value;
			AppShell.Instance.Transition(GameController.ControllerType.SocialSpace);
		}

		[Description("Disables player billboards.")]
		public void DisableBillboards()
		{
			PlayerBillboard[] array = Utils.FindObjectsOfType<PlayerBillboard>();
			PlayerBillboard[] array2 = array;
			foreach (PlayerBillboard playerBillboard in array2)
			{
				playerBillboard.gameObject.SetActiveRecursively(false);
			}
		}

		[Description("Shortcut to warp you to Villainville")]
		public void vv()
		{
			GoToZone("Villainville");
		}

		[Description("Teleport to the control room (only works in VillainVille)")]
		public void vvcc()
		{
			TeleportTo(-100f, -52f, 17f);
		}

		[Description("Lists the parent objects in the current scene")]
		public string GetSceneParents()
		{
			List<Transform> list = new List<Transform>();
			Transform[] array = (Transform[])UnityEngine.Object.FindObjectsOfType(typeof(Transform));
			Transform[] array2 = array;
			foreach (Transform transform in array2)
			{
				Transform root = transform.root;
				if (!list.Contains(root))
				{
					list.Add(root);
				}
			}
			string text = string.Empty;
			foreach (Transform item in list)
			{
				text = text + item.name + "\n";
			}
			return text;
		}

		[Description("Dumps object or component properties to the console")]
		public string Dump(string Obj)
		{
			return DebugUtil.Dump(Obj, true);
		}

		public string Dump()
		{
			return Dump(null);
		}

		[Description("Dumps the contents of the AppShell shared hash table to the console")]
		public string DumpHashTable()
		{
			StringBuilder stringBuilder = new StringBuilder("Shared Hash Table Contents:\n");
			foreach (DictionaryEntry item in AppShell.Instance.SharedHashTable)
			{
				stringBuilder.Append("[\"");
				stringBuilder.Append(item.Key.ToString());
				stringBuilder.Append("\"] = ");
				if (item.Value is string || !(item.Value is IEnumerable))
				{
					stringBuilder.Append(item.Value);
				}
				else
				{
					stringBuilder.Append("( ");
					foreach (object item2 in (IEnumerable)item.Value)
					{
						stringBuilder.Append("     ");
						stringBuilder.Append(item2);
						stringBuilder.Append("\n");
					}
					stringBuilder.Append(")");
				}
				stringBuilder.Append("\n");
			}
			return stringBuilder.ToString();
		}

		[Description("Causes player to eat")]
		public void Eat(float heightPercentage)
		{
			AppShell.Instance.EventMgr.Fire(null, new EatMessage(heightPercentage));
		}

		[Description("Sets player level")]
		public void SetLevel(int level)
		{
			AppShell.Instance.EventMgr.Fire(null, new SetLevelMessage(level));
		}

		[Description("Fills Power Bar")]
		public void FillPowerBar()
		{
			AppShell.Instance.EventMgr.Fire(null, new FillPowerBarMessage());
		}

		[Description("Prints a list of currently active debug keys")]
		public string PrintDebugKeys()
		{
			return SHSDebugInput.Inst.PrintDebugKeys();
		}

		[Description("Send a message to all users")]
		public void SendGameMessage(string message)
		{
			AppShell.Instance.EventMgr.Fire(null, new SendGameMessageMessage(message));
		}

		[Description("Cancel character selection timer")]
		public void CancelCharacterSelectTimer()
		{
			AppShell.Instance.EventMgr.Fire(null, new CancelTimerMessage());
		}

		[Description("Skips a cinematic")]
		public void SkipCinematic()
		{
			AppShell.Instance.EventMgr.Fire(null, new SkipCinematicMessage());
		}

		[Description("Completes the current mission")]
		public void CompleteMission()
		{
			AppShell.Instance.EventMgr.Fire(null, new CompleteMissionMessage());
		}

		[Description("Causes player to sit")]
		public void Sit()
		{
			ChangeBehavior("BehaviorSit");
		}

		[Description("Causes player to stand")]
		public void Stand()
		{
			AppShell.Instance.EventMgr.Fire(null, new CallMethodOnBehavior("stand"));
		}

		[Description("Toggles manipulate behavior on player")]
		public void Manipulate()
		{
			ChangeBehavior("BehaviorManipulate");
		}

		[Description("Changes the players behavior")]
		public void ChangeBehavior(string behaviorName)
		{
			AppShell.Instance.EventMgr.Fire(null, new ChangeBehaviorMessage(behaviorName));
		}

		[Description("Advance to next room in HQ")]
		public void NextRoom()
		{
			AppShell.Instance.EventMgr.Fire(null, new HQRoomChangeRequestMessage(HQRoomChangeRequestMessage.RoomCycleDirection.Next));
		}

		[Description("Go to previous room in HQ")]
		public void PreviousRoom()
		{
			AppShell.Instance.EventMgr.Fire(null, new HQRoomChangeRequestMessage(HQRoomChangeRequestMessage.RoomCycleDirection.Previous));
		}

		[Description("Toggle Physics in HQ")]
		public void TogglePhysics()
		{
			AppShell.Instance.EventMgr.Fire(null, new TogglePhysicsMessage());
		}

		[Description("Advance to next theme in HQ")]
		public void NextTheme()
		{
			AppShell.Instance.EventMgr.Fire(null, new NextThemeMessage());
		}

		[Description("Toggle attack chain numbers display")]
		public void ToggleAttackChainNumbers()
		{
			AppShell.Instance.EventMgr.Fire(null, new ShowAttackChainNumbersMessage());
		}

		[Description("Toggle attack colliders display")]
		public void ToggleAttackColliders()
		{
			AppShell.Instance.EventMgr.Fire(null, new ShowAttackCollidersMessage());
		}

		[Description("Forces the next mission to have the given medal.  0 = Bronze, 3 = Adamantium.")]
		public void ForceBrawlerMedal(int medalValue)
		{
			AppShell.Instance.EventMgr.Fire(null, new BrawlerForceMedalMessage(medalValue));
		}

		[Description("Spits out an estimated mission scoring value range for the current stage")]
		public void BrawlerEstimateScore()
		{
			ScoreCalculator.CalculateLevelScore();
		}

		[Description("Spits out the current scoring data for each player in the brawler")]
		public void BrawlerOutputScores()
		{
			if (BrawlerStatManager.Active)
			{
				BrawlerStatManager.instance.OutputCurrentScores();
			}
		}

		[Description("Adds specified amount to the brawler score.")]
		public void BrawlerAddScore(int score)
		{
			AppShell.Instance.EventMgr.Fire(this, new ScenarioEventScoreMessage(score));
		}

		[Description("Defaults all active brawler enemies.")]
		public void DefeatActiveEnemies()
		{
			CombatController.AttackData attackData = AttackDataManager.Instance.getAttackData("DebugCheatKillAttack");
			if (attackData == null)
			{
				return;
			}
			GameObject localPlayer = GameController.GetController().LocalPlayer;
			if (localPlayer == null)
			{
				return;
			}
			CombatController combatController = localPlayer.GetComponent(typeof(CombatController)) as CombatController;
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(CharacterGlobals));
			UnityEngine.Object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				CharacterGlobals characterGlobals = (CharacterGlobals)array2[i];
				if (characterGlobals != null)
				{
					SpawnData spawnData = characterGlobals.spawnData;
					if (spawnData != null && ((spawnData.spawnType & CharacterSpawn.Type.AI) != 0 || (spawnData.spawnType & CharacterSpawn.Type.Boss) != 0))
					{
						combatController.attackHit(characterGlobals.transform.position, characterGlobals.combatController, attackData, attackData.impacts[0]);
					}
				}
			}
		}

		[Description("Set the timescale to 0")]
		public void SetTimescale(float timeScale)
		{
			Time.timeScale = timeScale;
		}

		[Description("Toggle god mode.")]
		public void GodMode()
		{
			PlayerCombatController playerCombatController = null;
			foreach (PlayerCombatController player in PlayerCombatController.PlayerList)
			{
				if (player.CharGlobals != null && player.CharGlobals.spawnData != null && (player.CharGlobals.spawnData.spawnType & CharacterSpawn.Type.LocalPlayer) != 0)
				{
					playerCombatController = player;
					break;
				}
			}
			if (!(playerCombatController != null))
			{
				return;
			}
			string[] array = new string[4]
			{
				"CheatSuperStamina",
				"CheatSuperStrength",
				"CheatSuperCharged",
				"CheatSuperSpeed"
			};
			foreach (string text in array)
			{
				if (playerCombatController.currentActiveEffects.ContainsKey(text))
				{
					playerCombatController.removeCombatEffect(text);
				}
				else
				{
					playerCombatController.createCombatEffect(text, playerCombatController, false);
				}
			}
		}

		[Description("Health regen.")]
		public void Regen()
		{
			PlayerCombatController playerCombatController = null;
			foreach (PlayerCombatController player in PlayerCombatController.PlayerList)
			{
				if (player.CharGlobals != null && player.CharGlobals.spawnData != null && (player.CharGlobals.spawnData.spawnType & CharacterSpawn.Type.LocalPlayer) != 0)
				{
					playerCombatController = player;
					break;
				}
			}
			if (!(playerCombatController != null))
			{
				return;
			}
			string[] array = new string[1]
			{
				"CheatHealthRegen"
			};
			foreach (string text in array)
			{
				if (playerCombatController.currentActiveEffects.ContainsKey(text))
				{
					playerCombatController.removeCombatEffect(text);
				}
				else
				{
					playerCombatController.createCombatEffect(text, playerCombatController, false);
				}
			}
		}

		[Description("Jump to Mini-Mission Airlock")]
		public void GotoMiniMission(string missionId)
		{
			AppShell.Instance.Matchmaker2.MiniBrawler(missionId, OnAcceptBrawler);
		}

		[Description("Jump to Solo Mission")]
		public void GotoSoloMission(string missionId)
		{
			AppShell.Instance.Matchmaker2.SoloBrawler(missionId, OnAcceptSoloBrawler);
		}

		[Description("Enabled work in progress missions on the brawler gadget")]
		public void EnableWIPMissions(bool enabled)
		{
			if (enabled)
			{
				PlayerPrefs.SetInt("WipMissions", 1);
			}
			else
			{
				PlayerPrefs.SetInt("WipMissions", 0);
			}
		}

		[Description("Enables logging of damage values")]
		public void EnableDamageDisplay(bool enabled)
		{
			CombatController.displayDamageInfo = enabled;
		}

		[Description("Enables damage/score/KO popup display")]
		public void EnablePopups(bool enabled)
		{
			CombatController.displayPopups = enabled;
		}

		[Description("Enables various levels of keyboard control.  0 - None, 1 - Movement only, 2 - All")]
		public void EnableKeyboardControls(int level)
		{
			if (level >= 0 && level <= 2)
			{
				PlayerPrefs.SetInt("KeyboardLevel", level);
				PlayerInputController.keyboardControlLevel = level;
			}
		}

		[Description("Brings up code redemption dialog")]
		public void CodeRedemption()
		{
			AppShell.Instance.EventMgr.Fire(null, new ShowCodeRedemptionDialogMessage());
		}

		[Description("Set challenge counter")]
		public void SetChallengeCounter(int value)
		{
			ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter("ChallengeCounter");
			counter.SetCounter(value);
		}

		[Description("Set a Mayhem Mode counter")]
		public void SetMayhemModeCounter(string counterKey, int value)
		{
			ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter("MayhemMode." + counterKey);
			counter.SetCounter(value);
		}

		[Description("Spawns a new brick in HQ")]
		public void SpawnBrick()
		{
			AppShell.Instance.EventMgr.Fire(null, new SpawnBrickMessage());
		}

		[Description("Follows player that the cursor is over")]
		public void Follow()
		{
			AppShell.Instance.EventMgr.Fire(null, new FollowMessage());
		}

		[Description("Toggles debug keys on and off")]
		public void SetDebugKeys(bool status)
		{
			if (status)
			{
				SHSDebugInput.Inst.DeactivateCurrentKeyBank();
				SHSDebugInput.Inst.ActivateDebugKeys();
				SHSDebugInput.Inst.ActivateCurrentKeyBank();
			}
			else
			{
				SHSDebugInput.Inst.DeactivateCurrentKeyBank();
				SHSDebugInput.Inst.DeactivateDebugKeys();
				SHSDebugInput.Inst.ActivateCurrentKeyBank();
			}
		}

		[Description("Invites <playerId> to a private Squad Battle")]
		public void Battle(int playerId)
		{
			AppShell.Instance.EventMgr.Fire(null, new BattleInviteMessage(playerId));
		}

		[Description("Invites <opponentName> to a private Squad Battle")]
		public void Battle(string opponentName)
		{
			AppShell.Instance.EventMgr.Fire(null, new BattleInviteMessage(opponentName));
		}

		[Description("Launches emote <emotecommand>")]
		public string Emote(string emotecommand)
		{
			EmotesDefinition.EmoteDefinition emoteByCommand = EmotesDefinition.Instance.GetEmoteByCommand(emotecommand);
			if (emoteByCommand != null)
			{
				UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(typeof(PlayerInputController));
				if (@object != null)
				{
					CspUtils.DebugLog("PlayerInputController is type:" + @object.GetType().ToString());
					UnityEngine.Component component = @object as UnityEngine.Component;
					if (component != null && (bool)component.gameObject)
					{
						AppShell.Instance.EventMgr.Fire(component.gameObject, new EmoteMessage(component.gameObject, emoteByCommand.id));
						return "Firing emote:" + emotecommand;
					}
				}
				return "Could not find PlayerInputController";
			}
			return "Could not find emote with command name:" + emotecommand;
		}

		[Description("Selects the specified character for the Social Space or Brawler")]
		public void SelectCharacter(string costumeName)
		{
			GameController controller = GameController.GetController();
			SocialSpaceController x = controller as SocialSpaceController;
			BrawlerController x2 = controller as BrawlerController;
			if (x != null)
			{
				CharacterSelectedMessage msg = new CharacterSelectedMessage(costumeName);
				AppShell.Instance.EventMgr.Fire(null, msg);
			}
			else if (x2 != null)
			{
				CharacterRequestedMessage msg2 = new CharacterRequestedMessage(costumeName);
				AppShell.Instance.EventMgr.Fire(null, msg2);
			}
			else
			{
				CspUtils.DebugLog("Unable to find a SocialSpaceController or BrawlerController.  Use 'SelectCharacter' on the character selection screen of either a SocialSpace or Brawler instance.");
			}
		}

		[Description("Display the current zone instance.")]
		public string ShowZoneInstance()
		{
			return AppShell.Instance.ServerConnection.GetRoomName();
		}

		[Description("Refresh the inventory/currency from the server.")]
		public string RefreshInventory()
		{
			AppShell.Instance.Profile.StartInventoryFetch();
			AppShell.Instance.Profile.StartCurrencyFetch();
			return "Requested an inventory/currency update from the server.";
		}

		[Description("Display the player's User ID in the console.")]
		public string ShowUserId()
		{
			AppShell.Instance.Profile.StartCurrencyFetch();
			return "Your User ID is: " + AppShell.Instance.Profile.UserId;
		}

		[Description("Show another player's MySquad Information")]
		public void ShowSquad(long playerId)
		{
			SHSMySquadGadget dialogWindow = new SHSMySquadGadget(playerId);
			GUIManager.Instance.ShowDynamicWindow(dialogWindow, GUIControl.ModalLevelEnum.Default);
		}

		[Description("Retrieve Another player's profile information")]
		public void FetchRemoteProfile(long playerId)
		{
			RemotePlayerProfile.FetchProfile(playerId, delegate(UserProfile profile)
			{
				SHSMySquadGadget dialogWindow = new SHSMySquadGadget(profile);
				GUIManager.Instance.ShowDynamicWindow(dialogWindow, GUIControl.ModalLevelEnum.Default);
			});
		}

		[Description("Retrieve Another player's profile information")]
		public void ClearRemoteCounterBank(long playerId)
		{
			AppShell.Instance.CounterManager.RemoveCounterBank(playerId);
		}

		[Description("Add coins to user")]
		public string AddTokens(int coins)
		{
			AppShell.Instance.EventReporter.ReportAwardTokens(coins);
			return "add_tokens event sent to smartfox";
		}

		[Description("Add tickets to user")]
		public string AddTickets(int tickets)
		{
			AppShell.Instance.EventReporter.ReportAwardTickets(tickets);
			return "add_tickets event sent to smartfox";
		}

		[Description("Chad's test function.  If you're not Chad, don't call it.")]
		public void ChadScore()
		{
			Hashtable hashtable = new Hashtable();
			hashtable["mission_id"] = "m_0001_1_SampleMission";
			hashtable["hero"] = "hulk";
			AppShell.Instance.ServerConnection.ReportBrawlerEvent("mission_end", hashtable);
		}

		[Description("Chad's test function.  If you're not Chad, don't call it.")]
		public void ChadCardSolo()
		{
			AppShell.Instance.Matchmaker2.SoloCardGame(1, "cyclops", "ST073:40;", 1);
			AppShell.Instance.Transition(GameController.ControllerType.CardGame);
		}

		[Description("Chad's test function.  If you're not Chad, don't call it.")]
		public void ChadCardInvite(long userId)
		{
			AppShell.Instance.Matchmaker2.InviteCardGame(new long[1]
			{
				userId
			}, Util.GetRandomArena(), "cyclops", "ST073:40;", null);
			AppShell.Instance.Transition(GameController.ControllerType.CardGame);
		}

		[Description("Chad's test function.  If you're not Chad, don't call it.")]
		public string ChadAccept()
		{
			Matchmaker2.Invitation topInvitation = AppShell.Instance.Matchmaker2.GetTopInvitation();
			if (topInvitation != null)
			{
				switch (topInvitation.gameType)
				{
				case Matchmaker2.GameType.CARD:
					topInvitation.Accept(OnAcceptCard);
					return "trying to accept card game";
				case Matchmaker2.GameType.BRAWLER:
					topInvitation.Accept(OnAcceptBrawler);
					return "trying to accept brawler";
				default:
					return "unknown invitation type";
				}
			}
			return "no invitation";
		}

		protected void OnAcceptCard(Matchmaker2.Ticket ticket)
		{
			if (ticket.status == Matchmaker2.Ticket.Status.SUCCESS)
			{
				AppShell.Instance.SharedHashTable["CardGameTicket"] = ticket;
				AppShell.Instance.Transition(GameController.ControllerType.CardGame);
			}
		}

		protected void OnAcceptSoloBrawler(Matchmaker2.Ticket ticket)
		{
			AppShell.Instance.SharedHashTable["BrawlerAirlockPlaySolo"] = true;
			OnBrawlerTicket(ticket);
		}

		protected void OnAcceptBrawler(Matchmaker2.Ticket ticket)
		{
			CspUtils.DebugLog(ticket + " Console Command OnAcceptBrawler");
			AppShell.Instance.SharedHashTable["BrawlerAirlockPlaySolo"] = false;
			OnBrawlerTicket(ticket);
		}

		protected void OnBrawlerTicket(Matchmaker2.Ticket ticket)
		{
			if (ticket.status != 0)
			{
				CspUtils.DebugLog("Matchmaker returned " + ticket.status);
				return;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(ticket.ticket);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("//mission");
			if (xmlNode == null)
			{
				CspUtils.DebugLog("Brawler ticket does not contain the mission: " + ticket.ticket);
				return;
			}
			AppShell.Instance.SharedHashTable["BrawlerTicket"] = ticket;
			ActiveMission activeMission = new ActiveMission(xmlNode.InnerText);
			activeMission.BecomeActiveMission();
			AppShell.Instance.Transition(GameController.ControllerType.Brawler);
		}

		[Description("Chad's test function.  If you're not Chad, don't call it.")]
		public void ChadCancel()
		{
			Matchmaker2.Invitation topInvitation = AppShell.Instance.Matchmaker2.GetTopInvitation();
			if (topInvitation != null)
			{
				topInvitation.Cancel(true);
			}
		}

		[Description("Chad's test function.  If you're not Chad, don't call it.")]
		public void ChadClearOwner()
		{
			AppShell.Instance.ServerConnection.ResetAllOwnership();
		}

		[Description("Chad's test function.  If you're not Chad, don't call it.")]
		public string ChadBrawlerSolo()
		{
			string text = "m_1001_1_DocOck001";
			AppShell.Instance.Matchmaker2.SoloBrawler(text, OnAcceptBrawler);
			return "Starting brawler with mission " + text;
		}

		[Description("Chad's test function.  If you're not Chad, don't call it.")]
		public string ChadBrawlerAnyone()
		{
			AppShell.Instance.Matchmaker2.AnyoneBrawler(OnAcceptBrawler, "");  // CSP
			return "Starting brawler anyone";
		}

		[Description("Chad's test function.  If you're not Chad, don't call it.")]
		public string ChadBrawlerFriends()
		{
			string text = "m_1001_1_DocOck001";
			AppShell.Instance.Matchmaker2.FriendsBrawler(text, OnAcceptBrawler);
			return "Starting BRAWLER friends with mission " + text;
		}

		[Description("Audio Manager debug. Dumps the state of the AudioManager.")]
		public string AudioDump()
		{
			AppShell.Instance.AudioManager.DumpToLog();
			return "AudioManager data dumped to the log.";
		}

		[Description("For Josh")]
		public void JoshTest()
		{
			StartInfo startInfo = new StartInfo();
			startInfo.Players[0].DeckRecipe = "ST073:40;";
			startInfo.Players[0].Hero = "cyclops";
			startInfo.Players[1].Hero = "wasp";
			startInfo.Players[1].DeckRecipe = "ST220:40;";
			startInfo.ArenaName = "Sewers";
			startInfo.ArenaScenario = 1;
			startInfo.QuestID = 4001;
			startInfo.QuestNodeID = 0;
			startInfo.QuestConditions = "InitialPower:2;";
			AppShell.Instance.SharedHashTable["CardGameLevel"] = startInfo;
			AppShell.Instance.SharedHashTable["CardGameTicket"] = null;
			AppShell.Instance.QueueLocationInfo();
			AppShell.Instance.Matchmaker2.SoloCardGame(1, startInfo.Players[0].Hero, startInfo.Players[0].DeckRecipe, startInfo.QuestNodeID);
			AppShell.Instance.Transition(GameController.ControllerType.CardGame);
		}

		[Description("Override the card deck for player N")]
		public void ForceDeck(int player, string decklist)
		{
			AppShell.Instance.SharedHashTable["OverrideDeck" + player] = decklist;
		}

		[Description("Override the card game arena")]
		public void ForceArena(string arena)
		{
			AppShell.Instance.SharedHashTable["OverrideCGArena"] = arena;
		}

		[Description("Override the avatar for player N")]
		public void ForceAvatar(int player, string hero)
		{
			AppShell.Instance.SharedHashTable["OverrideAvatar" + player] = hero;
		}

		[Description("Cause a Poke to be sent, same as clicking the 'Poke' button")]
		public void ForcePoke()
		{
			if (CardGameController.Instance != null && CardGameController.Instance.players != null)
			{
				CardGameController.Instance.players[0].SendPoke();
			}
		}

		[Description("Launch the card game deck builder")]
		public void DeckBuilder()
		{
			AppShell.Instance.Transition(GameController.ControllerType.DeckBuilder);
		}

		[Description("Launch the arcade shell prototype")]
		public void Arcade(string game)
		{
			AppShell.Instance.SharedHashTable["AracdeGame"] = game;
			AppShell.Instance.Transition(GameController.ControllerType.ArcadeShell);
		}

		[Description("Auto-build a deck based on the specified card list")]
		public string AutoDeck(string recipe)
		{
			DeckBuilderController.Instance.BuildAutoDeck(recipe);
			return "Running autodeckbuilder for " + recipe;
		}

		public void AutoDeck()
		{
			AutoDeck(string.Empty);
		}

		[Description("Reload card data from web services (quests/cards/decks owned)")]
		public string ReloadQuests()
		{
			AppShell.Instance.CardQuestManager.Clear();
			AppShell.Instance.WebService.StartRequest("resources$data/json/card-quests/", AppShell.Instance.OnCardQuestDataWebResponse, null, ShsWebService.ShsWebServiceType.RASP);
			AppShell.Instance.Profile.StartQuestsFetch();
			return "Reloaded card quest data";
		}

		[Description("Open a Booster Pack, getting the cards inside.")]
		public void OpenBoosterPack(int boosterPackId)
		{
			BoosterPackService.OpenBoosterPack(boosterPackId, null);
		}

		[Description("Unload Unused Assets")]
		public void UnloadUnusedAssets()
		{
			Resources.UnloadUnusedAssets();
		}

		[Description("Change the game execution speed")]
		public void TimeScale(float newTime)
		{
			Time.timeScale = newTime;
		}

		[Description("Toggles Logging through CspUtils.DebugLog")]
		public void ToggleDebugLog()
		{
			SHSDebug.PassThrough = !SHSDebug.PassThrough;
		}

		[Description("Resets all social space collection activities")]
		public void ResetCollectionActivities()
		{
			AppShell.Instance.ActivityManager.ResetCollectionActivities();
		}

		[Description("Resets the collection timer of feathers for all characters.")]
		public void ResetFeatherTimer()
		{
			SHSFeathersActivity sHSFeathersActivity = AppShell.Instance.ActivityManager.GetActivity("feathersactivity") as SHSFeathersActivity;
			sHSFeathersActivity.ResetTimer();
			sHSFeathersActivity.RespawnFeathers();
		}

		[Description("Resets the feather collection state for all characters.")]
		public void ResetFeatherStates()
		{
			SHSFeathersActivity sHSFeathersActivity = AppShell.Instance.ActivityManager.GetActivity("feathersactivity") as SHSFeathersActivity;
			sHSFeathersActivity.ResetFeatherStates();
			sHSFeathersActivity.RespawnFeathers();
		}

		[Description("Displays the current feather collection state for the current character.")]
		public string GetFeatherState()
		{
			SHSFeathersActivity sHSFeathersActivity = AppShell.Instance.ActivityManager.GetActivity("feathersactivity") as SHSFeathersActivity;
			BitArray featherState = sHSFeathersActivity.GetFeatherState();
			if (featherState == null)
			{
				return "No character selected.  Enter a zone and select a character before invoking this command.";
			}
			string text = "Collection state: ";
			foreach (bool item in featherState)
			{
				text += ((!item) ? "<active> " : "<collected> ");
			}
			return text;
		}

		[Description("Spawns a feather at each feather spawner.  Call DespawnExtraFeathers to revert back to normal behavior.")]
		public void SpawnAllFeathers()
		{
			SHSFeathersActivity sHSFeathersActivity = AppShell.Instance.ActivityManager.GetActivity("feathersactivity") as SHSFeathersActivity;
			sHSFeathersActivity.Debug_SpawnAllFeathers();
		}

		[Description("Reverts the Feathers activity back to its normal operation.  This is the inverse operation of SpawnAllFeathers.")]
		public void DespawnExtraFeathers()
		{
			SHSFeathersActivity sHSFeathersActivity = AppShell.Instance.ActivityManager.GetActivity("feathersactivity") as SHSFeathersActivity;
			sHSFeathersActivity.Debug_UndoSpawnAllFeathers();
		}

		[Description("Displays the number of collected feathers, after having called SpawnAllFeathers")]
		public string NumFeathers()
		{
			SHSFeathersActivity sHSFeathersActivity = AppShell.Instance.ActivityManager.GetActivity("feathersactivity") as SHSFeathersActivity;
			int num = sHSFeathersActivity.Debug_GetCollectionCount();
			return "Collected " + num + ((num != 1) ? " feathers" : " feather");
		}

		[Description("Resets the collection timer of fractals for all characters.")]
		public void ResetFractalTimer()
		{
			SHSFractalsActivity sHSFractalsActivity = AppShell.Instance.ActivityManager.GetActivity("fractalsactivity") as SHSFractalsActivity;
			sHSFractalsActivity.ResetTimer();
			sHSFractalsActivity.RespawnFractals();
		}

		[Description("Resets the fractal collection state for all characters.")]
		public void ResetFractalStates()
		{
			SHSFractalsActivity sHSFractalsActivity = AppShell.Instance.ActivityManager.GetActivity("fractalsactivity") as SHSFractalsActivity;
			sHSFractalsActivity.ResetFractalStates();
			sHSFractalsActivity.RespawnFractals();
		}

		[Description("Displays the current fractal collection state for the current character.")]
		public string GetFractalState()
		{
			SHSFractalsActivity sHSFractalsActivity = AppShell.Instance.ActivityManager.GetActivity("fractalsactivity") as SHSFractalsActivity;
			BitArray fractalState = sHSFractalsActivity.GetFractalState();
			if (fractalState == null)
			{
				return "No character selected.  Enter a zone and select a character before invoking this command.";
			}
			string text = "Collection state: ";
			foreach (bool item in fractalState)
			{
				text += ((!item) ? "<active> " : "<collected> ");
			}
			return text;
		}

		[Description("Spawns a fractal at each fractal spawner.  Call DespawnExtraFractals to revert back to normal behavior.")]
		public void SpawnAllFractals()
		{
			SHSFractalsActivity sHSFractalsActivity = AppShell.Instance.ActivityManager.GetActivity("fractalsactivity") as SHSFractalsActivity;
			sHSFractalsActivity.Debug_SpawnAllFractals();
		}

		[Description("Reverts the Fractals activity back to its normal operation.  This is the inverse operation of SpawnAllFractals.")]
		public void DespawnExtraFractals()
		{
			SHSFractalsActivity sHSFractalsActivity = AppShell.Instance.ActivityManager.GetActivity("fractalsactivity") as SHSFractalsActivity;
			sHSFractalsActivity.Debug_UndoSpawnAllFractals();
		}

		[Description("Displays the number of collected fractals, after having called SpawnAllFractals")]
		public string NumFractals()
		{
			SHSFractalsActivity sHSFractalsActivity = AppShell.Instance.ActivityManager.GetActivity("fractalsactivity") as SHSFractalsActivity;
			int num = sHSFractalsActivity.Debug_GetCollectionCount();
			return "Collected " + num + ((num != 1) ? " fractals" : " fractal");
		}

		[Description("Spawns a robber at each robber spawn point in the current zone.")]
		public void SpawnAllRobbers()
		{
			SHSRobberRallyActivity sHSRobberRallyActivity = AppShell.Instance.ActivityManager.GetActivity("robberrallyactivity") as SHSRobberRallyActivity;
			sHSRobberRallyActivity.SpawnAllRobbers();
		}

		[Description("Spawns the robber with the name [ex. Rally_1]")]
		public void SpawnRobber(string robberName)
		{
			SHSRobberRallyActivity sHSRobberRallyActivity = AppShell.Instance.ActivityManager.GetActivity("robberrallyactivity") as SHSRobberRallyActivity;
			sHSRobberRallyActivity.SpawnRobber(robberName);
		}

		[Description("Spawns a troublebot at each troublebot spawn location in the current zone.")]
		public void SpawnAllTroubleBots()
		{
			SHSTroubleBotActivity sHSTroubleBotActivity = AppShell.Instance.ActivityManager.GetActivity("troublebots") as SHSTroubleBotActivity;
			sHSTroubleBotActivity.deployAll();
		}

		[Description("Displays progress on background downloading of asset bundles.")]
		public void DownloadStatus()
		{
			if (!SHSStagedDownloadWindow.DownloadStatusCurrentlyShowing)
			{
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				Type type = executingAssembly.GetType("SHSStagedDownloadWindow");
				GUIWindow gUIWindow = (GUIWindow)Activator.CreateInstance(type);
				GUIManager.Instance.UIRoots[GUIManager.UILayer.System].Add(gUIWindow);
				gUIWindow.Show(GUIControl.ModalLevelEnum.Default);
			}
		}

		[Description("Toggles the character's ability to double jump.")]
		public string DoubleJump()
		{
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(GameController.GetController().LocalPlayer);
			component.doubleJump = !component.doubleJump;
			return "Double jump " + ((!component.doubleJump) ? "DISABLED" : "ENABLED");
		}

		[Description("Sets the first (and second, if double jump is enabled) jump height to the specified value")]
		public void JumpHeight(float jumpHeight)
		{
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(GameController.GetController().LocalPlayer);
			component.jumpHeight = jumpHeight;
			component.secondJumpHeight = jumpHeight;
		}

		[Description("Toggles hold-to-jump functionality.")]
		public string HoldJump()
		{
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(GameController.GetController().LocalPlayer);
			component.holdJump = !component.holdJump;
			return "Hold jump " + ((!component.holdJump) ? "DISABLED" : "ENABLED");
		}

		[Description("Toggles hold-to-glide functionality.")]
		public string ToggleGlide()
		{
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(GameController.GetController().LocalPlayer);
			component.CanGlide = !component.CanGlide;
			return "Can glide " + ((!component.CanGlide) ? "DISABLED" : "ENABLED");
		}

		[Description("Grants XP to the current local hero.")]
		public void GrantXP(int xp)
		{
			GameObject localPlayer = GameController.GetController().LocalPlayer;
			if (localPlayer != null && SocialSpaceController.Instance != null && SocialSpaceController.Instance.Controller != null)
			{
				SocialSpaceController.Instance.Controller.GrantXP(Utils.GetComponent<CharacterGlobals>(localPlayer), xp);
			}
		}

		[Description("Toggles AI Behavior display")]
		public void ShowBehaviors()
		{
			AIControllerHQ.showBehaviors = !AIControllerHQ.showBehaviors;
		}

		[Description("Spawns <characterName> at door")]
		public void Spawn(string characterName)
		{
			if (HqController2.Instance != null)
			{
				Ray ray = Camera.main.ScreenPointToRay(SHSInput.mousePosition);
				Vector3 position = Vector3.zero;
				RaycastHit hitInfo;
				if (HqController2.Instance.ActiveRoom != null)
				{
					position = HqController2.Instance.ActiveRoom.RandomDoor;
				}
				else if (Physics.Raycast(ray, out hitInfo, 1000f, 98304))
				{
					position = hitInfo.point;
				}
				GameObject gameObject = HqAIProxy.CreateProxy(characterName);
				gameObject.transform.position = position;
				gameObject.transform.parent = HqController2.Instance.ActiveRoom.transform;
				HqAIProxy component = Utils.GetComponent<HqAIProxy>(gameObject);
				if (component == null)
				{
					CspUtils.DebugLog("Could not find HqAIProxy component!");
					return;
				}
				component.Spawn(characterName, position, HqController2.Instance.ActiveRoom);
				HqController2.Instance.AddProxy(component);
			}
		}

		[Description("Despawns <characterName>")]
		public void Despawn(string characterName)
		{
			if (HqController2.Instance != null)
			{
				HqController2.Instance.Despawn(characterName);
			}
		}

		[Description("Despawns all AI in HQ")]
		public void DespawnAllHqAI()
		{
			if (HqController2.Instance != null)
			{
				HqController2.Instance.DespawnAllAI();
			}
		}

		[Description("Despawns AI in current room")]
		public void DespawnAIInRoom()
		{
			if (HqController2.Instance != null)
			{
				HqController2.Instance.DespawnAIInCurrentRoom();
			}
		}

		[Description("Goes to the room <characterName> is in")]
		public void GoTo(string characterName)
		{
			if (HqController2.Instance != null)
			{
				HqController2.Instance.GoTo(characterName);
			}
		}

		[Description("Goes to the fallback scene")]
		public void GoToFallback()
		{
			AppShell.Instance.Transition(GameController.ControllerType.Fallback);
		}

		[Description("Sets HQ AI logging")]
		public void HQAILogging(string CharacterName, string LogChannel)
		{
			AppShell.Instance.EventMgr.Fire(null, new HqAILogMessage(CharacterName, LogChannel));
		}

		[Description("Set hunger value on an AI [ex. sethunger <all | character name> <value>]")]
		public void SetHunger(string CharacterName, float Value)
		{
			AppShell.Instance.EventMgr.Fire(null, new HqAISetHungerMessage(CharacterName, Value));
		}

		[Description("Clears placed items in HQ room")]
		public string ClearHQRoom()
		{
			if (HqController2.Instance != null && HqController2.Instance.ActiveRoom != null)
			{
				HqController2.Instance.ActiveRoom.Reset();
			}
			return "Cleared Items";
		}

		[Description("Saves HQ Layout as file specified")]
		public void SaveHQLayout(string filePath)
		{
			if (HqController2.Instance != null)
			{
				TextWriter textWriter = new StreamWriter(filePath, false);
				foreach (HqRoom2 value in HqController2.Instance.Rooms.Values)
				{
					textWriter.WriteLine("Begin Room " + value.Id);
					textWriter.Write(value.BuildSaveData());
					textWriter.WriteLine("End Room " + value.Id);
				}
				textWriter.Close();
			}
		}

		[Description("Loads HQ Layout from specified file")]
		public string LoadHQLayout(string filePath)
		{
			CspUtils.DebugLog("Loading HQ Layout");
			if (File.Exists(filePath))
			{
				TextReader textReader = new StreamReader(filePath);
				if (textReader != null)
				{
					Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
					for (string text = textReader.ReadLine(); text != null; text = textReader.ReadLine())
					{
						if (text.StartsWith("Begin Room "))
						{
							string key = text.Substring("Begin Room".Length + 1);
							List<string> list = new List<string>();
							string text2 = textReader.ReadLine();
							while (text2 != null && !text2.StartsWith("End Room "))
							{
								list.Add(text2);
								text2 = textReader.ReadLine();
							}
							dictionary[key] = list.ToArray();
						}
					}
					foreach (HqRoom2 value in HqController2.Instance.Rooms.Values)
					{
						if (dictionary.ContainsKey(value.Id))
						{
							value.LoadFromFile(dictionary[value.Id]);
						}
					}
					textReader.Close();
				}
			}
			return "HQ Layout loaded.";
		}

		[Description("Purchases all hq rooms")]
		public void PurchaseAllRooms()
		{
			if (HqController2.Instance != null)
			{
				foreach (HqRoom2 value in HqController2.Instance.Rooms.Values)
				{
					if (value.GetType() == typeof(HqUnpurchasedRoom))
					{
						string roomIdFromKey = AppShell.Instance.HQRoomManifest.GetRoomIdFromKey(value.Id);
						if (!string.IsNullOrEmpty(roomIdFromKey))
						{
							AppShell.Instance.EventMgr.Fire(this, new ShoppingItemPurchasedMessage(OwnableDefinition.Category.HQRoom, roomIdFromKey, value.Id));
						}
					}
				}
			}
		}

		[Description("Grant XP to a hero for their action in an HQ room.")]
		public void GrantHQXP(string heroName, int amount)
		{
			HQService.GrantXP(heroName, amount);
		}

		[Description("Dump list of EventMgr listeners to log")]
		public void DumpEvents()
		{
			AppShell.Instance.EventMgr.DumpEventListeners();
		}

		[Description("Force a server ping")]
		public void ServerPing()
		{
			AppShell.Instance.ServerConnection.PingServer();
		}

		[Description("Display the average server latency")]
		public string ShowServerLatency()
		{
			return "Latency = " + ServerTime.Instance.GetAverageLatency();
		}

		[Description("Print client's interpretation of the current server time")]
		public string ShowServerTime()
		{
			double time = ServerTime.time;
			DateTime serverTimeInDateTime = ServerTime.Instance.GetServerTimeInDateTime();
			return "Seconds = " + time + ", DateTime UTC = " + serverTimeInDateTime;
		}

		[Description("Sets the minimum amount of time to wait before swapping placeholder characters with real characters.")]
		public void SetPlaceholderDelay(float delay)
		{
			PlaceholderSpawnInterrupt.debug_swapDelay = delay;
			StagedSpawner.debug_swapDelay = delay;
		}

		[Description("Gets all the user variable names associated with an account")]
		public string GetUserVariables(int userId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Hashtable userVariablesDict = AppShell.Instance.ServerConnection.GetUserVariablesDict(userId);
			if (userVariablesDict == null || userVariablesDict.Count == 0)
			{
				return "No User Variables for " + userId;
			}
			foreach (string key in userVariablesDict.Keys)
			{
				stringBuilder.Append(key + " = ");
				object obj = userVariablesDict[key];
				stringBuilder.Append(obj ?? "Null").Append(", ");
			}
			return stringBuilder.ToString();
		}

		[Description("Dump contents of the player dictionary")]
		public string GetPlayerDictionary()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<int, PlayerDictionary.Player> item in AppShell.Instance.PlayerDictionary)
			{
				stringBuilder.Append(item.Key + ": " + item.Value + "\n");
			}
			return stringBuilder.ToString();
		}

		[Description("Sets a user variable associated with this user")]
		public string SetUserVariable(string name, string value)
		{
			AppShell.Instance.ServerConnection.SetUserVariable(name, value);
			return "ok";
		}

		[Description("Teleports the local player to an arbitrary location")]
		public void TeleportTo(float x, float y, float z)
		{
			GameObject localPlayer = GameController.GetController().LocalPlayer;
			if (localPlayer != null)
			{
				CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(localPlayer);
				if (component != null)
				{
					Vector3 vector = new Vector3(x, y, z);
					component.teleportTo(vector);
					component.setDestination(vector);
				}
			}
		}

		[Description("Sets the status of the local character.")]
		public void SetStatus(string status)
		{
			PlayerStatusDefinition.Status status2 = PlayerStatusDefinition.Instance.GetStatus(status);
			if (status2 != null)
			{
				PlayerStatus.SetLocalStatus(status2);
			}
		}

		[Description("Clears the status of the local character.")]
		public void ClearStatus()
		{
			PlayerStatus.ClearLocalStatus();
		}

		[Description("Dumps the transaction that the WaitWatcher cares about, if any.")]
		public void DumpWaitWatcher()
		{
			AppShell.Instance.EventMgr.Fire(this, new WaitWatcherEvent.DumpWaitWatcher());
		}

		[Description("Sets the amount of idle time required before being interpreted as AFK, in seconds.")]
		public string SetAFKTime(float newAFKTime)
		{
			GameObject localPlayer = GameController.GetController().LocalPlayer;
			if (localPlayer != null)
			{
				AFKWatcher component = Utils.GetComponent<AFKWatcher>(localPlayer);
				if (component != null)
				{
					component.afkTime = newAFKTime;
					return "Set new AFK time to " + component.afkTime;
				}
			}
			return "Error setting afk time";
		}

		[Description("Displays the current number of collected stars.")]
		public string StarCount()
		{
			return AppShell.Instance.Profile.Stars.ToString();
		}

		[Description("Changes the apparent number of players for spawn calculations for the remainder of the current mission.  Only works on the host client.")]
		public string SetDebugPlayerCount(int playerCount)
		{
			if (BrawlerController.Instance == null)
			{
				return "Not currently in a mission, command invalid.";
			}
			if (playerCount < 0 || playerCount > 4)
			{
				return "PlayerCount must be between 0 and 4, command invalid.";
			}
			BrawlerController.Instance.debugPlayerCount = playerCount;
			if (playerCount == 0)
			{
				return "DebugPlayerCount is disabled.";
			}
			return "DebugPlayerCount is now " + playerCount.ToString();
		}

		[Description("Changes the default impact matrix type for the local player")]
		public string SetDefaultIMType(int newType)
		{
			GameObject localPlayer = GameController.GetController().LocalPlayer;
			if (localPlayer != null)
			{
				CombatController component = localPlayer.GetComponent<CombatController>();
				if (component != null)
				{
					component.DefaultImpactMatrixType = (ImpactMatrix.Type)newType;
					return "Set default impact matrix type to " + ((ImpactMatrix.Type)newType).ToString() + " for " + localPlayer.name;
				}
				return "No combat controller attached to the local player";
			}
			return "No local player found";
		}

		[Description("Request to purchase an item")]
		public void PurchaseItem(int itemId, int catalogOwnableId, int quantity)
		{
			ShoppingService.PurchaseItem(itemId, catalogOwnableId, quantity, 0);
		}

		[Description("Request to sell an item for silver.")]
		public void SellItem(int itemId)
		{
			ShoppingService.SellItem(itemId, 1);
		}

		[Description("Request to sell an item for silver.")]
		public void SellItem(int itemId, int quantity)
		{
			ShoppingService.SellItem(itemId, quantity);
		}

		[Description("Opens the old shop UI for testing purposes.")]
		public void OpenOldShop()
		{
		}

		[Description("Open Prize Wheel")]
		public void ShowPrizeWheel()
		{
		}

		[Description("Show Player Location Coordinates - For Automation Purposes")]
		public string ShowPlayerPosition()
		{
			return AutomationManager.Instance.LocalPlayer.transform.position.ToString();
		}

		[Description("Display information about the next clicked object")]
		public void Inspect()
		{
			Utils.GetComponent<PlayerInputController>(GameController.GetController().LocalPlayer).DebugNextClick();
		}

		[Description("Dumps the various factions and their existing entities.")]
		public string DumpFactionLists()
		{
			int i = 0;
			string text = string.Empty;
			for (; i < 5; i++)
			{
				CombatController.Faction faction = CombatController.Faction.None;
				switch (i)
				{
				case 0:
					faction = CombatController.Faction.Player;
					break;
				case 1:
					faction = CombatController.Faction.Enemy;
					break;
				case 2:
					faction = CombatController.Faction.Neutral;
					break;
				case 3:
					faction = CombatController.Faction.Environment;
					break;
				}
				List<CombatController> factionList = CombatController.GetFactionList(faction);
				if (factionList != null)
				{
					string text2 = text;
					text = text2 + "Faction: " + faction + "\n";
					foreach (CombatController item in factionList)
					{
						text = text + "\t" + item.gameObject.name + "\n";
					}
				}
			}
			return text;
		}

		[Description("Changes a character's faction")]
		public string ChangeFaction(string character, string faction)
		{
			GameObject gameObject = GameObject.Find(character);
			if (gameObject == null)
			{
				return "Failed to find character <" + character + ">";
			}
			string a = faction.ToLower();
			CombatController.Faction faction2 = CombatController.Faction.None;
			if (a == "player")
			{
				faction2 = CombatController.Faction.Player;
			}
			else if (a == "enemy")
			{
				faction2 = CombatController.Faction.Enemy;
			}
			else if (a == "neutral")
			{
				faction2 = CombatController.Faction.Neutral;
			}
			else if (a == "environment")
			{
				faction2 = CombatController.Faction.Environment;
			}
			if (faction2 == CombatController.Faction.None)
			{
				return "Faction <" + faction + "> to is not one of the specified factions: player, enemy, neutral, or environment";
			}
			CombatController component = gameObject.GetComponent<CombatController>();
			if (component == null)
			{
				return "Character <" + character + "> does not have a combat controller";
			}
			component.ChangeFaction(faction2);
			return "Character <" + character + ">'s faction successfully changed to <" + faction + ">";
		}

		[Description("Post debug log to server")]
		public void PostLog()
		{
			PostLogFile.PostToServer();
		}

		[Description("Logs scenario event record to console")]
		public string LogScenarioEventRecord()
		{
			if (ScenarioEventManager.Instance != null)
			{
				ScenarioEventManager.Instance.LogEventRecord();
				return "Logging scenario event record to console";
			}
			return "Unable to log scenario event record because scenario event manager instance is null";
		}

		[Description("Dumps scenario event record to file - editor only")]
		public string DumpScenarioEventRecord(string filePath)
		{
			if (ScenarioEventManager.Instance != null)
			{
				ScenarioEventManager.Instance.DumpEventRecord(filePath);
				return "Dumped scenario event record to " + filePath;
			}
			return "Unable to dump scenario event record because scenario event manager instance is null";
		}

		[Description("If true then more detailed information is available in scenario event record")]
		public string DebugScenarioEventRecord(bool debug)
		{
			if (ScenarioEventManager.Instance != null)
			{
				ScenarioEventManager.Instance.DebugRecordEvents(debug);
				return "Now debugging scenario event record: " + debug;
			}
			return "Unable to use debug scenario event record because scenario event manager instance is null";
		}

		[Description("Clears both normal and debug scenario event records")]
		public string ClearScenarioEventRecord()
		{
			if (ScenarioEventManager.Instance != null)
			{
				ScenarioEventManager.Instance.ClearEventRecord();
				return "Cleared all scenario event records";
			}
			return "Unable to use clear scenario event records because scenario event manager instance is null";
		}

		[Description("Sets a priority attack for an AI character given the character's name and attack name")]
		public string SetPriorityAttack(string aiCharacter, string aiAttack)
		{
			if (string.IsNullOrEmpty(aiCharacter))
			{
				return "Cannot set priority attack - character string is null or empty";
			}
			if (string.IsNullOrEmpty(aiAttack))
			{
				return "Cannot set priority attack - attack string is null or empty";
			}
			GameObject gameObject = GameObject.Find(aiCharacter);
			if (gameObject == null)
			{
				return "Cannot set priority attack - character <" + aiCharacter + "> not found";
			}
			AICombatController component = gameObject.GetComponent<AICombatController>();
			if (component == null)
			{
				return "Cannot set priority attack - character <" + aiCharacter + "> does not have an ai combat controller";
			}
			CombatController.AttackData attackDataByName = component.getAttackDataByName(aiAttack);
			if (attackDataByName == null)
			{
				return "Cannot set priority attack - character <" + aiCharacter + "> does not have an attack called <" + aiAttack + ">";
			}
			component.PrioritizedAttack = attackDataByName;
			return "Set priority attack <" + aiAttack + "> for character <" + aiCharacter + ">";
		}

		[Description("Clears the priority attack for an AI character given the character's name")]
		public string ClearPriorityAttack(string aiCharacter)
		{
			if (string.IsNullOrEmpty(aiCharacter))
			{
				return "Cannot clear priority attack - character string is null or empty";
			}
			GameObject gameObject = GameObject.Find(aiCharacter);
			if (gameObject == null)
			{
				return "Cannot clear priority attack - character <" + aiCharacter + "> not found";
			}
			AICombatController component = gameObject.GetComponent<AICombatController>();
			if (component == null)
			{
				return "Cannot clear priority attack - character <" + aiCharacter + "> does not have an ai combat controller";
			}
			if (component.PrioritizedAttack != null)
			{
				string attackName = component.PrioritizedAttack.attackName;
				component.PrioritizedAttack = null;
				return "Cleared priority attack <" + attackName + "> for character <" + aiCharacter + ">";
			}
			return "No priority attack to clear for character <" + aiCharacter + ">";
		}

		[Description("Apply a combat effect to the local player.")]
		public void AddBuff(string combatEffectName)
		{
			GameObject localPlayer = GameController.GetController().LocalPlayer;
			if (localPlayer != null)
			{
				CombatController component = localPlayer.GetComponent<CombatController>();
				if (component != null)
				{
					component.createCombatEffect(combatEffectName, component, false);
				}
			}
		}

		[Description("Remove a combat effect from the local player.")]
		public void RemoveBuff(string combatEffectName)
		{
			GameObject localPlayer = GameController.GetController().LocalPlayer;
			if (localPlayer != null)
			{
				CombatController component = localPlayer.GetComponent<CombatController>();
				if (component != null)
				{
					component.removeCombatEffect(combatEffectName);
				}
			}
		}

		[Description("'drink' a potion / consumable")]
		public void DrinkPotion(int ownableTypeId)
		{
			string name = GameController.GetController().LocalPlayer.name;
			AppShell.Instance.EventReporter.ConsumePotion(name, ownableTypeId);
		}

		[Description("simulate daily reward")]
		public void DailyReward(int consecutiveDays, bool rewardReceived, string countdown)
		{
			AppShell.Instance.EventMgr.Fire(null, new DailyRewardMessage(consecutiveDays, rewardReceived, countdown));
		}

		[Description("Enables all disabled VO actions for this game session.")]
		public string EnableVO()
		{
			foreach (VOAction value in Singleton<VOActionDataManager>.instance.VOActions.Values)
			{
				value.Disabled = false;
			}
			return "All VO actions enabled.";
		}

		[Description("Enables warning output for attempts to play unresolved VO objects.")]
		public string WarnOnMissingVO()
		{
			VOObject.AlwaysWarnIfUnresolved = true;
			return "Always warn if VO is unresolved ENABLED.";
		}

		[Description("Enables all coin dispensers in the current scene.")]
		public void EnableAllCoinMachines()
		{
			CoinViewState[] array = Utils.FindObjectsOfType<CoinViewState>();
			foreach (CoinViewState coinViewState in array)
			{
				if (!coinViewState.IsReadyToSpew())
				{
					coinViewState.ForceShow();
					CspUtils.DebugLog("Enabled coin machine: " + coinViewState.name);
				}
			}
		}

		[Description("Dumps the inventory record for the session to the console log")]
		public void LogInventoryRecord()
		{
			AppShell.Instance.InventoryRecorder.LogInventoryRecordList();
		}

		[Description("Enables/Disables character combinations.")]
		public void EnableCharacterCombinations(bool enable)
		{
			CharacterCombinationManager.EnableCombinations(enable);
		}

		[Description("Displays active character combinations in the UI.")]
		public void ShowActiveCharacterCombinations()
		{
			CharacterCombinationBridge characterCombinationBridge = UnityEngine.Object.FindObjectOfType(typeof(CharacterCombinationBridge)) as CharacterCombinationBridge;
			if (characterCombinationBridge != null)
			{
				characterCombinationBridge.ClearDisplayedCombinations();
			}
			if (BrawlerController.Instance != null && BrawlerController.Instance.CharacterCombinationManager != null)
			{
				AppShell.Instance.EventMgr.Fire(null, new PresetCombinationsApplyMessage(BrawlerController.Instance.CharacterCombinationManager.ActiveCombinations));
			}
		}

		[Description("Set the timescale (0 = paused, 1 = default speed).")]
		public void SetTimeScale(float timescale)
		{
			Time.timeScale = timescale;
		}
	}
}
