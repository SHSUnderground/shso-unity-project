public interface ICaptureHandler
{
	ICaptureManager Manager
	{
		get;
		set;
	}

	CaptureHandlerResponse HandleCapture(SHSKeyCode code);

	void OnCaptureAcquired();

	void OnCaptureUnacquired();
}
