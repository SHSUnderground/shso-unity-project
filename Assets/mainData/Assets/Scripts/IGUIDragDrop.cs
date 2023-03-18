public interface IGUIDragDrop
{
	bool CanDrag
	{
		get;
	}

	bool CanDrop(DragDropInfo DragDropInfo);

	void SetDragInfo(DragDropInfo DragDropInfo);

	void OnDragBegin(DragDropInfo DragDropInfo);

	void OnDragEnd(DragDropInfo DragDropInfo);

	void OnDragOver(DragDropInfo DragOverInfo);
}
