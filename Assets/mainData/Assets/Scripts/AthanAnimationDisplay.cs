using UnityEngine;

public class AthanAnimationDisplay : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private class TestAnimatins : AnimClipBuilder
	{
		public static AnimPath Anim(AthanAnimationDisplay s)
		{
			return SHSAnimations.GenericPaths.LinearWithWiggle(0f, 10f, s.V(0), (float)(-Mathf.Abs(10)) * s.V(1), s.V(0) * s.V(2));
		}
	}

	public float step;

	public float[] Values = new float[3]
	{
		0.384f,
		0.077f,
		0.616f
	};

	private float V(int i)
	{
		if (i < Values.Length)
		{
			return Values[i];
		}
		return 1f;
	}

	private void Update()
	{
		AnimClipBuilder.FullDebugPrint(TestAnimatins.Anim(this), step, new Vector3(0f, 0f, 0f), false);
	}
}
