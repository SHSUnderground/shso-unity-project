using UnityEngine;

public class PolymorphSpawnData
{
	private GameObject mOriginal;

	public GameObject Original
	{
		get
		{
			return mOriginal;
		}
	}

	public PolymorphSpawnData(GameObject polymorph)
	{
		mOriginal = polymorph;
	}
}
