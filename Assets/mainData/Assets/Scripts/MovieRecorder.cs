using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

[AddComponentMenu("Lab/Misc/Movie Recorder")]
public class MovieRecorder : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string FileNamePrefix;

	public int FramesPerSecond = 30;

	public float ClipLength;

	public bool CustomResolution;

	public int ResolutionX = 1280;

	public int ResolutionY = 720;

	public bool Alpha = true;

	public int MaxFrameBuffers = 8;

	private bool _Recording;

	private Stack<Texture2D> _TextureCache = new Stack<Texture2D>();

	private int _TextureCacheAllocated;

	private int _TextureCacheSizeX;

	private int _TextureCacheSizeY;

	private bool _TextureCacheAlpha = true;

	private void Update()
	{
		if (!_Recording)
		{
			StartCoroutine(RecordMovie());
		}
	}

	private IEnumerator RecordMovie()
	{
		_Recording = true;
		Directory.CreateDirectory(Path.GetDirectoryName(FileNamePrefix));
		Application.runInBackground = true;
		Time.captureFramerate = FramesPerSecond;
		float stopTime = Time.time + ClipLength;
		int frameNumber = 0;
		int resX = (!CustomResolution) ? Screen.width : ResolutionX;
		int resY = (!CustomResolution) ? Screen.height : ResolutionY;
		int queueCount = 0;
		RenderTexture target = (!CustomResolution) ? null : new RenderTexture(resX, resY, 24);
		while (base.enabled && (ClipLength <= 0f || Time.time < stopTime))
		{
			yield return new WaitForEndOfFrame();
			try
			{
				Texture2D frame = GetTexture(resX, resY);
				CaptureFrame(frame, target);
				string filename = FileNamePrefix + frameNumber.ToString("D6") + ".png";
				Interlocked.Increment(ref queueCount);
				SaveTexture(frame, filename);
				RecycleTexture(frame);
				Interlocked.Decrement(ref queueCount);
			}
			catch
			{
				ClearTextures();
				Time.captureFramerate = 0;
				base.enabled = false;
				_Recording = false;
				throw;
			}
			frameNumber++;
		}
		if (target != null)
		{
			Object.Destroy(target);
		}
		Time.captureFramerate = 0;
		while (queueCount > 0)
		{
			yield return null;
		}
		ClearTextures();
		base.enabled = false;
		_Recording = false;
	}

	private void CaptureFrame(Texture2D buffer, RenderTexture target)
	{
		Camera camera = base.camera ?? Camera.main;
		Rect source = camera.pixelRect;
		if (target != null)
		{
			camera.targetTexture = target;
			camera.Render();
			camera.targetTexture = null;
			source = new Rect(0f, 0f, target.width, target.height);
			RenderTexture.active = target;
		}
		buffer.ReadPixels(source, 0, 0, false);
		buffer.Apply();
		if (target != null)
		{
			RenderTexture.active = null;
		}
	}

	private static void SaveTexture(Texture2D buffer, string filename)
	{
		byte[] buffer2 = buffer.EncodeToPNG();
		using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(filename, FileMode.Create)))
		{
			binaryWriter.Write(buffer2);
		}
	}

	private Texture2D GetTexture(int resX, int resY)
	{
		Texture2D texture2D = null;
		while (texture2D == null)
		{
			lock (_TextureCache)
			{
				if (_TextureCacheSizeX != resX || _TextureCacheSizeY != resY || _TextureCacheAlpha != Alpha)
				{
					ClearTextures();
					_TextureCacheSizeX = resX;
					_TextureCacheSizeY = resY;
					_TextureCacheAlpha = Alpha;
				}
				if (_TextureCache.Count == 0)
				{
					if (_TextureCacheAllocated >= MaxFrameBuffers)
					{
						Thread.Sleep(0);
					}
					else
					{
						Interlocked.Increment(ref _TextureCacheAllocated);
						CspUtils.DebugLog("Allocating texture " + _TextureCacheAllocated + " out of " + MaxFrameBuffers);
						texture2D = new Texture2D(resX, resY, (!Alpha) ? TextureFormat.RGB24 : TextureFormat.ARGB32, false);
					}
				}
				else
				{
					texture2D = _TextureCache.Pop();
				}
			}
		}
		return texture2D;
	}

	private void RecycleTexture(Texture2D texture)
	{
		lock (_TextureCache)
		{
			_TextureCache.Push(texture);
		}
	}

	private void ClearTextures()
	{
		lock (_TextureCache)
		{
			_TextureCache.Clear();
			_TextureCacheAllocated = 0;
		}
	}
}
