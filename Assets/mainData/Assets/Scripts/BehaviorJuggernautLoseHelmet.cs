public class BehaviorJuggernautLoseHelmet : BehaviorJuggernautChangeHelmetState
{
	public override void behaviorBegin()
	{
		changeHelmetAnimName = "recoil_helmet_start";
		changeHelmetSequence = "lose_helmet_sequence";
		nextBehavior = "BehaviorJuggernautVulnerable";
		attackingSuppressed = true;
		base.behaviorBegin();
	}
}
