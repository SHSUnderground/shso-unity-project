public class MissionRewardChunk
{
	protected int coins;

	protected int tickets;

	protected int xp;

	protected string ownableId = string.Empty;

	public int Coins
	{
		get
		{
			return coins;
		}
	}

	public int Tickets
	{
		get
		{
			return tickets;
		}
	}

	public int Xp
	{
		get
		{
			return xp;
		}
	}

	public string OwnableID
	{
		get
		{
			return ownableId;
		}
	}

	public MissionRewardChunk(int rewardCoins, int rewardTickets, int rewardXP, string rewardOwnableId)
	{
		coins = rewardCoins;
		tickets = rewardTickets;
		xp = rewardXP;
		ownableId = rewardOwnableId;
	}

	public MissionRewardChunk(int rewardCoins, int rewardTickets, int rewardXP)
		: this(rewardCoins, rewardTickets, rewardXP, string.Empty)
	{
	}

	public MissionRewardChunk(DataWarehouse data)
		: this(data.GetInt("coins"), data.GetInt("tickets"), data.GetInt("xp"), data.TryGetString("ownable", string.Empty))
	{
	}

	public override string ToString()
	{
		return string.Format("tickets{0}, coins:{1}, xp{2}.", coins, tickets, xp);
	}
}
