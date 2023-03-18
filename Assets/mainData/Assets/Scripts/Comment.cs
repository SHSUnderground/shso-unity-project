using UnityEngine;

public class Comment : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string _Text;

	public string Text
	{
		get
		{
			return _Text;
		}
		set
		{
			_Text = value;
		}
	}
}
