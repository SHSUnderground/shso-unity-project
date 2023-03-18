using UnityEngine;

[AddComponentMenu("Hq/Explosives/Proximity Explosive")]
public class HqProximityExplosive : HqExplosive
{
	public override void Update()
	{
		if (base.IsEnabled)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, Radius);
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				if (collider != null)
				{
					AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(collider.gameObject, Utils.SearchChildren);
					if (component != null)
					{
						Explode();
						break;
					}
				}
			}
		}
		base.Update();
	}
}
