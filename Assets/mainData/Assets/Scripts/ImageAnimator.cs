using UnityEngine;

public class ImageAnimator : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public int rows = 2;

	public int columns = 4;

	public int frameCount = 8;

	public float framesPerSecond = 8f;

	public bool loop;

	private float uStride;

	private float vStride;

	private float secondsPerFrame;

	private float timeUntilNextFrame;

	private int currentFrame;

	private void Start()
	{
		uStride = 1f / (float)columns;
		vStride = 1f / (float)rows;
		secondsPerFrame = 1f / framesPerSecond;
		base.renderer.material.mainTextureScale = new Vector2(uStride, vStride);
	}

	private void Update()
	{
		timeUntilNextFrame -= Time.deltaTime;
		if (timeUntilNextFrame < 0f)
		{
			if (loop)
			{
				currentFrame = (currentFrame + 1) % frameCount;
			}
			else
			{
				currentFrame = Mathf.Min(currentFrame + 1, frameCount - 1);
			}
			float num = Mathf.Floor(currentFrame / columns);
			float num2 = (float)currentFrame - num * (float)columns;
			Vector2 mainTextureOffset = new Vector2(uStride * num2, vStride * ((float)rows - num - 1f));
			base.renderer.material.mainTextureOffset = mainTextureOffset;
			timeUntilNextFrame += secondsPerFrame;
		}
	}

	private void OnEnable()
	{
		currentFrame = 0;
		timeUntilNextFrame = secondsPerFrame;
		base.renderer.material.mainTextureOffset = new Vector2(0f, vStride * (float)(rows - 1));
	}
}
