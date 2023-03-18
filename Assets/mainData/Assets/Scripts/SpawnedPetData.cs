using UnityEngine;

public class SpawnedPetData
{
	public int petTypeID;

	public GameObject petObject;

	public GameObject parentPlayerObject;

	public SpawnedPetData(int typeID, GameObject spawnedObject, GameObject playerObject)
	{
		petTypeID = typeID;
		petObject = spawnedObject;
		parentPlayerObject = playerObject;
	}
}
