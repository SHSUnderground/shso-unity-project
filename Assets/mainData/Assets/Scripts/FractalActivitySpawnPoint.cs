public class FractalActivitySpawnPoint : ActivitySpawnPoint
{
	public enum FractalType
	{
		Fractal = 100,
		SurvivalModeSinglePlayer,
		SurvivalModeMultiplayer,
		DinosaurBone
	}

	public bool onlyAccessibleToFlyingHeroes;

	public bool onlyAccessibleToWallCrawlHeroes;

	public float minimumTotalJumpHeight = -1f;

	private int assignedFractalIndex;

	public FractalType fractalType = FractalType.Fractal;

	public int AssignedFractalIndex
	{
		get
		{
			return assignedFractalIndex;
		}
		set
		{
			assignedFractalIndex = value;
		}
	}

	public bool CanCharacterReach(CharacterGlobals character)
	{
		if (onlyAccessibleToFlyingHeroes && (character.motionController.hotSpotType & HotSpotType.Style.Flying) == 0)
		{
			return false;
		}
		if (onlyAccessibleToWallCrawlHeroes && (character.motionController.hotSpotType & HotSpotType.Style.Web) == 0)
		{
			return false;
		}
		if (minimumTotalJumpHeight > 0f && GetTotalCharacterJumpHeight(character) < minimumTotalJumpHeight)
		{
			return false;
		}
		return true;
	}

	private float GetTotalCharacterJumpHeight(CharacterGlobals character)
	{
		float num = character.motionController.jumpHeight;
		if (character.motionController.doubleJump)
		{
			num += character.motionController.secondJumpHeight;
		}
		return num;
	}
}
