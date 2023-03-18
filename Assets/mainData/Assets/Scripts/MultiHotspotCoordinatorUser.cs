using UnityEngine;

public class MultiHotspotCoordinatorUser : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public MultiHotspotCoordinator coordinator;

	private void OnDisable()
	{
		if (coordinator != null)
		{
			coordinator.RemoveUser(base.gameObject);
			coordinator = null;
		}
	}

	public static void Attach(GameObject obj, MultiHotspotCoordinator coordinator)
	{
		if (obj != null && coordinator != null)
		{
			MultiHotspotCoordinatorUser multiHotspotCoordinatorUser = Utils.AddComponent<MultiHotspotCoordinatorUser>(obj);
			multiHotspotCoordinatorUser.coordinator = coordinator;
			coordinator.AddUser(obj);
		}
	}

	public static void Detach(GameObject obj)
	{
		if (obj != null)
		{
			MultiHotspotCoordinatorUser component = Utils.GetComponent<MultiHotspotCoordinatorUser>(obj);
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
	}
}
