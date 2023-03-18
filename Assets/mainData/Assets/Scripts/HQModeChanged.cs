using System;

public class HQModeChanged : ShsEventMessage
{
	public Type modeType;

	public HQModeChanged(Type modeType)
	{
		this.modeType = modeType;
	}
}
