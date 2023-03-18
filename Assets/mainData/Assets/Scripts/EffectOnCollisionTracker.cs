using System.Collections;
using UnityEngine;

public class EffectOnCollisionTracker : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private const float kMovementEpsilon = 0.5f;

	private const float kRaycastDistance = 3f;

	private const float kMinimumTime = 1f;

	public EffectSequence attachedSequence;

	public Vector3 collisionNormal;

	public Collider contactCollider;

	public EffectOnCollision source;

	protected bool unloading;

	private void Start()
	{
		if (attachedSequence != null)
		{
			StartCoroutine(CheckPerpendicularMovement());
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private void OnUnload()
	{
		unloading = true;
	}

	private void OnDisable()
	{
		if (attachedSequence != null && !unloading)
		{
			Object.Destroy(attachedSequence.gameObject);
		}
	}

	private IEnumerator CheckPerpendicularMovement()
	{
		Vector3 originalPosition = base.gameObject.transform.position;
		Vector3 originalEffectPosition = attachedSequence.transform.position;
		float startTime = Time.time;
		bool maintainViaSource2 = true;
		Vector3 delta;
		bool hitViaRaycast;
		do
		{
			yield return 0;
			delta = base.transform.position - originalPosition;
			attachedSequence.transform.position = originalEffectPosition + delta;
			hitViaRaycast = false;
			RaycastHit[] hits = Physics.RaycastAll(attachedSequence.transform.position + collisionNormal, -1f * collisionNormal, 3f);
			RaycastHit[] array = hits;
			foreach (RaycastHit hit in array)
			{
				if (hit.collider == contactCollider)
				{
					hitViaRaycast = true;
				}
			}
			maintainViaSource2 = (source == null || source.ShouldMaintainEffect(base.gameObject));
		}
		while (hitViaRaycast && maintainViaSource2 && Vector3.Project(delta, collisionNormal).sqrMagnitude < 0.5f);
		Object.Destroy(attachedSequence.gameObject);
		if (Time.time - startTime < 1f)
		{
			yield return new WaitForSeconds(1f - (Time.time - startTime));
		}
		Object.Destroy(this);
	}
}
