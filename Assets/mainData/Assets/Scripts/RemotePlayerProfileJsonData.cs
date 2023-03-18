using System.Collections.Generic;

public class RemotePlayerProfileJsonData
{
	public List<RemotePlayerProfileJsonHeroData> heroes;

	public int shield_play_allow;

	public string player_name;

	public int last_celebrated;

	public int current_challenge;

	public List<RemotePlayerProfileJsonCounterData> counters;

	public List<RemotePlayerProfileJsonServerCounterData> server_counters;

	public int title_id;

	public int medallion_id;

	public int sidekick_id;

	public int sidekick_tier;

	public int achievement_points;

	public string current_costume;
}
