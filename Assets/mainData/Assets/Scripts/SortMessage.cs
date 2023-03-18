internal class SortMessage : ShsEventMessage
{
	public enum SortDirectionEnum
	{
		Ascending,
		Descending
	}

	public readonly string SortString;

	public readonly SortDirectionEnum Direction;

	public SortMessage(string SortString, SortDirectionEnum Direction)
	{
		this.SortString = SortString;
		this.Direction = Direction;
	}
}
