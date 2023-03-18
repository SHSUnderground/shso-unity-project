using UnityEngine;

[AddComponentMenu("Hq/Explosives/Timed Explosive")]
public class HqTimedExplosive : HqExplosive
{
	public float DetonationTimer;

	protected float elapsedTime;

	protected bool isFuseLit;

	public override void Start()
	{
		isFuseLit = false;
		base.Start();
	}

	public override void Update()
	{
		if (base.IsEnabled)
		{
			if (!isFuseLit)
			{
				isFuseLit = true;
				elapsedTime = 0f;
			}
			else
			{
				elapsedTime += Time.deltaTime;
				if (elapsedTime >= DetonationTimer)
				{
					Explode();
					isFuseLit = false;
				}
			}
		}
		base.Update();
	}
}
