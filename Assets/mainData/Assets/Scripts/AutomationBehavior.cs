using UnityEngine;

public class AutomationBehavior : IAutomationBehaviorAPI
{
	private CharacterGlobals charGlobals;

	private CharacterMotionController motionController;

	private CombatController combatController;

	private BehaviorManager behaviorManager;

	private GameObject gameObjectPlayer;

	private GameObject gameObjectEnemy;

	public static AutomationBehavior Instance
	{
		get
		{
			return new AutomationBehavior();
		}
	}

	public CombatController combatCtrl
	{
		get
		{
			return combatController;
		}
	}

	public CharacterMotionController motionCtrl
	{
		get
		{
			return motionController;
		}
	}

	public AutomationBehavior()
	{
		//Discarded unreachable code: IL_008a
		try
		{
			gameObjectPlayer = AutomationManager.Instance.LocalPlayer;
			gameObjectEnemy = AutomationManager.Instance.LocalEnemy;
			charGlobals = (gameObjectPlayer.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals);
			motionController = charGlobals.motionController;
			combatController = charGlobals.combatController;
			behaviorManager = charGlobals.behaviorManager;
		}
		catch
		{
			throw new AutomationExecuteException("LocalPlayer Object not found");
		}
	}

	public virtual bool move(Vector3 location)
	{
		bool result = true;
		if ((bool)gameObjectPlayer)
		{
			motionController.setDestination(location);
			behaviorManager.getBehavior().destinationChanged();
		}
		else
		{
			result = false;
		}
		return result;
	}

	public virtual bool teleport(Vector3 location)
	{
		bool result = true;
		if ((bool)gameObjectPlayer)
		{
			motionController.teleportTo(location);
			behaviorManager.getBehavior().destinationChanged();
		}
		else
		{
			result = false;
		}
		return result;
	}

	public virtual bool fight()
	{
		bool result = true;
		if ((bool)gameObjectPlayer)
		{
			if (combatController.IsObjectEnemy(gameObjectEnemy))
			{
				if (!combatController.beginAttack(gameObjectEnemy, false))
				{
					if (combatController.IsAttackAvailable(true))
					{
						combatController.pursueTarget(gameObjectEnemy, true);
					}
					else
					{
						combatController.pursueTarget(gameObjectEnemy, false);
					}
				}
				else
				{
					AutomationManager.Instance.moveInProgress = true;
				}
			}
			else
			{
				result = false;
			}
		}
		else
		{
			result = false;
		}
		return result;
	}

	public virtual bool fling()
	{
		return true;
	}

	public virtual bool throwat()
	{
		return true;
	}

	public virtual bool pickup(string objectName)
	{
		return Interact(objectName);
	}

	public virtual bool collect()
	{
		return true;
	}

	public virtual bool place()
	{
		return true;
	}

	public virtual bool jump()
	{
		bool result = true;
		try
		{
			motionController.jumpPressed();
			return result;
		}
		catch
		{
			return false;
		}
	}

	public virtual bool fly(string objectName)
	{
		return Interact(objectName);
	}

	public bool Interact(InteractiveObject o)
	{
		return o.OnMouseClick(AutomationManager.Instance.LocalPlayer);
	}

	public bool Interact(string interactiveObject)
	{
		InteractiveObject[] array = Utils.FindObjectsOfType<InteractiveObject>();
		bool result = false;
		try
		{
			InteractiveObject[] array2 = array;
			foreach (InteractiveObject interactiveObject2 in array2)
			{
				Vector3 position = interactiveObject2.transform.position;
				Vector3 position2 = AutomationManager.Instance.LocalPlayer.transform.position;
				if (interactiveObject.Trim().Equals(string.Empty))
				{
					CspUtils.DebugLog(AutomationManager.Instance.GetDistance(position.x, position.y, position.z, position2.x, position2.y, position2.z));
					if (AutomationManager.Instance.GetDistance(position.x, position.y, position.z, position2.x, position2.y, position2.z) < 10f)
					{
						interactiveObject2.OnMouseClick(AutomationManager.Instance.LocalPlayer);
						result = true;
					}
				}
				else if (interactiveObject2.name.ToString().Contains(interactiveObject) && AutomationManager.Instance.GetDistance(position.x, position.y, position.z, position2.x, position2.y, position2.z) < 3f)
				{
					interactiveObject2.OnMouseClick(AutomationManager.Instance.LocalPlayer);
					result = true;
				}
			}
			return result;
		}
		catch
		{
			return false;
		}
	}
}
