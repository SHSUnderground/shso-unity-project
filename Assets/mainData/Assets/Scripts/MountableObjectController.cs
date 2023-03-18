using System.Collections;
using UnityEngine;

public class MountableObjectController : HotspotInteractiveObjectController
{
	public enum CameraMountStyle
	{
		StayOnCharacter,
		TargetThePelvis
	}

	protected class PlayerBlob
	{
		private MountableObjectController owner;

		private GameObject player;

		private OnDone onDone;

		private CameraLiteManager cameraMgr;

		private Transform cameraTargetStorage;

		private bool bCameraOverriden;

		private bool isLocal;

		private bool ReadyToMount
		{
			get
			{
				return !isLocal || owner.readyToMount;
			}
			set
			{
				if (isLocal)
				{
					Highlightable = value;
					owner.readyToMount = value;
				}
			}
		}

		private bool Highlightable
		{
			set
			{
				owner.owner.highlightOnHover = value;
				owner.owner.highlightOnProximity = value;
				owner.owner.GotoBestState();
			}
		}

		public PlayerBlob(MountableObjectController owner)
		{
			this.owner = owner;
		}

		public bool StartWithPlayer(GameObject player, OnDone onDone)
		{
			this.player = player;
			isLocal = Utils.IsLocalPlayer(player);
			this.onDone = onDone;
			if (!ReadyToMount)
			{
				CspUtils.DebugLog("not ready to mount this mountable object");
				return false;
			}
			BehaviorManager component = player.GetComponent<BehaviorManager>();
			if (component == null)
			{
				CspUtils.DebugLog("Player attempted to mount an interactive object, but the player does not have an attached BehaviorManager");
				return false;
			}
			PlayVO(player);
			BehaviorApproach behaviorApproach = component.requestChangeBehavior(typeof(BehaviorApproach), false) as BehaviorApproach;
			if (behaviorApproach == null)
			{
				CspUtils.DebugLog("Could not change player behavior to BehaviorApproach");
				return false;
			}
			PointOrientation approachPoint = owner.GetApproachPoint(player);
			behaviorApproach.Initialize(approachPoint.Position, approachPoint.Rotation, true, ApproachArrived, ApproachCancelled, 0f, 2f, true, false);
			ReadyToMount = false;
			return true;
		}

		private void ApproachArrived(GameObject player)
		{
			Mount(player);
		}

		private void ApproachCancelled(GameObject player)
		{
			ReadyToMount = true;
			if (onDone != null)
			{
				onDone(player, CompletionStateEnum.Canceled);
			}
		}

		private void Mount(GameObject player)
		{
			DockPoint[] components = Utils.GetComponents<DockPoint>(owner.transform.parent.gameObject, Utils.SearchChildren);
			DockPoint dockPoint = null;
			DockPoint[] array = components;
			foreach (DockPoint dockPoint2 in array)
			{
				if (dockPoint2.Name.Contains("use"))
				{
					dockPoint = dockPoint2;
					break;
				}
			}
			if (dockPoint == null)
			{
				return;
			}
			if (owner.cameraMountStyle == CameraMountStyle.TargetThePelvis && Utils.IsLocalPlayer(player))
			{
				if (cameraMgr == null)
				{
					cameraMgr = CameraLiteManager.Instance;
				}
				if (cameraMgr != null)
				{
					CameraLite currentCamera = cameraMgr.GetCurrentCamera();
					if (currentCamera != null)
					{
						cameraTargetStorage = currentCamera.GetTarget();
						Transform transform = Utils.FindNodeInChildren(player.transform, "Pelvis");
						if (transform != null)
						{
							currentCamera.SetTarget(transform);
						}
						else
						{
							cameraTargetStorage = null;
						}
					}
				}
			}
			BehaviorManager component = player.GetComponent<BehaviorManager>();
			BehaviorMount behaviorMount = component.requestChangeBehavior(typeof(BehaviorMount), false) as BehaviorMount;
			if (behaviorMount != null)
			{
				behaviorMount.Initialize(dockPoint, OnMountDone, owner.jumpHeight, owner.mountAnimPart1, owner.mountAnimPart2, owner.mountAnimPart3, owner.mountSequenceName, owner.mountTime);
				return;
			}
			CspUtils.DebugLog("Could not switch to mount behavior");
			ReadyToMount = true;
			if (onDone != null)
			{
				onDone(player, CompletionStateEnum.Success);
			}
		}

		public void OnMountDone(GameObject player)
		{
			ReadyToMount = true;
			owner.StartCoroutine(RestoreCamera());
			bool flag = Utils.IsLocalPlayer(player);
			bCameraOverriden = false;
			if (flag)
			{
				if (cameraMgr == null)
				{
					cameraMgr = CameraLiteManager.Instance;
				}
				if (owner.alternativeCamera != null)
				{
					CameraTarget cameraTarget = owner.alternativeCamera.GetComponent(typeof(CameraTarget)) as CameraTarget;
					if (cameraTarget != null && cameraTarget.Target == null)
					{
						cameraTarget.Target = player.transform;
					}
					if (cameraMgr != null)
					{
						cameraMgr.PushCamera(owner.alternativeCamera, owner.cameraBlendInTime);
						bCameraOverriden = true;
						if (owner.cameraBlendOutStartTime > 0f)
						{
							owner.StartCoroutine(CameraWait(owner.cameraBlendOutStartTime, player));
						}
					}
				}
				if (cameraMgr != null)
				{
					cameraMgr.GetCurrentCamera().SplineState(true);
				}
			}
			if (owner.animationComponent != null)
			{
				owner.StartCoroutine(MountedObjectResponse(0f));
			}
			BehaviorManager behaviorManager = player.GetComponent(typeof(BehaviorManager)) as BehaviorManager;
			if (behaviorManager == null)
			{
				CspUtils.DebugLog("No BehaviorManager");
				return;
			}
			BehaviorSpline behaviorSpline = behaviorManager.requestChangeBehavior(typeof(BehaviorSpline), true) as BehaviorSpline;
			if (behaviorSpline == null)
			{
				CspUtils.DebugLog("No BehaviorSpline");
			}
			else
			{
				behaviorSpline.Initialize(owner.spline, true, SplineDone, owner.ignoreCollision, owner.splineFreeFall);
			}
		}

		protected void SnapToSpline()
		{
			if (owner.spline == null)
			{
				CspUtils.DebugLog("Warning: No movement spline found for this object!");
				ReadyToMount = true;
				return;
			}
			Vector3 pos;
			Quaternion rot;
			owner.spline.GetFirstPoint(out pos, out rot);
			CharacterMotionController component = player.GetComponent<CharacterMotionController>();
			component.setDestination(pos, true);
			component.teleportTo(pos, rot);
			ReadyToMount = true;
		}

		private IEnumerator RestoreCamera()
		{
			yield return 0;
			if (cameraTargetStorage != null)
			{
				cameraMgr.GetCurrentCamera().SetTarget(cameraTargetStorage);
				cameraTargetStorage = null;
			}
		}

		private IEnumerator MountedObjectResponse(float seconds)
		{
			yield return new WaitForSeconds(seconds);
			if (owner.animationComponent != null && !string.IsNullOrEmpty(owner.animationToPlay))
			{
				owner.animationComponent.Rewind();
				owner.animationComponent.Sample();
				owner.animationComponent[owner.animationToPlay].wrapMode = WrapMode.Once;
				owner.animationComponent.Play(owner.animationToPlay);
			}
			else
			{
				CspUtils.DebugLog("No Animation component found for " + owner.name);
			}
			if (owner.sound != null)
			{
				ShsAudioSource.PlayAutoSound(owner.sound);
			}
			if (owner.effect != null)
			{
				if (owner.fxInstance != null)
				{
					Utils.GetComponent<EffectSequence>(owner.fxInstance).Cancel();
				}
				owner.fxInstance = (Object.Instantiate(owner.effect.gameObject, Vector3.zero, Quaternion.identity) as GameObject);
				EffectSequence fxSequence = Utils.GetComponent<EffectSequence>(owner.fxInstance);
				if (!fxSequence.Initialized)
				{
					fxSequence.Initialize(owner.gameObject, delegate(EffectSequence seq)
					{
						Object.Destroy(seq.gameObject);
					}, null);
				}
				fxSequence.StartSequence();
			}
			if (owner.animationComponent != null && !string.IsNullOrEmpty(owner.animationToPlay))
			{
				yield return new WaitForSeconds(owner.animationComponent[owner.animationToPlay].length);
				owner.animationComponent.Stop();
				owner.animationComponent.Rewind();
				owner.animationComponent.Sample();
			}
		}

		protected virtual void SplineDone(GameObject player)
		{
			if (onDone != null)
			{
				onDone(player, CompletionStateEnum.Success);
			}
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(player);
			if (component != null)
			{
				component.DisableNetUpdates(false);
			}
			CameraExit(player);
		}

		protected IEnumerator CameraWait(float time, GameObject obj)
		{
			yield return new WaitForSeconds(time);
			CameraExit(obj);
		}

		protected void CameraExit(GameObject obj)
		{
			if (Utils.IsLocalPlayer(obj) && cameraMgr != null)
			{
				cameraMgr.GetCurrentCamera().SplineState(false);
				if (bCameraOverriden)
				{
					cameraMgr.PopCamera(owner.cameraBlendOutTime);
					bCameraOverriden = false;
				}
			}
		}

		protected void PlayVO(GameObject player)
		{
			if ((owner.hotSpotType & HotSpotType.Style.Web) != 0)
			{
				VOManager.Instance.PlayVO("use_web_hotspot", player);
			}
		}
	}

	public SplineController spline;

	public bool splineFreeFall;

	public float mountDistance = 2f;

	public float jumpHeight = 3f;

	private bool readyToMount = true;

	public Animation animationComponent;

	public CameraMountStyle cameraMountStyle = CameraMountStyle.TargetThePelvis;

	public float mountTime = 1f;

	public string mountAnimPart1 = "jump_run_up";

	public string mountAnimPart2 = "jump_run_fall";

	public string mountAnimPart3 = "jump_run_land";

	public string mountSequenceName = "mount_sequence";

	public string animationToPlay = "Take 001";

	public GameObject sound;

	public EffectSequence effect;

	private GameObject fxInstance;

	public CameraLite alternativeCamera;

	public float cameraBlendInTime = 0.5f;

	public float cameraBlendOutTime = 0.5f;

	public float cameraBlendOutStartTime = -1f;

	public bool ignoreCollision;

	private void OnDrawGizmosSelected()
	{
		Vector3 upVector = default(Vector3);
		Vector3 atVector = default(Vector3);
		float vectorLength = 0f;
		DockPoint dockPoint = null;
		Gizmos.color = Color.blue;
		DockPoint[] components = Utils.GetComponents<DockPoint>(base.gameObject.transform.parent.gameObject, Utils.SearchChildren);
		DockPoint[] array = components;
		foreach (DockPoint dockPoint2 in array)
		{
			if (dockPoint2.Name.Contains("use"))
			{
				dockPoint = dockPoint2;
				break;
			}
		}
		if (dockPoint == null)
		{
			return;
		}
		DockPoint[] array2 = components;
		foreach (DockPoint dockPoint3 in array2)
		{
			if (dockPoint3.Name.Contains("entry"))
			{
				InitMountVectors(dockPoint3.gameObject, dockPoint.transform.position, ref upVector, ref atVector, ref vectorLength);
				int num = (int)Mathf.Min(Mathf.Max(Mathf.Ceil(vectorLength / 0.5f), 1f), 40f);
				float num2 = 1f / (float)num;
				for (int k = 0; k < num; k++)
				{
					float percentage = (float)k * num2;
					float percentage2 = (float)(k + 1) * num2;
					Vector3 from = EvalPosition(percentage, jumpHeight, dockPoint3.transform.position, atVector);
					Vector3 to = EvalPosition(percentage2, jumpHeight, dockPoint3.transform.position, atVector);
					Gizmos.DrawLine(from, to);
				}
			}
		}
	}

	public static void InitMountVectors(GameObject source, Vector3 destination, ref Vector3 upVector, ref Vector3 atVector, ref float vectorLength)
	{
		upVector.y = 1f;
		atVector = destination - source.transform.position;
		vectorLength = atVector.magnitude;
		float num = Mathf.Sqrt(1f - upVector.y * upVector.y);
		Vector3 vector = new Vector3(atVector.x, 0f, atVector.z);
		vector.Normalize();
		upVector.x = vector.x * num * vectorLength;
		upVector.y *= vectorLength * 3f;
		upVector.z = vector.z * num * vectorLength;
	}

	public static Vector3 EvalPosition(float percentage, float amplitude, Vector3 source, Vector3 destination)
	{
		float y = (1f - Mathf.Pow(percentage * 2f - 1f, 2f)) * amplitude;
		Vector3 a = destination * percentage;
		return a + source + new Vector3(0f, y, 0f);
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		if (animationComponent == null)
		{
			animationComponent = GetComponentInChildren<Animation>();
		}
		reportHotSpotUse(player);
		return new PlayerBlob(this).StartWithPlayer(player, onDone);
	}

	protected PointOrientation GetApproachPoint(GameObject player)
	{
		DockPoint[] components = Utils.GetComponents<DockPoint>(owner, Utils.SearchChildren);
		if (components.Length > 0)
		{
			DockPoint dockPoint = null;
			float num = -1f;
			DockPoint[] array = components;
			foreach (DockPoint dockPoint2 in array)
			{
				if (dockPoint2.name.Contains("entry"))
				{
					float sqrMagnitude = (dockPoint2.transform.position - player.transform.position).sqrMagnitude;
					if (dockPoint == null || sqrMagnitude < num)
					{
						dockPoint = dockPoint2;
						num = sqrMagnitude;
					}
				}
			}
			if (dockPoint != null)
			{
				return new PointOrientation(dockPoint.transform);
			}
		}
		Vector3 direction = base.transform.position - player.transform.position;
		return new PointOrientation(base.transform.position - direction.normalized * mountDistance, direction);
	}
}
