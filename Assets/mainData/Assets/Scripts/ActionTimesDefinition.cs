public class ActionTimesDefinition : StaticDataDefinition, IStaticDataDefinition
{
	public static ActionTimesDefinition Instance;

	protected float attachMin;

	protected float attachMax = 10f;

	protected float getupMin;

	protected float getupMax = 10f;

	protected float knockdownMin;

	protected float knockdownMax = 10f;

	protected float largeMin;

	protected float largeMax = 10f;

	protected float launchMin;

	protected float launchMax = 10f;

	protected float smallMin;

	protected float smallMax = 10f;

	protected float stunMin;

	protected float stunMax = 10f;

	protected float pickupMin;

	protected float pickupMax = 10f;

	protected float throwMin;

	protected float throwMax = 10f;

	public float AttachMin
	{
		get
		{
			return attachMin;
		}
	}

	public float AttachMax
	{
		get
		{
			return attachMax;
		}
	}

	public float GetupMin
	{
		get
		{
			return getupMin;
		}
	}

	public float GetupMax
	{
		get
		{
			return getupMax;
		}
	}

	public float KnockdownMin
	{
		get
		{
			return knockdownMin;
		}
	}

	public float KnockdownMax
	{
		get
		{
			return knockdownMax;
		}
	}

	public float LargeMin
	{
		get
		{
			return largeMin;
		}
	}

	public float LargeMax
	{
		get
		{
			return largeMax;
		}
	}

	public float LaunchMin
	{
		get
		{
			return launchMin;
		}
	}

	public float LaunchMax
	{
		get
		{
			return launchMax;
		}
	}

	public float SmallMin
	{
		get
		{
			return smallMin;
		}
	}

	public float SmallMax
	{
		get
		{
			return smallMax;
		}
	}

	public float StunMin
	{
		get
		{
			return stunMin;
		}
	}

	public float StunMax
	{
		get
		{
			return stunMax;
		}
	}

	public float PickupMin
	{
		get
		{
			return pickupMin;
		}
	}

	public float PickupMax
	{
		get
		{
			return pickupMax;
		}
	}

	public float ThrowMin
	{
		get
		{
			return throwMin;
		}
	}

	public float ThrowMax
	{
		get
		{
			return throwMax;
		}
	}

	public void InitializeFromData(DataWarehouse data)
	{
		if (data.GetCount("brawler_action_times") != 1)
		{
			CspUtils.DebugLog("Invalid number of action time blocks!");
		}
		DataWarehouse data2 = data.GetData("brawler_action_times");
		attachMin = data2.TryGetFloat("attach_min", 0f);
		attachMax = data2.TryGetFloat("attach_max", 10f);
		getupMin = data2.TryGetFloat("getup_min", 0f);
		getupMax = data2.TryGetFloat("getup_max", 10f);
		knockdownMin = data2.TryGetFloat("knockdown_min", 0f);
		knockdownMax = data2.TryGetFloat("knockdown_max", 10f);
		largeMin = data2.TryGetFloat("large_min", 0f);
		largeMax = data2.TryGetFloat("large_max", 10f);
		launchMin = data2.TryGetFloat("launch_min", 0f);
		launchMax = data2.TryGetFloat("launch_max", 10f);
		smallMin = data2.TryGetFloat("small_min", 0f);
		smallMax = data2.TryGetFloat("small_max", 10f);
		stunMin = data2.TryGetFloat("stun_min", 0f);
		stunMax = data2.TryGetFloat("stun_max", 10f);
		pickupMin = data2.TryGetFloat("pickup_min", 0f);
		pickupMax = data2.TryGetFloat("pickup_max", 10f);
		throwMin = data2.TryGetFloat("throw_min", 0f);
		throwMax = data2.TryGetFloat("throw_max", 10f);
	}

	public float ClampActionTime(float speed, float length, float minTime, float maxTime)
	{
		if (speed > 0f)
		{
			length /= speed;
		}
		if (length < minTime)
		{
			speed = length / minTime;
		}
		if (length > maxTime)
		{
			speed = length / maxTime;
		}
		return speed;
	}
}
