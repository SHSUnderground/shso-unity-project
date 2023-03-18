public class ContentReference
{
	public ContentTypeEnum ContentType;

	public object ReferenceKey;

	public ContentReference(ContentTypeEnum ContentType, object ReferenceKey)
	{
		this.ContentType = ContentType;
		this.ReferenceKey = ReferenceKey;
	}
}
