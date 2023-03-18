using UnityEngine;

[AddComponentMenu("Brawler/ObjectCombatController")]
public class ObjectCombatController : CombatController
{
	public float objectHealth = 1f;

	public GameObject destructionEffectPrefab;

	public string destructionScenarioEvent;

	public bool destructionScenarioEventHostOnly;

	public float targetHeightOverride;

	protected NetworkComponent netComp;

	protected override void Start()
	{
		base.Start();
		netComp = (GetComponent(typeof(NetworkComponent)) as NetworkComponent);
	}

	protected override void Update()
	{
		if (base.transform.rigidbody != null && base.transform.rigidbody.IsSleeping())
		{
			base.transform.rigidbody.WakeUp();
		}
		base.Update();
	}

	public override void hitByAttack(Vector3 impactPosition, CombatController sourceCombatController, GameObject source, float damage, ImpactResultData impactResultData)
	{
		NetworkComponent networkComponent = source.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
		if (networkComponent != null)
		{
			NetActionHitObject action = new NetActionHitObject(source, netComp.goNetId, damage);
			networkComponent.QueueNetAction(action);
		}
		takeDamage(damage, source);
	}

	public void takeDamageRemote(float damage, GameObject source)
	{
		takeDamage(damage, source);
	}

	protected override void takeDamage(float damage, GameObject source)
	{
		playImpactEffect();
		if (objectHealth > 0f)
		{
			objectHealth -= damage;
			if (objectHealth <= 0f)
			{
				killed(source, 0f);
			}
		}
	}

	public override void killed(GameObject killer, float duration)
	{
		if (isKilled)
		{
			return;
		}
		isKilled = true;
		bool flag = true;
		if (AppShell.Instance.ServerConnection != null && netComp != null)
		{
			flag = false;
			if (AppShell.Instance.ServerConnection.IsGameHost())
			{
				dropPickup();
			}
		}
		else
		{
			dropPickup();
		}
		if (destructionEffectPrefab != null)
		{
			if (flag || destructionEffectPrefab.GetComponent(typeof(NetworkComponent)) == null)
			{
				GameObject g = Object.Instantiate(destructionEffectPrefab, base.transform.position, base.transform.rotation) as GameObject;
				EffectSequence component = Utils.GetComponent<EffectSequence>(g);
				if ((bool)component)
				{
					component.Initialize(null, null, null);
					component.StartSequence();
				}
			}
			else if (AppShell.Instance.ServerConnection.IsGameHost())
			{
				GameObject newObject = Object.Instantiate(destructionEffectPrefab, base.transform.position, base.transform.rotation) as GameObject;
				netComp.AnnounceObjectSpawn(newObject, "ObjectCombatController", destructionEffectPrefab.name);
			}
		}
		if (destructionScenarioEvent != null && destructionScenarioEvent != string.Empty && (!destructionScenarioEventHostOnly || (AppShell.Instance.ServerConnection != null && AppShell.Instance.ServerConnection.IsGameHost())))
		{
			ScenarioEventManager.Instance.FireScenarioEvent(destructionScenarioEvent, false);
		}
		Utils.ActivateTree(base.gameObject, false);
		if (killer != null)
		{
			PlayerInputController component2 = killer.GetComponent<PlayerInputController>();
			if (component2 != null && component2.IsMouseOver(base.gameObject))
			{
				component2.ForceMouseRollverUpdate();
			}
		}
	}

	public override void playImpactEffectFromSource(CombatController sourceCombatController, string impactName, bool faceCamera)
	{
		GameObject gameObject = sourceCombatController.effectSequenceSource.GetEffectSequencePrefabByName(impactName) as GameObject;
		if (gameObject != null)
		{
			GameObject gameObject2 = Object.Instantiate(gameObject) as GameObject;
			gameObject2.transform.position = base.TargetPosition;
			if (faceCamera)
			{
				gameObject2.transform.LookAt(Camera.main.transform);
			}
			gameObject2.transform.parent = base.transform;
		}
		else
		{
			CspUtils.DebugLog("Specified impact effect '" + impactName + "' does not exist in attack effect list for " + sourceCombatController.gameObject.name);
		}
	}

	public override GameObject playImpactEffect()
	{
		GameObject gameObject = base.playImpactEffect();
		if (gameObject != null)
		{
			EffectSequence component = Utils.GetComponent<EffectSequence>(gameObject);
			if (component != null)
			{
				component.Initialize(base.gameObject, null, null);
				component.StartSequence();
			}
		}
		return gameObject;
	}

	public override GameObject RemoteSpawn(Vector3 spawnLoc, Quaternion spawnRot, GoNetId newID, string prefabNam, GameObject parent)
	{
		GameObject gameObject = Object.Instantiate(destructionEffectPrefab, spawnLoc, Quaternion.identity) as GameObject;
		if (newID.IsValid())
		{
			NetworkComponent networkComponent = gameObject.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
			if (networkComponent != null)
			{
				networkComponent.goNetId = newID;
			}
		}
		return gameObject;
	}

	public override void GetTargetTransform()
	{
		targetTransform = base.gameObject.transform;
		targetHeight = targetHeightOverride;
		if (targetHeightOverride == 0f)
		{
			Collider collider = GetComponent(typeof(Collider)) as Collider;
			if (collider != null)
			{
				Bounds bounds = collider.bounds;
				Vector3 center = bounds.center;
				float y = center.y;
				Vector3 position = base.transform.position;
				targetHeight = y - position.y;
				Vector3 size = bounds.size;
				heightOffsetTolerance = size.y;
			}
		}
	}

	public override bool isHealthInitialized()
	{
		return true;
	}
}
