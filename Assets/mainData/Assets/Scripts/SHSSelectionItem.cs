using UnityEngine;

public abstract class SHSSelectionItem<C> where C : GUIControl
{
	public enum SelectionState
	{
		Active,
		Passive,
		Selected,
		Special,
		Subscription,
		Highlighted
	}

	public bool active = true;

	public C item;

	public Vector2 itemSize;

	public bool OddInSequence;

	public bool RequestRefresh;

	public SelectionState currentState;
}
