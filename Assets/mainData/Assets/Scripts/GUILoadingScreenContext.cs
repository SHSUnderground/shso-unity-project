using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUILoadingScreenContext
{
	public struct LocationInfo
	{
		public string locationName;

		public static LocationInfo NoInfo
		{
			get
			{
				return new LocationInfo(string.Empty);
			}
		}

		public LocationInfo(string locationName)
		{
			this.locationName = locationName;
		}
	}

	public struct LoadingContext
	{
		public string category;

		public string backgroundTextureSource;

		public string additionalTextureSource;

		public Vector2 additionalTextureOffset;

		public Vector2 additionalTextureSize;

		public CustomContextSetup customSetup;

		public SmartTipUsageInstance smartTipUsageInstance;

		public static LoadingContext EmptyContext
		{
			get
			{
				return new LoadingContext(string.Empty, string.Empty);
			}
		}

		public LoadingContext(string backgroundTextureSource, string category)
		{
			this.category = category;
			this.backgroundTextureSource = backgroundTextureSource;
			customSetup = null;
			additionalTextureSource = string.Empty;
			additionalTextureSize = Vector2.zero;
			additionalTextureOffset = Vector2.zero;
			smartTipUsageInstance = SmartTipsManager.DefaultSmartTipUsageInstance;
		}
	}

	public delegate void CustomContextSetup(SHSWaitWindow window);

	private const string loadingScreenBundle = "loading_bundle";

	public const string UnityLoadingScreenSrc = "GUI/loading/mshs_loading_background_01";

	public const string UnityPreLoadingScreenSrc = "GUI/loading/preloading_blue_backdrop";

	public const string TASGAZLogoSrc = "GUI/loading/loader_gaz_tas_logos";

	public const string StandardLoadingScreenSrc = "GUI/loading/Loading";

	public const string HeroCityLoadingScreenSrc = "GUI/loading/Loading";

	public const string HeadquartersLoadingScreenSrc = "GUI/loading/Loading";

	public const string BrawlerLoadingScreenSrc = "GUI/loading/Loading";

	private static Hashtable contextLookup;

	private static Hashtable locationLookup;

	private static List<string> validLoadingScreens;

	private static bool loadingScreenBundleRequested;

	private static bool loadingScreenAssetsAvailable;

	private CustomContextSetup customSetup;

	private string tipText;

	private string whoseTipText;

	private string backgroundTextureSource;

	private string additionalTextureSource;

	private Vector2 additionalTextureOffset;

	private Vector2 additionalTextureSize;

	private string tipTextureSource;

	private string locationName;

	public static bool LoadingScreenAssetsAvailable
	{
		get
		{
			return loadingScreenAssetsAvailable;
		}
	}

	public CustomContextSetup CustomSetup
	{
		get
		{
			return customSetup;
		}
	}

	public string TipText
	{
		get
		{
			return tipText;
		}
	}

	public string WhoseTipText
	{
		get
		{
			return whoseTipText;
		}
	}

	public string CompleteWhoseTipText
	{
		get
		{
			return whoseTipText + " Tip:";
		}
	}

	public string BackgroundTextureSource
	{
		get
		{
			bool flag = backgroundTextureSource == "GUI/loading/mshs_loading_background_01";
			bool flag2 = backgroundTextureSource == string.Empty || !loadingScreenAssetsAvailable || !validLoadingScreens.Contains(backgroundTextureSource);
			if (!flag && flag2)
			{
				backgroundTextureSource = "GUI/loading/mshs_loading_background_01";
			}
			return backgroundTextureSource;
		}
	}

	public string AdditionalTextureSource
	{
		get
		{
			return additionalTextureSource;
		}
	}

	public Vector2 AdditionalTextureOffset
	{
		get
		{
			return additionalTextureOffset;
		}
	}

	public Vector2 AdditionalTextureSize
	{
		get
		{
			return additionalTextureSize;
		}
		set
		{
			additionalTextureSize = value;
		}
	}

	public string TipTextureSource
	{
		get
		{
			return tipTextureSource;
		}
	}

	public string LocationName
	{
		get
		{
			return locationName;
		}
	}

	public GUILoadingScreenContext()
	{
	}

	public GUILoadingScreenContext(GUILoadingScreenContext otherContext)
	{
		tipText = otherContext.TipText;
		whoseTipText = otherContext.WhoseTipText;
		backgroundTextureSource = otherContext.BackgroundTextureSource;
		tipTextureSource = otherContext.TipTextureSource;
		customSetup = otherContext.CustomSetup;
		additionalTextureSource = otherContext.AdditionalTextureSource;
		additionalTextureOffset = otherContext.AdditionalTextureOffset;
		additionalTextureSize = otherContext.AdditionalTextureSize;
	}

	private static void CreateContextLookup()
	{
		if (contextLookup == null)
		{
			contextLookup = new Hashtable();
			contextLookup[GameController.ControllerType.None] = new Hashtable();
			contextLookup[GameController.ControllerType.FrontEnd] = new Hashtable();
			contextLookup[GameController.ControllerType.SocialSpace] = new Hashtable();
			contextLookup[GameController.ControllerType.Brawler] = new Hashtable();
			contextLookup[GameController.ControllerType.HeadQuarters] = new Hashtable();
			contextLookup[GameController.ControllerType.CardGame] = new Hashtable();
			contextLookup[GameController.ControllerType.RailsGameWorld] = new Hashtable();
			contextLookup[GameController.ControllerType.DeckBuilder] = new Hashtable();
			contextLookup[GameController.ControllerType.ArcadeShell] = new Hashtable();
			LoadingContext loadingContext = new LoadingContext("GUI/loading/mshs_loading_background_01", string.Empty);
			loadingContext.additionalTextureSource = "GUI/loading/welcome/mshs_welcome_screen_game_logo";
			loadingContext.additionalTextureOffset = new Vector2(0f, -165f);
			loadingContext.additionalTextureSize = new Vector2(231f, 209f);
			((Hashtable)contextLookup[GameController.ControllerType.None])[GameController.ControllerType.None] = loadingContext;
			((Hashtable)contextLookup[GameController.ControllerType.RailsGameWorld])[GameController.ControllerType.RailsGameWorld] = new LoadingContext("GUI/loading/Loading", "GameWorld");
			((Hashtable)contextLookup[GameController.ControllerType.RailsGameWorld])[GameController.ControllerType.SocialSpace] = new LoadingContext("GUI/loading/Loading", "GameWorld");
			((Hashtable)contextLookup[GameController.ControllerType.FrontEnd])[GameController.ControllerType.SocialSpace] = new LoadingContext("GUI/loading/Loading", "GameWorld");
			((Hashtable)contextLookup[GameController.ControllerType.FrontEnd])[GameController.ControllerType.Brawler] = new LoadingContext("GUI/loading/Loading", string.Empty);
			((Hashtable)contextLookup[GameController.ControllerType.FrontEnd])[GameController.ControllerType.HeadQuarters] = new LoadingContext("GUI/loading/Loading", "HQ");
			((Hashtable)contextLookup[GameController.ControllerType.FrontEnd])[GameController.ControllerType.CardGame] = new LoadingContext("GUI/loading/Loading", "CardGame");
			((Hashtable)contextLookup[GameController.ControllerType.FrontEnd])[GameController.ControllerType.FrontEnd] = new LoadingContext("GUI/loading/Loading", string.Empty);
			((Hashtable)contextLookup[GameController.ControllerType.FrontEnd])[GameController.ControllerType.DeckBuilder] = new LoadingContext("GUI/loading/Loading", "CardGame");
			((Hashtable)contextLookup[GameController.ControllerType.FrontEnd])[GameController.ControllerType.ArcadeShell] = new LoadingContext("GUI/loading/Loading", "Arcade");
			((Hashtable)contextLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.SocialSpace] = new LoadingContext("GUI/loading/Loading", "GameWorld");
			((Hashtable)contextLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.Brawler] = new LoadingContext("GUI/loading/Loading", "Missions");
			((Hashtable)contextLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.HeadQuarters] = new LoadingContext("GUI/loading/Loading", "HQ");
			((Hashtable)contextLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.CardGame] = new LoadingContext("GUI/loading/Loading", "CardGame");
			((Hashtable)contextLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.FrontEnd] = new LoadingContext("GUI/loading/Loading", string.Empty);
			((Hashtable)contextLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.RailsGameWorld] = new LoadingContext("GUI/loading/Loading", string.Empty);
			((Hashtable)contextLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.DeckBuilder] = new LoadingContext("GUI/loading/Loading", "CardGame");
			((Hashtable)contextLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.ArcadeShell] = new LoadingContext("GUI/loading/Loading", "Arcade");
			((Hashtable)contextLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.SocialSpace] = new LoadingContext("GUI/loading/Loading", "GameWorld");
			((Hashtable)contextLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.Brawler] = new LoadingContext("GUI/loading/Loading", "Missions");
			((Hashtable)contextLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.HeadQuarters] = new LoadingContext("GUI/loading/Loading", "HQ");
			((Hashtable)contextLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.CardGame] = new LoadingContext("GUI/loading/Loading", "CardGame");
			((Hashtable)contextLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.FrontEnd] = new LoadingContext("GUI/loading/Loading", string.Empty);
			((Hashtable)contextLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.DeckBuilder] = new LoadingContext("GUI/loading/Loading", "CardGame");
			((Hashtable)contextLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.ArcadeShell] = new LoadingContext("GUI/loading/Loading", "Arcade");
			((Hashtable)contextLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.SocialSpace] = new LoadingContext("GUI/loading/Loading", "GameWorld");
			((Hashtable)contextLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.Brawler] = new LoadingContext("GUI/loading/Loading", "Missions");
			((Hashtable)contextLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.HeadQuarters] = new LoadingContext("GUI/loading/Loading", "HQ");
			((Hashtable)contextLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.CardGame] = new LoadingContext("GUI/loading/Loading", "CardGame");
			((Hashtable)contextLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.FrontEnd] = new LoadingContext("GUI/loading/Loading", string.Empty);
			((Hashtable)contextLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.DeckBuilder] = new LoadingContext("GUI/loading/Loading", "CardGame");
			((Hashtable)contextLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.ArcadeShell] = new LoadingContext("GUI/loading/Loading", "Arcade");
			((Hashtable)contextLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.SocialSpace] = new LoadingContext("GUI/loading/Loading", "GameWorld");
			((Hashtable)contextLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.Brawler] = new LoadingContext("GUI/loading/Loading", "Missions");
			((Hashtable)contextLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.HeadQuarters] = new LoadingContext("GUI/loading/Loading", "HQ");
			((Hashtable)contextLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.CardGame] = new LoadingContext("GUI/loading/Loading", "CardGame");
			((Hashtable)contextLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.FrontEnd] = new LoadingContext("GUI/loading/Loading", "CardGame");
			((Hashtable)contextLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.ArcadeShell] = new LoadingContext("GUI/loading/Loading", "Arcade");
			((Hashtable)contextLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.SocialSpace] = new LoadingContext("GUI/loading/Loading", "GameWorld");
			((Hashtable)contextLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.Brawler] = new LoadingContext("GUI/loading/Loading", "Missions");
			((Hashtable)contextLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.HeadQuarters] = new LoadingContext("GUI/loading/Loading", "HQ");
			((Hashtable)contextLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.CardGame] = new LoadingContext("GUI/loading/Loading", "CardGame");
			((Hashtable)contextLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.FrontEnd] = new LoadingContext("GUI/loading/Loading", string.Empty);
			((Hashtable)contextLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.DeckBuilder] = new LoadingContext("GUI/loading/Loading", "CardGame");
			((Hashtable)contextLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.ArcadeShell] = new LoadingContext("GUI/loading/Loading", "Arcade");
			((Hashtable)contextLookup[GameController.ControllerType.ArcadeShell])[GameController.ControllerType.SocialSpace] = new LoadingContext("GUI/loading/Loading", "GameWorld");
			((Hashtable)contextLookup[GameController.ControllerType.ArcadeShell])[GameController.ControllerType.Brawler] = new LoadingContext("GUI/loading/Loading", "Missions");
			((Hashtable)contextLookup[GameController.ControllerType.ArcadeShell])[GameController.ControllerType.HeadQuarters] = new LoadingContext("GUI/loading/Loading", "HQ");
			((Hashtable)contextLookup[GameController.ControllerType.ArcadeShell])[GameController.ControllerType.CardGame] = new LoadingContext("GUI/loading/Loading", "CardGame");
			((Hashtable)contextLookup[GameController.ControllerType.ArcadeShell])[GameController.ControllerType.FrontEnd] = new LoadingContext("GUI/loading/Loading", string.Empty);
			((Hashtable)contextLookup[GameController.ControllerType.ArcadeShell])[GameController.ControllerType.RailsGameWorld] = new LoadingContext("GUI/loading/Loading", string.Empty);
			((Hashtable)contextLookup[GameController.ControllerType.ArcadeShell])[GameController.ControllerType.DeckBuilder] = new LoadingContext("GUI/loading/Loading", "CardGame");
			((Hashtable)contextLookup[GameController.ControllerType.ArcadeShell])[GameController.ControllerType.ArcadeShell] = new LoadingContext("GUI/loading/Loading", "Arcade");
		}
	}

	private static void CreateLocationLookup()
	{
		if (locationLookup == null)
		{
			locationLookup = new Hashtable();
			locationLookup[GameController.ControllerType.None] = new Hashtable();
			locationLookup[GameController.ControllerType.FrontEnd] = new Hashtable();
			locationLookup[GameController.ControllerType.SocialSpace] = new Hashtable();
			locationLookup[GameController.ControllerType.Brawler] = new Hashtable();
			locationLookup[GameController.ControllerType.HeadQuarters] = new Hashtable();
			locationLookup[GameController.ControllerType.CardGame] = new Hashtable();
			locationLookup[GameController.ControllerType.RailsGameWorld] = new Hashtable();
			locationLookup[GameController.ControllerType.DeckBuilder] = new Hashtable();
			((Hashtable)locationLookup[GameController.ControllerType.None])[GameController.ControllerType.None] = new LocationInfo(string.Empty);
			((Hashtable)locationLookup[GameController.ControllerType.RailsGameWorld])[GameController.ControllerType.SocialSpace] = new LocationInfo("#LOAD_TITLE_GAMEWORLD");
			((Hashtable)locationLookup[GameController.ControllerType.FrontEnd])[GameController.ControllerType.SocialSpace] = new LocationInfo("#LOAD_TITLE_GAMEWORLD");
			((Hashtable)locationLookup[GameController.ControllerType.FrontEnd])[GameController.ControllerType.Brawler] = new LocationInfo("#LOAD_TITLE_MISSIONS");
			((Hashtable)locationLookup[GameController.ControllerType.FrontEnd])[GameController.ControllerType.HeadQuarters] = new LocationInfo("#LOAD_TITLE_HEADQUARTERS");
			((Hashtable)locationLookup[GameController.ControllerType.FrontEnd])[GameController.ControllerType.CardGame] = new LocationInfo("#LOAD_TITLE_CARDGAME");
			((Hashtable)locationLookup[GameController.ControllerType.FrontEnd])[GameController.ControllerType.FrontEnd] = new LocationInfo("#LOAD_TITLE_LOGIN");
			((Hashtable)locationLookup[GameController.ControllerType.FrontEnd])[GameController.ControllerType.DeckBuilder] = new LocationInfo("#LOAD_TITLE_DECKBUILDER");
			((Hashtable)locationLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.SocialSpace] = new LocationInfo("#LOAD_TITLE_GAMEWORLD");
			((Hashtable)locationLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.Brawler] = new LocationInfo("#LOAD_TITLE_MISSIONS");
			((Hashtable)locationLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.HeadQuarters] = new LocationInfo("#LOAD_TITLE_HEADQUARTERS");
			((Hashtable)locationLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.CardGame] = new LocationInfo("#LOAD_TITLE_CARDGAME");
			((Hashtable)locationLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.FrontEnd] = new LocationInfo("#LOAD_TITLE_LOGIN");
			((Hashtable)locationLookup[GameController.ControllerType.SocialSpace])[GameController.ControllerType.DeckBuilder] = new LocationInfo("#LOAD_TITLE_DECKBUILDER");
			((Hashtable)locationLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.SocialSpace] = new LocationInfo("#LOAD_TITLE_GAMEWORLD");
			((Hashtable)locationLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.Brawler] = new LocationInfo("#LOAD_TITLE_MISSIONS");
			((Hashtable)locationLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.HeadQuarters] = new LocationInfo("#LOAD_TITLE_HEADQUARTERS");
			((Hashtable)locationLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.CardGame] = new LocationInfo("#LOAD_TITLE_CARDGAME");
			((Hashtable)locationLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.FrontEnd] = new LocationInfo("#LOAD_TITLE_LOGIN");
			((Hashtable)locationLookup[GameController.ControllerType.CardGame])[GameController.ControllerType.DeckBuilder] = new LocationInfo("#LOAD_TITLE_DECKBUILDER");
			((Hashtable)locationLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.SocialSpace] = new LocationInfo("#LOAD_TITLE_GAMEWORLD");
			((Hashtable)locationLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.Brawler] = new LocationInfo("#LOAD_TITLE_MISSIONS");
			((Hashtable)locationLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.HeadQuarters] = new LocationInfo("#LOAD_TITLE_HEADQUARTERS");
			((Hashtable)locationLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.CardGame] = new LocationInfo("#LOAD_TITLE_CARDGAME");
			((Hashtable)locationLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.FrontEnd] = new LocationInfo("#LOAD_TITLE_LOGIN");
			((Hashtable)locationLookup[GameController.ControllerType.Brawler])[GameController.ControllerType.DeckBuilder] = new LocationInfo("#LOAD_TITLE_DECKBUILDER");
			((Hashtable)locationLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.SocialSpace] = new LocationInfo("#LOAD_TITLE_GAMEWORLD");
			((Hashtable)locationLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.Brawler] = new LocationInfo("#LOAD_TITLE_MISSIONS");
			((Hashtable)locationLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.HeadQuarters] = new LocationInfo("#LOAD_TITLE_HEADQUARTERS");
			((Hashtable)locationLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.CardGame] = new LocationInfo("#LOAD_TITLE_CARDGAME");
			((Hashtable)locationLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.FrontEnd] = new LocationInfo("#LOAD_TITLE_LOGIN");
			((Hashtable)locationLookup[GameController.ControllerType.HeadQuarters])[GameController.ControllerType.DeckBuilder] = new LocationInfo("#LOAD_TITLE_DECKBUILDER");
			((Hashtable)locationLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.SocialSpace] = new LocationInfo("#LOAD_TITLE_GAMEWORLD");
			((Hashtable)locationLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.Brawler] = new LocationInfo("#LOAD_TITLE_MISSIONS");
			((Hashtable)locationLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.HeadQuarters] = new LocationInfo("#LOAD_TITLE_HEADQUARTERS");
			((Hashtable)locationLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.CardGame] = new LocationInfo("#LOAD_TITLE_CARDGAME");
			((Hashtable)locationLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.FrontEnd] = new LocationInfo("#LOAD_TITLE_LOGIN");
			((Hashtable)locationLookup[GameController.ControllerType.DeckBuilder])[GameController.ControllerType.DeckBuilder] = new LocationInfo("#LOAD_TITLE_DECKBUILDER");
		}
	}

	private static void FetchLoadingScreensBundle()
	{
		if (!loadingScreenBundleRequested)
		{
			GUIManager.Instance.BundleManager.LoadBundle("loading_bundle", delegate
			{
				loadingScreenAssetsAvailable = true;
			}, null);
			loadingScreenBundleRequested = true;
		}
	}

	private static void PopulateLoadingScreenList()
	{
		if (validLoadingScreens == null)
		{
			validLoadingScreens = new List<string>();
			validLoadingScreens.Add("GUI/loading/mshs_loading_background_01");
			validLoadingScreens.Add("GUI/loading/Loading");
			validLoadingScreens.Add("GUI/loading/Loading");
			validLoadingScreens.Add("GUI/loading/Loading");
			validLoadingScreens.Add("GUI/loading/Loading");
		}
	}

	private static void LoadAllContextData()
	{
		CreateContextLookup();
		CreateLocationLookup();
		PopulateLoadingScreenList();
		FetchLoadingScreensBundle();
	}

	public void SetLoadingContext(GameController.ControllerType from, GameController.ControllerType to)
	{
		LoadAllContextData();
		if (contextLookup.ContainsKey(from))
		{
			Hashtable hashtable = (Hashtable)contextLookup[from];
			if (hashtable.ContainsKey(to))
			{
				LoadingContext loadingContext = (LoadingContext)hashtable[to];
				loadingContext.smartTipUsageInstance = ((AppShell.Instance.SmartTipsManager == null) ? SmartTipsManager.DefaultSmartTipUsageInstance : AppShell.Instance.SmartTipsManager.GetSmartTip(loadingContext.category));
				string tip = loadingContext.smartTipUsageInstance.tip;
				tipText = ((!string.IsNullOrEmpty(tip)) ? tip.Substring(0, Math.Min(tip.Length, 128)) : string.Empty);
				whoseTipText = loadingContext.smartTipUsageInstance.titleKey;
				backgroundTextureSource = loadingContext.backgroundTextureSource;
				tipTextureSource = loadingContext.smartTipUsageInstance.titleIcon;
				additionalTextureSource = loadingContext.additionalTextureSource;
				additionalTextureOffset = loadingContext.additionalTextureOffset;
				additionalTextureSize = loadingContext.additionalTextureSize;
				customSetup = loadingContext.customSetup;
			}
			else
			{
				CspUtils.DebugLog("Context table doesn't have the 'to' controller type built-in. The type passed in was: " + to.ToString());
			}
		}
		else
		{
			CspUtils.DebugLog("Context table doesn't have the 'from' controller type built-in. The type passed in was: " + from.ToString());
		}
	}

	public void SetLoadingContext(LoadingContext context)
	{
		LoadAllContextData();
		backgroundTextureSource = context.backgroundTextureSource;
		additionalTextureOffset = context.additionalTextureOffset;
		additionalTextureSize = context.additionalTextureSize;
		additionalTextureSource = context.additionalTextureSource;
		customSetup = context.customSetup;
	}

	public void SetLocationInfo(GameController.ControllerType from, GameController.ControllerType to)
	{
		LoadAllContextData();
		if (locationLookup.ContainsKey(from))
		{
			Hashtable hashtable = (Hashtable)locationLookup[from];
			if (hashtable.ContainsKey(to))
			{
				LocationInfo locationInfo = (LocationInfo)hashtable[to];
				locationName = locationInfo.locationName;
			}
			else
			{
				CspUtils.DebugLog("Location table doesn't have the 'to' controller type built-in. The type passed in was: " + to.ToString());
			}
		}
		else
		{
			CspUtils.DebugLog("Location table doesn't have the 'from' controller type built-in. The type passed in was: " + from.ToString());
		}
	}

	public void SetLocationInfo(LocationInfo locInfo)
	{
		locationName = locInfo.locationName;
	}
}
