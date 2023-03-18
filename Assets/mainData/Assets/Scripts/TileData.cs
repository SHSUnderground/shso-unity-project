using UnityEngine;

public class TileData
{
	public string id;

	public string bundle;

	public Vector3 position;

	public Quaternion rotation;

	public GameObject tileObj;

	public TileData()
	{
		id = string.Empty;
		bundle = string.Empty;
		position = Vector3.zero;
		rotation = Quaternion.identity;
		tileObj = null;
	}

	public void InitializeFromData(DataWarehouse data)
	{
		id = data.TryGetString("id", string.Empty);
		bundle = data.TryGetString("bundle", string.Empty);
		position = data.TryGetVector("position", Vector3.zero);
		rotation = data.TryGetQuaternion("rotation", Quaternion.identity);
	}
}
