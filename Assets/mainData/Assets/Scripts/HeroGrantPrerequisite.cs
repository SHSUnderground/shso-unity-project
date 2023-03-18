public class HeroGrantPrerequisite : ExpendablesPrerequisiteHandlerBase
{
	public override PrerequisiteCheckResult Check(ExpendablesManager manager, ExpendableDefinition def)
	{
		PrerequisiteCheckResult result = default(PrerequisiteCheckResult);
		result.State = PrerequisiteCheckStateEnum.Usable;
		result.StateExplanation = string.Empty;
		return result;
	}
}
