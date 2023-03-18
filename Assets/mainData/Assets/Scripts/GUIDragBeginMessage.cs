public class GUIDragBeginMessage : ShsEventMessage
{
	private DragDropInfo dragDropInfo;

	public DragDropInfo DragDropInfo
	{
		get
		{
			return dragDropInfo;
		}
	}

	public GUIDragBeginMessage(DragDropInfo DragDropInfo)
	{
		dragDropInfo = DragDropInfo;
	}
}
