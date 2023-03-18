using UnityEngine;

public class Zone
{
	public string zone;

	public Vector2 nameSize;

	public string launchKey;

	public ContentReference contentReference;

	public Zone(string zone, Vector2 nameSize, string launchKey, ContentReference contentReference)
	{
		this.zone = zone;
		this.nameSize = nameSize;
		this.launchKey = launchKey;
		this.contentReference = contentReference;
	}
}
