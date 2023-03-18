using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterCombinationManager
{
	private List<BaseCharacterCombination> mPresetCombinations;

	private List<BaseCharacterCombination> mDynamicCombinations;

	private List<BaseCharacterCombination> mCurPresetCombinations;

	private Dictionary<string, List<BaseCharacterCombination>> mCurDynamicCombinations;

	private List<GameObject> mPlayers;

	[CompilerGenerated]
	private static bool _003CCombinationsEnabled_003Ek__BackingField;

	public List<BaseCharacterCombination> PresetCombinations
	{
		get
		{
			return mPresetCombinations;
		}
	}

	public List<BaseCharacterCombination> ActiveCombinations
	{
		get
		{
			return mCurPresetCombinations;
		}
	}

	public static bool CombinationsEnabled
	{
		[CompilerGenerated]
		get
		{
			return _003CCombinationsEnabled_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CCombinationsEnabled_003Ek__BackingField = value;
		}
	}

	public CharacterCombinationManager()
	{
		mPresetCombinations = new List<BaseCharacterCombination>();
		mDynamicCombinations = new List<BaseCharacterCombination>();
		mCurDynamicCombinations = new Dictionary<string, List<BaseCharacterCombination>>();
		mPlayers = new List<GameObject>();
	}

	static CharacterCombinationManager()
	{
		CombinationsEnabled = true;
	}

	public void InitializeFromData(DataWarehouse data)
	{
		DataWarehouse data2 = data.GetData("character_combination_data");
		InitializeCombinationListFromData(mPresetCombinations, data2.GetIterator("preset_combination"), true);
		InitializeCombinationListFromData(mDynamicCombinations, data2.GetIterator("dynamic_combination"), false);
	}

	public void AddEventListeners()
	{
		AppShell.Instance.EventMgr.AddListener<EntitySpawnMessage>(OnCharacterSpawn);
		AppShell.Instance.EventMgr.AddListener<EntityDespawnMessage>(OnCharacterDespawn);
		AppShell.Instance.EventMgr.AddListener<RoomUserLeaveMessage>(OnRoomUserLeave);
	}

	public void RemoveEventListeners()
	{
		AppShell.Instance.EventMgr.RemoveListener<EntitySpawnMessage>(OnCharacterSpawn);
		AppShell.Instance.EventMgr.RemoveListener<EntityDespawnMessage>(OnCharacterDespawn);
		AppShell.Instance.EventMgr.RemoveListener<RoomUserLeaveMessage>(OnRoomUserLeave);
	}

	public void OnCharacterSpawn(EntitySpawnMessage msg)
	{
		if (msg != null && !(msg.go == null) && (msg.spawnType & CharacterSpawn.Type.Player) != 0 && !mPlayers.Contains(msg.go))
		{
			mPlayers.Add(msg.go);
			ClearPresetCombinations();
			FindPresetCombinations(GetStoredCharacterNames());
			ApplyPresetCombinations();
		}
	}

	public void OnCharacterDespawn(EntityDespawnMessage msg)
	{
		if (msg != null && !(msg.go == null) && (msg.type & CharacterSpawn.Type.Player) != 0 && mPlayers.Contains(msg.go))
		{
			RemoveStoredPlayer(msg.go);
		}
	}

	public void OnRoomUserLeave(RoomUserLeaveMessage msg)
	{
		NetGameManager game = AppShell.Instance.ServerConnection.Game;
		if (game != null)
		{
			GameObject gameObjectFromNetId = game.GetGameObjectFromNetId(new GoNetId(GoNetId.PLAYER_ID_FLAG, msg.userId));
			foreach (GameObject mPlayer in mPlayers)
			{
				if (mPlayer == gameObjectFromNetId)
				{
					RemoveStoredPlayer(mPlayer);
					break;
				}
			}
		}
	}

	public void OutputCombinations()
	{
		foreach (BaseCharacterCombination mPresetCombination in mPresetCombinations)
		{
			CspUtils.DebugLog(mPresetCombination);
		}
		foreach (BaseCharacterCombination mDynamicCombination in mDynamicCombinations)
		{
			CspUtils.DebugLog(mDynamicCombination);
		}
		if (mCurPresetCombinations != null)
		{
			foreach (BaseCharacterCombination mCurPresetCombination in mCurPresetCombinations)
			{
				CspUtils.DebugLog(mCurPresetCombination);
			}
		}
	}

	public void FindPresetCombinations(List<string> characterNames)
	{
		if (CombinationsEnabled)
		{
			mCurPresetCombinations = mPresetCombinations.FindAll(delegate(BaseCharacterCombination combination)
			{
				return combination.IsActive(characterNames);
			});
			AppShell.Instance.EventMgr.Fire(this, new PresetCombinationsSelectedMessage(mCurPresetCombinations));
		}
	}

	public void ApplyPresetCombinations()
	{
		if (CombinationsEnabled && mCurPresetCombinations != null)
		{
			GameController controller = GameController.GetController();
			if (!(controller == null))
			{
				foreach (BaseCharacterCombination mCurPresetCombination in mCurPresetCombinations)
				{
					mCurPresetCombination.Apply(controller.LocalPlayer);
				}
				AppShell.Instance.EventMgr.Fire(null, new PresetCombinationsApplyMessage(ActiveCombinations));
			}
		}
	}

	public void ClearPresetCombinations()
	{
		if (mCurPresetCombinations != null)
		{
			GameController controller = GameController.GetController();
			foreach (BaseCharacterCombination mCurPresetCombination in mCurPresetCombinations)
			{
				if (mCurPresetCombination.IsApplied)
				{
					mCurPresetCombination.Erase(controller.LocalPlayer);
				}
			}
			mCurPresetCombinations = null;
		}
	}

	public void FindDynamicCombinations(List<string> characterNames)
	{
		mCurDynamicCombinations.Clear();
		List<BaseCharacterCombination> list = mDynamicCombinations.FindAll(delegate(BaseCharacterCombination combination)
		{
			return combination.IsActive(characterNames);
		});
		using (List<string>.Enumerator enumerator = characterNames.GetEnumerator())
		{
			string character;
			while (enumerator.MoveNext())
			{
				character = enumerator.Current;
				List<BaseCharacterCombination> list2 = list.FindAll(delegate(BaseCharacterCombination combination)
				{
					return combination.GetCharacter(BaseDynamicCombination.TRIGGER_CHARACTER) == character;
				});
				if (list2.Count > 0)
				{
					mCurDynamicCombinations.Add(character, list2);
				}
			}
		}
	}

	public static void EnableCombinations(bool enable)
	{
		CombinationsEnabled = enable;
		if (!(BrawlerController.Instance == null) && BrawlerController.Instance.CharacterCombinationManager != null)
		{
			CharacterCombinationManager characterCombinationManager = BrawlerController.Instance.CharacterCombinationManager;
			characterCombinationManager.ClearPresetCombinations();
			if (enable)
			{
				characterCombinationManager.FindPresetCombinations(characterCombinationManager.GetStoredCharacterNames());
				characterCombinationManager.ApplyPresetCombinations();
			}
		}
	}

	protected List<string> GetStoredCharacterNames()
	{
		if (mPlayers.Count <= 0)
		{
			return null;
		}
		List<string> list = new List<string>();
		foreach (GameObject mPlayer in mPlayers)
		{
			if (mPlayer != null)
			{
				list.Add(mPlayer.name);
			}
		}
		return list;
	}

	private void InitializeCombinationListFromData(List<BaseCharacterCombination> list, IEnumerable<DataWarehouse> itr, bool preset)
	{
		foreach (DataWarehouse item in itr)
		{
			BaseCharacterCombination baseCharacterCombination = (!preset) ? ((BaseCharacterCombination)new BaseDynamicCombination()) : ((BaseCharacterCombination)new BasePresetCombination());
			baseCharacterCombination.InitializeFromData(item);
			list.Add(baseCharacterCombination);
		}
	}

	private void RemoveStoredPlayer(GameObject player)
	{
		mPlayers.Remove(player);
		GameController controller = GameController.GetController();
		if (mCurPresetCombinations != null)
		{
			List<string> storedCharacterNames = GetStoredCharacterNames();
			List<BaseCharacterCombination> list = new List<BaseCharacterCombination>();
			foreach (BaseCharacterCombination mCurPresetCombination in mCurPresetCombinations)
			{
				if (!mCurPresetCombination.IsActive(storedCharacterNames))
				{
					list.Add(mCurPresetCombination);
				}
			}
			foreach (BaseCharacterCombination item in list)
			{
				mCurPresetCombinations.Remove(item);
				if (item.IsApplied)
				{
					item.Erase(controller.LocalPlayer);
				}
			}
			AppShell.Instance.EventMgr.Fire(this, new PresetCombinationsSelectedMessage(mCurPresetCombinations));
		}
		if (player == controller.LocalPlayer)
		{
			ClearPresetCombinations();
		}
	}
}
