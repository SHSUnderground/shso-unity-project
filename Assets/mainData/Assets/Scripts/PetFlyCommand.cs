using UnityEngine;

public class PetFlyCommand : PetCommandBase
{
	public class PetFlightEndedEvent : ShsEventMessage
	{
		public GameObject Pet;

		public PetFlightEndedEvent(GameObject Pet)
		{
			this.Pet = Pet;
		}
	}

	private bool wasSuspended;

	public PetFlyCommand()
	{
		type = PetCommandTypeEnum.Fly;
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
			else
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
		AppShell.Instance.EventMgr.RemoveListener<PetFlightEndedEvent>(OnPetFlightEnded);
	}

	public override void Resume()
	{
		base.Resume();
		AppShell.Instance.EventMgr.AddListener<PetFlightEndedEvent>(OnPetFlightEnded);
	}

	protected void OnPetFlightEnded(PetFlightEndedEvent e)
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
