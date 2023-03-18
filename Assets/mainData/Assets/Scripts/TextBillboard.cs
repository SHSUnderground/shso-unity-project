using UnityEngine;

public class TextBillboard : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool billboard;

	public string Text
	{
		get
		{
			return Utils.GetComponent<TextMesh>(base.gameObject).text;
		}
		set
		{
			Utils.GetComponent<TextMesh>(base.gameObject).text = value;
		}
	}

	public TextBillboard()
	{
		billboard = false;
	}

	public Transform GetTransform()
	{
		return base.transform;
	}

	private void Update()
	{
		if (billboard)
		{
			Camera main = Camera.main;
			if (!(main == null))
			{
				Vector3 b = main.transform.position - base.transform.position;
				base.transform.LookAt(base.transform.position - b);
			}
		}
	}
}
