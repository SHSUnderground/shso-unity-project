public class AIControllerHQTutorial : AIControllerHQ
{
	protected bool canUseItems;

	public bool CanUseItems
	{
		set
		{
			canUseItems = value;
		}
	}

	protected override float CurrentHunger
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public override void SaveHungerValue()
	{
	}

	protected override void ChooseActivity()
	{
		if (canUseItems)
		{
			base.ChooseActivity();
		}
		else
		{
			DoDefaultBehavior();
		}
	}
}
