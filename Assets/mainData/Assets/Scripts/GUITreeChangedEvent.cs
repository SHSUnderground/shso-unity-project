public class GUITreeChangedEvent : GUIEvent
{
	public string ChangedName;

	public string Path;

	public GUITreeChangedEvent()
	{
	}

	public GUITreeChangedEvent(string changed, string path)
	{
		Path = path;
		ChangedName = changed;
	}
}
