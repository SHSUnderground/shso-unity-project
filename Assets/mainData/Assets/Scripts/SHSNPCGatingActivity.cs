using System.Collections.Generic;
using UnityEngine;

public class SHSNPCGatingActivity : SHSActivityBase
{
	private int spawnedCharacters;

	private List<LimitingThreshold> limitingThresholdsOriginal;

	private List<LimitingThreshold> limitingThresholds;

	private LimitingThreshold currentThreshold;

	private Stack<AIControllerNPC> activeNpcs;

	private Stack<AIControllerNPC> inactiveNpcs;

	private LinkedList<AIControllerNPC> choppingBlock = new LinkedList<AIControllerNPC>();

	private List<AIControllerNPC> loadingNPCs;

	private int totalNpcCount;

	private bool initialized;

	private NPCSpawn[] allNpcSpawners;

	private Queue<CharacterSpawn> pendingSpawnerRegistrations = new Queue<CharacterSpawn>();

	public string CondensedStatus
	{
		get
		{
			return string.Format("[{0}:{1}:{2}]", choppingBlock.Count, activeNpcs.Count, inactiveNpcs.Count);
		}
	}

	public override void Configure(SHSActivityManager manager, DataWarehouse data)
	{
		base.Configure(manager, data);
		limitingThresholdsOriginal = LimitingThreshold.LoadAndSort(activityConfigurationData, "configuration/limiting_thresholds");
		limitingThresholds = limitingThresholdsOriginal;
	}

	public override void OnZoneLoaded(string zoneName, int zoneID)
	{
		loadingNPCs = new List<AIControllerNPC>();
		allNpcSpawners = (Object.FindObjectsOfType(typeof(NPCSpawn)) as NPCSpawn[]);
		totalNpcCount = 0;
		NPCSpawn[] array = allNpcSpawners;
		foreach (NPCSpawn nPCSpawn in array)
		{
			if (nPCSpawn.IsNpc)
			{
				totalNpcCount++;
				nPCSpawn.onSpawnCallback += OnNPCSpawn;
			}
		}
		base.OnZoneLoaded(zoneName, zoneID);
	}

	public override void OnZoneUnloaded(string zoneName)
	{
		base.OnZoneUnloaded(zoneName);
		if (initialized)
		{
			initialized = false;
			AppShell.Instance.EventMgr.RemoveListener<RoomUserLeaveMessage>(OnUserLeaveRoom);
			AppShell.Instance.EventMgr.RemoveListener<RoomUserEnterMessage>(OnUserEnterRoom);
			AppShell.Instance.EventMgr.RemoveListener<GraphicsOptionsChange>(OnGraphicsOptionsChange);
			activeNpcs.Clear();
			inactiveNpcs.Clear();
		}
		if (loadingNPCs != null)
		{
			loadingNPCs.Clear();
		}
	}

	private void OnNPCSpawn(GameObject obj)
	{
		if (!initialized)
		{
			loadingNPCs.Add(Utils.GetComponent<AIControllerNPC>(obj));
			totalNpcCount = 0;
			NPCSpawn[] array = allNpcSpawners;
			foreach (NPCSpawn nPCSpawn in array)
			{
				if (nPCSpawn != null && nPCSpawn.IsNpc && nPCSpawn.gameObject.active)
				{
					totalNpcCount++;
				}
			}
			if (loadingNPCs.Count == totalNpcCount)
			{
				Initialize();
			}
		}
		else
		{
			LinkedListNode<AIControllerNPC> linkedListNode = choppingBlock.Find(null);
			if (linkedListNode != null)
			{
				linkedListNode.Value = obj.GetComponent<AIControllerNPC>();
				CspUtils.DebugLog("NPC Gating has detected an NPC respawn and has updated the \"to be deactivated\" list; " + CondensedStatus);
			}
			else
			{
				int num = 0;
				foreach (AIControllerNPC activeNpc in activeNpcs)
				{
					if (activeNpc == null)
					{
						AIControllerNPC[] array2 = activeNpcs.ToArray();
						array2[num] = obj.GetComponent<AIControllerNPC>();
						activeNpcs = new Stack<AIControllerNPC>(array2);
						CspUtils.DebugLog("NPC Gating has detected an NPC respawn and has updated the \"active\" list; " + CondensedStatus);
						break;
					}
					num++;
				}
			}
		}
		pendingSpawnerRegistrations.Enqueue(obj.GetComponent<SpawnData>().spawner);
	}

	private void Initialize()
	{
		loadingNPCs.Sort(delegate(AIControllerNPC left, AIControllerNPC right)
		{
			float spawnPriority = ((NPCSpawn)Utils.GetComponent<SpawnData>(left).spawner).spawnPriority;
			float spawnPriority2 = ((NPCSpawn)Utils.GetComponent<SpawnData>(right).spawner).spawnPriority;
			return (!(spawnPriority2 < spawnPriority)) ? 1 : (-1);
		});
		activeNpcs = new Stack<AIControllerNPC>(loadingNPCs);
		loadingNPCs.Clear();
		inactiveNpcs = new Stack<AIControllerNPC>();
		totalNpcCount = activeNpcs.Count;
		spawnedCharacters = AppShell.Instance.ServerConnection.GetGameUserCount();
		OnGraphicsOptionsChange(null);
		AppShell.Instance.EventMgr.AddListener<RoomUserEnterMessage>(OnUserEnterRoom);
		AppShell.Instance.EventMgr.AddListener<RoomUserLeaveMessage>(OnUserLeaveRoom);
		AppShell.Instance.EventMgr.AddListener<GraphicsOptionsChange>(OnGraphicsOptionsChange);
		initialized = true;
	}

	public override void Update()
	{
		base.Update();
		while (pendingSpawnerRegistrations.Count > 0)
		{
			pendingSpawnerRegistrations.Dequeue().onSpawnCallback += OnNPCSpawn;
		}
		while (choppingBlock.Count > 0 && choppingBlock.First.Value != null && !(choppingBlock.First.Value.CurrentCommand is NPCReactCommand))
		{
			AIControllerNPC value = choppingBlock.First.Value;
			choppingBlock.RemoveFirst();
			Utils.ActivateTree(value.gameObject, false);
			inactiveNpcs.Push(value);
		}
	}

	private void OnUserEnterRoom(RoomUserEnterMessage e)
	{
		spawnedCharacters++;
		CheckThresholds();
	}

	private void OnUserLeaveRoom(RoomUserLeaveMessage e)
	{
		spawnedCharacters = Mathf.Max(spawnedCharacters - 1, 0);
		CheckThresholds();
	}

	private void OnGraphicsOptionsChange(GraphicsOptionsChange e)
	{
		limitingThresholds = limitingThresholdsOriginal;
		CheckThresholds();
	}

	private void CheckThresholds()
	{
		LimitingThreshold limitingThreshold = null;
		foreach (LimitingThreshold limitingThreshold2 in limitingThresholds)
		{
			if (spawnedCharacters < limitingThreshold2.CharacterCount)
			{
				break;
			}
			limitingThreshold = limitingThreshold2;
		}
		if (limitingThreshold == currentThreshold)
		{
			return;
		}
		if (limitingThreshold == null)
		{
			CspUtils.DebugLog("Character threshold reached, increasing NPC count to " + totalNpcCount + "; " + CondensedStatus);
			ActivateNPCs(totalNpcCount - activeNpcs.Count);
		}
		else
		{
			int num = (int)(limitingThreshold.MaxPercent / 100f * (float)totalNpcCount);
			if (currentThreshold != null && limitingThreshold.MaxPercent > currentThreshold.MaxPercent)
			{
				CspUtils.DebugLog("Character threshold reached, increasing NPC count to " + num + "; " + CondensedStatus);
				ActivateNPCs(num - activeNpcs.Count);
			}
			else
			{
				CspUtils.DebugLog("Character threshold reached, decreasing NPC count to " + num + "; " + CondensedStatus);
				DeactivateNPCs(activeNpcs.Count - num);
			}
		}
		currentThreshold = limitingThreshold;
	}

	private void ActivateNPCs(int count)
	{
		if (count > inactiveNpcs.Count + choppingBlock.Count)
		{
			CspUtils.DebugLog("Tried to activate " + count + " NPCs, but there were only " + (inactiveNpcs.Count + choppingBlock.Count) + " available.");
			return;
		}
		for (int i = 0; i < count; i++)
		{
			if (choppingBlock.Count > 0)
			{
				AIControllerNPC value = choppingBlock.Last.Value;
				choppingBlock.RemoveLast();
				activeNpcs.Push(value);
			}
			else
			{
				AIControllerNPC aIControllerNPC = inactiveNpcs.Pop();
				activeNpcs.Push(aIControllerNPC);
				Utils.ActivateTree(aIControllerNPC.gameObject, true);
				aIControllerNPC.ForceMoveToCurrentNode();
			}
		}
	}

	private void DeactivateNPCs(int count)
	{
		if (count > activeNpcs.Count)
		{
			CspUtils.DebugLog("Tried to deactivate " + count + " NPCs, but there were only " + activeNpcs.Count + " available.");
			return;
		}
		for (int i = 0; i < count; i++)
		{
			choppingBlock.AddLast(activeNpcs.Pop());
		}
	}
}
