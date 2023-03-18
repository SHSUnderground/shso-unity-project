using UnityEngine;

public class InteractableObject : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected enum State
	{
		Inactive,
		Proximity,
		Hover,
		Click,
		Emote
	}

	public bool playerOnly = true;

	public float maxInteractRange = 5f;

	private GameObject hoverGameObject;

	protected GameObject proximityRoot;

	protected GameObject modelRoot;

	protected GameObject hoverRoot;

	protected GameObject clickRoot;

	protected GameObject emoteRoot;

	protected State state;

	protected float timeProximityClear;

	protected float timeHoverClear;

	protected float timeClickClear;

	protected float timeEmoteClear;

	protected float ProximityTimeDelay = 0.5f;

	protected float HoverTimeDelay = 0.5f;

	protected float ClickTimeDelay = 0.5f;

	protected float EmoteTimeDelay = 0.5f;

	public void Start()
	{
		CspUtils.DebugLogError("InteractableObject component is deprecated <" + base.gameObject.name + ">, switch to an interactive object");
		foreach (Transform item in base.transform)
		{
			if (item.gameObject.name == "OnProximity")
			{
				proximityRoot = item.gameObject;
			}
			else if (item.gameObject.name == "OnHover")
			{
				hoverRoot = item.gameObject;
			}
			else if (item.gameObject.name == "OnClick")
			{
				clickRoot = item.gameObject;
			}
			else if (item.gameObject.name == "OnPowerEmote")
			{
				emoteRoot = item.gameObject;
			}
			else if (item.gameObject.name == "Model")
			{
				modelRoot = item.gameObject;
			}
		}
		Component[] componentsInChildren = base.gameObject.GetComponentsInChildren(typeof(InteractableObjectPlayAnimation), true);
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			InteractableObjectPlayAnimation interactableObjectPlayAnimation = (InteractableObjectPlayAnimation)array[i];
			interactableObjectPlayAnimation.Initialize(this, modelRoot);
		}
		ProximityTimeDelay = GetTreeLength(proximityRoot);
		ClickTimeDelay = GetTreeLength(clickRoot);
		EmoteTimeDelay = GetTreeLength(emoteRoot);
		state = State.Inactive;
		DeactivateTree(proximityRoot);
		DeactivateTree(hoverRoot);
		DeactivateTree(clickRoot);
		DeactivateTree(emoteRoot);
	}

	public void Update()
	{
		switch (state)
		{
		case State.Proximity:
			timeProximityClear -= Time.deltaTime;
			if (timeProximityClear <= 0f)
			{
				state = State.Inactive;
				DeactivateTree(proximityRoot);
			}
			break;
		case State.Hover:
			if (hoverGameObject != null)
			{
				GUIManager.Instance.CursorManager.SetCursorType((!rangeCheck(hoverGameObject.transform.position)) ? GUICursorManager.CursorType.Uninteractable : GUICursorManager.CursorType.Interactable);
			}
			if (hoverRoot != null)
			{
				timeHoverClear -= Time.deltaTime;
				if (timeHoverClear <= 0f)
				{
					state = State.Inactive;
					DeactivateTree(hoverRoot);
				}
			}
			break;
		case State.Click:
			timeClickClear -= Time.deltaTime;
			if (timeClickClear <= 0f)
			{
				state = State.Inactive;
				DeactivateTree(clickRoot);
				DeactivateTree(hoverRoot);
			}
			break;
		case State.Emote:
			timeEmoteClear -= Time.deltaTime;
			if (timeEmoteClear <= 0f)
			{
				state = State.Inactive;
				DeactivateTree(emoteRoot);
			}
			break;
		}
	}

	public void OnTriggerStay(Collider other)
	{
		SpawnData spawnData = other.gameObject.GetComponent(typeof(SpawnData)) as SpawnData;
		if (!playerOnly || (spawnData != null && (spawnData.spawnType & CharacterSpawn.Type.Player) != 0))
		{
			if (state == State.Inactive && proximityRoot != null)
			{
				state = State.Proximity;
				timeProximityClear = ProximityTimeDelay;
				ActivateTree(proximityRoot);
			}
			else if (state == State.Proximity)
			{
				timeProximityClear = ProximityTimeDelay;
			}
		}
	}

	public void OnMouseRolloverEnter(object data)
	{
		state = State.Hover;
		if (state == State.Inactive && hoverRoot != null)
		{
			timeHoverClear = HoverTimeDelay * 100f;
			ActivateTree(hoverRoot);
		}
		if (clickRoot != null)
		{
			MouseRollover mouseRollover = data as MouseRollover;
			if (mouseRollover.allowUserInput)
			{
				hoverGameObject = mouseRollover.character;
			}
		}
	}

	public void OnMouseRolloverExit()
	{
		if (state == State.Hover)
		{
			timeHoverClear = HoverTimeDelay;
			hoverGameObject = null;
		}
	}

	public bool OnMouseClick(GameObject player)
	{
		if (state != State.Click && clickRoot != null && rangeCheck(player.transform.position))
		{
			state = State.Click;
			timeClickClear = ClickTimeDelay;
			ActivateTree(clickRoot);
		}
		return true;
	}

	private bool rangeCheck(Vector3 distantPosition)
	{
		return Vector3.Distance(distantPosition, base.transform.position) < maxInteractRange;
	}

	public void OnPowerEmote(GameObject player)
	{
		if (state != State.Emote && emoteRoot != null)
		{
			state = State.Emote;
			timeEmoteClear = EmoteTimeDelay;
			ActivateTree(emoteRoot);
		}
	}

	protected void ActivateTree(GameObject obj)
	{
		if (!(obj == null))
		{
			Utils.ForEachTree(obj, delegate(GameObject o)
			{
				o.active = true;
				if (o.particleEmitter != null)
				{
					o.particleEmitter.emit = true;
				}
			});
		}
	}

	protected void DeactivateTree(GameObject obj)
	{
		if (!(obj == null))
		{
			Utils.ForEachTree(obj, delegate(GameObject o)
			{
				if (o.particleEmitter != null)
				{
					o.particleEmitter.emit = false;
				}
				else
				{
					o.active = false;
				}
			});
		}
	}

	protected float GetTreeLength(GameObject obj)
	{
		if (obj == null)
		{
			return 0f;
		}
		float time = 0f;
		Utils.ForEachTree(obj, delegate(GameObject o)
		{
			InteractableObjectPlayAnimation interactableObjectPlayAnimation = o.GetComponent(typeof(InteractableObjectPlayAnimation)) as InteractableObjectPlayAnimation;
			if ((bool)interactableObjectPlayAnimation)
			{
				time += interactableObjectPlayAnimation.GetLength();
			}
		});
		return time;
	}
}
