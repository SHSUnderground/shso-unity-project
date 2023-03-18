using System.Collections;
using UnityEngine;

public class ViewfinderInteractiveController : GlowableInteractiveController
{
	public class Use : BaseUse
	{
		protected delegate void CameraSequenceFinishDelegate();

		private ViewfinderInteractiveController owner;

		public Use(GameObject player, ViewfinderInteractiveController owner, OnDone onDone)
			: base(player, owner, onDone)
		{
			this.owner = owner;
		}

		public override bool Start()
		{
			if (base.Start())
			{
				Approach(owner.approachTarget ?? owner.transform);
				return true;
			}
			return false;
		}

		protected override void OnApproachArrived(GameObject player)
		{
			BehaviorLoopSequence behaviorLoopSequence = ChangeBehavior<BehaviorLoopSequence>(false);
			behaviorLoopSequence.Initialize(owner.useAnimation, OnManipulateFinished, owner.useDuration);
			VOManager.Instance.PlayVO("use_object", player);
		}

		protected void OnManipulateFinished(GameObject player)
		{
			base.Player.behaviorManager.endBehavior();
			ChangeBehavior<BehaviorIdle>(false);
			owner.StartCoroutine(DoCameraSequence(OnCameraSequenceFinished));
		}

		protected void OnCameraSequenceFinished()
		{
			if (base.Player != null)
			{
				base.Player.behaviorManager.endBehavior();
			}
			Done();
		}

		private IEnumerator DoCameraSequence(CameraSequenceFinishDelegate onCameraSequenceFinished)
		{
			PlaySFX(owner.startSFX);
			yield return owner.StartCoroutine(PushCamera(owner.cameraOverride, owner.cameraZoomInTime));
			yield return owner.StartCoroutine(PushCamera(owner.viewFinderCamera, -1f));
			ShowOverlay();
			yield return new WaitForSeconds(owner.viewHoldDuration);
			HideOverlay();
			yield return owner.StartCoroutine(PopCamera(-1f));
			PlaySFX(owner.endSFX);
			yield return owner.StartCoroutine(PopCamera(owner.cameraZoomOutTime));
			if (onCameraSequenceFinished != null)
			{
				onCameraSequenceFinished();
			}
		}

		private IEnumerator PushCamera(CameraLite newCam, float blendTime)
		{
			if (base.IsLocal)
			{
				CameraLiteManager.Instance.PushCamera(newCam, blendTime);
			}
			yield return owner.StartCoroutine(WaitForBlend(blendTime));
		}

		private IEnumerator PopCamera(float blendTime)
		{
			if (base.IsLocal)
			{
				CameraLiteManager.Instance.PopCamera(blendTime);
			}
			yield return owner.StartCoroutine(WaitForBlend(blendTime));
		}

		private IEnumerator WaitForBlend(float blendTime)
		{
			if (blendTime > 0f)
			{
				yield return new WaitForSeconds(blendTime);
			}
		}

		private void PlaySFX(ShsAudioSource sfx)
		{
			if (sfx != null)
			{
				ShsAudioSource.PlayAutoSound(sfx.gameObject, owner.transform);
			}
		}

		private void ShowOverlay()
		{
			if (base.IsLocal)
			{
				MeshRenderer[] componentsInChildren = owner.viewFinderCamera.GetComponentsInChildren<MeshRenderer>();
				foreach (MeshRenderer meshRenderer in componentsInChildren)
				{
					meshRenderer.enabled = true;
				}
			}
		}

		private void HideOverlay()
		{
			if (base.IsLocal)
			{
				MeshRenderer[] componentsInChildren = owner.viewFinderCamera.GetComponentsInChildren<MeshRenderer>();
				foreach (MeshRenderer meshRenderer in componentsInChildren)
				{
					meshRenderer.enabled = false;
				}
			}
		}
	}

	public Transform approachTarget;

	public string useAnimation = "emote_manipulate_loop";

	public float useDuration = 1f;

	public CameraLite cameraOverride;

	public CameraLite viewFinderCamera;

	public float cameraZoomInTime = 0.5f;

	public float viewHoldDuration = 3f;

	public float cameraZoomOutTime = 1f;

	public ShsAudioSource startSFX;

	public ShsAudioSource endSFX;

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		return new Use(player, this, onDone).Start();
	}
}
