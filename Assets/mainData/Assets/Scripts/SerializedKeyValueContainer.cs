using System;
using System.Text.RegularExpressions;

public class SerializedKeyValueContainer
{
	public delegate void ContainerChanged();

	public delegate void ContainerInvoked();

	private string propString = string.Empty;

	private char delim = '|';

	private bool containerInvoked;

	public string PropertyString
	{
		get
		{
			return propString;
		}
		set
		{
			if (!containerInvoked)
			{
				propString = value;
				return;
			}
			throw new Exception("Property string not modifiable once the container has been initialized.");
		}
	}

	public string this[string key]
	{
		get
		{
			if (!containerInvoked)
			{
				if (this.OnContainerInvoked != null)
				{
					this.OnContainerInvoked();
				}
				containerInvoked = true;
			}
			if (ContainsKey(key))
			{
				Regex regex = new Regex(".*\\\\" + delim + key + "=(.*?)\\" + delim + ".*");
				Match match = regex.Match(propString);
				if (match.Groups.Count > 0)
				{
					return match.Groups[1].Value;
				}
			}
			return null;
		}
		set
		{
			if (!ContainsKey(key))
			{
				string text = propString;
				propString = text + delim + key + "=" + value + delim;
				return;
			}
			Regex regex = new Regex("(\\" + delim + key + "=.*?\\" + delim + ")");
			Match match = regex.Match(propString);
			if (match.Groups.Count > 0)
			{
				propString = regex.Replace(propString, delegate
				{
					return string.Empty;
				});
				string text = propString;
				propString = text + delim + key + "=" + value + delim;
				if (this.OnContainerChanged != null)
				{
					this.OnContainerChanged();
				}
			}
			else
			{
				CspUtils.DebugLog("Unable to find the key and value pair and replace it.");
			}
		}
	}

	public event ContainerChanged OnContainerChanged;

	public event ContainerInvoked OnContainerInvoked;

	public bool ContainsKey(string key)
	{
		if (!containerInvoked)
		{
			if (this.OnContainerInvoked != null)
			{
				this.OnContainerInvoked();
			}
			containerInvoked = true;
		}
		return propString != null && propString.Contains(delim + key);
	}

	public void Clear()
	{
		propString = string.Empty;
		if (this.OnContainerChanged != null)
		{
			this.OnContainerChanged();
		}
	}

	public override string ToString()
	{
		return propString;
	}
}
