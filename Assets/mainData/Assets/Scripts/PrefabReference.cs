using UnityEngine;

[AddComponentMenu("Miscellaneous/PrefabReference")]
public class PrefabReference : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject prefab;

	public bool usePrefabTransform = true;

	public bool createAsChild = true;

	protected bool created;

	private void OnEnable()
	{
		if (!created && prefab != null)
		{
			Vector3 position = (!usePrefabTransform) ? Vector3.zero : prefab.transform.position;
			Quaternion rotation = (!usePrefabTransform) ? Quaternion.identity : prefab.transform.rotation;
			GameObject gameObject = Object.Instantiate(prefab, position, rotation) as GameObject;
			if (usePrefabTransform)
			{
				gameObject.transform.localScale = prefab.transform.localScale;
			}
			Utils.AttachGameObject(base.gameObject, gameObject);
			if (!createAsChild)
			{
				gameObject.transform.parent = base.transform.parent;
			}
			created = true;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
