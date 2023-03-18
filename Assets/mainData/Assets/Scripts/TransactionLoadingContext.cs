using System.Collections;
using System.Collections.Generic;

public class TransactionLoadingContext
{
	public struct TransactionContext
	{
		public string transactionId;

		public float timeOut;

		public TransactionMonitor.OnCompleteDelegate onComplete;

		public List<string> steps;

		public object extraData;

		public static TransactionContext EmptyContext
		{
			get
			{
				return new TransactionContext(null, string.Empty);
			}
		}

		public TransactionContext(List<string> steps, string transactionId)
		{
			this.steps = steps;
			this.transactionId = transactionId;
			onComplete = null;
			timeOut = float.MaxValue;
			extraData = null;
		}
	}

	private static Hashtable contextLookup;

	private TransactionMonitor transaction;

	public TransactionMonitor Transaction
	{
		get
		{
			return transaction;
		}
	}

	private static void CreateContextLookup()
	{
		if (contextLookup == null)
		{
			List<string> list = null;
			contextLookup = new Hashtable();
			contextLookup[GameController.ControllerType.FrontEnd] = new Hashtable();
			contextLookup[GameController.ControllerType.Fallback] = new Hashtable();
			contextLookup[GameController.ControllerType.SocialSpace] = new Hashtable();
			contextLookup[GameController.ControllerType.HeadQuarters] = new Hashtable();
			contextLookup[GameController.ControllerType.CardGame] = new Hashtable();
			contextLookup[GameController.ControllerType.DeckBuilder] = new Hashtable();
			contextLookup[GameController.ControllerType.Brawler] = new Hashtable();
			contextLookup[GameController.ControllerType.RailsHq] = new Hashtable();
			contextLookup[GameController.ControllerType.RailsGameWorld] = new Hashtable();
			contextLookup[GameController.ControllerType.RailsBrawler] = new Hashtable();
			contextLookup[GameController.ControllerType.ArcadeShell] = new Hashtable();
			list = new List<string>(new string[1]
			{
				"init"
			});
			TransactionContext transactionContext = new TransactionContext(list, "social_space_start_transaction");
			((Hashtable)contextLookup[GameController.ControllerType.FrontEnd])[GameController.ControllerType.SocialSpace] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.SocialSpace] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.SocialSpace] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.SocialSpace] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.SocialSpace] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.SocialSpace] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.Fallback])[GameController.ControllerType.SocialSpace] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.RailsGameWorld])[GameController.ControllerType.SocialSpace] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.ArcadeShell])[GameController.ControllerType.SocialSpace] = transactionContext;
			list = new List<string>(new string[1]
			{
				"rails_init"
			});
			transactionContext = new TransactionContext(list, "rails_game_world_transaction");
			((Hashtable)contextLookup[GameController.ControllerType.FrontEnd])[GameController.ControllerType.RailsGameWorld] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.RailsGameWorld])[GameController.ControllerType.RailsGameWorld] = transactionContext;
			list = new List<string>(new string[9]
			{
				"pickupDataLoaded",
				"dropTableDataLoaded",
				"characterCombinationDataLoaded",
				"enemyBarDataLoaded",
				"brawlerCharacterDataLoaded",
				"brawlerAssetBundleLoaded",
				"brawlerStartTransaction",
				"selectCharacterTransaction",
				"brawlerOrthographicHudDataLoaded"
			});
			transactionContext = new TransactionContext(list, "brawler_start_transaction");
			((Hashtable)contextLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.Brawler] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.Brawler] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.Brawler] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.Brawler] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.Brawler] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.Fallback])[GameController.ControllerType.Brawler] = transactionContext;
			list = new List<string>(new string[1]
			{
				"controllerReady"
			});
			transactionContext = new TransactionContext(list, "hq_start_transaction");
			((Hashtable)contextLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.HeadQuarters] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.HeadQuarters] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.HeadQuarters] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.HeadQuarters] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.HeadQuarters] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.Fallback])[GameController.ControllerType.HeadQuarters] = transactionContext;
			list = new List<string>(new string[8]
			{
				"ticket",
				"playerdata",
				"bundle",
				"textureBundle",
				"cardData",
				"createPlayers",
				"loadArena",
				"connectRoom"
			});
			transactionContext = new TransactionContext(list, "card_start_transaction");
			((Hashtable)contextLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.CardGame] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.CardGame] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.CardGame] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.CardGame] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.CardGame] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.Fallback])[GameController.ControllerType.CardGame] = transactionContext;
			list = new List<string>(new string[8]
			{
				"deckBuilderBundleLoaded",
				"toolboxBundleLoaded",
				"instantiation",
				"recipeloaded",
				"textureBundle",
				"cardDataLoaded",
				"fetchCardCollection",
				"addCounters"
			});
			transactionContext = new TransactionContext(list, "deck_builder_transaction");
			((Hashtable)contextLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.DeckBuilder] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.DeckBuilder] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.DeckBuilder] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.DeckBuilder] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.DeckBuilder] = transactionContext;
			((Hashtable)contextLookup[GameController.ControllerType.Fallback])[GameController.ControllerType.DeckBuilder] = transactionContext;
			list = new List<string>(new string[2]
			{
				"get_url",
				"init"
			});
			transactionContext = new TransactionContext(list, "arcade_shell_transaction");
			((Hashtable)contextLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.ArcadeShell] = transactionContext;
		}
	}

	private static void OnTransactionComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
	}

	public void SetTransactionContext(GameController.ControllerType from, GameController.ControllerType to)
	{
		CreateContextLookup();
		if (contextLookup.ContainsKey(from))
		{
			Hashtable hashtable = (Hashtable)contextLookup[from];
			if (hashtable.Contains(to))
			{
				TransactionContext transactionContext = (TransactionContext)hashtable[to];
				List<string> steps = transactionContext.steps;
				transaction = TransactionMonitor.CreateTransactionMonitor(transactionContext.transactionId, (transactionContext.onComplete == null) ? new TransactionMonitor.OnCompleteDelegate(OnTransactionComplete) : transactionContext.onComplete, transactionContext.timeOut, transactionContext.extraData);
				foreach (string item in steps)
				{
					transaction.AddStep(item, TransactionMonitor.DumpTransactionStatus);
				}
			}
			else
			{
				CspUtils.DebugLog("Trying to set Transaction Context when the 'to' controller type isn't specified. The type passed in was: " + to);
			}
		}
		else
		{
			CspUtils.DebugLog("Trying to set Transaction Context when the 'from' controller type isn't specified. The type passed in was: " + from);
		}
	}

	public void SetTransactionContext(TransactionContext context)
	{
		if (transaction == null)
		{
			List<string> steps = context.steps;
			transaction = TransactionMonitor.CreateTransactionMonitor(context.transactionId, (context.onComplete == null) ? new TransactionMonitor.OnCompleteDelegate(OnTransactionComplete) : context.onComplete, context.timeOut, context.extraData);
			foreach (string item in steps)
			{
				transaction.AddStep(item, TransactionMonitor.DumpTransactionStatus);
			}
		}
	}

	public void RemoveChildTransaction(string transactionId)
	{
		TransactionMonitor transactionMonitor = null;
		foreach (TransactionMonitor transaction2 in transaction.Transactions)
		{
			if (transaction2.Id == transactionId)
			{
				transactionMonitor = transaction2;
				break;
			}
		}
		if (transactionMonitor != null)
		{
			transaction.RemoveChild(transactionMonitor);
		}
	}

	public void AddChildTransaction(TransactionMonitor monitorToAdd)
	{
		if (transaction != null)
		{
			transaction.AddChild(monitorToAdd);
		}
	}

	public void CompleteChildTransactionStep(string childTransactionId, string childStep)
	{
		TransactionMonitor transactionMonitor = null;
		foreach (TransactionMonitor transaction2 in transaction.Transactions)
		{
			if (transaction2.Id == childTransactionId)
			{
				transactionMonitor = transaction2;
				break;
			}
		}
		if (transactionMonitor != null && transactionMonitor.HasStep(childStep))
		{
			transactionMonitor.CompleteStep(childStep);
		}
	}
}
