using System;
using UnityEngine;

public class SHSBrawlerBossBar : GUIControlWindow
{
	public static float barScale = 0.6f;

	public static Vector2 bossBarSize = new Vector2(808f, 292f) * barScale;

	private SHSBrawlerPlayerBar.HealthEnergyBar healthDamageLogicBar;

	private SHSBrawlerPlayerBar.HealthEnergyBar healthLogicBar;

	private GUIImage healthBarFrame;

	private GUIImage healthHalfBarFrame;

	private GUIImage bossPortraitFrame;

	private GUIImage bossPortrait;

	private GUIImage bossNameBGR;

	private GUIImage bossNameBGC;

	private GUIImage bossNameBGL;

	private GUILabel bossName;

	private GUILabel bossNameShadow;

	private GUISimpleControlWindow healthDamageBarWindow;

	private GUIImage healthDamageBar;

	private GUIImage healthDamageHalfBar;

	private GUISimpleControlWindow healthBarWindow;

	private GUIImage healthBar;

	private GUIImage healthHalfBar;

	private int bossId;

	private float healthBarScale = 1f;

	private static Vector2 bossNameBGROffset = new Vector2(-256f, 184f) * barScale;

	private static Vector2 bossNameBGRSize = new Vector2(58f, 81f) * barScale;

	private static Vector2 bossNameBGCOffset = new Vector2(-314f, 184f) * barScale;

	private static Vector2 bossNameBGCSize = new Vector2(8f, 81f) * barScale;

	private static Vector2 bossNameBGLOffset = new Vector2(-322f, 184f) * barScale;

	private static Vector2 bossNameBGLSize = new Vector2(108f, 81f) * barScale;

	private static Vector2 bossPortraitFrameOffset = new Vector2(-105f, 98f) * barScale;

	private static Vector2 bossPortraitFrameSize = new Vector2(203f, 194f) * barScale;

	private static Vector2 healthBarSize = new Vector2(548f, 41f) * barScale;

	private static Vector2 healthHalfBarSize = new Vector2(274f, 41f) * barScale;

	private static Vector2 healthBarOffset = new Vector2(-250f, 232f) * barScale;

	private static Vector2 bossNameOffset = new Vector2(-314f, 203f) * barScale;

	private static Vector2 bossNameShadowOffset = new Vector2(-312f, 205f) * barScale;

	private static Vector2 bossNameSize = new Vector2(476f, 40f) * barScale;

	private static Vector2 bossPortraitOffset = new Vector2(-78f, 68f) * barScale;

	private static Vector2 bossPortraitSize = new Vector2(256f, 256f) * barScale;

	public float OffsetX
	{
		get
		{
			return healthBarSize.x * healthBarScale - healthBarOffset.x - 104f * barScale;
		}
	}

	public float OffsetY
	{
		get
		{
			Vector2 size = Size;
			return size.y;
		}
	}

	public int BossId
	{
		get
		{
			return bossId;
		}
	}

	public override bool InitializeResources(bool reload)
	{
		DoNormalLayout();
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		return base.InitializeResources(reload);
	}

	public override void OnShow()
	{
		base.OnShow();
		ShowHealthBars();
	}

	public void DoNormalLayout()
	{
		bossNameBGR = GUIControl.CreateControl<GUIImage>(bossNameBGRSize, bossNameBGROffset, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		bossNameBGR.TextureSource = "brawler_bundle|mshs_brawler_hud_boss_name_right";
		Add(bossNameBGR);
		bossNameBGC = GUIControl.CreateControl<GUIImage>(bossNameBGCSize, bossNameBGCOffset, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		bossNameBGC.TextureSource = "brawler_bundle|mshs_brawler_hud_boss_name_stretch";
		Add(bossNameBGC);
		bossNameBGL = GUIControl.CreateControl<GUIImage>(bossNameBGLSize, bossNameBGLOffset, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		bossNameBGL.TextureSource = "brawler_bundle|mshs_brawler_hud_boss_name_left";
		Add(bossNameBGL);
		bossPortraitFrame = GUIControl.CreateControl<GUIImage>(bossPortraitFrameSize, bossPortraitFrameOffset, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		bossPortraitFrame.TextureSource = "brawler_bundle|mshs_brawler_hud_boss_frame";
		Add(bossPortraitFrame);
		healthBarFrame = GUIControl.CreateControl<GUIImage>(healthBarSize, healthBarOffset, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		healthBarFrame.TextureSource = "brawler_bundle|mshs_brawler_hud_boss_lifebar_bg";
		Add(healthBarFrame);
		healthHalfBarFrame = GUIControl.CreateControl<GUIImage>(healthHalfBarSize, healthBarOffset, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		healthHalfBarFrame.TextureSource = "brawler_bundle|mshs_brawler_hud_boss_lifebar_bg_hlf";
		healthHalfBarFrame.IsVisible = false;
		Add(healthHalfBarFrame);
		healthDamageBarWindow = GUIControl.CreateControl<GUISimpleControlWindow>(healthBarSize, healthBarOffset, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		healthDamageBar = GUIControl.CreateControl<GUIImage>(healthBarSize, Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		healthDamageBar.TextureSource = "brawler_bundle|mshs_brawler_hud_boss_lifebar_damage";
		healthDamageBarWindow.Add(healthDamageBar);
		healthDamageHalfBar = GUIControl.CreateControl<GUIImage>(healthHalfBarSize, Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		healthDamageHalfBar.TextureSource = "brawler_bundle|mshs_brawler_hud_boss_lifebar_damage_hlf";
		healthDamageHalfBar.IsVisible = false;
		healthDamageBarWindow.Add(healthDamageHalfBar);
		healthDamageLogicBar = SHSBrawlerPlayerBar.CreateHealthEnergyBar(healthBarSize.x, 0f, true, 1f);
		healthDamageLogicBar.SHSBarEvent += delegate(object sender, SHSBarEventArgs args)
		{
			if (args.EventType == SHSBarEventArgs.SHSBarEventType.Start)
			{
				healthDamageLogicBar.UpdateSpeed = Math.Abs(args.Value - args.CurrentValue) * 2f;
			}
		};
		healthDamageLogicBar.UpdateSpeed = 10f;
		healthDamageLogicBar.Direction = SHSBaseLogicBar.BarDirection.AscendRight;
		healthDamageBarWindow.Add(healthDamageLogicBar);
		Add(healthDamageBarWindow);
		healthBarWindow = GUIControl.CreateControl<GUISimpleControlWindow>(healthBarSize, healthBarOffset, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		healthBar = GUIControl.CreateControl<GUIImage>(healthBarSize, Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		healthBar.TextureSource = "brawler_bundle|mshs_brawler_hud_boss_lifebar_fill";
		healthBarWindow.Add(healthBar);
		healthHalfBar = GUIControl.CreateControl<GUIImage>(healthHalfBarSize, Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		healthHalfBar.TextureSource = "brawler_bundle|mshs_brawler_hud_boss_lifebar_fill_hlf";
		healthHalfBar.IsVisible = false;
		healthBarWindow.Add(healthHalfBar);
		healthLogicBar = SHSBrawlerPlayerBar.CreateHealthEnergyBar(healthBarSize.x, 0f, true, 1f);
		healthLogicBar.UpdateSpeed = 3000f;
		healthLogicBar.Direction = SHSBaseLogicBar.BarDirection.AscendRight;
		healthBarWindow.Add(healthLogicBar);
		Add(healthBarWindow);
		bossPortrait = GUIControl.CreateControl<GUIImage>(bossPortraitSize, bossPortraitOffset, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		bossPortrait.Alpha = 0f;
		Add(bossPortrait);
		bossNameShadow = GUIControl.CreateControl<GUILabel>(bossNameSize, bossNameShadowOffset, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		bossNameShadow.SetupText(GUIFontManager.SupportedFontEnum.Grobold, (int)(30f * barScale), Utils.ColorFromBytes(0, 21, 105, 128), TextAnchor.UpperRight);
		bossNameShadow.IsVisible = true;
		Add(bossNameShadow);
		bossName = GUIControl.CreateControl<GUILabel>(bossNameSize, bossNameOffset, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		bossName.SetupText(GUIFontManager.SupportedFontEnum.Grobold, (int)(30f * barScale), Utils.ColorFromBytes(58, 71, 94, byte.MaxValue), TextAnchor.UpperRight);
		bossName.IsVisible = true;
		Add(bossName);
	}

	public void UpdateSize(float scale)
	{
		CspUtils.DebugLog("SHSBrawlerBossBar has scale of " + scale + " " + bossPortraitSize * scale);
		SetSize(size.x * scale, size.y * scale);
		bossNameBGCOffset *= scale;
		bossNameBGLOffset *= scale;
		bossNameBGROffset *= scale;
		bossNameBGCSize *= scale;
		bossNameBGLSize *= scale;
		bossNameBGRSize *= scale;
		bossNameOffset *= scale;
		bossNameShadowOffset *= scale;
		bossNameSize *= scale;
		bossPortraitSize *= scale;
		bossNameShadow.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 22, Utils.ColorFromBytes(0, 21, 105, 128), TextAnchor.UpperRight);
		bossName.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 22, Utils.ColorFromBytes(58, 71, 94, byte.MaxValue), TextAnchor.UpperRight);
		healthLogicBar.windowMax *= scale;
		healthDamageLogicBar.windowMax *= scale;
		healthBarOffset *= scale;
		healthBarSize *= scale;
		UpdateControlListSize(this, scale);
		ScaleHealthBar(scale);
	}

	public void ScaleHealthBar(float scale)
	{
		healthBarScale = scale;
		float num = healthBarSize.x * healthBarScale;
		Vector2 size = new Vector2(num, healthBarSize.y);
		healthBarWindow.SetSize(size);
		healthDamageBarWindow.SetSize(size);
		ShowHealthBars();
		healthLogicBar.windowMax = num;
		healthDamageLogicBar.windowMax = num;
	}

	public override void OnActive()
	{
		base.OnActive();
		if (AppShell.Instance != null)
		{
			AppShell.Instance.EventMgr.AddListener<CharacterStat.StatChangeEvent>(OnCharacterStatChange);
		}
	}

	public override void OnInactive()
	{
		base.OnInactive();
		if (AppShell.Instance != null)
		{
			AppShell.Instance.EventMgr.RemoveListener<CharacterStat.StatChangeEvent>(OnCharacterStatChange);
		}
	}

	private void OnCharacterStatChange(CharacterStat.StatChangeEvent e)
	{
		if (e != null && !(e.Character == null) && e.Character.GetInstanceID() == bossId && !(e.Character.GetComponent<BossAIControllerBrawler>() == null) && e.StatType == CharacterStats.StatType.Health)
		{
			healthLogicBar.Value = e.NewValue / e.MaxValue * 100f;
			healthDamageLogicBar.Value = healthLogicBar.Value;
		}
	}

	private void UpdateControlListSize(GUIWindow window, float scale)
	{
		foreach (IGUIControl control in window.ControlList)
		{
			GUIControl gUIControl = (GUIControl)control;
			Vector2 size = gUIControl.Size;
			float width = size.x * scale;
			Vector2 size2 = gUIControl.Size;
			gUIControl.SetSize(width, size2.y * scale);
			Vector2 position = gUIControl.Position;
			float x = position.x * scale;
			Vector2 position2 = gUIControl.Position;
			gUIControl.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(x, position2.y * scale));
			if (gUIControl is GUIWindow)
			{
				UpdateControlListSize((GUIWindow)gUIControl, scale);
			}
		}
	}

	private void ShowHealthBars()
	{
		if (healthBarScale > 0.5f)
		{
			healthBar.IsVisible = true;
			healthDamageBar.IsVisible = true;
			healthBarFrame.IsVisible = true;
			healthHalfBar.IsVisible = false;
			healthDamageHalfBar.IsVisible = false;
			healthHalfBarFrame.IsVisible = false;
		}
		else
		{
			healthBar.IsVisible = false;
			healthDamageBar.IsVisible = false;
			healthBarFrame.IsVisible = false;
			healthHalfBar.IsVisible = true;
			healthDamageHalfBar.IsVisible = true;
			healthHalfBarFrame.IsVisible = true;
		}
	}

	public void SetBoss(GameObject boss)
	{
		bossId = boss.GetInstanceID();
		string text = boss.name;
		CharacterGlobals component = boss.GetComponent<CharacterGlobals>();
		if (component != null && component.definitionData != null && component.definitionData.BossBarIconName != null)
		{
			text = component.definitionData.BossBarIconName;
		}
		CspUtils.DebugLog("SetBoss " + boss.name + " " + text);
		Vector2 b = Vector2.zero;
		bool flag = true;
		Texture2D texture;
		if (!GUIManager.Instance.LoadTexture("characters_bundle|" + text + "_HUD_default", out texture) && !GUIManager.Instance.LoadTexture("characters_bundle|" + text + "_targeted_default", out texture))
		{
			if (!text.Contains("_villain"))
			{
				flag = false;
			}
			else
			{
				string str = text.Substring(0, text.IndexOf("_villain"));
				CspUtils.DebugLog("   " + str + " ");
				if (GUIManager.Instance.LoadTexture("characters_bundle|" + str + "_targeted_default", out texture))
				{
					b = new Vector2(0f, 9f);
				}
				else
				{
					flag = false;
				}
			}
		}
		if (flag)
		{
			bossPortrait.Texture = texture;
			bossPortrait.SetSize((float)bossPortrait.Texture.width * barScale, (float)bossPortrait.Texture.height * barScale);
			Vector2 size = bossPortrait.Size;
			float x = (size.x - bossPortraitSize.x) / 2f + bossPortraitOffset.x;
			float y = bossPortraitSize.y;
			Vector2 size2 = bossPortrait.Size;
			Vector2 offset = new Vector2(x, (y - size2.y) / 2f + bossPortraitOffset.y) + b;
			bossPortrait.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, offset);
			bossPortrait.Alpha = 1f;
		}
		else
		{
			bossPortrait.Alpha = 0f;
		}
		bossName.Text = AppShell.Instance.CharacterDescriptionManager[boss.name].CharacterName;
		bossNameShadow.Text = bossName.Text;
		GUIContent content = new GUIContent(bossName.Text);
		Rect position = new Rect(0f, 0f, bossNameSize.x, bossNameSize.y);
		Vector2 cursorPixelPosition = bossName.Style.UnityStyle.GetCursorPixelPosition(position, content, 0);
		float num = bossNameSize.x - cursorPixelPosition.x;
		num -= 48f * barScale;
		Vector2 position2 = bossNameBGC.Position;
		Vector2 offset2 = bossNameBGR.Offset;
		float num2 = Mathf.Floor(offset2.x);
		Vector2 size3 = bossNameBGR.Size;
		position2.x = num2 - Mathf.Round(size3.x);
		bossNameBGC.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, position2);
		bossNameBGC.SetSize(new Vector2(num, bossNameBGCSize.y));
		position2 = bossNameBGL.Position;
		Vector2 offset3 = bossNameBGC.Offset;
		float num3 = Mathf.Floor(offset3.x);
		Vector2 size4 = bossNameBGC.Size;
		position2.x = num3 - Mathf.Round(size4.x);
		bossNameBGL.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, position2);
		healthLogicBar.Value = 100f;
	}
}
