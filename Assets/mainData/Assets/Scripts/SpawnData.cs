using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnData : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	[Flags]
	public enum PlayerType
	{
		Stranger = 0x0,
		ShieldAgent = 0x1,
		Blocked = 0x2,
		Met = 0x4,
		Friend = 0x8,
		Self = 0x10,
		Moderator = 0x20
	}

	public string modelName;

	public float sizeRank;

	public CharacterSpawn spawner;

	public CharacterSpawn.Type spawnType;

	public bool sendNewEntityMsg = true;

	public bool rewardsPoints = true;

	protected bool cached;

	protected string cachedSquadName = string.Empty;

	private bool _spawnEventFired;

	public bool SpawnEventFired
	{
		get
		{
			return _spawnEventFired;
		}
	}

	public void GetSquadLevel(out int squadLevel)
	{
		squadLevel = 0;
		if ((spawnType & CharacterSpawn.Type.Local) != 0 && (spawnType & CharacterSpawn.Type.Player) != 0)
		{
			UserProfile profile = AppShell.Instance.Profile;
			if (profile != null)
			{
				squadLevel = profile.SquadLevel;
			}
			return;
		}
		NetworkComponent component = Utils.GetComponent<NetworkComponent>(base.transform.root.gameObject);
		if (component != null && component.goNetId.IsPlayerId() && component.goNetId.ChildId != AppShell.Instance.ServerConnection.GetGameUserId())
		{
			int childId = component.goNetId.ChildId;
			List<NetworkManager.UserInfo> gameAllUsers = AppShell.Instance.ServerConnection.GetGameAllUsers();
			foreach (NetworkManager.UserInfo item in gameAllUsers)
			{
				if (item.userId == childId)
				{
					PlayerDictionary.Player value;
					AppShell.Instance.PlayerDictionary.TryGetValue(item.userId, out value);
					if (value != null)
					{
						squadLevel = value.SquadLevel;
					}
				}
			}
		}
	}

	public void GetHeroLevel(out int heroLevel)
	{
		heroLevel = 0;
		if ((spawnType & CharacterSpawn.Type.Local) != 0 && (spawnType & CharacterSpawn.Type.Player) != 0)
		{
			UserProfile profile = AppShell.Instance.Profile;
			if (profile != null)
			{
				heroLevel = profile.AvailableCostumes[base.name].Level;
			}
			return;
		}
		NetworkComponent component = Utils.GetComponent<NetworkComponent>(base.transform.root.gameObject);
		if (component != null && component.goNetId.IsPlayerId() && component.goNetId.ChildId != AppShell.Instance.ServerConnection.GetGameUserId())
		{
			int childId = component.goNetId.ChildId;
			List<NetworkManager.UserInfo> gameAllUsers = AppShell.Instance.ServerConnection.GetGameAllUsers();
			foreach (NetworkManager.UserInfo item in gameAllUsers)
			{
				if (item.userId == childId)
				{
					PlayerDictionary.Player value;
					AppShell.Instance.PlayerDictionary.TryGetValue(item.userId, out value);
					if (value != null)
					{
						heroLevel = value.HeroLevel;
					}
				}
			}
		}
	}

	public void GetSquadRelation(out string squadName, out PlayerType playerType, bool allowCached)
	{
		playerType = PlayerType.Stranger;
		if (!allowCached || !cached)
		{
			cachedSquadName = "Anonymous Squad";
			if ((spawnType & CharacterSpawn.Type.Local) != 0 && (spawnType & CharacterSpawn.Type.Player) != 0)
			{
				UserProfile profile = AppShell.Instance.Profile;
				if (profile != null)
				{
					playerType |= PlayerType.Self;
					cachedSquadName = profile.PlayerName;
					if (Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldPlayAllow))
					{
						playerType |= PlayerType.ShieldAgent;
					}
				}
			}
			else
			{
				NetworkComponent component = Utils.GetComponent<NetworkComponent>(base.transform.root.gameObject);
				if (component != null && component.goNetId.IsPlayerId() && component.goNetId.ChildId != AppShell.Instance.ServerConnection.GetGameUserId())
				{
					int childId = component.goNetId.ChildId;
					List<NetworkManager.UserInfo> gameAllUsers = AppShell.Instance.ServerConnection.GetGameAllUsers();
					foreach (NetworkManager.UserInfo item in gameAllUsers)
					{
						if (item.userId == childId)
						{
							cachedSquadName = item.userName;
							PlayerDictionary.Player value;
							AppShell.Instance.PlayerDictionary.TryGetValue(item.userId, out value);
							if (value != null && value.IsShieldAgent)
							{
								playerType |= PlayerType.ShieldAgent;
							}
							if (AppShell.Instance.Profile.AvailableFriends.ContainsKey(Utils.GetGazillionID(childId).ToString()))
							{
								playerType |= PlayerType.Friend;
							}
							else if (AppShell.Instance.Profile.AvailableFriends.AvailableBlocked.ContainsKey(Utils.GetGazillionID(childId).ToString()))
							{
								playerType |= PlayerType.Blocked;
							}
							break;
						}
					}
				}
			}
			cached = true;
		}
		squadName = cachedSquadName;
	}

	public long GetPlayerId()
	{
		int num = -1;
		NetworkComponent component = Utils.GetComponent<NetworkComponent>(base.transform.root.gameObject);
		if (component != null && component.goNetId.IsPlayerId() && component.goNetId.ChildId != AppShell.Instance.ServerConnection.GetGameUserId())
		{
			num = component.goNetId.ChildId;
		}
		return num;
	}

	public void Start()
	{
		StartCoroutine(FireSpawnEvent());
	}

	public void Despawn(EntityDespawnMessage.despawnType cause)
	{
		Despawn(cause, true);
	}

	public void Despawn(EntityDespawnMessage.despawnType cause, bool sendDeleteEntityMsg)
	{
		Despawn(cause, sendDeleteEntityMsg, true);
	}

	public void Despawn(EntityDespawnMessage.despawnType cause, bool sendDeleteEntityMsg, bool releaseOwnership)
	{
		if (spawner.despawnOnDeath)
		{
			EntityDespawnMessage msg = new EntityDespawnMessage(base.gameObject, spawnType, cause, sendDeleteEntityMsg, releaseOwnership);
			AppShell.Instance.EventMgr.Fire(spawner, msg);
			Utils.DelayedDestroy(base.gameObject, 2f);
		}
	}

	public void Die()
	{
		AppShell.Instance.EventMgr.Fire(spawner, new CombatCharacterKilledMessage(base.gameObject, null, null));
	}

	public bool IsPolymorph()
	{
		return spawner as PolymorphSpawn != null;
	}

	protected IEnumerator FireSpawnEvent()
	{
		yield return 0;
		AppShell.Instance.EventMgr.Fire(spawner, new EntitySpawnMessage(base.gameObject, spawnType, sendNewEntityMsg));
		_spawnEventFired = true;
	}
}
