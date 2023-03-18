using System.Collections.Generic;

public class DebugOptions
{
	private Dictionary<string, object> optionsList;

	public List<string> SettingsNameList
	{
		get
		{
			string[] array = new string[optionsList.Count];
			optionsList.Keys.CopyTo(array, 0);
			return new List<string>(array);
		}
	}

	public DebugOptions()
	{
		optionsList = new Dictionary<string, object>();
	}

	public void AddSetting(string setting, object value)
	{
		optionsList[setting] = value;
	}

	public void SetSetting(string setting, object value)
	{
		if (optionsList.ContainsKey(setting))
		{
			optionsList[setting] = value;
		}
	}

	public object GetSetting(string setting)
	{
		return optionsList[setting];
	}

	public T GetSetting<T>(string setting) where T : class
	{
		return optionsList[setting] as T;
	}

	public bool GetSettingAsBool(string setting)
	{
		return (bool)optionsList[setting];
	}

	public string GetSettingAsString(string setting)
	{
		return (string)optionsList[setting];
	}
}
