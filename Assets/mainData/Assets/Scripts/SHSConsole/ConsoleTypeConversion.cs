using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

namespace SHSConsole
{
	public class ConsoleTypeConversion
	{
		private static ConsoleTypeConversion inst;

		private Dictionary<Type, MethodInfo> methods = new Dictionary<Type, MethodInfo>();

		public static ConsoleTypeConversion Inst
		{
			get
			{
				if (inst == null)
				{
					inst = new ConsoleTypeConversion();
				}
				return inst;
			}
		}

		private ConsoleTypeConversion()
		{
			MethodInfo[] array = typeof(ConsoleTypeConversion).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo[] array2 = array;
			foreach (MethodInfo methodInfo in array2)
			{
				if (methodInfo.Name != "convert")
				{
					continue;
				}
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (parameters.Length == 3)
				{
					ParameterInfo parameterInfo = parameters[2];
					if (parameterInfo.ParameterType.IsByRef)
					{
						methods[parameterInfo.ParameterType.GetElementType()] = methodInfo;
					}
				}
			}
		}

		private bool convert(object[] Params, ref int ParamIndex, out bool boolVal)
		{
			if (ParamIndex < Params.Length && Params[ParamIndex] is string)
			{
				if (string.Compare("1", (string)Params[ParamIndex], true) == 0 || string.Compare("yes", (string)Params[ParamIndex], true) == 0 || string.Compare("on", (string)Params[ParamIndex], true) == 0 || string.Compare("true", (string)Params[ParamIndex], true) == 0)
				{
					boolVal = true;
					ParamIndex++;
					return true;
				}
				if (string.Compare("0", (string)Params[ParamIndex], true) == 0 || string.Compare("no", (string)Params[ParamIndex], true) == 0 || string.Compare("off", (string)Params[ParamIndex], true) == 0 || string.Compare("false", (string)Params[ParamIndex], true) == 0)
				{
					boolVal = false;
					ParamIndex++;
					return true;
				}
			}
			boolVal = false;
			return false;
		}

		private bool convert(object[] Params, ref int ParamIndex, out Vector2 pt)
		{
			object[] array = null;
			int num = 0;
			if (Params[ParamIndex] is object[] && ((object[])Params[ParamIndex]).Length == 2 && ((object[])Params[ParamIndex])[0] is string && ((object[])Params[ParamIndex])[1] is string)
			{
				array = (object[])Params[ParamIndex];
				num = 1;
			}
			else if (ParamIndex < Params.Length - 1 && Params[ParamIndex] is string && Params[ParamIndex + 1] is string)
			{
				array = new object[2];
				Array.Copy(Params, ParamIndex, array, 0, 2);
				num = 2;
			}
			if (array != null)
			{
				float x = float.Parse((string)array[0]);
				float y = float.Parse((string)array[1]);
				pt = new Vector2(x, y);
				ParamIndex += num;
				return true;
			}
			pt = Vector2.zero;
			return false;
		}

		public bool CanConvertTo(Type type)
		{
			if (type.IsArray)
			{
				type = type.GetElementType();
			}
			TypeConverter converter = TypeDescriptor.GetConverter(type);
			if (converter.CanConvertFrom(typeof(string)))
			{
				return true;
			}
			MethodInfo value = null;
			if (methods.TryGetValue(type, out value))
			{
				return true;
			}
			return false;
		}

		public bool ConvertToType(object[] Params, ref int ParamIndex, Type toType, out object ConvertedParam)
		{
			//Discarded unreachable code: IL_0045
			ConvertedParam = null;
			if (ParamIndex >= Params.Length)
			{
				return false;
			}
			if (Params[ParamIndex] is string)
			{
				try
				{
					TypeConverter converter = TypeDescriptor.GetConverter(toType);
					ConvertedParam = converter.ConvertFromString((string)Params[ParamIndex]);
					ParamIndex++;
					return true;
				}
				catch (Exception)
				{
				}
			}
			if (toType.IsArray && Params[ParamIndex] is object[])
			{
				object[] array = (object[])Params[ParamIndex];
				object[] array2 = (object[])Array.CreateInstance(toType.GetElementType(), array.Length);
				int ParamIndex2 = 0;
				while (ParamIndex2 < array.Length)
				{
					int num = ParamIndex2;
					object ConvertedParam2 = null;
					if (!ConvertToType(array, ref ParamIndex2, toType.GetElementType(), out ConvertedParam2))
					{
						break;
					}
					array2[num] = ConvertedParam2;
				}
				if (ParamIndex2 == array.Length)
				{
					ConvertedParam = array2;
					ParamIndex++;
					return true;
				}
			}
			try
			{
				MethodInfo value = null;
				if (methods.TryGetValue(toType, out value))
				{
					ConvertedParam = null;
					object[] array3 = new object[3]
					{
						Params,
						ParamIndex,
						ConvertedParam
					};
					if ((bool)value.Invoke(this, array3))
					{
						ParamIndex = (int)array3[1];
						ConvertedParam = array3[2];
						return true;
					}
				}
			}
			catch (Exception)
			{
			}
			return false;
		}
	}
}
