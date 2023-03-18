using System;
using UnityEngine;

public class SHSLoginWindow : GUIDialogWindow
{
	public GUIDrawTexture bgd1;

	public GUIDrawTexture bgd2;

	public GUIDrawTexture characterGroup;

	public GUIDrawTexture lightRaysTop;

	public GUIDrawTexture lightRaysBottom;

	public GUIDrawTexture loginBackgroundPanel;

	public GUIDrawTexture logoSuperHeroSquadOnline;

	public GUIDrawTexture usernameLabel;

	public GUITextField UserNameTextField;

	public GUIButton rememberMeButton;

	public GUILabel rememberMeButtonInfo;

	public GUIDrawTexture passwordLabel;

	public GUITextField PasswordTextField;

	public GUIDefaultButton OkButton;

	public GUILabel ErrorLabel;

	public GUILabel Iforgot;

	public GUILabel loginStatus;

	public GUIDrawTexture TASLogo;

	public GUIDrawTexture GazLogo;

	public float lightRaysTopRotation = 1f;

	public float lightRaysBottomRotation = 1f;

	public bool rememberMeButtonActive;

	public SHSLoginWindow()
	{
		FontInfo = new GUIFontInfo(GUIFontManager.SupportedFontEnum.Komica, 16);
		SetPosition(new Vector2(0f, 0f), DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
		SetSize(new Vector2(1022f, 644f));
		Traits.LifeSpan = ControlTraits.LifeSpanTraitEnum.KeepAlive;
		bgd1 = new GUIDrawTexture();
		bgd1.SetPositionAndSize(new Vector2(0f, 0f), DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, Vector2.zero, new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		bgd1.TextureSource = "login_bundle|background_1_login_screen";
		bgd1.IsVisible = true;
		bgd1.TooltipKey = string.Empty;
		bgd1.Id = "LoginBgd";
		Add(bgd1);
		Vector2 vector = new Vector2(510f, 170f);
		Vector2 point = new Vector2(510f, 140f);
		lightRaysBottom = new GUIDrawTexture();
		lightRaysBottom.SetSize(1302f, 1302f);
		GUIDrawTexture gUIDrawTexture = lightRaysBottom;
		float x = vector.x;
		Vector2 rectSize = lightRaysBottom.RectSize;
		float x2 = x - rectSize.x * 0.5f;
		float y = vector.y;
		Vector2 rectSize2 = lightRaysBottom.RectSize;
		gUIDrawTexture.SetPosition(x2, y - rectSize2.y * 0.5f);
		lightRaysBottom.TextureSource = "login_bundle|light_rays_bottom_layer";
		lightRaysBottom.IsVisible = true;
		lightRaysBottom.TooltipKey = string.Empty;
		lightRaysBottom.Id = "lightRaysBottom";
		Add(lightRaysBottom);
		lightRaysTop = new GUIDrawTexture();
		lightRaysTop.SetSize(603f, 603f);
		GUIDrawTexture gUIDrawTexture2 = lightRaysTop;
		float x3 = vector.x;
		Vector2 rectSize3 = lightRaysTop.RectSize;
		float x4 = x3 - rectSize3.x * 0.5f;
		float y2 = vector.y;
		Vector2 rectSize4 = lightRaysTop.RectSize;
		gUIDrawTexture2.SetPosition(x4, y2 - rectSize4.y * 0.5f);
		lightRaysTop.TextureSource = "login_bundle|light_rays_top_layer";
		lightRaysTop.IsVisible = true;
		lightRaysTop.TooltipKey = string.Empty;
		Add(lightRaysTop);
		bgd2 = new GUIDrawTexture();
		bgd2.SetPositionAndSize(new Vector2(0f, 0f), DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, Vector2.zero, new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		bgd2.TextureSource = "login_bundle|background_2_login_screen";
		bgd2.IsVisible = true;
		bgd2.TooltipKey = string.Empty;
		Add(bgd2);
		loginBackgroundPanel = new GUIDrawTexture();
		loginBackgroundPanel.SetSize(432f, 464f);
		GUIDrawTexture gUIDrawTexture3 = loginBackgroundPanel;
		Vector2 vector2 = autoCenter(loginBackgroundPanel, point);
		gUIDrawTexture3.SetPosition(vector2.x, point.y - 55f);
		loginBackgroundPanel.TextureSource = "login_bundle|login_background_panel";
		loginBackgroundPanel.IsVisible = true;
		loginBackgroundPanel.TooltipKey = string.Empty;
		Add(loginBackgroundPanel);
		logoSuperHeroSquadOnline = new GUIDrawTexture();
		logoSuperHeroSquadOnline.SetSize(231f, 209f);
		GUIDrawTexture gUIDrawTexture4 = logoSuperHeroSquadOnline;
		Vector2 vector3 = autoCenter(logoSuperHeroSquadOnline, point);
		gUIDrawTexture4.SetPosition(vector3.x, point.y - 135f);
		logoSuperHeroSquadOnline.TextureSource = "login_bundle|logo_superherosquadonline_normal";
		logoSuperHeroSquadOnline.IsVisible = true;
		logoSuperHeroSquadOnline.TooltipKey = string.Empty;
		logoSuperHeroSquadOnline.Id = "logoSuperHeroSquadOnline";
		Add(logoSuperHeroSquadOnline);
		usernameLabel = new GUIDrawTexture();
		usernameLabel.Size = new Vector2(283f, 65f);
		GUIDrawTexture gUIDrawTexture5 = usernameLabel;
		Vector2 vector4 = autoCenter(usernameLabel, point);
		gUIDrawTexture5.Position = new Vector2(vector4.x, point.y + 67f);
		usernameLabel.Id = "usernameLabel";
		usernameLabel.IsVisible = true;
		usernameLabel.TooltipKey = string.Empty;
		usernameLabel.TextureSource = "login_bundle|L_textfield_username_normal";
		Add(usernameLabel);
		UserNameTextField = new GUITextField();
		UserNameTextField.Size = new Vector2(250f, 40f);
		UserNameTextField.Position = new Vector2(usernameLabel.Rect.x + 15f, usernameLabel.Rect.y + 30f);
		UserNameTextField.Text = string.Empty;
		UserNameTextField.MaxLength = 32;
		UserNameTextField.IsVisible = true;
		UserNameTextField.TooltipKey = string.Empty;
		UserNameTextField.ControlName = "UsernameTextField";
		Add(UserNameTextField);
		passwordLabel = new GUIDrawTexture();
		passwordLabel.Size = new Vector2(283f, 65f);
		GUIDrawTexture gUIDrawTexture6 = passwordLabel;
		Vector2 vector5 = autoCenter(passwordLabel, point);
		gUIDrawTexture6.Position = new Vector2(vector5.x, point.y + 142f);
		passwordLabel.IsVisible = true;
		passwordLabel.TooltipKey = string.Empty;
		passwordLabel.TextureSource = "login_bundle|L_textfield_password_normal";
		Add(passwordLabel);
		PasswordTextField = new GUITextField();
		PasswordTextField.Size = new Vector2(250f, 40f);
		PasswordTextField.Position = new Vector2(passwordLabel.Rect.x + 15f, passwordLabel.Rect.y + 30f);
		PasswordTextField.Text = string.Empty;
		PasswordTextField.MaxLength = 32;
		PasswordTextField.IsVisible = true;
		PasswordTextField.TooltipKey = string.Empty;
		PasswordTextField.PasswordFieldEnabled = true;
		PasswordTextField.ControlName = "PasswordTextField";
		Add(PasswordTextField);
		characterGroup = new GUIDrawTexture();
		characterGroup.SetSize(997f, 586f);
		GUIDrawTexture gUIDrawTexture7 = characterGroup;
		Vector2 vector6 = autoCenter(characterGroup, point);
		gUIDrawTexture7.SetPosition(vector6.x + 10f, 52f);
		characterGroup.TextureSource = "login_bundle|character_group";
		characterGroup.IsVisible = true;
		characterGroup.TooltipKey = string.Empty;
		Add(characterGroup);
		ErrorLabel = new GUILabel();
		ErrorLabel.Text = string.Empty;
		ErrorLabel.Color = Color.red;
		ErrorLabel.Size = new Vector2(250f, 100f);
		ErrorLabel.Position = new Vector2(PasswordTextField.Rect.x + 2f, PasswordTextField.Rect.y);
		ErrorLabel.IsVisible = true;
		ErrorLabel.TooltipKey = string.Empty;
		ErrorLabel.StyleInfo = new SHSNamedStyleInfo("ErrorField");
		ErrorLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, Color.red, TextAnchor.UpperCenter);
		Add(ErrorLabel);
		Iforgot = new GUILabel();
		Iforgot.Text = "#forgot";
		Iforgot.Size = new Vector2(230f, 20f);
		GUILabel iforgot = Iforgot;
		Vector2 vector7 = autoCenter(ErrorLabel, point);
		iforgot.Position = new Vector2(vector7.x + 10f, point.y + 342f);
		Iforgot.IsVisible = true;
		Iforgot.TooltipKey = string.Empty;
		Iforgot.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(69, 67, 37), TextAnchor.MiddleCenter);
		Add(Iforgot);
		loginStatus = new GUILabel();
		loginStatus.Size = new Vector2(500f, 20f);
		loginStatus.TextAlignment = TextAnchor.MiddleCenter;
		GUILabel gUILabel = loginStatus;
		Vector2 vector8 = autoCenter(loginStatus, point);
		gUILabel.Position = new Vector2(vector8.x, point.y + 400f);
		loginStatus.IsVisible = true;
		Add(loginStatus);
		rememberMeButton = new GUIButton();
		rememberMeButton.Id = "rememberMeButton";
		rememberMeButton.SetSize(30f, 29f);
		GUIButton gUIButton = rememberMeButton;
		Vector2 vector9 = autoCenter(rememberMeButton, point);
		gUIButton.SetPosition(new Vector2(vector9.x - 40f, point.y + 215f));
		rememberMeButton.IsVisible = true;
		rememberMeButton.ToolTip = new NamedToolTipInfo("#rememberme");
		rememberMeButton.StyleInfo = new SHSButtonStyleInfo("login_bundle|radiobutton_rememberme");
		rememberMeButton.Click += delegate
		{
			rememberMeButtonActive = !rememberMeButtonActive;
			rememberMeButton.IsSelected = rememberMeButtonActive;
			if (rememberMeButtonActive)
			{
				ShsPlayerPrefs.SetInt(ShsPlayerPrefs.Keys.RememberMe, 1);
			}
			else
			{
				ShsPlayerPrefs.SetInt(ShsPlayerPrefs.Keys.RememberMe, 0);
				ShsPlayerPrefs.SetString(ShsPlayerPrefs.Keys.Password, string.Empty);
			}
		};
		Add(rememberMeButton);
		rememberMeButtonInfo = new GUILabel();
		rememberMeButtonInfo.SetSize(100f, 20f);
		GUILabel gUILabel2 = rememberMeButtonInfo;
		Vector2 vector10 = autoCenter(rememberMeButtonInfo, point);
		gUILabel2.SetPosition(new Vector2(vector10.x + 15f, rememberMeButton.Rect.y + 5f));
		rememberMeButtonInfo.Text = "#rememberme";
		Add(rememberMeButtonInfo);
		OkButton = new GUIDefaultButton();
		OkButton.SetSize(122f, 111f);
		GUIDefaultButton okButton = OkButton;
		Vector2 vector11 = autoCenter(OkButton, point);
		okButton.SetPosition(new Vector2(vector11.x, point.y + 236f));
		OkButton.Rotation = 0f;
		OkButton.Color = new Color(1f, 1f, 1f, 1f);
		OkButton.IsVisible = true;
		OkButton.StyleInfo = new SHSButtonStyleInfo("login_bundle|L_button_login");
		OkButton.Click += delegate
		{
			if(rememberMeButtonActive)
			{
				setRememberMe();	
			}
			AppShell.Instance.ServerConnection.Login(UserNameTextField.Text.Trim(), PasswordTextField.Text.Trim());
			OkButton.IsEnabled = false;
		};
		OkButton.HitTestType = HitTestTypeEnum.Circular;
		Add(OkButton);
		TASLogo = new GUIDrawTexture();
		TASLogo.SetSize(104f, 52f);
		TASLogo.SetPosition(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, OffsetType.Absolute, new Vector2(-10f, -10f));
		TASLogo.TextureSource = "login_bundle|logo_theamazingsociety_normal";
		Add(TASLogo);
		GazLogo = new GUIDrawTexture();
		GazLogo.SetSize(103f, 41f);
		GazLogo.SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(10f, -10f));
		GazLogo.TextureSource = "login_bundle|logo_gazillionentertainment_normal";
		Add(GazLogo);
		Traits.VisibleAncestryTrait = ControlTraits.VisibilityAncestryTraitEnum.EnsureAncestorsVisible;
	}

	protected override void InitializeBundleList()
	{
		//CspUtils.DebugLog("InitializeBundleList");

		base.InitializeBundleList();
		if ((Application.platform != RuntimePlatform.OSXWebPlayer && Application.platform != RuntimePlatform.WindowsWebPlayer) || inAutomationMode())
		{
			supportingAssetBundles.Add(new SupportingAssetBundleInfo("login_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		}
	}
	
	private void setRememberMe(){
	
		ShsPlayerPrefs.SetString(ShsPlayerPrefs.Keys.UserName, UserNameTextField.Text);
		ShsPlayerPrefs.SetString(ShsPlayerPrefs.Keys.Password, PasswordTextField.Text);
		
	}

	private bool inAutomationMode()
	{
		//CspUtils.DebugLog("inAutomationMode");

		bool result = false;
		string value;
		if (AppShell.Instance.WebService.UrlParameters.TryGetValue("auto", out value))
		{
			result = value.Equals("true", StringComparison.OrdinalIgnoreCase);
		}
		return result;
	}

	public Vector2 autoCenter(GUIControl control, Vector2 point)
	{
		//CspUtils.DebugLog("autoCenter");

		float x = point.x;
		Vector2 rectSize = control.RectSize;
		float x2 = x - rectSize.x * 0.5f;
		float y = point.y;
		Vector2 rectSize2 = control.RectSize;
		return new Vector2(x2, y - rectSize2.y * 0.5f);
	}

	public bool isMouseIsOver(GUIControl toBeOver)
	{
		//CspUtils.DebugLog("isMouseIsOver");

		if (toBeOver.Rect.Contains(Event.current.mousePosition))
		{
			return true;
		}
		return false;
	}

	public override void Draw(DrawModeSetting drawFlags)
	{

		if (GUIManager.Instance.FocusManager.hasFocus(UserNameTextField))
		{
			usernameLabel.TextureSource = "login_bundle|L_textfield_username_pressed";
		}
		else
		{
			usernameLabel.TextureSource = "login_bundle|L_textfield_username_normal";
		}
		if (GUIManager.Instance.FocusManager.hasFocus(PasswordTextField))
		{
			passwordLabel.TextureSource = "login_bundle|L_textfield_password_pressed";
		}
		else
		{
			passwordLabel.TextureSource = "login_bundle|L_textfield_password_normal";
		}
		if (isMouseIsOver(Iforgot))
		{
			Iforgot.FontSize = 15;
		}
		else
		{
			Iforgot.FontSize = 14;
		}
		if (isMouseIsOver(rememberMeButton))
		{
		}
		if (Event.current.isKey)
		{
			ErrorLabel.Text = string.Empty;
		}
		if (ErrorLabel.Text != string.Empty)
		{
			PasswordTextField.Text = string.Empty;
		}
		base.Draw(drawFlags);
	}

	public override void OnActive()
	{
		//CspUtils.DebugLog("OnActive");

		base.OnActive();
		AppShell.Instance.EventMgr.AddListener<LoginCompleteMessage>(OnLoginCompleted);
		AppShell.Instance.EventMgr.AddListener<LoginStatusMessage>(OnLoginStatusMessage);
		if (!Application.isEditor)
		{
			if (ShsPlayerPrefs.HasKey(ShsPlayerPrefs.Keys.UserName))
			{
				UserNameTextField.Text = ShsPlayerPrefs.GetString(ShsPlayerPrefs.Keys.UserName);
			}
			if (ShsPlayerPrefs.HasKey(ShsPlayerPrefs.Keys.RememberMe) && ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.RememberMe) == 1 && ShsPlayerPrefs.HasKey(ShsPlayerPrefs.Keys.Password))
			{
				PasswordTextField.Text = ShsPlayerPrefs.GetString(ShsPlayerPrefs.Keys.Password);
			}
			if (ShsPlayerPrefs.HasKey(ShsPlayerPrefs.Keys.RememberMe) && ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.RememberMe) == 1)
			{
				rememberMeButtonActive = true;
				rememberMeButton.IsSelected = rememberMeButtonActive;
			}
		}
		if (string.IsNullOrEmpty(UserNameTextField.Text))
		{
			GUIManager.Instance.SetKeyboardFocus(UserNameTextField.ControlName);
		}
		else if (string.IsNullOrEmpty(PasswordTextField.Text))
		{
			GUIManager.Instance.SetKeyboardFocus(PasswordTextField.ControlName);
		}
		GUIManager.Instance.SetScreenBackColor(new Color(0f, 0f, 0f, 1f));
		OkButton.IsEnabled = true;
	}

	public override void OnInactive()
	{
		//CspUtils.DebugLog("OnInactive");

		base.OnInactive();
		AppShell.Instance.EventMgr.RemoveListener<LoginCompleteMessage>(OnLoginCompleted);
		AppShell.Instance.EventMgr.RemoveListener<LoginStatusMessage>(OnLoginStatusMessage);
		GUIManager.Instance.SetScreenBackColor(false);
	}

	protected override void OnEscape(SHSKeyCode code)
	{
		//CspUtils.DebugLog("OnEscape");
	}

	protected override void OnEnter(SHSKeyCode code)
	{
		//CspUtils.DebugLog("OnEnter");

		UserNameTextField.Text = UserNameTextField.Text.TrimEnd();
		PasswordTextField.Text = PasswordTextField.Text.TrimEnd();
		base.OnEnter(code);
	}

	private void OnLoginCompleted(LoginCompleteMessage message)
	{
		//CspUtils.DebugLog("OnLoginCompleted");

		if (message.status == LoginCompleteMessage.LoginStatus.LoginSucceeded)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
			{
				ShsPlayerPrefs.SetString(ShsPlayerPrefs.Keys.UserName, UserNameTextField.Text);
				if (ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.RememberMe) == 1)
				{
					ShsPlayerPrefs.SetString(ShsPlayerPrefs.Keys.Password, PasswordTextField.Text);
				}
			}
			AppShell.Instance.SharedHashTable["SocialSpaceLevel"] = "Daily_Bugle";
			GameController.ControllerType @int = (GameController.ControllerType)PlayerPrefs.GetInt("startupscene", 8);
			AppShell.Instance.Transition(@int);
			GUIManager.Instance.ClearKeyboardFocus();
		}
		else
		{
			CspUtils.DebugLog("Login complete with status 'failed': " + message.message);
			ErrorLabel.Text = message.message;
			ErrorLabel.IsVisible = true;
			OkButton.IsEnabled = true;
		}
	}

	private void OnLoginStatusMessage(LoginStatusMessage message)
	{
		//CspUtils.DebugLog("OnLoginStatusMessage");

		loginStatus.Text = message.Message;
	}

	public override void ConfigureKeyBanks()
	{
		//CspUtils.DebugLog("ConfigureKeyBanks");

		base.ConfigureKeyBanks();
		keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.Return, false, false, false), OnEnter);
		keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.Escape, false, false, false), OnOptions);
	}

	private void OnDebugMinimized(SHSKeyCode code)
	{
		//CspUtils.DebugLog("OnDebugMinimized");
	}

	private void OnDebug(SHSKeyCode code)
	{
		//CspUtils.DebugLog("OnDebug");

	}

	public void OnOptions(SHSKeyCode code)
	{
		//CspUtils.DebugLog("OnOptions");

		if (!SHSOptionsGadget.OptionsCurrentlyShowing)
		{
			GUIManager.Instance.ShowDialog(typeof(SHSOptionsGadget), delegate
			{
			}, ModalLevelEnum.Default);
		}
	}
}
