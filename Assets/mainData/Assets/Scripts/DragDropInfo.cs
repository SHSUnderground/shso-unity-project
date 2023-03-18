using UnityEngine;

public class DragDropInfo
{
	public enum CollectionType
	{
		Items,
		Heroes,
		None,
		Emote
	}

	public readonly DragDropSourceType SourceType;

	public DragDropSourceType TargetType;

	public readonly IGUIControl UISource;

	public readonly GameObject WorldSource;

	public IGUIControl UITarget;

	public GameObject WorldTarget;

	public string GameObjectSource;

	public string IconSource;

	public Vector2 IconSize;

	public CollectionType CollectionId;

	public string ItemId;

	public DragDropResult Result;

	public DragDropInfo(IGUIControl UISource)
		: this(DragDropSourceType.UI, UISource, null)
	{
	}

	public DragDropInfo(GameObject WorldSource)
		: this(DragDropSourceType.World, null, WorldSource)
	{
	}

	public DragDropInfo(DragDropSourceType SourceType, IGUIControl UISource, GameObject WorldSource)
	{
		this.SourceType = SourceType;
		this.UISource = UISource;
		this.WorldSource = WorldSource;
		IconSize = new Vector2(-1f, -1f);
	}
}
