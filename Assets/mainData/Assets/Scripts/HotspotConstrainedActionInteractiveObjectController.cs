using UnityEngine;

public class HotspotConstrainedActionInteractiveObjectController : HotspotActionInteractiveObjectController
{
	public ActionConstraint[] Constraints;

	public override bool CanPlayerUse(GameObject player)
	{
		if (inUse)
		{
			return false;
		}
		if (IsPlayerAirborne(player))
		{
			return false;
		}
		if (Constraints != null)
		{
			ActionConstraint[] constraints = Constraints;
			foreach (ActionConstraint actionConstraint in constraints)
			{
				if (!actionConstraint.Passes(player))
				{
					return false;
				}
			}
			return true;
		}
		return base.CanPlayerUse(player);
	}
}
