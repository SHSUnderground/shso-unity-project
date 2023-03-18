using UnityEngine;

public class GUILayoutScrollWindow : GUIControlWindow
{
	public delegate void ChangedEventDelegate(GUIControl sender, GUIScrollChangedEvent eventData);

	private Vector2 cacheValue = Vector2.zero;

	protected Vector2 scrollPosition = Vector2.zero;

	public Vector2 ScrollPosition
	{
		get
		{
			return scrollPosition;
		}
		set
		{
			scrollPosition = value;
		}
	}

	public event ChangedEventDelegate Changed;

	public GUILayoutScrollWindow()
	{
		layoutType = LayoutTypeEnum.Flow;
		clippingEnabled = false;
	}

	public override void DrawPreprocess()
	{
		base.DrawPreprocess();
		GUILayout.BeginArea(base.rect);
		cacheValue = scrollPosition;
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(size.x), GUILayout.Height(size.y));
		if (cacheValue != scrollPosition && this.Changed != null)
		{
			this.Changed(this, new GUIScrollChangedEvent(cacheValue, scrollPosition));
		}
	}

	public override void DrawFinalize()
	{
		base.DrawFinalize();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}
}
