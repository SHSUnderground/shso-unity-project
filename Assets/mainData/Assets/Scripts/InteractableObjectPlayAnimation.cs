using UnityEngine;

public class InteractableObjectPlayAnimation : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public int loops = -1;

	protected Animation animComp;

	public void Initialize(InteractableObject top, GameObject model)
	{
		animComp = model.animation;
	}

	public float GetLength()
	{
		if (loops > 0)
		{
			return (float)loops * animComp.clip.length;
		}
		return animComp.clip.length;
	}

	public void OnEnable()
	{
		if (animComp != null)
		{
			animComp.wrapMode = WrapMode.Loop;
			animComp.Rewind();
			animComp.Play();
		}
	}

	public void OnDisable()
	{
		if (animComp != null)
		{
			animComp.Stop();
		}
	}
}
