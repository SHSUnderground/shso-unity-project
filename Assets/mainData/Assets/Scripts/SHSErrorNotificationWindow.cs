using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SHSErrorNotificationWindow : SHSCommonDialogWindow
{
	public enum ErrorIcons
	{
		Redirected,
		Kicked,
		BannedTemp,
		BannedPerm,
		Warning,
		AutoLogout,
		Moderator,
		Moderator02,
		Moderator03,
		Moderator04,
		Sorry,
		UhOh1,
		UhOh2,
		UhOh3,
		Whoops,
		Wrong,
		Search,
		OutOfMoney,
		ServerFull,
		ParentNoApproval,
		MissionNoGo,
		TooManyFriends,
		NotASubscriber,
		CardGameNoGo,
		FriendBlocking,
		SubscriptionNotAvailable,
		FriendUnavailable
	}

	public enum ErrorChoiceType
	{
		YesNo,
		OkCancel
	}

	public struct ErrorIconInfo
	{
		public string IconPath;

		public Vector2 IconSize;

		public Vector2 IconOffset;

		public ErrorIconInfo(string IconPath, Vector2 IconSize, Vector2 IconOffset)
		{
			this.IconPath = IconPath;
			this.IconSize = IconSize;
			this.IconOffset = IconOffset;
		}

		public ErrorIconInfo(string IconPath, Vector2 IconSize)
		{
			this = new ErrorIconInfo(IconPath, IconSize, Vector2.zero);
		}
	}

	public struct ErrorChoiceInfo
	{
		public string cancelBundlePath;

		public string confirmBundlePath;

		public Type canceButtonType;

		public Type confirmButtonType;

		public static ErrorChoiceInfo None
		{
			get
			{
				return default(ErrorChoiceInfo);
			}
		}

		public ErrorChoiceInfo(string confirmBundlePath, string cancelBundlePath, Type confirmButtonType, Type cancelButtonType)
		{
			this.confirmBundlePath = confirmBundlePath;
			this.cancelBundlePath = cancelBundlePath;
			this.confirmButtonType = confirmButtonType;
			canceButtonType = cancelButtonType;
		}
	}

	private float timeOutTimer;

	private float errorMessageDisplayTimeout = 20f;

	private bool allowTimeout = true;

	private static Dictionary<SHSErrorCodes.Code, ErrorIcons> ErrorCodeLookup;

	private static Dictionary<ErrorIcons, ErrorIconInfo> ErrorIconLookup;

	private static bool errorIconsInitialized;

	private GUIButton submitLogButton;

	private static Hashtable errorChoiceLookup;

	[CompilerGenerated]
	private bool _003CSystemLevelError_003Ek__BackingField;

	public bool AllowTimeout
	{
		get
		{
			return allowTimeout;
		}
		set
		{
			allowTimeout = value;
		}
	}

	public float ErrorMessageDisplayTimeout
	{
		get
		{
			return errorMessageDisplayTimeout;
		}
		set
		{
			errorMessageDisplayTimeout = value;
		}
	}

	public bool SystemLevelError
	{
		[CompilerGenerated]
		get
		{
			return _003CSystemLevelError_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CSystemLevelError_003Ek__BackingField = value;
		}
	}

	// public SHSErrorNotificationWindow()
	// 	: this(SHSErrorCodes.Code.FeatureNotImplemented)
	// {
	// }

	// public SHSErrorNotificationWindow(SHSErrorCodes.Code errorCode)
	// {
	// 	ErrorIconInfo errorIconInfo = GetErrorIconInfo(errorCode);
	// 	super(errorIconInfo.IconPath, "common_bundle|L_mshs_button_ok", typeof(SHSDialogOkButton), false);
	// 	if (!SHSErrorCodes.GetResponse(errorCode).SuppressErrorNumber)
	// 	{
	// 		SystemLevelError = true;
	// 	}
	// 	initialize(GetErrorIconInfo(errorCode));
	// }

	// public SHSErrorNotificationWindow(ErrorIcons iconType)
	// {
	// 	ErrorIconInfo errorIconInfo = GetErrorIconInfo(iconType);
	// 	super(errorIconInfo.IconPath, "common_bundle|L_mshs_button_ok", typeof(SHSDialogOkButton), false);
	// 	initialize(GetErrorIconInfo(iconType));
	// }

	// public SHSErrorNotificationWindow(SHSErrorCodes.Code errorCode, ErrorChoiceType choiceType)
	// {
	// 	ErrorIconInfo errorIconInfo = GetErrorIconInfo(errorCode);
	// 	string iconPath = errorIconInfo.IconPath;
	// 	ErrorChoiceInfo errorChoiceInfo = GetErrorChoiceInfo(choiceType);
	// 	string confirmBundlePath = errorChoiceInfo.confirmBundlePath;
	// 	ErrorChoiceInfo errorChoiceInfo2 = GetErrorChoiceInfo(choiceType);
	// 	string cancelBundlePath = errorChoiceInfo2.cancelBundlePath;
	// 	ErrorChoiceInfo errorChoiceInfo3 = GetErrorChoiceInfo(choiceType);
	// 	Type confirmButtonType = errorChoiceInfo3.confirmButtonType;
	// 	ErrorChoiceInfo errorChoiceInfo4 = GetErrorChoiceInfo(choiceType);
	// 	super(iconPath, "common_bundle|button_close", confirmBundlePath, cancelBundlePath, confirmButtonType, errorChoiceInfo4.canceButtonType, false);
	// 	initialize(GetErrorIconInfo(errorCode));
	// }

	// public SHSErrorNotificationWindow(ErrorIcons iconType, ErrorChoiceType choiceType)
	// {
	// 	ErrorIconInfo errorIconInfo = GetErrorIconInfo(iconType);
	// 	string iconPath = errorIconInfo.IconPath;
	// 	ErrorChoiceInfo errorChoiceInfo = GetErrorChoiceInfo(choiceType);
	// 	string confirmBundlePath = errorChoiceInfo.confirmBundlePath;
	// 	ErrorChoiceInfo errorChoiceInfo2 = GetErrorChoiceInfo(choiceType);
	// 	string cancelBundlePath = errorChoiceInfo2.cancelBundlePath;
	// 	ErrorChoiceInfo errorChoiceInfo3 = GetErrorChoiceInfo(choiceType);
	// 	Type confirmButtonType = errorChoiceInfo3.confirmButtonType;
	// 	ErrorChoiceInfo errorChoiceInfo4 = GetErrorChoiceInfo(choiceType);
	// 	super(iconPath, "common_bundle|button_close", confirmBundlePath, cancelBundlePath, confirmButtonType, errorChoiceInfo4.canceButtonType, false);
	// 	initialize(GetErrorIconInfo(iconType));
	// }

	public SHSErrorNotificationWindow() : this(SHSErrorCodes.Code.FeatureNotImplemented)
    {
    }

    public SHSErrorNotificationWindow(SHSErrorCodes.Code errorCode) : base(GetErrorIconInfo(errorCode).IconPath, "common_bundle|L_mshs_button_ok", typeof(SHSDialogOkButton), false)
    {
        this.errorMessageDisplayTimeout = 20f;
        this.allowTimeout = true;
        if (!SHSErrorCodes.GetResponse(errorCode).SuppressErrorNumber)
        {
            this.SystemLevelError = true;
        }
        this.initialize(GetErrorIconInfo(errorCode));
    }

    public SHSErrorNotificationWindow(ErrorIcons iconType) : base(GetErrorIconInfo(iconType).IconPath, "common_bundle|L_mshs_button_ok", typeof(SHSDialogOkButton), false)
    {
        this.errorMessageDisplayTimeout = 20f;
        this.allowTimeout = true;
        this.initialize(GetErrorIconInfo(iconType));
    }

    public SHSErrorNotificationWindow(SHSErrorCodes.Code errorCode, ErrorChoiceType choiceType) : base(GetErrorIconInfo(errorCode).IconPath, "common_bundle|button_close", GetErrorChoiceInfo(choiceType).confirmBundlePath, GetErrorChoiceInfo(choiceType).cancelBundlePath, GetErrorChoiceInfo(choiceType).confirmButtonType, GetErrorChoiceInfo(choiceType).canceButtonType, false)
    {
        this.errorMessageDisplayTimeout = 20f;
        this.allowTimeout = true;
        this.initialize(GetErrorIconInfo(errorCode));
    }

    public SHSErrorNotificationWindow(ErrorIcons iconType, ErrorChoiceType choiceType) : base(GetErrorIconInfo(iconType).IconPath, "common_bundle|button_close", GetErrorChoiceInfo(choiceType).confirmBundlePath, GetErrorChoiceInfo(choiceType).cancelBundlePath, GetErrorChoiceInfo(choiceType).confirmButtonType, GetErrorChoiceInfo(choiceType).canceButtonType, false)
    {
        this.errorMessageDisplayTimeout = 20f;
        this.allowTimeout = true;
        this.initialize(GetErrorIconInfo(iconType));
    }


	static SHSErrorNotificationWindow()
	{
		GenerateErrorIconInfo();
	}

	protected static void GenerateErrorChoiceInfo()
	{
		if (errorChoiceLookup == null)
		{
			errorChoiceLookup = new Hashtable();
			errorChoiceLookup[ErrorChoiceType.YesNo] = new ErrorChoiceInfo("common_bundle|L_mshs_button_ok", "common_bundle|L_mshs_button_cancel", typeof(SHSDialogYesButton), typeof(SHSDialogNoButton));
			errorChoiceLookup[ErrorChoiceType.OkCancel] = new ErrorChoiceInfo("common_bundle|L_mshs_button_ok", "common_bundle|L_mshs_button_cancel", typeof(SHSDialogOkButton), typeof(SHSDialogCancelButton));
		}
	}

	protected static void GenerateErrorIconInfo()
	{
		if (!errorIconsInitialized)
		{
			ErrorIconLookup = new Dictionary<ErrorIcons, ErrorIconInfo>();
			ErrorIconLookup[ErrorIcons.Redirected] = new ErrorIconInfo("common_bundle|notification_icon_redirected", new Vector2(188f, 167f));
			ErrorIconLookup[ErrorIcons.BannedTemp] = new ErrorIconInfo("common_bundle|notification_icon_moderator04", new Vector2(256f, 256f));
			ErrorIconLookup[ErrorIcons.BannedPerm] = new ErrorIconInfo("common_bundle|notification_icon_moderator03", new Vector2(256f, 256f));
			ErrorIconLookup[ErrorIcons.Kicked] = new ErrorIconInfo("common_bundle|notification_icon_moderator", new Vector2(176f, 130f));
			ErrorIconLookup[ErrorIcons.Warning] = new ErrorIconInfo("common_bundle|notification_icon_moderator02", new Vector2(176f, 185f));
			ErrorIconLookup[ErrorIcons.AutoLogout] = new ErrorIconInfo("common_bundle|notification_icon_autologout", new Vector2(114f, 178f));
			ErrorIconLookup[ErrorIcons.Moderator] = new ErrorIconInfo("common_bundle|notification_icon_moderator", new Vector2(176f, 130f));
			ErrorIconLookup[ErrorIcons.Moderator02] = new ErrorIconInfo("common_bundle|notification_icon_moderator02", new Vector2(176f, 185f));
			ErrorIconLookup[ErrorIcons.Moderator03] = new ErrorIconInfo("common_bundle|notification_icon_moderator03", new Vector2(188f, 185f));
			ErrorIconLookup[ErrorIcons.Moderator04] = new ErrorIconInfo("common_bundle|notification_icon_moderator04", new Vector2(177f, 179f));
			ErrorIconLookup[ErrorIcons.Sorry] = new ErrorIconInfo("common_bundle|notification_icon_sorry", new Vector2(142f, 153f));
			ErrorIconLookup[ErrorIcons.UhOh1] = new ErrorIconInfo("common_bundle|notification_icon_uhoh01", new Vector2(148f, 133f));
			ErrorIconLookup[ErrorIcons.UhOh2] = new ErrorIconInfo("common_bundle|notification_icon_uhoh02", new Vector2(144f, 118f));
			ErrorIconLookup[ErrorIcons.UhOh3] = new ErrorIconInfo("common_bundle|notification_icon_uhoh03", new Vector2(167f, 120f));
			ErrorIconLookup[ErrorIcons.Whoops] = new ErrorIconInfo("common_bundle|notification_icon_whoops", new Vector2(168f, 149f));
			ErrorIconLookup[ErrorIcons.Wrong] = new ErrorIconInfo("common_bundle|notification_icon_wrong", new Vector2(150f, 134f));
			ErrorIconLookup[ErrorIcons.Search] = new ErrorIconInfo("common_bundle|notification_searcherror", new Vector2(158f, 185f));
			ErrorIconLookup[ErrorIcons.OutOfMoney] = new ErrorIconInfo("common_bundle|notification_outofmoney", new Vector2(149f, 185f));
			ErrorIconLookup[ErrorIcons.ServerFull] = new ErrorIconInfo("common_bundle|notification_serverfull", new Vector2(154f, 143f));
			ErrorIconLookup[ErrorIcons.ParentNoApproval] = new ErrorIconInfo("common_bundle|notification_parentalcontrol", new Vector2(141f, 163f));
			ErrorIconLookup[ErrorIcons.MissionNoGo] = new ErrorIconInfo("common_bundle|notification_icon_missionnogo", new Vector2(157f, 209f));
			ErrorIconLookup[ErrorIcons.TooManyFriends] = new ErrorIconInfo("common_bundle|notification_icon_toomany", new Vector2(162f, 205f));
			ErrorIconLookup[ErrorIcons.NotASubscriber] = new ErrorIconInfo("common_bundle|notification_icon_subscription", new Vector2(190f, 170f));
			ErrorIconLookup[ErrorIcons.CardGameNoGo] = new ErrorIconInfo("common_bundle|notification_icon_cardgamenogo", new Vector2(159f, 198f));
			ErrorIconLookup[ErrorIcons.FriendBlocking] = new ErrorIconInfo("common_bundle|notification_icon_blocking", new Vector2(153f, 238f));
			ErrorIconLookup[ErrorIcons.SubscriptionNotAvailable] = new ErrorIconInfo("mysquadgadget_bundle|L_shield_recruit_logo_01", new Vector2(140f, 140f), new Vector2(0f, 35f));
			ErrorIconLookup[ErrorIcons.FriendUnavailable] = new ErrorIconInfo("common_bundle|notification_icon_friendnotavailable", new Vector2(215f, 210f));
			ErrorCodeLookup = new Dictionary<SHSErrorCodes.Code, ErrorIcons>();
			ErrorCodeLookup[SHSErrorCodes.Code.OutOfMemory] = ErrorIcons.Wrong;
			ErrorCodeLookup[SHSErrorCodes.Code.ApplicationLoadDataFail] = ErrorIcons.Wrong;
			ErrorCodeLookup[SHSErrorCodes.Code.ApplicationDisconnectedError] = ErrorIcons.Redirected;
			ErrorCodeLookup[SHSErrorCodes.Code.UserProfileLoadFail] = ErrorIcons.Wrong;
			ErrorCodeLookup[SHSErrorCodes.Code.AudioDefinitionsLoadFail] = ErrorIcons.Wrong;
			ErrorCodeLookup[SHSErrorCodes.Code.UnableToConnect] = ErrorIcons.Wrong;
			ErrorCodeLookup[SHSErrorCodes.Code.GameServerDisconnectedError] = ErrorIcons.Redirected;
			ErrorCodeLookup[SHSErrorCodes.Code.NotificationServerDisconnectedError] = ErrorIcons.Redirected;
			ErrorCodeLookup[SHSErrorCodes.Code.CantEnterGameWorld] = ErrorIcons.Wrong;
			ErrorCodeLookup[SHSErrorCodes.Code.GameWorldSpawnerOwnershipFail] = ErrorIcons.Wrong;
			ErrorCodeLookup[SHSErrorCodes.Code.CantEnterCardGame] = ErrorIcons.CardGameNoGo;
			ErrorCodeLookup[SHSErrorCodes.Code.CantEnterHQ] = ErrorIcons.Wrong;
			ErrorCodeLookup[SHSErrorCodes.Code.BrawlerTransactionTimeout] = ErrorIcons.MissionNoGo;
			ErrorCodeLookup[SHSErrorCodes.Code.BrawlerTransactionError] = ErrorIcons.MissionNoGo;
			ErrorCodeLookup[SHSErrorCodes.Code.BrawlerCantSpawnPlayer] = ErrorIcons.MissionNoGo;
			ErrorCodeLookup[SHSErrorCodes.Code.BrawlerSpawnerOwnershipFail] = ErrorIcons.MissionNoGo;
			ErrorCodeLookup[SHSErrorCodes.Code.ApplicationDisconnectedError] = ErrorIcons.Redirected;
			ErrorCodeLookup[SHSErrorCodes.Code.FriendsRequestFailedTooManyFriendsSource] = ErrorIcons.TooManyFriends;
			ErrorCodeLookup[SHSErrorCodes.Code.FriendsRequestFailedTooManyFriendsTarget] = ErrorIcons.TooManyFriends;
			ErrorCodeLookup[SHSErrorCodes.Code.FriendsRequestFailedTooManyInvites] = ErrorIcons.TooManyFriends;
			ErrorCodeLookup[SHSErrorCodes.Code.FriendsRequestFailedInvitingFriendNotAllowed] = ErrorIcons.ParentNoApproval;
			ErrorCodeLookup[SHSErrorCodes.Code.FriendsRequestFailedNoMatchingPlayerFound] = ErrorIcons.Whoops;
			ErrorCodeLookup[SHSErrorCodes.Code.PrizeWheelFailedMalformedPrizeWheelData] = ErrorIcons.Wrong;
			ErrorCodeLookup[SHSErrorCodes.Code.PrizeWheelFailedTimeout] = ErrorIcons.Wrong;
			ErrorCodeLookup[SHSErrorCodes.Code.FeatureNotImplemented] = ErrorIcons.Redirected;
			ErrorCodeLookup[SHSErrorCodes.Code.DeckControllerFail] = ErrorIcons.Wrong;
			ErrorCodeLookup[SHSErrorCodes.Code.CoreLoadFail] = ErrorIcons.Wrong;
			ErrorCodeLookup[SHSErrorCodes.Code.FeatureNotImplemented] = ErrorIcons.Redirected;
			ErrorCodeLookup[SHSErrorCodes.Code.AutoLogout] = ErrorIcons.AutoLogout;
			ErrorCodeLookup[SHSErrorCodes.Code.ModeratorKicked] = ErrorIcons.Kicked;
			ErrorCodeLookup[SHSErrorCodes.Code.ModeratorWarning] = ErrorIcons.Warning;
			ErrorCodeLookup[SHSErrorCodes.Code.ModeratorBanTemp] = ErrorIcons.Moderator04;
			ErrorCodeLookup[SHSErrorCodes.Code.ModeratorBanPerm] = ErrorIcons.Moderator03;
			ErrorCodeLookup[SHSErrorCodes.Code.ModeratorMuted] = ErrorIcons.Moderator03;
			ErrorCodeLookup[SHSErrorCodes.Code.SearchError] = ErrorIcons.Search;
			ErrorCodeLookup[SHSErrorCodes.Code.ParentalControlLimit] = ErrorIcons.ParentNoApproval;
			ErrorCodeLookup[SHSErrorCodes.Code.OutOfGold] = ErrorIcons.OutOfMoney;
			ErrorCodeLookup[SHSErrorCodes.Code.OutOfSilver] = ErrorIcons.OutOfMoney;
			ErrorCodeLookup[SHSErrorCodes.Code.LoggedInElsewhere] = ErrorIcons.Kicked;
			ErrorCodeLookup[SHSErrorCodes.Code.GeneralServerDisconnect] = ErrorIcons.Kicked;
			ErrorCodeLookup[SHSErrorCodes.Code.ServerFull] = ErrorIcons.ServerFull;
			ErrorCodeLookup[SHSErrorCodes.Code.NotASubscriber] = ErrorIcons.NotASubscriber;
			ErrorCodeLookup[SHSErrorCodes.Code.CardGameUnavailable] = ErrorIcons.CardGameNoGo;
			ErrorCodeLookup[SHSErrorCodes.Code.CardGameBotdUnavailable] = ErrorIcons.CardGameNoGo;
			ErrorCodeLookup[SHSErrorCodes.Code.MissionsUnavailable] = ErrorIcons.MissionNoGo;
			ErrorCodeLookup[SHSErrorCodes.Code.HQUnavailable] = ErrorIcons.Sorry;
			ErrorCodeLookup[SHSErrorCodes.Code.PlayerReported] = ErrorIcons.FriendBlocking;
			ErrorCodeLookup[SHSErrorCodes.Code.SubscriptionNotAvailable] = ErrorIcons.SubscriptionNotAvailable;
			ErrorCodeLookup[SHSErrorCodes.Code.FriendNotOnline] = ErrorIcons.FriendUnavailable;
			ErrorCodeLookup[SHSErrorCodes.Code.TestModeratorMessage] = ErrorIcons.Moderator;
			ErrorCodeLookup[SHSErrorCodes.Code.TestModerator02Message] = ErrorIcons.Moderator02;
			ErrorCodeLookup[SHSErrorCodes.Code.TestModerator03Message] = ErrorIcons.Moderator03;
			ErrorCodeLookup[SHSErrorCodes.Code.TestModerator04Message] = ErrorIcons.Moderator04;
			ErrorCodeLookup[SHSErrorCodes.Code.TestSorryMessage] = ErrorIcons.Sorry;
			ErrorCodeLookup[SHSErrorCodes.Code.TestUhOh1Message] = ErrorIcons.UhOh1;
			ErrorCodeLookup[SHSErrorCodes.Code.TestUhOh2Message] = ErrorIcons.UhOh2;
			ErrorCodeLookup[SHSErrorCodes.Code.TestUhOh3Message] = ErrorIcons.UhOh3;
			ErrorCodeLookup[SHSErrorCodes.Code.TestWhoopsMessage] = ErrorIcons.Whoops;
			ErrorCodeLookup[SHSErrorCodes.Code.TestWrongMessage] = ErrorIcons.Wrong;
			errorIconsInitialized = true;
		}
	}

	private void initialize(ErrorIconInfo info)
	{
		dialogIcon.Size = info.IconSize;
		iconOffset.x = info.IconOffset.x;
		if (SystemLevelError)
		{
			submitLogButton = new GUIButton();
			submitLogButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, new Vector2(128f, 128f));
			submitLogButton.Id = "LogButton";
			submitLogButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_mshs_button_send");
			submitLogButton.Click += submitLogButton_Click;
			Add(submitLogButton);
		}
	}

	private void submitLogButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		GUIManager.Instance.ShowDialog(typeof(SHSSubmitClientLogWindow), string.Empty, new GUIDialogNotificationSink(delegate
		{
		}, delegate
		{
		}, delegate
		{
		}, delegate
		{
		}, delegate(string Id, DialogState state)
		{
			if (state != DialogState.Cancel)
			{
				Hide();
			}
		}), ModalLevelEnum.Default);
	}

	public override void OnShow()
	{
		type = NotificationType.Error;
		base.OnShow();
		timeOutTimer = 0f;
		GUIControl gUIControl = (GUIControl)this["LogButton"];
		if (gUIControl != null)
		{
			Vector2 position = okButton.Position;
			float x = position.x + 120f;
			Vector2 position2 = okButton.Position;
			gUIControl.SetPosition(x, position2.y);
			gUIControl.Rotation = Rotation;
		}
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (timeOutTimer >= errorMessageDisplayTimeout)
		{
			Hide();
		}
	}

	public static ErrorIconInfo GetErrorIconInfo(SHSErrorCodes.Code errorCode)
	{
		ErrorIcons value;
		if (!ErrorCodeLookup.TryGetValue(errorCode, out value))
		{
			return ErrorIconLookup[ErrorIcons.Wrong];
		}
		ErrorIconInfo value2;
		if (!ErrorIconLookup.TryGetValue(value, out value2))
		{
			return ErrorIconLookup[ErrorIcons.Wrong];
		}
		return value2;
	}

	public static ErrorIconInfo GetErrorIconInfo(ErrorIcons iconType)
	{
		ErrorIconInfo value;
		if (!ErrorIconLookup.TryGetValue(iconType, out value))
		{
			return ErrorIconLookup[ErrorIcons.Wrong];
		}
		return value;
	}

	public static ErrorChoiceInfo GetErrorChoiceInfo(ErrorChoiceType choiceType)
	{
		GenerateErrorChoiceInfo();
		if (errorChoiceLookup.ContainsKey(choiceType))
		{
			return (ErrorChoiceInfo)errorChoiceLookup[choiceType];
		}
		return ErrorChoiceInfo.None;
	}
}
