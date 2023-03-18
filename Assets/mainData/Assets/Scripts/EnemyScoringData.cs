public class EnemyScoringData
{
	protected int hitScore = 10;

	protected int defeat = 1;

	public int HitScore
	{
		get
		{
			return hitScore;
		}
	}

	public int Defeat
	{
		get
		{
			return defeat;
		}
	}

	public EnemyScoringData()
	{
	}

	public EnemyScoringData(DataWarehouse data)
	{
		InitializeFromData(data);
	}

	public void InitializeFromData(DataWarehouse data)
	{
		hitScore = data.TryGetInt("./hit", 10);
		if (data.GetCount("./defeat") != 0)
		{
			defeat = data.GetInt("./defeat");
		}
	}
}
