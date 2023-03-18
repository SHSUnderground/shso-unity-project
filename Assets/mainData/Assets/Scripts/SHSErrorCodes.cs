using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSErrorCodes
{
	public enum Message
	{
		Error,
		Detail,
		UnknownMessage,
		UnknownResponseMessage,
		ResponseGameWorld,
		ResponseLogin,
		ResponseQuit,
		ResponseFallback,
		ResponseConfirm,
		UserProfileError,
		AudioDefinitionsLoadFail,
		ApplicationDisconnectedError,
		UnableToConnect,
		OutOfMemory,
		GameServerDisconnectedError,
		CantEnterGameWorld,
		CantEnterBrawler,
		CantEnterCardGame,
		CantEnterHQ,
		CardGameUnavailable,
		CardGameBotdUnavailable,
		MissionsUnavailable,
		HQUnavailable,
		NotificationServerDisconnectedError,
		ApplicationLoadFailMessage,
		FriendsRequestFailedTooManyFriendsSource,
		FriendsRequestFailedTooManyFriendsTarget,
		FriendsRequestFailedTooManyInvites,
		FriendsRequestFailedInvitingFriendNotAllowed,
		FriendsRequestFailedNoMatchingPlayerFound,
		PrizeWheelFailedMalformedPrizeWheelData,
		PrizeWheelFailedTimeout,
		UnableToStartGame,
		DeckControllerFail,
		FeatureNotImplemented,
		ServerFull,
		ModeratorKicked,
		ModeratorWarning,
		ModeratorBanTemp,
		ModeratorBanPerm,
		ModeratorMuted,
		AutoLogout,
		ParentalControlLimit,
		OutOfGold,
		OutOfSilver,
		LoggedInElsewhere,
		GeneralServerDisconnect,
		ServerMessageStub,
		NotASubscriber,
		PlayerReported,
		SubscriptionNotAvailable,
		ArcadeNotAvailable,
		FriendNotOnline,
		TestMessage
	}

	public enum Code
	{
		ApplicationDisconnectedError,
		ApplicationLoadDataFail,
		UnableToConnect,
		OutOfMemory,
		AudioDefinitionsLoadFail,
		UserProfileLoadFail,
		GameServerDisconnectedError,
		CantEnterGameWorld,
		GameWorldSpawnerOwnershipFail,
		BrawlerTransactionTimeout,
		BrawlerTransactionError,
		BrawlerCantSpawnPlayer,
		BrawlerSpawnerOwnershipFail,
		CantEnterCardGame,
		CantEnterHQ,
		CardGameUnavailable,
		CardGameBotdUnavailable,
		ArcadeUnavailable,
		MissionsUnavailable,
		HQUnavailable,
		NotificationServerDisconnectedError,
		FriendsRequestFailedTooManyFriendsSource,
		FriendsRequestFailedTooManyFriendsTarget,
		FriendsRequestFailedTooManyInvites,
		FriendsRequestFailedInvitingFriendNotAllowed,
		FriendsRequestFailedNoMatchingPlayerFound,
		PrizeWheelFailedMalformedPrizeWheelData,
		PrizeWheelFailedTimeout,
		CoreLoadFail,
		DeckControllerFail,
		FeatureNotImplemented,
		ModeratorKicked,
		ModeratorBanTemp,
		ModeratorBanPerm,
		ModeratorWarning,
		ModeratorMuted,
		AutoLogout,
		SearchError,
		OutOfGold,
		OutOfSilver,
		ServerFull,
		LoggedInElsewhere,
		GeneralServerDisconnect,
		ParentalControlLimit,
		NotASubscriber,
		PlayerReported,
		SubscriptionNotAvailable,
		ArcadeGameUrlFail,
		FriendNotOnline,
		TestModeratorMessage,
		TestModerator02Message,
		TestModerator03Message,
		TestModerator04Message,
		TestSorryMessage,
		TestUhOh1Message,
		TestUhOh2Message,
		TestUhOh3Message,
		TestWhoopsMessage,
		TestWrongMessage
	}

	public class Response
	{
		public Code Code;

		public int Number;

		public ErrorResponseDelegate ResponseFunction;

		public Message Message;

		public bool SuppressErrorNumber;

		private string compositeMessage;

		public bool BypassNotification;

		public Response(int errorNumber, Code code, ErrorResponseDelegate responseFunc, Message message)
		{
			configureResponse(code, errorNumber, responseFunc, message, false);
		}

		public Response(int errorNumber, Code code, ErrorResponseDelegate responseFunc, Message message, bool suppressErrorNumber)
		{
			configureResponse(code, errorNumber, responseFunc, message, suppressErrorNumber);
		}

		private void configureResponse(Code code, int errorNumber, ErrorResponseDelegate responseFunc, Message message, bool suppressErrorNumber)
		{
			Code = code;
			Number = errorNumber;
			ResponseFunction = responseFunc;
			Message = message;
			SuppressErrorNumber = suppressErrorNumber;
			string id = (!messages.ContainsKey(Message)) ? messages[Message.UnknownMessage].Value : messages[Message].Value;
			compositeMessage = AppShell.Instance.stringTable[id] + Environment.NewLine;
			if (!suppressErrorNumber)
			{
				compositeMessage = AppShell.Instance.stringTable[messages[Message.Error].Value] + Number + " :" + Environment.NewLine + compositeMessage;
			}
			compositeMessage += Environment.NewLine;
		}

		public string getMessage()
		{
			return compositeMessage;
		}
	}

	public delegate void ErrorResponseDelegate(Code code);

	private static readonly Dictionary<Code, Response> responses;

	private static readonly Dictionary<Message, KeyValuePair<string, string>> messages;

	private static Dictionary<ErrorResponseDelegate, Message> responseMessages;

	static SHSErrorCodes()
	{
		messages = new Dictionary<Message, KeyValuePair<string, string>>();
		messages[Message.Error] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_ERROR");
		messages[Message.Detail] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_DETAIL");
		messages[Message.UnknownMessage] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_UNKNOWNMESSAGE");
		messages[Message.UnknownResponseMessage] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_UNKNOWNRESPONSE");
		messages[Message.ResponseGameWorld] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_GAMEWORLDRESPONSE");
		messages[Message.ResponseLogin] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_LOGINRESPONSE");
		messages[Message.ResponseConfirm] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_CONFIRMRESPONSE");
		messages[Message.ResponseQuit] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_QUITRESPONSE");
		messages[Message.ResponseFallback] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_FALLBACKRESPONSE");
		messages[Message.UserProfileError] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_USERPROFILEERROR");
		messages[Message.AudioDefinitionsLoadFail] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_AUDIODEFINITIONSLOADFAIL");
		messages[Message.ApplicationDisconnectedError] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_APPLICATIONDISCONNECTEDERROR");
		messages[Message.OutOfMemory] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_OUTOFMEMORY");
		messages[Message.UnableToConnect] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_UNABLETOCONNECT");
		messages[Message.GameServerDisconnectedError] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_GAMESERVERDISCONNECTEDERROR");
		messages[Message.CantEnterGameWorld] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_CANTENTERGAMEWORLD");
		messages[Message.CantEnterCardGame] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_CANTENTERCARDGAME");
		messages[Message.CantEnterHQ] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_CANTENTERHQ");
		messages[Message.CantEnterBrawler] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_CANTENTERBRAWLER");
		messages[Message.ServerFull] = new KeyValuePair<string, string>("#error_oops_title", "#error_server_full");
		messages[Message.NotificationServerDisconnectedError] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_NOTSERVERDISCONNECTEDERROR");
		messages[Message.ApplicationLoadFailMessage] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_APPLICATIONLOADFAILMESSAGE");
		messages[Message.FriendsRequestFailedTooManyFriendsSource] = new KeyValuePair<string, string>("#error_friends_request_title", "#ERRMESSAGE_FRIENDSREQUESTFAILEDTOOMANYFRIENDSSOURCE");
		messages[Message.FriendsRequestFailedTooManyFriendsTarget] = new KeyValuePair<string, string>("#error_friends_request_title", "#ERRMESSAGE_FRIENDSREQUESTFAILEDTOOMANYFRIENDSTARGET");
		messages[Message.FriendsRequestFailedTooManyInvites] = new KeyValuePair<string, string>("#error_friends_request_title", "#ERRMESSAGE_FRIENDSREQUESTFAILEDTOOMANYINVITES");
		messages[Message.FriendsRequestFailedInvitingFriendNotAllowed] = new KeyValuePair<string, string>("#error_friends_search_title", "#ERRMESSAGE_FRIENDSREQUESTFRIENDNOTAVAILABLE");
		messages[Message.FriendsRequestFailedNoMatchingPlayerFound] = new KeyValuePair<string, string>("#error_friends_search_title", "#ERRMESSAGE_FRIENDSREQUESTFAILEDTARGETDOESNOTEXISTORISNOTONLINE");
		messages[Message.PrizeWheelFailedMalformedPrizeWheelData] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_PRIZEWHEELFAILEDMALFORMEDPRIZEWHEELDATA");
		messages[Message.PrizeWheelFailedTimeout] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_PRIZEWHEELFAILEDTIMEOUT");
		messages[Message.UnableToStartGame] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_UNABLETOSTARTGAME");
		messages[Message.DeckControllerFail] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_UNABLETOSTARTDECKBUILDER");
		messages[Message.FeatureNotImplemented] = new KeyValuePair<string, string>("#error_oops_title", "#ERRMESSAGE_FEATURE_NOT_IMPLEMENTED");
		messages[Message.ModeratorKicked] = new KeyValuePair<string, string>("#moderator_kick_title", "#moderator_kick");
		messages[Message.ModeratorWarning] = new KeyValuePair<string, string>("#moderator_warn_title", "#moderator_warn");
		messages[Message.ModeratorBanTemp] = new KeyValuePair<string, string>("#moderator_ban_temp_title", "#moderator_ban_temp");
		messages[Message.ModeratorBanPerm] = new KeyValuePair<string, string>("#moderator_ban_perm_title", "#moderator_ban_perm");
		messages[Message.ModeratorMuted] = new KeyValuePair<string, string>("#moderator_mute_title", "#moderator_mute");
		messages[Message.AutoLogout] = new KeyValuePair<string, string>("#moderator_afk_title", "#moderator_afk");
		messages[Message.ParentalControlLimit] = new KeyValuePair<string, string>("#parent_noapproval_title", "#parent_noapproval");
		messages[Message.OutOfSilver] = new KeyValuePair<string, string>("#purchase_outofmoney_title", "#purchase_outofsilver");
		messages[Message.OutOfGold] = new KeyValuePair<string, string>("#purchase_outofmoney_title", "#purchase_outofgold");
		messages[Message.LoggedInElsewhere] = new KeyValuePair<string, string>("#loggedin_elsewhere_title", "#loggedin_elsewhere");
		messages[Message.GeneralServerDisconnect] = new KeyValuePair<string, string>("#serverdisconnect_title", "#serverdisconnect");
		messages[Message.TestMessage] = new KeyValuePair<string, string>("#error_oops_title", "TESTING MESSAGE");
		messages[Message.NotASubscriber] = new KeyValuePair<string, string>("#not_subscriber_title", "#not_subscriber");
		messages[Message.ServerMessageStub] = new KeyValuePair<string, string>("#error_oops_title", string.Empty);
		messages[Message.CardGameUnavailable] = new KeyValuePair<string, string>("#cardgame_not_available_title", "#card_game_unavailable");
		messages[Message.CardGameBotdUnavailable] = new KeyValuePair<string, string>("#cardgame_not_available_title", "#CARDGAME_BOTD_COMING_SOON");
		messages[Message.ArcadeNotAvailable] = new KeyValuePair<string, string>("#arcade_not_available_title", "#arcade_not_available_text");
		messages[Message.MissionsUnavailable] = new KeyValuePair<string, string>("#missions_not_available_title", "#missions_not_available");
		messages[Message.HQUnavailable] = new KeyValuePair<string, string>("#hq_not_available_title", "#hq_not_available");
		messages[Message.PlayerReported] = new KeyValuePair<string, string>("#blockreport_confirmed_title", "#blockreport_confirmed");
		messages[Message.SubscriptionNotAvailable] = new KeyValuePair<string, string>("#sub_unavailable_title", "#sub_unavailable");
		messages[Message.FriendNotOnline] = new KeyValuePair<string, string>("#friend_not_online_title", "#friend_request_playername_notonline");
		responses = new Dictionary<Code, Response>();
		responses[Code.CoreLoadFail] = new Response(450, Code.CoreLoadFail, ShutDownGame, Message.UnableToStartGame, true);
		responses[Code.CoreLoadFail].BypassNotification = true;
		responses[Code.OutOfMemory] = new Response(1, Code.OutOfMemory, ShutDownGame, Message.OutOfMemory);
		responses[Code.ApplicationDisconnectedError] = new Response(2, Code.ApplicationDisconnectedError, LoginRedirect, Message.ApplicationDisconnectedError);
		responses[Code.UserProfileLoadFail] = new Response(3, Code.UserProfileLoadFail, LoginRedirect, Message.UserProfileError);
		responses[Code.AudioDefinitionsLoadFail] = new Response(4, Code.AudioDefinitionsLoadFail, LoginRedirect, Message.AudioDefinitionsLoadFail);
		responses[Code.UnableToConnect] = new Response(101, Code.UnableToConnect, LoginRedirect, Message.UnableToConnect);
		responses[Code.GameServerDisconnectedError] = new Response(102, Code.GameServerDisconnectedError, GameDisconnectRedirect, Message.GameServerDisconnectedError);
		responses[Code.NotificationServerDisconnectedError] = new Response(103, Code.NotificationServerDisconnectedError, LoginRedirect, Message.NotificationServerDisconnectedError);
		responses[Code.CantEnterGameWorld] = new Response(201, Code.CantEnterGameWorld, LoginRedirect, Message.CantEnterGameWorld);
		responses[Code.GameWorldSpawnerOwnershipFail] = new Response(202, Code.GameWorldSpawnerOwnershipFail, FallbackRedirect, Message.CantEnterGameWorld);
		responses[Code.CantEnterCardGame] = new Response(301, Code.CantEnterCardGame, StandardRedirect, Message.CantEnterCardGame);
		responses[Code.CantEnterHQ] = new Response(401, Code.CantEnterHQ, StandardRedirect, Message.CantEnterHQ);
		responses[Code.BrawlerTransactionTimeout] = new Response(111, Code.BrawlerTransactionTimeout, StandardRedirect, Message.CantEnterBrawler);
		responses[Code.BrawlerTransactionError] = new Response(112, Code.BrawlerTransactionError, StandardRedirect, Message.CantEnterBrawler);
		responses[Code.BrawlerCantSpawnPlayer] = new Response(113, Code.BrawlerCantSpawnPlayer, StandardRedirect, Message.CantEnterBrawler);
		responses[Code.BrawlerSpawnerOwnershipFail] = new Response(114, Code.BrawlerSpawnerOwnershipFail, StandardRedirect, Message.CantEnterBrawler);
		responses[Code.ApplicationDisconnectedError] = new Response(115, Code.ApplicationLoadDataFail, LoginRedirect, Message.ApplicationLoadFailMessage);
		responses[Code.ArcadeGameUrlFail] = new Response(116, Code.ArcadeGameUrlFail, StandardRedirect, Message.ArcadeNotAvailable, true);
		responses[Code.FriendsRequestFailedTooManyFriendsSource] = new Response(400, Code.FriendsRequestFailedTooManyFriendsSource, NoRedirect, Message.FriendsRequestFailedTooManyFriendsSource, true);
		responses[Code.FriendsRequestFailedTooManyFriendsTarget] = new Response(430, Code.FriendsRequestFailedTooManyFriendsTarget, NoRedirect, Message.FriendsRequestFailedTooManyFriendsTarget, true);
		responses[Code.FriendsRequestFailedTooManyInvites] = new Response(408, Code.FriendsRequestFailedTooManyInvites, NoRedirect, Message.FriendsRequestFailedTooManyInvites, true);
		responses[Code.FriendsRequestFailedInvitingFriendNotAllowed] = new Response(409, Code.FriendsRequestFailedInvitingFriendNotAllowed, NoRedirect, Message.FriendsRequestFailedInvitingFriendNotAllowed);
		responses[Code.FriendsRequestFailedNoMatchingPlayerFound] = new Response(439, Code.FriendsRequestFailedNoMatchingPlayerFound, NoRedirect, Message.FriendsRequestFailedNoMatchingPlayerFound, true);
		responses[Code.PrizeWheelFailedMalformedPrizeWheelData] = new Response(417, Code.PrizeWheelFailedMalformedPrizeWheelData, NoRedirect, Message.PrizeWheelFailedMalformedPrizeWheelData);
		responses[Code.PrizeWheelFailedTimeout] = new Response(418, Code.PrizeWheelFailedTimeout, NoRedirect, Message.PrizeWheelFailedTimeout);
		responses[Code.FeatureNotImplemented] = new Response(421, Code.FeatureNotImplemented, NoRedirect, Message.FeatureNotImplemented);
		responses[Code.DeckControllerFail] = new Response(500, Code.DeckControllerFail, StandardRedirect, Message.DeckControllerFail);
		responses[Code.AutoLogout] = new Response(600, Code.AutoLogout, ShutDownGame, Message.AutoLogout, true);
		responses[Code.ModeratorKicked] = new Response(601, Code.ModeratorKicked, LoginRedirect, Message.ModeratorKicked, true);
		responses[Code.ModeratorBanPerm] = new Response(603, Code.ModeratorBanPerm, LoginRedirect, Message.ModeratorBanPerm, true);
		responses[Code.ModeratorBanTemp] = new Response(604, Code.ModeratorBanTemp, LoginRedirect, Message.ModeratorBanTemp, true);
		responses[Code.ModeratorWarning] = new Response(605, Code.ModeratorWarning, NoRedirect, Message.ModeratorWarning, true);
		responses[Code.ModeratorMuted] = new Response(621, Code.ModeratorMuted, NoRedirect, Message.ModeratorMuted, true);
		responses[Code.SearchError] = new Response(606, Code.SearchError, NoRedirect, Message.TestMessage, true);
		responses[Code.OutOfSilver] = new Response(610, Code.OutOfSilver, NoRedirect, Message.OutOfSilver, true);
		responses[Code.OutOfGold] = new Response(611, Code.OutOfGold, NoRedirect, Message.OutOfGold, true);
		responses[Code.LoggedInElsewhere] = new Response(612, Code.LoggedInElsewhere, ShutDownGame, Message.LoggedInElsewhere, true);
		responses[Code.GeneralServerDisconnect] = new Response(613, Code.GeneralServerDisconnect, ShutDownGame, Message.GeneralServerDisconnect, true);
		responses[Code.ParentalControlLimit] = new Response(608, Code.ParentalControlLimit, NoRedirect, Message.ParentalControlLimit, true);
		responses[Code.ServerFull] = new Response(609, Code.ServerFull, NoRedirect, Message.TestMessage, true);
		responses[Code.NotASubscriber] = new Response(615, Code.NotASubscriber, NoRedirect, Message.NotASubscriber, true);
		responses[Code.CardGameUnavailable] = new Response(617, Code.CardGameUnavailable, NoRedirect, Message.CardGameUnavailable, true);
		responses[Code.CardGameBotdUnavailable] = new Response(627, Code.CardGameBotdUnavailable, NoRedirect, Message.CardGameBotdUnavailable, true);
		responses[Code.ArcadeUnavailable] = new Response(618, Code.ArcadeUnavailable, NoRedirect, Message.ArcadeNotAvailable, true);
		responses[Code.MissionsUnavailable] = new Response(618, Code.MissionsUnavailable, NoRedirect, Message.MissionsUnavailable, true);
		responses[Code.HQUnavailable] = new Response(619, Code.HQUnavailable, NoRedirect, Message.HQUnavailable, true);
		responses[Code.PlayerReported] = new Response(620, Code.PlayerReported, NoRedirect, Message.PlayerReported, true);
		responses[Code.SubscriptionNotAvailable] = new Response(621, Code.SubscriptionNotAvailable, NoRedirect, Message.SubscriptionNotAvailable, true);
		responses[Code.ServerFull] = new Response(622, Code.ServerFull, LoginRedirect, Message.ServerFull, true);
		responses[Code.FriendNotOnline] = new Response(623, Code.FriendNotOnline, NoRedirect, Message.FriendNotOnline, true);
		responses[Code.TestModeratorMessage] = new Response(-106, Code.TestModeratorMessage, NoRedirect, Message.TestMessage);
		responses[Code.TestModerator02Message] = new Response(-107, Code.TestModerator02Message, NoRedirect, Message.TestMessage);
		responses[Code.TestModerator03Message] = new Response(-108, Code.TestModerator03Message, NoRedirect, Message.TestMessage);
		responses[Code.TestModerator04Message] = new Response(-109, Code.TestModerator04Message, NoRedirect, Message.TestMessage);
		responses[Code.TestSorryMessage] = new Response(-110, Code.TestSorryMessage, NoRedirect, Message.TestMessage);
		responses[Code.TestUhOh1Message] = new Response(-111, Code.TestUhOh1Message, NoRedirect, Message.TestMessage);
		responses[Code.TestUhOh2Message] = new Response(-112, Code.TestUhOh2Message, NoRedirect, Message.TestMessage);
		responses[Code.TestUhOh3Message] = new Response(-113, Code.TestUhOh3Message, NoRedirect, Message.TestMessage);
		responses[Code.TestWhoopsMessage] = new Response(-114, Code.TestWhoopsMessage, NoRedirect, Message.TestMessage);
		responses[Code.TestWrongMessage] = new Response(-115, Code.TestWrongMessage, NoRedirect, Message.TestMessage);
	}

	internal static Response GetResponse(Code code)
	{
		if (responses.ContainsKey(code))
		{
			return responses[code];
		}
		return null;
	}

	internal static string GetResponseMessage(Response response)
	{
		if (messages.ContainsKey(response.Message))
		{
			return messages[response.Message].Value;
		}
		return null;
	}

	internal static string GetResponseMessageTitle(Response response)
	{
		if (messages.ContainsKey(response.Message))
		{
			return messages[response.Message].Key;
		}
		return null;
	}

	private static void NoRedirect(Code code)
	{
	}

	private static void StandardRedirect(Code code)
	{
		AppShell.Instance.EventMgr.Fire(null, new WaitWatcherEvent.KillWaitWatcher());
		AppShell.Instance.Transition(GameController.ControllerType.SocialSpace);
	}

	private static void FallbackRedirect(Code code)
	{
		AppShell.Instance.Transition(GameController.ControllerType.Fallback);
	}

	private static void GameDisconnectRedirect(Code code)
	{
		GameController controller = GameController.GetController();
		if (controller != null)
		{
			if (controller is SocialSpaceController)
			{
				LoginRedirect(code);
			}
			else
			{
				StandardRedirect(code);
			}
		}
		else
		{
			LoginRedirect(code);
		}
	}

	private static void LoginRedirect(Code code)
	{
		if (Application.isWebPlayer)
		{
			ShutDownGame(code);
			return;
		}
		AppShell.Instance.EventMgr.Fire(null, new WaitWatcherEvent.KillWaitWatcher());
		AppShell.Instance.ServerConnection.Logout();
		AppShell.Instance.Transition(GameController.ControllerType.FrontEnd);
	}

	private static void ShutDownGame(Code code)
	{
		Application.ExternalCall("HEROUPNS.RedirectToLogin");
		AppShell.Instance.Quit();
	}
}
