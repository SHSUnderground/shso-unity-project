using System.Collections;
using UnityEngine;

public class CharacterSpawnData
{
	public CharacterSpawn spawner;

	public string ModelName;

	public int R2Attack;

	public Object CharacterPrefab;

	public Transform Location;

	public CharacterSpawn.Type Type;

	public CharacterSpawn.CharacterSpawned OnSpawnCallback;

	public DataWarehouse characterData;

	public DataWarehouse hqData;

	public DataWarehouse npcData;

	public AssetBundle characterAssets;

	public Hashtable netExtraData;

	public bool IsNetworked;

	public object extra;

	public string characterBundlePath;

	public TransactionMonitor spawnTransaction;

	public TransactionMonitor assetLoadTransaction;

	public TransactionMonitor prespawnTransaction;

	public float ownershipAttempts;

	public GameObject prespawnedCharacter;

	public GoNetId goNetId;

	public CharacterSpawnData(string modelName, int r2Attack, Transform location, CharacterSpawn.Type type, Hashtable netExtraData, CharacterSpawn.CharacterSpawned callback, bool isNetworked, object extra)
	{
		CharacterSpawn.d(modelName + " CharacterSpawnData ");
		ModelName = modelName;
		R2Attack = r2Attack;
		Location = location;
		Type = type;
		OnSpawnCallback = callback;
		characterAssets = null;
		this.netExtraData = netExtraData;
		IsNetworked = isNetworked;
		this.extra = extra;
	}

	public void SetSpawner(CharacterSpawn spawner)
	{
		this.spawner = spawner;
		goNetId = spawner.goNetId;
	}

	public void AddOwnershipStep()
	{
		if (spawnTransaction != null)
		{
			spawnTransaction.AddStep("ownership");
			ownershipAttempts = 0f;
			if (spawner != null && spawner.spawnerNetwork != null && spawner.IsLocal)
			{
				AttemptOwnership();
			}
			else
			{
				spawnTransaction.CompleteStep("ownership");
			}
		}
	}

	protected void AttemptOwnership()
	{
		ownershipAttempts += 1f;
		bool autoTransfer = BrawlerController.Instance == null || !spawner.IsAI;
		AppShell.Instance.ServerConnection.Game.TakeOwnership(spawner.gameObject, OnOwnershipChange, autoTransfer);
	}

	protected IEnumerator AttemptOwnershipDelay()
	{
		yield return new WaitForSeconds(2f);
		AttemptOwnership();
	}

	protected void OnOwnershipChange(GameObject go, bool bAssumedOwnership)
	{
		if (spawnTransaction != null)
		{
			if (bAssumedOwnership)
			{
				spawnTransaction.CompleteStep("ownership");
			}
			else if (BrawlerController.Instance != null && spawner.IsAI && ownershipAttempts < (float)CharacterSpawn.MAX_OWNERSHIP_ATTEMPTS)
			{
				spawner.StartCoroutine(AttemptOwnershipDelay());
			}
			else
			{
				spawnTransaction.FailStep("ownership", "Did not receive ownership");
			}
		}
	}
}
