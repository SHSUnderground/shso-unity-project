using System.Collections.Generic;
using System.Xml.XPath;
using UnityEngine;
using System;

public abstract class ShsCollectionBase<T> : Dictionary<string, T> where T : ShsCollectionItem, new()
{
	protected string collectionElementName;

	protected string keyName;

	protected bool subscriberOnly;

	public new virtual T this[string key]
	{
		get
		{
			if (base.ContainsKey(key))
			{
				return base[key];
			}
			return (T)null;
		}
		set
		{
			bool flag = !ContainsKey(key);
			base[key] = value;
			if (flag)
			{
				AddedItem(key);
			}
		}
	}

	public ShsCollectionBase()
	{
	}

	public ShsCollectionBase(int capacity)
		: base(capacity)
	{
	}

	protected virtual string GetKeyFromItemData(DataWarehouse itemData)
	{
		return itemData.GetString(keyName);
	}

	protected virtual void InitializeFromData(DataWarehouse data)
	{
		CspUtils.DebugLog("collectionElementName=" + collectionElementName);
		XPathNodeIterator values = data.GetValues(collectionElementName);
		
		foreach (XPathNavigator item in Utils.Enumerate(values))
		{
			DataWarehouse data2 = new DataWarehouse(item);
			T value = new T();
			if (value.InitializeFromData(data2))
			{
				Add(value.GetKey(), value);
			}
			else
			{
				CspUtils.DebugLog("Item initialization failed");
			}
		}
	}

	public virtual void UpdateItemsFromData(DataWarehouse data)
	{
		XPathNodeIterator values = data.GetValues(collectionElementName);
		int count = values.Count;
		if (count <= 0)
		{
			CspUtils.DebugLog("Asked to update " + collectionElementName + " but got <" + count + "> data elements.");
		}
		foreach (XPathNavigator item in Utils.Enumerate(values))
		{
			DataWarehouse dataWarehouse = new DataWarehouse(item);
			string keyFromItemData = GetKeyFromItemData(dataWarehouse);
			T value = (T)null;
			if (TryGetValue(keyFromItemData, out value))
			{
				value.UpdateFromData(dataWarehouse);
			}
			else
			{
				value = new T();
				value.InitializeFromData(dataWarehouse);
				Add(value.GetKey(), value);
			}
		}
	}

	public virtual void UpdateItemFromData(DataWarehouse itemData)
	{
		string keyFromItemData = GetKeyFromItemData(itemData);
		T value = (T)null;
		if (TryGetValue(keyFromItemData, out value))
		{
			value.UpdateFromData(itemData);
			return;
		}
		value = new T();
		value.InitializeFromData(itemData);
		Add(value.GetKey(), value);
	}

	public new virtual void Add(string key, T value)
	{
		base.Add(key, value);
		AddedItem(key);
	}

	public new virtual bool Remove(string key)
	{
		bool flag = base.Remove(key);
		if (flag)
		{
			RemovedItem(key);
		}
		return flag;
	}

	public new virtual bool ContainsKey(string key)
	{
		return base.ContainsKey(key);
	}

	public new virtual void Clear()
	{
		base.Clear();
		ClearedItems();
	}

	protected virtual void AddedItem(string key)
	{
		CollectionAddedMessage msg = new CollectionAddedMessage(key);
		AppShell.Instance.EventMgr.Fire(this, msg);
	}

	protected virtual void RemovedItem(string key)
	{
		CollectionRemovedMessage msg = new CollectionRemovedMessage(key);
		AppShell.Instance.EventMgr.Fire(this, msg);
	}

	protected virtual void ClearedItems()
	{
		CollectionClearedMessage msg = new CollectionClearedMessage();
		AppShell.Instance.EventMgr.Fire(this, msg);
	}
}
