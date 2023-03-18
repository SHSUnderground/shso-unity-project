using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHSSocialSurvivalModeLeaderboardSelector : GUIDynamicWindow
{
	public class SliderAnimation : SHSAnimations
	{
		public static AnimClip CenterSlider(GUISlider slider, float CenterLocation, bool muted, Action<float> del)
		{
			float mod = (!muted) ? 1f : 0.25f;
			float time = (!muted) ? 0.3f : 0.5f;
			return Custom.Function(GenericPaths.LinearWithMutedSingleWiggle(slider.Value, CenterLocation, mod, time), del);
		}
	}

	public class OwnedMission : MissionCard
	{
		public OwnedMission(string missionId, string missionKey)
			: base(missionId, missionKey, CardType.OwnedMission)
		{
			GUIDrawTexture gUIDrawTexture = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(MissionCard.ModSize(0.85f), Vector2.zero);
			gUIDrawTexture.TextureSource = "brawlergadget_bundle|brawler_gadget_buymission_panel";
			AddItem(gUIDrawTexture);
			GUIDrawTexture gUIDrawTexture2 = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(MissionCard.ModSize(0.85f), Vector2.zero);
			gUIDrawTexture2.TextureSource = "missions_bundle|L_mshs_gameworld_" + missionKey;
			AddItem(gUIDrawTexture2);
			gUIDrawTexture2.Click += delegate
			{
				ClickedOnAMission();
			};
			InitializeRequiredContent();
		}

		public override int SelfCompare(MissionCard other)
		{
			OwnedMission ownedMission = other as OwnedMission;
			if (ownedMission != null)
			{
				return string.Compare(missionKey, ownedMission.missionKey, true);
			}
			return string.Compare(missionKey, other.missionKey, true);
		}
	}

	public abstract class MissionCard : GUISubScalingWindow, IComparable<MissionCard>
	{
		public enum CardType
		{
			MissionOfTheDay,
			OwnedMission,
			UnownedMission
		}

		protected static Rect contentLoadingRect = new Rect(50f, 70f, 180f, 180f);

		protected CardType cardType;

		public string missionId;

		public string missionKey;

		private SHSSocialSurvivalModeLeaderboardSelector missionWindow;

		public SHSSocialSurvivalModeLeaderboardSelector MissionWindow
		{
			set
			{
				missionWindow = value;
			}
		}

		public MissionCard(string missionId, string missionKey, CardType cardType)
			: base(ModSize(1f))
		{
			this.cardType = cardType;
			this.missionId = missionId;
			this.missionKey = missionKey;
			SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
			Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			Traits.VisibleAncestryTrait = ControlTraits.VisibilityAncestryTraitEnum.DetachedVisibility;
		}

		protected void InitializeRequiredContent()
		{
			if (missionKey == LauncherSequences.FixedMissionKey)
			{
				ConfigureRequiredContent(new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.SpecialMission));
			}
			else
			{
				ConfigureRequiredContent(new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.MissionsAndEnemies));
			}
			ContentLoadingCustomDrawRect = contentLoadingRect;
		}

		public override void HandleResize(GUIResizeMessage message)
		{
			base.HandleResize(message);
			ContentLoadingCustomDrawRect = contentLoadingRect;
		}

		public void ClickedOnAMission()
		{
			SHSSocialSurvivalModeLeaderboard dialogWindow = new SHSSocialSurvivalModeLeaderboard(missionId);
			GUIManager.Instance.ShowDynamicWindow(dialogWindow, ModalLevelEnum.Full);
			missionWindow.Hide();
		}

		public void Move(float value)
		{
			float num = Mathf.Abs(value);
			Alpha = Mathf.Clamp01(1.5f / num - 0.75f);
			Offset = new Vector2(Mathf.Sin((float)Math.PI / 180f * (value * 135f)) * 80f, -10f);
			Rotation = Mathf.Sin((float)Math.PI / 180f * (value * 100f)) * 4f;
			IsVisible = (num < 1.1666f);
			if (num < 0.5f)
			{
				ToTheFront();
				missionWindow.FrontMissionKey = missionKey;
				ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter("MissionsComplete");
				if (counter != null)
				{
					missionWindow.TimesPlayedText = counter.GetCurrentValue(missionKey).ToString();
				}
				ISHSCounterType counter2 = AppShell.Instance.CounterManager.GetCounter("MissionsHighestMedal");
				if (counter2 != null)
				{
					long currentValue = counter2.GetCurrentValue(missionKey);
					string medalName = missionWindow.GetMedalName(currentValue);
					missionWindow.MissionMedalTextureSource = "brawlergadget_bundle|brawler_" + medalName + "_small";
				}
			}
		}

		public void ToTheFront()
		{
			Parent.ControlList.Remove(this);
			Parent.ControlList.Add(this);
		}

		public static Vector2 ModSize(float perc)
		{
			return new Vector2(330f, 428f) * 0.9f * perc;
		}

		public abstract int SelfCompare(MissionCard other);

		public int CompareTo(MissionCard other)
		{
			if (cardType == other.cardType)
			{
				return SelfCompare(other);
			}
			return cardType - other.cardType;
		}
	}

	private GUISlider slider;

	private GUIButton okButton;

	private GUIButton backButton;

	private GUISimpleControlWindow contentWindow;

	private GUIImage background;

	private GUIImage missionInfo;

	private GUIImage missionInfoSign;

	private GUIImage bestRank;

	private GUIImage timesPlayed;

	private GUIImage missionMedal;

	private GUIDropShadowTextLabel timesPlayedText;

	private List<MissionCard> missions = new List<MissionCard>();

	private Hashtable medalNameLookups = new Hashtable();

	private string missionToSelect;

	private string frontMissionKey = string.Empty;

	private bool disableAnimation;

	public AnimClip centerAnimation;

	public string MissionMedalTextureSource
	{
		set
		{
			missionMedal.TextureSource = value;
		}
	}

	public string TimesPlayedText
	{
		set
		{
			timesPlayedText.Text = value;
		}
	}

	public string FrontMissionKey
	{
		get
		{
			return frontMissionKey;
		}
		set
		{
			frontMissionKey = value;
		}
	}

	public SHSSocialSurvivalModeLeaderboardSelector()
	{
		medalNameLookups[0L] = "bronze";
		medalNameLookups[1L] = "silver";
		medalNameLookups[2L] = "gold";
		medalNameLookups[3L] = "adamantium";
	}

	public string GetMedalName(long medalValue)
	{
		if (medalNameLookups.ContainsKey(medalValue))
		{
			return (string)medalNameLookups[medalValue];
		}
		return string.Empty;
	}

	public override bool InitializeResources(bool reload)
	{
		contentWindow = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(SHSGadget.CENTER_WINDOW_SIZE, new Vector2(-100f, 0f));
		background = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(791f, 401f), new Vector2(0f, 0f));
		background.TextureSource = "brawlergadget_bundle|brawler_gadget_choosemission_backdrop_new";
		background.Id = "background";
		Vector2 position = new Vector2(GUIManager.ScreenRect.width / 2f, GUIManager.ScreenRect.height / 2.3f);
		SetPositionAndSize(AnchorAlignmentEnum.Middle, position, background.Size + new Vector2(0f, 200f));
		missionInfo = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(168f, 327f), new Vector2(300f, -20f));
		missionInfo.TextureSource = "brawlergadget_bundle|brawler_gadget_mission_info_panel";
		missionInfo.Id = "missionInfo";
		missionInfoSign = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(107f, 60f), new Vector2(300f, -130f));
		missionInfoSign.TextureSource = "brawlergadget_bundle|L_choosemission_missioninfo";
		missionInfoSign.Id = "missionInfoSign";
		bestRank = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(120f, 31f), new Vector2(295f, -70f));
		bestRank.TextureSource = "brawlergadget_bundle|L_choosemission_bestRank";
		bestRank.Id = "bestRank";
		timesPlayed = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(83f, 48f), new Vector2(277f, 38f));
		timesPlayed.TextureSource = "brawlergadget_bundle|L_choosemission_timesplayed_new";
		timesPlayed.Id = "timesPlayed";
		timesPlayedText = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(50f, 100f), new Vector2(285f, 80f));
		timesPlayedText.SetupText(GUIFontManager.SupportedFontEnum.Komica, 32, GUILabel.GenColor(255, 241, 148), TextAnchor.MiddleCenter);
		timesPlayedText.FrontColor = GUILabel.GenColor(255, 241, 148);
		timesPlayedText.BackColor = GUILabel.GenColor(0, 21, 105);
		timesPlayedText.TextOffset = new Vector2(-2f, 3f);
		timesPlayedText.Text = "0";
		timesPlayedText.Id = "timesPlayedText";
		missionMedal = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(80f, 80f), new Vector2(290f, -32f));
		missionMedal.TextureSource = "brawlergadget_bundle|brawler_bronze_small";
		okButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(512f, 512f), new Vector2(-3f, 244f));
		okButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_okbutton_big");
		okButton.HitTestSize = new Vector2(0.8f, 0.25f);
		okButton.Click += delegate
		{
			bool flag = false;
			foreach (MissionCard mission in missions)
			{
				if (mission.missionKey == frontMissionKey)
				{
					flag = true;
					SHSSocialSurvivalModeLeaderboard dialogWindow = new SHSSocialSurvivalModeLeaderboard(mission.missionId);
					GUIManager.Instance.ShowDynamicWindow(dialogWindow, ModalLevelEnum.Full);
					break;
				}
			}
			if (!flag)
			{
			}
			Hide();
		};
		backButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(-224f, 255f));
		backButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_airlock_backbutton");
		backButton.HitTestSize = new Vector2(0.5f, 0.45f);
		backButton.Click += delegate
		{
			Hide();
		};
		RefreshMissionList();
		slider = GUIControl.CreateControlFrameCentered<GUISlider>(new Vector2(732f, 56f), new Vector2(0f, 170f));
		slider.Orientation = GUISlider.SliderOrientationEnum.Horizontal;
		slider.ArrowsEnabled = true;
		slider.Min = 0f;
		slider.Max = missions.Count - 1;
		slider.UseMouseWheelScroll = true;
		slider.ConsrainToMaxAndMin = true;
		slider.MouseScrollWheelAmount = 1f;
		slider.Changed += slider_Changed;
		slider.StartArrow.ToolTip = new NamedToolTipInfo("#shopping_arrows_missions");
		slider.EndArrow.ToolTip = new NamedToolTipInfo("#shopping_arrows_missions");
		slider.ThumbButton.ToolTip = new NamedToolTipInfo("#shopping_button_missions");
		Add(background);
		Add(contentWindow);
		Add(slider);
		Add(missionInfo);
		Add(missionInfoSign);
		Add(bestRank);
		Add(timesPlayed);
		Add(timesPlayedText);
		Add(missionMedal);
		Add(okButton);
		Add(backButton);
		return base.InitializeResources(reload);
	}

	private void OnShoppingItemPurchased(ShoppingItemPurchasedMessage message)
	{
		if (message.ItemType == OwnableDefinition.Category.Mission)
		{
			missionToSelect = message.OwnableName;
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		slider.FireChanged();
		AppShell.Instance.EventMgr.AddListener<MissionFetchCompleteMessage>(OnMissionFetchComplete);
		AppShell.Instance.EventMgr.AddListener<ShoppingItemPurchasedMessage>(OnShoppingItemPurchased);
	}

	public override void OnHide()
	{
		base.OnHide();
		AppShell.Instance.EventMgr.RemoveListener<MissionFetchCompleteMessage>(OnMissionFetchComplete);
		AppShell.Instance.EventMgr.RemoveListener<ShoppingItemPurchasedMessage>(OnShoppingItemPurchased);
	}

	private void OnMissionFetchComplete(MissionFetchCompleteMessage msg)
	{
		RefreshMissionList();
		SelectDefaultMission();
	}

	private void SelectDefaultMission()
	{
		if (missionToSelect != null)
		{
			int i;
			for (i = 0; i < missions.Count && !(missions[i].missionKey == missionToSelect); i++)
			{
			}
			if (i >= missions.Count)
			{
				i = 0;
			}
			missionToSelect = null;
			float value = slider.Value;
			slider.Value = i;
			if ((int)value == i)
			{
				slider.FireChanged();
			}
		}
		else
		{
			slider.Value = 0f;
		}
	}

	private void slider_Changed(GUIControl sender, GUIChangedEvent eventData)
	{
		for (int i = 0; i < missions.Count; i++)
		{
			missions[i].Move((float)i - slider.Value);
		}
		if (slider.IsDragging && !base.AnimationInProgress)
		{
			base.AnimationPieceManager.ClearAll();
			centerAnimation = null;
		}
		if (!slider.IsDragging && !disableAnimation)
		{
			AnimateCenter(Mathf.RoundToInt(slider.Value), false);
		}
	}

	public void AnimateCenter(float CenterLocation, bool muted)
	{
		if (!base.AnimationInProgress)
		{
			base.AnimationPieceManager.ClearAll();
			centerAnimation = SliderAnimation.CenterSlider(slider, CenterLocation, muted, AnimateSlider);
			base.AnimationPieceManager.Add(centerAnimation);
		}
	}

	public void AnimateSlider(float x)
	{
		disableAnimation = true;
		slider.Value = x;
		disableAnimation = false;
	}

	public void RefreshMissionList()
	{
		foreach (MissionCard mission in missions)
		{
			contentWindow.Remove(mission);
		}
		missions.Clear();
		PopulateMissionList();
		foreach (MissionCard mission2 in missions)
		{
			contentWindow.Add(mission2);
		}
	}

	public void PopulateMissionList()
	{
		PopulateMissionListWithAllMissions();
		missions.Sort();
	}

	private void PopulateMissionListWithAllMissions()
	{
		using (Dictionary<string, MissionManifestEntry>.Enumerator enumerator = AppShell.Instance.MissionManifest.GetEnumerator())
		{
			KeyValuePair<string, MissionManifestEntry> missionManifest;
			while (enumerator.MoveNext())
			{
				missionManifest = enumerator.Current;
				string missionKeyFromId = AppShell.Instance.MissionManifest.GetMissionKeyFromId(missionManifest.Key);
				OwnableDefinition def = OwnableDefinition.getDef(Convert.ToInt32(missionManifest.Value.TypeId));
				if (missionKeyFromId.ToLowerInvariant().Contains("skydome") && def != null && def.released == 1)
				{
					MissionCard missionCard = missions.Find(delegate(MissionCard card)
					{
						return card.missionId == missionManifest.Key;
					});
					if (missionCard == null)
					{
						MissionCard missionCard2 = new OwnedMission(missionManifest.Key, AppShell.Instance.MissionManifest[missionManifest.Key].MissionKey);
						missionCard2.MissionWindow = this;
						missions.Add(missionCard2);
					}
				}
			}
		}
	}

	protected override void dispose(bool disposing)
	{
		base.dispose(disposing);
		slider = null;
		okButton = null;
		backButton = null;
		background = null;
		missionInfo = null;
		missionInfoSign = null;
		bestRank = null;
		timesPlayed = null;
		missionMedal = null;
	}
}
