using UnityEngine;

public class MovieMaker : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private const int recordingFrameRate = 25;

	private string runningMovieFolder;

	private bool recording = true;

	public bool Recording
	{
		get
		{
			return recording;
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (recording)
		{
			string filename = string.Format("{0}\\ss_{1:D5}.png", runningMovieFolder, Time.frameCount);
			Application.CaptureScreenshot(filename);
		}
	}

	private void OnGUI()
	{
	}

	public void StartRecording()
	{
	}

	public void StopRecording()
	{
	}
}
