using System.Collections.Generic;

public class CaptureManager
{
	public List<ICaptureHandler> captureHandlers = new List<ICaptureHandler>();

	public void AddCaptureHandler(ICaptureHandler handler)
	{
		if (handler != null && !captureHandlers.Contains(handler))
		{
			captureHandlers.Add(handler);
			handler.OnCaptureAcquired();
		}
	}

	public void AddCaptureHandlers(ICaptureHandler[] handlers)
	{
		ICaptureHandler captureHandler = null;
		if (handlers == null || handlers.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < handlers.Length; i++)
		{
			captureHandler = handlers[i];
			if (captureHandler != null && !captureHandlers.Contains(captureHandler))
			{
				captureHandlers.Add(captureHandler);
				captureHandler.OnCaptureAcquired();
			}
		}
	}

	public void RemoveCaptureHandler(ICaptureHandler handlerToRemove)
	{
		if (captureHandlers.Contains(handlerToRemove))
		{
			handlerToRemove.OnCaptureUnacquired();
			captureHandlers.Remove(handlerToRemove);
			CspUtils.DebugLog("Removed " + handlerToRemove.ToString() + ". Capture Manager now owns:");
			foreach (ICaptureHandler captureHandler in captureHandlers)
			{
				CspUtils.DebugLog(captureHandler.ToString());
			}
		}
	}

	public void RemoveAllCaptureHandlers()
	{
		foreach (ICaptureHandler captureHandler in captureHandlers)
		{
			captureHandler.OnCaptureUnacquired();
		}
		captureHandlers.Clear();
	}

	public bool InputAllowed(SHSKeyCode keyCode)
	{
		bool result = false;
		for (int i = 0; i < captureHandlers.Count; i++)
		{
			if (captureHandlers[i].HandleCapture(keyCode) == CaptureHandlerResponse.Passthrough)
			{
				result = true;
			}
		}
		return result;
	}
}
