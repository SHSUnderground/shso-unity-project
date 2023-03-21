using System;
using System.Text;
using System.Xml;
using UnityEngine;

public class CommunicationManager : IDisposable
{
	private const string FILTER_CHARS = "0123456789@#$%^&*()-=_+[]{}|\\;:\"<>/~`'";

	private const int MAX_SPACES = 2;

	public void Initialize()
	{
		AppShell.Instance.EventMgr.AddListener<UserWarningMessage>(OnWarningMessageReceived);
		AppShell.Instance.EventMgr.AddListener<UserRoomErrorMessage>(OnUserRoomErrorMessage);
		AppShell.Instance.EventMgr.AddListener<UserAdminMessage>(OnUserAdminMessage);
		AppShell.Instance.EventMgr.AddListener<UserMutedMessage>(OnUserMutedMessage);
	}

	private static void OnUserMutedMessage(UserMutedMessage message)
	{
		if (message.Duration <= 0)
		{
			GUIManager.Instance.ShowDialogWithTitle(GUIManager.DialogTypeEnum.OkDialog, "#MODERATOR_UNMUTE_TITLE", "#MODERATOR_UNMUTE", null, GUIControl.ModalLevelEnum.Full);
		}
		else if (string.IsNullOrEmpty(message.Message))
		{
			GUIManager.Instance.ShowErrorDialog(SHSErrorCodes.Code.ModeratorMuted, string.Empty);
		}
		else
		{
			GUIManager.Instance.ShowErrorDialog(SHSErrorCodes.Code.ModeratorMuted, message.Message, string.Empty);
		}
	}

	private static void OnUserAdminMessage(UserAdminMessage message)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(message.Message);
		string attribute = xmlDocument["message"].GetAttribute("type");
		string text = (xmlDocument["message"].FirstChild == null) ? string.Empty : xmlDocument["message"].FirstChild.Value;
		SHSErrorCodes.Code code;
		switch (attribute)
		{
		case "tempBan":
			code = SHSErrorCodes.Code.ModeratorBanTemp;
			break;
		case "permBan":
			code = SHSErrorCodes.Code.ModeratorBanPerm;
			break;
		case "kick":
			code = SHSErrorCodes.Code.ModeratorKicked;
			if (!string.IsNullOrEmpty(text) && !(text == "null"))
			{
				AppShell.Instance.ServerConnection.Logout();
				GUIManager.Instance.ShowErrorDialog(code, text, string.Empty);
				return;
			}
			break;
		case "loggedInElsewhere":
			AppShell.Instance.ServerConnection.NotifyPendingAdminDisconnect();
			code = SHSErrorCodes.Code.LoggedInElsewhere;
			GUIManager.Instance.ShowErrorDialog(code, text);
			return;
		case "gameTooFull":
			AppShell.Instance.ServerConnection.NotifyPendingAdminDisconnect();
			code = SHSErrorCodes.Code.ServerFull;
			GUIManager.Instance.ShowErrorDialog(code, text);
			return;
		default:
			code = SHSErrorCodes.Code.GeneralServerDisconnect;
			break;
		}
		AppShell.Instance.ServerConnection.Logout();
		GUIManager.Instance.ShowErrorDialog(code, text);
	}

	private static void OnWarningMessageReceived(UserWarningMessage message)
	{
		GUIManager.Instance.ShowErrorDialog(message.warningType, message.warningMessage, "Admin message sent to this player: " + message.warningMessage);
	}

	private void OnUserRoomErrorMessage(UserRoomErrorMessage message)
	{
		switch (message.errorCode)
		{
		case "INVALID_MESSAGE":
			FireGameWorldOpenChatErrorMessage("#Chat_Error_INVALID_MESSAGE");
			break;
		case "SENDER_MUTED":
		{
			CspUtils.DebugLog(message.arguments["muted_for_seconds"]);
			double result;
			string message2 = (!double.TryParse(Convert.ToString(message.arguments["muted_for_seconds"]), out result)) ? "#Chat_Error_SENDER_MUTED" : string.Format(AppShell.Instance.stringTable["#Chat_Error_SENDER_MUTED_time"], TimeSpan.FromSeconds(Math.Abs(result)).ToString());
			FireGameWorldOpenChatErrorMessage(message2);
			break;
		}
		case "SENDER_ENTITLEMENT_RESTRICTION":
			FireGameWorldOpenChatErrorMessage("#Chat_Error_SENDER_ENTITLEMENT_RESTRICTION");
			break;
		case "INVALID_SENDER_STATE":
			FireGameWorldOpenChatErrorMessage("#Chat_Error_SENDER_STATE");
			break;
		case "SENDER_NOT_IN_ROOM":
			FireGameWorldOpenChatErrorMessage("#Chat_Error_NOT_IN_ROOM");
			break;
		case "MESSAGE_RATE_EXCEEDED":
			FireGameWorldOpenChatErrorMessage("#Chat_Error_RATE_EXCEEDED");
			break;
		case "MAX_MESSAGE_LENGTH_EXCEEDED":
			FireGameWorldOpenChatErrorMessage("#Chat_Error_MAX_MESSAGE_LENGTH_EXCEEDED");
			break;
		case "CHAT_DISABLED":
			FireGameWorldOpenChatErrorMessage("#Chat_Error_RATE_CHAT_DISABLED");
			break;
		default:
			CspUtils.DebugLog("Received message of type " + message.errorCode + " and message: " + message.errorMessage);
			break;
		}
	}

	private void FireGameWorldOpenChatErrorMessage(string message)
	{
		AppShell.Instance.EventMgr.Fire(this, new GameWorldOpenChatMessage(AppShell.Instance.stringTable[message], AppShell.Instance.Profile.UserId, GameWorldOpenChatMessage.ChatMessageTypeEnum.Filter));
	}

	public static OpenChatMessage ProcessChatMessage(GameWorldOpenChatMessage msg, string playerName, SpawnData.PlayerType playerType)
	{
		CspUtils.DebugLog("ProcessChatMessage");
		OpenChatMessage.MessageStyle messageStyle = ConvertPlayerTypeToMessageStyle(msg.messageType, playerType);
		string empty = string.Empty;
		empty = ((messageStyle == OpenChatMessage.MessageStyle.System) ? msg.chatMessage : FilterChatString(msg.chatMessage));
		return new OpenChatMessage(messageStyle, empty, playerName, msg.sendingPlayerId);
	}

	public static OpenChatMessage ProcessChatMessage(GameWorldMenuChatMessage msg, string playerName, long playerId, SpawnData.PlayerType playerType)
	{
		CspUtils.DebugLog("ProcessChatMessage");
		OpenChatMessage.MessageStyle messageStyle = ConvertPlayerTypeToMessageStyle(GameWorldOpenChatMessage.ChatMessageTypeEnum.PlayerChat, playerType);
		string message = AppShell.Instance.stringTable[msg.Group.PhraseKey];
		return new OpenChatMessage(messageStyle, message, playerName, playerId);
	}

	private static OpenChatMessage.MessageStyle ConvertPlayerTypeToMessageStyle(GameWorldOpenChatMessage.ChatMessageTypeEnum type, SpawnData.PlayerType pType)
	{
		switch (type)
		{
		case GameWorldOpenChatMessage.ChatMessageTypeEnum.Filter:
			return OpenChatMessage.MessageStyle.System;
		case GameWorldOpenChatMessage.ChatMessageTypeEnum.Moderator:
			return OpenChatMessage.MessageStyle.System;
		default:
			if ((pType & SpawnData.PlayerType.Self) != 0)
			{
				return OpenChatMessage.MessageStyle.Self;
			}
			if ((pType & SpawnData.PlayerType.Friend) != 0)
			{
				return OpenChatMessage.MessageStyle.Friend;
			}
			if ((pType & SpawnData.PlayerType.ShieldAgent) != 0)
			{
				return OpenChatMessage.MessageStyle.Sub;
			}
			return OpenChatMessage.MessageStyle.NonSub;
		}
	}

	private static OpenChatMessage.MessageStyle ConvertSelfMessageStyleIntoMessageStyle()
	{
		if (Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldPlayAllow))
		{
			return OpenChatMessage.MessageStyle.Sub;
		}
		return OpenChatMessage.MessageStyle.NonSub;
	}

	public static Color GetMessageColor(OpenChatMessage.MessageStyle style)
	{
		switch (style)
		{
		case OpenChatMessage.MessageStyle.Self:
			return GetMessageColor(ConvertSelfMessageStyleIntoMessageStyle());
		case OpenChatMessage.MessageStyle.NonSub:
			return ColorUtil.FromRGB255(41, 103, 188);
		case OpenChatMessage.MessageStyle.Sub:
			return ColorUtil.FromRGB255(208, 142, 0);
		case OpenChatMessage.MessageStyle.Friend:
			return ColorUtil.FromRGB255(56, 172, 117);
		case OpenChatMessage.MessageStyle.System:
			return ColorUtil.FromRGB255(127, 17, 17);
		default:
			return Color.white;
		}
	}

	public void Dispose()
	{
		Dispose(false);
		GC.SuppressFinalize(this);
	}

	public void Dispose(bool isDisposing)
	{
		AppShell.Instance.EventMgr.RemoveListener<UserWarningMessage>(OnWarningMessageReceived);
	}

	public static void ReportPlayer(int playerId, string playerName, string reportType, string reportSubType, string reportMessage)
	{
		if (!(AppShell.Instance == null) && AppShell.Instance.Profile != null)
		{
			long userId = AppShell.Instance.Profile.UserId;
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("reported_player_id", playerId.ToString());
			wWWForm.AddField("report_type", reportType);
			wWWForm.AddField("report_subtype", reportSubType);
			wWWForm.AddField("report_message", reportMessage);
			Entitlements.ServerValueEntitlement serverValueEntitlement = Singleton<Entitlements>.instance.EntitlementsSet[Entitlements.EntitlementFlagEnum.PlayerLanguage] as Entitlements.ServerValueEntitlement;
			Entitlements.ServerValueEntitlement serverValueEntitlement2 = Singleton<Entitlements>.instance.EntitlementsSet[Entitlements.EntitlementFlagEnum.PlayerCountry] as Entitlements.ServerValueEntitlement;
			if (serverValueEntitlement != null)
			{
				wWWForm.AddField("playerlanguage", serverValueEntitlement.Value);
			}
			if (serverValueEntitlement2 != null)
			{
				wWWForm.AddField("playercountry", serverValueEntitlement2.Value);
			}
			AppShell.Instance.WebService.StartRequest("resources$users/csr_report_player.py", delegate(ShsWebResponse response)
			{
				if (response.Status == 200)
				{
					GUIManager.Instance.ShowErrorDialog(SHSErrorCodes.Code.PlayerReported, string.Empty);
				}
				else
				{
					CspUtils.DebugLog("Server returned: " + response.Status + ":" + response.Body);
				}
			}, wWWForm.data, ShsWebService.ShsWebServiceType.RASP);
		}
	}

	public static string FilterChatString(string message)
	{
		StringBuilder stringBuilder = new StringBuilder(message.Length);
		int num = 0;
		foreach (char c in message)
		{
			if (c == ' ')
			{
				if (num >= 2)
				{
					continue;
				}
				num++;
			}
			else
			{
				num = 0;
			}
			if ("0123456789@#$%^&*()-=_+[]{}|\\;:\"<>/~`'".IndexOf(c) < 0)
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}
}
