using UnityEngine;

public class RetreatingPanel : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool ignoreX;

	public bool ignoreY;

	public bool ignoreZ = true;

	public Vector3 advancePosition;

	public Vector3 retreatPosition;

	public bool retreat;

	public bool advance;

	private AnimClipManager AnimationPieceManager;

	private void Awake()
	{
		AnimationPieceManager = new AnimClipManager();
	}

	private void Update()
	{
		if (retreat)
		{
			retreat = false;
			Retreat(1f);
		}
		if (advance)
		{
			advance = false;
			Advance(1f);
		}
		AnimationPieceManager.Update(Time.deltaTime);
	}

	public void Advance(float animTime)
	{
		Animate(base.transform.localPosition, advancePosition, animTime);
	}

	public void Retreat(float animTime)
	{
		Animate(base.transform.localPosition, retreatPosition, animTime);
	}

	private void Animate(Vector3 start, Vector3 end, float animTime)
	{
		AnimPath path = SHSAnimations.GenericPaths.LinearWithWiggle(0f, 1f, animTime);
		AnimationPieceManager.Add(AnimClipBuilder.Custom.Function(path, delegate(float t)
		{
			Vector3 localPosition = new Vector3(start.x, start.y, start.z);
			if (!ignoreX)
			{
				localPosition.x = Mathf.Lerp(start.x, end.x, t);
			}
			if (!ignoreY)
			{
				localPosition.y = Mathf.Lerp(start.y, end.y, t);
			}
			if (!ignoreZ)
			{
				localPosition.z = Mathf.Lerp(start.z, end.z, t);
			}
			base.transform.localPosition = localPosition;
		}));
	}
}
