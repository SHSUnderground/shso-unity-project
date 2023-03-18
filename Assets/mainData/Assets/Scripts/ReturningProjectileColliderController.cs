using UnityEngine;

public class ReturningProjectileColliderController : ProjectileColliderController
{
	private bool returning;

	private float returnDuration = 1f;

	private Vector3 returnStart = default(Vector3);

	private float returnArc;

	private float currentReturnTime;

	protected override void Start()
	{
		returning = false;
		base.Start();
	}

	protected override void Update()
	{
		if (impacted && !returning)
		{
			startReturning();
			return;
		}
		if (Time.time > endTime)
		{
			if (returning)
			{
				returnArrived();
			}
			else
			{
				startReturning();
			}
			return;
		}
		if (returning)
		{
			if (returnArc != 0f)
			{
				currentReturnTime += Time.deltaTime;
				float num = Mathf.Clamp01(currentReturnTime / returnDuration);
				Vector3 vector = returnStart * (1f - num) + emitter.transform.position * num;
				float num2 = 0f;
				num2 = ((!(num < 0.5f)) ? (2f * (1f - num)) : (2f * num));
				num2 = 1f - (1f - num2) * (1f - num2);
				num2 *= returnArc;
				vector.y += num2;
				Vector3 b = vector - base.transform.position;
				base.rigidbody.velocity = Vector3.zero;
				base.rigidbody.MovePosition(vector);
				if (num >= 1f)
				{
					returnArrived();
					return;
				}
				if (impactData.projectileRotateToVelocity)
				{
					base.gameObject.transform.LookAt(base.gameObject.transform.position - b);
				}
			}
			else
			{
				Vector3 velocity = emitter.transform.position - base.transform.position;
				float f = impactData.projectileSpeed * Time.deltaTime;
				f = Mathf.Pow(f, 2f);
				if (velocity.sqrMagnitude < f)
				{
					returnArrived();
					return;
				}
				velocity.Normalize();
				velocity *= impactData.projectileSpeed;
				base.rigidbody.velocity = velocity;
				if (impactData.projectileRotateToVelocity)
				{
					base.gameObject.transform.LookAt(base.gameObject.transform.position - base.gameObject.transform.rigidbody.velocity);
				}
			}
		}
		base.Update();
	}

	protected override void OnTriggerEnter(Collider collider)
	{
		if (!returning)
		{
			base.OnTriggerEnter(collider);
		}
	}

	protected void returnArrived()
	{
		if (charGlobals != null)
		{
			charGlobals.combatController.ReturningProjectileArrived();
		}
		Object.Destroy(base.gameObject);
	}

	protected void startReturning()
	{
		returning = true;
		endTime = Time.time + impactData.projectileLifespan;
		if (impactData.projectileReturnArc != 0f)
		{
			returnStart = base.transform.position;
			returnDuration = Mathf.Sqrt(impactData.projectileReturnArc * 4f / (charGlobals.motionController.gravity * 3f));
			returnArc = impactData.projectileReturnArc;
			currentReturnTime = 0f;
		}
	}
}
