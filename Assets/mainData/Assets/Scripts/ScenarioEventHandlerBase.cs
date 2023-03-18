using UnityEngine;

public class ScenarioEventHandlerBase : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public virtual void DrawTriggerGizmo(string triggerEventName, GameObject triggerObject)
	{
	}

	protected virtual void OnDrawGizmosSelected()
	{
		GameObject[] array = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		TriggerBase triggerBase = null;
		SpawnController spawnController = null;
		ScenarioEventTransferBox scenarioEventTransferBox = null;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			triggerBase = (gameObject.GetComponent(typeof(TriggerBase)) as TriggerBase);
			if (triggerBase != null && triggerBase.TriggerScenarioEvent != string.Empty)
			{
				DrawTriggerGizmo(triggerBase.TriggerScenarioEvent, gameObject);
			}
			spawnController = (gameObject.GetComponent(typeof(SpawnController)) as SpawnController);
			if (spawnController != null && spawnController.spawnCompleteEvent != string.Empty)
			{
				DrawTriggerGizmo(spawnController.spawnCompleteEvent, gameObject);
			}
			scenarioEventTransferBox = (gameObject.GetComponent(typeof(ScenarioEventTransferBox)) as ScenarioEventTransferBox);
			if (scenarioEventTransferBox != null && scenarioEventTransferBox.sendEvent != string.Empty)
			{
				DrawTriggerGizmo(scenarioEventTransferBox.sendEvent, gameObject);
			}
		}
	}
}
