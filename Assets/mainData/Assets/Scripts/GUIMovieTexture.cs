using UnityEngine;

public class GUIMovieTexture : GUIDrawTexture
{
	public enum PlayModeEnum
	{
		Streaming,
		Bundle
	}

	private WWW movieFetchRequest;

	private MovieTexture movieTexture;

	private bool fetchOnNextUpdate;

	private string movieUri = "http://www.unity3d.com/webplayers/Movie/sample.ogg";

	private PlayModeEnum playMode;

	private bool isMovieInitializing;

	private bool loop;

	private bool autoPlay;

	public string MovieUri
	{
		get
		{
			return movieUri;
		}
		set
		{
			reset();
			movieUri = value;
			fetchOnNextUpdate = true;
			playMode = PlayModeEnum.Streaming;
		}
	}

	public override string TextureSource
	{
		set
		{
			reset();
			fetchOnNextUpdate = true;
			playMode = PlayModeEnum.Bundle;
			base.TextureSource = value;
		}
	}

	public PlayModeEnum PlayMode
	{
		get
		{
			return playMode;
		}
	}

	public bool IsMovieInitializing
	{
		get
		{
			return isMovieInitializing;
		}
	}

	public bool IsMoviePlaying
	{
		get
		{
			return movieTexture != null && movieTexture.isPlaying;
		}
	}

	public bool Loop
	{
		get
		{
			return loop;
		}
		set
		{
			loop = value;
		}
	}

	public bool AutoPlay
	{
		get
		{
			return autoPlay;
		}
		set
		{
			autoPlay = value;
		}
	}

	public override void OnShow()
	{
	}

	public override void OnHide()
	{
		base.OnHide();
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (fetchOnNextUpdate)
		{
			fetchOnNextUpdate = false;
			if (PlayMode == PlayModeEnum.Streaming)
			{
				movieFetchRequest = new WWW(movieUri);
				movieTexture = movieFetchRequest.movie;
				isMovieInitializing = true;
				resourcesInitialized = true;
				isLoading = true;
			}
		}
		if (movieTexture == null)
		{
			return;
		}
		if (isMovieInitializing)
		{
			if (movieTexture.isReadyToPlay)
			{
				isLoading = false;
				base.Texture = movieTexture;
				movieTexture.loop = loop;
				if (autoPlay)
				{
					movieTexture.Play();
				}
				isMovieInitializing = false;
			}
		}
		else if (!IsMoviePlaying && movieTexture != null && loop)
		{
			movieTexture.loop = true;
			movieTexture.Play();
		}
	}

	public void Play()
	{
		if (!IsMoviePlaying && movieTexture != null && movieTexture.isReadyToPlay)
		{
			movieTexture.Stop();
			movieTexture.Play();
		}
	}

	public void Pause()
	{
		if (IsMoviePlaying)
		{
			movieTexture.Pause();
		}
	}

	public void Resume()
	{
		if (!IsMoviePlaying)
		{
			movieTexture.Play();
		}
	}

	public void Stop()
	{
		if (movieTexture != null && movieTexture.isPlaying)
		{
			movieTexture.Stop();
		}
	}

	private void reset()
	{
		if (movieTexture != null)
		{
			if (movieTexture.isPlaying)
			{
				movieTexture.Stop();
			}
			movieTexture = null;
		}
		resourcesInitialized = false;
	}

	protected override void loadTexture()
	{
		if (Traits.ResourceLoadingTrait == ControlTraits.ResourceLoadingTraitEnum.Sync)
		{
			texture = GUIManager.Instance.LoadMovieTexture(textureSource);
			if (autoSizeToTexture)
			{
				initTextureSize();
			}
			resourcesInitialized = (texture != null);
			movieTexture = (MovieTexture)texture;
			isMovieInitializing = true;
		}
		else if (Traits.ResourceLoadingTrait == ControlTraits.ResourceLoadingTraitEnum.Async)
		{
			GUIManager.Instance.LoadMovieTexture(textureSource, OnResourceAsyncLoaded, 0);
			isLoading = true;
		}
	}

	protected override void OnResourceAsyncLoaded(Object obj, AssetBundle bundle, object extraData)
	{
		CspUtils.DebugLog("Movie asynchonously loaded.");
		texture = (Texture)obj;
		movieTexture = (MovieTexture)texture;
		resourcesInitialized = true;
		isMovieInitializing = true;
		isLoading = false;
	}
}
