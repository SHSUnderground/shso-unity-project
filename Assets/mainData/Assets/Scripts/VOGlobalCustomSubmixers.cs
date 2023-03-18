using System.Collections.Generic;

public class VOGlobalCustomSubmixers
{
	public static void AddGlobalCustomSubmixers(Dictionary<string, IVOMixer> submixersDestination, IVOMixer output)
	{
		LinkSubmix(submixersDestination, output, "mission_start_queue", new VOMissionBriefingSubmixer());
	}

	protected static void LinkSubmix(Dictionary<string, IVOMixer> submixersDestination, IVOMixer output, string submixerName, IVOMixer submixer)
	{
		submixer.SetOutput(output);
		submixersDestination[submixerName] = submixer;
	}
}
