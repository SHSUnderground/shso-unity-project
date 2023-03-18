using UnityEngine;

public class Branch
{
	public Rect rc;

	public Vector2 sz;

	public string Text;

	public IGUIContainer Tag;

	public static int spanY = 75;

	public static int marginX = 10;

	public static int marginY = 55;

	public float delta;

	public float kidsW;

	public float prevW;

	public Vector2 Location = new Vector2(0f, 0f);

	public Rect wide
	{
		get
		{
			return new Rect(Location.x, Location.y, sz.x + delta, 5f);
		}
	}

	public Rect rail
	{
		get
		{
			float num = sz.x + delta;
			return new Rect(Location.x + (num - sz.x) / 2f - 1f, Location.y + (float)marginY, sz.x + 1f, sz.y);
		}
	}

	public Vector2 topJoin
	{
		get
		{
			Rect rail = this.rail;
			return new Vector2(rail.xMin + rail.width / 2f, rail.yMin);
		}
	}

	public Vector2 bottomJoin
	{
		get
		{
			Rect rail = this.rail;
			return new Vector2(rail.xMin + rail.width / 2f, rail.yMax);
		}
	}

	public Branch(IGUIContainer tag)
	{
		Tag = tag;
	}
}
