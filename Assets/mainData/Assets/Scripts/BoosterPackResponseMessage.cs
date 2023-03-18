using System.Collections;

public class BoosterPackResponseMessage : ShsEventMessage
{
	public bool success;

	public Hashtable payload;

	public bool IncludesSuperRare;

	public BoosterPackResponseMessage(bool success, Hashtable payload)
	{
		this.success = success;
		this.payload = payload;
		if (this.payload.ContainsKey("includes_super_rare"))
		{
			CspUtils.DebugLog("Open_booster_pack_response includes_super_rare = " + this.payload["includes_super_rare"]);
			IncludesSuperRare = bool.Parse(this.payload["includes_super_rare"].ToString());
		}
	}
}
