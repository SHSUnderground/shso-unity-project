using UnityEngine;

[AddComponentMenu("Hq/Switch/Explosive Controller")]
public class HqExplosiveController : HqSwitchController
{
	public override void Flip()
	{
		if (base.gameObject.transform.parent != null)
		{
			HqExplosive component = Utils.GetComponent<HqExplosive>(base.gameObject.transform.parent, Utils.SearchChildren);
			if (component != null)
			{
				component.Explode();
			}
		}
	}

	public override bool CanUse()
	{
		return true;
	}
}
