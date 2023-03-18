using System;

[AttributeUsage(AttributeTargets.Method)]
public class AnimTagAttribute : Attribute
{
	public interface IParser
	{
		string Parse(string data);
	}

	public class DefaultParser : IParser
	{
		public string Parse(string data)
		{
			return data.ToLower();
		}
	}

	public string Tag;

	public Type Parser;

	public AnimTagAttribute(string tag)
	{
		Tag = tag;
		Parser = typeof(DefaultParser);
	}

	public AnimTagAttribute(string tag, Type parserType)
	{
		Tag = tag;
		Parser = parserType;
	}
}
