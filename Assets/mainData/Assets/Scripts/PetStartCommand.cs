using UnityEngine;

public class PetStartCommand : PetCommandBase
{
	public PetStartCommand()
	{
		interruptable = false;
		type = PetCommandTypeEnum.Start;
	}

	public override void Start()
	{
		Start(null);
	}

	public override void Start(params object[] initValues)
	{
		gameObject.transform.position = target.transform.position + new Vector3(0f, 1.5f, 0f);
		PetFollowCharacterCommand petFollowCharacterCommand = new PetFollowCharacterCommand();
		petFollowCharacterCommand.target = target;
		manager.AddCommand(petFollowCharacterCommand);
	}

	public override void Suspend()
	{
		base.Suspend();
	}

	public override void Resume()
	{
		base.Resume();
	}

	public override void Completed()
	{
		base.Completed();
	}

	public override PetCommandResultEnum Update()
	{
		return PetCommandResultEnum.Completed;
	}

	public override string ToString()
	{
		return type.ToString();
	}
}
