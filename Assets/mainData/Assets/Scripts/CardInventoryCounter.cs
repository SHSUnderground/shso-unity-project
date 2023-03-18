using UnityEngine;

[ExecuteInEditMode]
public class CardInventoryCounter : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public int count;

	private int lastCount;

	private TextMesh[] textMeshes;

	private void Start()
	{
		count = 1;
		lastCount = 1;
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
