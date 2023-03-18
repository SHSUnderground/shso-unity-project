using System;
using System.Collections.Generic;
using UnityEngine;

public class HqInput : MonoBehaviour, IInputHandler
{
	public enum FlingaInputMode
	{
		FlingaHand,
		SlingShot
	}

	[Flags]
	protected enum ScrollRegion
	{
		None = 0x0,
		Top = 0x1,
		Bottom = 0x2,
		Right = 0x4,
		Left = 0x8
	}

	internal class ViewMode : IDisposable, IShsState
	{
		protected AIControllerHQ selectedAI;

		protected HqObject2 selectedObject;

		protected HqInput parent;

		internal ViewMode(HqInput parent)
		{
			this.parent = parent;
		}

		public void Dispose()
		{
			parent = null;
		}

		public void Enter(Type previousState)
		{
			if (parent.block != null)
			{
				CspUtils.DebugLog("Unexpected state in ViewMode.Enter");
			}
			SelectedObjectController component = Utils.GetComponent<SelectedObjectController>(Camera.main);
			if (!(component != null))
			{
				return;
			}
			int num = 0;
			AIControllerHQ component2;
			while (true)
			{
				if (num < component.Count)
				{
					GameObject objectAt = component.GetObjectAt(num);
					component2 = Utils.GetComponent<AIControllerHQ>(objectAt, Utils.SearchChildren);
					if (component2 != null)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			selectedAI = component2;
		}

		public void Update()
		{
			parent.UpdateScroll();
			if (SHSInput.IsOverUI())
			{
				return;
			}
			HqObject2 hqObject = null;
			AIControllerHQ aIControllerHQ = null;
			if (SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Left) || SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Right))
			{
				Ray ray = Camera.main.ScreenPointToRay(SHSInput.mousePosition);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, 1000f, 3174912))
				{
					if (hitInfo.collider.gameObject == null)
					{
						CspUtils.DebugLog("hitInfo.collider.gameObject is null!");
						return;
					}
					GameObject gameObject = hitInfo.collider.gameObject;
					while (hqObject == null && gameObject != null)
					{
						HqSwitchObject component = Utils.GetComponent<HqSwitchObject>(gameObject);
						if (component != null)
						{
							component.OnMouseClick(gameObject);
							return;
						}
						hqObject = Utils.GetComponent<HqObject2>(gameObject);
						if (hqObject != null)
						{
							break;
						}
						aIControllerHQ = Utils.GetComponent<AIControllerHQ>(gameObject);
						if (aIControllerHQ != null && aIControllerHQ.CanPickUp)
						{
							break;
						}
						if (gameObject.transform.parent == null)
						{
							return;
						}
						gameObject = gameObject.transform.parent.gameObject;
					}
				}
			}
			if (hqObject != null)
			{
				HqItem component2 = Utils.GetComponent<HqItem>(hqObject.gameObject);
				if (!SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Right))
				{
					if (component2 != null)
					{
						component2.CancelUsers();
					}
					parent.blockOld = true;
					parent.blockOldPos = hqObject.gameObject.transform.position;
					parent.blockOldRot = hqObject.gameObject.transform.rotation;
					DragDropInfo dragDropInfo = new DragDropInfo(hqObject.gameObject);
					dragDropInfo.CollectionId = DragDropInfo.CollectionType.Items;
					dragDropInfo.ItemId = hqObject.InventoryId;
					dragDropInfo.IconSource = hqObject.IconSource;
					dragDropInfo.IconSize = new Vector2(59f, 59f);
					dragDropInfo.Result = DragDropResult.Pending;
					AppShell.Instance.EventMgr.Fire(this, new ItemSelectedMessage(dragDropInfo.ItemId));
					AppShell.Instance.EventMgr.Fire(this, new GUIDragBeginMessage(dragDropInfo));
					return;
				}
				if (selectedAI != null && component2 != null)
				{
					selectedAI.CurrentActivityItem = component2;
				}
			}
			if (!(aIControllerHQ != null))
			{
				return;
			}
			if (SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Right))
			{
				if (selectedAI != null && selectedAI != aIControllerHQ)
				{
					selectedAI.Selected = false;
				}
				aIControllerHQ.Selected = true;
				selectedAI = aIControllerHQ;
				return;
			}
			DragDropInfo dragDropInfo2 = new DragDropInfo(aIControllerHQ.gameObject);
			dragDropInfo2.CollectionId = DragDropInfo.CollectionType.Heroes;
			dragDropInfo2.ItemId = aIControllerHQ.name;
			dragDropInfo2.IconSource = aIControllerHQ.InventoryIcon;
			dragDropInfo2.IconSize = new Vector2(103f, 103f);
			dragDropInfo2.Result = DragDropResult.Pending;
			AppShell.Instance.EventMgr.Fire(this, new ItemSelectedMessage(dragDropInfo2.ItemId));
			AppShell.Instance.EventMgr.Fire(this, new GUIDragBeginMessage(dragDropInfo2));
			parent.blockOld = true;
			parent.blockOldPos = aIControllerHQ.gameObject.transform.position;
			aIControllerHQ.ChangeBehavior<BehaviorPaused>(true);
		}

		public void Leave(Type nextState)
		{
			if (selectedAI != null)
			{
				selectedAI.Selected = false;
			}
		}
	}

	internal class PlacementMode : IDisposable, IShsState
	{
		protected HqInput parent;

		internal PlacementMode(HqInput parent)
		{
			this.parent = parent;
		}

		public void Dispose()
		{
			parent = null;
		}

		public void Enter(Type previousState)
		{
			if (parent.block == null)
			{
				HqController2.Instance.GotoViewMode();
			}
		}

		public void Update()
		{
			if (parent.block == null)
			{
				HqController2.Instance.GotoViewMode();
			}
			parent.UpdateScroll();
			if (parent.displayingDialog)
			{
				Utils.ForEachTree(parent.block, delegate(GameObject go)
				{
					if (go.renderer != null)
					{
						go.renderer.enabled = false;
					}
				});
				GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Interactable);
				return;
			}
			AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(parent.block);
			bool flag = true;
			bool flag2 = false;
			if (SHSInput.IsOverUI())
			{
				if (component == null)
				{
					Utils.ForEachTree(parent.block, delegate(GameObject go)
					{
						if (go != null && go.renderer != null)
						{
							if (go.particleEmitter != null)
							{
								go.particleEmitter.enabled = false;
							}
							go.renderer.enabled = false;
						}
					});
				}
				else
				{
					component.StopRendering();
				}
				flag2 = true;
				flag = false;
			}
			else
			{
				HqObject2 component2 = Utils.GetComponent<HqObject2>(parent.block);
				if (parent.block != null && component2 != null)
				{
					Utils.ForEachTree(parent.block, delegate(GameObject go)
					{
						if (go != null && go.renderer != null)
						{
							if (go.particleEmitter != null)
							{
								go.particleEmitter.enabled = true;
							}
							go.renderer.enabled = true;
						}
					});
					bool flag3 = Utils.GetComponent<HqAIProxy>(parent.block) != null;
					Ray ray = Camera.main.ScreenPointToRay(SHSInput.mousePosition);
					int layerMask = 1147392;
					if (!parent.ignoreBlocks)
					{
						layerMask = 1155584;
					}
					flag = true;
					RaycastHit hitInfo;
					if (Physics.Raycast(ray, out hitInfo, 1000f, layerMask))
					{
						if (parent.ignoreSides)
						{
							float num = Vector3.Angle(Vector3.up, hitInfo.normal);
							if (num >= 70f)
							{
								if (Physics.Raycast(ray, out hitInfo, 1000f, 98304))
								{
									flag = !component2.PlaceAt(hitInfo.point);
								}
							}
							else
							{
								flag = !component2.PlaceAt(hitInfo.point);
							}
						}
						else
						{
							flag = !component2.PlaceAt(hitInfo.point);
						}
					}
					if (flag3)
					{
						if (SHSInput.GetKey(KeyCode.RightArrow))
						{
							parent.block.transform.RotateAround(parent.block.transform.up, parent.rotateSpeed * Time.deltaTime);
						}
						if (SHSInput.GetKey(KeyCode.LeftArrow))
						{
							parent.block.transform.RotateAround(parent.block.transform.up, (0f - parent.rotateSpeed) * Time.deltaTime);
						}
						if (SHSInput.GetKey(KeyCode.Home))
						{
							parent.block.transform.rotation = Quaternion.identity;
						}
					}
					else
					{
						if (SHSInput.GetKey(KeyCode.RightArrow))
						{
							parent.block.transform.RotateAround(parent.block.transform.forward, parent.rotateSpeed * Time.deltaTime);
						}
						if (SHSInput.GetKey(KeyCode.LeftArrow))
						{
							parent.block.transform.RotateAround(parent.block.transform.forward, (0f - parent.rotateSpeed) * Time.deltaTime);
						}
						if (SHSInput.GetKey(KeyCode.UpArrow))
						{
							parent.block.transform.RotateAround(parent.block.transform.right, parent.rotateSpeed * Time.deltaTime);
						}
						if (SHSInput.GetKey(KeyCode.DownArrow))
						{
							parent.block.transform.RotateAround(parent.block.transform.right, (0f - parent.rotateSpeed) * Time.deltaTime);
						}
						if (SHSInput.GetKey(KeyCode.Home))
						{
							HqItem item = HqController2.Instance.ActiveRoom.GetItem(component2.PlacedId);
							if (item != null)
							{
								item.ResetRotation();
							}
						}
					}
				}
				else if (component != null)
				{
					component.StartRendering();
					Ray ray2 = Camera.main.ScreenPointToRay(SHSInput.mousePosition);
					int layerMask2 = 106496;
					RaycastHit hitInfo2;
					if (Physics.Raycast(ray2, out hitInfo2, 1000f, layerMask2))
					{
						float num2 = Vector3.Angle(Vector3.up, hitInfo2.normal);
						if (num2 >= 70f)
						{
							if (Physics.Raycast(ray2, out hitInfo2, 1000f, 98304))
							{
								component.PlaceAt(hitInfo2.point);
							}
						}
						else
						{
							component.PlaceAt(hitInfo2.point);
						}
						flag = false;
					}
					if (SHSInput.GetKey(KeyCode.RightArrow))
					{
						component.gameObject.transform.RotateAround(Vector3.up, parent.rotateSpeed * Time.deltaTime);
					}
					if (SHSInput.GetKey(KeyCode.LeftArrow))
					{
						component.gameObject.transform.RotateAround(Vector3.up, (0f - parent.rotateSpeed) * Time.deltaTime);
					}
					if (SHSInput.GetKey(KeyCode.Home))
					{
						component.gameObject.transform.rotation = Quaternion.identity;
					}
				}
			}
			if (!flag)
			{
				SelectedObjectController component3 = Utils.GetComponent<SelectedObjectController>(Camera.main);
				if (component3 != null)
				{
					component3.pulseColor = Color.white;
				}
				if (!flag2 && SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Left))
				{
					DragDropInfo currentDragDropInfo = parent.CurrentDragDropInfo;
					currentDragDropInfo.TargetType = DragDropSourceType.World;
					currentDragDropInfo.WorldTarget = parent.block;
					currentDragDropInfo.Result = DragDropResult.WorldDropped;
					AppShell.Instance.EventMgr.Fire(this, new GUIDragEndMessage(parent.CurrentDragDropInfo));
				}
			}
			else if (!flag2)
			{
				GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Uninteractable);
				SelectedObjectController component4 = Utils.GetComponent<SelectedObjectController>(Camera.main);
				if (component4 != null)
				{
					component4.pulseColor = Color.red;
				}
			}
		}

		public void Leave(Type nextState)
		{
			if (parent.block != null)
			{
				DragDropInfo currentDragDropInfo = parent.CurrentDragDropInfo;
				if (currentDragDropInfo != null)
				{
					currentDragDropInfo.TargetType = DragDropSourceType.World;
					currentDragDropInfo.WorldTarget = parent.block;
					currentDragDropInfo.Result = DragDropResult.Cancelled;
					AppShell.Instance.EventMgr.Fire(this, new GUIDragEndMessage(parent.CurrentDragDropInfo));
				}
				if (parent.block != null)
				{
					CspUtils.DebugLog("Deleting block!");
					HqController2.Instance.ActiveRoom.DelItem(parent.block);
					UnityEngine.Object.Destroy(parent.block);
					parent.block = null;
				}
			}
			GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Normal);
		}
	}

	internal class FlingaMode : IDisposable, IShsState
	{
		protected HqInput parent;

		internal FlingaMode(HqInput parent)
		{
			this.parent = parent;
		}

		public void Dispose()
		{
			parent = null;
		}

		public void Enter(Type previousState)
		{
			foreach (HqItem placedItem in HqController2.Instance.ActiveRoom.PlacedItems)
			{
				if (!(placedItem == null))
				{
					HqTrigger component = Utils.GetComponent<HqTrigger>(placedItem, Utils.SearchChildren);
					if (component != null)
					{
						placedItem.gameObject.active = false;
						placedItem.gameObject.active = true;
					}
				}
			}
		}

		public void Update()
		{
			parent.UpdateScroll();
			bool mouseButtonDown = SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Left);
			bool mouseButtonDown2 = SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Right);
			if (!mouseButtonDown && !mouseButtonDown2)
			{
				return;
			}
			Ray ray = Camera.main.ScreenPointToRay(SHSInput.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 2000f, 3174912))
			{
				HqObject2 hqObject = null;
				AIControllerHQ aIControllerHQ = null;
				GameObject gameObject = hitInfo.collider.gameObject;
				while (gameObject != null)
				{
					HqSwitchObject component = Utils.GetComponent<HqSwitchObject>(gameObject);
					if (component != null && mouseButtonDown)
					{
						component.OnMouseClick(gameObject);
						return;
					}
					hqObject = Utils.GetComponent<HqObject2>(gameObject);
					if (hqObject != null)
					{
						break;
					}
					aIControllerHQ = Utils.GetComponent<AIControllerHQ>(gameObject);
					if (aIControllerHQ != null && !aIControllerHQ.IsFlingaObjectActive)
					{
						if (mouseButtonDown2)
						{
							aIControllerHQ.Selected = true;
							HqController2.Instance.GotoViewMode();
							return;
						}
						break;
					}
					if (gameObject.transform.parent == null)
					{
						break;
					}
					gameObject = gameObject.transform.parent.gameObject;
				}
				if (mouseButtonDown2)
				{
					return;
				}
				if (aIControllerHQ != null)
				{
					if (!aIControllerHQ.CanPickUp)
					{
						return;
					}
					hqObject = aIControllerHQ.FlingaObject;
					aIControllerHQ.PickUpAI();
				}
				HqItem component2 = Utils.GetComponent<HqItem>(hqObject.gameObject);
				if (component2 != null)
				{
					component2.CancelUsers();
				}
				if (aIControllerHQ == null && component2 != null && (bool)component2.AIInControl)
				{
					CspUtils.DebugLog("Cannot move item because it is in AIControl.");
				}
				else if (hqObject != null)
				{
					parent.flingaBlock = hqObject;
					switch (parent.inputMode)
					{
					case FlingaInputMode.FlingaHand:
						parent.fsm.GotoState<FlingaHandMode>();
						break;
					case FlingaInputMode.SlingShot:
						parent.fsm.GotoState<SlingshotMode>();
						break;
					}
				}
			}
			else
			{
				CspUtils.DebugLog("Did not hit anything");
			}
		}

		public void Leave(Type nextState)
		{
			if (nextState != typeof(SlingshotMode) && nextState != typeof(FlingaHandMode))
			{
				parent.flingaBlock = null;
			}
		}
	}

	internal class FlingaHandMode : IDisposable, IInputHandler, IShsState
	{
		internal class IntroState : IDisposable, IShsState
		{
			internal FlingaHandMode parent;

			internal IntroState(FlingaHandMode parent)
			{
				this.parent = parent;
			}

			public void Dispose()
			{
				parent = null;
			}

			public void Enter(Type previousState)
			{
				parent.parent.flingaBlock.gameObject.active = false;
				parent.parent.flingaBlock.gameObject.active = true;
			}

			public void Update()
			{
				parent.fsm.GotoState<MoveState>();
			}

			public void Leave(Type nextState)
			{
				if (nextState != null)
				{
					parent.parent.flingaBlock.GotoFlingaSelectedMode();
				}
			}
		}

		internal class MoveState : IDisposable, IShsState
		{
			internal FlingaHandMode parent;

			internal Vector3 mouseDelta;

			internal Vector3 keyRotate;

			internal float mouseDownTime = -1f;

			internal float gestureTimeStart = -1f;

			internal float gestureTimeZero = -1f;

			internal MoveState(FlingaHandMode parent)
			{
				this.parent = parent;
			}

			public void Dispose()
			{
				parent = null;
			}

			public void Enter(Type previousState)
			{
				Screen.lockCursor = true;
				mouseDelta = Vector3.zero;
				mouseDownTime = -1f;
				gestureTimeStart = Time.time;
				gestureTimeZero = -1f;
				parent.parent.flingaBlock.rigidbody.isKinematic = false;
				parent.parent.flingaBlock.rigidbody.useGravity = true;
				parent.parent.flingaBlock.fixedUpdateFunction = FixedUpdate;
			}

			public void Update()
			{
				if (SHSInput.GetKey(KeyCode.Escape))
				{
				}
				float axis = SHSInput.GetAxis("Mouse X");
				float axis2 = SHSInput.GetAxis("Mouse Y");
				if (axis == 0f && axis2 == 0f)
				{
					if (gestureTimeZero >= 0f)
					{
						if (Time.time - gestureTimeZero >= 0.1f)
						{
							gestureTimeStart = Time.time;
						}
					}
					else
					{
						gestureTimeZero = Time.time;
					}
				}
				else
				{
					gestureTimeZero = -1f;
				}
				Vector3 normalized = Camera.main.transform.forward.normalized;
				Vector3 normalized2 = Camera.main.transform.right.normalized;
				normalized *= axis2 * parent.parent.tweakFlingaHand.mouseToVelocity;
				normalized2 *= axis * parent.parent.tweakFlingaHand.mouseToVelocity;
				normalized.y = 0f;
				normalized2.y = 0f;
				mouseDelta = normalized + normalized2;
				if (mouseDownTime < 0f)
				{
					if (SHSInput.GetKey(KeyCode.Mouse0))
					{
						mouseDownTime = Time.time;
					}
				}
				else if (!SHSInput.GetKey(KeyCode.Mouse0))
				{
					parent.gestureMouseDuration = Time.time - mouseDownTime;
					parent.gestureDuration = Time.time - gestureTimeStart;
					parent.fsm.GotoState<ThrowState>();
				}
				if (SHSInput.GetKey(KeyCode.RightArrow))
				{
					keyRotate = new Vector3(0f, parent.parent.rotateSpeed, 0f);
				}
				else if (SHSInput.GetKey(KeyCode.LeftArrow))
				{
					keyRotate = new Vector3(0f, 0f - parent.parent.rotateSpeed, 0f);
				}
				else if (SHSInput.GetKey(KeyCode.UpArrow))
				{
					keyRotate = new Vector3(parent.parent.rotateSpeed, 0f, 0f);
				}
				else if (SHSInput.GetKey(KeyCode.DownArrow))
				{
					keyRotate = new Vector3(0f - parent.parent.rotateSpeed, 0f, 0f);
				}
				else if (SHSInput.GetKey(KeyCode.Home))
				{
					if (parent.parent.flingaBlock.AIController == null)
					{
						HqItem item = HqController2.Instance.ActiveRoom.GetItem(parent.parent.flingaBlock.PlacedId);
						if (item != null)
						{
							item.ResetRotation();
						}
					}
					else
					{
						parent.parent.flingaBlock.gameObject.transform.rotation = Quaternion.identity;
					}
				}
				else
				{
					keyRotate = Vector3.zero;
				}
			}

			public void FixedUpdate()
			{
				HqObject2 flingaBlock = parent.parent.flingaBlock;
				Vector3 vector = mouseDelta / Time.deltaTime;
				if (vector.magnitude >= parent.parent.tweakFlingaHand.maxVelocity)
				{
					vector.Normalize();
					vector *= parent.parent.tweakFlingaHand.maxVelocity;
				}
				Vector3 velocity = flingaBlock.rigidbody.velocity;
				vector.y = Mathf.Clamp(velocity.y, -100f, 3f);
				RaycastHit hitInfo;
				if (PhysicsEx.ShapeCast(flingaBlock.rigidbody, vector, Time.deltaTime, out hitInfo, 4685824))
				{
					Vector3 a = vector - Vector3.Dot(hitInfo.normal, vector) * hitInfo.normal;
					a.Normalize();
					float magnitude = vector.magnitude;
					vector *= 0.1f;
					vector += a * (0.5f * magnitude);
				}
				flingaBlock.rigidbody.velocity = vector;
				flingaBlock.rigidbody.angularVelocity += keyRotate;
				Vector3 velocity2 = flingaBlock.rigidbody.velocity;
				velocity2.y = 0f;
				parent.speeds[parent.speedsIdx++ % parent.speeds.Length] = velocity2.magnitude;
			}

			public void Leave(Type nextState)
			{
				parent.parent.flingaBlock.fixedUpdateFunction = null;
				Screen.lockCursor = false;
				if (nextState == null)
				{
					parent.parent.flingaBlock.GotoFlingaMode();
				}
			}
		}

		internal class ThrowState : IDisposable, IShsState
		{
			internal FlingaHandMode parent;

			internal PiecewiseLinearFunction upCurve;

			internal PiecewiseLinearFunction gestureCurve;

			internal ThrowState(FlingaHandMode parent)
			{
				this.parent = parent;
			}

			public void Dispose()
			{
				parent = null;
			}

			public void Enter(Type previousState)
			{
				HqObject2 flingaBlock = parent.parent.flingaBlock;
				gestureCurve = new PiecewiseLinearFunction(parent.parent.tweakFlingaHand.gestureTimeToVelocityUp);
				float num = gestureCurve.Eval(parent.gestureDuration) * parent.parent.tweakFlingaHand.maxVelocityUp;
				Vector3 velocity = flingaBlock.rigidbody.velocity;
				velocity.y += num;
				if (velocity.magnitude >= parent.parent.tweakFlingaHand.maxVelocity)
				{
					velocity.Normalize();
					velocity *= parent.parent.tweakFlingaHand.maxVelocity;
				}
				flingaBlock.rigidbody.AddForce(velocity, ForceMode.Impulse);
				flingaBlock.GotoFlingaMode();
			}

			public void Update()
			{
				parent.fsm.ClearState();
			}

			public void Leave(Type nextState)
			{
				parent.parent.GotoFlingaMode();
			}
		}

		protected HqInput parent;

		protected ShsFSM fsm;

		protected KeyBank keyBank;

		protected float[] speeds;

		protected int speedsIdx;

		protected float gestureMouseDuration;

		protected float gestureDuration;

		public SHSInput.InputRequestorType InputRequestorType
		{
			get
			{
				return SHSInput.InputRequestorType.World;
			}
		}

		public bool CanHandleInput
		{
			get
			{
				return true;
			}
		}

		internal FlingaHandMode(HqInput parent)
		{
			this.parent = parent;
			speeds = new float[10];
			fsm = new ShsFSM();
			fsm.AddState(new IntroState(this));
			fsm.AddState(new MoveState(this));
			fsm.AddState(new ThrowState(this));
			fsm.ClearState();
		}

		public void Dispose()
		{
			parent = null;
			fsm.Dispose();
			fsm = null;
		}

		public void Enter(Type previousState)
		{
			keyBank = new KeyBank(this, GUIControl.KeyInputState.Visible, KeyBank.KeyBankTypeEnum.Additive, new Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate>());
			keyBank.AddKey(new KeyCodeEntry(KeyCode.Escape, false, false, false), OnCancelled);
			SHSInput.RegisterListener(keyBank);
			GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.None);
			gestureDuration = 0f;
			gestureMouseDuration = 0f;
			speedsIdx = 0;
			for (int i = 0; i < speeds.Length; i++)
			{
				speeds[i] = 0f;
			}
			fsm.GotoState<IntroState>();
		}

		public void Update()
		{
			parent.UpdateDragScroll();
			fsm.Update();
		}

		public void Leave(Type nextState)
		{
			fsm.ClearState();
			parent.flingaBlock = null;
			SHSInput.UnregisterListener(keyBank);
			GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Normal);
		}

		public Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> GetKeyList(GUIControl.KeyInputState inputState)
		{
			return new Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate>();
		}

		public void ConfigureKeyBanks()
		{
		}

		public bool IsDescendantHandler(IInputHandler handler)
		{
			return false;
		}

		public void OnCancelled(SHSKeyCode code)
		{
			fsm.ClearState();
			parent.GotoFlingaMode();
		}
	}

	internal class SlingshotMode : IDisposable, IShsState
	{
		internal class IntroState : IDisposable, IShsState
		{
			internal SlingshotMode parent;

			internal float time;

			internal Vector3 oldPos;

			internal Quaternion oldRot;

			internal SlingshotTweak tweakSlingshot
			{
				get
				{
					return parent.parent.tweakSlingshot;
				}
			}

			internal IntroState(SlingshotMode parent)
			{
				this.parent = parent;
			}

			public void Dispose()
			{
				parent = null;
			}

			public void Enter(Type previousState)
			{
				GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.None);
				if (tweakSlingshot.animationTime <= 0f)
				{
					HqObject2 flingaBlock = parent.parent.flingaBlock;
					flingaBlock.transform.position = Camera.main.transform.position + Camera.main.transform.forward * tweakSlingshot.offsetFromCamera;
					flingaBlock.transform.rotation = flingaBlock.DefaultRotation;
					parent.fsm.GotoState<TargetState>();
				}
				else
				{
					HqObject2 flingaBlock2 = parent.parent.flingaBlock;
					oldPos = flingaBlock2.transform.position;
					oldRot = flingaBlock2.transform.rotation;
					time = 0f;
				}
			}

			public void Update()
			{
				HqObject2 flingaBlock = parent.parent.flingaBlock;
				time += Time.deltaTime / tweakSlingshot.animationTime;
				if (time < 1f)
				{
					Vector3 to = Camera.main.transform.position + Camera.main.transform.forward * tweakSlingshot.offsetFromCamera;
					Quaternion defaultRotation = flingaBlock.DefaultRotation;
					flingaBlock.fixedUpdate = true;
					flingaBlock.fixedUpdatePos = Vector3.Lerp(oldPos, to, time);
					flingaBlock.fixedUpdateRot = Quaternion.Lerp(oldRot, defaultRotation, time);
				}
				else
				{
					parent.fsm.GotoState<TargetState>();
				}
			}

			public void Leave(Type nextState)
			{
			}
		}

		internal class TargetState : IShsState
		{
			internal SlingshotMode parent;

			internal TargetState(SlingshotMode parent)
			{
				this.parent = parent;
			}

			public void Dispose()
			{
				parent = null;
			}

			public void Enter(Type previousState)
			{
			}

			public void Update()
			{
				Ray ray = Camera.main.ScreenPointToRay(SHSInput.mousePosition);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, 1000f, 98304))
				{
					GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Attack);
					if (SHSInput.GetKey(KeyCode.Mouse0))
					{
						parent.target = hitInfo.point;
						parent.fsm.GotoState<PowerState>();
					}
				}
				else
				{
					GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Uninteractable);
				}
			}

			public void Leave(Type nextState)
			{
			}
		}

		internal class PowerState : IShsState
		{
			internal SlingshotMode parent;

			internal SlingshotTweak tweakSlingshot
			{
				get
				{
					return parent.parent.tweakSlingshot;
				}
			}

			internal PowerState(SlingshotMode parent)
			{
				this.parent = parent;
			}

			public void Dispose()
			{
				parent = null;
			}

			public void Enter(Type previousState)
			{
				parent.power = tweakSlingshot.powerMin;
			}

			public void Update()
			{
				if (SHSInput.GetKey(KeyCode.Mouse0))
				{
					parent.power += Time.deltaTime * tweakSlingshot.powerRate;
					if (parent.power >= tweakSlingshot.powerMax)
					{
						parent.fsm.GotoState<ThrowState>();
					}
				}
				else
				{
					parent.fsm.GotoState<ThrowState>();
				}
			}

			public void Leave(Type nextState)
			{
			}
		}

		internal class ThrowState : IShsState
		{
			internal SlingshotMode parent;

			internal Vector3 velocity;

			internal float time;

			internal SlingshotTweak tweakSlingshot
			{
				get
				{
					return parent.parent.tweakSlingshot;
				}
			}

			internal ThrowState(SlingshotMode parent)
			{
				this.parent = parent;
			}

			public void Dispose()
			{
				parent = null;
			}

			public void Enter(Type previousState)
			{
				HqObject2 flingaBlock = parent.parent.flingaBlock;
				Vector3 vector = parent.target - flingaBlock.transform.position;
				float num = Mathf.Clamp(parent.power, tweakSlingshot.powerMin, tweakSlingshot.powerMax) * tweakSlingshot.velocityMax;
				velocity = vector.normalized * num;
				float num2 = 10f;
				RaycastHit hitInfo;
				if (Physics.Linecast(parent.target, flingaBlock.transform.position, out hitInfo, 393216) && hitInfo.distance < num2)
				{
					num2 = hitInfo.distance;
				}
				time = (vector.magnitude - num2) / num;
				CspUtils.DebugLog("shot distance = " + vector.magnitude);
			}

			public void Update()
			{
				HqObject2 flingaBlock = parent.parent.flingaBlock;
				time -= Time.deltaTime;
				if (time >= 0f)
				{
					Vector3 b = velocity * Time.deltaTime;
					flingaBlock.fixedUpdate = true;
					flingaBlock.fixedUpdatePos = flingaBlock.transform.position + b;
					flingaBlock.fixedUpdateRot = flingaBlock.DefaultRotation;
				}
				else
				{
					flingaBlock.GotoFlingaMode();
					parent.fsm.ClearState();
				}
			}

			public void Leave(Type nextState)
			{
				parent.parent.GotoFlingaMode();
			}
		}

		protected HqInput parent;

		protected ShsFSM fsm;

		protected Vector3 target = Vector3.zero;

		protected float power;

		internal SlingshotMode(HqInput parent)
		{
			this.parent = parent;
			fsm = new ShsFSM();
			fsm.AddState(new IntroState(this));
			fsm.AddState(new TargetState(this));
			fsm.AddState(new PowerState(this));
			fsm.AddState(new ThrowState(this));
			fsm.ClearState();
		}

		public void Dispose()
		{
			parent = null;
			fsm.Dispose();
			fsm = null;
		}

		public void Enter(Type previousState)
		{
			parent.flingaBlock.GotoFlingaSelectedMode();
			fsm.GotoState<IntroState>();
		}

		public void Update()
		{
			fsm.Update();
		}

		public void Leave(Type nextState)
		{
			parent.flingaBlock = null;
		}
	}

	internal abstract class TutorialSectionBase : IDisposable
	{
		protected HqInputTutorial parent;

		protected bool canProceed;

		protected HqControllerTutorial Controller
		{
			get
			{
				return HqController2.Instance as HqControllerTutorial;
			}
		}

		protected void UpdateMovement()
		{
			parent.UpdateScroll();
			if (SHSInput.GetKeyDown(KeyCode.Equals) || SHSInput.GetKeyDown(KeyCode.KeypadPlus))
			{
				HqController2.Instance.ActiveRoom.ZoomUp();
			}
			if (SHSInput.GetKeyDown(KeyCode.Minus) || SHSInput.GetKeyDown(KeyCode.KeypadMinus))
			{
				HqController2.Instance.ActiveRoom.ZoomDown();
			}
		}

		public void Dispose()
		{
			parent = null;
		}
	}

	internal class TutorialCameraMovementSection : TutorialSectionBase, IShsState
	{
		protected bool userScrolled;

		protected bool userZoomed;

		internal TutorialCameraMovementSection(HqInputTutorial parent)
		{
			base.parent = parent;
		}

		public void Enter(Type previousState)
		{
			CspUtils.DebugLog("Entering Camera Movement section.");
		}

		public void Update()
		{
			UpdateMovement();
			canProceed = (HqController2.Instance.ActiveRoom.HasScrolled && HqController2.Instance.ActiveRoom.HasZoomed);
			if (canProceed && SHSInput.GetKeyDown(KeyCode.RightArrow))
			{
				parent.GotoPausePlaySection();
			}
		}

		public void Leave(Type nextState)
		{
		}
	}

	internal class TutorialPausePlaySection : TutorialSectionBase, IShsState
	{
		protected bool paused;

		internal TutorialPausePlaySection(HqInputTutorial parent)
		{
			base.parent = parent;
		}

		public void Enter(Type previousState)
		{
			CspUtils.DebugLog("Entering pause play section.");
		}

		public void Update()
		{
			UpdateMovement();
			if (SHSInput.GetKeyDown(KeyCode.Pause))
			{
				if (!paused)
				{
					base.Controller.GoToPauseMode();
					paused = true;
				}
				else
				{
					base.Controller.GoToPlayMode();
					canProceed = true;
					paused = false;
				}
			}
			if (SHSInput.GetKeyDown(KeyCode.RightArrow) && canProceed)
			{
				parent.GotoChangeRoomSection();
			}
			else if (SHSInput.GetKeyDown(KeyCode.LeftArrow))
			{
				parent.GotoCameraMovementSection();
			}
		}

		public void Leave(Type nextState)
		{
			parent.allowPausing = true;
		}
	}

	internal class TutorialChangeRoomsSection : TutorialSectionBase, IShsState
	{
		internal TutorialChangeRoomsSection(HqInputTutorial parent)
		{
			base.parent = parent;
		}

		public void Enter(Type previousState)
		{
			CspUtils.DebugLog("Entering change room section");
		}

		public void Update()
		{
			UpdateMovement();
			if (SHSInput.GetKeyDown(KeyCode.N))
			{
				base.Controller.GoToNextRoom();
				canProceed = true;
			}
			if (canProceed && SHSInput.GetKeyDown(KeyCode.RightArrow))
			{
				parent.GotoFollowHeroSection();
			}
			else if (SHSInput.GetKeyDown(KeyCode.LeftArrow))
			{
				parent.GotoPausePlaySection();
			}
		}

		public void Leave(Type nextState)
		{
		}
	}

	internal class TutorialFollowHeroSection : TutorialSectionBase, IShsState
	{
		protected bool isFollowing;

		protected AIControllerHQ ai;

		internal TutorialFollowHeroSection(HqInputTutorial parent)
		{
			base.parent = parent;
		}

		public void Enter(Type previousState)
		{
			CspUtils.DebugLog("Entering follow hero section");
			ai = base.Controller.GetAIController("falcon");
		}

		public void Update()
		{
			UpdateMovement();
			if (SHSInput.GetKeyDown(KeyCode.F))
			{
				base.Controller.Dorm1.RoomState = HqRoom2.AccessState.Unlocked;
				if (ai.CurrentRoom != base.Controller.ActiveRoom)
				{
					base.Controller.SetActiveRoom(ai.CurrentRoom);
				}
				parent.SetHeroCamera(ai, -1f);
				isFollowing = true;
			}
			if (isFollowing && !parent.IsHeroCamTarget(ai))
			{
				canProceed = true;
			}
			if (canProceed && SHSInput.GetKeyDown(KeyCode.RightArrow))
			{
				parent.GotoPaintRoomSection();
			}
		}

		public void Leave(Type nextState)
		{
		}
	}

	internal class TutorialPaintRoomSection : TutorialSectionBase, IShsState
	{
		internal TutorialPaintRoomSection(HqInputTutorial parent)
		{
			base.parent = parent;
		}

		public void Enter(Type previousState)
		{
			CspUtils.DebugLog("Entering paint room section");
		}

		public void Update()
		{
			UpdateMovement();
			if (SHSInput.GetKeyDown(KeyCode.P))
			{
				CspUtils.DebugLog("Applying paint!");
				base.Controller.ActiveRoom.ApplyTheme("Moss Green Paint", false);
				canProceed = true;
			}
			if (canProceed && SHSInput.GetKeyDown(KeyCode.RightArrow))
			{
				parent.GotoPlaceItemsSection();
			}
		}

		public void Leave(Type nextState)
		{
		}
	}

	internal class TutorialPlaceItemsSection : TutorialSectionBase, IShsState
	{
		private GameObject itemInstance;

		internal TutorialPlaceItemsSection(HqInputTutorial parent)
		{
			base.parent = parent;
		}

		public void Enter(Type previousState)
		{
			CspUtils.DebugLog("Entering place items section");
		}

		public void Update()
		{
			UpdateMovement();
			if (itemInstance != null)
			{
				HqObject2 component = Utils.GetComponent<HqObject2>(itemInstance);
				bool flag = true;
				bool flag2 = false;
				if (SHSInput.IsOverUI())
				{
					Utils.ForEachTree(itemInstance, delegate(GameObject go)
					{
						if (go != null && go.renderer != null)
						{
							if (go.particleEmitter != null)
							{
								go.particleEmitter.enabled = false;
							}
							go.renderer.enabled = false;
						}
					});
					flag2 = true;
					flag = false;
				}
				else
				{
					Utils.ForEachTree(itemInstance, delegate(GameObject go)
					{
						if (go != null && go.renderer != null)
						{
							go.renderer.enabled = true;
						}
					});
					if (component != null)
					{
						Ray ray = Camera.main.ScreenPointToRay(SHSInput.mousePosition);
						int layerMask = 1147392;
						if (!parent.ignoreBlocks)
						{
							layerMask = 1155584;
						}
						flag = true;
						RaycastHit hitInfo;
						if (Physics.Raycast(ray, out hitInfo, 1000f, layerMask))
						{
							if (parent.ignoreSides)
							{
								float num = Vector3.Angle(Vector3.up, hitInfo.normal);
								if (num >= 70f)
								{
									if (Physics.Raycast(ray, out hitInfo, 1000f, 98304))
									{
										flag = !component.PlaceAt(hitInfo.point);
									}
								}
								else
								{
									flag = !component.PlaceAt(hitInfo.point);
								}
							}
							else
							{
								flag = !component.PlaceAt(hitInfo.point);
							}
						}
						if (SHSInput.GetKey(KeyCode.RightArrow))
						{
							itemInstance.transform.RotateAround(itemInstance.transform.forward, parent.rotateSpeed * Time.deltaTime);
						}
						if (SHSInput.GetKey(KeyCode.LeftArrow))
						{
							itemInstance.transform.RotateAround(itemInstance.transform.forward, (0f - parent.rotateSpeed) * Time.deltaTime);
						}
						if (SHSInput.GetKey(KeyCode.UpArrow))
						{
							itemInstance.transform.RotateAround(itemInstance.transform.right, parent.rotateSpeed * Time.deltaTime);
						}
						if (SHSInput.GetKey(KeyCode.DownArrow))
						{
							itemInstance.transform.RotateAround(itemInstance.transform.right, (0f - parent.rotateSpeed) * Time.deltaTime);
						}
						if (SHSInput.GetKey(KeyCode.Home))
						{
							HqItem item = base.Controller.ActiveRoom.GetItem(component.PlacedId);
							if (item != null)
							{
								item.ResetRotation();
							}
						}
					}
				}
				if (!flag)
				{
					SelectedObjectController component2 = Utils.GetComponent<SelectedObjectController>(Camera.main);
					if (component2 != null)
					{
						component2.pulseColor = Color.white;
					}
					if (!flag2 && SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Left))
					{
						component.GotoPlacementMode(true);
						base.Controller.ActiveRoom.MoveItem(itemInstance);
						itemInstance = null;
						canProceed = true;
					}
				}
				else if (!flag2)
				{
					GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Uninteractable);
					SelectedObjectController component3 = Utils.GetComponent<SelectedObjectController>(Camera.main);
					if (component3 != null)
					{
						component3.pulseColor = Color.red;
					}
				}
			}
			else if (SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Left) && parent.Paused)
			{
				itemInstance = parent.PickObject();
				if (itemInstance != null)
				{
					HqObject2 component4 = Utils.GetComponent<HqObject2>(itemInstance);
					if (component4 != null)
					{
						component4.GotoPlacementSelectedMode();
					}
				}
			}
			if (SHSInput.GetKeyDown(KeyCode.P))
			{
				parent.Paused = true;
				Item value = null;
				GameObject original = null;
				if (base.Controller.Profile.AvailableItems.TryGetValue("31", out value) && value.Definition.PlacedObjectAssetBundle != null)
				{
					AssetBundle assetBundle = base.Controller.GetAssetBundle(value.Definition.PlacedObjectAssetBundle);
					if (assetBundle != null)
					{
						original = (assetBundle.Load(value.Definition.PlacedObjectPrefab) as GameObject);
					}
				}
				itemInstance = (UnityEngine.Object.Instantiate(original) as GameObject);
				HqObject2 component5 = Utils.GetComponent<HqObject2>(itemInstance);
				if (component5 != null)
				{
					component5.GotoPlacementSelectedMode();
					component5.InventoryId = "31";
					base.Controller.ActiveRoom.AddItem(itemInstance);
				}
			}
			if (canProceed && SHSInput.GetKeyDown(KeyCode.RightArrow))
			{
				parent.GotoPlaceHeroSection();
			}
		}

		public void Leave(Type nextState)
		{
		}
	}

	internal class TutorialPlaceHeroSection : TutorialSectionBase, IShsState
	{
		private GameObject itemInstance;

		internal TutorialPlaceHeroSection(HqInputTutorial parent)
		{
			base.parent = parent;
		}

		public void Enter(Type previousState)
		{
			CspUtils.DebugLog("Entering place hero section");
		}

		public void Update()
		{
			UpdateMovement();
			if (itemInstance != null)
			{
				HqObject2 component = Utils.GetComponent<HqObject2>(itemInstance);
				bool flag = true;
				bool flag2 = false;
				if (SHSInput.IsOverUI())
				{
					Utils.ForEachTree(itemInstance, delegate(GameObject go)
					{
						if (go != null && go.renderer != null)
						{
							if (go.particleEmitter != null)
							{
								go.particleEmitter.enabled = false;
							}
							go.renderer.enabled = false;
						}
					});
					flag2 = true;
					flag = false;
				}
				else
				{
					Utils.ForEachTree(itemInstance, delegate(GameObject go)
					{
						if (go != null && go.renderer != null)
						{
							go.renderer.enabled = true;
						}
					});
					if (component != null)
					{
						Ray ray = Camera.main.ScreenPointToRay(SHSInput.mousePosition);
						int layerMask = 1147392;
						if (!parent.ignoreBlocks)
						{
							layerMask = 1155584;
						}
						flag = true;
						RaycastHit hitInfo;
						if (Physics.Raycast(ray, out hitInfo, 1000f, layerMask))
						{
							if (parent.ignoreSides)
							{
								float num = Vector3.Angle(Vector3.up, hitInfo.normal);
								if (num >= 70f)
								{
									if (Physics.Raycast(ray, out hitInfo, 1000f, 98304))
									{
										flag = !component.PlaceAt(hitInfo.point);
									}
								}
								else
								{
									flag = !component.PlaceAt(hitInfo.point);
								}
							}
							else
							{
								flag = !component.PlaceAt(hitInfo.point);
							}
						}
						if (SHSInput.GetKey(KeyCode.RightArrow))
						{
							parent.block.transform.RotateAround(parent.block.transform.up, parent.rotateSpeed * Time.deltaTime);
						}
						if (SHSInput.GetKey(KeyCode.LeftArrow))
						{
							parent.block.transform.RotateAround(parent.block.transform.up, (0f - parent.rotateSpeed) * Time.deltaTime);
						}
						if (SHSInput.GetKey(KeyCode.Home))
						{
							parent.block.transform.rotation = Quaternion.identity;
						}
					}
				}
				if (!flag)
				{
					SelectedObjectController component2 = Utils.GetComponent<SelectedObjectController>(Camera.main);
					if (component2 != null)
					{
						component2.pulseColor = Color.white;
					}
					if (!flag2 && SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Left))
					{
						component.GotoPlacementMode(true);
						base.Controller.ActiveRoom.MoveItem(itemInstance);
						HqAIProxy component3 = Utils.GetComponent<HqAIProxy>(itemInstance);
						if (component3 != null)
						{
							component3.Spawn("cyclops", itemInstance.transform.position, base.Controller.ActiveRoom);
							base.Controller.AddProxy(component3);
						}
						itemInstance = null;
						canProceed = true;
					}
				}
				else if (!flag2)
				{
					GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Uninteractable);
					SelectedObjectController component4 = Utils.GetComponent<SelectedObjectController>(Camera.main);
					if (component4 != null)
					{
						component4.pulseColor = Color.red;
					}
				}
			}
			else if (SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Left) && parent.Paused)
			{
				itemInstance = parent.PickObject();
				if (itemInstance != null)
				{
					HqObject2 component5 = Utils.GetComponent<HqObject2>(itemInstance);
					if (component5 != null)
					{
						component5.GotoPlacementSelectedMode();
					}
				}
			}
			if (SHSInput.GetKeyDown(KeyCode.P))
			{
				parent.Paused = true;
				itemInstance = HqAIProxy.CreateProxy("cyclops");
				if (itemInstance != null && itemInstance.animation != null)
				{
					itemInstance.animation["movement_idle"].time = 1f;
					itemInstance.animation["movement_idle"].enabled = true;
					itemInstance.animation.Sample();
					itemInstance.animation["movement_idle"].enabled = false;
				}
				HqObject2 component6 = Utils.GetComponent<HqObject2>(itemInstance);
				if (component6 != null)
				{
					component6.GotoPlacementSelectedMode();
					base.Controller.ActiveRoom.AddItem(itemInstance);
				}
			}
			if (canProceed && SHSInput.GetKeyDown(KeyCode.RightArrow))
			{
				parent.GotoCommandSection();
			}
		}

		public void Leave(Type nextState)
		{
		}
	}

	internal class TutorialCommandSection : TutorialSectionBase, IShsState
	{
		protected AIControllerHQ selectedAI;

		protected GameObject selectedObject;

		protected bool waitingForAIToUseObject;

		internal TutorialCommandSection(HqInputTutorial parent)
		{
			base.parent = parent;
		}

		public void Enter(Type previousState)
		{
		}

		public void Update()
		{
			UpdateMovement();
			if (waitingForAIToUseObject && selectedAI != null && selectedObject != null && selectedAI.IsUsingItem(selectedObject))
			{
				canProceed = true;
				foreach (AIControllerHQTutorial item in base.Controller.ActiveRoom.AIInRoom)
				{
					item.CanUseItems = true;
				}
			}
			if (parent.Paused && SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Right))
			{
				GameObject gameObject = parent.PickObject();
				if (gameObject != null)
				{
					HqObject2 component = Utils.GetComponent<HqObject2>(gameObject);
					if (component != null)
					{
						HqItem component2 = Utils.GetComponent<HqItem>(component.gameObject);
						if (selectedAI != null && component2 != null)
						{
							selectedObject = gameObject;
							(selectedAI as AIControllerHQTutorial).CanUseItems = true;
							selectedAI.CurrentActivityItem = component2;
						}
					}
					else
					{
						AIControllerHQ component3 = Utils.GetComponent<AIControllerHQ>(gameObject);
						if (component3 != null)
						{
							if (selectedAI != null && selectedAI != component3)
							{
								selectedAI.Selected = false;
							}
							component3.Selected = true;
							selectedAI = component3;
						}
					}
				}
			}
			if (selectedAI != null && selectedAI.CurrentActivityItem != null && SHSInput.GetKeyDown(KeyCode.Pause))
			{
				selectedAI.Paused = false;
				selectedAI.Selected = false;
				(HqController2.Instance as HqControllerTutorial).GoToPlayMode();
				waitingForAIToUseObject = true;
			}
		}

		public void Leave(Type nextState)
		{
		}
	}

	protected const string STR_REMOVE_SAVED_ITEM_PROMPT = "#hq_remove_saved_item_prompt";

	public bool ignoreBlocks = true;

	public bool ignoreSides;

	public bool snapToCenter = true;

	public float rotateSpeed = 5f;

	public FlingaInputMode inputMode;

	public ScrollTweak tweakScroll = new ScrollTweak();

	public FlingaHandTweak tweakFlingaHand = new FlingaHandTweak();

	public SlingshotTweak tweakSlingshot = new SlingshotTweak();

	public ShsFSM fsm;

	protected Type onDragBeginState;

	protected bool introCameraActive;

	protected bool heroCameraActive;

	protected string heroCameraName;

	protected AIControllerHQ heroCameraTarget;

	protected bool heroCameraIsTrackingFlinga;

	protected KeyBank keyBank;

	protected float scrollTimer;

	protected float scrollSpeed;

	protected GameObject block;

	protected bool blockOld;

	protected Vector3 blockOldPos;

	protected Quaternion blockOldRot;

	protected HqObject2 flingaBlock;

	private DragDropInfo currentDragDropInfo;

	private bool displayingDialog;

	protected bool dragLastFrame;

	public DragDropInfo CurrentDragDropInfo
	{
		get
		{
			return currentDragDropInfo;
		}
	}

	public SHSInput.InputRequestorType InputRequestorType
	{
		get
		{
			return SHSInput.InputRequestorType.World;
		}
	}

	public bool CanHandleInput
	{
		get
		{
			return true;
		}
	}

	public Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> GetKeyList(GUIControl.KeyInputState inputState)
	{
		return new Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate>();
	}

	public void ConfigureKeyBanks()
	{
	}

	public bool IsDescendantHandler(IInputHandler handler)
	{
		return false;
	}

	public virtual void Start()
	{
		fsm = new ShsFSM();
		fsm.AddState(new ViewMode(this));
		fsm.AddState(new PlacementMode(this));
		fsm.AddState(new FlingaMode(this));
		fsm.AddState(new FlingaHandMode(this));
		fsm.AddState(new SlingshotMode(this));
		fsm.GotoState<ViewMode>();
		heroCameraActive = false;
		heroCameraTarget = null;
		heroCameraName = null;
		AppShell.Instance.EventMgr.AddListener<GUIDragBeginMessage>(onDragBegin);
		AppShell.Instance.EventMgr.AddListener<HQRoomZoomRequestMessage>(onZoomRequest);
	}

	public void DeactiveRoom()
	{
		ClearHeroCamera(1f);
	}

	public void ActivateRoom()
	{
		CameraLiteManager.Instance.ReplaceCamera(HqController2.Instance.ActiveRoom.CameraRoom, 0f);
		if (heroCameraName != null)
		{
			GameObject gameObject = GameObject.Find(heroCameraName);
			if (gameObject != null)
			{
				SetHeroCamera(gameObject.GetComponent<AIControllerHQ>(), 0f);
			}
			heroCameraName = null;
		}
	}

	public void SetHeroCamera(string heroName)
	{
		heroCameraName = heroName;
	}

	public bool IsHeroCamTarget(AIControllerHQ ai)
	{
		return ai == heroCameraTarget;
	}

	public void SetHeroCamera(AIControllerHQ hero, float blend)
	{
		introCameraActive = false;
		CameraLite cameraHero = HqController2.Instance.ActiveRoom.CameraHero;
		if (heroCameraTarget != null)
		{
			AppShell.Instance.EventMgr.RemoveListener<HQAIRoomChanged>(OnHeroChangedRooms);
		}
		heroCameraTarget = hero;
		if (hero.IsFlingaObjectActive)
		{
			heroCameraIsTrackingFlinga = true;
			cameraHero.GetComponent<CameraTarget>().Target = heroCameraTarget.FlingaObject.transform;
		}
		else
		{
			heroCameraIsTrackingFlinga = false;
			cameraHero.GetComponent<CameraTarget>().Target = heroCameraTarget.transform;
		}
		cameraHero.Reset();
		AppShell.Instance.EventMgr.AddListener<HQAIRoomChanged>(OnHeroChangedRooms);
		if (!heroCameraActive)
		{
			CameraLiteManager.Instance.PushCamera(cameraHero, blend);
			heroCameraActive = true;
		}
	}

	public void ClearHeroCamera(float blendTime)
	{
		if (heroCameraActive)
		{
			AppShell.Instance.EventMgr.RemoveListener<HQAIRoomChanged>(OnHeroChangedRooms);
			CameraLiteManager.Instance.PopCamera(blendTime);
		}
		introCameraActive = false;
		heroCameraActive = false;
		heroCameraTarget = null;
	}

	public void SpecialIntroSetHeroCamera(AIControllerHQ hero)
	{
		SetHeroCamera(hero, -1f);
		introCameraActive = true;
	}

	public void SpecialIntroClearHeroCamera(float blendTime)
	{
		if (introCameraActive)
		{
			ClearHeroCamera(blendTime);
			introCameraActive = false;
		}
	}

	private void OnHeroChangedRooms(HQAIRoomChanged msg)
	{
		if (msg.ai == heroCameraTarget.gameObject)
		{
			ClearHeroCamera(1f);
			AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(msg.ai);
			if (component != null)
			{
				component.Paused = true;
			}
			HqController2.Instance.SetActiveRoom(msg.room);
			msg.room.PlayTransporterEffect(component.gameObject.transform.position);
			if (component != null)
			{
				component.Paused = false;
			}
			SetHeroCamera(component, -1f);
		}
	}

	protected void onZoomRequest(HQRoomZoomRequestMessage message)
	{
		if (message.up)
		{
			HqController2.Instance.ActiveRoom.ZoomUp();
		}
		else
		{
			HqController2.Instance.ActiveRoom.ZoomDown();
		}
	}

	private void onDragBegin(GUIDragBeginMessage message)
	{
		currentDragDropInfo = message.DragDropInfo;
		if (currentDragDropInfo.WorldSource == null)
		{
			HqController2.Instance.GotoViewMode();
			if (snapToCenter)
			{
				Screen.lockCursor = true;
				Screen.lockCursor = false;
			}
			GameObject original = HqController2.Instance.GetTempObjectPrefab(0);
			UserProfile profile = HqController2.Instance.Profile;
			if (profile != null)
			{
				if (currentDragDropInfo.CollectionId == DragDropInfo.CollectionType.Items)
				{
					Item value = null;
					if (profile.AvailableItems.TryGetValue(currentDragDropInfo.ItemId, out value) && value.Definition.PlacedObjectAssetBundle != null)
					{
						AssetBundle assetBundle = HqController2.Instance.GetAssetBundle(value.Definition.PlacedObjectAssetBundle);
						if (assetBundle != null)
						{
							original = (assetBundle.Load(value.Definition.PlacedObjectPrefab) as GameObject);
						}
					}
					block = (UnityEngine.Object.Instantiate(original) as GameObject);
				}
				else if (currentDragDropInfo.CollectionId == DragDropInfo.CollectionType.Heroes)
				{
					GameObject aI = HqController2.Instance.GetAI(currentDragDropInfo.ItemId);
					if (aI != null)
					{
						block = aI;
						AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(aI);
						if (component != null)
						{
							if (component.IsInActiveRoom)
							{
								component.Interrupt();
							}
							else
							{
								component.CurrentRoom = HqController2.Instance.ActiveRoom;
							}
						}
						if (!block.active)
						{
							Utils.ActivateTree(block, true);
						}
					}
					else
					{
						block = HqAIProxy.CreateProxy(currentDragDropInfo.ItemId);
						if (block != null && block.animation != null)
						{
							block.animation["movement_idle"].time = 1f;
							block.animation["movement_idle"].enabled = true;
							block.animation.Sample();
							block.animation["movement_idle"].enabled = false;
						}
					}
				}
			}
		}
		else
		{
			block = currentDragDropInfo.WorldSource;
		}
		onDragBeginState = fsm.GetCurrentState();
		if (block != null)
		{
			HqObject2 component2 = Utils.GetComponent<HqObject2>(block);
			if (component2 != null)
			{
				if (currentDragDropInfo != null && currentDragDropInfo.ItemId != null)
				{
					component2.InventoryId = currentDragDropInfo.ItemId;
					component2.IconSource = currentDragDropInfo.IconSource;
				}
				component2.GotoPlacementSelectedMode();
				if (component2.PlacedId == -1 && HqController2.Instance.ActiveRoom.AddItem(block) == null)
				{
					if (block != null)
					{
						UnityEngine.Object.Destroy(block);
					}
					string[] keys = new string[1]
					{
						currentDragDropInfo.ItemId
					};
					string collectionId = (currentDragDropInfo.CollectionId != 0) ? "Heroes" : "Items";
					AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(keys, CollectionResetMessage.ActionType.Add, collectionId));
					currentDragDropInfo.Result = DragDropResult.Cancelled;
					return;
				}
			}
		}
		HqController2.Instance.GotoPlacementMode();
		AppShell.Instance.EventMgr.AddListener<GUIDragEndMessage>(onDragEnd);
	}

	private void onDragEnd(GUIDragEndMessage message)
	{
		if (message.DragDropInfo.Result == DragDropResult.WorldDropped)
		{
			if (message.DragDropInfo.CollectionId == DragDropInfo.CollectionType.Items)
			{
				Utils.GetComponent<HqObject2>(block).GotoPlacementMode(true);
				HqController2.Instance.ActiveRoom.MoveItem(block);
				block = null;
			}
			else if (message.DragDropInfo.CollectionId == DragDropInfo.CollectionType.Heroes)
			{
				GameObject aI = HqController2.Instance.GetAI(message.DragDropInfo.ItemId);
				if (aI != null)
				{
					HqController2.Instance.ReDropAI(message.DragDropInfo.ItemId, block.transform.position, HqController2.Instance.ActiveRoom);
				}
				else
				{
					HqController2.Instance.ActiveRoom.MoveItem(block);
					HqController2.Instance.CheckRoomAssignment(message.DragDropInfo.ItemId, HqController2.Instance.ActiveRoom);
					HqAIProxy component = Utils.GetComponent<HqAIProxy>(block);
					if (component != null)
					{
						component.Spawn(message.DragDropInfo.ItemId, block.transform.position, HqController2.Instance.ActiveRoom);
						HqController2.Instance.AddProxy(component);
					}
				}
				block = null;
			}
		}
		else if (message.DragDropInfo.Result == DragDropResult.UIDropped || message.DragDropInfo.Result == DragDropResult.Pending)
		{
			if (block != null)
			{
				HqItem component2 = Utils.GetComponent<HqItem>(block);
				if (component2 != null && component2.IsSaved)
				{
					RemoveSavedItem(message);
					return;
				}
				if (Utils.GetComponent<AIControllerHQ>(block) != null)
				{
					AIControllerHQ component3 = Utils.GetComponent<AIControllerHQ>(block);
					component3.Despawn();
				}
				if (Utils.GetComponent<HqObject2>(block) != null)
				{
					HqController2.Instance.ActiveRoom.DelItem(block);
					UnityEngine.Object.Destroy(block);
					block = null;
				}
				string[] keys = new string[1]
				{
					message.DragDropInfo.ItemId
				};
				string collectionId = (message.DragDropInfo.CollectionId != 0) ? "Heroes" : "Items";
				AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(keys, CollectionResetMessage.ActionType.Add, collectionId));
			}
		}
		else if (message.DragDropInfo.Result == DragDropResult.Cancelled)
		{
			if (message.DragDropInfo.SourceType == DragDropSourceType.World)
			{
				if (blockOld)
				{
					if (message.DragDropInfo.CollectionId == DragDropInfo.CollectionType.Heroes)
					{
						HqController2.Instance.ReDropAI(message.DragDropInfo.ItemId, blockOldPos, HqController2.Instance.ActiveRoom);
					}
					else if (block != null && Utils.GetComponent<HqObject2>(block) != null)
					{
						block.transform.position = blockOldPos;
						block.transform.rotation = blockOldRot;
						Utils.GetComponent<HqObject2>(block).GotoPlacementMode(true);
					}
				}
			}
			else
			{
				if (block != null)
				{
					if (Utils.GetComponent<AIControllerHQ>(block) != null)
					{
						AIControllerHQ component4 = Utils.GetComponent<AIControllerHQ>(block);
						component4.Despawn();
					}
					HqController2.Instance.ActiveRoom.DelItem(block);
					UnityEngine.Object.Destroy(block);
				}
				string[] keys2 = new string[1]
				{
					message.DragDropInfo.ItemId
				};
				string collectionId2 = (message.DragDropInfo.CollectionId != 0) ? "Heroes" : "Items";
				AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(keys2, CollectionResetMessage.ActionType.Add, collectionId2));
			}
			block = null;
		}
		currentDragDropInfo = null;
		onDragBeginState = null;
		AppShell.Instance.EventMgr.RemoveListener<GUIDragEndMessage>(onDragEnd);
		GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Normal);
		HqController2.Instance.GotoViewMode();
	}

	private void RemoveSavedItem(GUIDragEndMessage message)
	{
		displayingDialog = true;
		GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, "#hq_remove_saved_item_prompt", delegate(string Id, GUIDialogWindow.DialogState state)
		{
			if (state == GUIDialogWindow.DialogState.Cancel)
			{
				if (blockOld && Utils.GetComponent<HqObject2>(block) != null)
				{
					block.transform.position = blockOldPos;
					block.transform.rotation = blockOldRot;
					Utils.ForEachTree(block, delegate(GameObject go)
					{
						if (go.renderer != null)
						{
							go.renderer.enabled = true;
						}
					});
					Utils.GetComponent<HqObject2>(block).GotoPlacementMode(true);
				}
			}
			else
			{
				if (Utils.GetComponent<HqObject2>(block) != null)
				{
					HqController2.Instance.ActiveRoom.DelItem(block);
					UnityEngine.Object.Destroy(block);
					block = null;
				}
				else if (Utils.GetComponent<AIControllerHQ>(block) != null)
				{
					AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(block);
					component.Despawn();
				}
				string[] keys = new string[1]
				{
					message.DragDropInfo.ItemId
				};
				string collectionId = (message.DragDropInfo.CollectionId != 0) ? "Heroes" : "Items";
				AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(keys, CollectionResetMessage.ActionType.Add, collectionId));
			}
			block = null;
			currentDragDropInfo = null;
			onDragBeginState = null;
			AppShell.Instance.EventMgr.RemoveListener<GUIDragEndMessage>(onDragEnd);
			GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Normal);
			displayingDialog = false;
			HqController2.Instance.GotoViewMode();
		}, GUIControl.ModalLevelEnum.Default);
	}

	public void OnLeavingHQ()
	{
		if (!(block != null) || currentDragDropInfo == null)
		{
			return;
		}
		if (blockOld)
		{
			if (currentDragDropInfo.CollectionId == DragDropInfo.CollectionType.Items)
			{
				block.transform.position = blockOldPos;
				block.transform.rotation = blockOldRot;
			}
			else if (currentDragDropInfo.CollectionId == DragDropInfo.CollectionType.Heroes)
			{
				AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(new string[1]
				{
					currentDragDropInfo.ItemId
				}, CollectionResetMessage.ActionType.Add, "Heroes"));
			}
		}
		else
		{
			HqObject2 component = Utils.GetComponent<HqObject2>(block);
			if (component != null)
			{
				string inventoryId = component.InventoryId;
				HqController2.Instance.ActiveRoom.DelItem(block);
				UnityEngine.Object.Destroy(block);
				if (currentDragDropInfo.CollectionId == DragDropInfo.CollectionType.Heroes)
				{
					AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(new string[1]
					{
						currentDragDropInfo.ItemId
					}, CollectionResetMessage.ActionType.Add, "Heroes"));
				}
				else if (currentDragDropInfo.CollectionId == DragDropInfo.CollectionType.Items)
				{
					AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(new string[1]
					{
						inventoryId
					}, CollectionResetMessage.ActionType.Add, "Items"));
				}
			}
		}
		block = null;
		fsm.ClearState();
	}

	public virtual void OnDisable()
	{
		if (fsm != null)
		{
			fsm.Dispose();
			fsm = null;
		}
		ClearHeroCamera(1f);
		SHSInput.UnregisterListener(keyBank);
		AppShell.Instance.EventMgr.RemoveListener<GUIDragBeginMessage>(onDragBegin);
		AppShell.Instance.EventMgr.RemoveListener<GUIDragEndMessage>(onDragEnd);
		AppShell.Instance.EventMgr.RemoveListener<HQRoomZoomRequestMessage>(onZoomRequest);
	}

	public virtual void Update()
	{
		if (HqController2.Instance.ActiveRoom == null)
		{
			return;
		}
		if (heroCameraActive)
		{
			if (!SHSInput.IsOverUI() && (SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Left) || SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Right)))
			{
				ClearHeroCamera(1f);
			}
			if (heroCameraTarget != null && heroCameraTarget.IsFlingaObjectActive != heroCameraIsTrackingFlinga)
			{
				SetHeroCamera(heroCameraTarget, -1f);
			}
		}
		if (fsm != null)
		{
			fsm.Update();
		}
	}

	public void GotoViewMode()
	{
		fsm.GotoState<ViewMode>();
	}

	public void GotoPlacementMode()
	{
		fsm.GotoState<PlacementMode>();
	}

	public void GotoFlingaMode()
	{
		fsm.GotoState<FlingaMode>();
	}

	protected void UpdateScroll()
	{
		float num = scrollTimer;
		float num2 = scrollSpeed;
		scrollTimer = 0f;
		scrollSpeed = 0f;
		if (SHSInput.GetKeyDown(KeyCode.Equals) || SHSInput.GetKeyDown(KeyCode.KeypadPlus))
		{
			HqController2.Instance.ActiveRoom.ZoomUp();
		}
		if (SHSInput.GetKeyDown(KeyCode.Minus) || SHSInput.GetKeyDown(KeyCode.KeypadMinus))
		{
			HqController2.Instance.ActiveRoom.ZoomDown();
		}
		UpdateDragScroll();
		if (tweakScroll.scrollDrag || SHSInput.IsOverUI())
		{
			return;
		}
		Vector2 vector = SHSInput.mousePosition;
		if (vector.x < 0f || vector.y < 0f || vector.x > Camera.main.pixelWidth || vector.y > Camera.main.pixelHeight)
		{
			return;
		}
		ScrollRegion scrollRegion = ScrollRegion.None;
		float a = 0f;
		float num3 = Camera.main.pixelWidth * tweakScroll.scrollArea;
		float num4 = Camera.main.pixelWidth - num3;
		if (vector.x >= 0f && vector.x <= num3)
		{
			a = (num3 - vector.x) / num3;
			scrollRegion |= ScrollRegion.Left;
		}
		else if (vector.x >= num4 && vector.x <= Camera.main.pixelWidth)
		{
			a = (vector.x - num4) / num3;
			scrollRegion |= ScrollRegion.Right;
		}
		float b = 0f;
		float num5 = Camera.main.pixelHeight * tweakScroll.scrollArea;
		float num6 = Camera.main.pixelHeight - num5;
		if (vector.y >= 0f && vector.y <= num5)
		{
			b = (num5 - vector.y) / num5;
			scrollRegion |= ScrollRegion.Bottom;
		}
		else if (vector.y >= num6 && vector.y <= Camera.main.pixelHeight)
		{
			b = (vector.y - num6) / num5;
			scrollRegion |= ScrollRegion.Top;
		}
		if (scrollRegion != 0)
		{
			scrollTimer = num + Time.deltaTime;
		}
		if (scrollTimer >= tweakScroll.scrollDelay)
		{
			float time = Mathf.Max(a, b);
			PiecewiseLinearFunction piecewiseLinearFunction = new PiecewiseLinearFunction(tweakScroll.distToAcceleration);
			scrollSpeed = num2 + Time.deltaTime * tweakScroll.scrollSpeedAcc * piecewiseLinearFunction.Eval(time);
			scrollSpeed = Mathf.Clamp(scrollSpeed, 0f, tweakScroll.scrollSpeedMax * piecewiseLinearFunction.Eval(time));
			if ((scrollRegion & ScrollRegion.Top) != 0)
			{
				HqController2.Instance.ActiveRoom.ScrollUp(scrollSpeed);
			}
			if ((scrollRegion & ScrollRegion.Bottom) != 0)
			{
				HqController2.Instance.ActiveRoom.ScrollDown(scrollSpeed);
			}
			if ((scrollRegion & ScrollRegion.Left) != 0)
			{
				HqController2.Instance.ActiveRoom.ScrollLeft(scrollSpeed);
			}
			if ((scrollRegion & ScrollRegion.Right) != 0)
			{
				HqController2.Instance.ActiveRoom.ScrollRight(scrollSpeed);
			}
		}
	}

	protected void UpdateDragScroll()
	{
		if (!dragLastFrame)
		{
			if (SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Right) && !SHSInput.IsOverUI())
			{
				dragLastFrame = true;
			}
		}
		else if (SHSInput.GetMouseButton(SHSInput.MouseButtonType.Right))
		{
			Vector3 a = Vector3.zero;
			a.x = SHSInput.GetAxis("Mouse X");
			a.y = SHSInput.GetAxis("Mouse Y");
			a /= Time.deltaTime * tweakScroll.scrollDragSpeed;
			if (tweakScroll.scrollDragInvert)
			{
				a = -a;
			}
			float num = Time.deltaTime;
			while ((double)num > 0.0)
			{
				float num2 = num;
				if (num2 > 0.0166666675f)
				{
					num2 = 0.0166666675f;
				}
				num -= num2;
				HqController2.Instance.ActiveRoom.ScrollVector(new Vector2(a.x * num2, a.y * num2));
			}
		}
		else
		{
			dragLastFrame = false;
		}
	}
}
