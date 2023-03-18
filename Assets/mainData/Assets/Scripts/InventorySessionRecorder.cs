using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InventorySessionRecorder
{
	public class InventoryRecord
	{
		public enum RecordType
		{
			UnknownOwnable,
			HQOwnable,
			HeroOwnable,
			ExpendableOwnable
		}

		[CompilerGenerated]
		private string _003CRecordOwnableId_003Ek__BackingField;

		[CompilerGenerated]
		private RecordType _003CRecordOwnableType_003Ek__BackingField;

		[CompilerGenerated]
		private List<float> _003CRecordTimeStampList_003Ek__BackingField;

		public string RecordOwnableId
		{
			[CompilerGenerated]
			get
			{
				return _003CRecordOwnableId_003Ek__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				_003CRecordOwnableId_003Ek__BackingField = value;
			}
		}

		public RecordType RecordOwnableType
		{
			[CompilerGenerated]
			get
			{
				return _003CRecordOwnableType_003Ek__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				_003CRecordOwnableType_003Ek__BackingField = value;
			}
		}

		public List<float> RecordTimeStampList
		{
			[CompilerGenerated]
			get
			{
				return _003CRecordTimeStampList_003Ek__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				_003CRecordTimeStampList_003Ek__BackingField = value;
			}
		}

		public float RecordLatestTimeStamp
		{
			get
			{
				return RecordTimeStampList[0];
			}
		}

		public InventoryRecord(string ownableId, RecordType ownableType)
		{
			RecordOwnableId = ownableId;
			RecordOwnableType = ownableType;
			RecordTimeStampList = new List<float>();
			RecordTimeStampList.Add(Time.time);
		}

		public void TimeStamp()
		{
			RecordTimeStampList.Insert(0, Time.time);
		}

		public override string ToString()
		{
			string text = string.Format("* Inventory Record\n\t-OwnableId={0}\n\t-OwnableType={1}\n\t-TimeStamp List:\n", RecordOwnableId, RecordOwnableType);
			foreach (float recordTimeStamp in RecordTimeStampList)
			{
				float num = recordTimeStamp;
				string text2 = text;
				text = text2 + "\t\t" + num + "\n";
			}
			return text;
		}
	}

	[CompilerGenerated]
	private List<InventoryRecord> _003CInventoryRecordList_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CIsInitialized_003Ek__BackingField;

	public List<InventoryRecord> InventoryRecordList
	{
		[CompilerGenerated]
		get
		{
			return _003CInventoryRecordList_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CInventoryRecordList_003Ek__BackingField = value;
		}
	}

	public bool IsInitialized
	{
		[CompilerGenerated]
		get
		{
			return _003CIsInitialized_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CIsInitialized_003Ek__BackingField = value;
		}
	}

	public InventorySessionRecorder()
	{
		IsInitialized = false;
	}

	~InventorySessionRecorder()
	{
		Destroy();
	}

	public void Initialize()
	{
		if (IsInitialized)
		{
			Destroy();
		}
		InventoryRecordList = new List<InventoryRecord>();
		AppShell.Instance.EventMgr.AddListener<ShoppingItemPurchasedMessage>(OnShoppingItemPurchased);
		AppShell.Instance.EventMgr.AddListener<LeveledUpMessage>(OnLeveledUp);
		AppShell.Instance.EventMgr.AddListener<PrizeWheelSlotResultMessage>(OnPrizeWheelSlotResult);
		AppShell.Instance.EventMgr.AddListener<ChallengeRewardSelectedMessage>(OnChallengeRewardSelected);
		AppShell.Instance.EventMgr.AddListener<BrawlerResultsMessage>(OnBrawlerRewardsResult);
		IsInitialized = true;
	}

	public void Destroy()
	{
		if (IsInitialized)
		{
			AppShell.Instance.EventMgr.RemoveListener<ShoppingItemPurchasedMessage>(OnShoppingItemPurchased);
			AppShell.Instance.EventMgr.RemoveListener<LeveledUpMessage>(OnLeveledUp);
			AppShell.Instance.EventMgr.RemoveListener<PrizeWheelSlotResultMessage>(OnPrizeWheelSlotResult);
			AppShell.Instance.EventMgr.RemoveListener<ChallengeRewardSelectedMessage>(OnChallengeRewardSelected);
			AppShell.Instance.EventMgr.RemoveListener<BrawlerResultsMessage>(OnBrawlerRewardsResult);
			InventoryRecordList = null;
			IsInitialized = false;
		}
	}

	public void RecordInventoryAdd(string ownableId)
	{
		if (!IsInitialized || string.IsNullOrEmpty(ownableId))
		{
			return;
		}
		InventoryRecord.RecordType inventoryOwnableType = GetInventoryOwnableType(ownableId);
		if (inventoryOwnableType != 0)
		{
			InventoryRecord inventoryRecord = FindRecord(ownableId);
			if (inventoryRecord != null)
			{
				inventoryRecord.TimeStamp();
				InventoryRecordList.Remove(inventoryRecord);
			}
			else
			{
				inventoryRecord = new InventoryRecord(ownableId, inventoryOwnableType);
			}
			InventoryRecordList.Insert(0, inventoryRecord);
			SHSInventoryAnimatedWindow.InventoryItemAcquired(inventoryOwnableType);
		}
	}

	public int FindRecordIndex(string ownableId)
	{
		if (!IsInitialized)
		{
			return -1;
		}
		if (string.IsNullOrEmpty(ownableId))
		{
			return -1;
		}
		return InventoryRecordList.FindIndex(delegate(InventoryRecord record)
		{
			return record.RecordOwnableId == ownableId;
		});
	}

	public InventoryRecord FindRecord(string ownableId)
	{
		int num = FindRecordIndex(ownableId);
		return (num < 0) ? null : InventoryRecordList[num];
	}

	public void LogInventoryRecordList()
	{
		string str = "Logging inventory record list:\n====================\n";
		foreach (InventoryRecord inventoryRecord in InventoryRecordList)
		{
			str = str + inventoryRecord.ToString() + "\n";
		}
		str += "====================";
		CspUtils.DebugLog(str);
	}

	private void OnShoppingItemPurchased(ShoppingItemPurchasedMessage msg)
	{
		switch (msg.ItemType)
		{
		case OwnableDefinition.Category.HQItem:
		case OwnableDefinition.Category.Potion:
			RecordInventoryAdd(msg.OwnableId);
			break;
		case OwnableDefinition.Category.Hero:
			RecordInventoryAdd(msg.OwnableName);
			break;
		}
	}

	private void OnLeveledUp(LeveledUpMessage msg)
	{
		RecordInventoryAdd(msg.OwnableTypeId);
	}

	private void OnPrizeWheelSlotResult(PrizeWheelSlotResultMessage msg)
	{
		if (msg.slotAwarded != null && msg.slotAwarded.itemId.HasValue)
		{
			RecordInventoryAdd(msg.slotAwarded.itemId.Value.ToString());
		}
	}

	private void OnChallengeRewardSelected(ChallengeRewardSelectedMessage msg)
	{
		RecordInventoryAdd(msg.hero);
	}

	private void OnBrawlerRewardsResult(BrawlerResultsMessage msg)
	{
		if (!(AppShell.Instance != null) || AppShell.Instance.Profile == null || !(AppShell.Instance.Profile is LocalPlayerProfile) || !msg.Results.PlayerResults.ContainsKey(AppShell.Instance.Profile.UserId))
		{
			return;
		}
		MissionResults missionResults = msg.Results.PlayerResults[AppShell.Instance.Profile.UserId];
		if (!string.IsNullOrEmpty(missionResults.ownable))
		{
			RecordInventoryAdd(missionResults.ownable);
			InventoryRecord.RecordType inventoryOwnableType = GetInventoryOwnableType(missionResults.ownable);
			if (inventoryOwnableType == InventoryRecord.RecordType.ExpendableOwnable)
			{
				AppShell.Instance.Profile.StartPotionFetch();
			}
			else
			{
				AppShell.Instance.Profile.StartInventoryFetch();
			}
		}
	}

	private InventoryRecord.RecordType GetInventoryOwnableType(string ownableId)
	{
		if (AppShell.Instance != null)
		{
			if (AppShell.Instance.ItemDictionary != null && AppShell.Instance.ItemDictionary.ContainsKey(ownableId))
			{
				return InventoryRecord.RecordType.HQOwnable;
			}
			if (AppShell.Instance.ExpendablesManager != null && AppShell.Instance.ExpendablesManager.ExpendableTypes != null && AppShell.Instance.ExpendablesManager.ExpendableTypes.ContainsKey(ownableId))
			{
				ExpendableDefinition expendableDefinition = AppShell.Instance.ExpendablesManager.ExpendableTypes[ownableId];
				if (!expendableDefinition.CategoryList.Contains(ExpendableDefinition.Categories.Internal))
				{
					return InventoryRecord.RecordType.ExpendableOwnable;
				}
			}
			if (AppShell.Instance.CharacterDescriptionManager != null && AppShell.Instance.CharacterDescriptionManager.Contains(ownableId))
			{
				return InventoryRecord.RecordType.HeroOwnable;
			}
		}
		return InventoryRecord.RecordType.UnknownOwnable;
	}
}
