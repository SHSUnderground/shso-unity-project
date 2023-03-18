using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShsEventMgr
{
	public interface ICallback
	{
		object ReferencedFunction
		{
			get;
		}

		void Fire(ShsEventMessage msg);
	}

	protected class Callback<T> : ICallback where T : ShsEventMessage
	{
		public GenericDelegate<T> func;

		public object ReferencedFunction
		{
			get
			{
				return func.Target + ":  " + func.Method;
			}
		}

		public Callback(GenericDelegate<T> func)
		{
			this.func = func;
		}

		public void Fire(ShsEventMessage msg)
		{
			func(msg as T);
		}
	}

	protected struct CallbackData
	{
		public object obj;

		public Type type;

		public ICallback callback;
	}

	public class ObjectNameComparer : IComparer
	{
		int IComparer.Compare(object x, object y)
		{
			GameObject gameObject = x as GameObject;
			string text = (!(gameObject != null)) ? x.ToString() : gameObject.name;
			gameObject = (y as GameObject);
			string strB = (!(gameObject != null)) ? y.ToString() : gameObject.name;
			return text.CompareTo(strB);
		}
	}

	public delegate void GenericDelegate<T>(T arg) where T : ShsEventMessage;

	protected Dictionary<object, Dictionary<Type, List<ICallback>>> listeners;

	protected List<CallbackData> pendingDeletes;

	protected List<CallbackData> pendingListens;

	protected int nFire;

	public Dictionary<object, Dictionary<Type, List<ICallback>>> Listeners
	{
		get
		{
			return listeners;
		}
	}

	public ShsEventMgr()
	{
		listeners = new Dictionary<object, Dictionary<Type, List<ICallback>>>();
		pendingDeletes = new List<CallbackData>();
		pendingListens = new List<CallbackData>();
		nFire = 0;
	}

	public void AddListener<T>(GenericDelegate<T> func) where T : ShsEventMessage
	{
		AddListener(this, func);
	}

	public void AddListener<T>(object obj, GenericDelegate<T> func) where T : ShsEventMessage
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		if (func == null)
		{
			throw new ArgumentNullException("func");
		}
		CallbackData item = default(CallbackData);
		item.obj = obj;
		item.type = typeof(T);
		item.callback = new Callback<T>(func);
		pendingListens.Add(item);
		ProcessPendingListens();
	}

	public void RemoveListener<T>(GenericDelegate<T> func) where T : ShsEventMessage
	{
		RemoveListener(this, func);
	}

	public void RemoveListener<T>(object obj, GenericDelegate<T> func) where T : ShsEventMessage
	{
		if (obj == null)
		{
			CspUtils.DebugLog("Null object in RemoveListener");
			return;
		}
		Dictionary<Type, List<ICallback>> value = null;
		if (listeners.TryGetValue(obj, out value))
		{
			List<ICallback> value2 = null;
			if (value.TryGetValue(typeof(T), out value2))
			{
				for (int num = value2.Count - 1; num >= 0; num--)
				{
					Callback<T> callback = value2[num] as Callback<T>;
					if (callback != null && callback.func == func)
					{
						CallbackData item = default(CallbackData);
						item.obj = obj;
						item.type = typeof(T);
						item.callback = value2[num];
						pendingDeletes.Add(item);
					}
				}
			}
		}
		ProcessPendingDeletes();
	}

	public void Fire(object sender, ShsEventMessage msg)
	{
		if (msg.GetType() == typeof(OpenChatMessage)) 
		{
			if (sender != null)
				CspUtils.DebugLog("sender=" + sender.ToString());
			CspUtils.DebugLog("Calling 1st FireEventWithFilter...");
		}
		FireEventWithFilter(this, sender, msg);
		if (msg.GetType() == typeof(OpenChatMessage)) 
			CspUtils.DebugLog("Calling 2nd FireEventWithFilter...");
		FireEventWithFilter(sender, sender, msg);
		ProcessPendingDeletes();
		ProcessPendingListens();
	}

	protected void FireEventWithFilter(object filter, object sender, ShsEventMessage msg)
	{
		nFire++;
		try
		{
			if (filter != null)
			{
				Dictionary<Type, List<ICallback>> value = null;
				if (listeners.TryGetValue(filter, out value))
				{
					List<ICallback> value2 = null;
					if (value.TryGetValue(msg.GetType(), out value2))
					{
						int listCnt = 0;
						foreach (ICallback item in value2)
						{

							///////////////// test block by CSP ///////////////////
							listCnt++;
							GameObject gameObject = filter as GameObject;
							string str = (!(gameObject != null)) ? filter.ToString() : gameObject.name;
							//CspUtils.DebugLog("Filter Object:  " + str);
							//CspUtils.DebugLog("   --> " + msg.GetType().ToString() + " == " + item.ReferencedFunction.ToString());
				
							//CspUtils.DebugLog(listCnt + " FireEventWithFilter " + msg.GetType().ToString());
							//if (msg.GetType() == typeof(OpenChatMessage)) {
							//	DumpEventListeners();
							//}
							//if (msg.GetType() == typeof(CardGameEvent.ServerMessage)) {
							//	DumpEventListeners();
							//}
		
							///////////////////////////////////////

							try
							{
								item.Fire(msg);
							}
							catch (Exception message)
							{
								CspUtils.DebugLog(message);
							}
						}
					}
				}
			}
		}
		catch (Exception message2)
		{
			CspUtils.DebugLog(message2);
		}
		nFire--;
	}

	protected void ProcessPendingDeletes()
	{
		if (nFire <= 0 && pendingDeletes.Count > 0)
		{
			using (List<CallbackData>.Enumerator enumerator = pendingDeletes.GetEnumerator())
			{
				CallbackData data;
				while (enumerator.MoveNext())
				{
					data = enumerator.Current;
					Dictionary<Type, List<ICallback>> value = null;
					if (listeners.TryGetValue(data.obj, out value))
					{
						List<ICallback> value2 = null;
						if (value.TryGetValue(data.type, out value2))
						{
							value2.RemoveAll(delegate(ICallback a)
							{
								return a == data.callback;
							});
							if (value2.Count == 0)
							{
								value.Remove(data.type);
							}
						}
						if (value.Count == 0)
						{
							listeners.Remove(data.obj);
						}
					}
				}
			}
			pendingDeletes.Clear();
		}
	}

	protected void ProcessPendingListens()
	{
		if (nFire <= 0 && pendingListens.Count > 0)
		{
			foreach (CallbackData pendingListen in pendingListens)
			{
				Dictionary<Type, List<ICallback>> value = null;
				if (!listeners.TryGetValue(pendingListen.obj, out value))
				{
					value = new Dictionary<Type, List<ICallback>>();
					listeners.Add(pendingListen.obj, value);
				}
				List<ICallback> value2 = null;
				if (!value.TryGetValue(pendingListen.type, out value2))
				{
					value2 = new List<ICallback>();
					value.Add(pendingListen.type, value2);
				}
				if (pendingListen.type == typeof(OpenChatMessage)) {
						CspUtils.DebugLog("ProcessPendingListens OpenChatMessage " + pendingListens.Count);
				}
				value2.Add(pendingListen.callback);
			}
			pendingListens.Clear();
		}
	}

	public void DetectLeakedObjects()
	{
		foreach (object key in Listeners.Keys)
		{
			if (key is GameObject)
			{
				Dictionary<Type, List<ICallback>> dictionary = Listeners[key];
				GameObject gameObject = (GameObject)key;
				if (gameObject == null)
				{
					CspUtils.DebugLogError("Deleted game object found in event table: ");
				}
				else
				{
					CspUtils.DebugLogError("Active game object (" + gameObject.name + ") found in event table");
				}
				foreach (KeyValuePair<Type, List<ICallback>> item in dictionary)
				{
					foreach (ICallback item2 in item.Value)
					{
						CspUtils.DebugLogError("--> " + item.Key.ToString() + " : " + item2.ReferencedFunction.ToString());
					}
				}
			}
		}
	}

	public void DumpEventListeners()
	{
		object[] array = new object[Listeners.Count];
		Listeners.Keys.CopyTo(array, 0);
		Array.Sort(array, new ObjectNameComparer());
		object[] array2 = array;
		foreach (object obj in array2)
		{
			Dictionary<Type, List<ICallback>> dictionary = Listeners[obj];
			Type[] array3 = new Type[dictionary.Count];
			dictionary.Keys.CopyTo(array3, 0);
			Array.Sort(array3, new ObjectNameComparer());
			GameObject gameObject = obj as GameObject;
			string str = (!(gameObject != null)) ? obj.ToString() : gameObject.name;
			CspUtils.DebugLog("Event Object:  " + str);
			Type[] array4 = array3;
			foreach (Type type in array4)
			{
				foreach (ICallback item in dictionary[type])
				{
					CspUtils.DebugLog("   --> " + type.ToString() + " == " + item.ReferencedFunction.ToString());
				}
			}
		}
	}
}
