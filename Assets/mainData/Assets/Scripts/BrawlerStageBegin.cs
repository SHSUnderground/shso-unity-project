public class BrawlerStageBegin : ShsEventMessage
{
	public int whatStage;

	public BrawlerStageBegin(int whatStage)
	{
		this.whatStage = whatStage;
	}
}
