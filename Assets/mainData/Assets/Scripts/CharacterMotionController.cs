using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(BehaviorManager))]
public class CharacterMotionController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public delegate void MotionCallback();

	protected float totalRotation;

	public MotionCallback landCallback;

	public MotionCallback airborneCallback;

	public float cameraForceFollowTime;

	private static float immediateTeleportDistanceFactor = 1f;

	private static float motionlessTeleportDistanceFactor = 0.2f;

	private static float motionlessTimeout = 0.5f;

	private static readonly float shortJumpCutoffTime = 0.3f;

	private static readonly float standardJumpHeight = 10f;

	public static readonly float minimumMoveDistance = 0.05f;

	public float speed = 6f;

	protected float baseSpeed;

	protected float speedMultiplier = 1f;

	protected float runSpeedMultiplier = 1f;

	public float runAnimationSpeed = 1f;

	public bool checkForNetCatchups = true;

	public float decelerationDistance = 0.4f;

	public float gravity = 20f;

	public float gravityTerminalVelocity = 20f;

	private float _jumpHeight = 10f;

	private bool useJumpSequence;

	public float secondJumpHeight = 10f;

	private bool _doubleJump;

	public int doubleJumpAnimIndex = -1;

	private bool _holdJump;

	private EffectSequence glideSequence;

	private bool DataCanGlide;

	private bool _canGlide;

	public float jumpTimeout = 0.15f;

	protected float _pickupStrength = 5f;

	public float rotateSpeed = 500f;

	public float fallTolerance = 0.5f;

	public HotSpotType.Style hotSpotType;

	protected float maximumAutomaticSpeed = 20f;

	protected CharacterGlobals charGlobals;

	protected ShsCharacterController characterController;

	protected BehaviorManager behaviorManager;

	protected Transform motionExportTransform;

	protected Vector3 motionExportPosition;

	protected float motionExportRotation;

	protected float verticalVelocity;

	protected Vector3 destination;

	protected GameObject destinationObject;

	protected float destinationObjectDistance;

	protected Vector3 destinationFacing = Vector3.zero;

	protected Vector3 lookDirection;

	protected Vector3 lastVelocity;

	protected Vector3 forcedVelocity;

	protected float forcedVelocityTime;

	protected bool forcedVelocityAllowed = true;

	protected Vector3 extraVelocity;

	protected float jumpPressedTime = -1f;

	protected float lastJumpPressedTime = -1f;

	protected float lastJumpStartElevation;

	protected bool jumping;

	protected bool doubleJumping;

	protected bool gliding;

	protected bool inMotion;

	protected float motionlessTime;

	protected float distanceToDestination;

	protected bool isOnGround = true;

	protected bool wasOnGround = true;

	protected bool lastFrameOnGround = true;

	protected float currentJumpCancelForce;

	protected float delayedRotateAngle;

	protected int delayedRotateDelay;

	public float positionSampleRate = 0.1f;

	public float positionSyncRate = 1f;

	protected float minimumPositionRate = 4f;

	protected float positionLastSentTime;

	protected float positionFullLastSentTime;

	protected Vector3 positionLastSent;

	private NetworkComponent networkComponent;

	private SpawnData spawnData;

	private SpawnAnimateData spawnAnimation;

	protected string landingEffectName;

	protected GameObject landingEffectPrefab;

	protected string doublejumpEffectName;

	protected GameObject doublejumpEffectPrefab;

	protected string fallingEffectName;

	protected GameObject fallingEffectPrefab;

	protected GameObject fallingEffectInstance;

	private CollisionFlags LastCollisionFlags;

	public ThrowableCarry carriedThrowable;

	protected Vector3 lastGoodPosition = Vector3.zero;

	protected float lastGoodPositionOffset;

	protected int scaleAdjustments;

	protected Transform exportTransform;

	protected HashSet<float> speedMultiplierList = new HashSet<float>();

	protected float timeToEnableCollision = -1f;

	protected bool trackCollisions;

	protected List<ControllerColliderHit> collidersHit;

	protected bool restoreCameraOnLand;

	protected int debugTeleportType;

	protected Vector3 debugTeleportLoc;

	[CompilerGenerated]
	private string _003CSwingData_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CCanSwing_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CSitOverride_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CNetUpdatesDisabled_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CEnemyCollisionsDisabled_003Ek__BackingField;

	public float jumpHeight
	{
		get
		{
			if (charGlobals != null && charGlobals.activeSidekick != null && charGlobals.activeSidekick.motionController.holdJump)
			{
				return charGlobals.activeSidekick.motionController.jumpHeight;
			}
			return _jumpHeight;
		}
		set
		{
			_jumpHeight = value;
		}
	}

	public bool UseJumpSequence
	{
		get
		{
			return useJumpSequence;
		}
	}

	public bool doubleJump
	{
		get
		{
			if (_doubleJump)
			{
				return true;
			}
			if (charGlobals != null && charGlobals.activeSidekick != null && charGlobals.activeSidekick.motionController.doubleJump)
			{
				return true;
			}
			return false;
		}
		set
		{
			_doubleJump = value;
		}
	}

	public bool holdJump
	{
		get
		{
			if (_holdJump)
			{
				return true;
			}
			if (charGlobals != null && charGlobals.activeSidekick != null && charGlobals.activeSidekick.motionController.holdJump)
			{
				return true;
			}
			return false;
		}
		set
		{
			_holdJump = value;
		}
	}

	public bool CanGlide
	{
		get
		{
			if (_canGlide)
			{
				return true;
			}
			if (charGlobals != null && charGlobals.activeSidekick != null && charGlobals.activeSidekick.motionController.CanGlide)
			{
				return true;
			}
			return false;
		}
		set
		{
			_canGlide = (value || DataCanGlide);
		}
	}

	public float pickupStrength
	{
		get
		{
			if (charGlobals != null && charGlobals.activeSidekick != null && charGlobals.activeSidekick.motionController.pickupStrength > _pickupStrength)
			{
				return charGlobals.activeSidekick.motionController.pickupStrength;
			}
			return _pickupStrength;
		}
		set
		{
			_pickupStrength = value;
		}
	}

	public string SwingData
	{
		[CompilerGenerated]
		get
		{
			return _003CSwingData_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CSwingData_003Ek__BackingField = value;
		}
	}

	public bool CanSwing
	{
		[CompilerGenerated]
		get
		{
			return _003CCanSwing_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CCanSwing_003Ek__BackingField = value;
		}
	}

	public string SitOverride
	{
		[CompilerGenerated]
		get
		{
			return _003CSitOverride_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CSitOverride_003Ek__BackingField = value;
		}
	}

	public bool NetUpdatesDisabled
	{
		[CompilerGenerated]
		get
		{
			return _003CNetUpdatesDisabled_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CNetUpdatesDisabled_003Ek__BackingField = value;
		}
	}

	public Vector3 ExtraVelocity
	{
		get
		{
			return extraVelocity;
		}
		set
		{
			extraVelocity = value;
		}
	}

	public float LastJumpDuration
	{
		get
		{
			float num = (!doubleJumping) ? jumpHeight : secondJumpHeight;
			return num / ((gravity != 0f) ? gravity : 1f);
		}
	}

	public bool DoubleJumping
	{
		get
		{
			return doubleJumping;
		}
		set
		{
			doubleJumping = value;
		}
	}

	public bool EnemyCollisionsDisabled
	{
		[CompilerGenerated]
		get
		{
			return _003CEnemyCollisionsDisabled_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CEnemyCollisionsDisabled_003Ek__BackingField = value;
		}
	}

	public float JumpPotential
	{
		get
		{
			return GetJumpPotential(jumpHeight);
		}
	}

	public float SecondJumpPotential
	{
		get
		{
			return GetJumpPotential(secondJumpHeight);
		}
	}

	private void Start()
	{
		charGlobals = (GetComponent(typeof(CharacterGlobals)) as CharacterGlobals);
		characterController = charGlobals.characterController;
		behaviorManager = charGlobals.behaviorManager;
		setDestination(base.transform.position);
		lookDirection = base.transform.TransformDirection(Vector3.forward);
		speed = baseSpeed * speedMultiplier;
		updateRunAnimationSpeed();
		Animation animationComponent = charGlobals.animationComponent;
		if (animationComponent["movement_run"] != null)
		{
			animationComponent["movement_run"].wrapMode = WrapMode.Loop;
		}
		if (animationComponent["movement_idle"] != null)
		{
			animationComponent["movement_idle"].wrapMode = WrapMode.Loop;
		}
		if (animationComponent["pickup_run"] != null)
		{
			animationComponent["pickup_run"].wrapMode = WrapMode.Loop;
		}
		if (animationComponent["pickup_idle"] != null)
		{
			animationComponent["pickup_idle"].wrapMode = WrapMode.Loop;
		}
		if (animationComponent["jump_up"] != null)
		{
			animationComponent["jump_up"].wrapMode = WrapMode.ClampForever;
		}
		if (animationComponent["jump_up_2"] != null)
		{
			animationComponent["jump_up_2"].wrapMode = WrapMode.ClampForever;
		}
		if (animationComponent["jump_run_up"] != null)
		{
			animationComponent["jump_run_up"].wrapMode = WrapMode.ClampForever;
		}
		if (animationComponent["jump_run_land"] != null)
		{
			animationComponent["jump_run_land"].wrapMode = WrapMode.ClampForever;
		}
		if (animationComponent["jump_run_fall"] != null)
		{
			animationComponent["jump_run_fall"].wrapMode = WrapMode.ClampForever;
		}
		if (animationComponent["idle_to_run"] != null)
		{
			animationComponent["idle_to_run"].wrapMode = WrapMode.ClampForever;
		}
		if (animationComponent["run_to_idle"] != null)
		{
			animationComponent["run_to_idle"].wrapMode = WrapMode.ClampForever;
		}
		if (animationComponent["jump_fall"] != null)
		{
			animationComponent["jump_fall"].wrapMode = WrapMode.ClampForever;
		}
		if (animationComponent["jump_land"] != null)
		{
			animationComponent["jump_land"].wrapMode = WrapMode.ClampForever;
		}
		if (animationComponent["jump_fall_loop"] != null)
		{
			animationComponent["jump_fall_loop"].wrapMode = WrapMode.Loop;
		}
		if (animationComponent["sleep_idle"] != null)
		{
			animationComponent["sleep_idle"].wrapMode = WrapMode.Loop;
		}
		if (animationComponent["sleep_wake_up"] != null)
		{
			animationComponent["sleep_wake_up"].wrapMode = WrapMode.ClampForever;
		}
		networkComponent = charGlobals.networkComponent;
		spawnData = charGlobals.spawnData;
		positionLastSentTime = -1f;
		positionFullLastSentTime = -1f;
		positionLastSent = Vector3.zero;
		landCallback = (MotionCallback)Delegate.Combine(landCallback, new MotionCallback(CheckCameraRestore));
		if (BrawlerController.Instance != null)
		{
			airborneCallback = (MotionCallback)Delegate.Combine(airborneCallback, new MotionCallback(FreezeCameraInAir));
		}
		StartCoroutine(DelayedSpawnAnimation());
		trackCollisions = false;
		collidersHit = new List<ControllerColliderHit>();
		AppShell.Instance.EventMgr.AddListener<LeveledUpMessage>(UpdateCanGlide);
	}

	public void OnDestroy()
	{
		AppShell.Instance.EventMgr.RemoveListener<LeveledUpMessage>(UpdateCanGlide);
	}

	private void UpdateCanGlide(LeveledUpMessage msg)
	{
		HeroPersisted value = null;
		if (AppShell.Instance != null && AppShell.Instance.Profile != null && AppShell.Instance.Profile.SelectedCostume != null && AppShell.Instance.Profile.AvailableCostumes.TryGetValue(AppShell.Instance.Profile.SelectedCostume, out value) && value.Tier == 1 && value.Level == value.MaxLevel && DataCanGlide)
		{
			CanGlide = true;
		}
	}

	public IEnumerator DelayedSpawnAnimation()
	{
		if (spawnAnimation != null)
		{
			teleportTo(base.transform.position + Vector3.up * 100f);
			yield return 0;
			BehaviorSpawnAnimate introAnimation = behaviorManager.requestChangeBehavior(typeof(BehaviorSpawnAnimate), true) as BehaviorSpawnAnimate;
			if (introAnimation != null)
			{
				charGlobals.effectsList.RequestLoadedCallback(EffectsLoadedCallback, introAnimation);
			}
		}
	}

	public void EffectsLoadedCallback(EffectSequenceList fxList, object returnObject)
	{
		teleportTo(base.transform.position + Vector3.down * 100f);
		BehaviorSpawnAnimate behaviorSpawnAnimate = returnObject as BehaviorSpawnAnimate;
		behaviorSpawnAnimate.Initialize(spawnAnimation.spawnerSource, spawnAnimation.spawnDest, true, null, true);
	}

	public virtual void Awake()
	{
		motionExportTransform = Utils.FindNodeInChildren(base.transform, "motion_export");
		exportTransform = Utils.FindNodeInChildren(base.transform, "export_node");
	}

	public void InitializeFromData(DataWarehouse characterMotionData)
	{
		baseSpeed = characterMotionData.TryGetFloat("speed", 6f);
		if (baseSpeed == 0f && CardGameController.Instance != null)
		{
			baseSpeed = 4f;
		}
		runAnimationSpeed = characterMotionData.TryGetFloat("run_animation_speed", 1f);
		decelerationDistance = characterMotionData.TryGetFloat("deceleration_distance", 0.4f);
		gravity = characterMotionData.TryGetFloat("gravity", 20f);
		if (GameController.GetController() is SocialSpaceController)
		{
			float defaultValue = characterMotionData.TryGetFloat("jump_height", 10f);
			jumpHeight = characterMotionData.TryGetFloat("gw_jump_height", defaultValue);
			doubleJump = characterMotionData.TryGetBool("double_jump", false);
			doubleJumpAnimIndex = characterMotionData.TryGetInt("double_jump_anim_index", -1);
			holdJump = characterMotionData.TryGetBool("hold_jump", true);
			DataCanGlide = characterMotionData.TryGetBool("can_glide", false);
			UpdateCanGlide(null);
		}
		else
		{
			jumpHeight = characterMotionData.TryGetFloat("jump_height", 10f);
			holdJump = false;
			DataCanGlide = false;
		}
		SwingData = characterMotionData.TryGetString("swing_data", null);
		CanSwing = characterMotionData.TryGetBool("can_swing", false);
		SitOverride = characterMotionData.TryGetString("sit_override", null);
		if (SitOverride == string.Empty)
		{
			SitOverride = null;
		}
		secondJumpHeight = characterMotionData.TryGetFloat("second_jump_height", 10f);
		useJumpSequence = characterMotionData.TryGetBool("use_jump_sequence", false);
		jumpTimeout = characterMotionData.TryGetFloat("jump_timeout", 0.15f);
		rotateSpeed = characterMotionData.TryGetFloat("rotate_speed", 500f);
		fallTolerance = characterMotionData.TryGetFloat("fall_tolerance", 0.5f);
		landingEffectName = characterMotionData.TryGetString("landing_effect_name", null);
		doublejumpEffectName = characterMotionData.TryGetString("double_jump_effect_name", null);
		fallingEffectName = characterMotionData.TryGetString("falling_effect_name", null);
		pickupStrength = characterMotionData.TryGetFloat("pickup_strength", 5f);
		hotSpotType = HotSpotType.Style.None;
		foreach (DataWarehouse item in characterMotionData.GetIterator("hot_spots/hot_spot"))
		{
			hotSpotType |= HotSpotType.GetEnumFromString(item.TryGetString("text()", "none"));
		}
	}

	public void InitializeFromCopy(CharacterMotionController source)
	{
		baseSpeed = source.baseSpeed;
		runAnimationSpeed = source.runAnimationSpeed;
		decelerationDistance = source.decelerationDistance;
		gravity = source.gravity;
		jumpHeight = source.jumpHeight;
		jumpTimeout = source.jumpTimeout;
		rotateSpeed = source.rotateSpeed;
		fallTolerance = source.fallTolerance;
		landingEffectName = source.landingEffectName;
		doublejumpEffectName = source.doublejumpEffectName;
		fallingEffectName = source.fallingEffectName;
		pickupStrength = source.pickupStrength;
		hotSpotType = source.hotSpotType;
	}

	public void InitializePlaceholderFromData(DataWarehouse characterMotionData)
	{
		baseSpeed = characterMotionData.TryGetFloat("speed", 6f);
		decelerationDistance = characterMotionData.TryGetFloat("deceleration_distance", 0.4f);
		gravity = characterMotionData.TryGetFloat("gravity", 20f);
		if (GameController.GetController() is SocialSpaceController)
		{
			float defaultValue = characterMotionData.TryGetFloat("jump_height", 10f);
			jumpHeight = characterMotionData.TryGetFloat("gw_jump_height", defaultValue);
			doubleJump = characterMotionData.TryGetBool("double_jump", false);
			holdJump = characterMotionData.TryGetBool("hold_jump", true);
		}
		else
		{
			jumpHeight = characterMotionData.TryGetFloat("jump_height", 10f);
			holdJump = false;
		}
		secondJumpHeight = characterMotionData.TryGetFloat("second_jump_height", 10f);
		jumpTimeout = characterMotionData.TryGetFloat("jump_timeout", 0.15f);
		rotateSpeed = characterMotionData.TryGetFloat("rotate_speed", 500f);
		fallTolerance = characterMotionData.TryGetFloat("fall_tolerance", 0.5f);
		pickupStrength = characterMotionData.TryGetFloat("pickup_strength", 5f);
		hotSpotType = HotSpotType.Style.None;
		foreach (DataWarehouse item in characterMotionData.GetIterator("hot_spots/hot_spot"))
		{
			hotSpotType |= HotSpotType.GetEnumFromString(item.TryGetString("text()", "none"));
		}
	}

	public bool IsForcedVelocity()
	{
		return Time.time < forcedVelocityTime;
	}

	public void DisableNetUpdates(bool disable)
	{
		NetUpdatesDisabled = disable;
	}

	public void MotionUpdate()
	{
		Vector3 vector;
		if (IsForcedVelocity())
		{
			if (behaviorManager.useMotionControllerGravity())
			{
				updateGravity();
			}
			vector = forcedVelocity;
			vector.y = verticalVelocity;
			vector *= Time.deltaTime;
			moveCharacter(vector + extraVelocity * Time.deltaTime);
			setDestination(base.transform.position);
			CheckRestorePosition();
		}
		else if (!behaviorManager.useMotionController() || speed <= 0f)
		{
			vector = Vector3.zero;
			if (behaviorManager.useMotionControllerGravity())
			{
				updateGravity();
				vector.y = verticalVelocity * Time.deltaTime;
			}
			moveCharacter(vector + extraVelocity * Time.deltaTime);
			CheckRestorePosition();
		}
		else
		{
			if (destinationObject != null)
			{
				vector = destinationObject.transform.position - base.transform.position;
				if (destinationObjectDistance > 0f)
				{
					Vector3 vector2 = vector.normalized * destinationObjectDistance;
					vector -= vector2;
				}
			}
			else
			{
				vector = destination - base.transform.position;
			}
			vector.y = 0f;
			float num = distanceToDestination;
			distanceToDestination = vector.magnitude;
			if (distanceToDestination < speed * Time.deltaTime && networkComponent != null && !networkComponent.IsOwner() && networkComponent.IsNextPositionAvailable())
			{
				networkComponent.ProcessActionPosition();
				vector = destination - base.transform.position;
				vector.y = 0f;
				distanceToDestination = vector.magnitude;
			}
			if (distanceToDestination <= minimumMoveDistance)
			{
				vector = Vector3.zero;
				if (!inMotion)
				{
				}
			}
			else if (distanceToDestination < decelerationDistance + speed * Time.deltaTime && (networkComponent == null || !networkComponent.IsNextPositionAvailable()))
			{
				float num2 = 0f;
				if (distanceToDestination > decelerationDistance)
				{
					num2 = distanceToDestination - decelerationDistance;
				}
				num2 += (speed - num2) * (distanceToDestination / (decelerationDistance + speed * Time.deltaTime));
				vector.Normalize();
				vector *= num2;
			}
			else
			{
				vector.Normalize();
				vector *= speed;
				if (networkComponent != null && !networkComponent.IsOwner() && !NetUpdatesDisabled && checkForNetCatchups)
				{
					float num3 = num - distanceToDestination;
					if (num3 < speed * Time.deltaTime / 4f)
					{
						motionlessTime += Time.deltaTime;
					}
					else
					{
						motionlessTime = 0f;
					}
					if (distanceToDestination > immediateTeleportDistanceFactor * speed || (distanceToDestination > motionlessTeleportDistanceFactor * speed && motionlessTime > motionlessTimeout))
					{
						if ((double)extraVelocity.magnitude < 0.01)
						{
							base.gameObject.transform.localPosition = destination;
							motionlessTime = 0f;
						}
						HandleMotionCallbacks();
						return;
					}
					if (distanceToDestination > speed * charGlobals.motionController.positionSampleRate * 3f)
					{
						float num4 = Mathf.Min(1.5f, distanceToDestination / (speed * charGlobals.motionController.positionSampleRate * 3f));
						vector *= num4;
					}
				}
			}
			updateGravity();
			bool flag = IsOnGround();
			if ((flag || (doubleJump && !doubleJumping) || (CanSwing && flag)) && jumpPressedTime + jumpTimeout > Time.time)
			{
				jumping = true;
				jumpPressedTime = -1f;
				currentJumpCancelForce = 0f;
				wasOnGround = false;
				if (flag)
				{
					verticalVelocity = jumpHeight;
				}
				else
				{
					doubleJumping = true;
					if (CanSwing)
					{
						if (LastCollisionFlags == CollisionFlags.None)
						{
							behaviorManager.requestChangeBehavior(typeof(BehaviorSwingMovement), false);
						}
						else
						{
							doubleJumping = false;
						}
					}
					else
					{
						verticalVelocity = secondJumpHeight;
						if (!string.IsNullOrEmpty(doublejumpEffectName))
						{
							charGlobals.effectsList.OneShot(doublejumpEffectName);
						}
					}
				}
				behaviorManager.getBehavior().motionJumped();
				if (networkComponent != null && spawnData != null && (spawnData.spawnType & CharacterSpawn.Type.Local) != 0)
				{
					NetActionJump action = new NetActionJump(base.gameObject);
					networkComponent.QueueNetAction(action);
				}
			}
			if (!flag)
			{
				if (verticalVelocity < 0f)
				{
					if (gliding)
					{
						verticalVelocity *= 0.2f;
						vector *= 1.5f;
						if ((bool)charGlobals.animationComponent["hs_fly"])
						{
							charGlobals.animationComponent.CrossFade("hs_fly");
						}
					}
					else if (jumping)
					{
						behaviorManager.getBehavior().motionBeganFalling();
						jumping = false;
						currentJumpCancelForce = 0f;
						CreateFallingEffect();
					}
				}
				CheckRestorePosition();
			}
			Vector3 position = base.gameObject.transform.position;
			Vector3 a = new Vector3(vector.x, verticalVelocity, vector.z);
			a += extraVelocity;
			moveCharacter(a * Time.deltaTime);
			lastVelocity = (base.gameObject.transform.position - position) / Time.deltaTime;
			Vector3 vector3 = lastVelocity;
			vector3.y = 0f;
			if ((double)vector3.sqrMagnitude > 0.01)
			{
				rotateTowards(vector);
				if (!inMotion)
				{
					behaviorManager.getBehavior().motionBeganMoving();
					inMotion = true;
				}
			}
			else
			{
				if (destinationFacing != Vector3.zero)
				{
					rotateTowards(destinationFacing);
				}
				if (inMotion)
				{
					behaviorManager.getBehavior().motionStoppedMoving();
					inMotion = false;
					if ((double)vector.sqrMagnitude < 0.01)
					{
						behaviorManager.getBehavior().motionArrived();
					}
				}
			}
		}
		if ((LastCollisionFlags & CollisionFlags.Sides) != 0)
		{
			behaviorManager.getBehavior().motionCollided();
		}
		if (!behaviorManager.useMotionController())
		{
			HandleMotionCallbacks();
			return;
		}
		if (IsOnGround())
		{
			lastGoodPosition = base.transform.position;
			lastGoodPositionOffset = 0.5f;
			if (!wasOnGround)
			{
				if (jumping && networkComponent != null && spawnData != null && (spawnData.spawnType & CharacterSpawn.Type.Local) != 0 && positionLastSentTime + positionSampleRate <= Time.time && !NetUpdatesDisabled)
				{
					NetActionPositionFull action2 = new NetActionPositionFull(base.gameObject);
					networkComponent.QueueNetAction(action2);
				}
				jumping = false;
				doubleJumping = false;
				behaviorManager.getBehavior().motionLanded();
				wasOnGround = true;
				if (landingEffectName != null)
				{
					if (landingEffectPrefab == null)
					{
						landingEffectPrefab = (charGlobals.effectsList.GetEffectSequencePrefabByName(landingEffectName) as GameObject);
					}
					if (landingEffectPrefab != null)
					{
						GameObject child = UnityEngine.Object.Instantiate(landingEffectPrefab) as GameObject;
						Utils.AttachGameObject(base.gameObject, child);
					}
				}
				DestroyFallingEffect();
			}
			verticalVelocity = 0f;
		}
		else
		{
			if (wasOnGround)
			{
				Ray ray = new Ray(base.gameObject.transform.position + new Vector3(0f, 0.1f, 0f), Vector3.down);
				RaycastHit hitInfo;
				if (!Physics.Raycast(ray, out hitInfo, fallTolerance, 1073741823))
				{
					if (!jumping)
					{
						behaviorManager.getBehavior().motionBeganFalling();
						CreateFallingEffect();
					}
					wasOnGround = false;
				}
			}
			if (verticalVelocity > 0f && (LastCollisionFlags & CollisionFlags.Above) != 0)
			{
				verticalVelocity = 0f;
			}
		}
		//// this test block added by CSP   /////
		//NetActionPositionFull action14 = new NetActionPositionFull(base.gameObject);
		//networkComponent.QueueNetAction(action14);
		//NetActionPositionFullRotation action13 = new NetActionPositionFullRotation(base.gameObject);
		//networkComponent.QueueNetAction(action13);

		//CspUtils.DebugLog("NetActionPosition msgs queued!");
		/////////////////////////////////////////
		if (networkComponent != null && networkComponent.IsOwner() && !NetUpdatesDisabled && checkForNetCatchups && spawnData != null && (spawnData.spawnType & CharacterSpawn.Type.Local) != 0 && positionLastSentTime + positionSampleRate <= Time.time)
		{
			bool flag2 = inMotion || verticalVelocity != 0f;
			Vector3 delta = base.gameObject.transform.position - positionLastSent;
			bool flag3 = Mathf.Abs(delta.x) > 1.2f || Mathf.Abs(delta.y) > 12f || Mathf.Abs(delta.z) > 1.2f;
			if ((!flag2 && positionFullLastSentTime != positionLastSentTime) || positionFullLastSentTime + positionSyncRate <= Time.time || flag3)
			{
				if (positionLastSent != base.gameObject.transform.position || positionFullLastSentTime != positionLastSentTime || positionLastSentTime + minimumPositionRate <= Time.time)
				{
					if (!flag2)
					{
						NetActionPositionFullRotation action3 = new NetActionPositionFullRotation(base.gameObject);
						networkComponent.QueueNetAction(action3);
					}
					else
					{
						NetActionPositionFull action4 = new NetActionPositionFull(base.gameObject);
						networkComponent.QueueNetAction(action4);
					}
				}
			}
			else if (positionLastSent != base.gameObject.transform.position)
			{
				if (positionLastSentTime < Time.time - positionSampleRate * 2f)
				{
					positionLastSentTime = Time.time;
				}
				else
				{
					NetActionPositionUpdate action5 = new NetActionPositionUpdate(delta);
					networkComponent.QueueNetAction(action5);
					//CspUtils.DebugLog("NetActionPositionUpdate queued!");
				}
			}
		}
		HandleMotionCallbacks();
	}

	protected void HandleMotionCallbacks()
	{
		if (lastFrameOnGround && !isOnGround && airborneCallback != null)
		{
			airborneCallback();
		}
		if (!lastFrameOnGround && isOnGround && landCallback != null)
		{
			landCallback();
		}
		lastFrameOnGround = isOnGround;
	}

	public void positionSent(bool fullPosition)
	{
		positionLastSent = base.transform.position;
		positionLastSentTime = Time.time;
		if (fullPosition)
		{
			positionFullLastSentTime = positionLastSentTime;
		}
	}

	protected void CreateFallingEffect()
	{
		if (fallingEffectName != null && fallingEffectInstance == null)
		{
			if (fallingEffectPrefab == null)
			{
				fallingEffectPrefab = (charGlobals.effectsList.GetEffectSequencePrefabByName(fallingEffectName) as GameObject);
			}
			if (fallingEffectPrefab != null)
			{
				fallingEffectInstance = (UnityEngine.Object.Instantiate(fallingEffectPrefab) as GameObject);
				Utils.AttachGameObject(base.gameObject, fallingEffectInstance);
			}
		}
	}

	protected void DestroyFallingEffect()
	{
		if (fallingEffectInstance != null)
		{
			UnityEngine.Object.Destroy(fallingEffectInstance);
			fallingEffectInstance = null;
		}
		if (glideSequence != null)
		{
			UnityEngine.Object.Destroy(glideSequence);
			glideSequence = null;
		}
	}

	protected void moveCharacter(Vector3 motion)
	{
		collidersHit.Clear();
		trackCollisions = true;
		LastCollisionFlags = characterController.Move(motion);
		trackCollisions = false;
		if (motion.x == 0f && motion.z == 0f)
		{
			base.transform.position = base.transform.position;
		}
		if (motion.y != 0f)
		{
			isOnGround = ((LastCollisionFlags & CollisionFlags.Below) != 0);
		}
	}

	public void setIsOnGround(bool newIsOnGround)
	{
		isOnGround = newIsOnGround;
		verticalVelocity = 0f;
	}

	public void setWasOnGround(bool newWasOnGround)
	{
		wasOnGround = newWasOnGround;
	}

	protected void updateGravity()
	{
		verticalVelocity -= gravity * Time.deltaTime;
		if (currentJumpCancelForce > 0f && verticalVelocity > 0f)
		{
			verticalVelocity -= currentJumpCancelForce * Time.deltaTime;
		}
		else if (verticalVelocity <= 0f)
		{
			currentJumpCancelForce = 0f;
		}
		if (verticalVelocity < 0f - gravityTerminalVelocity)
		{
			verticalVelocity = 0f - gravityTerminalVelocity;
		}
	}

	public void rotateTowards(Vector3 lookDir)
	{
		if (behaviorManager.useMotionControllerRotate())
		{
			lookDir.y = 0f;
			if (lookDir.sqrMagnitude > 0.0001f)
			{
				lookDirection = Vector3.RotateTowards(lookDirection, lookDir, rotateSpeed * ((float)Math.PI / 180f) * Time.deltaTime, 1000f);
				base.transform.rotation = Quaternion.LookRotation(lookDirection);
			}
		}
	}

	public void rotate(float angle)
	{
		base.transform.Rotate(Vector3.up, angle, Space.Self);
		lookDirection = base.transform.forward;
	}

	public void rotate(Vector3 lookAt)
	{
		Vector3 forward = lookAt - base.transform.position;
		forward.y = 0f;
		charGlobals.transform.rotation = Quaternion.LookRotation(forward);
		lookDirection = base.transform.forward;
	}

	public void updateLookDirection()
	{
		lookDirection = base.transform.forward;
	}

	public void delayedRotate(float angle, int frameDelay)
	{
		delayedRotateAngle = angle;
		delayedRotateDelay = frameDelay;
	}

	public void moveTowards(Vector3 moveDestination, float speedModifier)
	{
		Vector3 motion = moveDestination - base.gameObject.transform.position;
		if (speedModifier > 0f)
		{
			motion.Normalize();
			motion *= speed * speedModifier;
			motion *= Time.deltaTime;
		}
		moveCharacter(motion);
	}

	public bool IsOnGround()
	{
		return isOnGround;
	}

	public bool IsJumping()
	{
		return jumping;
	}

	public void jumpPressed()
	{
		if (!(charGlobals.combatController != null) || !charGlobals.combatController.IsJumpRestricted)
		{
			if (carriedThrowable == null)
			{
				jumpPressedTime = Time.time;
				lastJumpPressedTime = jumpPressedTime;
				Vector3 position = base.gameObject.transform.position;
				lastJumpStartElevation = position.y;
			}
			if (CanGlide)
			{
				gliding = true;
				charGlobals.effectsList.TryGetEffectSequenceByName("glide sequence", out glideSequence);
			}
		}
	}

	public void jumpReleased()
	{
		if (IsJumping())
		{
			float num = Time.time - lastJumpPressedTime;
			if (holdJump && num < shortJumpCutoffTime)
			{
				behaviorManager.getBehavior().motionJumpCancelled();
				float num2 = (!doubleJumping) ? jumpHeight : secondJumpHeight;
				currentJumpCancelForce = (num2 - standardJumpHeight) / (standardJumpHeight / gravity - num);
				if (networkComponent != null && networkComponent.IsOwner())
				{
					NetActionJumpCancel netActionJumpCancel = new NetActionJumpCancel(base.gameObject);
					netActionJumpCancel.JumpDuration = num;
					netActionJumpCancel.SecondJump = doubleJumping;
					networkComponent.QueueNetAction(netActionJumpCancel);
				}
			}
		}
		if (CanGlide)
		{
			gliding = false;
			if (glideSequence != null)
			{
				UnityEngine.Object.Destroy(glideSequence);
				glideSequence = null;
			}
		}
	}

	public void netJumpReleased(NetActionJumpCancel jumpCancel)
	{
		if (!jumpCancel.SecondJump || !CanSwing)
		{
			float num = (!jumpCancel.SecondJump) ? jumpHeight : secondJumpHeight;
			float jumpDuration = jumpCancel.JumpDuration;
			float num2 = num - gravity * jumpDuration;
			float num3 = num2 * jumpDuration + lastJumpStartElevation;
			float num4 = Time.time - lastJumpPressedTime - jumpCancel.JumpDuration;
			float num5 = (num - standardJumpHeight) / (standardJumpHeight / gravity - jumpCancel.JumpDuration);
			float num6 = num5 + gravity;
			jumpDuration = num2 / num6;
			float num7 = num2 * num4 - num6 * num4 * num4 + num3;
			if (num4 > jumpDuration)
			{
				num4 -= jumpDuration;
				num7 -= gravity * num4 * num4;
			}
			else
			{
				verticalVelocity = num2 - num6 * num4;
				currentJumpCancelForce = num5;
			}
			float num8 = num7;
			Vector3 position = base.gameObject.transform.position;
			moveCharacter(new Vector3(0f, num8 - position.y, 0f));
		}
	}

	public Vector3 getDestination()
	{
		return destination;
	}

	public void setDestination(Vector3 newDestination)
	{
		setDestination(newDestination, true);
	}

	public void setDestination(Vector3 newDestination, bool clearFacing)
	{
		if (newDestination == Vector3.zero)
		{
			CspUtils.DebugLog("Zero Destination - AAAAARRRRRRGGGGGGG");
		}
		destination = newDestination;
		destinationObject = null;
		if (clearFacing)
		{
			destinationFacing = Vector3.zero;
		}
		if (GameController.GetController().LocalPlayer == base.gameObject)
		{
			NewTutorialManager.sendEvent(TutorialEvent.TutorialEventType.HERO_MOVE);
		}
	}

	public void setDestination(Vector3 newDestination, Vector3 newFacing)
	{
		setDestination(newDestination);
		destinationFacing = newFacing;
	}

	public void setDestination(GameObject newDestinationObject)
	{
		setDestination(newDestinationObject, 0f);
	}

	public void setDestination(GameObject newDestinationObject, float distance)
	{
		destinationObject = newDestinationObject;
		destinationObjectDistance = distance;
	}

	public void setNewFacing(Vector3 newFacing)
	{
		destinationFacing = newFacing;
	}

	public void forceNewFacing(Vector3 newFacing)
	{
		destinationFacing = newFacing;
		lookDirection = newFacing;
	}

	public void RotateInPlace(float speed, bool toTheLeft)
	{
		Quaternion rotation = (!toTheLeft) ? Quaternion.Euler(0f, speed * Time.deltaTime, 0f) : Quaternion.Euler(0f, 0f - speed * Time.deltaTime, 0f);
		destinationFacing = rotation * lookDirection;
	}

	public void setForcedVelocity(Vector3 newVelocity, float duration)
	{
		if (forcedVelocityAllowed)
		{
			forcedVelocity = newVelocity;
			forcedVelocityTime = Time.time + duration;
		}
	}

	public void setForcedVelocityDuration(float duration)
	{
		if (forcedVelocityAllowed)
		{
			forcedVelocityTime = Time.time + duration;
		}
	}

	public void setVerticalVelocity(float newVerticalVelocity)
	{
		if (forcedVelocityAllowed)
		{
			verticalVelocity = newVerticalVelocity;
		}
	}

	public float getVerticalVelocity()
	{
		return verticalVelocity;
	}

	public void setForcedVelocityAllow(bool allow)
	{
		forcedVelocityAllowed = allow;
	}

	public void stopGently()
	{
		stopGently(true);
	}

	public void stopGently(bool clearFacing)
	{
		if (inMotion)
		{
			Vector3 b = destination - base.transform.position;
			b.y = 0f;
			b.Normalize();
			b *= decelerationDistance;
			setDestination(base.transform.position + b, clearFacing);
		}
		else
		{
			setDestination(base.transform.position, clearFacing);
		}
	}

	public Vector3 getVelocity()
	{
		return lastVelocity;
	}

	public Vector3 getVelocityFlat()
	{
		Vector3 result = lastVelocity;
		result.y = 0f;
		return result;
	}

	public void teleportTo(Vector3 position)
	{
		if (position == Vector3.zero)
		{
			CspUtils.DebugLog("Zero Teleport - AAAAARRRRRRGGGGGGG");
		}
		debugTeleportType = 1;
		debugTeleportLoc = position;
		base.gameObject.transform.position = position;
	}

	public void teleportTo(Vector3 position, Vector3 newFacing)
	{
		teleportTo(position, Quaternion.LookRotation(newFacing));
	}

	public void teleportTo(Vector3 position, Quaternion rotation)
	{
		base.transform.rotation = rotation;
		teleportTo(position);
	}

	public void teleportToDestination()
	{
		if (destination == Vector3.zero)
		{
			CspUtils.DebugLog("Zero Teleport - AAAAARRRRRRGGGGGGG");
		}
		debugTeleportType = 2;
		debugTeleportLoc = destination;
		base.gameObject.transform.position = destination;
	}

	public void behaviorLateUpdate()
	{
		if (motionExportTransform != null)
		{
			motionExportPosition = motionExportTransform.localPosition;
			Vector3 eulerAngles = motionExportTransform.localRotation.eulerAngles;
			motionExportRotation = eulerAngles.z;
		}
		if (delayedRotateAngle != 0f && delayedRotateDelay-- == 0)
		{
			rotate(delayedRotateAngle);
			delayedRotateAngle = 0f;
		}
		if (cameraForceFollowTime > 0f)
		{
			cameraForceFollowTime -= Time.deltaTime;
			if (cameraForceFollowTime <= 0f)
			{
				cameraForceFollowTime = 0f;
			}
		}
		if (timeToEnableCollision > 0f)
		{
			timeToEnableCollision -= Time.deltaTime;
			if (timeToEnableCollision <= 0f)
			{
				ToggleEnemyCollisions(true);
			}
		}
	}

	public void performRootMotion()
	{
		performRootMotion(false);
	}

	public void performRootMotion(bool allowLateralTranslation)
	{
		if (motionExportTransform == null)
		{
			return;
		}
		Vector3 localPosition = motionExportTransform.localPosition;
		if (!((double)Mathf.Abs(localPosition.y) < 0.1))
		{
			float y = motionExportPosition.y;
			Vector3 localPosition2 = motionExportTransform.localPosition;
			float d = y - localPosition2.y;
			Vector3 motion = base.transform.forward * d;
			if (allowLateralTranslation)
			{
				motion += base.transform.TransformDirection(motionExportTransform.localPosition - motionExportPosition);
			}
			moveCharacter(motion);
			setDestination(base.transform.position);
		}
	}

	public void addSpeedMultiplier(float multiplier)
	{
		speedMultiplierList.Add(multiplier);
		updateSpeed();
	}

	public void removeSpeedMultiplier(float multiplier)
	{
		speedMultiplierList.Remove(multiplier);
		updateSpeed();
	}

	protected float recalcSpeedMultiplier()
	{
		float num = 1f;
		float num2 = 1f;
		foreach (float speedMultiplier2 in speedMultiplierList)
		{
			float num3 = speedMultiplier2;
			if (num3 > num)
			{
				num = num3;
			}
			else if (num3 < num2)
			{
				num2 = num3;
			}
		}
		return num * num2;
	}

	public void setBaseSpeed(float newSpeed)
	{
		baseSpeed = newSpeed;
	}

	protected void updateSpeed()
	{
		speedMultiplier = recalcSpeedMultiplier();
		if (speedMultiplier < 0f)
		{
			speedMultiplier = 0f;
		}
		speed = baseSpeed * speedMultiplier;
		updateRunAnimationSpeed();
	}

	public void changeRunSpeedMultiplier(float newRunMultiplier)
	{
		runSpeedMultiplier = newRunMultiplier;
		updateRunAnimationSpeed();
	}

	protected void updateRunAnimationSpeed()
	{
		float num = runAnimationSpeed * speedMultiplier * runSpeedMultiplier;
		Animation animation = null;
		animation = ((!(charGlobals != null)) ? (GetComponent(typeof(Animation)) as Animation) : charGlobals.animationComponent);
		if (!(animation != null))
		{
			return;
		}
		if (animation["movement_run"] != null)
		{
			animation["movement_run"].speed = num;
			if (animation["pickup_run"] != null)
			{
				animation["pickup_run"].speed = num;
			}
		}
		if (animation["movement_run_sidekick"] != null)
		{
			animation["movement_run_sidekick"].speed = num;
		}
	}

	public void changeScale(Vector3 scaleAdjustment, bool adding)
	{
		if (exportTransform == null)
		{
			return;
		}
		Vector3 localScale = exportTransform.localScale;
		if (adding)
		{
			scaleAdjustments++;
			localScale.x *= scaleAdjustment.x;
			localScale.y *= scaleAdjustment.y;
			localScale.z *= scaleAdjustment.z;
		}
		else
		{
			scaleAdjustments--;
			if (scaleAdjustments <= 0)
			{
				localScale = new Vector3(1f, 1f, 1f);
			}
			else
			{
				localScale.x /= scaleAdjustment.x;
				localScale.y /= scaleAdjustment.y;
				localScale.z /= scaleAdjustment.z;
			}
		}
		exportTransform.localScale = localScale;
		changeRunSpeedMultiplier(1f / Mathf.Max(localScale.x, localScale.y));
	}

	public void LockTargetCamera(bool lockX, bool lockY, bool lockZ, bool restoreOnLand, float timeOut)
	{
		CameraTargetHelper componentInChildren = GetComponentInChildren<CameraTargetHelper>();
		if (componentInChildren != null)
		{
			if (cameraForceFollowTime > 0f)
			{
				lockX = false;
				lockY = false;
				lockZ = false;
			}
			restoreCameraOnLand = restoreOnLand;
			componentInChildren.LockAxes(lockX, lockY, lockZ, timeOut);
		}
	}

	public void dropThrowable()
	{
		if (carriedThrowable != null && networkComponent.IsOwner())
		{
			carriedThrowable.DropObject(false);
			carriedThrowable = null;
			NetActionDropThrowable action = new NetActionDropThrowable(false);
			networkComponent.QueueNetAction(action);
		}
	}

	public void dropThrowableRemote(bool silent)
	{
		StartCoroutine(dropThrowableRemoteDelay(silent));
	}

	public IEnumerator dropThrowableRemoteDelay(bool silent)
	{
		if (silent)
		{
			yield return new WaitForSeconds(0.25f);
		}
		if (carriedThrowable != null)
		{
			carriedThrowable.DropObject(silent);
			carriedThrowable = null;
		}
	}

	public void setSpawnAnimation(SpawnAnimateData toPlay)
	{
		spawnAnimation = toPlay;
	}

	protected void DebugCollision()
	{
		RestorePosition();
	}

	public void SetRestoreLocation(Vector3 restorePoint)
	{
		lastGoodPosition = restorePoint;
		lastGoodPositionOffset = 0.5f;
	}

	protected void CheckRestorePosition()
	{
		Vector3 position = base.gameObject.transform.position;
		if (position.y < -100f)
		{
			RestorePosition();
		}
	}

	protected void RestorePosition()
	{
		Vector3 vector = lastGoodPosition;
		vector.y += lastGoodPositionOffset;
		Ray ray = new Ray(vector, Vector3.down);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 5000f, 98305))
		{
		}
		teleportTo(vector);
		verticalVelocity = 0f;
		forcedVelocity = Vector3.zero;
		destination = vector;
		lastGoodPositionOffset = Mathf.Clamp(lastGoodPositionOffset + 1f, 0f, 5f);
	}

	private void OnDrawGizmosSelected()
	{
		if (destination != Vector3.zero)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(base.transform.position, destination);
		}
	}

	public void OnCutSceneStart()
	{
		stopGently();
	}

	protected void CheckCameraRestore()
	{
		if (restoreCameraOnLand)
		{
			LockTargetCamera(false, false, false, false, 0f);
		}
	}

	protected void FreezeCameraInAir()
	{
		CameraTargetHelper componentInChildren = GetComponentInChildren<CameraTargetHelper>();
		if (componentInChildren != null && !componentInChildren.AxesLocked())
		{
			LockTargetCamera(false, true, false, true, 4f);
		}
	}

	public void ToggleEnemyCollisions(bool enabled)
	{
		EnemyCollisionsDisabled = !enabled;
		CombatController[] array = Utils.FindObjectsOfType<CombatController>();
		foreach (CombatController combatController in array)
		{
			if (combatController as ObjectCombatController == null && charGlobals.combatController.IsControllerEnemy(combatController))
			{
				Physics.IgnoreCollision(base.collider, combatController.collider, !enabled);
				PushAwayColliderController componentInChildren = base.gameObject.GetComponentInChildren<PushAwayColliderController>();
				if (componentInChildren != null)
				{
					componentInChildren.IgnoreCharacter(combatController.GetComponent<CharacterGlobals>(), !enabled);
				}
			}
		}
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (trackCollisions)
		{
			collidersHit.Add(hit);
		}
	}

	public bool HitColliderOnSide()
	{
		if ((LastCollisionFlags & CollisionFlags.Sides) == 0)
		{
			return false;
		}
		if (collidersHit.Count == 0)
		{
			return false;
		}
		float num = (characterController.height - characterController.radius * 2f) / 2f;
		if (num < 0f)
		{
			num = 0f;
		}
		Vector3 position = base.transform.position;
		float y = position.y;
		Vector3 center = characterController.center;
		float num2 = y + center.y;
		float num3 = num2 + num;
		float num4 = num2 - num;
		foreach (ControllerColliderHit item in collidersHit)
		{
			if (item.gameObject.GetComponent<CharacterGlobals>() != null)
			{
				CspUtils.DebugLog("hit a character and ignoring it " + item.gameObject.name);
			}
			else
			{
				Vector3 moveDirection = item.moveDirection;
				float x = moveDirection.x;
				Vector3 moveDirection2 = item.moveDirection;
				float num5 = x * moveDirection2.x;
				Vector3 moveDirection3 = item.moveDirection;
				float z = moveDirection3.z;
				Vector3 moveDirection4 = item.moveDirection;
				float num6 = num5 + z * moveDirection4.z;
				if (!(Mathf.Abs(num6) < float.Epsilon))
				{
					float f = characterController.radius * characterController.radius / num6;
					float num7 = Mathf.Sqrt(f);
					Vector3 moveDirection5 = item.moveDirection;
					float num8 = num2 + num7 * moveDirection5.y;
					if (num8 <= num3 && num8 >= num4)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	protected float GetJumpPotential(float jumpVelocity)
	{
		float num = jumpVelocity / gravity;
		return jumpVelocity * num - 0.5f * gravity * num * num;
	}

	[AnimTag("footstep")]
	public void OnFootstepAnimTag(AnimationEvent evt)
	{
		if (!inMotion)
		{
			return;
		}
		string stringParameter = evt.stringParameter;
		int num = (!string.IsNullOrEmpty(stringParameter)) ? int.Parse(stringParameter) : 0;
		GameObject gameObject = (num != 0) ? (charGlobals.effectsList.TryGetEffectSequencePrefabByName("FootfallEffectRight") as GameObject) : (charGlobals.effectsList.TryGetEffectSequencePrefabByName("FootfallEffectLeft") as GameObject);
		if (gameObject != null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, gameObject.transform.localPosition, gameObject.transform.localRotation) as GameObject;
			EffectSequence effectSequence = gameObject2.GetComponent(typeof(EffectSequence)) as EffectSequence;
			if (effectSequence != null)
			{
				effectSequence.Initialize(base.gameObject, null, null);
			}
		}
	}

	[AnimTag("enemy_collision")]
	public void OnEnemyCollisionAnimTag(AnimationEvent evt)
	{
		if (!(networkComponent != null) || networkComponent.IsOwner())
		{
			float result = 0f;
			float.TryParse(evt.stringParameter, out result);
			if (result != 0f)
			{
				result = (timeToEnableCollision = result / base.animation[base.animation.clip.name].speed);
				ToggleEnemyCollisions(false);
			}
		}
	}

	[AnimTag("intangibility_on")]
	public void OnIntangibilityOnAnimTag(AnimationEvent evt)
	{
		ToggleEnemyCollisions(false);
	}

	[AnimTag("intangibility_off")]
	public void OnIntangibilityOffAnimTag(AnimationEvent evt)
	{
		ToggleEnemyCollisions(true);
	}
}
