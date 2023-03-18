using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace SHSConsole
{
	internal class Console : IDisposable
	{
		private ConsoleCommands commandObj;

		private static Console inst;

		private Dictionary<string, List<MethodInfo>> consoleMethods;

		private bool disposed;

		public Dictionary<string, List<MethodInfo>> ConsoleMethods
		{
			get
			{
				return consoleMethods;
			}
		}

		public static Console Inst
		{
			get
			{
				if (inst == null)
				{
					inst = new Console();
				}
				return inst;
			}
		}

		private Console()
		{
			commandObj = new ConsoleCommands();
			consoleMethods = new Dictionary<string, List<MethodInfo>>();
			buildConsoleMethodDictionary();
		}

		[DllImport("user32.dll")]
		private static extern int MessageBox(IntPtr hWnd, string text, string caption, int type);

		public string ExecuteCommand(string Cmd)
		{
			//Discarded unreachable code: IL_000d, IL_0029
			try
			{
				return execute(Cmd);
			}
			catch (ConsoleCommandException ex)
			{
				return "ERROR: " + ex.Message;
			}
		}

		~Console()
		{
			dispose(false);
		}

		public void Dispose()
		{
			dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
				}
				disposed = true;
			}
		}

		private string execute(string Command)
		{
			string empty = string.Empty;
			object[] array = splitParams(Command);
			if (array.Length == 0)
			{
				return string.Empty;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] is ConsoleCommandWrapper)
				{
					array[i] = execute(((ConsoleCommandWrapper)array[i]).Command);
				}
			}
			if (!(array[0] is string))
			{
				throw new ConsoleCommandException("Unknown command \"" + array[0].ToString() + "\"");
			}
			string text = ((string)array[0]).ToLower();
			if ((text == "help" || text == "?") && array.Length == 1)
			{
				return buildHelpString();
			}
			IList list = null;
			List<MethodInfo> value = null;
			if (consoleMethods.TryGetValue(text, out value))
			{
				list = consoleMethods[text];
			}
			else
			{
				string[] array2 = text.Split('.');
				if (array2.Length == 2)
				{
					list = findStaticMethods(array2[0], array2[1]);
				}
			}
			if (list != null && list.Count > 0)
			{
				object[] Parameters = null;
				foreach (MethodInfo item in list)
				{
					if (parametersMatch(item.GetParameters(), array, out Parameters))
					{
						return executeCmd(item, Parameters);
					}
				}
				if (array.Length > 1 && array[1] is object[])
				{
					object[] array3 = new object[((object[])array[1]).Length + 1];
					array3[0] = array[0];
					Array.Copy((object[])array[1], 0, array3, 1, ((object[])array[1]).Length);
					foreach (MethodInfo item2 in list)
					{
						if (parametersMatch(item2.GetParameters(), array3, out Parameters))
						{
							return executeCmd(item2, Parameters);
						}
					}
				}
				empty = "Could not find matching parameter list for command \"" + array[0] + "\" with parameters [";
				for (int j = 1; j < array.Length; j++)
				{
					if (j > 1)
					{
						empty += " ";
					}
					empty += array[j].ToString();
				}
				empty += "].";
				throw new ConsoleCommandException(empty);
			}
			throw new ConsoleCommandException("Unknown command \"" + array[0] + "\"");
		}

		private void buildConsoleMethodDictionary()
		{
			List<string> list = new List<string>();
			MethodInfo[] methods = typeof(ConsoleCommands).GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
			MethodInfo[] array = methods;
			foreach (MethodInfo methodInfo in array)
			{
				if (methodInfo.Name == "ToString" || methodInfo.Name == "Equals" || methodInfo.Name == "GetHashCode" || methodInfo.Name == "GetType")
				{
					continue;
				}
				ParameterInfo[] parameters = methodInfo.GetParameters();
				ParameterInfo[] array2 = parameters;
				foreach (ParameterInfo parameterInfo in array2)
				{
					if (!ConsoleTypeConversion.Inst.CanConvertTo(parameterInfo.ParameterType))
					{
						list.Add(methodInfo.Name);
					}
				}
				List<MethodInfo> value = null;
				if (!consoleMethods.TryGetValue(methodInfo.Name.ToLower(), out value))
				{
					consoleMethods.Add(methodInfo.Name.ToLower(), new List<MethodInfo>());
				}
				consoleMethods[methodInfo.Name.ToLower()].Add(methodInfo);
			}
			if (list.Count > 0)
			{
				string text = "WARNING!\n\nThe following console methods contain parameters with no supported conversion:\n\n";
				foreach (string item in list)
				{
					text = text + item + "\n";
				}
				if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
				{
					MessageBox(IntPtr.Zero, text, "Warning", 0);
				}
			}
		}

		private MethodInfo[] findStaticMethods(string ClassName, string MethodName)
		{
			List<MethodInfo> list = new List<MethodInfo>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			foreach (Assembly assembly in array)
			{
				Type[] types = assembly.GetTypes();
				Type[] array2 = types;
				foreach (Type type in array2)
				{
					if (string.Compare(type.Name, ClassName, true) != 0)
					{
						continue;
					}
					MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					MethodInfo[] array3 = methods;
					foreach (MethodInfo methodInfo in array3)
					{
						if (string.Compare(methodInfo.Name, MethodName, true) == 0)
						{
							list.Add(methodInfo);
						}
					}
					break;
				}
			}
			return list.ToArray();
		}

		private object[] splitParams(string ParamString)
		{
			ParamString = ParamString.Trim();
			List<object> list = new List<object>();
			while (ParamString.Length > 0)
			{
				list.Add(parseCommandParameter(ref ParamString));
			}
			return list.ToArray();
		}

		private object parseCommandParameter(ref string Params)
		{
			Params = Params.Trim();
			if (Params[0] == '"')
			{
				int num = Params.IndexOf('"', 1);
				while (num > 0 && Params[num - 1] == '\\')
				{
					num = Params.IndexOf('"', num + 1);
				}
				if (num > 0)
				{
					string text = Params.Substring(1, num - 1);
					Params = Params.Substring(num + 1).Trim();
					return text.Replace("\\\"", "\"");
				}
			}
			if (Params[0] == '[' || Params[0] == '{')
			{
				char c = '[';
				char c2 = ']';
				if (Params[0] == '{')
				{
					c = '{';
					c2 = '}';
				}
				int num2 = 1;
				int num3 = 1;
				while (num2 > 0 && num3 < Params.Length)
				{
					if (Params[num3] == c2)
					{
						num2--;
					}
					else if (Params[num3] == c)
					{
						num2++;
					}
					num3++;
				}
				if (num2 == 0)
				{
					object result = new ConsoleCommandWrapper(Params.Substring(1, num3 - 2).Trim());
					Params = Params.Substring(num3).Trim();
					return result;
				}
			}
			if (Params[0] == '(')
			{
				char value = ')';
				int num4 = Params.IndexOf(value);
				if (num4 > 0)
				{
					string paramString = Params.Substring(1, num4 - 1);
					Params = Params.Substring(num4 + 1).Trim();
					return splitParams(paramString);
				}
			}
			string result2 = Params;
			int num5 = Params.IndexOf(' ');
			if (num5 > 0)
			{
				result2 = Params.Substring(0, num5);
				Params = Params.Substring(num5 + 1).Trim();
				return result2;
			}
			Params = string.Empty;
			return result2;
		}

		private bool parseParameterType(Type ParamType, object[] CmdParams, ref int CmdParamIndex, out object Param)
		{
			int num = CmdParamIndex;
			bool flag = ConsoleTypeConversion.Inst.ConvertToType(CmdParams, ref CmdParamIndex, ParamType, out Param);
			if (flag)
			{
			}
			return flag;
		}

		private bool parametersMatch(ParameterInfo[] pi, object[] CmdParams, out object[] Parameters)
		{
			Parameters = null;
			List<object> list = new List<object>();
			int CmdParamIndex = 1;
			int num = 0;
			for (num = 0; num < pi.Length; num++)
			{
				object Param = null;
				if (!parseParameterType(pi[num].ParameterType, CmdParams, ref CmdParamIndex, out Param))
				{
					break;
				}
				list.Add(Param);
			}
			if (num < pi.Length)
			{
				return false;
			}
			if (CmdParamIndex < CmdParams.Length)
			{
				return false;
			}
			Parameters = list.ToArray();
			return true;
		}

		private string executeCmd(MethodInfo mi, object[] Parameters)
		{
			object obj = mi.Invoke(commandObj, Parameters);
			return (obj != null) ? obj.ToString() : string.Empty;
		}

		private string buildHelpString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("-----------------------------------------------------------------------------------------");
			List<MethodInfo>[] array = new List<MethodInfo>[consoleMethods.Count];
			consoleMethods.Values.CopyTo(array, 0);
			Array.Sort(array, delegate(List<MethodInfo> MethodList1, List<MethodInfo> MethodList2)
			{
				return (MethodList1.Count > 0 && MethodList2.Count > 0) ? string.Compare(MethodList1[0].Name, MethodList2[0].Name) : 0;
			});
			List<MethodInfo>[] array2 = array;
			foreach (List<MethodInfo> list in array2)
			{
				foreach (MethodInfo item in list)
				{
					ParameterInfo[] parameters = item.GetParameters();
					stringBuilder.Append("> " + item.Name);
					if (parameters.Length == 0)
					{
						stringBuilder.Append("()");
					}
					else
					{
						stringBuilder.Append(" (");
						bool flag = true;
						ParameterInfo[] array3 = parameters;
						foreach (ParameterInfo parameterInfo in array3)
						{
							if (!flag)
							{
								stringBuilder.Append(", ");
							}
							stringBuilder.Append(parameterInfo.ParameterType.Name + " " + parameterInfo.Name);
							flag = false;
						}
						stringBuilder.Append(")");
					}
					stringBuilder.AppendLine(string.Empty);
					DescriptionAttribute[] array4 = (DescriptionAttribute[])item.GetCustomAttributes(typeof(DescriptionAttribute), false);
					if (array4.Length > 0)
					{
						stringBuilder.AppendLine(array4[0].Description);
					}
				}
			}
			stringBuilder.AppendLine("-----------------------------------------------------------------------------------------");
			return stringBuilder.ToString();
		}
	}
}
