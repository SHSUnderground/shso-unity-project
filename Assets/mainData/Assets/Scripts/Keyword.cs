using System.Collections.Generic;

public class Keyword
{
	public string keyword;

	public string readable_name;

	public string icon = string.Empty;

	public string tooltip = string.Empty;

	private Dictionary<string, string> contexts = new Dictionary<string, string>();

	public static Dictionary<string, Keyword> keywordsByName = new Dictionary<string, Keyword>();

	public Keyword(KeywordJson data)
	{
		keyword = data.keyword;
		readable_name = data.keyword_readable;
		icon = string.Empty + data.icon;
		tooltip = string.Empty + data.tooltip;
		if (data.keyword_context != null && data.keyword_context.Length > 0)
		{
			string[] array = data.keyword_context.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				contexts.Add(text, text);
			}
		}
		keywordsByName.Add(keyword, this);
	}

	public bool hasContext(string context)
	{
		return contexts.ContainsKey(context);
	}
}
