using UnityEngine;

[RequireComponent(typeof(Collider))]
[AddComponentMenu("Triggers/Spawn Trigger")]
public class SpawnTrigger : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject objectToSpawn;

	public GameObject parent;

	private NetworkComponent _cachedTriggerNet;

	private GameObject spawnedObject;

	protected NetworkComponent TriggerNet
	{
		get
		{
			if (_cachedTriggerNet == null)
			{
				_cachedTriggerNet = GetComponent<NetworkComponent>();
			}
			return _cachedTriggerNet;
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (!(objectToSpawn == null) && !(spawnedObject != null))
		{
			CharacterGlobals component = Utils.GetComponent<CharacterGlobals>(other, Utils.SearchParents);
			if (!(component == null))
			{
				LocalSpawn(component);
			}
		}
	}

	public GameObject RemoteSpawn(Vector3 spawnLoc, Quaternion spawnRot, GoNetId newID, string prefabName, GameObject parent)
	{
		GameObject gameObject = InstantiateObject();
		if (gameObject != null)
		{
			gameObject.transform.position = spawnLoc;
			gameObject.transform.rotation = spawnRot;
			if (newID.IsValid())
			{
				NetworkComponent component = gameObject.GetComponent<NetworkComponent>();
				if (component != null)
				{
					component.goNetId = newID;
				}
			}
		}
		return gameObject;
	}

	protected void LocalSpawn(CharacterGlobals character)
	{
		NetworkComponent component = objectToSpawn.GetComponent<NetworkComponent>();
		if (component != null && AppShell.Instance.ServerConnection != null)
		{
			if (TriggerNet == null)
			{
				CspUtils.DebugLog("Attempted to spawn networked <" + objectToSpawn.name + "> using a non-networked SpawnTrigger <" + base.name + ">: Aborting spawn");
				return;
			}
			NetworkComponent networkComponent = character.networkComponent;
			if ((networkComponent != null && networkComponent.IsOwner()) || (networkComponent == null && AppShell.Instance.ServerConnection.IsGameHost()))
			{
				GameObject gameObject = InstantiateObject();
				if (gameObject != null)
				{
					TriggerNet.AnnounceObjectSpawn(gameObject, "SpawnTrigger", objectToSpawn.name);
				}
			}
		}
		else
		{
			InstantiateObject();
		}
	}

	protected GameObject InstantiateObject()
	{
		if (spawnedObject != null || objectToSpawn == null)
		{
			return null;
		}
		spawnedObject = (Object.Instantiate(objectToSpawn) as GameObject);
		if (spawnedObject != null)
		{
			Utils.AttachGameObject(parent ?? base.gameObject, spawnedObject);
		}
		return spawnedObject;
	}
}
