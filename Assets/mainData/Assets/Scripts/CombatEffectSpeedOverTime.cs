using UnityEngine;

public class CombatEffectSpeedOverTime : CombatEffectBase
{
	protected enum Segment
	{
		Start,
		Mid,
		End
	}

	protected float curSpeedMutliplier;

	protected float curSpeedDelta;

	protected float startTime;

	protected float midTime;

	protected float endTime;

	protected Segment curSegment;

	public override void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		startTime = Time.time;
		float num = (combatEffectData.speedMultiplierTime - combatEffectData.speedMultiplierMidTime) / 2f;
		midTime = startTime + num;
		endTime = midTime + combatEffectData.speedMultiplierMidTime;
		curSpeedMutliplier = combatEffectData.speedStartMultiplier;
		curSegment = Segment.Start;
		curSpeedDelta = (combatEffectData.speedMidMultiplier - combatEffectData.speedStartMultiplier) / num;
		charGlobals.motionController.addSpeedMultiplier(curSpeedMutliplier);
	}

	private void Update()
	{
		Segment segment = Segment.Start;
		if (Time.time >= endTime)
		{
			segment = Segment.End;
		}
		else if (Time.time >= midTime)
		{
			segment = Segment.Mid;
		}
		float num = curSpeedMutliplier;
		if (curSegment != segment)
		{
			float num2 = Time.time - startTime;
			switch (segment)
			{
			case Segment.Mid:
				curSpeedMutliplier = combatEffectData.speedMidMultiplier;
				curSpeedDelta = 0f;
				break;
			case Segment.End:
			{
				curSpeedMutliplier = combatEffectData.speedMidMultiplier;
				float num3 = combatEffectData.speedMultiplierTime - num2;
				curSpeedDelta = (combatEffectData.speedEndMultiplier - combatEffectData.speedMidMultiplier) / num3;
				break;
			}
			}
		}
		else
		{
			curSpeedMutliplier += curSpeedDelta * Time.deltaTime;
		}
		curSegment = segment;
		if (num != curSpeedMutliplier)
		{
			charGlobals.motionController.removeSpeedMultiplier(num);
			charGlobals.motionController.addSpeedMultiplier(curSpeedMutliplier);
		}
	}

	protected override void ReleaseEffect()
	{
		charGlobals.motionController.removeSpeedMultiplier(curSpeedMutliplier);
		base.ReleaseEffect();
	}
}
