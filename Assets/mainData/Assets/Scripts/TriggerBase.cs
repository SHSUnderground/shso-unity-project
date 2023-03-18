using System.Collections;
using UnityEngine;

public abstract class TriggerBase : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject[] TriggerTargets;

	public string TriggerScenarioEvent = string.Empty;

	public bool TriggerAllChildren;

	public string TriggerMethod = "Triggered";

	public SendMessageOptions TriggerSendOptions;

	public float TriggerResetTime;

	public float TriggerFiringDelay;

	public bool OneShot;

	protected float nextTriggerTime;

	protected bool hasFired;

	protected Object triggeringObject;

	public Object TriggeringObject
	{
		get
		{
			return triggeringObject;
		}
	}

	protected virtual void Awake()
	{
		DetermineTriggerTargets();
	}

	protected void DetermineTriggerTargets()
	{
		if (TriggerTargets == null || TriggerTargets.Length == 0)
		{
			TriggerTargets = new GameObject[1]
			{
				base.gameObject
			};
		}
	}

	protected virtual bool OnTriggered(Object triggeringObject)
	{
		this.triggeringObject = triggeringObject;
		bool flag = OnTriggeredWithReset(TriggerTargets);
		this.triggeringObject = null;
		if (flag && TriggerScenarioEvent != string.Empty)
		{
			ScenarioEventManager.Instance.FireScenarioEvent(TriggerScenarioEvent, true);
		}
		return flag;
	}

	protected bool OnTriggeredWithReset(GameObject[] triggerTargets)
	{
		if (OneShot && hasFired)
		{
			return false;
		}
		if (TriggerResetTime > 0f && nextTriggerTime > Time.time)
		{
			return false;
		}
		if (TriggerResetTime > 0f)
		{
			nextTriggerTime = Time.time + TriggerResetTime;
		}
		OnTriggeredHelper(triggerTargets);
		return true;
	}

	protected void OnTriggeredHelper(GameObject[] triggerTargets)
	{
		hasFired = true;
		if (TriggerFiringDelay > 0f)
		{
			StartCoroutine(FireWithDelay(TriggerFiringDelay, triggerTargets, triggeringObject));
		}
		else
		{
			OnTriggeredFire(triggerTargets);
		}
	}

	protected IEnumerator FireWithDelay(float delayTime, GameObject[] triggerTargets, Object triggeringObject)
	{
		yield return new WaitForSeconds(delayTime);
		this.triggeringObject = triggeringObject;
		OnTriggeredFire(triggerTargets);
		this.triggeringObject = null;
	}

	protected void OnTriggeredFire(GameObject[] triggerTargets)
	{
		Object @object = triggeringObject;
		if (@object == null)
		{
			@object = this;
		}
		foreach (GameObject gameObject in triggerTargets)
		{
			if (!(gameObject == null))
			{
				if (TriggerAllChildren)
				{
					gameObject.BroadcastMessage(TriggerMethod, @object, TriggerSendOptions);
				}
				else
				{
					gameObject.SendMessage(TriggerMethod, @object, TriggerSendOptions);
				}
			}
		}
	}

	public void ManualReset()
	{
		hasFired = false;
		nextTriggerTime = 0f;
	}

	public void ManualReset(TriggerBase anotherTrigger)
	{
		ManualReset();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "TriggerDataIcon.png");
	}

	protected virtual void OnDrawGizmosSelected()
	{
		GameObject[] triggerTargets = TriggerTargets;
		foreach (GameObject gameObject in triggerTargets)
		{
			if (!(gameObject == null))
			{
				Gizmos.color = Color.red;
				Gizmos.DrawLine(base.transform.position, gameObject.transform.position);
				Quaternion rotation = gameObject.transform.rotation;
				Matrix4x4 matrix = Gizmos.matrix;
				try
				{
					gameObject.transform.LookAt(base.transform);
					Gizmos.matrix = gameObject.transform.localToWorldMatrix;
					Gizmos.DrawFrustum(gameObject.transform.position, 20f, 0f, 0.3f, 1f);
				}
				finally
				{
					gameObject.transform.rotation = rotation;
					Gizmos.matrix = matrix;
				}
			}
		}
		if (!(TriggerScenarioEvent != string.Empty))
		{
			return;
		}
		GameObject[] array = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		ScenarioEventHandlerBase scenarioEventHandlerBase = null;
		GameObject[] array2 = array;
		foreach (GameObject gameObject2 in array2)
		{
			scenarioEventHandlerBase = (gameObject2.GetComponent(typeof(ScenarioEventHandlerBase)) as ScenarioEventHandlerBase);
			if (scenarioEventHandlerBase != null)
			{
				scenarioEventHandlerBase.DrawTriggerGizmo(TriggerScenarioEvent, base.gameObject);
			}
		}
	}
}
