public class GUIDragEndMessage : ShsEventMessage
{
	private DragDropInfo dragDropInfo;

	public DragDropInfo DragDropInfo
	{
		get
		{
			return dragDropInfo;
		}
	}

	public GUIDragEndMessage(DragDropInfo DragDropInfo)
	{
		dragDropInfo = DragDropInfo;
	}
}
