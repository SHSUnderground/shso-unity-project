using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class AnimTagManager : Singleton<AnimTagManager>
{
	private struct TagHandler
	{
		public string Target;

		public AnimTagAttribute.IParser Parser;

		public static TagHandler Make(string target, AnimTagAttribute.IParser parser)
		{
			TagHandler result = default(TagHandler);
			result.Target = target;
			result.Parser = parser;
			return result;
		}
	}

	private Dictionary<string, List<TagHandler>> Handlers = new Dictionary<string, List<TagHandler>>();

	public AnimTagManager()
	{
		CollectHandlers();
	}

	public static KeyValuePair<string, string>[] GetBindings(string tag, string data)
	{
		//Discarded unreachable code: IL_002a
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		List<TagHandler> list2;
		try
		{
			list2 = Singleton<AnimTagManager>.instance.Handlers[tag];
		}
		catch
		{
			return list.ToArray();
		}
		foreach (TagHandler item in list2)
		{
			list.Add(new KeyValuePair<string, string>(item.Target, item.Parser.Parse(data)));
		}
		return list.ToArray();
	}

	private void CollectHandlers()
	{
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		Type[] types = executingAssembly.GetTypes();
		foreach (Type type in types)
		{
			if (!type.IsSubclassOf(typeof(MonoBehaviour)))
			{
				continue;
			}
			MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo methodInfo in methods)
			{
				object[] customAttributes = methodInfo.GetCustomAttributes(typeof(AnimTagAttribute), false);
				object[] array = customAttributes;
				foreach (object obj in array)
				{
					AnimTagAttribute animTagAttribute = obj as AnimTagAttribute;
					if (!Array.Exists(animTagAttribute.Parser.GetInterfaces(), delegate(Type t)
					{
						return t == typeof(AnimTagAttribute.IParser);
					}))
					{
						CspUtils.DebugLog(string.Format("Unable to register animation tag handler {0}->{1}.  {2} does not implement AnimTagAttribute.IParser.", animTagAttribute.Tag, methodInfo.Name, animTagAttribute.Parser.ToString()));
						return;
					}
					AnimTagAttribute.IParser parser = animTagAttribute.Parser.GetConstructor(Type.EmptyTypes).Invoke(null) as AnimTagAttribute.IParser;
					Register(animTagAttribute.Tag, methodInfo.Name, parser);
				}
			}
		}
	}

	private void Register(string tag, string target, AnimTagAttribute.IParser parser)
	{
		TagHandler item = TagHandler.Make(target, parser);
		List<TagHandler> value = null;
		if (!Handlers.TryGetValue(tag, out value))
		{
			value = new List<TagHandler>();
			Handlers.Add(tag, value);
		}
		foreach (TagHandler item2 in value)
		{
			if (item2.Target == target)
			{
				if (item2.Parser.GetType() != parser.GetType())
				{
					CspUtils.DebugLog(string.Format("AnimTag {0} handler {1} has multiple parser definitons ({2} and {3})", tag, target, item2.Parser.GetType().ToString(), parser.GetType().ToString()));
				}
				return;
			}
		}
		value.Add(item);
	}
}
