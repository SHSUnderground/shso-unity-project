using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Image Effects/Image Effect Storage")]
public class ImageEffectStorage : ScenarioEventHandlerEnableBase
{
	public List<string> ignoredComponentTypes;

	private List<Component> addedComponents = new List<Component>();

	private GameObject tempStorage;

	public ImageEffectStorage()
	{
		string[] collection = new string[9]
		{
			"ImageEffectStorage",
			"DynamicShadowController",
			"AOSceneCamera",
			"SeeThrough",
			"CameraTarget",
			"AudioListener",
			"GUILayer",
			"FlareLayer",
			"Transform"
		};
		ignoredComponentTypes = new List<string>(collection);
	}

	protected override void OnEnableEvent(string eventName)
	{
		ApplyToMainCamera();
	}

	protected override void OnDisableEvent(string eventName)
	{
		RemoveFromMainCamera();
	}

	public override void ManualReset()
	{
		OnDisableEvent(disableEvent);
	}

	public void ApplyToMainCamera()
	{
		GameObject gameObject = Camera.main.gameObject;
		ClearTempStorage();
		foreach (Component item in StorableComponents(base.gameObject))
		{
			Component component2 = gameObject.GetComponent(item.GetType());
			if (component2 != null)
			{
				StoreExistingComponent(component2);
				ComponentCopier.Copy(item, gameObject);
			}
			else
			{
				addedComponents.Add(ComponentCopier.Copy(item, gameObject));
			}
		}
	}

	public void RemoveFromMainCamera()
	{
		GameObject gameObject = Camera.main.gameObject;
		foreach (Component addedComponent in addedComponents)
		{
			Object.Destroy(addedComponent);
		}
		addedComponents.Clear();
		if (tempStorage != null)
		{
			Component[] components = tempStorage.GetComponents<Component>();
			foreach (Component sourceComponent in components)
			{
				ComponentCopier.Copy(sourceComponent, gameObject);
			}
			Object.Destroy(tempStorage);
		}
	}

	public void StoreMainCameraSettings()
	{
		ClearTempStorage();
		Component[] components = base.gameObject.GetComponents<Component>();
		foreach (Component component in components)
		{
			if (component != this && !ignoredComponentTypes.Contains(component.GetType().Name))
			{
				Object.Destroy(component);
			}
		}
		GameObject gameObject = Camera.main.gameObject;
		foreach (Component item in StorableComponents(gameObject))
		{
			ComponentCopier.Copy(item, base.gameObject);
		}
		Camera component2 = base.gameObject.GetComponent<Camera>();
		if (component2 != null)
		{
			component2.enabled = false;
		}
	}

	private IEnumerable StorableComponents(GameObject obj)
	{
		Component[] components = obj.GetComponents<Component>();
		foreach (Component component in components)
		{
			if (!ignoredComponentTypes.Contains(component.GetType().Name))
			{
				yield return component;
			}
		}
	}

	private void StoreExistingComponent(Component component)
	{
		if (tempStorage == null)
		{
			tempStorage = new GameObject("_TempCameraSettingsStorage");
			Utils.AttachGameObject(base.gameObject, tempStorage);
			tempStorage.active = false;
		}
		ComponentCopier.Copy(component, tempStorage);
	}

	private void ClearTempStorage()
	{
		if (tempStorage != null)
		{
			Object.Destroy(tempStorage);
		}
		addedComponents.Clear();
	}
}
