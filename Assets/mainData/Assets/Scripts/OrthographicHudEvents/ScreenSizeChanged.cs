namespace OrthographicHudEvents
{
	public class ScreenSizeChanged : ShsEventMessage
	{
		public int width;

		public int height;

		public float aspectRatio;

		public float orthoSize;

		public ScreenSizeChanged(int width, int height, float aspectRatio, float orthoSize)
		{
			this.width = width;
			this.height = height;
			this.aspectRatio = aspectRatio;
			this.orthoSize = orthoSize;
		}
	}
}
