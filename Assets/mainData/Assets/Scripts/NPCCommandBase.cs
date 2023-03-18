using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public abstract class NPCCommandBase : INPCCommand
{
	protected bool interruptable = true;

	protected bool initialized;

	protected bool isDone;

	protected bool suspended;

	protected float startTime;

	protected AIControllerNPC aiController;

	protected NPCCommandManager manager;

	protected GameObject gameObject;

	protected BehaviorManager behaviorManager;

	public GameObject target;

	protected NPCCommandTypeEnum type;

	protected int commandSet;

	public bool Suspended
	{
		get
		{
			return suspended;
		}
	}

	public NPCCommandTypeEnum Type
	{
		get
		{
			return type;
		}
	}

	public int CommandSet
	{
		get
		{
			return commandSet;
		}
		set
		{
			commandSet = value;
		}
	}

	public bool Interruptable
	{
		get
		{
			return interruptable;
		}
	}

	public bool Initialized
	{
		get
		{
			return initialized;
		}
	}

	public NPCCommandBase()
	{
		type = NPCCommandTypeEnum.None;
	}

	public virtual void Init(NPCCommandManager manager)
	{
		this.manager = manager;
		gameObject = manager.gameObject;
		behaviorManager = (gameObject.GetComponent(typeof(BehaviorManager)) as BehaviorManager);
		aiController = (gameObject.GetComponent(typeof(AIControllerNPC)) as AIControllerNPC);
		initialized = true;
	}

	public virtual void Start()
	{
		Start(null);
	}

	public virtual void Start(params object[] initValues)
	{
		startTime = Time.time;
	}

	public virtual NPCCommandResultEnum Update()
	{
		throw new NotImplementedException();
	}

	public virtual void Suspend()
	{
		if (!interruptable)
		{
			throw new Exception("Can't suspend a non-interruptible NPC command.");
		}
		suspended = true;
	}

	public virtual void Resume()
	{
		suspended = false;
	}

	public virtual void Completed()
	{
		throw new NotImplementedException();
	}

	public static NPCCommandBase CreateCommand(NPCCommandHint hint)
	{
		Type type = NPCCommandHint.CommandList[hint.NPCCommand];
		NPCCommandBase nPCCommandBase = Activator.CreateInstance(type) as NPCCommandBase;
		if (nPCCommandBase != null)
		{
			nPCCommandBase.target = hint.target;
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				string empty = string.Empty;
				Type fieldType = fieldInfo.FieldType;
				if (!hint.storedPropertiesContainer.ContainsKey(fieldInfo.Name))
				{
					continue;
				}
				empty = hint.storedPropertiesContainer[fieldInfo.Name];
				if (System.Type.GetTypeCode(fieldType) == TypeCode.String)
				{
					fieldInfo.SetValue(nPCCommandBase, empty);
				}
				else if (!(fieldInfo.Name == "target"))
				{
					MethodInfo method = fieldType.GetMethod("Parse", new Type[1]
					{
						typeof(string)
					});
					if (method == null)
					{
						CspUtils.DebugLog(string.Format("Missing Parse() method while overriding property {0}.  (Unsupported type)", fieldInfo.Name));
						continue;
					}
					object value = method.Invoke(nPCCommandBase, new object[1]
					{
						empty
					});
					fieldInfo.SetValue(nPCCommandBase, value);
				}
			}
		}
		return nPCCommandBase;
	}

	public override string ToString()
	{
		return type.ToString();
	}
}
