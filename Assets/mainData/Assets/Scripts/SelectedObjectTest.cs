using UnityEngine;

public class SelectedObjectTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Material selectedMaterials;

	public float pulseRate = 1f;

	protected GameObject selected;

	protected float pulse;

	protected float pulseSin;

	public void Start()
	{
		MeshFilter component = Utils.GetComponent<MeshFilter>(base.gameObject);
		MeshRenderer component2 = Utils.GetComponent<MeshRenderer>(base.gameObject);
		selected = new GameObject("Selected Mesh");
		selected.layer = 1;
		selected.transform.position = base.transform.position;
		selected.transform.rotation = base.transform.rotation;
		selected.transform.localScale = base.transform.localScale;
		selected.transform.parent = base.transform;
		MeshFilter meshFilter = Utils.AddComponent<MeshFilter>(selected);
		meshFilter.mesh = component.mesh;
		MeshRenderer meshRenderer = Utils.AddComponent<MeshRenderer>(selected);
		Material[] array = new Material[component2.materials.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new Material(selectedMaterials);
		}
		meshRenderer.materials = array;
	}

	public void Update()
	{
		pulse += Time.deltaTime * pulseRate;
		pulseSin = Mathf.Sin(pulse);
		MeshRenderer component = Utils.GetComponent<MeshRenderer>(selected);
		Material[] materials = component.materials;
		for (int i = 0; i < materials.Length; i++)
		{
			materials[i].SetFloat("_Pulse", pulseSin);
		}
	}
}
