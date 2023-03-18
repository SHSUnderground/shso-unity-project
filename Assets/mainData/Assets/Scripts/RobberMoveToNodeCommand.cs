public class RobberMoveToNodeCommand : NPCMoveToNodeCommand
{
	private NPCPathNode nodeToLeave;

	private SHSRobberRallyActivity parentActivity;

	public NPCPathNode NodeToLeave
	{
		set
		{
			nodeToLeave = value;
		}
	}

	public SHSRobberRallyActivity ParentActivity
	{
		set
		{
			parentActivity = value;
		}
	}

	public override void Start()
	{
		ignorePathingChecks = true;
		base.Start();
		RobberPathNode robberPathNode = nodeToLeave as RobberPathNode;
		if (robberPathNode != null && !string.IsNullOrEmpty(robberPathNode.overrideAnimationName))
		{
			behaviorManager.RemoveAnimationOverride("movement_idle", robberPathNode.overrideAnimationName);
		}
		if (parentActivity != null && nodeToLeave != null)
		{
			parentActivity.OnRobberLeaveNode(nodeToLeave);
		}
	}
}
