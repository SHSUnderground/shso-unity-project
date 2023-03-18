using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(PetCommandManager))]
public class AIControllerPet : EmoteListener
{
	public enum NPCGender
	{
		Unknown,
		Male,
		Female
	}

	protected const float kMaxMotionlessTime = 30f;

	public static List<string> ReactionTable;

	protected PetCommandManager commandManager;

	public GameObject target;

	public PetData petData;

	protected float lastMovedTime = Time.time;

	protected Vector3 lastMovedPos = Vector3.zero;

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

	public IPetCommand CurrentCommand
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

	static AIControllerPet()
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
		commandManager = (GetComponent(typeof(PetCommandManager)) as PetCommandManager);
		specializedEmoteReactionList = new Dictionary<string, string>();
	}

	public virtual void Start()
	{
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
		PetStartCommand petStartCommand = new PetStartCommand();
		petStartCommand.target = target;
		commandManager.AddCommand(petStartCommand, true);
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

	public override void OnEmoteBroadcast(sbyte emoteID, GameObject broadcaster)
	{
	}

	public virtual void OnInteract(GameObject player)
	{
		CspUtils.DebugLog("Pet - player interacted with us");
	}

	public void OnStartled(object value)
	{
		CspUtils.DebugLog("Pet - OnStartled");
	}

	public void Wait()
	{
		CspUtils.DebugLog("AIControllerPet got a Wait() command, which shouldn't happen?");
	}

	public void StopWaiting()
	{
		CspUtils.DebugLog("AIControllerPet got a StopWaiting() command, which shouldn't happen?");
	}

	protected IEnumerator CheckForMotion()
	{
		lastMovedPos = base.transform.position;
		lastMovedTime = Time.time;
		while (DespawnOnExtendedIdle)
		{
			if ((base.transform.position - lastMovedPos).sqrMagnitude > 0.1f || CurrentCommand is PetWaitForCharacterMoveCommand)
			{
				lastMovedPos = base.transform.position;
				lastMovedTime = Time.time;
			}
			else if (Time.time - lastMovedTime > 30f)
			{
				CspUtils.DebugLog(string.Format("Pet <{0}> is motionless; current command: {1}", base.gameObject.name, commandManager.CurrentCommand));
				Object.Destroy(base.gameObject);
			}
			yield return new WaitForSeconds(1f);
		}
	}
}
