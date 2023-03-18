public class LevelupPrerequisite : ExpendablesPrerequisiteHandlerDefault
{
	public override PrerequisiteCheckResult Check(ExpendablesManager manager, ExpendableDefinition def)
	{
		PrerequisiteCheckResult result = base.Check(manager, def);
		if (result.State == PrerequisiteCheckStateEnum.Usable)
		{
			UserProfile profile = AppShell.Instance.Profile;
			if (profile.AvailableCostumes[profile.SelectedCostume].Level >= profile.AvailableCostumes[profile.SelectedCostume].MaxLevel)
			{
				PrerequisiteCheckResult result2 = default(PrerequisiteCheckResult);
				result2.State = PrerequisiteCheckStateEnum.CustomStateInvalid;
				result2.StateExplanation = "#EXP_MAXLEVEL_INVALID";
				return result2;
			}
		}
		return result;
	}
}
