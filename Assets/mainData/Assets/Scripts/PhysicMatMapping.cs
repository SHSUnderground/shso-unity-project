using System.Collections.Generic;
using UnityEngine;

public class PhysicMatMapping : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public PhysicMaterialEx[] materials;

	private static PhysicMatMapping _instance;

	private Dictionary<PhysicMaterialEx.MaterialType, PhysicMaterialEx> properties;

	public static PhysicMatMapping Instance
	{
		get
		{
			return _instance;
		}
	}

	public PhysicMaterialEx this[PhysicMaterialEx.MaterialType idx]
	{
		get
		{
			if (Instance.properties.ContainsKey(idx))
			{
				return Instance.properties[idx];
			}
			return null;
		}
	}

	public void Awake()
	{
		if (_instance != null)
		{
			CspUtils.DebugLog("A second PhysicMatMapping is being created.  This may lead to instabilities!");
		}
		else
		{
			_instance = this;
		}
		properties = new Dictionary<PhysicMaterialEx.MaterialType, PhysicMaterialEx>();
		for (int i = 0; i < materials.Length; i++)
		{
			properties.Add(materials[i].name, materials[i]);
		}
	}

	public void Disable()
	{
		_instance = null;
	}
}
