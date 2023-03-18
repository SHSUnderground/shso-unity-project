using UnityEngine;

[AddComponentMenu("Miscellaneous/Animation Player")]
public class PlayAnimation : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string animationName = "movement_idle";

	public WrapMode animationWrap = WrapMode.Loop;

	private void Start()
	{
		Animation component = Utils.GetComponent<Animation>(base.gameObject, Utils.SearchChildren);
		if (component != null)
		{
			component.Play(animationName);
			component[animationName].wrapMode = animationWrap;
		}
	}
}
