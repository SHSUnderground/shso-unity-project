using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SHSLeveledUpNotifyWindow : GUIDynamicWindow
{
	protected enum AwardType
	{
		None,
		Item,
		PowerEmote,
		Health,
		Power,
		PowerAttack,
		PowerAttackImprovement,
		HeroUpAttackImprovement
	}

	protected class LevelAwardData
	{
		[CompilerGenerated]
		private AwardType _003Ctype_003Ek__BackingField;

		[CompilerGenerated]
		private string _003Ctext_003Ek__BackingField;

		[CompilerGenerated]
		private string _003Cicon_003Ek__BackingField;

		[CompilerGenerated]
		private Vector2 _003Csize_003Ek__BackingField;

		[CompilerGenerated]
		private Vector2 _003Coffset_003Ek__BackingField;

		public AwardType type
		{
			[CompilerGenerated]
			get
			{
				return _003Ctype_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003Ctype_003Ek__BackingField = value;
			}
		}

		public string text
		{
			[CompilerGenerated]
			get
			{
				return _003Ctext_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003Ctext_003Ek__BackingField = value;
			}
		}

		public string icon
		{
			[CompilerGenerated]
			get
			{
				return _003Cicon_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003Cicon_003Ek__BackingField = value;
			}
		}

		public Vector2 size
		{
			[CompilerGenerated]
			get
			{
				return _003Csize_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003Csize_003Ek__BackingField = value;
			}
		}

		public Vector2 offset
		{
			[CompilerGenerated]
			get
			{
				return _003Coffset_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003Coffset_003Ek__BackingField = value;
			}
		}

		public LevelAwardData(AwardType type, string text, string icon, Vector2 size, Vector2 offset)
		{
			this.type = type;
			this.text = text;
			this.icon = icon;
			this.size = size;
			this.offset = offset;
		}
	}

	protected class LevelData
	{
		public LevelAwardData primaryAward;

		public LevelAwardData secondaryAward;

		[CompilerGenerated]
		private int _003Clevel_003Ek__BackingField;

		public int level
		{
			[CompilerGenerated]
			get
			{
				return _003Clevel_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003Clevel_003Ek__BackingField = value;
			}
		}

		public LevelData(int level, LevelAwardData primaryAward, LevelAwardData secondaryAward)
		{
			this.level = level;
			this.primaryAward = primaryAward;
			this.secondaryAward = secondaryAward;
		}
	}

	private const int textBlockOffset = 25;

	private const float gapDistance = 17f;

	private const float awardIconShrinkPercentage = 0.75f;

	private const float buttonShrinkPercentage = 0.75f;

	private const float okButtonOffset = 25f;

	private const string LEVEL_UP_MSG_PREFIX = "#level_up_msg_";

	private const string POWER_EMOTE_UNLOCKED = "#level_up_new_power_emote_unlocked";

	private LeveledUpMessage levelMessage;

	private GUIImage backgroundLeft;

	private GUIImage backgroundRight;

	private GUIImage backgroundMiddle;

	private GUILabel levelLabel;

	private GUILabel primaryAwardLabel;

	private GUILabel primaryAwardNameLabel;

	private GUIImage primaryAwardIcon;

	private BadgeGUI badgeIcon;

	private GUILabel secondaryAwardLabel;

	private GUIImage secondaryAwardIcon;

	private GUIButton okButton;

	protected static LevelAwardData itemAward = new LevelAwardData(AwardType.Item, "#level_up_item_notification", string.Empty, new Vector2(48f, 48f), Vector2.zero);

	protected static LevelAwardData healthAward = new LevelAwardData(AwardType.Health, "#level_up_health_notification", "persistent_bundle|mshs_levelup_healthupgrade_icon", new Vector2(48f, 48f), new Vector2(-5f, 0f));

	protected static LevelAwardData powerAward = new LevelAwardData(AwardType.Power, "#level_up_power_notification", string.Empty, new Vector2(48f, 48f), new Vector2(-5f, 0f));

	protected static LevelAwardData noAward = new LevelAwardData(AwardType.None, string.Empty, string.Empty, Vector2.zero, Vector2.zero);

	private readonly LevelData[] LevelInformation = new LevelData[39]
	{
		new LevelData(2, healthAward, noAward),
		new LevelData(3, new LevelAwardData(AwardType.PowerAttack, "#level_up_new_power_attack_unlocked", "mysquadgadget_bundle|mysquad_heroinfo_upgrades_power_attack_2", new Vector2(64f, 64f), new Vector2(-7f, -7f)), noAward),
		new LevelData(4, healthAward, noAward),
		new LevelData(5, new LevelAwardData(AwardType.PowerEmote, "#level_up_new_power_emote_unlocked", "communication_bundle|emote_pemote1", new Vector2(85f, 85f), new Vector2(-20f, -20f)), new LevelAwardData(AwardType.PowerAttackImprovement, "#level_up_improved_power_attack_notification", "mysquadgadget_bundle|mysquad_heroinfo_upgrades_power_attack_1plus", new Vector2(64f, 64f), new Vector2(15f, -29f))),
		new LevelData(6, healthAward, noAward),
		new LevelData(7, new LevelAwardData(AwardType.PowerAttack, "#level_up_new_power_attack_unlocked", "mysquadgadget_bundle|mysquad_heroinfo_upgrades_power_attack_3", new Vector2(64f, 64f), new Vector2(-7f, -7f)), noAward),
		new LevelData(8, new LevelAwardData(AwardType.PowerAttackImprovement, "#level_up_improved_power_attack_notification", "mysquadgadget_bundle|mysquad_heroinfo_upgrades_power_attack_2plus", new Vector2(64f, 64f), new Vector2(-7f, -7f)), noAward),
		new LevelData(9, new LevelAwardData(AwardType.PowerAttackImprovement, "#level_up_improved_power_attack_notification", "mysquadgadget_bundle|mysquad_heroinfo_upgrades_power_attack_3plus", new Vector2(64f, 64f), new Vector2(-7f, -7f)), noAward),
		new LevelData(10, new LevelAwardData(AwardType.HeroUpAttackImprovement, "#level_up_improved_hero_up_attack_notification", "mysquadgadget_bundle|mysquad_heroinfo_upgrades_hero_up_2", new Vector2(48f, 48f), Vector2.zero), noAward),
		new LevelData(11, new LevelAwardData(AwardType.PowerEmote, "#level_up_new_power_emote_unlocked", "communication_bundle|emote_pemote2", new Vector2(85f, 85f), new Vector2(-20f, -20f)), noAward),
		new LevelData(12, new LevelAwardData(AwardType.PowerAttackImprovement, "#level_up_improved_power_attack_notification", "mysquadgadget_bundle|mysquad_heroinfo_upgrades_power_attack_1plus2", new Vector2(64f, 64f), new Vector2(-7f, -7f)), noAward),
		new LevelData(13, healthAward, noAward),
		new LevelData(14, healthAward, noAward),
		new LevelData(15, new LevelAwardData(AwardType.PowerAttackImprovement, "#level_up_improved_power_attack_notification", "mysquadgadget_bundle|mysquad_heroinfo_upgrades_power_attack_2plus2", new Vector2(64f, 64f), new Vector2(-7f, -7f)), noAward),
		new LevelData(16, healthAward, noAward),
		new LevelData(17, healthAward, noAward),
		new LevelData(18, new LevelAwardData(AwardType.PowerAttackImprovement, "#level_up_improved_power_attack_notification", "mysquadgadget_bundle|mysquad_heroinfo_upgrades_power_attack_3plus2", new Vector2(64f, 64f), new Vector2(-7f, -7f)), noAward),
		new LevelData(19, healthAward, noAward),
		new LevelData(20, new LevelAwardData(AwardType.HeroUpAttackImprovement, "#level_up_improved_hero_up_attack_notification", "mysquadgadget_bundle|mysquad_heroinfo_upgrades_hero_up_3", new Vector2(48f, 48f), Vector2.zero), noAward),
		new LevelData(21, healthAward, powerAward),
		new LevelData(22, healthAward, powerAward),
		new LevelData(23, healthAward, powerAward),
		new LevelData(24, healthAward, powerAward),
		new LevelData(25, healthAward, powerAward),
		new LevelData(26, healthAward, powerAward),
		new LevelData(27, healthAward, powerAward),
		new LevelData(28, healthAward, powerAward),
		new LevelData(29, healthAward, powerAward),
		new LevelData(30, healthAward, powerAward),
		new LevelData(31, healthAward, powerAward),
		new LevelData(32, healthAward, powerAward),
		new LevelData(33, healthAward, powerAward),
		new LevelData(34, healthAward, powerAward),
		new LevelData(35, healthAward, powerAward),
		new LevelData(36, healthAward, powerAward),
		new LevelData(37, healthAward, powerAward),
		new LevelData(38, healthAward, powerAward),
		new LevelData(39, healthAward, powerAward),
		new LevelData(40, healthAward, powerAward)
	};

	public SHSLeveledUpNotifyWindow(LeveledUpMessage message)
	{
		levelMessage = message;
	}

	public override bool InitializeResources(bool reload)
	{
		if (reload)
		{
			return base.InitializeResources(reload);
		}
		LevelData levelData = null;
		for (int i = 0; i < LevelInformation.Length; i++)
		{
			if (LevelInformation[i].level == levelMessage.NewLevel)
			{
				levelData = LevelInformation[i];
			}
		}
		if (levelData == null)
		{
			CspUtils.DebugLog("Could not find the information for the new level!");
			Hide();
			return false;
		}
		List<GUILabel> list = new List<GUILabel>();
		primaryAwardLabel = new GUILabel();
		primaryAwardLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
		primaryAwardLabel.Text = levelData.primaryAward.text;
		list.Add(primaryAwardLabel);
		primaryAwardIcon = new GUIImage();
		primaryAwardIcon.SetSize(levelData.primaryAward.size);
		if (levelData.primaryAward.type == AwardType.Item)
		{
			string text = null;
			string textureSource = string.Empty;
			ItemDefinition value = null;
			if (levelMessage.OwnableTypeId != null)
			{
				AppShell.Instance.ItemDictionary.TryGetValue(levelMessage.OwnableTypeId, out value);
			}
			if (value != null)
			{
				text = value.Name;
				textureSource = "items_bundle|" + value.Icon;
			}
			primaryAwardIcon.TextureSource = textureSource;
			primaryAwardNameLabel = new GUILabel();
			primaryAwardNameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
			primaryAwardNameLabel.Text = (text ?? "<insert item name here>");
			list.Add(primaryAwardNameLabel);
		}
		else
		{
			primaryAwardIcon.TextureSource = levelData.primaryAward.icon;
		}
		secondaryAwardLabel = new GUILabel();
		secondaryAwardLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
		if (levelData.secondaryAward.type != 0)
		{
			secondaryAwardLabel.Text = levelData.secondaryAward.text;
			list.Add(secondaryAwardLabel);
			secondaryAwardIcon = new GUIImage();
			secondaryAwardIcon.TextureSource = levelData.secondaryAward.icon;
			secondaryAwardIcon.SetSize(levelData.secondaryAward.size);
		}
		else
		{
			secondaryAwardLabel.Text = string.Empty;
			secondaryAwardLabel.IsVisible = false;
		}
		GUIContent gUIContent = new GUIContent();
		levelLabel = new GUILabel();
		levelLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 239, 104), TextAnchor.UpperCenter);
		levelLabel.Text = "#level_up_msg_" + levelMessage.NewLevel;
		levelLabel.IsVisible = true;
		list.Add(levelLabel);
		backgroundLeft = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(57f, 192f), Vector2.zero);
		backgroundLeft.SetPosition(new Vector2(0f, 0f));
		backgroundLeft.TextureSource = "notification_bundle|mshs_levelup_window_left";
		float textBlockSize = GetTextBlockSize(list);
		Vector2 size = primaryAwardIcon.Size;
		backgroundMiddle = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(textBlockSize + size.x, 192f), Vector2.zero);
		GUIImage gUIImage = backgroundMiddle;
		Vector2 position = backgroundLeft.Position;
		float x = position.x;
		Vector2 size2 = backgroundLeft.Size;
		float x2 = x + size2.x;
		Vector2 position2 = backgroundLeft.Position;
		gUIImage.SetPosition(new Vector2(x2, position2.y));
		backgroundMiddle.TextureSource = "notification_bundle|mshs_levelup_window_center";
		backgroundRight = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(54f, 192f), Vector2.zero);
		GUIImage gUIImage2 = backgroundRight;
		Vector2 position3 = backgroundMiddle.Position;
		float x3 = position3.x;
		Vector2 size3 = backgroundMiddle.Size;
		float x4 = x3 + size3.x;
		Vector2 position4 = backgroundMiddle.Position;
		gUIImage2.SetPosition(new Vector2(x4, position4.y));
		backgroundRight.TextureSource = "notification_bundle|mshs_levelup_window_right";
		okButton = new GUIButton();
		okButton.SetSize(new Vector2(96f, 96f));
		GUIButton gUIButton = okButton;
		Vector2 position5 = backgroundMiddle.Position;
		float x5 = position5.x;
		Vector2 size4 = backgroundMiddle.Size;
		float num = x5 + size4.x * 0.5f;
		Vector2 size5 = okButton.Size;
		float x6 = num - size5.x * 0.5f;
		Vector2 position6 = backgroundLeft.Position;
		float y = position6.y;
		Vector2 size6 = backgroundLeft.Size;
		float num2 = y + size6.y;
		Vector2 size7 = okButton.Size;
		gUIButton.SetPosition(new Vector2(x6, num2 - size7.y + 25f));
		okButton.StyleInfo = new SHSButtonStyleInfo("notification_bundle|simple_ok_button");
		bool flag = false;
		if (levelData.level == StatLevelReqsDefinition.MAX_HERO_LEVEL_NORMAL || levelData.level == StatLevelReqsDefinition.MAX_HERO_LEVEL_BADGE1)
		{
			HeroDefinition heroDef = OwnableDefinition.getHeroDef(AppShell.Instance.Profile.SelectedCostume);
			OwnableDefinition badgeDef = null;
			int badgeTier = 1;
			if (levelData.level == StatLevelReqsDefinition.MAX_HERO_LEVEL_BADGE1)
			{
				badgeTier = 2;
			}
			badgeDef = heroDef.getBadgeDef(badgeTier);
			if (badgeDef != null && AppShell.Instance.NewShoppingManager.itemForSale(badgeDef.ownableTypeID) && GameController.GetController() is SocialSpaceController)
			{
				flag = true;
				okButton.Click += delegate
				{
					primaryAwardIcon.IsVisible = false;
					badgeIcon = new BadgeGUI(badgeTier);
					BadgeGUI badgeGUI = badgeIcon;
					Vector2 position16 = primaryAwardIcon.Position;
					float x18 = position16.x - 7.5f;
					Vector2 position17 = primaryAwardIcon.Position;
					badgeGUI.SetPosition(new Vector2(x18, position17.y));
					badgeIcon.SetSize(new Vector2(60f, 60f));
					badgeIcon.Traits.ActivationTrait = ControlTraits.ActivationTraitEnum.Manual;
					badgeIcon.HitTestType = HitTestTypeEnum.Circular;
					badgeIcon.IsVisible = true;
					badgeIcon.setHero(heroDef.name);
					badgeIcon.ownableTypeID = badgeDef.ownableTypeID;
					Add(badgeIcon);
					if (levelData.level == StatLevelReqsDefinition.MAX_HERO_LEVEL_NORMAL)
					{
						primaryAwardLabel.Text = "#TT_HERO_BADGE_NOT_OWNED";
					}
					else
					{
						primaryAwardLabel.Text = "#TT_HERO_BADGE_NOT_OWNED_2";
					}
					GUILabel gUILabel4 = primaryAwardLabel;
					Vector2 size16 = primaryAwardLabel.Size;
					float x19 = size16.x;
					Vector2 size17 = primaryAwardLabel.Size;
					gUILabel4.SetSize(x19, size17.y * 3f);
					okButton.Click += delegate
					{
						ShoppingWindow shoppingWindow = new ShoppingWindow(badgeDef.ownableTypeID);
						shoppingWindow.launch();
						Hide();
					};
				};
			}
		}
		if (!flag)
		{
			okButton.Click += delegate
			{
				Hide();
			};
		}
		gUIContent.text = levelLabel.Text;
		Vector2 vector = levelLabel.Style.UnityStyle.CalcSize(gUIContent);
		GUILabel gUILabel = levelLabel;
		Vector2 position7 = backgroundLeft.Position;
		float x7 = position7.x;
		Vector2 size8 = backgroundLeft.Size;
		float num3 = x7 + size8.x;
		Vector2 size9 = backgroundMiddle.Size;
		float x8 = num3 + (size9.x * 0.5f - vector.x * 0.5f);
		Vector2 position8 = backgroundLeft.Position;
		gUILabel.Position = new Vector2(x8, position8.y + 25f);
		levelLabel.SetSize(new Vector2(vector.x, vector.y));
		Vector2 position9 = backgroundLeft.Position;
		float x9 = position9.x;
		Vector2 size10 = backgroundLeft.Size;
		float x10 = x9 + size10.x;
		Vector2 position10 = backgroundLeft.Position;
		Vector2 vector2 = new Vector2(x10, position10.y + 25f);
		primaryAwardLabel.Position = new Vector2(105f, 45f);
		gUIContent.text = primaryAwardLabel.Text;
		vector = primaryAwardLabel.Style.UnityStyle.CalcSize(gUIContent);
		primaryAwardLabel.SetSize(vector.x, vector.y);
		GUIImage gUIImage3 = primaryAwardIcon;
		float x11 = vector2.x;
		Vector2 offset = levelData.primaryAward.offset;
		float x12 = x11 + offset.x;
		float num4 = vector2.y + vector.y;
		Vector2 offset2 = levelData.primaryAward.offset;
		gUIImage3.Position = new Vector2(x12, num4 + offset2.y);
		Add(backgroundLeft);
		Add(backgroundMiddle);
		Add(backgroundRight);
		Add(levelLabel);
		Add(primaryAwardIcon);
		Add(primaryAwardLabel);
		float y2 = vector.y;
		if (levelData.primaryAward.type == AwardType.Item)
		{
			GUILabel gUILabel2 = primaryAwardNameLabel;
			Vector2 position11 = primaryAwardLabel.Position;
			gUILabel2.Position = new Vector2(110f, position11.y + vector.y);
			gUIContent.text = primaryAwardNameLabel.Text;
			vector = primaryAwardNameLabel.Style.UnityStyle.CalcSize(gUIContent);
			primaryAwardNameLabel.SetSize(new Vector2(vector.x, vector.y));
			Add(primaryAwardNameLabel);
			y2 += vector.y;
		}
		if (levelData.secondaryAward.type != 0)
		{
			GUIImage gUIImage4 = secondaryAwardIcon;
			Vector2 position12 = primaryAwardIcon.Position;
			float x13 = position12.x;
			Vector2 offset3 = levelData.secondaryAward.offset;
			float x14 = x13 + offset3.x;
			Vector2 position13 = primaryAwardIcon.Position;
			float y3 = position13.y;
			Vector2 size11 = primaryAwardIcon.Size;
			float num5 = y3 + size11.y;
			Vector2 offset4 = levelData.secondaryAward.offset;
			gUIImage4.Position = new Vector2(x14, num5 + offset4.y);
			Add(secondaryAwardIcon);
			GUILabel gUILabel3 = secondaryAwardLabel;
			Vector2 position14 = primaryAwardLabel.Position;
			float x15 = position14.x;
			Vector2 position15 = secondaryAwardIcon.Position;
			gUILabel3.Position = new Vector2(x15, position15.y + 10f);
			gUIContent.text = secondaryAwardLabel.Text;
			vector = secondaryAwardLabel.Style.UnityStyle.CalcSize(gUIContent);
			secondaryAwardLabel.SetSize(new Vector2(vector.x, vector.y));
			Add(secondaryAwardLabel);
		}
		Add(okButton);
		Vector2 offset5 = new Vector2(175f, 175f);
		Vector2 size12 = backgroundLeft.Size;
		float x16 = size12.x;
		Vector2 size13 = backgroundMiddle.Size;
		float num6 = x16 + size13.x;
		Vector2 size14 = backgroundRight.Size;
		float x17 = num6 + size14.x;
		Vector2 size15 = backgroundLeft.Size;
		SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, offset5, new Vector2(x17, size15.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		if (levelData.level == StatLevelReqsDefinition.MAX_HERO_LEVEL_BADGE2 && SocialSpaceController.Instance != null && SocialSpaceController.Instance.LocalPlayer != null)
		{
			SocialSpaceController.Instance.LocalPlayer.GetComponent<PlayerCombatController>().changeLevel(levelData.level);
		}
		return base.InitializeResources(reload);
	}

	public override void Hide()
	{
		AppShell.Instance.EventMgr.Fire(this, new LeveledUpAwardHiddenMessage(levelMessage));
		base.Hide();
	}

	protected float GetTextBlockSize(List<GUILabel> textLabels)
	{
		float num = 0f;
		foreach (GUILabel textLabel in textLabels)
		{
			Vector2 vector = textLabel.Style.UnityStyle.CalcSize(new GUIContent(textLabel.Text));
			if (num < vector.x)
			{
				num = vector.x;
			}
		}
		return num;
	}
}
