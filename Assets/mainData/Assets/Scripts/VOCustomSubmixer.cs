using System.Collections.Generic;

public class VOCustomSubmixer : IVOMixer
{
	protected IVOMixer output;

	protected Dictionary<string, IVOMixer> namedSubmixers = new Dictionary<string, IVOMixer>();

	public IVOMixer Output
	{
		get
		{
			return output;
		}
	}

	public IVOMixer this[string name]
	{
		get
		{
			return GetNamedSubmixer(name);
		}
		set
		{
			AddNamedSubmixer(name, value);
		}
	}

	public void Update()
	{
		foreach (KeyValuePair<string, IVOMixer> namedSubmixer in namedSubmixers)
		{
			if (namedSubmixer.Value != null)
			{
				namedSubmixer.Value.Update();
			}
		}
	}

	public void SetOutput(IVOMixer output)
	{
		this.output = output;
	}

	public void SendVO(IVOMixerItem item)
	{
		IVOMixer iVOMixer = GetNamedSubmixer(item.Action.CustomSubmixerName) ?? output;
		if (iVOMixer != null)
		{
			iVOMixer.SendVO(item);
		}
	}

	public void InitializeGlobalCustomSubmixers()
	{
		VOGlobalCustomSubmixers.AddGlobalCustomSubmixers(namedSubmixers, Output);
	}

	public void AddNamedSubmixer(string name, IVOMixer submixer)
	{
		namedSubmixers[name] = submixer;
	}

	public bool RemoveNamedSubmixer(string name)
	{
		return namedSubmixers.Remove(name);
	}

	public IVOMixer GetNamedSubmixer(string name)
	{
		IVOMixer value = null;
		if (!string.IsNullOrEmpty(name))
		{
			namedSubmixers.TryGetValue(name, out value);
		}
		return value;
	}

	public void ClearAllSubmixers()
	{
		namedSubmixers.Clear();
	}
}
