using CardGame;
using System;
using System.Collections.Generic;

public class PlayCardsCmd : AutomationCmd
{
	private const int FRAMELIMIT = 100;

	private const int GAMETIMEOUT = 600;

	private AutomationCardGame AutoCardGame;

	private CardGamePlayer player;

	private CardGamePlayer player2;

	private int logicType;

	private int frames;

	private bool myturn;

	private bool isAIMoveLogged;

	private string logMessage;

	private WaitForInit transition;

	public PlayCardsCmd(string cmdline)
		: base(cmdline)
	{
		myturn = false;
		frames = 0;
		logicType = 1;
		AutomationManager.Instance.nCardGame++;
	}

	public PlayCardsCmd(string cmdline, string logic)
		: base(cmdline)
	{
		myturn = false;
		frames = 0;
		logicType = Convert.ToInt32(logic);
		AutomationManager.Instance.nCardGame++;
	}

	public override bool isReady()
	{
		bool flag = base.isReady();
		logMessage = string.Empty;
		base.StartTime = DateTime.Now;
		try
		{
			player = CardGameController.Instance.players[0];
			player2 = CardGameController.Instance.players[1];
			AutoCardGame = new AutomationCardGame(player);
			if (flag)
			{
				flag = (AutomationManager.Instance.activeController == GameController.ControllerType.CardGame);
			}
			if (flag)
			{
				return flag;
			}
			base.ErrorMsg = "Card Game has failed to load";
			base.ErrorCode = "P001";
			AutomationManager.Instance.LogCardInfoToFile(base.ErrorMsg);
			return flag;
		}
		catch
		{
			base.ErrorMsg = " Card Game has failed to load due to connection issues";
			base.ErrorCode = "PC001";
			AutomationManager.Instance.LogCardInfoToFile(base.ErrorMsg);
			return flag;
		}
	}

	public override bool isCompleted()
	{
		frames++;
		List<int> cards = ConvertToList(player.Hand);
		bool flag = true;
		if (DateTime.Now - base.StartTime < TimeSpan.FromSeconds(600.0))
		{
			if (flag)
			{
				if (frames > 100)
				{
					myturn = true;
					frames = 0;
				}
				if (myturn)
				{
					logMessage = string.Empty;
					AutoCardGame.StartPlayerAI(cards, true, PickCardType.Attack, logicType);
					myturn = false;
					int num = player.Discard.Count - 1;
					if (isAIMoveLogged)
					{
						isAIMoveLogged = false;
						if (num >= 0)
						{
							string text = logMessage;
							logMessage = text + CardGameController.Instance.powerLevel + "," + GetHand(1) + "," + player.Discard[num].ServerID + "/" + player.Discard[num].Type + "," + GetKeepers(1) + ",";
						}
					}
				}
				else
				{
					int num2 = player2.Discard.Count - 1;
					if (!isAIMoveLogged)
					{
						isAIMoveLogged = true;
						if (num2 >= 0 && !logMessage.Equals(string.Empty))
						{
							string text = logMessage;
							logMessage = text + GetHand(2) + "," + player2.Discard[num2].ServerID + "/" + player2.Discard[num2].Type + "," + GetKeepers(2);
						}
						if (!logMessage.Trim().Equals(string.Empty) && !logMessage.Equals(string.Empty))
						{
							AutomationManager.Instance.LogCardInfoToFile(logMessage);
						}
					}
				}
				if (CardGameController.Instance.winner == 0)
				{
					AutomationManager.Instance.matchWon++;
					AutomationManager.Instance.LogCardInfoToFile(",Win,,,Lose");
					flag = true;
					base.ErrorMsg = "Success";
					base.ErrorCode = "OK";
				}
				else if (CardGameController.Instance.winner == 1)
				{
					AutomationManager.Instance.matchLost++;
					AutomationManager.Instance.LogCardInfoToFile(",Lose,,,Win");
					flag = true;
					base.ErrorMsg = "Success";
					base.ErrorCode = "OK";
				}
				else
				{
					flag = false;
				}
			}
		}
		else
		{
			AutomationManager.Instance.matchIncomplete++;
			AutomationManager.Instance.errCardGame++;
			base.ErrorCode = "C001";
			base.ErrorMsg = "Game has failed to complete";
			AutomationManager.Instance.LogCardInfoToFile(base.ErrorMsg);
			flag = false;
			CspUtils.DebugLog("TIME->" + (DateTime.Now - base.StartTime) + ":" + TimeSpan.FromSeconds(600.0) + "isTrue:" + (DateTime.Now - base.StartTime > TimeSpan.FromSeconds(600.0)));
			AppShell.Instance.SharedHashTable["CardGameLevel"] = null;
			AppShell.Instance.SharedHashTable["CardGameTicket"] = null;
			AppShell.Instance.QueueLocationInfo();
		}
		return flag;
	}

	public string GetHand(int user)
	{
		string str = "[";
		switch (user)
		{
		case 1:
			foreach (BattleCard item in player.Hand)
			{
				str = str + item.Type + ";";
			}
			break;
		case 2:
			foreach (BattleCard item2 in player2.Hand)
			{
				str = str + item2.Type + ";";
			}
			break;
		}
		return str + "]";
	}

	public string GetKeepers(int user)
	{
		string str = "[";
		switch (user)
		{
		case 1:
			foreach (BattleCard keeper in player.Keepers)
			{
				str = str + keeper.Type + ";";
			}
			break;
		case 2:
			foreach (BattleCard keeper2 in player2.Keepers)
			{
				str = str + keeper2.Type + ";";
			}
			break;
		}
		return str + "]";
	}

	public List<int> ConvertToList(CardPile CardPile)
	{
		List<int> list = new List<int>();
		string arg = string.Empty;
		foreach (BattleCard item in CardPile)
		{
			list.Add(item.ServerID);
			arg = arg + "," + item.ServerID;
		}
		return list;
	}
}
