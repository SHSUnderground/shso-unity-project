using System.Collections;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum inputEventType
	{
		clickToMove,
		clickToDrive,
		jump,
		primaryAttack,
		secondaryAttack,
		superAttack,
		count
	}

	public enum SuperActivationMode
	{
		BothButton,
		Context,
		Hold
	}

	protected const float clickThreshhold = 0.25f;

	protected const float disengageTime = 1f;

	protected const float disengageDistance = 100f;

	protected const float emoteCooldownDuration = 1f;

	protected const float holdSuperModeHoldTime = 1f;

	protected const float bothClickTime = 0.25f;

	protected const int bothClickFrames = 4;

	public bool registerAsLocalPlayerOnStart = true;

	public bool sendRolloverEvents = true;

	public bool allowObjectInteraction = true;

	protected CharacterGlobals charGlobals;

	protected CharacterMotionController motionController;

	protected BehaviorManager behaviorManager;

	protected CombatController combatController;

	protected GameController gameController;

	protected GameObject lastRolloverObject;

	protected ObjectPicker picker;

	protected bool combatMode;

	protected bool previousClickHandled;

	protected bool stopped = true;

	protected bool allowInput = true;

	protected bool stopInputStrict;

	protected float currentMouseScale = 1f;

	protected float moveTimeLeft = -1f;

	protected float timeSinceMoveClick = 1f;

	protected float timeSinceEnemyClick = 1f;

	protected Vector3 enemyClickPosition;

	protected float lastEmoteTime = -1f;

	protected bool holdSuperFired;

	protected float timeSinceLeftClick;

	protected float timeSinceRightClick;

	protected int framesSinceLeftClick;

	protected int framesSinceRightClick;

	protected float minNavLifetime = 1f;

	protected bool nextClickIsDebug;

	protected bool firedDestructionMessage;

	private Vector3 mouseCenter;

	private Vector3 lastMousePosition;

	private Vector3 keyboardOffset;

	private bool directionAsleep = true;

	private int keyboardDirection;

	private bool facingAsleep = true;

	protected float interactRange = 2.5f;

	public static int keyboardControlLevel = 1;

	private Vector3 lastPosition;

	private Vector3 throwTargetPosition;

	private Object playerMoveEffectResource;

	private GameObject playerMoveEffectInstance;

	private SHSKeyCode playerKeyCode = new SHSKeyCode();

	protected bool[] currentInputActions;

	private float mouseHoldTime;

	public SuperActivationMode CurrentSuperActivationMode;

	public bool[] CurrentInputActions
	{
		get
		{
			return currentInputActions;
		}
	}

	public bool AllowInput
	{
		get
		{
			return allowInput;
		}
		set
		{
			allowInput = value;
		}
	}

	private void Awake()
	{
		currentInputActions = new bool[6];
	}

	protected virtual void Start()
	{
		charGlobals = (GetComponent(typeof(CharacterGlobals)) as CharacterGlobals);
		motionController = charGlobals.motionController;
		behaviorManager = charGlobals.behaviorManager;
		combatController = charGlobals.combatController;
		GameObject gameObject = GameObject.FindGameObjectWithTag("GameController");
		if ((bool)gameObject)
		{
			gameController = (gameObject.GetComponent(typeof(GameController)) as GameController);
		}
		if (registerAsLocalPlayerOnStart)
		{
			gameController.LocalPlayer = base.gameObject;
		}
		Object[] array = Object.FindSceneObjectsOfType(typeof(InteractiveObject));
		Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			InteractiveObject interactiveObject = (InteractiveObject)array2[i];
			interactiveObject.ResetForNewPlayer();
		}
		lastRolloverObject = null;
		picker = new ObjectPicker(base.gameObject);
		mouseCenter = new Vector3(0f, 0f, 0f);
		lastMousePosition = new Vector3(0f, 0f, 0f);
		keyboardOffset = new Vector3(0f, 0f, 0f);
		lastPosition = new Vector3(0f, 0f, 0f);
		keyboardControlLevel = PlayerPrefs.GetInt("KeyboardLevel", 1);
		AppShell.Instance.EventMgr.AddListener<EmoteMessage>(OnEmote);
		AppShell.Instance.EventMgr.AddListener<PowerMoveMessage>(OnPowerMove);
		AppShell.Instance.EventMgr.AddListener<EntityFactionChangeMessage>(OnEntityFactionChange);
		AppShell.Instance.EventMgr.AddListener<NPCEmoteReactMessage>(OnNPCReactMessage);
		AppShell.Instance.EventMgr.Fire(this, new PlayerInputControllerCreated(this));
	}

	private void OnEnable()
	{
	}

	protected void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<EmoteMessage>(OnEmote);
		AppShell.Instance.EventMgr.RemoveListener<PowerMoveMessage>(OnPowerMove);
		AppShell.Instance.EventMgr.RemoveListener<EntityFactionChangeMessage>(OnEntityFactionChange);
		AppShell.Instance.EventMgr.RemoveListener<NPCEmoteReactMessage>(OnNPCReactMessage);
		if (gameController != null && gameController.LocalPlayer == base.gameObject)
		{
			gameController.LocalPlayer = null;
		}
	}

	protected void OnDestroy()
	{
		FireDestructionMessage();
	}

	protected bool testHotSpotClick(GameObject obj)
	{
		HotSpotManager component = Utils.GetComponent<HotSpotManager>(obj);
		if ((bool)component)
		{
			if (component.OnMouseClick(base.gameObject))
			{
				return true;
			}
		}
		else
		{
			HotSpot component2 = Utils.GetComponent<HotSpot>(obj);
			if ((bool)component2 && component2.OnMouseClick(base.gameObject))
			{
				return true;
			}
		}
		return false;
	}

	protected bool testInteractableClick(GameObject obj)
	{
		InteractiveObject component = Utils.GetComponent<InteractiveObject>(obj);
		if ((bool)component && component.OnMouseClick(base.gameObject))
		{
			return true;
		}
		InteractiveObjectForwarder component2 = Utils.GetComponent<InteractiveObjectForwarder>(obj);
		if ((bool)component2 && component2.OnMouseClick(base.gameObject))
		{
			return true;
		}
		InteractableObject interactableObject = obj.GetComponent(typeof(InteractableObject)) as InteractableObject;
		if ((bool)interactableObject && interactableObject.OnMouseClick(base.gameObject))
		{
			return true;
		}
		return false;
	}

	protected bool testThrowableClick(GameObject obj)
	{
		ThrowableGround throwableGround = obj.GetComponent(typeof(ThrowableGround)) as ThrowableGround;
		if ((bool)throwableGround && throwableGround.OnMouseClick(charGlobals))
		{
			return true;
		}
		return false;
	}

	public void ForceMouseRollverUpdate()
	{
		if (lastRolloverObject != null)
		{
			lastRolloverObject.SendMessage("OnMouseRolloverExit", null, SendMessageOptions.DontRequireReceiver);
			lastRolloverObject = null;
		}
		GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Normal);
		GUIManager.Instance.DetachMouseoverIndicator();
		Update();
	}

	public bool GetMouseProjectPoint(out RaycastHit hit)
	{
		Ray ray = Camera.main.ScreenPointToRay(SHSInput.mousePosition);
		int layerMask = 803704297;
		return Physics.Raycast(ray, out hit, float.PositiveInfinity, layerMask) && !SHSInput.IsOverUI();
	}

	protected void DoCursorScaling(GameObject cursorTarget, RaycastHit hit)
	{
		float near = Camera.main.near;
		float num = 1000f;
		if (!SHSInput.IsOverUI())
		{
			RaycastHit hit2;
			if (cursorTarget != null)
			{
				num = hit.distance;
			}
			else if (GetMouseProjectPoint(out hit2))
			{
				num = hit2.distance;
			}
		}
		else
		{
			num = near;
		}
		float num2 = near / num * 32f;
		if (num2 > 0.85f)
		{
			num2 = 0.85f;
		}
		if (num2 < 0.2f)
		{
			num2 = 0.2f;
		}
		float num3 = num2 - currentMouseScale;
		float num4 = Mathf.Abs(num3);
		float num5 = (2f * (1f - num4) + 3f * num4) * Time.deltaTime;
		if (num3 > 0f)
		{
			if (num3 > num5)
			{
				num3 = num5;
			}
		}
		else if (num3 < 0f - num5)
		{
			num3 = 0f - num5;
		}
		currentMouseScale += num3;
		GUIManager.Instance.CursorManager.CursorScale = new Vector2(currentMouseScale, currentMouseScale);
	}

	protected virtual void Update()
	{
		timeSinceMoveClick += Time.deltaTime;
		timeSinceEnemyClick += Time.deltaTime;
		//CspUtils.DebugLog("charGlobals.networkComponent=" + charGlobals.networkComponent);
		//CspUtils.DebugLog("charGlobals.networkComponent.IsOwner()" + charGlobals.networkComponent.IsOwner());
		if (GameController.GetController().controllerType != GameController.ControllerType.RailsGameWorld && (charGlobals.networkComponent == null || !charGlobals.networkComponent.IsOwner()) && charGlobals.spawnData.modelName != "mr_placeholder")
		{
			return;   // CSP - temporarily commented out
		}
		for (int i = 0; i < currentInputActions.Length; i++)
		{
			currentInputActions[i] = false;
		}
		if (!behaviorManager.allowUserInput() || !AllowInput)
		{
			if (!stopInputStrict)
			{
				if (lastRolloverObject != null)
				{
					lastRolloverObject.SendMessage("OnMouseRolloverExit", null, SendMessageOptions.DontRequireReceiver);
					lastRolloverObject = null;
				}
				if (!SHSInput.IsOverUI())
				{
					GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Normal);
				}
				GUIManager.Instance.DetachMouseoverIndicator();
			}
			behaviorManager.userInputOverride();
			combatMode = false;
			ResetKeyboard();
			return;
		}
		GameObject gameObject = picker.GetFirstObjectUnderCursor((!(BrawlerController.Instance == null)) ? 50 : 0);
		bool flag = true;
		if (gameObject != null)
		{
			if (SHSInput.IsObjectAllowedInput(gameObject) && sendRolloverEvents)
			{
				if (gameObject != lastRolloverObject && lastRolloverObject != null)
				{
					lastRolloverObject.SendMessage("OnMouseRolloverExit", null, SendMessageOptions.DontRequireReceiver);
					lastRolloverObject = null;
				}
				if (lastRolloverObject == null)
				{
					lastRolloverObject = gameObject;
					MouseRollover value = new MouseRollover(charGlobals, behaviorManager.allowUserInput(), base.gameObject);
					lastRolloverObject.SendMessage("OnMouseRolloverEnter", value, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		else
		{
			if (lastRolloverObject != null)
			{
				lastRolloverObject.SendMessage("OnMouseRolloverExit", null, SendMessageOptions.DontRequireReceiver);
				lastRolloverObject = null;
			}
			if (combatMode)
			{
				GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Normal);
			}
		}
		bool flag2 = false;
		bool flag3 = false;
		if (SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Right))
		{
			holdSuperFired = false;
			if (motionController.carriedThrowable != null)
			{
				if (behaviorManager.getBehavior() is BehaviorMovement)
				{
					BehaviorApproach behaviorApproach = behaviorManager.requestChangeBehavior(typeof(BehaviorApproach), true) as BehaviorApproach;
					if (behaviorApproach != null)
					{
						throwTargetPosition = calcPointOnPlane(false);
						Vector3 forward = throwTargetPosition - base.transform.position;
						behaviorApproach.Initialize(base.transform.position, Quaternion.LookRotation(forward), true, UntargettedThrowReady, null, 0.1f, 100f, true, false);
					}
				}
			}
			else if (CurrentSuperActivationMode == SuperActivationMode.Context && ((PlayerCombatController)combatController).checkPowerFull())
			{
				((PlayerCombatController)combatController).UsePower();
			}
			else if (CurrentSuperActivationMode != SuperActivationMode.Hold)
			{
				if (gameObject != null)
				{
					startSecondaryAttack(gameObject);
				}
				else if (combatController.getSecondaryAttackData(combatController.selectedSecondaryAttack).targetlessAttack && GameController.GetController().AllowTargetlessAttacks())
				{
					combatController.beginAttack(null, true);
				}
			}
			if (GameController.GetController() is SocialSpaceController)
			{
				GUIManager.Instance.DetachTargetedPlayerIndicator();
				AppShell.Instance.EventMgr.Fire(this, new SelectedPlayerMessage(-1, string.Empty, string.Empty, null));
			}
		}
		else if (CurrentSuperActivationMode == SuperActivationMode.Hold && SHSInput.GetMouseButtonUp(SHSInput.MouseButtonType.Right))
		{
			if (!holdSuperFired)
			{
				if (gameObject != null)
				{
					startSecondaryAttack(gameObject);
				}
				else if (combatController.getSecondaryAttackData(combatController.selectedSecondaryAttack).targetlessAttack && GameController.GetController().AllowTargetlessAttacks())
				{
					combatController.beginAttack(null, true);
				}
			}
		}
		else if (SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Left))
		{
			bool flag4 = false;
			flag3 = true;
			if (gameObject != null && nextClickIsDebug)
			{
				InspectObject(gameObject);
				nextClickIsDebug = false;
			}
			else if (gameObject != null && allowObjectInteraction)
			{
				if (!flag4 && combatController.IsObjectEnemy(gameObject))
				{
					flag4 = true;
					if (combatController.IsEnemyAttackable(gameObject))
					{
						timeSinceEnemyClick = 0f;
						enemyClickPosition = SHSInput.mousePosition;
						gameObject.SendMessage("OnMouseLeftClick", null, SendMessageOptions.DontRequireReceiver);
						combatMode = true;
						((PlayerCombatController)combatController).autoChaining = true;
						if (combatController.beginAttack(gameObject, false))
						{
							flag2 = false;
						}
						else
						{
							flag2 = false;
							combatController.pursueTarget(gameObject, false);
						}
						currentInputActions[3] = true;
					}
					else
					{
						flag2 = false;
					}
				}
				else if (!flag4 && GameController.GetController() is SocialSpaceController)
				{
					ObjectPicker.PickResult pickResult = picker.PickPlayerOrObject();
					if (pickResult != null)
					{
						if (pickResult.type == ObjectPicker.PickResult.ResultType.Object)
						{
							gameObject = pickResult.gameObject;
						}
						else if (pickResult.type == ObjectPicker.PickResult.ResultType.Player)
						{
							(GameController.GetController() as SocialSpaceController).InteractWithPlayer(pickResult.gameObject);
							picker.SelectedPlayer = pickResult.gameObject;
							flag4 = true;
							flag2 = false;
						}
					}
				}
				if (!flag4 && testInteractableClick(gameObject))
				{
					flag4 = true;
					flag2 = false;
				}
				if (!flag4 && testHotSpotClick(gameObject))
				{
					flag4 = true;
					flag2 = false;
				}
				if (!flag4 && testThrowableClick(gameObject))
				{
					flag4 = true;
					flag2 = false;
				}
				previousClickHandled = flag4;
			}
			else
			{
				previousClickHandled = false;
				if (timeSinceEnemyClick < 1f && Vector3.Distance(SHSInput.mousePosition, enemyClickPosition) < 100f)
				{
					previousClickHandled = true;
				}
			}
			timeSinceMoveClick = 0f;
			moveTimeLeft = 0f;
			GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Click);
		}
		if (mouseHoldTime >= 0.1f)
		{
			playerKeyCode.code = KeyCode.Mouse0;
			playerKeyCode.keyName = string.Empty;
			SHSInput.RegisterBlockTestOverrideCode(playerKeyCode);
			if (mouseHoldTime >= 0.25f)
			{
				previousClickHandled = false;
				if (playerMoveEffectInstance != null)
				{
					// CSP - temporarily comment out block
					CoroutineContainer component = playerMoveEffectInstance.GetComponent<CoroutineContainer>();
					if (component.enabled)
					{
						component.enabled = true;
						component.StartCoroutine(CoShrinkMoveEffect(playerMoveEffectInstance, 0f));
						playerMoveEffectInstance = null;
					}
				}
			}
		}
		if (SHSInput.GetMouseButton(SHSInput.MouseButtonType.Left))
		{
			mouseHoldTime += Time.deltaTime;
			if (!combatMode)
			{
				flag2 = true;
			}
		}
		else
		{
			mouseHoldTime = 0f;
			if (combatMode)
			{
				combatMode = false;
			}
			if (moveTimeLeft > 0f)
			{
				currentInputActions[0] = true;
				moveTimeLeft -= Time.deltaTime;
				Vector3 vector = base.transform.position - lastPosition;
				lastPosition = base.transform.position;
				float num = motionController.speed * Time.deltaTime * 0.1f;
				if (moveTimeLeft <= 0f || vector.sqrMagnitude < num * num)
				{
					moveTimeLeft = 0f;
					motionController.stopGently();
				}
			}
		}
		SHSInput.DestroyBlockTestOverrideCode(playerKeyCode);
		if (!previousClickHandled && flag2)
		{
			Vector3 a = Camera.main.WorldToScreenPoint(combatController.TargetPosition);
			if (flag3)
			{
				mouseCenter = a;
			}
			else
			{
				Vector3 vector2 = SHSInput.mousePosition - lastMousePosition;
				Vector3 vector3 = a - mouseCenter;
				if (vector3.x <= 0f)
				{
					vector2.x = Mathf.Min(vector2.x, 0f);
					vector3.x = Mathf.Max(vector3.x, vector2.x);
				}
				if (vector3.x >= 0f)
				{
					vector2.x = Mathf.Max(vector2.x, 0f);
					vector3.x = Mathf.Min(vector3.x, vector2.x);
				}
				if (vector3.y <= 0f && vector2.y <= 0f)
				{
					vector2.y = Mathf.Min(vector2.y, 0f);
					vector3.y = Mathf.Max(vector3.y, vector2.y);
				}
				if (vector3.y >= 0f && vector2.y >= 0f)
				{
					vector2.y = Mathf.Max(vector2.y, 0f);
					vector3.y = Mathf.Min(vector3.y, vector2.y);
				}
				mouseCenter += vector3;
			}
			Vector3 offset = SHSInput.mousePosition - mouseCenter;
			MoveByOffset(offset);
			lastMousePosition = SHSInput.mousePosition;
			currentInputActions[1] = true;
		}
		if (SHSInput.GetMouseButtonUp(SHSInput.MouseButtonType.Left))
		{
			if (previousClickHandled)
			{
				previousClickHandled = false;
			}
			else
			{
				if (timeSinceMoveClick > 0.25f)
				{
					motionController.stopGently();
				}
				else
				{
					Vector3 position = base.transform.position;
					position = ((!(GameController.GetController() is SocialSpaceController)) ? calcPointOnPlane(true) : GetGroundPoint());
					PlayPlayerMoveEffect(position);
					motionController.setDestination(position);
					behaviorManager.getBehavior().destinationChanged();
					moveTimeLeft = (position - base.transform.position).magnitude / motionController.speed;
					if (playerMoveEffectInstance != null)
					{
						float delay = (!(moveTimeLeft > minNavLifetime)) ? minNavLifetime : moveTimeLeft;
						CoroutineContainer component2 = playerMoveEffectInstance.GetComponent<CoroutineContainer>();
						component2.enabled = true;
						component2.StartCoroutine(CoShrinkMoveEffect(playerMoveEffectInstance, delay));
					}
					lastPosition = base.transform.position;
					currentInputActions[0] = true;
				}
				GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Normal);
			}
			if (gameObject != null)
			{
				gameObject.SendMessage("OnMouseLeftRelease", null, SendMessageOptions.DontRequireReceiver);
			}
		}
		playerKeyCode.code = KeyCode.None;
		playerKeyCode.keyName = "Jump";
		SHSInput.RegisterBlockTestOverrideCode(playerKeyCode);
		if (SHSInput.GetButtonDown("Jump"))
		{
			NewTutorialManager.sendEvent(TutorialEvent.TutorialEventType.HERO_JUMP);
			motionController.jumpPressed();
			currentInputActions[2] = true;
		}
		else if (SHSInput.GetButtonUp("Jump"))
		{
			motionController.jumpReleased();
		}
		SHSInput.DestroyBlockTestOverrideCode(playerKeyCode);
		if (SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Left))
		{
			timeSinceLeftClick = 0.25f;
			framesSinceLeftClick = 4;
		}
		else if ((timeSinceLeftClick > 0f || framesSinceLeftClick > 0) && SHSInput.GetMouseButtonUp(SHSInput.MouseButtonType.Left))
		{
			timeSinceLeftClick = 0f;
			framesSinceLeftClick = 0;
		}
		if (SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Right))
		{
			if (CurrentSuperActivationMode == SuperActivationMode.Hold)
			{
				timeSinceRightClick = 1f;
			}
			else
			{
				timeSinceRightClick = 0.25f;
			}
			framesSinceRightClick = 4;
		}
		else if ((timeSinceRightClick > 0f || framesSinceRightClick > 0) && SHSInput.GetMouseButtonUp(SHSInput.MouseButtonType.Right))
		{
			timeSinceRightClick = 0f;
			framesSinceRightClick = 0;
		}
		bool flag5 = false;
		if (SHSInput.GetButtonDown("SuperMove") && keyboardControlLevel > 0)
		{
			flag5 = true;
		}
		if (CurrentSuperActivationMode == SuperActivationMode.BothButton && (timeSinceLeftClick > 0f || framesSinceLeftClick > 0) && (timeSinceRightClick > 0f || framesSinceRightClick > 0))
		{
			flag5 = true;
		}
		if (timeSinceLeftClick > 0f)
		{
			timeSinceLeftClick -= Time.deltaTime;
		}
		if (timeSinceRightClick > 0f)
		{
			timeSinceRightClick -= Time.deltaTime;
			if (timeSinceRightClick <= 0f && CurrentSuperActivationMode == SuperActivationMode.Hold && SHSInput.GetMouseButton(SHSInput.MouseButtonType.Right))
			{
				flag5 = true;
				holdSuperFired = true;
			}
		}
		if (framesSinceLeftClick > 0)
		{
			framesSinceLeftClick--;
		}
		if (framesSinceRightClick > 0)
		{
			framesSinceRightClick--;
		}
		if (flag5)
		{
			if (combatController is PlayerCombatController && !(GameController.GetController() is SocialSpaceController))
			{
				((PlayerCombatController)combatController).UsePower();
				timeSinceLeftClick = 0f;
				timeSinceRightClick = 0f;
				framesSinceLeftClick = 0;
				framesSinceRightClick = 0;
			}
			currentInputActions[5] = true;
		}
		if (behaviorManager.allowUserInput() && keyboardControlLevel > 1 && DirectCombat())
		{
			combatMode = true;
		}
		if (!combatMode && behaviorManager.allowUserInput() && keyboardControlLevel > 0)
		{
			bool flag6 = false;
			if (keyboardControlLevel > 1)
			{
				flag6 = UsedInteractable();
			}
			if (flag6)
			{
				ResetKeyboard();
			}
			else
			{
				float num2 = Camera.main.pixelWidth * 0.5f;
				keyboardOffset.x = SHSInput.GetAxisRaw("Horizontal") * num2;
				keyboardOffset.y = SHSInput.GetAxisRaw("Vertical") * num2;
				if (keyboardOffset.x != 0f || keyboardOffset.y != 0f)
				{
					directionAsleep = false;
				}
				if (!directionAsleep)
				{
					currentInputActions[1] = true;
					MoveByOffset(keyboardOffset);
					if (keyboardOffset.x == 0f && keyboardOffset.y == 0f)
					{
						directionAsleep = true;
					}
				}
				keyboardDirection = (int)SHSInput.GetAxisRaw("Rotate");
				if (keyboardDirection != 0)
				{
					facingAsleep = false;
				}
				if (!facingAsleep)
				{
					RotateByOffset(keyboardDirection);
					if (keyboardDirection == 0)
					{
						facingAsleep = true;
					}
				}
			}
		}
		else
		{
			ResetKeyboard();
		}
		if (SHSInput.GetKeyDown(KeyCode.R) && GameController.GetController() is SocialSpaceController)
		{
			SHSFractalsActivity sHSFractalsActivity = AppShell.Instance.ActivityManager.GetActivity("fractalsactivity") as SHSFractalsActivity;
			NotificationHUD.addNotification(new DailyFractalNotificationData(sHSFractalsActivity.GetFractalCollectionCount(), sHSFractalsActivity.MaxFractals));
			SHSGoldenFractalActivity sHSGoldenFractalActivity = AppShell.Instance.ActivityManager.GetActivity("goldenfractalactivity") as SHSGoldenFractalActivity;
			NotificationHUD.addNotification(new DailyGoldenFractalNotificationData(sHSGoldenFractalActivity.GetFractalCollectionCount(), 1));
			HeroPersisted value2;
			if (AppShell.Instance.Profile.AvailableCostumes.TryGetValue(AppShell.Instance.Profile.SelectedCostume, out value2))
			{
				NotificationHUD.addNotification(new DailyScavengerNotificationData(value2.objectsCollected, value2.maxScavengeObjects));
			}
			SHSSeasonalActivity sHSSeasonalActivity = AppShell.Instance.ActivityManager.GetActivity("seasonalactivity") as SHSSeasonalActivity;
			if (sHSSeasonalActivity != null)
			{
				NotificationHUD.addNotification(new DailySeasonalNotificationData(sHSSeasonalActivity.GetCollectionCount(), sHSSeasonalActivity.Max, sHSSeasonalActivity.Icon));
			}
			SHSRareSeasonalActivity sHSRareSeasonalActivity = AppShell.Instance.ActivityManager.GetActivity("rareseasonalactivity") as SHSRareSeasonalActivity;
			if (sHSRareSeasonalActivity != null)
			{
				NotificationHUD.addNotification(new DailyRareSeasonalNotificationData(sHSRareSeasonalActivity.GetCollectionCount(), 1, sHSRareSeasonalActivity.Icon));
			}
			AppShell.Instance.EventMgr.Fire(this, new NotificationRefreshRequest());
		}
		if (SHSInput.GetKeyDown(KeyCode.Alpha1))
		{
			combatController.SelectSecondaryAttack(KeyCode.Alpha1);
		}
		if (SHSInput.GetKeyDown(KeyCode.Alpha2))
		{
			combatController.SelectSecondaryAttack(KeyCode.Alpha2);
		}
		if (SHSInput.GetKeyDown(KeyCode.Alpha3))
		{
			combatController.SelectSecondaryAttack(KeyCode.Alpha3);
		}
		if (SHSInput.GetKeyDown(KeyCode.Alpha4))
		{
			combatController.SelectSecondaryAttack(KeyCode.Alpha4);
		}
		if (SHSInput.GetKeyDown(KeyCode.Alpha5))
		{
			combatController.SelectSecondaryAttack(KeyCode.Alpha5);
		}
		lastMousePosition = SHSInput.mousePosition;
		SHSInput.DestroyBlockTestOverrideCode(playerKeyCode);
	}

	protected void startSecondaryAttack(GameObject target)
	{
		if (!combatController.IsObjectEnemy(target) || !combatController.IsEnemyAttackable(target))
		{
			return;
		}
		((PlayerCombatController)combatController).autoChaining = true;
		if (!combatController.beginAttack(target, true))
		{
			if (combatController.IsAttackAvailable(true))
			{
				combatController.pursueTarget(target, true);
				currentInputActions[4] = true;
			}
		}
		else
		{
			currentInputActions[4] = true;
		}
	}

	private IEnumerator CoShrinkMoveEffect(GameObject go, float delay)
	{
		yield return new WaitForSeconds(delay);
		GameObject topGO = go.transform.FindChild("top_layer").gameObject;
		topGO.animation["Take 001"].time = topGO.animation["Take 001"].length;
		topGO.animation["Take 001"].speed = -1f;
		topGO.animation.Play("Take 001");
		GameObject botGO = go.transform.FindChild("bottom_layer").gameObject;
		botGO.animation["Take 001"].time = botGO.animation["Take 001"].length;
		botGO.animation["Take 001"].speed = -1f;
		botGO.animation.Play("Take 001");
		yield return new WaitForSeconds(topGO.animation["Take 001"].length);
		Object.Destroy(go);
	}

	protected Vector3 calcPointOnPlane(bool useCombatPosition)
	{
		Ray ray = Camera.main.ScreenPointToRay(SHSInput.mousePosition);
		float num = 0f;
		if (useCombatPosition)
		{
			if (combatController != null)
			{
				Vector3 origin = ray.origin;
				float y = origin.y;
				Vector3 targetPosition = combatController.TargetPosition;
				num = y - targetPosition.y;
			}
			else
			{
				Vector3 origin2 = ray.origin;
				float y2 = origin2.y;
				Vector3 position = base.transform.position;
				num = y2 - (position.y + charGlobals.characterController.height / 2f);
			}
		}
		else
		{
			Vector3 origin3 = ray.origin;
			float y3 = origin3.y;
			Vector3 position2 = base.transform.position;
			num = y3 - position2.y;
		}
		float num2 = num;
		Vector3 direction = ray.direction;
		float num3 = num2 / direction.y;
		Vector3 result = ray.origin + ray.direction * (0f - num3);
		Vector3 position3 = base.transform.position;
		result.y = position3.y;
		return result;
	}

	protected Vector3 GetGroundPoint()
	{
		Ray ray = Camera.main.ScreenPointToRay(SHSInput.mousePosition);
		RaycastHit[] array = Physics.RaycastAll(ray, float.PositiveInfinity, 100608);
		if (array.Length > 0)
		{
			float num = -1f;
			RaycastHit raycastHit = array[0];
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit2 = array2[i];
				Vector3 point = raycastHit2.point;
				float y = point.y;
				Vector3 position = base.transform.position;
				float num2 = Mathf.Abs(y - position.y);
				if (num < 0f || num2 < num)
				{
					num = num2;
					raycastHit = raycastHit2;
				}
			}
			return raycastHit.point;
		}
		return base.transform.position;
	}

	private void MoveByOffset(Vector3 offset)
	{
		Quaternion rotation = Camera.main.transform.rotation;
		if (offset.magnitude < Camera.main.pixelWidth * 0.05f)
		{
			if (!stopped)
			{
				motionController.stopGently();
				stopped = true;
			}
			return;
		}
		stopped = false;
		offset.z = offset.y;
		offset.y = 0f;
		offset.Normalize();
		offset *= motionController.speed;
		offset = rotation * offset;
		motionController.setDestination(base.transform.position + offset);
		behaviorManager.getBehavior().destinationChanged();
	}

	private void RotateByOffset(int offset)
	{
		if (offset < 0)
		{
			motionController.RotateInPlace(180f, true);
			behaviorManager.getBehavior().destinationChanged();
		}
		if (offset > 0)
		{
			motionController.RotateInPlace(180f, false);
			behaviorManager.getBehavior().destinationChanged();
		}
	}

	private bool UsedInteractable()
	{
		if (motionController.carriedThrowable != null)
		{
			return false;
		}
		if (SHSInput.GetButtonDown("Interact"))
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, interactRange, 1077760);
			InteractiveObject interactiveObject = null;
			ThrowableGround throwableGround = null;
			float num = interactRange * interactRange * 100f;
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				float sqrMagnitude = (collider.transform.position - base.transform.position).sqrMagnitude;
				if (!(sqrMagnitude < num))
				{
					continue;
				}
				InteractiveObject interactiveObject2 = null;
				InteractiveObjectForwarder component = Utils.GetComponent<InteractiveObjectForwarder>(collider.gameObject);
				interactiveObject2 = ((!(component != null) || (component.options & InteractiveObjectForwarder.Options.Click) == 0) ? Utils.GetComponent<InteractiveObject>(collider.gameObject) : component.owner);
				if (interactiveObject2 != null)
				{
					if (interactiveObject2.WillAcceptMouseClick(base.gameObject))
					{
						interactiveObject = interactiveObject2;
						throwableGround = null;
						num = sqrMagnitude;
					}
					continue;
				}
				ThrowableGround component2 = Utils.GetComponent<ThrowableGround>(collider.gameObject);
				if (component2 != null)
				{
					interactiveObject = null;
					throwableGround = component2;
					num = sqrMagnitude;
				}
			}
			if (interactiveObject != null)
			{
				return interactiveObject.OnMouseClick(base.gameObject);
			}
			if (throwableGround != null)
			{
				return throwableGround.OnMouseClick(charGlobals);
			}
		}
		return false;
	}

	private bool DirectCombat()
	{
		if (BrawlerController.Instance == null)
		{
			return false;
		}
		bool flag = false;
		bool flag2 = false;
		if (SHSInput.GetButtonDown("LeftAttack"))
		{
			flag = true;
		}
		if (SHSInput.GetButtonDown("RightAttack"))
		{
			flag = true;
			flag2 = true;
		}
		if (motionController.carriedThrowable != null && SHSInput.GetButtonDown("Interact"))
		{
			flag = true;
		}
		if (flag)
		{
			CombatController.AttackData currentAttackData = combatController.getCurrentAttackData(flag2, false);
			if (currentAttackData == null)
			{
				return false;
			}
			float num = Mathf.Min(Mathf.Max(currentAttackData.maximumRange * 1.5f, 3f), 20f);
			Collider[] array = Physics.OverlapSphere(base.transform.position, num, 1077760);
			GameObject gameObject = null;
			float num2 = 0f;
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				if (!combatController.IsObjectEnemy(collider.gameObject))
				{
					continue;
				}
				Vector3 from = collider.transform.position - base.transform.position;
				from.y = 0f;
				float num3 = 0f;
				float num4 = 1f - Mathf.Min(from.magnitude / num, 1f);
				float num5 = num4 * 1f;
				num3 += num5;
				float num6 = 45f + 12.5f * num4;
				Vector3 forward = base.gameObject.transform.forward;
				forward.y = 0f;
				float num7 = Vector3.Angle(from, forward);
				if (!(num7 > num6))
				{
					float num8 = (1f - num7 / num6) * 1f;
					num3 += num8;
					if (num2 < num3)
					{
						gameObject = collider.gameObject;
						num2 = num3;
					}
				}
			}
			((PlayerCombatController)combatController).autoChaining = false;
			if (gameObject != null)
			{
				if (!combatController.beginAttack(gameObject, flag2))
				{
					combatController.pursueTarget(gameObject, flag2);
				}
			}
			else if (!flag2)
			{
				combatController.beginAttack(null, flag2);
			}
		}
		return false;
	}

	private void ResetKeyboard()
	{
		keyboardOffset.x = 0f;
		keyboardOffset.y = 0f;
		directionAsleep = false;
		keyboardDirection = 0;
		facingAsleep = false;
	}

	private void OnEmote(EmoteMessage msg)
	{
		if (base.gameObject.name.Contains("placeholder"))
		{
			CspUtils.DebugLog("Disallowed Mr. Placeholder from emoting.");
		}
		else
		{
			string failReason;
			if (!EmotesDefinition.Instance.RequirementsCheck(msg.emote, base.gameObject, out failReason) || (charGlobals.spawnData != null && (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Player) == 0) || !(Time.time - lastEmoteTime > 1f) || !behaviorManager.allowUserInput() || !motionController.IsOnGround())
			{
				return;
			}
			if (gameController.LocalPlayer == base.gameObject && AppShell.Instance != null && AppShell.Instance.Profile != null)
			{
				EmotesDefinition.EmoteDefinition emoteById = EmotesDefinition.Instance.GetEmoteById(msg.emote);
				AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "emote", emoteById.command, 3f);
				AppShell.callAnalytics("player", "emote_button", emoteById.command, string.Empty);
			}
			lastEmoteTime = Time.time;
			SocialSpaceController socialSpaceController = GameController.GetController() as SocialSpaceController;
			if (socialSpaceController != null)
			{
				socialSpaceController.DoEmote(base.gameObject, msg.emote);
				return;
			}
			BehaviorEmote behaviorEmote = behaviorManager.requestChangeBehavior(typeof(BehaviorEmote), false) as BehaviorEmote;
			if (behaviorEmote != null && !behaviorEmote.Initialize(msg.emote))
			{
				behaviorManager.endBehavior();
			}
		}
	}

	private void OnPowerMove(PowerMoveMessage msg)
	{
		if (combatController is PlayerCombatController)
		{
			((PlayerCombatController)combatController).UsePower();
			currentInputActions[5] = true;
		}
	}

	private void OnEntityFactionChange(EntityFactionChangeMessage msg)
	{
		if (IsMouseOver(msg.go))
		{
			ForceMouseRollverUpdate();
		}
	}

	private void UntargettedThrowReady(GameObject obj)
	{
		if (BrawlerController.Instance != null && BrawlerController.Instance.throwableTargetPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(BrawlerController.Instance.throwableTargetPrefab) as GameObject;
			gameObject.transform.position = throwTargetPosition;
			charGlobals.networkComponent.AnnounceObjectSpawn(gameObject, "PlayerCombatController", BrawlerController.Instance.throwableTargetPrefab.name);
			combatController.beginAttack(gameObject, false, true);
		}
		else
		{
			combatController.beginAttack(null, false);
		}
	}

	private void PlayPlayerMoveEffect(Vector3 pos)
	{
		if (playerMoveEffectResource == null)
		{
			playerMoveEffectResource = Resources.Load("GUI/Navigation/Navigation_prefab");
		}
		if (playerMoveEffectInstance != null)
		{
			Object.Destroy(playerMoveEffectInstance);
		}
		GUIManager.Instance.DetachAttackingIndicator();
		playerMoveEffectInstance = (Object.Instantiate(playerMoveEffectResource, pos, Quaternion.identity) as GameObject);
		GameObject gameObject = playerMoveEffectInstance.transform.FindChild("top_layer").gameObject;
		GameObject gameObject2 = playerMoveEffectInstance.transform.FindChild("bottom_layer").gameObject;
		gameObject.animation.Play("Take 001");
		gameObject2.animation.Play("Take 001");
	}

	private void InspectObject(GameObject obj)
	{
		GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, GetObjectLineageStr(obj, 0), (IGUIDialogNotification)null, GUIControl.ModalLevelEnum.Default);
	}

	private string GetObjectLineageStr(GameObject obj, int depth)
	{
		string text = string.Empty;
		for (int i = 0; i < 10 - depth; i++)
		{
			text += " ";
		}
		if (obj.transform.parent == null)
		{
			return text + obj.name;
		}
		return GetObjectLineageStr(obj.transform.parent.gameObject, depth + 1) + "\n" + text + obj.name;
	}

	public void OnCutSceneStart()
	{
		AllowInput = false;
		behaviorManager.OnCutSceneStart();
	}

	public void OnCutSceneEnd()
	{
		AllowInput = true;
	}

	public void DebugNextClick()
	{
		nextClickIsDebug = true;
	}

	public bool IsMouseOver(GameObject obj)
	{
		return lastRolloverObject == obj;
	}

	public void FireDestructionMessage()
	{
		if (!firedDestructionMessage)
		{
			AppShell.Instance.EventMgr.Fire(this, new PlayerInputControllerDestroyed(this));
			firedDestructionMessage = true;
		}
	}

	public void OnNPCReactMessage(NPCEmoteReactMessage msg)
	{
		if (msg.target == base.gameObject && (msg.reactionAnim == "emote_clap" || msg.reactionAnim == "emote_cheer" || msg.reactionAnim == "emote_approve" || msg.reactionAnim == "emote_laugh" || msg.reactionAnim == "emote_dance"))
		{
			string zoneName = SocialSpaceControllerImpl.getZoneName();
			AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "npc_impress", OwnableDefinition.simpleZoneName(zoneName), 3f);
		}
	}
}
