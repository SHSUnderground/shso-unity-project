using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSBrawlerPlayerBar : GUIControlWindow
{
	public class HealthEnergyBar : SHSBaseLogicBar
	{
		public float windowMax;

		public float windowMin;

		public override void Draw(DrawModeSetting drawFlags)
		{
			base.Draw(drawFlags);
			float percentage = GetPercentage();
			GUIControl gUIControl = Parent as GUIControl;
			if (gUIControl != null)
			{
				if (base.CurrentValue == base.Max)
				{
					gUIControl.SetSize(getAdjustedWidth(percentage) + 10f, 218f);
				}
				else
				{
					gUIControl.SetSize(getAdjustedWidth(percentage), 218f);
				}
			}
		}

		private float getAdjustedWidth(float percentage)
		{
			return (windowMax - windowMin) * percentage + windowMin;
		}
	}

	public class StarContainerAnimation : GUIChildWindow
	{
		public class HeroUpContainer : GUIChildWindow
		{
			private GUIImage heroUp;

			private GUIImage starUp;

			public override bool InitializeResources(bool reload)
			{
				Vector2 heroUpContainerSize = StarContainerAnimation.heroUpContainerSize;
				float x = heroUpContainerSize.x;
				Vector2 heroUpContainerSize2 = StarContainerAnimation.heroUpContainerSize;
				starUp = setupImage("brawler_bundle|mshs_brawler_hud_star_container_5", 0f, -7f, x, heroUpContainerSize2.y);
				Add(starUp);
				Vector2 heroUpContainerSize3 = StarContainerAnimation.heroUpContainerSize;
				float x2 = heroUpContainerSize3.x;
				Vector2 heroUpContainerSize4 = StarContainerAnimation.heroUpContainerSize;
				heroUp = setupImage("brawler_bundle|mshs_brawler_hud_hero_up_container", 0f, -7f, x2, heroUpContainerSize4.y);
				Add(heroUp);
				Traits.HitTestType = HitTestTypeEnum.Transparent;
				Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
				return base.InitializeResources(reload);
			}

			public void AnimateContainer()
			{
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Expected O, but got Unknown
				if (isVisible)
				{
					AnimClip animClip = AnimClipBuilder.Absolute.AnimationAlpha(AnimClipBuilder.Path.Sin(0f, 0.5f, 0.75f), heroUp);
					animClip.OnFinished += (Action)(object)new Action(AnimateContainer);
					base.AnimationPieceManager.Add(animClip);
				}
			}
		}

		public class PowerContainer : GUIChildWindow
		{
			private GUIImage container;

			private AnimClip animationClip;

			public override bool InitializeResources(bool reload)
			{
				Traits.HitTestType = HitTestTypeEnum.Transparent;
				Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
				return base.InitializeResources(reload);
			}

			public void SetContainerImage(GUIImage image)
			{
				container = image;
				Add(image);
			}

			public void AnimateContainer(float elapsedTime)
			{
				if (isVisible)
				{
					AnimateContainer();
					animationClip.ElapsedTime = elapsedTime;
				}
			}

			public float GetAnimationElapsedTime()
			{
				return (animationClip == null) ? 0f : animationClip.ElapsedTime;
			}

			private void AnimateContainer()
			{
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Expected O, but got Unknown
				animationClip = AnimClipBuilder.Absolute.AnimationAlpha(AnimClipBuilder.Path.Sin(0.35f, 0.39f, 1.5f), container);
				animationClip.OnFinished += (Action)(object)new Action(AnimateContainer);
				base.AnimationPieceManager.Add(animationClip);
			}
		}

		public HealthEnergyBar energyBar;

		public float containerScale = 1f;

		protected static readonly int starCount = 5;

		protected static readonly Vector2[] starContainerSizes = new Vector2[5]
		{
			new Vector2(32f, 30f),
			new Vector2(64f, 30f),
			new Vector2(102f, 44f),
			new Vector2(134f, 44f),
			new Vector2(172f, 44f)
		};

		protected static readonly Vector2[] starImageSizes = new Vector2[5]
		{
			new Vector2(32f, 30f),
			new Vector2(64f, 30f),
			new Vector2(172f, 44f),
			new Vector2(172f, 44f),
			new Vector2(172f, 44f)
		};

		protected static readonly Vector2[] starContainerPositions = new Vector2[5]
		{
			new Vector2(7f, 0f),
			new Vector2(7f, 0f),
			new Vector2(0f, -7f),
			new Vector2(0f, -7f),
			new Vector2(0f, -7f)
		};

		protected static readonly float[] starContainerOffsets = new float[4]
		{
			0f,
			32f,
			63f,
			95f
		};

		protected static readonly Vector2 heroUpContainerSize = new Vector2(172f, 44f);

		protected int minPowerMoveStars;

		protected float powerMovePercent = -1f;

		private HeroUpContainer heroUpContainer;

		private List<PowerContainer>[] powerContainerList;

		private float windowMaxSize;

		public float PowerMoveLevel
		{
			get
			{
				return powerMovePercent * (float)minPowerMoveStars;
			}
			set
			{
				powerMovePercent = Mathf.Min(value / energyBar.Max, 1f);
				minPowerMoveStars = (int)Mathf.Ceil(powerMovePercent * (float)starCount);
				minPowerMoveStars = Mathf.Max(minPowerMoveStars, 0);
			}
		}

		public override bool InitializeResources(bool reload)
		{
			powerContainerList = new List<PowerContainer>[starCount];
			for (int i = 0; i < starCount; i++)
			{
				powerContainerList[i] = new List<PowerContainer>();
				int num = i + 1;
				for (int j = 0; j < starCount - num; j += num)
				{
					PowerContainer powerContainer = CreatePowerContainer("brawler_bundle|mshs_brawler_hud_star_container_" + (i + 1), starContainerPositions[i].x + starContainerOffsets[j], starContainerPositions[i].y, starContainerSizes[i].x, starContainerSizes[i].y, starImageSizes[i].x, starImageSizes[i].y);
					powerContainer.IsVisible = false;
					powerContainerList[i].Add(powerContainer);
					Add(powerContainer);
				}
			}
			heroUpContainer = CreateHeroUpContainer();
			heroUpContainer.IsVisible = false;
			Add(heroUpContainer);
			for (int k = 0; k < starCount; k++)
			{
				GUIImage control = setupImage("brawler_bundle|mshs_brawler_hud_star_active", xStarOffsets[k], yStarOffset + 3f, 32f, 30f);
				Add(control);
			}
			Vector2 size = Size;
			windowMaxSize = size.x;
			Vector2 size2 = Size;
			SetSize(0f, size2.y);
			Traits.HitTestType = HitTestTypeEnum.Transparent;
			return base.InitializeResources(reload);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			animateHeroUpContainer();
			animatePowerContainers();
			animateWindowSize();
		}

		public HeroUpContainer CreateHeroUpContainer()
		{
			return GUIControl.CreateControlAbsolute<HeroUpContainer>(heroUpContainerSize * containerScale, Vector2.zero);
		}

		public PowerContainer CreatePowerContainer(string containerImage, float x, float y, float width, float height, float imageWidth, float imageHeight)
		{
			PowerContainer powerContainer = GUIControl.CreateControlAbsolute<PowerContainer>(new Vector2(width * containerScale, height * containerScale), new Vector2(x * containerScale, y * containerScale));
			GUIImage containerImage2 = setupImage(containerImage, 0f, 0f, imageWidth, imageHeight);
			powerContainer.SetContainerImage(containerImage2);
			return powerContainer;
		}

		private void animateHeroUpContainer()
		{
			if (energyBar.CurrentValue >= energyBar.Max)
			{
				if (heroUpContainer.IsVisible)
				{
					return;
				}
				heroUpContainer.IsVisible = true;
				heroUpContainer.AnimateContainer();
				if (powerContainerList.Length >= starCount)
				{
					List<PowerContainer> list = powerContainerList[starCount - 1];
					if (list.Count > 0)
					{
						list[0].IsVisible = true;
					}
				}
			}
			else
			{
				if (!heroUpContainer.IsVisible)
				{
					return;
				}
				heroUpContainer.IsVisible = false;
				if (powerContainerList.Length >= starCount)
				{
					List<PowerContainer> list2 = powerContainerList[starCount - 1];
					if (list2.Count > 0)
					{
						list2[0].IsVisible = false;
					}
				}
			}
		}

		private void animatePowerContainers()
		{
			if (energyBar.CurrentValue < energyBar.Max && energyBar.GetPercentage() >= powerMovePercent && minPowerMoveStars > 0 && minPowerMoveStars < starCount)
			{
				int num = (int)Mathf.Floor((float)starCount * energyBar.GetPercentage() / (float)minPowerMoveStars);
				int num2 = 0;
				if (powerContainerList.Length >= minPowerMoveStars)
				{
					float num3 = 0f;
					List<PowerContainer> list = powerContainerList[minPowerMoveStars - 1];
					foreach (PowerContainer item in list)
					{
						if (num2++ < num)
						{
							if (!item.IsVisible)
							{
								item.IsVisible = true;
								item.AnimateContainer(num3);
							}
							else if (num3 == 0f)
							{
								num3 = item.GetAnimationElapsedTime();
							}
						}
						else
						{
							item.IsVisible = false;
						}
					}
				}
				for (int i = 0; i < powerContainerList.Length; i++)
				{
					if (i != minPowerMoveStars - 1)
					{
						List<PowerContainer> list2 = powerContainerList[i];
						foreach (PowerContainer item2 in list2)
						{
							if (item2.IsVisible)
							{
								item2.IsVisible = false;
							}
						}
					}
				}
			}
			else
			{
				List<PowerContainer>[] array = powerContainerList;
				foreach (List<PowerContainer> list3 in array)
				{
					foreach (PowerContainer item3 in list3)
					{
						if (item3.IsVisible)
						{
							item3.IsVisible = false;
						}
					}
				}
			}
		}

		private void animateWindowSize()
		{
			float num = 0f;
			if (energyBar.CurrentValue >= energyBar.Max)
			{
				num = windowMaxSize;
			}
			else
			{
				PowerContainer powerContainer = null;
				if (minPowerMoveStars > 0 && minPowerMoveStars <= starCount)
				{
					List<PowerContainer> list = powerContainerList[minPowerMoveStars - 1];
					foreach (PowerContainer item in list)
					{
						if (!item.IsVisible)
						{
							break;
						}
						powerContainer = item;
					}
				}
				if (powerContainer != null)
				{
					num = powerContainer.Rect.xMax;
				}
			}
			Vector2 size = Size;
			if (size.x != num)
			{
				float width = num;
				Vector2 size2 = Size;
				SetSize(width, size2.y);
			}
		}
	}

	private const float EMOTE_TIME = 3f;

	private const float networkDisconnectCycleRate = 0.5f;

	public SHSBaseLogicBar healthBar;

	public SHSBaseLogicBar healthDamageBar;

	public SHSBaseLogicBar powerBar;

	public GUIImage portrait;

	public GUIImage networkDisconnect1;

	public GUIImage networkDisconnect2;

	public GUILabel nameBadge;

	public GUIStrokeTextLabel healthText;

	public CharacterStat localPlayerHealth;

	private StarContainerAnimation starContainer;

	protected bool healthbarInitFlag;

	protected bool powerbarInitFlag;

	private string characterName;

	private string squadName;

	private float emoteTimer;

	protected static float creationScale = 1f;

	private bool localBar;

	protected static readonly float[] xStarOffsets = new float[5]
	{
		7f,
		39f,
		70f,
		102f,
		133f
	};

	protected static float yStarOffset = -3f;

	private float networkDisconnectTime;

	public SHSBrawlerPlayerBar(float scale, bool local)
	{
		Traits.ResourceLoadingPhaseTrait = ControlTraits.ResourceLoadingPhaseTraitEnum.Active;
		creationScale = scale;
		localBar = local;
	}

	public void RestartControl()
	{
	}

	public override bool InitializeResources(bool reload)
	{
		Traits.VisibleAncestryTrait = ControlTraits.VisibilityAncestryTraitEnum.DetachedVisibility;
		Add(setupImage("brawler_bundle|mshs_brawler_hud_playerframe", 28f, 10f, 321f, 150f));
		GUIChildWindow gUIChildWindow = new GUIChildWindow();
		gUIChildWindow.SetPosition(181f * creationScale, 57f * creationScale);
		gUIChildWindow.Add(setupImage("brawler_bundle|mshs_brawler_hud_life_damage", 0f, 0f, 156f, 18f));
		HealthEnergyBar healthDamage = CreateHealthEnergyBar(156f, 0f, true, creationScale);
		healthDamage.SHSBarEvent += delegate(object sender, SHSBarEventArgs args)
		{
			if (args.EventType == SHSBarEventArgs.SHSBarEventType.Start)
			{
				healthDamage.UpdateSpeed = Math.Abs(args.Value - args.CurrentValue) * 2f;
			}
		};
		healthDamage.UpdateSpeed = 10f;
		gUIChildWindow.Add(healthDamage);
		Add(gUIChildWindow);
		GUIChildWindow gUIChildWindow2 = new GUIChildWindow();
		gUIChildWindow2.SetPosition(181f * creationScale, 57f * creationScale);
		gUIChildWindow2.Add(setupImage("brawler_bundle|mshs_brawler_hud_life_fill", 0f, 0f, 156f, 18f));
		HealthEnergyBar healthEnergyBar = CreateHealthEnergyBar(156f, 0f, true, creationScale);
		healthEnergyBar.UpdateSpeed = 3000f;
		gUIChildWindow2.Add(healthEnergyBar);
		Add(gUIChildWindow2);
		GUIChildWindow gUIChildWindow3 = new GUIChildWindow();
		gUIChildWindow3.Traits.HitTestType = HitTestTypeEnum.Transparent;
		gUIChildWindow3.SetPosition(170f * creationScale, 80f * creationScale);
		for (int i = 0; i < 5; i++)
		{
			GUIImage control = setupImage("brawler_bundle|mshs_brawler_hud_star_normal", xStarOffsets[i], yStarOffset, 32f, 30f);
			gUIChildWindow3.Add(control);
		}
		HealthEnergyBar healthEnergyBar2 = CreateHealthEnergyBar(161f, 10f, false, creationScale);
		healthEnergyBar2.UpdateSpeed = 70f;
		gUIChildWindow3.Add(healthEnergyBar2);
		healthBar = healthEnergyBar;
		healthDamageBar = healthDamage;
		powerBar = healthEnergyBar2;
		portrait = new GUIImage();
		portrait.SetSize(200f * creationScale, 218f * creationScale);
		portrait.SetPosition(-4f * creationScale, -34f * creationScale);
		portrait.IsVisible = false;
		Add(portrait);
		networkDisconnect1 = new GUIImage();
		networkDisconnect1.SetSize(91f * creationScale, 119f * creationScale);
		networkDisconnect1.SetPosition(57f * creationScale, -3f * creationScale);
		networkDisconnect1.IsVisible = false;
		Texture2D texture;
		if (GUIManager.Instance.LoadTexture("brawler_bundle|mshs_brawler_hud_network_disconnect_1", out texture))
		{
			networkDisconnect1.Texture = texture;
		}
		Add(networkDisconnect1);
		networkDisconnect2 = new GUIImage();
		networkDisconnect2.SetSize(91f * creationScale, 119f * creationScale);
		networkDisconnect2.SetPosition(57f * creationScale, -3f * creationScale);
		networkDisconnect2.IsVisible = false;
		if (GUIManager.Instance.LoadTexture("brawler_bundle|mshs_brawler_hud_network_disconnect_2", out texture))
		{
			networkDisconnect2.Texture = texture;
		}
		Add(networkDisconnect2);
		Add(gUIChildWindow3);
		if (localBar)
		{
			starContainer = CreateStarContainerAnimation(healthEnergyBar2);
			Add(starContainer);
		}
		int num = 14;
		if (creationScale < 0.75f)
		{
			num = 13;
		}
		nameBadge = new GUILabel();
		nameBadge.SetupText(GUIFontManager.SupportedFontEnum.Komica, num, Utils.ColorFromBytes(56, 71, 94, byte.MaxValue), TextAnchor.UpperLeft);
		nameBadge.SetPosition(152f * creationScale, 28f * creationScale);
		nameBadge.SetSize(184f * creationScale, 26f * creationScale);
		nameBadge.IsVisible = false;
		Add(nameBadge);
		if (localBar)
		{
			healthText = new GUIStrokeTextLabel();
			healthText.SetupText(GUIFontManager.SupportedFontEnum.Komica, num + 2, Utils.ColorFromBytes(56, 71, 94, byte.MaxValue), TextAnchor.UpperCenter);
			healthText.SetPosition(162f * creationScale, 55f * creationScale);
			healthText.SetSize(184f * creationScale, 26f * creationScale);
			Add(healthText);
		}
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		return base.InitializeResources(reload);
	}

	public static HealthEnergyBar CreateHealthEnergyBar(float windowMax, float windowMin, bool setMax, float scale)
	{
		HealthEnergyBar healthEnergyBar = new HealthEnergyBar();
		healthEnergyBar.windowMax = windowMax * scale;
		healthEnergyBar.windowMin = windowMin * scale;
		if (setMax)
		{
			healthEnergyBar.InitializeValue(healthEnergyBar.Max);
		}
		else
		{
			healthEnergyBar.InitializeValue(healthEnergyBar.Min);
		}
		return healthEnergyBar;
	}

	public StarContainerAnimation CreateStarContainerAnimation(HealthEnergyBar energyBar)
	{
		StarContainerAnimation starContainerAnimation = GUIControl.CreateControlAbsolute<StarContainerAnimation>(new Vector2(172f * creationScale, 44f * creationScale), new Vector2(170f * creationScale, 77f * creationScale));
		starContainerAnimation.containerScale = creationScale;
		starContainerAnimation.energyBar = energyBar;
		return starContainerAnimation;
	}

	private static GUIImage setupImage(string path, float x, float y, float width, float height)
	{
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetSize(width * creationScale, height * creationScale);
		gUIImage.SetPosition(x * creationScale, y * creationScale);
		gUIImage.TextureSource = path;
		return gUIImage;
	}

	public override void OnActive()
	{
		healthbarInitFlag = false;
		powerbarInitFlag = false;
		healthBar.InitializeValue(healthBar.Value);
		healthDamageBar.InitializeValue(healthDamageBar.Value);
		powerBar.InitializeValue(powerBar.Value);
		AppShell.Instance.EventMgr.AddListener<LocalPlayerChangedMessage>(OnCharacterChanged);
		base.OnActive();
	}

	public override void OnInactive()
	{
		healthBar.InitializeValue(healthBar.Value);
		healthDamageBar.InitializeValue(healthDamageBar.Value);
		powerBar.InitializeValue(powerBar.Value);
		AppShell.Instance.EventMgr.RemoveListener<LocalPlayerChangedMessage>(OnCharacterChanged);
		base.OnInactive();
	}

	private void OnCharacterChanged(LocalPlayerChangedMessage message)
	{
		localPlayerHealth = null;
		UpdateHealth(100f);
	}

	public void SetCharacterName(string CharacterName, string SquadName)
	{
		characterName = CharacterName;
		squadName = SquadName;
		UpdatePortrait(1);
	}

	public override void OnUpdate()
	{
		if (emoteTimer > 0f)
		{
			emoteTimer -= Time.deltaTime;
			if (emoteTimer <= 0f)
			{
				UpdatePortrait(1);
			}
		}
		animateNetworkDisconnect();
		base.OnUpdate();
	}

	public void ChangeHappiness(int happiness)
	{
		UpdatePortrait(happiness);
		emoteTimer = 3f;
	}

	public void UpdateHealth(float healthPercent)
	{
		if (localBar)
		{
			if (localPlayerHealth == null && BrawlerController.Instance != null && BrawlerController.Instance.LocalPlayer != null)
			{
				CharacterStats characterStats = BrawlerController.Instance.LocalPlayer.GetComponentInChildren(typeof(CharacterStats)) as CharacterStats;
				if (characterStats != null)
				{
					localPlayerHealth = characterStats.GetStat(CharacterStats.StatType.Health);
				}
			}
			if (localPlayerHealth != null)
			{
				healthText.Text = string.Format("{0} / {1}", (int)localPlayerHealth.Value, (int)localPlayerHealth.MaximumValue);
				if (localPlayerHealth.Value > 50f)
				{
					healthText.FrontColor = Color.Lerp(Color.yellow, Color.green, localPlayerHealth.Value / localPlayerHealth.MaximumValue * 2f - 1f);
				}
				else
				{
					healthText.FrontColor = Color.Lerp(Color.red, Color.yellow, localPlayerHealth.Value / localPlayerHealth.MaximumValue * 2f);
				}
			}
		}
		if (!healthbarInitFlag)
		{
			healthbarInitFlag = true;
			healthBar.InitializeValue(healthPercent * 100f);
			healthDamageBar.InitializeValue(healthBar.Value);
		}
		else
		{
			healthBar.Value = healthPercent * 100f;
			healthDamageBar.Value = healthBar.Value;
		}
	}

	public void UpdatePower(float powerPercent)
	{
		if (!powerbarInitFlag)
		{
			powerbarInitFlag = true;
			powerBar.InitializeValue(powerPercent * 100f);
		}
		else
		{
			powerBar.Value = powerPercent * 100f;
		}
	}

	public void UpdatePowerMoveLevel(float powerNeeded)
	{
		if (starContainer != null)
		{
			starContainer.PowerMoveLevel = powerNeeded;
		}
	}

	public void UpdatePortrait(int happiness)
	{
		string str = "_HUD_default";
		switch (happiness)
		{
		case 0:
			str = "_HUD_angry";
			break;
		case 2:
			str = "_HUD_happy";
			break;
		}
		if (characterName != string.Empty)
		{
			Texture2D texture;
			if (GUIManager.Instance.LoadTexture("characters_bundle|" + characterName + str, out texture))
			{
				portrait.Texture = texture;
				portrait.IsVisible = true;
			}
		}
		else
		{
			portrait.Texture = null;
		}
		if (squadName != string.Empty)
		{
			nameBadge.Text = squadName;
			nameBadge.IsVisible = true;
		}
		else
		{
			nameBadge.IsVisible = false;
		}
	}

	public void UpdateNetworkDisconnect(bool connected)
	{
		if (connected)
		{
			networkDisconnect1.IsVisible = false;
			networkDisconnect2.IsVisible = false;
			portrait.IsVisible = true;
		}
		else
		{
			networkDisconnect1.IsVisible = true;
			networkDisconnect2.IsVisible = true;
			portrait.IsVisible = false;
		}
	}

	public bool GetNetworkDisconnect()
	{
		return networkDisconnect1.IsVisible;
	}

	private void animateNetworkDisconnect()
	{
		if (!networkDisconnect1.IsVisible)
		{
			return;
		}
		networkDisconnectTime += Time.deltaTime;
		if (networkDisconnectTime > 0.5f)
		{
			networkDisconnectTime = 0f;
			float num = 0f;
			if (networkDisconnect1.Alpha == 1f)
			{
				num = 1f;
			}
			networkDisconnect1.Alpha = 1f - num;
			networkDisconnect2.Alpha = num;
		}
	}
}
