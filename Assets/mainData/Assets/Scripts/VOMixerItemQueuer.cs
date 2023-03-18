using System.Collections.Generic;

public class VOMixerItemQueuer : IVOMixer
{
	protected IVOMixer mainOutput;

	protected Dictionary<VOAction, IVOMixer> queueOutputs = new Dictionary<VOAction, IVOMixer>();

	public void Update()
	{
		foreach (KeyValuePair<VOAction, IVOMixer> queueOutput in queueOutputs)
		{
			queueOutput.Value.Update();
		}
	}

	public void SetOutput(IVOMixer output)
	{
		mainOutput = output;
		foreach (KeyValuePair<VOAction, IVOMixer> queueOutput in queueOutputs)
		{
			queueOutput.Value.SetOutput(mainOutput);
		}
	}

	public void SendVO(IVOMixerItem item)
	{
		if (item.Action.Routing.queuesSelf)
		{
			IVOMixer value;
			if (!queueOutputs.TryGetValue(item.Action.VOAction, out value))
			{
				value = CreateQueue(item.Action.VOAction);
			}
			value.SendVO(item);
		}
		else if (mainOutput != null)
		{
			mainOutput.SendVO(item);
		}
	}

	protected IVOMixer CreateQueue(VOAction action)
	{
		VOQueueMixer vOQueueMixer = new VOQueueMixer();
		vOQueueMixer.SetOutput(mainOutput);
		queueOutputs[action] = vOQueueMixer;
		return vOQueueMixer;
	}
}
