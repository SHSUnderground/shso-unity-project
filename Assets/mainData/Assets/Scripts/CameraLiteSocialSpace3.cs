using System;
using UnityEngine;

[AddComponentMenu("Camera/Camera Lite Social Space (two-zoom,zoom block)")]
public class CameraLiteSocialSpace3 : CameraLite
{
	public enum BoomStates
	{
		Default,
		Forward,
		Idle,
		Backwards,
		Zoom
	}

	[Serializable]
	public class BoomInfo
	{
		public Vector3 boom = Vector3.zero;

		public float distance;

		public float minTimeToEnter = 1f;
	}

	public class BaseState : IDisposable, IShsState
	{
		public CameraLiteSocialSpace3 owner;

		public BoomInfo info;

		public float timer;

		public BaseState(CameraLiteSocialSpace3 owner, BoomInfo info)
		{
			this.owner = owner;
			this.info = info;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
			owner = null;
			info = null;
		}

		public virtual void Enter(Type previousState)
		{
			timer = 0f;
		}

		public virtual void Update()
		{
			timer += owner.cameraDeltaTime;
		}

		public virtual void Leave(Type nextState)
		{
			timer = 0f;
		}

		public bool CheckForZoom()
		{
			if (owner.targetSpeed <= 0.001f)
			{
				float mouseWheelDelta = SHSInput.GetMouseWheelDelta();
				if (mouseWheelDelta > 0f)
				{
					owner.fsm.GotoState<ZoomState>();
					return true;
				}
			}
			return false;
		}

		public void UpdateSprings(Vector3 target, Vector3 lookAt)
		{
			if (owner.springEnabled)
			{
				owner.springCamera.Update(target, owner.cameraDeltaTime);
				float constant = owner.springLookAt.constant;
				if (owner.onSpline)
				{
					owner.springLookAt.constant *= owner.lookAtSplineMod;
				}
				owner.springLookAt.Update(lookAt, owner.cameraDeltaTime);
				owner.springLookAt.constant = constant;
				owner.cameraPosition = owner.springCamera.position;
				owner.lookAtPosition = owner.springLookAt.position;
			}
			else
			{
				owner.cameraPosition = target;
				owner.lookAtPosition = lookAt;
			}
		}

		public void LerpToDesired(float t)
		{
			Vector3 target = owner.target.position + info.boom.normalized * info.distance;
			UpdateSprings(target, owner.target.position);
		}
	}

	private class IdleState : BaseState
	{
		private ForwardState forward;

		private BackwardState backward;

		public IdleState(CameraLiteSocialSpace3 owner, BoomInfo info)
			: base(owner, info)
		{
		}

		public override void Dispose(bool disposing)
		{
			try
			{
				forward = null;
				backward = null;
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override void Update()
		{
			base.Update();
			if (CheckForZoom())
			{
				return;
			}
			if (forward == null)
			{
				forward = owner.fsm.GetState<ForwardState>();
				backward = owner.fsm.GetState<BackwardState>();
			}
			if (owner.targetSpeedCamRelative >= owner.deadZone)
			{
				forward.timer += owner.cameraDeltaTime;
				backward.timer = 0f;
				if (forward.timer >= forward.info.minTimeToEnter)
				{
					owner.fsm.GotoState<ForwardState>();
				}
			}
			else if (owner.targetSpeedCamRelative <= 0f - owner.deadZone)
			{
				forward.timer = 0f;
				backward.timer += owner.cameraDeltaTime;
				if (backward.timer >= backward.info.minTimeToEnter)
				{
					owner.fsm.GotoState<BackwardState>();
				}
			}
			else
			{
				forward.timer = 0f;
				backward.timer = 0f;
			}
			LerpToDesired(timer);
		}
	}

	private class ForwardState : BaseState
	{
		private IdleState idle;

		private BackwardState backward;

		public ForwardState(CameraLiteSocialSpace3 owner, BoomInfo info)
			: base(owner, info)
		{
		}

		public override void Dispose(bool disposing)
		{
			try
			{
				idle = null;
				backward = null;
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override void Update()
		{
			base.Update();
			if (CheckForZoom())
			{
				return;
			}
			if (idle == null)
			{
				idle = owner.fsm.GetState<IdleState>();
				backward = owner.fsm.GetState<BackwardState>();
			}
			if (owner.targetSpeedCamRelative >= owner.deadZone)
			{
				idle.timer = 0f;
				backward.timer = 0f;
			}
			else if (owner.targetSpeedCamRelative <= 0f - owner.deadZone)
			{
				idle.timer = 0f;
				backward.timer += owner.cameraDeltaTime;
				if (backward.timer >= backward.info.minTimeToEnter)
				{
					owner.fsm.GotoState<BackwardState>();
					return;
				}
			}
			else if (owner.targetSpeed <= 0.001f)
			{
				backward.timer = 0f;
				idle.timer += owner.cameraDeltaTime;
				if (idle.timer >= idle.info.minTimeToEnter)
				{
					owner.fsm.GotoState<IdleState>();
					return;
				}
			}
			LerpToDesired(timer);
		}
	}

	private class BackwardState : BaseState
	{
		private IdleState idle;

		private ForwardState forward;

		public BackwardState(CameraLiteSocialSpace3 owner, BoomInfo info)
			: base(owner, info)
		{
		}

		public override void Dispose(bool disposing)
		{
			try
			{
				idle = null;
				forward = null;
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override void Update()
		{
			base.Update();
			if (CheckForZoom())
			{
				return;
			}
			if (idle == null)
			{
				idle = owner.fsm.GetState<IdleState>();
				forward = owner.fsm.GetState<ForwardState>();
			}
			if (owner.targetSpeedCamRelative >= owner.deadZone)
			{
				idle.timer = 0f;
				forward.timer += owner.cameraDeltaTime;
				if (forward.timer >= forward.info.minTimeToEnter)
				{
					owner.fsm.GotoState<ForwardState>();
					return;
				}
			}
			else if (owner.targetSpeedCamRelative <= 0f - owner.deadZone)
			{
				idle.timer = 0f;
				forward.timer = 0f;
			}
			else if (owner.targetSpeed <= 0.001f)
			{
				forward.timer = 0f;
				idle.timer += owner.cameraDeltaTime;
				if (idle.timer >= idle.info.minTimeToEnter)
				{
					owner.fsm.GotoState<IdleState>();
					return;
				}
			}
			LerpToDesired(timer);
		}
	}

	private class ZoomState : BaseState
	{
		protected float wiggleTimer;

		protected bool blocked;

		public ZoomState(CameraLiteSocialSpace3 owner, BoomInfo info)
			: base(owner, info)
		{
		}

		public override void Enter(Type previousState)
		{
			base.Enter(previousState);
			wiggleTimer = 0f;
			blocked = false;
		}

		public override void Update()
		{
			base.Update();
			float mouseWheelDelta = SHSInput.GetMouseWheelDelta();
			if (mouseWheelDelta < 0f)
			{
				owner.fsm.GotoState<IdleState>();
				return;
			}
			if (owner.targetSpeed > 0.001f)
			{
				wiggleTimer += owner.cameraDeltaTime;
				if (wiggleTimer >= info.minTimeToEnter)
				{
					if (owner.targetSpeedCamRelative >= 0f)
					{
						owner.fsm.GotoState<ForwardState>();
					}
					else
					{
						owner.fsm.GotoState<BackwardState>();
					}
					return;
				}
			}
			else if (!blocked)
			{
				wiggleTimer = 0f;
			}
			if (!blocked)
			{
				Ray ray = new Ray(owner.transform.position, owner.transform.forward);
				int layerMask = 32769;
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, 1.5f, layerMask))
				{
					CspUtils.DebugLog("Camera zoom hit: " + hitInfo.collider.name);
					blocked = true;
					owner.springCamera.Reset(owner.transform.position);
					owner.springLookAt.Reset(owner.target.position);
					wiggleTimer = info.minTimeToEnter;
				}
				else
				{
					LerpToDesired(timer);
				}
			}
		}
	}

	public BoomInfo boomZoom;

	public BoomInfo boomForward;

	public BoomInfo boomIdle;

	public BoomInfo boomBackward;

	public float deadZone;

	public bool springEnabled = true;

	public Spring springCamera;

	public Spring springLookAt;

	public Spring springZoom;

	public Vector3 lookAtOffset = Vector3.zero;

	public float lookAtSplineMod = 2f;

	public BoomStates debugBoomOverride;

	protected ShsFSM fsm;

	protected CharacterMotionController targetMotionController;

	protected Vector3 targetLastFrame = Vector3.zero;

	protected float targetSpeed;

	protected float targetSpeedCamRelative;

	protected float cameraDeltaTime;

	protected Vector3 cameraPosition = Vector3.zero;

	protected Vector3 lookAtPosition = Vector3.zero;

	protected bool onSpline;

	public void OnEnable()
	{
		if (fsm == null)
		{
			fsm = new ShsFSM();
			fsm.AddState(new ForwardState(this, boomForward));
			fsm.AddState(new BackwardState(this, boomBackward));
			fsm.AddState(new ZoomState(this, boomZoom));
			fsm.AddState(new IdleState(this, boomIdle));
			fsm.GotoState<IdleState>();
		}
	}

	public void OnDisable()
	{
		if (fsm != null)
		{
			fsm.Dispose();
			fsm = null;
		}
	}

	public void Update()
	{
		if (!(target == null))
		{
			Vector3 lhs = (target.transform.position - targetLastFrame) / Time.deltaTime;
			targetLastFrame = target.transform.position;
			if (targetMotionController != null && targetMotionController.speed != 0f)
			{
				lhs /= targetMotionController.speed;
			}
			targetSpeed = lhs.magnitude;
			Vector3 forward = base.transform.forward;
			forward.y = 0f;
			targetSpeedCamRelative = Vector3.Dot(lhs, forward.normalized);
		}
	}

	public override void InitFromMgr()
	{
		base.InitFromMgr();
		Reset();
	}

	public override void WakeFromMgr()
	{
		base.WakeFromMgr();
		Reset();
	}

	public override void UpdateFromMgr(float deltaTime)
	{
		base.UpdateFromMgr(deltaTime);
		if (target == null)
		{
			return;
		}
		if (!onSpline)
		{
			switch (debugBoomOverride)
			{
			case BoomStates.Forward:
				fsm.GotoState<ForwardState>();
				break;
			case BoomStates.Idle:
				fsm.GotoState<IdleState>();
				break;
			case BoomStates.Backwards:
				fsm.GotoState<BackwardState>();
				break;
			case BoomStates.Zoom:
				fsm.GotoState<ZoomState>();
				break;
			default:
				CspUtils.DebugLog("Unknown debugBoomOverride: " + debugBoomOverride.ToString());
				break;
			case BoomStates.Default:
				break;
			}
		}
		else
		{
			fsm.GotoState<IdleState>();
		}
		cameraDeltaTime = deltaTime;
		fsm.Update();
		base.transform.position = cameraPosition;
		base.transform.LookAt(lookAtPosition + lookAtOffset);
		SetDistance((target.transform.position - base.transform.position).magnitude);
	}

	public override void SplineState(bool starting)
	{
		onSpline = starting;
	}

	public override void Reset()
	{
		base.Reset();
		fsm.ClearState();
		fsm.GotoState<IdleState>();
		targetMotionController = null;
		targetLastFrame = Vector3.zero;
		targetSpeed = 0f;
		targetSpeedCamRelative = 0f;
		onSpline = false;
		if (target != null)
		{
			targetLastFrame = target.position;
			targetMotionController = Utils.GetComponent<CharacterMotionController>(target, Utils.SearchChildren);
			springLookAt.Reset(target.position);
			cameraPosition = target.position + boomIdle.boom.normalized * boomIdle.distance;
			springCamera.Reset(cameraPosition);
		}
		else
		{
			springLookAt.Reset(base.transform.forward * 5000f);
			cameraPosition = base.transform.position;
			springCamera.Reset(cameraPosition);
		}
	}
}
