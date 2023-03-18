using System.Collections.Generic;
using UnityEngine;

public class SelectedObjectController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected class ObjData
	{
		public GameObject go;

		public List<GameObject> renderObjects;

		public List<int> renderObjectLayers;

		public ObjData(GameObject go)
		{
			this.go = go;
			renderObjects = new List<GameObject>(2);
			renderObjectLayers = new List<int>(2);
		}
	}

	public Shader shader;

	public Color pulseColor = Color.white;

	public float pulseRate = 5f;

	protected GameObject selectCamera;

	protected List<ObjData> selectedObjects;

	public int Count
	{
		get
		{
			if (selectedObjects != null)
			{
				return selectedObjects.Count;
			}
			return 0;
		}
	}

	public GameObject GetObjectAt(int index)
	{
		if (index < selectedObjects.Count)
		{
			return selectedObjects[index].go;
		}
		return null;
	}

	public void AddObject(GameObject go)
	{
		foreach (ObjData selectedObject in selectedObjects)
		{
			if (selectedObject.go == go)
			{
				return;
			}
		}
		selectedObjects.Add(new ObjData(go));
	}

	public void DelObject(GameObject go)
	{
		int num = 0;
		while (true)
		{
			if (num < selectedObjects.Count)
			{
				if (selectedObjects[num].go == go)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		selectedObjects.RemoveAt(num);
	}

	public void OnEnable()
	{
		selectedObjects = new List<ObjData>(2);
	}

	public void OnDisable()
	{
		selectedObjects.Clear();
		Object.Destroy(selectCamera);
		selectCamera = null;
	}

	public void OnPostRender()
	{
		if (base.enabled && base.gameObject.active && (bool)shader && selectedObjects.Count != 0)
		{
			if (!selectCamera)
			{
				selectCamera = new GameObject("SelectedObjectCamera", typeof(Camera));
				selectCamera.camera.enabled = false;
				selectCamera.hideFlags = HideFlags.HideAndDontSave;
			}
			Color color = pulseColor;
			color.a *= Mathf.Clamp(Mathf.Sin(Time.time * pulseRate), 0f, 0.35f);
			Shader.SetGlobalColor("_SelectedColor", color);
			foreach (ObjData selectedObject in selectedObjects)
			{
				Renderer[] components = Utils.GetComponents<Renderer>(selectedObject.go, Utils.SearchChildren);
				foreach (Renderer renderer in components)
				{
					if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
					{
						selectedObject.renderObjects.Add(renderer.gameObject);
						selectedObject.renderObjectLayers.Add(renderer.gameObject.layer);
						renderer.gameObject.layer = 29;
					}
				}
			}
			Camera camera = selectCamera.camera;
			camera.CopyFrom(base.camera);
			camera.backgroundColor = Color.green;
			camera.clearFlags = CameraClearFlags.Nothing;
			camera.cullingMask = 536870912;
			camera.RenderWithShader(shader, null);
			foreach (ObjData selectedObject2 in selectedObjects)
			{
				for (int j = 0; j < selectedObject2.renderObjects.Count; j++)
				{
					selectedObject2.renderObjects[j].layer = selectedObject2.renderObjectLayers[j];
				}
				selectedObject2.renderObjects.Clear();
				selectedObject2.renderObjectLayers.Clear();
			}
		}
	}
}
