public class WaitingForEffectsCallback
{
	public EffectSequenceList.EffectsLoadedCallback Callback;

	public object ExtraData;

	public WaitingForEffectsCallback(EffectSequenceList.EffectsLoadedCallback cb, object ed)
	{
		Callback = cb;
		ExtraData = ed;
	}
}
