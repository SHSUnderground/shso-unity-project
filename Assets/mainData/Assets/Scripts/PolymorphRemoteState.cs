using System;

public class PolymorphRemoteState : PolymorphBaseState
{
	public PolymorphRemoteState(PolymorphStateProxy proxy)
		: base(proxy)
	{
	}

	public override void Update()
	{
		base.Update();
		if (base.RemotePolymorphEnabled)
		{
			if (base.stateData.Polymorph != null)
			{
				done = Utils.IsCharacterSpawned(base.stateData.PolymorphObject);
			}
			else
			{
				done = (base.stateData.PolymorphObject != null);
			}
		}
	}

	public override Type GetNextState()
	{
		return typeof(PolymorphCharacterState);
	}
}
