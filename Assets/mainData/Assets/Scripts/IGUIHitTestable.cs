using UnityEngine;

public interface IGUIHitTestable : IGUINamed
{
	GUIControl.HitTestTypeEnum HitTestType
	{
		get;
		set;
	}

	GUIControl.BlockTestTypeEnum BlockTestType
	{
		get;
		set;
	}

	bool HitTest(Vector2 point);

	bool BlockTest(Vector2 point);
}
