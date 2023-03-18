using System;
using System.Collections.Generic;

public class HeroCollection : ShsCollectionBase<HeroPersisted>
{
	protected const string ELEMENT_NAME = "hero";

	protected const string KEY_NAME = "name";

	protected bool readOnly;

	public bool ReadOnly
	{
		get
		{
			return readOnly;
		}
		set
		{
			readOnly = value;
		}
	}

	public HeroCollection()
	{
		collectionElementName = "hero";
		keyName = "name";
	}

	public HeroCollection(DataWarehouse xmlData)
		: this()
	{
		InitializeFromData(xmlData);
		AppShell.Instance.EventMgr.AddListener<CollectionResetMessage>(OnHeroesReset);
	}

	public HeroCollection(List<RemotePlayerProfileJsonHeroData> jsonData)
		: this()
	{
		InitializeFromData(jsonData);
	}

	~HeroCollection()
	{
		if (!readOnly)
		{
			AppShell.Instance.EventMgr.RemoveListener<CollectionResetMessage>(OnHeroesReset);
		}
	}

	protected void InitializeFromData(List<RemotePlayerProfileJsonHeroData> dataList)
	{
		foreach (RemotePlayerProfileJsonHeroData data in dataList)
		{
			HeroPersisted heroPersisted = new HeroPersisted();
			if (heroPersisted.InitializeFromData(data))
			{
				Add(heroPersisted.GetKey(), heroPersisted);
			}
			else
			{
				CspUtils.DebugLog("Item Initialization failed");
			}
		}
		readOnly = true;
	}

	public void UpdateItemsFromData(string xmlData)
	{
		if (readOnly)
		{
			throw new NotSupportedException();
		}
		DataWarehouse dataWarehouse = new DataWarehouse(xmlData);
		dataWarehouse.Parse();
		DataWarehouse data = dataWarehouse.GetData("//heroes");
		UpdateItemsFromData(data);
	}

	public void UpdateItemFromData(string xmlData)
	{
		if (readOnly)
		{
			throw new NotSupportedException();
		}
		DataWarehouse dataWarehouse = new DataWarehouse(xmlData);
		dataWarehouse.Parse();
		DataWarehouse data = dataWarehouse.GetData("//hero");
		UpdateItemFromData(data);
	}

	protected override string GetKeyFromItemData(DataWarehouse itemData)
	{
		string @string = itemData.GetString(keyName);
		return (@string == null) ? null : @string.ToLower();
	}

	private void resetHeroes(string[] keys, bool add)
	{
		if (readOnly)
		{
			throw new NotSupportedException();
		}
		foreach (string b in keys)
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, HeroPersisted> current = enumerator.Current;
					if (current.Value.Name == b)
					{
						current.Value.Placed = !add;
					}
				}
			}
		}
	}

	private void OnHeroesReset(CollectionResetMessage message)
	{
		if (readOnly)
		{
			throw new NotSupportedException();
		}
		if (message.collectionId == "Heroes")
		{
			resetHeroes(message.keys, message.actionType == CollectionResetMessage.ActionType.Add);
		}
	}

	public void ResetPlacedValues()
	{
		if (readOnly)
		{
			throw new NotSupportedException();
		}
		HqController2 instance = HqController2.Instance;
		if (instance != null)
		{
			foreach (HeroPersisted value in base.Values)
			{
				value.Placed = instance.IsHeroPlaced(value);
			}
		}
	}
}
