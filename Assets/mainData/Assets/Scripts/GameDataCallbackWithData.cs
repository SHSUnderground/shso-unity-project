public class GameDataCallbackWithData
{
	public GameDataManager.GameDataLoadedCallback Callback;

	public object ExtraData;

	public GameDataCallbackWithData(GameDataManager.GameDataLoadedCallback cb, object ed)
	{
		Callback = cb;
		ExtraData = ed;
	}
}
