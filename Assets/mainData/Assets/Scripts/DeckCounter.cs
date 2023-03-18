using UnityEngine;

public class DeckCounter : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public int count;

	public string backdropTexture;

	private int lastCount;

	private TextMesh[] textMeshes;

	private void Start()
	{
		count = 0;
		lastCount = 0;
		Transform transform = base.gameObject.transform.Find("count_mesh/shadow");
		if (transform != null)
		{
			GameObject gameObject = transform.gameObject;
			MeshRenderer componentInChildren = gameObject.GetComponentInChildren<MeshRenderer>();
			componentInChildren.materials[componentInChildren.materials.Length - 1].color = GUILabel.GenColor(42, 49, 142);
		}
		textMeshes = GetComponentsInChildren<TextMesh>();
	}

	private void Update()
	{
		if (count != lastCount)
		{
			lastCount = count;
			TextMesh[] array = textMeshes;
			foreach (TextMesh textMesh in array)
			{
				textMesh.text = count.ToString();
			}
		}
	}

	public void SetCount(int c)
	{
		count = c;
		lastCount = c;
		if (textMeshes == null)
		{
			textMeshes = GetComponentsInChildren<TextMesh>();
		}
		TextMesh[] array = textMeshes;
		foreach (TextMesh textMesh in array)
		{
			textMesh.text = count.ToString();
		}
	}
}
