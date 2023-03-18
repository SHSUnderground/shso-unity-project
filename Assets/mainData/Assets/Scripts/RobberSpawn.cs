public class RobberSpawn : NPCSpawn
{
	public ShsAudioSource startSFX;

	public ShsAudioSource caughtSFX;

	public string caughtAnimation;

	public string gotAwayAnimation;

	public string caughtEffectName;

	public string gotAwayEffectName;

	public bool StareAtPlayer = true;

	public bool StareAtCameraWhileInactive = true;

	public EffectSequence despawnEffect;

	public float robberMoveSpeed = AIControllerRobber.kDefaultRobberMoveSpeed;

	public bool consideredHuman;

	private static int defaultNameIndex = 0;

	private static string[] defaultNames = new string[6]
	{
		"Sneaky",
		"Shifty",
		"Sketchy",
		"Sly",
		"Garrett",
		"Hamburglar"
	};

	public override string GetNextDefaultName()
	{
		if (defaultNameIndex >= defaultNames.Length)
		{
			defaultNameIndex = 0;
		}
		return defaultNames[defaultNameIndex++];
	}
}
