public class HqPortableHole : HqKillZone
{
	public override bool IsOn
	{
		get
		{
			if (hqObj != null)
			{
				return hqObj.State == typeof(HqObject2.HqObjectFlinga) || hqObj.State == typeof(HqObject2.HqObjectFlingaSelected);
			}
			return false;
		}
	}
}
