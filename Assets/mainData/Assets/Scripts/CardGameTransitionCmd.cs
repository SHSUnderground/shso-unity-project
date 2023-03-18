using CardGame;
using System;

public class CardGameTransitionCmd : AutomationCmd
{
	private int level;

	private int quest;

	private string heroName;

	private string deck;

	private string botDeck;

	private string aiDeck;

	private bool setStartInfo;

	private string message;

	private GUIManager currState;

	public CardGameTransitionCmd(string cmdline)
		: base(cmdline)
	{
		heroName = "ms_marvel";
		quest = 82;
		deck = string.Empty;
		level = 1;
		currState = new GUIManager();
		setStartInfo = false;
		AutomationManager.Instance.nCardGame++;
	}

	public CardGameTransitionCmd(string cmdline, string hero, string level, string quest)
		: base(cmdline)
	{
		heroName = hero;
		deck = string.Empty;
		this.level = Convert.ToInt32(level);
		this.quest = Convert.ToInt32(quest);
		currState = new GUIManager();
		setStartInfo = false;
		AutomationManager.Instance.nCardGame++;
	}

	public CardGameTransitionCmd(string cmdline, string hero, string level, string botDeckReceipt, string aiDeckReceipt, string quest)
		: base(cmdline)
	{
		heroName = hero;
		deck = botDeckReceipt;
		this.quest = Convert.ToInt32(quest);
		this.level = Convert.ToInt32(level);
		botDeck = botDeckReceipt;
		aiDeck = aiDeckReceipt;
		currState = new GUIManager();
		setStartInfo = true;
		AutomationManager.Instance.nCardGame++;
	}

	public override bool isReady()
	{
		bool flag = base.isReady();
		if (!PrepareCardGame(setStartInfo, level, heroName, botDeck, aiDeck))
		{
			message = "Card Game Failed To Set Start Information";
		}
		else if (flag)
		{
			flag = (currState.CurrentState != GUIManager.ModalStateEnum.Transition);
			CspUtils.DebugLog("Current State: " + currState.CurrentState);
		}
		else
		{
			message = "CardGame seems to appears in a Transition State";
		}
		base.ErrorMsg = message;
		return flag;
	}

	public override bool execute()
	{
		try
		{
			AppShell.Instance.Matchmaker2.SoloCardGame(level, heroName, deck, quest);
			AppShell.Instance.Transition(GameController.ControllerType.CardGame);
		}
		catch (Exception ex)
		{
			AutomationManager.Instance.errCardGame++;
			base.ErrorCode = "E001";
			base.ErrorMsg += ex.Message;
			CspUtils.DebugLog(base.ErrorMsg);
		}
		return base.execute();
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		if (flag)
		{
			flag = (AutomationManager.Instance.activeController == GameController.ControllerType.CardGame);
			CspUtils.DebugLog("Active Controller: " + AutomationManager.Instance.activeController);
			if (!flag)
			{
				base.ErrorMsg = "Active Controller is not CardGame";
			}
		}
		else
		{
			AutomationManager.Instance.errCardGame++;
			base.ErrorCode = "C001";
			base.ErrorMsg += "CardGameTransitionCmd Timed Out";
		}
		return flag;
	}

	private bool PrepareCardGame(bool setStartInfo, int level, string botHero, string botDeck, string gameAIDeck)
	{
		bool result = true;
		StartInfo startInfo = new StartInfo();
		try
		{
			if (setStartInfo)
			{
				startInfo.Players[0].DeckRecipe = botDeck;
				startInfo.Players[0].Hero = botHero;
				startInfo.Players[1].DeckRecipe = gameAIDeck;
				startInfo.QuestNodeID = level;
				CspUtils.DebugLog("Bot Deck: " + startInfo.Players[0].DeckRecipe);
				CspUtils.DebugLog("AI Deck: " + startInfo.Players[1].DeckRecipe);
				AutomationManager.Instance.LogCardInfoToFile(string.Empty);
				AutomationManager.Instance.LogCardInfoToFile("Bot Deck," + startInfo.Players[0].DeckRecipe);
				AutomationManager.Instance.LogCardInfoToFile("AI Deck," + startInfo.Players[1].DeckRecipe);
				AutomationManager.Instance.LogCardInfoToFile(string.Empty);
				AutomationManager.Instance.LogCardInfoToFile("PowerLevel,Bot Hand, Bot Last Discarded,Bot Keepers,AI Hand,AI Last Discarded, AI Keepers");
			}
			startInfo.QuestKeeper = "cyclops";
			startInfo.ArenaName = "sewers";
			startInfo.ArenaScenario = 1;
			startInfo.QuestConditions = string.Empty;
			startInfo.TicketsAwarded = 3;
			startInfo.SilverAwarded = 60;
			startInfo.XPAwarded = 90;
			AppShell.Instance.SharedHashTable["CardGameLevel"] = startInfo;
			AppShell.Instance.SharedHashTable["CardGameTicket"] = null;
			AppShell.Instance.QueueLocationInfo();
			return result;
		}
		catch (Exception ex)
		{
			message += ex.Message;
			result = false;
			CspUtils.DebugLog("FailureC001" + message);
			return result;
		}
	}
}
