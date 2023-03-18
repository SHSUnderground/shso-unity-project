public class TrackedMusicSource
{
	public MusicSource Source;

	public bool DiscardOnSceneTransition = true;

	public TrackedMusicSource(MusicSource source, bool discardOnSceneTransition)
	{
		Source = source;
		DiscardOnSceneTransition = discardOnSceneTransition;
	}
}
