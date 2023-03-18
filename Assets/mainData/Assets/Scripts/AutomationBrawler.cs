using System;
using System.Xml;
using UnityEngine;

public class AutomationBrawler : AutomationBehavior
{
	private GameObject snapObject;

	public static AutomationBrawler instance
	{
		get
		{
			return new AutomationBrawler();
		}
	}

	public override bool move(Vector3 location)
	{
		bool flag = base.move(location);
		if (flag)
		{
			AutomationManager.Instance.objLocation = location;
		}
		return flag;
	}

	public bool snap(string obj)
	{
		snapObject = GameObject.Find(obj);
		if ((bool)snapObject)
		{
			snapObject.active = false;
		}
		return true;
	}

	public override bool fight()
	{
		return base.fight();
	}

	public Vector3 POIMap(string pointOfInterest)
	{
		char[] separator = new char[6]
		{
			' ',
			',',
			'(',
			')',
			'[',
			']'
		};
		string[] array = pointOfInterest.Split(separator);
		Vector3 result = new Vector3(0f, 0f, 0f);
		if (array.Length == 3)
		{
			result.x = Convert.ToSingle(array[0]);
			result.y = Convert.ToSingle(array[1]);
			result.z = Convert.ToSingle(array[2]);
			return result;
		}
		throw new AutomationExecuteException("Invalid Arguments. Make sure you have entered correct number of arguments ");
	}

	public void PlaySolo(string mission)
	{
		AppShell.Instance.SharedHashTable["BrawlerAirlockPlaySolo"] = true;
		AppShell.Instance.QueueLocationInfo();
		AutomationManager.Instance.LogAttribute("missionName", mission);
		AppShell.Instance.Matchmaker2.SoloBrawler(mission, OnAcceptBrawler);
	}

	public void InviteFriends(string mission)
	{
		AutomationManager.Instance.LogAttribute("missionName", mission);
		AppShell.Instance.Matchmaker2.FriendsBrawler(mission, OnAcceptBrawler);
	}

	public void InviteEveryone(string mission)
	{
		string value = "*";
		if (!string.IsNullOrEmpty(mission))
		{
			value = mission;
		}
		AppShell.Instance.Matchmaker2.AnyoneBrawler(OnAcceptBrawler, ""); // CSP
		AutomationManager.Instance.LogAttribute("missionName", value);
	}

	public bool AcceptInvite()
	{
		Matchmaker2.Invitation topInvitation = AppShell.Instance.Matchmaker2.GetTopInvitation();
		if (topInvitation != null)
		{
			Matchmaker2.GameType gameType = topInvitation.gameType;
			if (gameType == Matchmaker2.GameType.BRAWLER)
			{
				topInvitation.Accept(OnAcceptBrawler);
				return true;
			}
			return false;
		}
		return false;
	}

	public void OnAcceptBrawler(Matchmaker2.Ticket ticket)
	{
		CspUtils.DebugLog(ticket + " Console Command OnAcceptBrawler");
		if (ticket.status == Matchmaker2.Ticket.Status.SUCCESS)
		{
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
			return;
		}
		throw new AutomationExecuteException("Unable to Accept Brawler Invite");
	}

	private void PlayBeginCallback(Matchmaker2.Ticket ticket)
	{
		if (ticket.status == Matchmaker2.Ticket.Status.SUCCESS)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(ticket.ticket);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("//mission");
			if (xmlNode == null)
			{
				CspUtils.DebugLog("Brawler ticket does not contain the mission: " + ticket.ticket);
				return;
			}
			ticket.local = true;
			AppShell.Instance.SharedHashTable["BrawlerTicket"] = ticket;
			ActiveMission activeMission = new ActiveMission(xmlNode.InnerText);
			activeMission.BecomeActiveMission();
			AppShell.Instance.Transition(GameController.ControllerType.Brawler);
		}
	}

	public void DefeatActiveEnemies()
	{
		CombatController.AttackData attackData = AttackDataManager.Instance.getAttackData("DebugCheatKillAttack");
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
}
