public class GUICaptureMessage : ShsEventMessage
{
	public readonly ICaptureHandler Handler;

	public GUICaptureMessage(ICaptureHandler Handler)
	{
		this.Handler = Handler;
	}
}
