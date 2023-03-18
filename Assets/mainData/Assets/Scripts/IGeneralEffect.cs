public interface IGeneralEffect
{
	float ChanceToPlay
	{
		get;
	}

	bool IsFinished();

	bool IsLooping();
}
