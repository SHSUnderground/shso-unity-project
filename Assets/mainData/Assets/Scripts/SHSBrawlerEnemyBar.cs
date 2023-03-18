using System;
using UnityEngine;

internal class SHSBrawlerEnemyBar : GUIControlWindow
{
	private class HealthLogicBar : SHSBaseLogicBar
	{
		private float mWinMaxWidth;

		private float mWinMinWidth;

		private GUIChildWindow mContainingWindow;

		public float BarMax
		{
			get
			{
				return mWinMaxWidth;
			}
			set
			{
				mWinMaxWidth = value;
			}
		}

		public float BarMin
		{
			get
			{
				return mWinMinWidth;
			}
			set
			{
				mWinMinWidth = value;
			}
		}

		public HealthLogicBar(GUIChildWindow containingWin, float winMaxWidth, float winMinWidth)
		{
			mContainingWindow = containingWin;
			mWinMaxWidth = winMaxWidth;
			mWinMinWidth = winMinWidth;
			base.Min = 0f;
			base.Max = 1f;
			InitializeValue(base.Max);
		}

		public override void Draw(DrawModeSetting drawFlags)
		{
			base.Draw(drawFlags);
			GUIChildWindow gUIChildWindow = mContainingWindow;
			float width = (mWinMaxWidth - mWinMinWidth) * GetPercentage() + mWinMinWidth;
			Vector2 size = mContainingWindow.Size;
			gUIChildWindow.SetSize(width, size.y);
		}
	}

	public static Vector2 screenOffset = new Vector2(0f, 10f);

	public static Vector3 worldOffset = new Vector3(0f, 0f, 0f);

	public static Vector2 minScreenSize = new Vector2(32f, 16f);

	public static Vector2 maxScreenSize = new Vector2(128f, 16f);

	public static float minScaleDistance = 15f;

	public static float maxScaleDistance = 40f;

	public static float lifeTime = 5f;

	public static float fadeStartTime = 4f;

	private float mLifeStart;

	private bool mFadeStarted;

	private GUIImage mHealthBackImg;

	private GUIChildWindow mHealthBackWin;

	private GUIImage mHealthDmgImg;

	private GUIChildWindow mHealthDmgWin;

	private HealthLogicBar mHealthDmgBar;

	private GUIImage mHealthLifeImg;

	private GUIChildWindow mHealthLifeWin;

	private HealthLogicBar mHealthLifeBar;

	private GUIImage mHealthGleamImg;

	private GUIChildWindow mHealthGleamWin;

	private GameObject mOwner;

	public bool IsAlive
	{
		get
		{
			return mLifeStart > 0f;
		}
	}

	public bool IsFading
	{
		get
		{
			return mFadeStarted;
		}
	}

	public SHSBrawlerEnemyBar()
	{
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		Traits.HitTestType = HitTestTypeEnum.Transparent;
		SetPosition(0f, 0f);
		SetSize(64f, 16f);
		mHealthDmgImg = CreateHealthBarImage("brawler_bundle|enemy_health_damage");
		mHealthDmgWin = CreateHealthBarWindow(mHealthDmgImg);
		GUIChildWindow containingWin = mHealthDmgWin;
		Vector2 size = mHealthDmgWin.Size;
		mHealthDmgBar = new HealthLogicBar(containingWin, size.x, 0f);
		mHealthDmgBar.UpdateSpeed = 10f;
		mHealthDmgBar.SHSBarEvent += delegate(object sender, SHSBarEventArgs args)
		{
			if (args.EventType == SHSBarEventArgs.SHSBarEventType.Start)
			{
				mHealthDmgBar.UpdateSpeed = Math.Abs(args.Value - args.CurrentValue) * 2f;
			}
		};
		mHealthDmgWin.Add(mHealthDmgBar);
		mHealthLifeImg = CreateHealthBarImage("brawler_bundle|enemy_health_full");
		mHealthLifeWin = CreateHealthBarWindow(mHealthLifeImg);
		GUIChildWindow containingWin2 = mHealthLifeWin;
		Vector2 size2 = mHealthLifeWin.Size;
		mHealthLifeBar = new HealthLogicBar(containingWin2, size2.x, 0f);
		mHealthLifeBar.UpdateSpeed = 3000f;
		mHealthLifeWin.Add(mHealthLifeBar);
		mHealthBackImg = CreateHealthBarImage("brawler_bundle|enemy_health_bg");
		mHealthBackWin = CreateHealthBarWindow(mHealthBackImg);
		mHealthGleamImg = CreateHealthBarImage("brawler_bundle|enemy_health_gleam");
		mHealthGleamWin = CreateHealthBarWindow(mHealthGleamImg);
		Add(mHealthBackWin);
		Add(mHealthDmgWin);
		Add(mHealthLifeWin);
		Add(mHealthGleamWin);
		InitializeHealthLogicBars(1f);
	}

	public override void Update()
	{
		base.Update();
		if (IsAlive && mOwner != null)
		{
			Vector3 viewport = Camera.main.WorldToViewportPoint(mOwner.transform.position);
			UpdateScale(viewport.z);
			UpdatePosition(viewport);
			UpdateLifeTime();
		}
	}

	public override void OnActive()
	{
		base.OnActive();
		if (AppShell.Instance != null)
		{
			AppShell.Instance.EventMgr.AddListener<CharacterStat.StatChangeEvent>(OnCharacterStatChange);
			AppShell.Instance.EventMgr.AddListener<EntityDespawnMessage>(OnEntityDespawn);
			AppShell.Instance.EventMgr.AddListener<EntityPolymorphMessage>(OnEntityPolymorph);
			AppShell.Instance.EventMgr.AddListener<EntityFactionChangeMessage>(OnEntityFactionChange);
		}
	}

	public override void OnInactive()
	{
		base.OnInactive();
		if (AppShell.Instance != null)
		{
			AppShell.Instance.EventMgr.RemoveListener<CharacterStat.StatChangeEvent>(OnCharacterStatChange);
			AppShell.Instance.EventMgr.RemoveListener<EntityDespawnMessage>(OnEntityDespawn);
			AppShell.Instance.EventMgr.RemoveListener<EntityPolymorphMessage>(OnEntityPolymorph);
			AppShell.Instance.EventMgr.RemoveListener<EntityFactionChangeMessage>(OnEntityFactionChange);
		}
	}

	public void InitializeHealthLogicBars(float health)
	{
		mHealthDmgBar.InitializeValue(health);
		mHealthLifeBar.InitializeValue(health);
	}

	public void StartLifeTick()
	{
		mLifeStart = Time.time;
		SetAlpha(1f);
		EndFadeTick();
	}

	public void EndLifeTick()
	{
		mLifeStart = 0f;
		EndFadeTick();
	}

	public void StartFadeTick()
	{
		mFadeStarted = true;
		mHealthDmgImg.Alpha = 0f;
	}

	public void EndFadeTick()
	{
		mFadeStarted = false;
		mHealthDmgImg.Alpha = 1f;
	}

	public void SetAlpha(float alpha)
	{
		mHealthBackImg.Alpha = alpha;
		mHealthLifeImg.Alpha = alpha;
		mHealthGleamImg.Alpha = alpha;
	}

	public void AttachHealthBar(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		SpawnData component = obj.GetComponent<SpawnData>();
		if (!(component == null) && (component.spawnType & CharacterSpawn.Type.AI) != 0 && (component.spawnType & CharacterSpawn.Type.Boss) == 0)
		{
			mOwner = obj;
			float health = 1f;
			CombatController component2 = obj.GetComponent<CombatController>();
			if ((bool)component2)
			{
				health = component2.getHealth() / component2.getMaxHealth();
			}
			InitializeHealthLogicBars(health);
			IsVisible = true;
			StartLifeTick();
			Vector3 viewport = Camera.main.WorldToViewportPoint(obj.transform.position);
			UpdateScale(viewport.z);
			UpdatePosition(viewport);
		}
	}

	public void DetachHealthBar()
	{
		if (!(mOwner == null))
		{
			mOwner = null;
			IsVisible = false;
			EndLifeTick();
		}
	}

	public void DetachHealthBar(GameObject obj)
	{
		if (!(mOwner == null) && !(obj != mOwner))
		{
			DetachHealthBar();
		}
	}

	public void TransferHealthBar(GameObject owner, GameObject obj)
	{
		if (!(mOwner != owner))
		{
			AttachHealthBar(obj);
		}
	}

	public static void InitializeHealthBar(DataWarehouse data)
	{
		DataWarehouse data2 = data.GetData("enemy_health_bar_data");
		screenOffset = data2.TryGetVector("screen_offset", screenOffset);
		worldOffset = data2.TryGetVector("world_offset", worldOffset);
		minScreenSize = data2.TryGetVector("min_screen_size", minScreenSize);
		maxScreenSize = data2.TryGetVector("max_screen_size", maxScreenSize);
		minScaleDistance = data2.TryGetFloat("min_scale_distance", minScaleDistance);
		maxScaleDistance = data2.TryGetFloat("max_scale_distance", maxScaleDistance);
		lifeTime = data2.TryGetFloat("life_time", lifeTime);
		fadeStartTime = data2.TryGetFloat("fade_start_time", fadeStartTime);
	}

	private GUIImage CreateHealthBarImage(string healthBarImgPath)
	{
		GUIImage gUIImage = new GUIImage();
		gUIImage.TextureSource = healthBarImgPath;
		gUIImage.SetSize(64f, 16f);
		gUIImage.SetPosition(Vector2.zero);
		return gUIImage;
	}

	private GUIChildWindow CreateHealthBarWindow(GUIImage healthBarImg)
	{
		GUIChildWindow gUIChildWindow = new GUIChildWindow();
		gUIChildWindow.SetSize(64f, 16f);
		gUIChildWindow.SetPosition(Vector2.zero);
		gUIChildWindow.Add(healthBarImg);
		return gUIChildWindow;
	}

	private void UpdatePosition(Vector3 viewport)
	{
		float num = ScreenRect.width / (2f * parent.ScreenRect.width);
		float num2 = ScreenRect.height / (2f * parent.ScreenRect.height);
		float num3 = screenOffset.x / parent.ScreenRect.width;
		float num4 = screenOffset.y / parent.ScreenRect.height;
		float x = (viewport.x - num + num3) * parent.ScreenRect.width;
		float y = (1f - viewport.y + num2 + num4) * parent.ScreenRect.height;
		SetPosition(x, y);
	}

	private void UpdateScale(float disToTarget)
	{
		float num = Mathf.Clamp(disToTarget, minScaleDistance, maxScaleDistance);
		float t = (num - minScaleDistance) / (maxScaleDistance - minScaleDistance);
		Vector2 size = Vector2.Lerp(maxScreenSize, minScreenSize, t);
		SetSize(size);
		mHealthLifeWin.SetSize(size);
		mHealthDmgWin.SetSize(size);
		mHealthBackWin.SetSize(size);
		mHealthGleamWin.SetSize(size);
		mHealthLifeImg.SetSize(size);
		mHealthDmgImg.SetSize(size);
		mHealthBackImg.SetSize(size);
		mHealthGleamImg.SetSize(size);
		mHealthLifeBar.BarMax = size.x;
		mHealthDmgBar.BarMax = size.x;
	}

	private void UpdateLifeTime()
	{
		float num = Time.time - mLifeStart;
		if (!IsFading && num >= fadeStartTime)
		{
			StartFadeTick();
		}
		if (IsFading)
		{
			float value = 1f - (num - fadeStartTime) / (lifeTime - fadeStartTime);
			SetAlpha(Mathf.Clamp(value, 0f, 1f));
		}
		if (num >= lifeTime)
		{
			DetachHealthBar();
		}
	}

	private void OnCharacterStatChange(CharacterStat.StatChangeEvent evt)
	{
		if (evt != null && !(evt.Character == null) && !(mOwner == null) && !(evt.Character != mOwner) && evt.StatType == CharacterStats.StatType.Health)
		{
			mHealthLifeBar.Value = evt.NewValue / evt.MaxValue;
			mHealthDmgBar.Value = mHealthLifeBar.Value;
			StartLifeTick();
		}
	}

	private void OnEntityDespawn(EntityDespawnMessage msg)
	{
		if (msg != null)
		{
			DetachHealthBar(msg.go);
		}
	}

	private void OnEntityPolymorph(EntityPolymorphMessage msg)
	{
		if (msg != null)
		{
			DetachHealthBar(msg.original);
		}
	}

	private void OnEntityFactionChange(EntityFactionChangeMessage msg)
	{
		if (msg == null)
		{
			return;
		}
		GameController controller = GameController.GetController();
		if (!(controller == null) && !(controller.LocalPlayer == null))
		{
			CombatController component = controller.LocalPlayer.GetComponent<CombatController>();
			if (!(component == null) && component.faction == msg.newFaction)
			{
				DetachHealthBar(msg.go);
			}
		}
	}
}
