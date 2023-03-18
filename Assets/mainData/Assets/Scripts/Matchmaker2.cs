using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using UnityEngine;

public class Matchmaker2
{
	public class Ticket
	{
		public enum Status
		{
			SUCCESS,
			CANCELED,
			ERROR
		}

		public Status status;

		public string server;

		public string ticket;

		public string invitation;

		public int invitationId = -1;

		public bool local;

		public Ticket(Status code, string server, string ticket, string invitation)
		{
			status = code;
			this.server = server;
			this.ticket = ticket;
			this.invitation = invitation;
			invitationId = -1;
			if (code != 0)
			{
				return;
			}
			if (invitation != null)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(invitation);
				XmlNode xmlNode = xmlDocument.SelectSingleNode("//invitation_id");
				if (xmlNode != null)
				{
					invitationId = int.Parse(xmlNode.InnerText);
				}
			}
			else if (ticket != null)
			{
				XmlDocument xmlDocument2 = new XmlDocument();
				xmlDocument2.LoadXml(ticket);
				XmlNode xmlNode2 = xmlDocument2.SelectSingleNode("//invitation_id");
				if (xmlNode2 != null)
				{
					invitationId = int.Parse(xmlNode2.InnerText);
				}
			}
		}

		public override string ToString()
		{
			return string.Format("Ticket: Status={0}, Server={1}, ticket={2}, invitationId={3}, invitation={4}", status, server, ticket, invitationId, invitation);
		}
	}

	public enum GameType
	{
		WORLD,
		BRAWLER,
		CARD,
		CARDSA,
		HQ,
		ARCADE
	}

	public abstract class Invitation
	{
		public int invitationId;

		public GameType gameType;

		public int inviterId;

		public string inviterName;

		public DateTime createdOn;

		public bool attemptingAccept;

		public abstract void Query();

		public abstract void Accept(OnTicketDelegate onTicket);

		public abstract void Cancel(bool remove);
	}

	public class CardGameInvitation : Invitation
	{
		private string selectedHero;

		private string selectedDeck;

		public CardGameInvitation()
		{
			gameType = GameType.CARD;
			createdOn = DateTime.Now;
			attemptingAccept = false;
		}

		public override void Query()
		{
		}

		public override void Accept(OnTicketDelegate onTicket)
		{
			if (selectedHero != null && selectedDeck != null)
			{
				AppShell.Instance.Matchmaker2.AcceptCardGame(invitationId, selectedHero, selectedDeck, onTicket);
			}
		}

		public override void Cancel(bool remove)
		{
			AppShell.Instance.Matchmaker2.RejectCardGame(invitationId, remove);
		}

		public void SetAcceptValues(string hero, string deck)
		{
			selectedHero = hero;
			selectedDeck = deck;
		}
	}

	public class BrawlerInvitation : Invitation
	{
		public string missionId;

		public BrawlerInvitation()
		{
			gameType = GameType.BRAWLER;
			createdOn = DateTime.Now;
			missionId = string.Empty;
			attemptingAccept = false;
		}

		public override void Query()
		{
		}

		public override void Accept(OnTicketDelegate onTicket)
		{
			AppShell.Instance.Matchmaker2.AcceptBrawler(invitationId, onTicket);
		}

		public override void Cancel(bool remove)
		{
			AppShell.Instance.Matchmaker2.CancelBrawler(invitationId, remove);
		}
	}

	public delegate void OnTicketDelegate(Ticket ticket);

	protected delegate void WebCallback(int inTransactionId, ShsWebResponse response);

	public const float cardGameInviteCooldown = 30f;

	public const float cardGameInviteReceiveCooldown = 30f;

	protected int transactionId;

	protected OnTicketDelegate transactionDelegate;

	protected Ticket lastTicket;

	protected List<Invitation> invitations;

	protected Hashtable invitationsDisposed;

	private OnTicketDelegate cspTicketDeletgate = null;  // CSP

	public Matchmaker2()
	{
		transactionId = 0;
		transactionDelegate = null;
		lastTicket = null;
		invitations = new List<Invitation>();
		invitationsDisposed = new Hashtable();
	}

	public void JoinGameWorld(string zone, OnTicketDelegate onTicket)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("game", GameTypeToString(GameType.WORLD));
		wWWForm.AddField("zone", zone);
		PostAndUpdateTransaction(wWWForm, onTicket, ProcessGameWorldResponse, "mm/join");
	}

	public void JoinGameWorldWithUser(int userId, OnTicketDelegate onTicket)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("game", GameTypeToString(GameType.WORLD));
		wWWForm.AddField("buddy_id", userId);
		PostAndUpdateTransaction(wWWForm, onTicket, ProcessGameWorldResponse, "mm/join");
	}

	public void OnCardGameInvitation(Hashtable payload)
	{
		int num = int.Parse((string)payload["inviter_id"]);
		if (!invitationsDisposed.ContainsKey(num) || Time.time - (float)invitationsDisposed[num] > 30f)
		{
			invitationsDisposed.Remove(num);
			CardGameInvitation cardGameInvitation = new CardGameInvitation();
			cardGameInvitation.invitationId = num;
			cardGameInvitation.inviterId = num;
			cardGameInvitation.inviterName = RTCClient.DecodeString((string)payload["inviter_name"]);
			AddInvitation(cardGameInvitation);
		}
	}

	public void OnCardGameStart(Hashtable payload)
	{
		int id = int.Parse((string)payload["invitation_id"]);
		DeleteInvitationById(id, InvitationCancelReason.GameIsStarting);
		CancelAllInvitations();
		if (transactionDelegate != null)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml((string)payload["ticket"]);
			string innerText = xmlDocument.SelectSingleNode("//ticket/server").InnerText;
			string outerXml = xmlDocument.SelectSingleNode("//ticket").OuterXml;
			CallTransactionDelegate(new Ticket(Ticket.Status.SUCCESS, innerText, outerXml, null));
		}
	}

	public void OnCardGameCancel(Hashtable payload)
	{
		int id = int.Parse((string)payload["invitation_id"]);
		DeleteInvitationById(id, InvitationCancelReason.PlayerCanceled);
	}

	public void SoloCardGame(int arena, string hero, string deck, int questNodeId)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("arena", arena);
		wWWForm.AddField("hero", hero);
		wWWForm.AddField("deck", deck);
		wWWForm.AddField("quest_node_id", questNodeId);
		PostAndUpdateTransaction(wWWForm, CardGameTicketDelegate, ProcessCardGameSoloResponse, "mm/card/solo");
	}

	protected void CardGameTicketDelegate(Ticket ticket)
	{
		AppShell.Instance.SharedHashTable["CardGameTicket"] = ticket;
	}

	public void InviteCardGame(long[] users, string arena, string hero, string deck, OnTicketDelegate onTicket)
	{
		string[] users2 = Array.ConvertAll(users, delegate(long userId)
		{
			return userId.ToString();
		});
		InviteCardGame(users2, arena, hero, deck, onTicket);
	}

	public void InviteCardGame(string[] users, string arena, string hero, string deck, OnTicketDelegate onTicket)
	{
		if (onTicket == null)
		{
			onTicket = DefaultCardGameTicketDelegate;
		}
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("game", GameTypeToString(GameType.CARD));
		wWWForm.AddField("arena", arena);
		wWWForm.AddField("hero", hero);
		wWWForm.AddField("deck", deck);
		if (users != null)
		{
			wWWForm.AddField("invitees", string.Join(",", users));
		}
		else
		{
			wWWForm.AddField("invitees", string.Empty);
		}
		PostAndUpdateTransaction(wWWForm, onTicket, ProcessCardGameInviteResponseSA, "mm/card/create/");
	}

	public void AcceptCardGame(int invitationId, string hero, string deck, OnTicketDelegate onTicket)
	{
		Invitation invitation = FindInvitationById(invitationId);
		if (invitation != null && invitation.gameType == GameType.CARD)
		{
			invitation.attemptingAccept = true;
			if (onTicket == null)
			{
				onTicket = DefaultCardGameTicketDelegate;
			}
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("game", GameTypeToString(GameType.CARD));
			wWWForm.AddField("invitation_id", invitationId);
			wWWForm.AddField("accept", 1);
			wWWForm.AddField("hero", hero);
			wWWForm.AddField("deck", deck);
			PostAndUpdateTransaction(wWWForm, onTicket, ProcessCardGameAcceptResponse, "mm/card/accept");
		}
		else if (onTicket != null)
		{
			onTicket(new Ticket(Ticket.Status.CANCELED, null, null, null));
		}
	}

	public void EnterPvPQueue(string hero, string deck, string arena, OnTicketDelegate onTicket)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("hero", hero);
		wWWForm.AddField("deck", deck);
		wWWForm.AddField("arena", arena);
		PostAndUpdateTransaction(wWWForm, onTicket, ProcessPvPQueueResponseSA, "mm/card/anyone");
	}

	public void LeavePvPQueue(OnTicketDelegate onTicket)
	{
		WWWForm formData = new WWWForm();
		PostAndUpdateTransaction(formData, onTicket, ProcessPvPQueueResponseSA, "mm/card/leave");
	}

	public void RejectCardGame(int invitationId, bool removeInvite)
	{
		Invitation invitation = FindInvitationById(invitationId);
		if (invitation != null && invitation.gameType == GameType.CARD)
		{
			if (removeInvite)
			{
				DeleteInvitationById(invitationId, InvitationCancelReason.PlayerCanceled);
			}
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("game", GameTypeToString(GameType.CARD));
			wWWForm.AddField("invitation_id", invitationId);
			wWWForm.AddField("accept", 0);
			PostAndNoUpdateTransaction(wWWForm, ProcessCardGameRejectResponse, "mm/card/accept");
		}
	}

	public void CancelCardGameInvitation(int invitationId)
	{
		Invitation invitation = FindInvitationById(invitationId);
		if (invitation != null && invitation.gameType == GameType.CARD)
		{
			DeleteInvitationById(invitationId, InvitationCancelReason.PlayerCanceled);
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("game", GameTypeToString(GameType.CARD));
			wWWForm.AddField("invitation_id", invitationId);
			PostAndNoUpdateTransaction(wWWForm, ProcessCardGameRejectResponse, "mm/card/leave");
		}
	}

	public void SoloBrawler(string missionId, OnTicketDelegate onTicket)
	{
		if (onTicket == null)
		{
			onTicket = DefaultBrawlerTicketDelegate;
		}
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("game", GameTypeToString(GameType.BRAWLER));
		wWWForm.AddField("mission", missionId);
		CspUtils.DebugLog("solobrawler: " + missionId);
		PostAndUpdateTransaction(wWWForm, onTicket, ProcessBrawlerSoloResponse, "mm/brawler/solo");
	}

	public void MiniBrawler(string missionId, OnTicketDelegate onTicket)
	{
		if (onTicket == null)
		{
			onTicket = DefaultBrawlerTicketDelegate;
		}
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("requested_mission", missionId);
		CspUtils.DebugLog("minibrawler: " + missionId);
		PostAndUpdateTransaction(wWWForm, onTicket, ProcessBrawlerInvitationResponse, "mm/brawler/anyone.py");
	}

	public void AnyoneBrawler(OnTicketDelegate onTicket, string missionId)
	{
		/////// block added by CSP for testing....needs to be fixed.  ///////////
		transactionId = 0;
		transactionDelegate = null;
		lastTicket = null;
		invitations = new List<Invitation>();
		invitationsDisposed = new Hashtable();
		cspTicketDeletgate = onTicket;
		/////////////////////////////////////////////////////////////////////////////////////////
		
		if (onTicket == null)
		{
			onTicket = DefaultBrawlerTicketDelegate;
		}
		WWWForm formData = new WWWForm();
		CspUtils.DebugLog("anyonebrawler: ");
		formData.AddField("mission", missionId);  // added by CSP
		//CspUtils.DebugLog("ab missionid=" + missionId); // CSP
		//PostAndUpdateTransaction(formData, onTicket, ProcessBrawlerInvitationResponse, "mm/brawler/anyone");
		PostAndUpdateTransaction(formData, onTicket, ProcessBrawlerAnyoneResponse, "mm/brawler/anyone.py");  // added by CSP
	}

	public void FriendsBrawler(string missionId, OnTicketDelegate onTicket)
	{
		if (onTicket == null)
		{
			onTicket = DefaultBrawlerTicketDelegate;
		}
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("game", GameTypeToString(GameType.BRAWLER));
		wWWForm.AddField("mission", missionId);
		CspUtils.DebugLog("friendsbrawler: " + missionId);
		PostAndUpdateTransaction(wWWForm, onTicket, ProcessBrawlerInvitationResponse, "mm/brawler/friends.py");
	}

	public void InviteBrawler(string missionId, string[] invitees, OnTicketDelegate onTicket)
	{
		if (onTicket == null)
		{
			onTicket = DefaultBrawlerTicketDelegate;
		}
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("game", GameTypeToString(GameType.BRAWLER));
		wWWForm.AddField("mission", missionId);
		wWWForm.AddField("invitees", string.Join(",", invitees));
		CspUtils.DebugLog("invitebrawler: " + missionId);
		PostAndUpdateTransaction(wWWForm, onTicket, ProcessBrawlerInvitationResponse, "mm/brawler/friends.py");
	}

	public void AcceptBrawler(int invitationId, OnTicketDelegate onTicket)
	{
		/////// block added by CSP for testing....needs to be fixed.  ///////////
		transactionId = 0;
		transactionDelegate = null;
		lastTicket = null;
		//invitations = new List<Invitation>();
		//invitationsDisposed = new Hashtable();
		cspTicketDeletgate = onTicket;
		/////////////////////////////////////////////////////////////////////////////////////////

		Invitation invitation = FindInvitationById(invitationId);
		if (invitation != null && invitation.gameType == GameType.BRAWLER)
		{
			invitation.attemptingAccept = true;
			if (onTicket == null)
			{
				onTicket = DefaultBrawlerTicketDelegate;
			}

			//////////// CSP temp block until fix accept mission invite /////////////////////////
			//WWWForm formData = new WWWForm();
			//CspUtils.DebugLog("anyonebrawler: ");
			//BrawlerInvitation bi = (BrawlerInvitation)invitation;
			//formData.AddField("mission", bi.missionId);  // added by CSP
			////CspUtils.DebugLog("ab missionid=" + missionId); // CSP
			////PostAndUpdateTransaction(formData, onTicket, ProcessBrawlerInvitationResponse, "mm/brawler/anyone");
			//PostAndUpdateTransaction(formData, onTicket, ProcessBrawlerAnyoneResponse, "mm/brawler/anyone.py");  // added by CSP
			//return;
			//////////////////////////////////


			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("game", GameTypeToString(GameType.BRAWLER));
			wWWForm.AddField("invitation_id", invitationId);
			wWWForm.AddField("accept", 1);
			wWWForm.AddField("inviter_id", invitation.inviterId);  // added by CSP
			wWWForm.AddField("mission", ((BrawlerInvitation)invitation).missionId);  // added by CSP
			
			PostAndUpdateTransaction(wWWForm, onTicket, delegate(int inTransactionId, ShsWebResponse response)
			{
				ProcessBrawlerAcceptResponse(inTransactionId, invitationId, response);
			}, "mm/brawler/accept.py");
		}
		else if (onTicket != null)
		{
			onTicket(new Ticket(Ticket.Status.CANCELED, null, null, null));
		}
	}

	public void CancelBrawler(int invitationId, bool remove)
	{
		Invitation invitation = FindInvitationById(invitationId);
		if (invitation != null && invitation.gameType == GameType.BRAWLER)
		{
			if (remove)
			{
				DeleteInvitationById(invitationId, InvitationCancelReason.PlayerCanceled);
			}
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("game", GameTypeToString(GameType.BRAWLER));
			wWWForm.AddField("invitation_id", invitationId);
			wWWForm.AddField("accept", 0);
			PostAndNoUpdateTransaction(wWWForm, ProcessNullResponse, "mm/brawler/accept");
		}
	}

	public void LockBrawler(int invitationId)
	{
		if (invitationId == -1)
		{
			if (lastTicket == null)
			{
				CspUtils.DebugLog("LockBrawler called with lastTicket == null");
				return;
			}
			if (lastTicket.status != 0)
			{
				CspUtils.DebugLog("LockBrawler called with lastTicket.status = " + lastTicket.status);
				return;
			}
			if (lastTicket.invitationId < 0)
			{
				CspUtils.DebugLog("Last ticket invitation was negative, cannot lock brawler to the MM.");
				return;
			}
			invitationId = lastTicket.invitationId;
		}
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("game", GameTypeToString(GameType.BRAWLER));
		wWWForm.AddField("invitation_id", invitationId);
		PostAndNoUpdateTransaction(wWWForm, ProcessNullResponse, "mm/brawler/lock");
	}

	public void OnBrawlerInvitation(Hashtable payload)
	{
		int num = int.Parse((string)payload["invitation_id"]);

		if (invitationsDisposed.ContainsKey(num))  // added by CSP for testing
			invitationsDisposed.Remove(num);	// added by CSP for testing

		if (!invitationsDisposed.ContainsKey(num))
		{
			BrawlerInvitation brawlerInvitation = new BrawlerInvitation();
			brawlerInvitation.invitationId = num;
			brawlerInvitation.inviterId = int.Parse((string)payload["inviter_id"]);
			brawlerInvitation.inviterName = (string)payload["inviter_name"]; // CSP //RTCClient.DecodeString((string)payload["inviter_name"]);
			brawlerInvitation.missionId = (string)payload["mission"];
			brawlerInvitation.createdOn = DateTime.Now;
			AddInvitation(brawlerInvitation);
		}
	}

	public void OnBrawlerStart(Hashtable payload)
	{
		int id = int.Parse((string)payload["invitation_id"]);
		DeleteInvitationById(id, InvitationCancelReason.GameIsStarting);
		CancelAllInvitations();
		if (transactionDelegate != null)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml((string)payload["ticket"]);
			string innerText = xmlDocument.SelectSingleNode("//ticket/server").InnerText;
			string outerXml = xmlDocument.SelectSingleNode("//ticket").OuterXml;
			CallTransactionDelegate(new Ticket(Ticket.Status.SUCCESS, innerText, outerXml, null));
		}
	}

	public void OnBrawlerLock(Hashtable payload)
	{
		int id = int.Parse((string)payload["invitation_id"]);
		DeleteInvitationById(id, InvitationCancelReason.GameIsStarting);
	}

	public void SoloHQ()
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("game", GameTypeToString(GameType.HQ));
		PostAndUpdateTransaction(wWWForm, null, ProcessHQResponse, "mm/hq/knock");
	}

	public void VisitHQ(int friendId, OnTicketDelegate onTicket)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("game", GameTypeToString(GameType.HQ));
		wWWForm.AddField("target", friendId);
		PostAndUpdateTransaction(wWWForm, onTicket, ProcessHQResponse, "mm/hq/knock");
	}

	public void Arcade(string game)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("game", GameTypeToString(GameType.ARCADE));
		wWWForm.AddField("playing", game);
		PostAndUpdateTransaction(wWWForm, null, ProcessArcadeResponse, "mm/arcade/enter");
	}

	public Invitation GetTopInvitation()
	{
		if (invitations.Count > 0)
		{
			return invitations[0];
		}
		return null;
	}

	public void Cancel()
	{
		SetTransactionDelegate(null);
		transactionId++;
	}

	public static string GameTypeToString(GameType game)
	{
		switch (game)
		{
		case GameType.WORLD:
			return "WORLD";
		case GameType.BRAWLER:
			return "BRAWLER";
		case GameType.CARD:
			return "CARD";
		case GameType.CARDSA:
			return "CARDSA";
		case GameType.HQ:
			return "HQ";
		case GameType.ARCADE:
			return "ARCADE";
		default:
			CspUtils.DebugLog("Unknown game type: " + game);
			return "WORLD";
		}
	}

	protected void MMLog(string x)
	{
	}

	protected Invitation FindInvitationById(int id)
	{
		return invitations.Find(delegate(Invitation i)
		{
			return i.invitationId == id;
		});
	}

	protected void DeleteInvitationById(int id, InvitationCancelReason reason)
	{
		Invitation invitation = FindInvitationById(id);
		if (invitation != null)
		{
			SendInvitationCancelEvent(invitation, reason);
			invitationsDisposed[id] = Time.time;
			invitations.RemoveAll(delegate(Invitation i)
			{
				return i.invitationId == id;
			});
		}
	}

	protected void SendInvitationCancelEvent(Invitation invite, InvitationCancelReason reason)
	{
		switch (invite.gameType)
		{
		case GameType.CARD:
			AppShell.Instance.EventMgr.Fire(null, new InvitationCardGameCanceledMessage((CardGameInvitation)invite, reason));
			break;
		case GameType.BRAWLER:
			AppShell.Instance.EventMgr.Fire(null, new InvitationBrawlerCanceledMessage((BrawlerInvitation)invite, reason));
			break;
		}
	}

	protected void AddInvitation(Invitation invite)
	{
		invitations.Add(invite);
		if (invite.inviterId != -1)
		{
			switch (invite.gameType)
			{
			case GameType.CARD:
				AppShell.Instance.EventMgr.Fire(null, new InvitationCardGameMessage((CardGameInvitation)invite));
				break;
			case GameType.BRAWLER:
				CspUtils.DebugLog("AddInvitation BRAWLER");
				AppShell.Instance.EventMgr.Fire(null, new InvitationBrawlerMessage((BrawlerInvitation)invite));
				AppShell.Instance.TimerMgr.CreateTimer(55f - (float)(DateTime.Now - invite.createdOn).Seconds, delegate(long timerId, bool canceled)
				{
					if (!canceled && !invite.attemptingAccept)
					{
						DeleteInvitationById(invite.invitationId, InvitationCancelReason.TimedOut);
					}
				});
				break;
			}
		}
	}

	public void CancelAllInvitations()
	{
		foreach (Invitation invitation in invitations)
		{
			SendInvitationCancelEvent(invitation, InvitationCancelReason.PlayerCanceled);
			invitation.Cancel(false);
			invitationsDisposed[invitation.invitationId] = Time.time;
		}
		invitations.Clear();
	}

	protected int IncrementTransactionId()
	{
		Cancel();
		return ++transactionId;
	}

	protected void SetTransactionDelegate(OnTicketDelegate onTicket)
	{
		if (transactionDelegate != null)
		{
			CspUtils.DebugLog("Canceling a pending matchmaking call");
			
			// the following line commented out by CSP for testing...needs to be fixed later.
			//transactionDelegate(new Ticket(Ticket.Status.CANCELED, null, null, null));
		}
		transactionDelegate = onTicket;
	}

	protected void CallTransactionDelegate(Ticket ticket)
	{
		lastTicket = ticket;
		if (transactionDelegate != null)
		{
			transactionDelegate(ticket);
			transactionDelegate = null;
		}
	}

	protected void PostAndUpdateTransaction(WWWForm formData, OnTicketDelegate onTicket, WebCallback callback, string url)
	{
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null)
		{
			throw new Exception("No user profile");
		}
		formData.AddField("user", profile.UserId.ToString());
		int localTransactionid = IncrementTransactionId();
		SetTransactionDelegate(onTicket);
		AppShell.Instance.WebService.StartRequest("resources$" + url, delegate(ShsWebResponse response)
		{
			callback(localTransactionid, response);
		}, formData.data, ShsWebService.ShsWebServiceType.RASP);
	}

	protected void PostAndNoUpdateTransaction(WWWForm formData, WebCallback callback, string url)
	{
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null)
		{
			throw new Exception("No user profile");
		}
		formData.AddField("user", profile.UserId.ToString());
		AppShell.Instance.WebService.StartRequest("resources$" + url, delegate(ShsWebResponse response)
		{
			callback(-1, response);
		}, formData.data, ShsWebService.ShsWebServiceType.RASP);
	}

	protected void ProcessGameWorldResponse(int inTransactionId, ShsWebResponse response)
	{
		if (inTransactionId == transactionId)
		{
			HttpStatusCode status = (HttpStatusCode)response.Status;
			HttpStatusCode httpStatusCode = status;
			if (httpStatusCode == HttpStatusCode.OK)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(response.Body);
				string innerText = xmlDocument.SelectSingleNode("//ticket/server").InnerText;
				string outerXml = xmlDocument.SelectSingleNode("//ticket").OuterXml;
				CallTransactionDelegate(new Ticket(Ticket.Status.SUCCESS, innerText, outerXml, null));
			}
			else
			{
				CallTransactionDelegate(new Ticket(Ticket.Status.ERROR, status.ToString(), response.Body, null));
			}
		}
	}

	protected void DefaultCardGameTicketDelegate(Ticket ticket)
	{
		AppShell.Instance.SharedHashTable["CardGameTicket"] = ticket;
	}

	protected void ProcessCardGameSoloResponse(int inTransactionId, ShsWebResponse response)
	{
		if (inTransactionId == transactionId)
		{
			HttpStatusCode status = (HttpStatusCode)response.Status;
			HttpStatusCode httpStatusCode = status;
			if (httpStatusCode == HttpStatusCode.OK)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(response.Body);
				string innerText = xmlDocument.SelectSingleNode("//ticket/server").InnerText;
				string outerXml = xmlDocument.SelectSingleNode("//ticket").OuterXml;
				CallTransactionDelegate(new Ticket(Ticket.Status.SUCCESS, innerText, outerXml, null));
			}
			else
			{
				CallTransactionDelegate(new Ticket(Ticket.Status.ERROR, status.ToString(), response.Body, null));
			}
		}
	}

	protected void ProcessCardGameInviteResponse(int inTransactionId, ShsWebResponse response)
	{
		HttpStatusCode status = (HttpStatusCode)response.Status;
		HttpStatusCode httpStatusCode = status;
		if (httpStatusCode == HttpStatusCode.OK)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response.Body);
			int invitationId = int.Parse(xmlDocument.SelectSingleNode("//invitation/invitation_id").InnerText);
			if (inTransactionId == transactionId)
			{
				CardGameInvitation cardGameInvitation = new CardGameInvitation();
				cardGameInvitation.invitationId = invitationId;
				cardGameInvitation.inviterId = -1;
				cardGameInvitation.inviterName = null;
				AddInvitation(cardGameInvitation);
			}
			else
			{
				RejectCardGame(invitationId, true);
			}
		}
	}

	protected void ProcessCardGameInviteResponseSA(int inTransactionId, ShsWebResponse response)
	{
		HttpStatusCode status = (HttpStatusCode)response.Status;
		HttpStatusCode httpStatusCode = status;
		if (httpStatusCode == HttpStatusCode.OK)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response.Body);
			int invitationId = int.Parse(xmlDocument.SelectSingleNode("//invitation/invitation_id").InnerText);
			if (inTransactionId == transactionId)
			{
				CardGameInvitation cardGameInvitation = new CardGameInvitation();
				cardGameInvitation.invitationId = invitationId;
				cardGameInvitation.inviterId = -1;
				cardGameInvitation.inviterName = null;
				AddInvitation(cardGameInvitation);
			}
			else
			{
				RejectCardGame(invitationId, true);
			}
		}
	}

	protected void ProcessCardGameAcceptResponse(int inTransactionId, ShsWebResponse response)
	{
		if (inTransactionId == transactionId)
		{
			HttpStatusCode status = (HttpStatusCode)response.Status;
			HttpStatusCode httpStatusCode = status;
			if (httpStatusCode != HttpStatusCode.OK)
			{
				CspUtils.DebugLog("ProcessCardGameAcceptResponse fail: " + response.Body);
			}
		}
	}

	protected void ProcessCardGameRejectResponse(int inTransactionId, ShsWebResponse response)
	{
		HttpStatusCode status = (HttpStatusCode)response.Status;
		HttpStatusCode httpStatusCode = status;
		if (httpStatusCode != HttpStatusCode.OK)
		{
			CspUtils.DebugLog("ProcessCardGameRejectResponse fail: " + response.Body);
		}
	}

	protected void ProcessPvPQueueResponseSA(int inTransactionId, ShsWebResponse response)
	{
		HttpStatusCode status = (HttpStatusCode)response.Status;
		HttpStatusCode httpStatusCode = status;
		if (httpStatusCode != HttpStatusCode.OK)
		{
			CspUtils.DebugLog("Received non-success response for PvP Queue request: " + response.Body);
		}
	}

	protected void DefaultBrawlerTicketDelegate(Ticket ticket)
	{
		AppShell.Instance.SharedHashTable["BrawlerTicket"] = ticket;
	}

	protected void ProcessNullResponse(int inTransactionId, ShsWebResponse response)
	{
	}

	protected void ProcessBrawlerSoloResponse(int inTransactionId, ShsWebResponse response)
	{
		if (inTransactionId == transactionId)
		{
			HttpStatusCode status = (HttpStatusCode)response.Status;
			HttpStatusCode httpStatusCode = status;
			if (httpStatusCode == HttpStatusCode.OK)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(response.Body);
				string innerText = xmlDocument.SelectSingleNode("//ticket/server").InnerText;
				string outerXml = xmlDocument.SelectSingleNode("//ticket").OuterXml;
				CallTransactionDelegate(new Ticket(Ticket.Status.SUCCESS, innerText, outerXml, null));
			}
			else
			{
				CallTransactionDelegate(new Ticket(Ticket.Status.ERROR, status.ToString(), response.Body, null));
			}
		}
	}

	// this method added by CSP
	protected void ProcessBrawlerAnyoneResponse(int inTransactionId, ShsWebResponse response)
	{
		CspUtils.DebugLog("ProcessBrawlerAnyoneResponse");
		// CspUtils.DebugLog("body=" + response.Body); // CSP
		if (inTransactionId != transactionId)
		{
			return;
		}
		HttpStatusCode status = (HttpStatusCode)response.Status;
		HttpStatusCode httpStatusCode = status;
		if (httpStatusCode == HttpStatusCode.OK)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response.Body);
			int num = int.Parse(xmlDocument.SelectSingleNode("//response/invitation/invitation_id").InnerText);
			if (!invitationsDisposed.ContainsKey(num))
			{
				BrawlerInvitation brawlerInvitation = new BrawlerInvitation();
				brawlerInvitation.invitationId = num;
				brawlerInvitation.inviterId = -1;
				brawlerInvitation.inviterName = null;
				AddInvitation(brawlerInvitation);

				/////////////  block added by CSP for testing. /////////////////////
				//brawlerInvitation.Accept(transactionDelegate); 

				//// NOTE!! NEED TO CHANGE THIS SO THAT A MSG IS SENT TO SERVER TO EITHER CREATE NEW ROOM
				////    OR JOIN EXISTING ROOM, THEN SEND BACK REPSONSE THAT WILL INVOKE OnBrawlerStart (With
				////    ticket sent to client)

				//// MORE NOTES: CLIENT TRIES TO CONNECT TO GAME ROOM (WITH TICKET) WHEN PLAYER ARRIVES AT AIRLOCK...
				////    ...WHICH HAPPENS AFTER CURRENT METHOD ProcessBrawlerInvitationResponse() IS CALLED.....
				////    SO CALL TO FIND/CREATE GAME ROOM CAN BE CALLED HERE.	

				//// NEW IDEA! write python script for mm/brawler/anyone. when processing response here, check if source was 
				////    anyone script. if it was, call another python script that does the stuff below...then remove the code 
				////    below. in ticket, if game=brawler and instance=-1, then get any brawler game room with less than 
				////    4 players (otherwise create new brlawer game room).


				//int roomType = 5;  // 0=lobby 1=bugle 2=baxter 3=asgard 4=villainville 5=daily mission 6=invite-to-mission
				//bool isHost = false;  // only really matters for invite-to-mission mode.			
				//AppShell.Instance.ServerConnection.GetGameRoom(roomType,isHost);
			
				Hashtable payload = new Hashtable();
				payload.Add("invitation_id", brawlerInvitation.invitationId.ToString());

				string ticketStr = xmlDocument.SelectSingleNode("//response/ticket").InnerXml;
				//CspUtils.DebugLog("ticketStr= " + ticketStr);
				string cspTicket = "<ticket>" + ticketStr + "</ticket>";
				payload.Add("ticket", cspTicket);

				OnBrawlerStart(payload);
				//////////////////////////////////////////////////////////////////
			}
		}
		else
		{
			CallTransactionDelegate(new Ticket(Ticket.Status.ERROR, status.ToString(), response.Body, null));
		}
	}

	protected void ProcessBrawlerInvitationResponse(int inTransactionId, ShsWebResponse response)
	{
		CspUtils.DebugLog("ProcessBrawlerInvitationResponse");
		//CspUtils.DebugLog("response.Body=" + response.Body);
		
		if (inTransactionId != transactionId)
		{
			CspUtils.DebugLog("ProcessBrawlerInvitationResponse - returning!");
			return;
		}
		HttpStatusCode status = (HttpStatusCode)response.Status;
		HttpStatusCode httpStatusCode = status;
		if (httpStatusCode == HttpStatusCode.OK)
		{
			CspUtils.DebugLog("ProcessBrawlerInvitationResponse - status OK");

			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response.Body);
			int num = int.Parse(xmlDocument.SelectSingleNode("//invitation/invitation_id").InnerText);
			
			if (invitationsDisposed.ContainsKey(num))  // added by CSP for testing
				invitationsDisposed.Remove(num);	// added by CSP for testing

			if (!invitationsDisposed.ContainsKey(num))
			{
				CspUtils.DebugLog("ProcessBrawlerInvitationResponse - prepping call to OnBrawlerSTart()");

				BrawlerInvitation brawlerInvitation = new BrawlerInvitation();
				brawlerInvitation.invitationId = num;
				brawlerInvitation.inviterId = -1;
				brawlerInvitation.inviterName = null;
				AddInvitation(brawlerInvitation);

				/////////////  block added by CSP for testing. /////////////////////
				
				//int roomType = 5;  // 0=lobby 1=bugle 2=baxter 3=asgard 4=villainville 5=daily mission 6=invite-to-mission
				//bool isHost = false;  // only really matters for invite-to-mission mode.			
				//AppShell.Instance.ServerConnection.GetGameRoom(roomType,isHost);
			
				Hashtable payload = new Hashtable();
				payload.Add("invitation_id", brawlerInvitation.invitationId.ToString());

				string ticketStr = xmlDocument.SelectSingleNode("//response/ticket").InnerXml;
				//CspUtils.DebugLog("ticketStr= " + ticketStr);
				string cspTicket = "<ticket>" + ticketStr + "</ticket>";
				payload.Add("ticket", cspTicket);

				OnBrawlerStart(payload);
				//////////////////////////////////////////////////////////////////
			}
		}
		else
		{
			CallTransactionDelegate(new Ticket(Ticket.Status.ERROR, status.ToString(), response.Body, null));
		}
	}

	protected void ProcessBrawlerAcceptResponse(int inTransactionId, int inviteId, ShsWebResponse response)
	{
		CspUtils.DebugLog("ProcessBrawlerAcceptResponse");

		if (inTransactionId != transactionId)
		{
			return;
		}
		HttpStatusCode status = (HttpStatusCode)response.Status;
		HttpStatusCode httpStatusCode = status;
		if (httpStatusCode == HttpStatusCode.OK)
		{

			/////////////  block added by CSP for testing. /////////////////////
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response.Body);
			int num = int.Parse(xmlDocument.SelectSingleNode("//invitation/invitation_id").InnerText);
			
			if (invitationsDisposed.ContainsKey(num))  // added by CSP for testing
				invitationsDisposed.Remove(num);	// added by CSP for testing
			
			if (!invitationsDisposed.ContainsKey(num))
			{
				BrawlerInvitation brawlerInvitation = new BrawlerInvitation();
				brawlerInvitation.invitationId = num;
				brawlerInvitation.inviterId = -1;
				brawlerInvitation.inviterName = null;
				AddInvitation(brawlerInvitation);				
				
				//int roomType = 5;  // 0=lobby 1=bugle 2=baxter 3=asgard 4=villainville 5=daily mission 6=invite-to-mission
				//bool isHost = false;  // only really matters for invite-to-mission mode.			
				//AppShell.Instance.ServerConnection.GetGameRoom(roomType,isHost);
			
				Hashtable payload = new Hashtable();
				payload.Add("invitation_id", brawlerInvitation.invitationId.ToString());

				string ticketStr = xmlDocument.SelectSingleNode("//response/ticket").InnerXml;
				//CspUtils.DebugLog("ticketStr= " + ticketStr);
				string cspTicket = "<ticket>" + ticketStr + "</ticket>";
				payload.Add("ticket", cspTicket);

				OnBrawlerStart(payload);
				
			}
			//////////////////////////////////////////////////////////////////////



			/// this block commented out by CSP for testing
			// switch (response.Body)
			// {
			// case "OK":
			// 	return;
			// }
			// DeleteInvitationById(inviteId, InvitationCancelReason.GameFull);
			// CallTransactionDelegate(new Ticket(Ticket.Status.CANCELED, status.ToString(), response.Body, null));
		}
		else
		{
			DeleteInvitationById(inviteId, InvitationCancelReason.GameFull);
			CallTransactionDelegate(new Ticket(Ticket.Status.ERROR, status.ToString(), response.Body, null));
		}
	}

	protected void ProcessHQResponse(int inTransactionId, ShsWebResponse response)
	{
		if (inTransactionId != transactionId)
		{
			return;
		}
		HttpStatusCode status = (HttpStatusCode)response.Status;
		HttpStatusCode httpStatusCode = status;
		if (httpStatusCode == HttpStatusCode.OK)
		{
			switch (response.Body)
			{
			case "OK":
				CallTransactionDelegate(new Ticket(Ticket.Status.SUCCESS, null, null, null));
				break;
			default:
				CallTransactionDelegate(new Ticket(Ticket.Status.ERROR, status.ToString(), response.Body, null));
				break;
			}
		}
		else
		{
			CallTransactionDelegate(new Ticket(Ticket.Status.ERROR, status.ToString(), response.Body, null));
		}
	}

	protected void ProcessArcadeResponse(int inTransactionId, ShsWebResponse response)
	{
		if (inTransactionId != transactionId)
		{
			return;
		}
		HttpStatusCode status = (HttpStatusCode)response.Status;
		HttpStatusCode httpStatusCode = status;
		if (httpStatusCode == HttpStatusCode.OK)
		{
			switch (response.Body)
			{
			case "OK":
				CallTransactionDelegate(new Ticket(Ticket.Status.SUCCESS, null, null, null));
				break;
			default:
				CallTransactionDelegate(new Ticket(Ticket.Status.ERROR, status.ToString(), response.Body, null));
				break;
			}
		}
		else
		{
			CallTransactionDelegate(new Ticket(Ticket.Status.ERROR, status.ToString(), response.Body, null));
		}
	}
}
