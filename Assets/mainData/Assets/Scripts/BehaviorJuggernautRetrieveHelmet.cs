public class BehaviorJuggernautRetrieveHelmet : BehaviorJuggernautChangeHelmetState
{
	public override void behaviorBegin()
	{
		changeHelmetAnimName = "recoil_helmet_end";
		changeHelmetSequence = "retrieve_helmet_sequence";
		nextBehavior = "BehaviorMovement";
		attackingSuppressed = false;
		base.behaviorBegin();
	}
}
