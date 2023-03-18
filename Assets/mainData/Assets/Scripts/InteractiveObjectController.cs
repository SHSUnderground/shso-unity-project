using UnityEngine;

public class InteractiveObjectController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum CompletionStateEnum
	{
		Success,
		Canceled,
		Failed,
		Unknown
	}

	public delegate void OnDone(GameObject player, CompletionStateEnum completionState);

	protected InteractiveObject owner;

	protected GameObject model;

	public virtual void Initialize(InteractiveObject owner, GameObject model)
	{
		this.owner = owner;
		this.model = model;
	}

	public virtual InteractiveObject.StateIdx GetStateForPlayer(GameObject player)
	{
		return InteractiveObject.StateIdx.Enable;
	}

	public virtual bool CanPlayerUse(GameObject player)
	{
		return true;
	}

	public virtual bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		if (onDone != null)
		{
			onDone(player, CompletionStateEnum.Success);
		}
		return true;
	}

	public virtual void AttemptedInvalidUse(GameObject player)
	{
	}

	public virtual bool ShouldIgnoreMouseClick(GameObject player)
	{
		return false;
	}

	public virtual void OnRootChanged(InteractiveObject.StateIdx root, GameObject oldRoot, GameObject newRoot)
	{
	}
}
