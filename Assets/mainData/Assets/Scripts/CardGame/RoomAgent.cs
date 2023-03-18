using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace CardGame
{
	internal class RoomAgent : Singleton<RoomAgent>
	{
		private bool connected;

		private ShsEventMgr eventMgr;

		private Dictionary<long, CardGamePlayer> playerMap = new Dictionary<long, CardGamePlayer>();

		private bool sendingMessagesDisabled;

		private bool busy;

		private Queue<CardGameEvent.ServerMessage> queue = new Queue<CardGameEvent.ServerMessage>();

		private CardGameEvent.ServerMessage currentMessage;

		public static RoomAgent Instance
		{
			get
			{
				return Singleton<RoomAgent>.instance;
			}
		}

		public Queue<CardGameEvent.ServerMessage> QueueContents
		{
			get
			{
				return queue;
			}
		}

		public CardGameEvent.ServerMessage CurrentMessage
		{
			get
			{
				return currentMessage;
			}
		}

		public static int QueueSize
		{
			get
			{
				return Singleton<RoomAgent>.instance.queue.Count;
			}
		}

		public static bool QueueBusy
		{
			get
			{
				return Singleton<RoomAgent>.instance.busy;
			}
		}

		public static IEnumerator Connect(StartInfo start)
		{
			return Singleton<RoomAgent>.instance._Connect(start);
		}

		/////////////// block added for faking cardserver ////////////////////////////////////////////////
		public static void Connect_CSP() {
			Singleton<RoomAgent>.instance._Connect_CSP();
		}
		public void _Connect_CSP() {
			busy = false;
			if (eventMgr == null)
			{
				eventMgr = AppShell.Instance.EventMgr;
			}
			eventMgr.AddListener<CardGameEvent.ServerMessage>(OnMessage);
			eventMgr.AddListener<CardGameEvent.CombatFinished>(OnCardGameResume);
			eventMgr.AddListener<CardGameEvent.ResumeServerQueue>(OnCardGameResume);
			eventMgr.AddListener<CardGameEvent.PokedTimerCompleted>(OnPokeSucceeded);
			eventMgr.AddListener<CardGameEvent.AnimFinished>(OnAnimFinished);
		}

		public void OnAnimFinished(ShsEventMessage evt)
		{
			Resume();
		}
		/////////////////////////////////////////////////////////////////

		private IEnumerator _Connect(StartInfo start)
		{
			if (connected)
			{
				Disconnect();
			}
			if (eventMgr == null)
			{
				eventMgr = AppShell.Instance.EventMgr;
			}
			eventMgr.AddListener<CardGameEvent.ServerMessage>(OnMessage);
			eventMgr.AddListener<CardGameEvent.CombatFinished>(OnCardGameResume);
			eventMgr.AddListener<CardGameEvent.ResumeServerQueue>(OnCardGameResume);
			eventMgr.AddListener<CardGameEvent.PokedTimerCompleted>(OnPokeSucceeded);
			Matchmaker2.Ticket ticket = start.RoomTicket;
			if (ticket == null)
			{
				throw new Exception("No ticket provided");
			}
			string connectError = null;
			bool connectDone = false;
			AppShell.Instance.ServerConnection.ConnectToGame("shs.all", ticket, delegate(bool success, string error)
			{
				if (!success)
				{
					connectError = error;
				}
				connectDone = true;
			});
			while (!connectDone)
			{
				yield return new WaitForEndOfFrame();
			}
			if (!string.IsNullOrEmpty(connectError))
			{
				throw new WebException(connectError);
			}
			busy = true;
			connected = true;
		}

		public static void Disconnect()
		{
			Singleton<RoomAgent>.instance._Disconnect();
		}

		private void _Disconnect()
		{
			if (connected)
			{
				eventMgr.RemoveListener<CardGameEvent.ServerMessage>(OnMessage);
				eventMgr.RemoveListener<CardGameEvent.CombatFinished>(OnCardGameResume);
				eventMgr.RemoveListener<CardGameEvent.ResumeServerQueue>(OnCardGameResume);
				eventMgr.RemoveListener<CardGameEvent.PokedTimerCompleted>(OnPokeSucceeded);
				AppShell.Instance.ServerConnection.DisconnectFromGame();
				//playerMap.Clear();    CSP commented out for cardserver faking
				queue.Clear();
				busy = false;
			}
		}

		public static void SetPlayers(CardGamePlayer[] players)
		{
			Singleton<RoomAgent>.instance._SetPlayers(players);
		}

		private void _SetPlayers(CardGamePlayer[] players)
		{
			playerMap.Clear();
			long n = 0; 
			foreach (CardGamePlayer cardGamePlayer in players)
			{
				//playerMap.Add(cardGamePlayer.Info.PlayerID, cardGamePlayer);
				playerMap.Add(n++, cardGamePlayer);   // CSP modified above line for cardserver faking.
			}
		}

		public static void SendMessage(string message, ArrayList args)
		{
			Singleton<RoomAgent>.instance._SendMessage(message, args);
		}

		private void _SendMessage(string message, ArrayList args)
		{
			CspUtils.DebugLog("Sending: " + message);
			if (sendingMessagesDisabled)
			{
				CspUtils.DebugLog("Trying to send message: " + message + " while sendingMessagesDisabled = true");
			}
			else
			{
				AppShell.Instance.ServerConnection.SendGameSAMessage(message, args);
			}
		}

		public static void Suspend()
		{
			Singleton<RoomAgent>.instance.busy = true;
		}

		private void OnMessage(CardGameEvent.ServerMessage message)
		{
			CspUtils.DebugLog("message.args.Length= " + message.args.Length);
			queue.Enqueue(message);
			
			connected = true; // added by CSP to simulate cardserver
			if (!busy)
			{
				AppShell.Instance.StartCoroutine(HandleNextMessage());
			}
		}

		private void OnCardGameResume(ShsEventMessage evt)
		{
			Resume();
		}

		private void OnPokeSucceeded(CardGameEvent.PokedTimerCompleted evt)
		{
			sendingMessagesDisabled = true;
		}

		public void Resume()
		{
			CspUtils.DebugLog("RoomAgent Resume() called!");
			busy = false;
			AppShell.Instance.StartCoroutine(HandleNextMessage());
		}

		private IEnumerator HandleNextMessage()
		{
			//yield return new WaitForSeconds(1);   // added by CSP to simulate cardserver
			connected = true; // added by CSP to simulate cardserver

			while (!busy && queue.Count > 0)
			{
				sendingMessagesDisabled = false;
				CardGameEvent.ServerMessage message = currentMessage = queue.Dequeue();
				int player = -1;
				string[] args = message.args;
				CspUtils.DebugLog("RoomAgent cmd: " + args[0]);
				if (!connected)
				{
					CspUtils.DebugLog("Card Game Room Agent received message while disconnected: " + message);
					break;
				}
				if (!int.TryParse(args[1], out player))
				{
					CspUtils.DebugLog("Network parse error: player ID");
					break;
				}
				if (args[0] != "PokeAppear")
				{
					AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.EnablePokeButton(false));
				}
				switch (args[0])
				{
				case "Anim":
					playerMap[player].OnAnim(int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4]));
					break;
				case "Block":
					playerMap[player].OnBlock(int.Parse(args[2]), int.Parse(args[3]), bool.Parse(args[4]));
					break;
				case "Buff":
					playerMap[player].OnBuff(int.Parse(args[2]), int.Parse(args[3]), bool.Parse(args[4]), int.Parse(args[5]));
					break;
				case "Damage":
				{
					List<string> cardList2 = new List<string>(args[3].Split(';'));
					if (cardList2[cardList2.Count - 1].Length < 1)
					{
						cardList2.RemoveAt(cardList2.Count - 1);
					}
					List<string> typeList = new List<string>(args[6].Split(';'));
					if (typeList[typeList.Count - 1].Length < 1)
					{
						typeList.RemoveAt(typeList.Count - 1);
					}
					playerMap[player].OnDamage(int.Parse(args[2]), cardList2.ConvertAll<int>(int.Parse), int.Parse(args[4]), bool.Parse(args[5]), typeList, int.Parse(args[7]), int.Parse(args[8]), bool.Parse(args[9]), bool.Parse(args[10]));
					break;
				}
				case "Debug":
					CspUtils.DebugLog("[CARDSERVER]: " + args[2]);
					break;
				case "GameOver":
					playerMap[player].OnGameOver(int.Parse(args[2]), bool.Parse(args[3]));
					break;
				case "Highlight":
					playerMap[player].OnHighlight(int.Parse(args[2]), int.Parse(args[3]), bool.Parse(args[4]));
					break;
				case "Info":
					playerMap[player].OnInfo(int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4]));
					break;
				case "InitCards":
					playerMap[player].OnInitCards(int.Parse(args[2]), int.Parse(args[3]), bool.Parse(args[4]));
					break;
				case "MoveCard":
					busy = true;  // added by CSP for cardserver fake
					playerMap[player].OnMoveCard(int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4]), bool.Parse(args[5]), args[6], int.Parse(args[7]), bool.Parse(args[8]));				
					break;
				case "NewTurn":
					//busy = true;  // added by CSP for cardserver fake
					playerMap[player].OnNewTurn(bool.Parse(args[2]));
					break;
				case "NoRewards":
					AppShell.Instance.EventMgr.Fire(this, new CardGameEvent.CardGameResults(0, 0, 0, null, null));
					break;
				case "PickPending":
					playerMap[player].OnPickPending();
					break;
				case "PickCard":
				{
					//busy = true;  // added by CSP for cardserver fake
					CspUtils.DebugLog("card list str: " + args[2]); // CSP
					List<string> cardList2 = new List<string>(args[2].Split(';'));
					if (cardList2[cardList2.Count - 1].Length < 1)
					{
						cardList2.RemoveAt(cardList2.Count - 1);
					}
					foreach (string card in cardList2) { 
						CspUtils.DebugLog("card list elm: " + card);  // CSP
					}
					PickCardType pickCardType = (PickCardType)int.Parse(args[4]);
					int opposingCardId = (args.Length <= 5) ? (-1) : int.Parse(args[5]);
					int passButtonId = (args.Length > 6) ? int.Parse(args[6]) : 0;
					playerMap[player].OnPickCard(cardList2.ConvertAll<int>(int.Parse), bool.Parse(args[3]), pickCardType, opposingCardId, passButtonId);
					break;
				}
				case "PickFactor":
					playerMap[player].OnPickFactor();
					break;
				case "PickNumber":
					playerMap[player].OnPickNumber(int.Parse(args[2]), int.Parse(args[3]));
					break;
				case "PickYesNo":
					playerMap[player].OnPickYesNo(int.Parse(args[2]), int.Parse(args[3]));
					break;
				case "SetPower":
					playerMap[player].OnSetPower(int.Parse(args[2]), bool.Parse(args[3]));
					break;
				case "PokeAppear":
					AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.EnablePokeButton(true));
					break;
				case "Poked":
					AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.PlayerPoked(int.Parse(args[2]), bool.Parse(args[3])));
					break;
				default:
					CspUtils.DebugLog("Unknown server command: " + message.args[0]);
					break;
				}
			}
			yield return 0;
		}
	}
}
