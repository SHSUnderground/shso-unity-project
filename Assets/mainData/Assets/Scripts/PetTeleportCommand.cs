using UnityEngine;

public class PetTeleportCommand : PetCommandBase
{
	public class PetTeleportEndedEvent : ShsEventMessage
	{
		public GameObject Pet;

		public PetTeleportEndedEvent(GameObject Pet)
		{
			this.Pet = Pet;
		}
	}

	private bool wasSuspended;

	public PetTeleportCommand()
	{
		type = PetCommandTypeEnum.Teleport;
	}

	public override void Start()
	{
		Start(null);
		Animation component = Utils.GetComponent<Animation>(gameObject, Utils.SearchChildren);
		if (component != null)
		{
			if (component["movement_idle_sidekick"] != null)
			{
				component.Play("movement_idle_sidekick");
				component["movement_idle_sidekick"].wrapMode = WrapMode.Loop;
			}
			else if (component["movement_idle"] != null)
			{
				component.Play("movement_idle");
				component["movement_idle"].wrapMode = WrapMode.Loop;
			}
		}
	}

	public override void Start(params object[] initValues)
	{
		Resume();
	}

	public override void Suspend()
	{
		base.Suspend();
		wasSuspended = true;
		AppShell.Instance.EventMgr.RemoveListener<PetTeleportEndedEvent>(OnPetTeleportEnded);
	}

	public override void Resume()
	{
		base.Resume();
		AppShell.Instance.EventMgr.AddListener<PetTeleportEndedEvent>(OnPetTeleportEnded);
	}

	protected void OnPetTeleportEnded(PetTeleportEndedEvent e)
	{
		if (e.Pet.GetInstanceID() == gameObject.GetInstanceID())
		{
			Suspend();
		}
	}

	public override void Completed()
	{
		base.Completed();
	}

	public override PetCommandResultEnum Update()
	{
		if (wasSuspended)
		{
			return PetCommandResultEnum.Completed;
		}
		return PetCommandResultEnum.InProgress;
	}

	public override string ToString()
	{
		return base.ToString() + " (" + ((!(target != null)) ? "null" : target.gameObject.name) + ") ";
	}
}
