using System.Collections;
using UnityEngine;

public class TeleportMovementActionNode : DockPoint
{
	public delegate void OnFinished();

	public EffectSequenceReference[] effects;

	public CameraLite cameraOverride;

	public float cameraBlendInTime = 0.5f;

	public float cameraBlendOutTime = 0.5f;

	private readonly Vector3 PURGATORY_OFFSET = new Vector3(0f, 1000f, 0f);

	public void Use(CharacterGlobals player, OnFinished onFinished)
	{
		StartCoroutine(CoUse(player, onFinished));
	}

	private IEnumerator CoUse(CharacterGlobals player, OnFinished onFinished)
	{
		yield return StartCoroutine(MovePlayer(player));
		yield return StartCoroutine(PlayAllSequences(player));
		if (cameraOverride != null && Utils.IsLocalPlayer(player))
		{
			CameraLiteManager.Instance.PopCamera(cameraBlendOutTime);
		}
		if (onFinished != null)
		{
			onFinished();
		}
	}

	private IEnumerator MovePlayer(CharacterGlobals player)
	{
		GameObject purgatory = new GameObject("_TeleportPurgatory");
		purgatory.transform.parent = base.transform;
		purgatory.transform.position = base.transform.position + PURGATORY_OFFSET;
		TeleportPlayer(player, purgatory.transform);
		float normalGravity = player.motionController.gravity;
		player.motionController.gravity = 0f;
		bool isLocal = Utils.IsLocalPlayer(player);
		CameraTargetHelper cameraTargetHelper = null;
		if (cameraOverride != null && isLocal)
		{
			CameraLiteManager.Instance.PushCamera(cameraOverride, cameraBlendInTime);
		}
		else if (isLocal)
		{
			cameraTargetHelper = player.GetComponentInChildren<CameraTargetHelper>();
			cameraTargetHelper.SetTarget(base.transform);
		}
		yield return new WaitForSeconds(cameraBlendInTime);
		TeleportPlayer(player, base.transform);
		if (cameraOverride == null && cameraTargetHelper != null && isLocal)
		{
			cameraTargetHelper.SetTarget(player.transform);
			cameraTargetHelper.transform.position = player.transform.position;
		}
		Object.Destroy(purgatory);
		player.motionController.gravity = normalGravity;
	}

	protected void TeleportPlayer(CharacterGlobals player, Transform newLocation)
	{
		player.motionController.teleportTo(newLocation.position, newLocation.rotation);
		player.motionController.setDestination(newLocation.position, newLocation.forward);
	}

	protected IEnumerator PlayAllSequences(CharacterGlobals player)
	{
		EffectSequenceReference[] array = effects;
		foreach (EffectSequenceReference effect in array)
		{
			yield return StartCoroutine(PlaySequence(player, effect));
		}
	}

	protected IEnumerator PlaySequence(CharacterGlobals player, EffectSequenceReference effect)
	{
		bool effectFinished = false;
		float timeoutTime = Time.time + 10f;
		EffectSequence sequencePrefab = effect.GetSequence(player.gameObject);
		if (sequencePrefab != null)
		{
			GameObject prefabInstance = Object.Instantiate(sequencePrefab.gameObject) as GameObject;
			EffectSequence sequenceInstance = prefabInstance.GetComponentInChildren<EffectSequence>();
			if (sequenceInstance != null)
			{
				sequenceInstance.Initialize(player.gameObject, delegate
				{
					effectFinished = true;
					Object.Destroy(sequenceInstance.gameObject);
				}, null);
				if (!sequenceInstance.AutoStart)
				{
					sequenceInstance.StartSequence();
				}
			}
			else
			{
				effectFinished = true;
				Object.Destroy(prefabInstance);
			}
		}
		else
		{
			effectFinished = true;
		}
		while (!effectFinished && Time.time < timeoutTime)
		{
			yield return 0;
		}
	}
}
