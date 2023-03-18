using VO;

public interface IResolvedVOActionHandler
{
	void HandleResolvedVOAction(ResolvedVOAction vo, OnVOActionFinishedDelegate onFinished);

	void CancelVOAction(ResolvedVOAction vo);
}
