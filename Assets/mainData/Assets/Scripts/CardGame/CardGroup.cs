using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame
{
	public class CardGroup : List<BattleCard>
	{
		public static int R3DemoHulkIronman = 176;

		public static int R3DemoWolverineStorm = 177;

		public static string R4DemoHulkRecipe = "ST156:7;ST048:7;ST143:5;ST026:4;ST150:4;ST187:1;ST186:2;ST031:3;ST222:2;ST155:1;ST194:1;ST173:1;ST062:1;ST064:1;";

		public static string R4DemoWolverineRecipe = "ST157:6;ST008:5;ST018:3;ST164:6;ST025:3;ST196:3;ST280:6;ST233:1;ST197:1;ST165:3;ST051:1;ST066:1;ST153:1;";

		public string DeckRecipe;

		public int DeckId = -1;

		public string DeckName;

		private string DefaultRecipe = R4DemoHulkRecipe;

		private Dictionary<string, BattleCard> deckTemplate = new Dictionary<string, BattleCard>();

		protected AssetBundle cardTextureBundle;

		private static ShsWebService webServiceComponent
		{
			get
			{
				return Utils.GetComponent<ShsWebService>(AppShell.Instance.gameObject);
			}
		}

		public IEnumerator LoadRecipe(string recipe, TransactionMonitor monitor, bool fullSize)
		{
			CspUtils.DebugLog("Loading recipe: " + recipe);
			DeckRecipe = ((!string.IsNullOrEmpty(recipe)) ? recipe : DefaultRecipe);
			return LoadRecipe(CardManager.ParseRecipe(recipe), monitor, fullSize);
		}

		public IEnumerator LoadRecipe(Dictionary<string, int> recipe, TransactionMonitor monitor, bool fullSize)
		{
			deckTemplate.Clear();
			deckTemplate = CardManager.ParseCardDataSet(recipe.Keys);
			if (monitor != null)
			{
				foreach (KeyValuePair<string, int> kard in recipe)
				{
					try
					{
						monitor.AddStep(kard.Key);
					}
					catch
					{
					}
					finally
					{
					}
				}
			}
			float progress = 0f;
			float progressQuanta = 1f / (float)recipe.Keys.Count;
			foreach (KeyValuePair<string, int> card in recipe)
			{
				try
				{
					BattleCard newCard;
					if (deckTemplate.TryGetValue(card.Key, out newCard))
					{
						LoadCardTextures(newCard, progress, fullSize);
						if (monitor != null)
						{
							monitor.CompleteStep(card.Key);
						}
						for (int numLeft = card.Value; numLeft > 0; numLeft--)
						{
							BattleCard clone = new BattleCard(newCard);
							clone.Instance = numLeft;
							Add(clone);
						}
						yield return new WaitForEndOfFrame();
					}
				}
				finally
				{
					progress += progressQuanta;
				}
			}
			Utils.ListShuffle(this);  // added by CSP
			yield return new WaitForEndOfFrame();
		}

		private void LoadCardTextures(BattleCard card, float progress, bool fullSize)
		{
			Texture2D texture2D = (!fullSize) ? CardManager.LoadCardTexture(card.Type + "_mini", card.BundleName) : CardManager.LoadCardTexture(card.Type, card.BundleName);
			if (texture2D == null)
			{
				CspUtils.DebugLog("No texture found in asset bundle for " + card.Type);
				return;
			}
			AppShell.Instance.EventMgr.Fire(null, new LoadingProgressMessage(LoadingProgressMessage.LoadingState.InProgress, progress, card.Type + "_lg.png"));
			card.FullTexture = texture2D;
			card.MiniTexture = texture2D;
		}

		public static int CountCards(string recipe)
		{
			int num = 0;
			Dictionary<string, int> dictionary = CardManager.ParseRecipe(recipe);
			foreach (int value in dictionary.Values)
			{
				num += value;
			}
			return num;
		}
	}
}
