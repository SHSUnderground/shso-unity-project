using UnityEngine;

[AddComponentMenu("Hq/Hq Preplaced Item")]
public class HqPlacedItemInfo : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string ItemName;

	public Bounds ItemBounds;

	public virtual void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(ItemBounds.center, ItemBounds.size);
	}
}
