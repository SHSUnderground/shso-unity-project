public class FeatherActivitySpawnPoint : ActivitySpawnPoint
{
	public bool onlyAccessibleToFlyingHeroes;

	public bool onlyAccessibleToWallCrawlHeroes;

	public float minimumTotalJumpHeight = -1f;

	private int assignedFeatherIndex;

	public int AssignedFeatherIndex
	{
		get
		{
			return assignedFeatherIndex;
		}
		set
		{
			assignedFeatherIndex = value;
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
