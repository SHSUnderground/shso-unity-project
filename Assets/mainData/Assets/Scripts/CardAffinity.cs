using System.Collections.Generic;

public class CardAffinity : Dictionary<string, int>
{
	public void Add(string raw)
	{
		string[] array = raw.Split(':');
		int result;
		if (array.Length > 1 && int.TryParse(array[0], out result))
		{
			if (ContainsKey(array[1].Trim()))
			{
				CspUtils.DebugLog("Duplicate affinity key [" + array[1].Trim() + "] found");
			}
			else
			{
				Add(array[1].Trim(), result);
			}
		}
	}

	public void Add(KeyValuePair<string, int> kvp)
	{
		if (ContainsKey(kvp.Key))
		{
			CardAffinity cardAffinity;
			CardAffinity cardAffinity2 = cardAffinity = this;
			string key;
			string key2 = key = kvp.Key;
			int num = cardAffinity[key];
			cardAffinity2[key2] = num + kvp.Value;
		}
		else
		{
			Add(kvp.Key, kvp.Value);
		}
	}

	public void Add(CardAffinity newAff)
	{
		foreach (KeyValuePair<string, int> item in newAff)
		{
			Add(item);
		}
	}

	public override string ToString()
	{
		string text = string.Empty;
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, int> current = enumerator.Current;
				string text2 = text;
				text = text2 + current.Key + ":" + current.Value + "\n";
			}
			return text;
		}
	}
}
