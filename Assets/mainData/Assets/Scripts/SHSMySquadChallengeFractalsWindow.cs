using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class SHSMySquadChallengeFractalsWindow : SHSGadget.GadgetCenterWindow
{
	private struct Entry
	{
		public string player;

		public int balance;

		public int rank;

		public Entry(string _player, int _balance, int _rank)
		{
			player = _player;
			balance = _balance;
			rank = _rank;
		}
	}

	[XmlRoot(ElementName = "bank")]
	public class ServerCounterXml
	{
		[XmlRoot(ElementName = "player_id")]
		public class ServerCounterInfo
		{
			public string counter_type;

			public long value;
		}

		[XmlElement("server_counter")]
		public ServerCounterInfo[] items;
	}

	private const int LEADERBOARD_INFO_X = 370;

	private List<Entry> current = new List<Entry>();

	private List<Entry> recent = new List<Entry>();

	private List<Entry> hallOfFame = new List<Entry>();

	private List<GUIStrokeTextLabel> labels = new List<GUIStrokeTextLabel>();

	private GUIStrokeTextLabel timeRemainingLabel;

	private GUIStrokeTextLabel balanceLabel;

	private GUIStrokeTextLabel rewardLabel;

	private GUIStrokeTextLabel currentButtonLabel;

	private GUIStrokeTextLabel recentButtonLabel;

	private GUIStrokeTextLabel hofButtonLabel;

	private GUIStrokeTextLabel currentBalanceLabel;

	private GUIImage clock;

	private GUIImage itemBox;

	private GUIImage itemImage;

	private GUIButton refreshButton;

	private GUIButton helpButton;

	private int hallOfFameRank;

	private TimeSpan timeLeft;

	private static bool shouldUpdate = true;

	public SHSMySquadChallengeFractalsWindow(MySquadDataManager dataManager)
	{
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetSize(974f, 502f);
		gUIImage.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 17f));
		gUIImage.TextureSource = "mysquadgadget_bundle|mysquad_2panels_challengeandheroespage_01";
		Add(gUIImage);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, ColorUtil.FromRGB255(255, 255, 254), ColorUtil.FromRGB255(0, 65, 157), ColorUtil.FromRGB255(0, 37, 89), new Vector2(-3f, 3f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(168f, -204f), new Vector2(200f, 25f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.Text = "#SQ_MYSQUAD_FRACTAL_LEADERBOARD";
		Add(gUIStrokeTextLabel);
		GUIStrokeTextLabel gUIStrokeTextLabel2 = new GUIStrokeTextLabel();
		gUIStrokeTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, ColorUtil.FromRGB255(255, 255, 254), ColorUtil.FromRGB255(0, 65, 157), ColorUtil.FromRGB255(0, 37, 89), new Vector2(-3f, 3f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel2.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-316f, -205f), new Vector2(200f, 25f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel2.Text = "#SQ_MYSQUAD_FRACTAL_MYFRACTALS";
		Add(gUIStrokeTextLabel2);
		GUIImage gUIImage2 = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(225f, 94f), new Vector2(50f, 160f));
		gUIImage2.TextureSource = "GUI/Notifications/gameworld_pickup_toast_herotokens";
		Add(gUIImage2);
		GUIImage gUIImage3 = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(64f, 64f), new Vector2(63f, 172f));
		gUIImage3.TextureSource = "common_bundle|fractal";
		Add(gUIImage3);
		balanceLabel = GUIControl.CreateControlTopLeftFrame<GUIStrokeTextLabel>(new Vector2(140f, 26f), new Vector2(110f, 192f));
		balanceLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(172, 214, 9), GUILabel.GenColor(0, 23, 67), GUILabel.GenColor(0, 23, 67), new Vector2(3f, 5f), TextAnchor.MiddleCenter);
		balanceLabel.SetVisible(false);
		Add(balanceLabel);
		clock = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(64f, 64f), new Vector2(585f, 142f));
		clock.TextureSource = "mysquadgadget_bundle|mysquad_gadget_clock_small";
		clock.SetVisible(false);
		Add(clock);
		timeRemainingLabel = new GUIStrokeTextLabel();
		timeRemainingLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(172, 214, 9), GUILabel.GenColor(0, 23, 67), GUILabel.GenColor(0, 23, 67), new Vector2(3f, 5f), TextAnchor.MiddleCenter);
		timeRemainingLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(550f, 150f), new Vector2(250f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		timeRemainingLabel.SetVisible(false);
		Add(timeRemainingLabel);
		refreshButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(64f, 64f), new Vector2(775f, 142f));
		refreshButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|button_refresh");
		refreshButton.Click += delegate
		{
			FetchInfo();
			refreshButton.SetVisible(false);
		};
		Add(refreshButton);
		helpButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(64f, 64f), new Vector2(845f, 142f));
		helpButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|button_help");
		helpButton.Click += delegate
		{
			SHSOkDialogWindow sHSOkDialogWindow = new SHSOkDialogWindow();
			sHSOkDialogWindow.TitleText = "#FRACTAL_INSTRUCTIONS_TITLE";
			sHSOkDialogWindow.Text = "#FRACTAL_INSTRUCTIONS";
			GUIManager.Instance.ShowDynamicWindow(sHSOkDialogWindow, ModalLevelEnum.Full);
		};
		Add(helpButton);
		float d = 0.85f;
		itemBox = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(260f, 336f) * d, new Vector2(50f, 250f));
		itemBox.TextureSource = "shopping_bundle|shopping_slideview_ItemBox";
		Add(itemBox);
		itemImage = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(183f, 183f) * d, new Vector2(77f, 313f));
		itemImage.SetVisible(false);
		Add(itemImage);
		rewardLabel = GUIControl.CreateControlTopLeftFrame<GUIStrokeTextLabel>(new Vector2(130f, 26f), new Vector2(90f, 477f));
		rewardLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(64, 64, 64), GUILabel.GenColor(64, 64, 64), new Vector2(3f, 3f), TextAnchor.MiddleCenter);
		rewardLabel.SetVisible(false);
		Add(rewardLabel);
		GUIButton gUIButton = new GUIButton();
		gUIButton.Rect = new Rect(358f, 445f, 151f, 90f);
		gUIButton.Color = new Color(0.117647059f, 48f / 85f, 1f, 1f);
		gUIButton.IsVisible = true;
		gUIButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|stdbutton");
		gUIButton.EntitlementFlag = Entitlements.EntitlementFlagEnum.DemoLimitsOn;
		gUIButton.Click += OnCurrentClicked;
		Add(gUIButton);
		currentButtonLabel = GUIControl.CreateControlTopLeftFrame<GUIStrokeTextLabel>(new Vector2(140f, 26f), new Vector2(358f, 477f));
		currentButtonLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(240, 240, 240), GUILabel.GenColor(0, 23, 67), GUILabel.GenColor(0, 23, 67), new Vector2(3f, 5f), TextAnchor.MiddleCenter);
		currentButtonLabel.Text = "#FRACTALS_CURRENT";
		Add(currentButtonLabel);
		GUIButton gUIButton2 = new GUIButton();
		gUIButton2.Rect = new Rect(552f, 445f, 151f, 90f);
		gUIButton2.Color = new Color(0.117647059f, 48f / 85f, 1f, 1f);
		gUIButton2.IsVisible = true;
		gUIButton2.StyleInfo = new SHSButtonStyleInfo("common_bundle|stdbutton");
		gUIButton2.Click += OnRecentClicked;
		Add(gUIButton2);
		recentButtonLabel = GUIControl.CreateControlTopLeftFrame<GUIStrokeTextLabel>(new Vector2(140f, 26f), new Vector2(555f, 477f));
		recentButtonLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(240, 240, 240), GUILabel.GenColor(0, 23, 67), GUILabel.GenColor(0, 23, 67), new Vector2(3f, 5f), TextAnchor.MiddleCenter);
		recentButtonLabel.Text = "#FRACTALS_RECENT";
		Add(recentButtonLabel);
		GUIButton gUIButton3 = new GUIButton();
		gUIButton3.Rect = new Rect(746f, 445f, 151f, 90f);
		gUIButton3.Color = new Color(0.117647059f, 48f / 85f, 1f, 1f);
		gUIButton3.IsVisible = true;
		gUIButton3.StyleInfo = new SHSButtonStyleInfo("common_bundle|stdbutton");
		gUIButton3.EntitlementFlag = Entitlements.EntitlementFlagEnum.DemoLimitsOn;
		gUIButton3.Click += OnHOFClicked;
		Add(gUIButton3);
		hofButtonLabel = GUIControl.CreateControlTopLeftFrame<GUIStrokeTextLabel>(new Vector2(140f, 26f), new Vector2(750f, 477f));
		hofButtonLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(240, 240, 240), GUILabel.GenColor(0, 23, 67), GUILabel.GenColor(0, 23, 67), new Vector2(3f, 5f), TextAnchor.MiddleCenter);
		hofButtonLabel.Text = "#FRACTALS_HOF";
		Add(hofButtonLabel);
		currentBalanceLabel = AddLabel(new Vector2(370f, 150f), new Vector2(450f, 50f), string.Format(AppShell.Instance.stringTable["#FRACTALS_RECENT_TITLE"], 0));
		currentBalanceLabel.FontSize = 16;
		Add(currentBalanceLabel);
		FetchInfo();
	}

	private GUIStrokeTextLabel AddLabel(Vector2 position, Vector2 size, string text)
	{
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 11, GUILabel.GenColor(172, 214, 9), GUILabel.GenColor(0, 23, 67), GUILabel.GenColor(0, 23, 67), new Vector2(3f, 5f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, position, size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.Text = text;
		Add(gUIStrokeTextLabel);
		return gUIStrokeTextLabel;
	}

	private void SetInfo(List<Entry> data)
	{
		foreach (GUIStrokeTextLabel label in labels)
		{
			Remove(label);
		}
		labels.Clear();
		int num = 200;
		foreach (Entry datum in data)
		{
			GUIStrokeTextLabel gUIStrokeTextLabel = AddLabel(new Vector2(370f, num), new Vector2(450f, 50f), datum.rank + ". " + datum.player);
			if (datum.balance != 0)
			{
				gUIStrokeTextLabel.Text += string.Format(" - {0:n0}", datum.balance);
			}
			if (datum.player == AppShell.Instance.Profile.PlayerName)
			{
				gUIStrokeTextLabel.FrontColor = GUILabel.GenColor(212, 124, 9);
			}
			labels.Add(gUIStrokeTextLabel);
			num += 20;
		}
	}

	private void OnCurrentClicked(GUIControl sender, GUIClickEvent eventArgs)
	{
		SetInfo(current);
		timeRemainingLabel.SetVisible(true);
		clock.SetVisible(true);
		currentBalanceLabel.SetVisible(true);
		refreshButton.SetVisible(true);
	}

	private void OnRecentClicked(GUIControl sender, GUIClickEvent eventArgs)
	{
		SetInfo(recent);
		GUIStrokeTextLabel gUIStrokeTextLabel = AddLabel(new Vector2(370f, 150f), new Vector2(450f, 50f), "#FRACTALS_RECENT");
		gUIStrokeTextLabel.FontSize = 16;
		labels.Add(gUIStrokeTextLabel);
		timeRemainingLabel.SetVisible(false);
		clock.SetVisible(false);
		currentBalanceLabel.SetVisible(false);
		refreshButton.SetVisible(false);
	}

	private void OnHOFClicked(GUIControl sender, GUIClickEvent eventArgs)
	{
		SetInfo(hallOfFame);
		GUIStrokeTextLabel gUIStrokeTextLabel = AddLabel(new Vector2(370f, 150f), new Vector2(450f, 50f), "#FRACTALS_HOF");
		gUIStrokeTextLabel.FontSize = 16;
		labels.Add(gUIStrokeTextLabel);
		if (hallOfFameRank == 0 || hallOfFameRank > 10)
		{
			gUIStrokeTextLabel = AddLabel(new Vector2(685f, 150f), new Vector2(450f, 50f), string.Format(AppShell.Instance.stringTable["#FRACTALS_YOURS"], (hallOfFameRank != 0) ? hallOfFameRank.ToString() : AppShell.Instance.stringTable["#FRACTALS_UNRANKED"]));
			gUIStrokeTextLabel.FontSize = 16;
			labels.Add(gUIStrokeTextLabel);
		}
		timeRemainingLabel.SetVisible(false);
		clock.SetVisible(false);
		currentBalanceLabel.SetVisible(false);
		refreshButton.SetVisible(false);
	}

	private void ClearInfo()
	{
		current.Clear();
		recent.Clear();
		hallOfFame.Clear();
		foreach (GUIStrokeTextLabel label in labels)
		{
			Remove(label);
		}
		labels.Clear();
	}

	private void FetchInfo()
	{
		ClearInfo();
		WWWForm formData = new WWWForm();
		formData.AddField("fractal_type_id", 100);
		AppShell.Instance.WebService.StartRequest("resources$users/get_leaderboard.py", delegate(ShsWebResponse response)
		{
			if (response.Status == 200)
			{
				CspUtils.DebugLog(response.Body);
				AppShell.Instance.WebService.StartRequest("resources$users/get_fractal_bank.py", delegate(ShsWebResponse iresponse)
				{
					if (response.Status == 200)
					{
						CspUtils.DebugLog(iresponse.Body);
						DataWarehouse dataWarehouse5 = new DataWarehouse(iresponse.Body);
						dataWarehouse5.Parse();
						balanceLabel.Text = dataWarehouse5.TryGetString("bank/balance", "0");
						balanceLabel.SetVisible(true);
						currentBalanceLabel.Text = string.Format(AppShell.Instance.stringTable["#FRACTALS_RECENT_TITLE"], dataWarehouse5.TryGetInt("bank/current_balance", 0));
						refreshButton.SetVisible(true);
					}
				}, formData.data);
				DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
				dataWarehouse.Parse();
				int num = 1;
				foreach (DataWarehouse item in dataWarehouse.GetIterator("//fractals/current/leaders/leader"))
				{
					current.Add(new Entry(item.GetString("player"), item.GetInt("balance"), item.TryGetInt("rank", num)));
					num++;
				}
				int num2 = 1;
				foreach (DataWarehouse item2 in dataWarehouse.GetIterator("//fractals/recent/winners/winner"))
				{
					recent.Add(new Entry(item2.GetString("player"), 0, num2));
					num2++;
				}
				foreach (DataWarehouse item3 in dataWarehouse.GetIterator("//fractals/HoF/leaders/leader"))
				{
					hallOfFame.Add(new Entry(item3.GetString("player"), item3.GetInt("balance"), item3.GetInt("rank")));
				}
				hallOfFameRank = dataWarehouse.TryGetInt("//fractals/HoF/current_rank", 0);
				try
				{
					timeLeft = TimeSpan.Parse(dataWarehouse.TryGetString("//fractals/time_left", "0:00:00"));
				}
				catch (FormatException)
				{
					timeLeft = TimeSpan.Parse("0:00:00");
				}
				timeRemainingLabel.Text = timeLeft.ToString();
				timeRemainingLabel.SetVisible(true);
				clock.SetVisible(true);
				OwnableDefinition def = OwnableDefinition.getDef(dataWarehouse.GetInt("//fractals/reward_id"));
				string text = def.name;
				if (def.category == OwnableDefinition.Category.Hero)
				{
					text = string.Format("#CIN_{0}_EXNM", text.ToUpper());
				}
				rewardLabel.Text = AppShell.Instance.stringTable[text];
				rewardLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
				rewardLabel.SetVisible(true);
				itemImage.TextureSource = def.shoppingIcon;
				itemImage.SetVisible(true);
				OnCurrentClicked(null, null);
			}
		}, formData.data);
	}

	public override void Update()
	{
		if (shouldUpdate)
		{
			base.Update();
			if (timeLeft.Hours > 0 || timeLeft.Minutes > 0 || timeLeft.Seconds > 0)
			{
				timeLeft = timeLeft.Subtract(TimeSpan.FromSeconds(Time.deltaTime));
				if (timeRemainingLabel != null)
				{
					timeRemainingLabel.Text = string.Format("{0}:{1}:{2}", timeLeft.Hours.ToString("D2"), timeLeft.Minutes.ToString("D2"), Math.Max(0, timeLeft.Seconds).ToString("D2"));
				}
				if (timeLeft.Hours <= 0 && timeLeft.Minutes <= 0 && timeLeft.Seconds <= 0)
				{
					FetchInfo();
				}
			}
		}
		shouldUpdate = !shouldUpdate;
	}
}
