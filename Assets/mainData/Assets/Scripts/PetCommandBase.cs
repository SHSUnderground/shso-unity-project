using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public abstract class PetCommandBase : IPetCommand
{
	protected bool interruptable = true;

	protected bool initialized;

	protected bool isDone;

	protected bool suspended;

	protected float startTime;

	protected AIControllerPet aiController;

	protected PetCommandManager manager;

	protected GameObject gameObject;

	protected BehaviorManager behaviorManager;

	public GameObject target;

	protected PetCommandTypeEnum type;

	protected int commandSet;

	public bool Suspended
	{
		get
		{
			return suspended;
		}
	}

	public PetCommandTypeEnum Type
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

	public PetCommandBase()
	{
		type = PetCommandTypeEnum.None;
	}

	public virtual void Init(PetCommandManager manager)
	{
		this.manager = manager;
		gameObject = manager.gameObject;
		behaviorManager = (gameObject.GetComponent(typeof(BehaviorManager)) as BehaviorManager);
		aiController = (gameObject.GetComponent(typeof(AIControllerPet)) as AIControllerPet);
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

	public virtual PetCommandResultEnum Update()
	{
		throw new NotImplementedException();
	}

	public virtual void Suspend()
	{
		if (!interruptable)
		{
			throw new Exception("Can't suspend a non-interruptible Pet command.");
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

	public static PetCommandBase CreateCommand(PetCommandHint hint)
	{
		Type type = PetCommandHint.CommandList[hint.PetCommand];
		PetCommandBase petCommandBase = Activator.CreateInstance(type) as PetCommandBase;
		if (petCommandBase != null)
		{
			petCommandBase.target = hint.target;
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
					fieldInfo.SetValue(petCommandBase, empty);
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
					object value = method.Invoke(petCommandBase, new object[1]
					{
						empty
					});
					fieldInfo.SetValue(petCommandBase, value);
				}
			}
		}
		return petCommandBase;
	}

	public override string ToString()
	{
		return type.ToString();
	}
}
