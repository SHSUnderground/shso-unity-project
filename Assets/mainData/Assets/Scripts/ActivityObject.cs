using UnityEngine;

public class ActivityObject : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public ActivitySpawnPoint spawner;

	private float defaultRadius;

	public bool isActivated;

	private bool isDespawned;

	protected virtual void OnEnable()
	{
		SphereCollider component = base.gameObject.GetComponent<SphereCollider>();
		if (component != null)
		{
			defaultRadius = component.radius;
		}
		if (isDespawned)
		{
			isDespawned = false;
		}
	}

	protected virtual void OnDisable()
	{
		if (!isDespawned)
		{
			isDespawned = true;
			AppShell.Instance.EventMgr.Fire(base.gameObject, new ActivityObjectDespawnMessage(base.gameObject, true));
		}
	}

	protected virtual void Start()
	{
		AppShell.Instance.EventMgr.Fire(base.gameObject, new ActivityObjectSpawnMessage(base.gameObject));
	}

	public virtual void Activate()
	{
		isActivated = true;
	}

	public virtual void Deactivate()
	{
		isActivated = false;
	}

	public virtual void ToggleActiveState(bool toggle)
	{
		if (toggle)
		{
			Activate();
		}
		else
		{
			Deactivate();
		}
	}

	public void Despawn()
	{
		if (!isDespawned)
		{
			isDespawned = true;
			AppShell.Instance.EventMgr.Fire(base.gameObject, new ActivityObjectDespawnMessage(base.gameObject, false));
			Object.Destroy(base.gameObject);
		}
	}

	public virtual void ActionTriggered(ActivityObjectActionNameEnum action)
	{
		spawner.activityReference.OnActivityAction(action, this, null);
	}

	public virtual void ResetForNewPet(PetData data)
	{
		SpecialAbility specialAbility = null;
		if (data != null)
		{
			specialAbility = data.abilities.Find(delegate(SpecialAbility ability)
			{
				if (!(ability is SidekickSpecialAbilityGrab))
				{
					return false;
				}
				return ((bool)spawner && spawner.GetType().Name.ToLowerInvariant().Contains(((ability as SidekickSpecialAbility) as SidekickSpecialAbilityGrab).activityType.ToLowerInvariant())) ? true : false;
			});
		}
		if (specialAbility != null && specialAbility.isUnlocked())
		{
			SphereCollider component = base.gameObject.GetComponent<SphereCollider>();
			if (component != null)
			{
				component.radius = (specialAbility as SidekickSpecialAbilityGrab).radius;
			}
		}
		else if (defaultRadius != 0f)
		{
			SphereCollider component2 = base.gameObject.GetComponent<SphereCollider>();
			if (component2 != null)
			{
				component2.radius = defaultRadius;
			}
		}
	}
}
