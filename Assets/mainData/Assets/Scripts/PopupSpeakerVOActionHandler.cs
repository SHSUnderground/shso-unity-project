using VO;

public class PopupSpeakerVOActionHandler : IResolvedVOActionHandler
{
	public delegate IVOSpeakerWindow VOWindowFactory();

	private IResolvedVOActionHandler baseHandler;

	private OnVOActionFinishedDelegate onFinished;

	private IVOSpeakerWindow speakerWindow;

	private int charIdInputIndex;

	private VOWindowFactory speakerWindowFactory;

	public PopupSpeakerVOActionHandler(int charIdInputIndex, VOWindowFactory windowFactory)
	{
		this.charIdInputIndex = charIdInputIndex;
		speakerWindowFactory = windowFactory;
	}

	public void HandleResolvedVOAction(ResolvedVOAction vo, OnVOActionFinishedDelegate onFinished)
	{
		HighlightCharacter(vo);
		baseHandler = VOManager.Instance.GetLocaleSpecificVOActionHandler();
		this.onFinished = onFinished;
		baseHandler.HandleResolvedVOAction(vo, OnBaseHandlerFinished);
	}

	public void CancelVOAction(ResolvedVOAction vo)
	{
		if (baseHandler != null)
		{
			baseHandler.CancelVOAction(vo);
		}
	}

	private void OnBaseHandlerFinished()
	{
		UnhighlightCharacter();
		if (onFinished != null)
		{
			onFinished();
		}
	}

	private void HighlightCharacter(ResolvedVOAction vo)
	{
		if (speakerWindowFactory != null)
		{
			speakerWindow = speakerWindowFactory();
			if (speakerWindow != null)
			{
				speakerWindow.SetVO(vo);
				speakerWindow.SetCharacter(vo.ResolvedInputs[charIdInputIndex]);
				speakerWindow.SetText(vo.TextID);
				speakerWindow.AnimateIn();
			}
		}
	}

	private void UnhighlightCharacter()
	{
		if (speakerWindow != null)
		{
			speakerWindow.AnimateOut();
		}
	}
}
