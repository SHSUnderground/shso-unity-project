using System;

public class RevertRemoteState : PolymorphBaseState
{
	public RevertRemoteState(PolymorphStateProxy proxy)
		: base(proxy)
	{
	}

	public override Type GetNextState()
	{
		return typeof(RevertCharacterState);
	}
}
