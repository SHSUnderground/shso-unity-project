using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(NPCCommandManager))]
public class AIControllerNPC : EmoteListener
{
	public enum NPCGender
	{
		Unknown,
		Male,
		Female
	}

	protected const float kMaxMotionlessTime = 30f;

	public static List<string> ReactionTable;

	protected NPCCommandManager commandManager;

	protected float lastMovedTime = Time.time;

	protected Vector3 lastMovedPos = Vector3.zero;

	private NPCPath assignedPath;

	private NPCPathNode currentNode;

	private Dictionary<string, string> specializedEmoteReactionList;

	[CompilerGenerated]
	private NPCGender _003CGender_003Ek__BackingField;

	public NPCGender Gender
	{
		[CompilerGenerated]
		get
		{
			return _003CGender_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CGender_003Ek__BackingField = value;
		}
	}

	public NPCPath AssignedPath
	{
		get
		{
			return assignedPath;
		}
		set
		{
			assignedPath = value;
		}
	}

	public NPCPathNode CurrentNode
	{
		get
		{
			return currentNode;
		}
		set
		{
			currentNode = value;
			commandManager.CommandSetCounter++;
		}
	}

	public INPCCommand CurrentCommand
	{
		get
		{
			return commandManager.CurrentCommand;
		}
	}

	public Dictionary<string, string> SpecializedEmoteReactionList
	{
		get
		{
			return specializedEmoteReactionList;
		}
	}

	public virtual bool DespawnOnExtendedIdle
	{
		get
		{
			return true;
		}
	}

	static AIControllerNPC()
	{
		ReactionTable = new List<string>();
		ReactionTable.Add("emote_approve");
		ReactionTable.Add("emote_cheer");
		ReactionTable.Add("emote_clap");
		ReactionTable.Add("emote_confused");
		ReactionTable.Add("emote_dance");
		ReactionTable.Add("emote_disapprove");
		ReactionTable.Add("emote_greet");
		ReactionTable.Add("emote_laugh");
		ReactionTable.Add("emote_rude");
		ReactionTable.Add("emote_scared");
		ReactionTable.Add("emote_shock");
		ReactionTable.Add("emote_surprise");
	}

	private void Awake()
	{
		commandManager = (GetComponent(typeof(NPCCommandManager)) as NPCCommandManager);
		specializedEmoteReactionList = new Dictionary<string, string>();
	}

	public virtual void Start()
	{
		NPCStartCommand command = new NPCStartCommand();
		commandManager.AddCommand(command);
		Gender = NPCGender.Unknown;
		Transform transform = Utils.FindNodeInChildren(base.transform, "export_node");
		if (transform != null)
		{
			Utils.ForEachTree(transform.gameObject, delegate(GameObject obj)
			{
				if (obj.name.Contains("female"))
				{
					Gender = NPCGender.Female;
				}
				else if (obj.name.Contains("male"))
				{
					Gender = NPCGender.Male;
				}
			});
		}
		else
		{
			CspUtils.DebugLog("NPC does not have a node named \"export_node\"; gender cannot be ascertained.");
		}
	}

	private void OnEnable()
	{
		StartCoroutine(CheckForMotion());
		SpawnData[] array = Object.FindObjectsOfType(typeof(SpawnData)) as SpawnData[];
		foreach (SpawnData spawnData in array)
		{
			if (spawnData.gameObject != base.gameObject)
			{
				Physics.IgnoreCollision(spawnData.collider, base.collider);
			}
		}
	}

	public virtual void InitializeFromData(DataWarehouse data)
	{
		foreach (DataWarehouse item in data.GetIterator("//emote_reactions/emote"))
		{
			string @string = item.GetString("command");
			string string2 = item.GetString("reaction");
			if (EmotesDefinition.Instance.GetEmoteByCommand(@string) == null)
			{
				CspUtils.DebugLog("Override for emote: " + @string + " invalid, emote doesn't exist.");
			}
			else if (string.IsNullOrEmpty(string2))
			{
				CspUtils.DebugLog("bad value for emote override; " + @string);
			}
			else
			{
				specializedEmoteReactionList[@string] = string2;
			}
		}
	}

	public virtual bool ConfigureCommandsFromNode(NPCPathNode node, bool setNodeAsCurrent)
	{
		bool flag = false;
		if (setNodeAsCurrent)
		{
			CurrentNode = node;
		}
		commandManager.CommandList.Clear();
		List<NPCCommandHint> commands = node.commands;
		foreach (NPCCommandHint item in commands)
		{
			if (flag)
			{
				CspUtils.DebugLog("Commands after a MoveToNode or MoveToNextNode are ignored. MoveTo* must be the last command specified");
				return flag;
			}
			if (item.NPCCommand != 0)
			{
				NPCCommandBase nPCCommandBase = NPCCommandBase.CreateCommand(item);
				commandManager.AddCommand(nPCCommandBase);
				if (nPCCommandBase is NPCMoveToNextNodeCommand || nPCCommandBase is NPCMoveToNodeCommand)
				{
					flag = true;
				}
			}
		}
		return flag;
	}

	public override void OnEmoteBroadcast(sbyte emoteID, GameObject broadcaster)
	{
		NPCReactCommand nPCReactCommand = new NPCReactCommand();
		nPCReactCommand.target = broadcaster;
		nPCReactCommand.emoteId = emoteID;
		nPCReactCommand.reactionType = NPCReactCommand.ReactionTypeEnum.Emote;
		commandManager.AddCommand(nPCReactCommand, true);
	}

	public virtual void OnInteract(GameObject player)
	{
		NPCReactCommand nPCReactCommand = new NPCReactCommand();
		nPCReactCommand.target = player;
		nPCReactCommand.reactionType = NPCReactCommand.ReactionTypeEnum.Interaction;
		commandManager.InsertQueuedCommand(nPCReactCommand);
	}

	public void OnStartled(object value)
	{
		if (!(value is KeyValuePair<sbyte, GameObject>))
		{
			CspUtils.DebugLog("invalid parameters passed into OnEmoteBroadcast");
			return;
		}
		KeyValuePair<sbyte, GameObject> keyValuePair = (KeyValuePair<sbyte, GameObject>)value;
		NPCReactCommand nPCReactCommand = new NPCReactCommand();
		nPCReactCommand.target = keyValuePair.Value;
		nPCReactCommand.emoteId = keyValuePair.Key;
		nPCReactCommand.reactionType = NPCReactCommand.ReactionTypeEnum.TroubleBotDestruction;
		commandManager.AddCommand(nPCReactCommand, true);
	}

	public void Wait()
	{
		NPCWaitCommand command = new NPCWaitCommand();
		commandManager.AddCommand(command, true);
	}

	public void StopWaiting()
	{
		NPCWaitCommand nPCWaitCommand = commandManager.CurrentCommand as NPCWaitCommand;
		if (nPCWaitCommand != null)
		{
			nPCWaitCommand.Finish();
		}
	}

	public void ForceMoveToCurrentNode()
	{
		commandManager.ForceCommand(new NPCMoveToCurrentNodeCommand());
	}

	protected IEnumerator CheckForMotion()
	{
		lastMovedPos = base.transform.position;
		lastMovedTime = Time.time;
		while (DespawnOnExtendedIdle)
		{
			if ((base.transform.position - lastMovedPos).sqrMagnitude > 0.1f || CurrentCommand is NPCReactCommand || CurrentCommand is NPCWaitCommand)
			{
				lastMovedPos = base.transform.position;
				lastMovedTime = Time.time;
			}
			else if (Time.time - lastMovedTime > 30f)
			{
				CspUtils.DebugLog(string.Format("NPC <{0}> is motionless; current command: {1}", base.gameObject.name, commandManager.CurrentCommand));
				NPCDerezCommand derez = new NPCDerezCommand();
				derez.respawn = true;
				derez.rezDelay = 0f;
				derez.fadeTime = 0f;
				commandManager.ForceCommand(derez);
			}
			yield return new WaitForSeconds(1f);
		}
	}
}
