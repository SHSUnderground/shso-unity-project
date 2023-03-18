public class CardListFilter
{
	public enum FilterTypes
	{
		Factor,
		Block,
		Level,
		Owned,
		Search
	}

	public int numVal;

	public string strVal = string.Empty;

	public FilterTypes type;

	public CardListFilter(FilterTypes ft, string val)
	{
		type = ft;
		strVal = val;
	}

	public CardListFilter(FilterTypes ft, int val)
	{
		type = ft;
		numVal = val;
	}
}
