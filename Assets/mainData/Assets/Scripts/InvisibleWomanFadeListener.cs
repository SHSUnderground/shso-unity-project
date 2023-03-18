using UnityEngine;

public class InvisibleWomanFadeListener : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public InvisibleWomanFadeController controller;

	[AnimTag("fade")]
	private void OnFadeAnimTag(AnimationEvent e)
	{
		string[] array = e.stringParameter.Split(':');
		string a = array[0];
		float num = 0f;
		if (array.Length > 1)
		{
			num = float.Parse(array[1]);
		}
		if (num > 0f)
		{
			controller.Fade(num);
		}
		else if (a == "toggle")
		{
			controller.Fade(controller.IsInvisible());
		}
		else
		{
			controller.Fade(a == "in");
		}
	}
}
