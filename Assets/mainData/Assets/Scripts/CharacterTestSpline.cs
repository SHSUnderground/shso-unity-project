using System.Collections.Generic;
using UnityEngine;

public class CharacterTestSpline : CharacterTestBase
{
	public GameObject splinePrefab;

	public Vector3 launchPositionOffset;

	protected bool activated;

	protected bool clicked;

	protected Dictionary<GameObject, InteractiveObject> interactiveObjects;

	protected float clickObjectTime;

	public void Update()
	{
		if (!activated)
		{
			return;
		}
		if (clicked)
		{
			bool flag = true;
			foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
			{
				if (!(character.behaviorManager.getBehavior() is BehaviorMovement))
				{
					flag = false;
				}
			}
			if (flag)
			{
				activated = false;
				CharacterTest.Instance.TestDone();
			}
		}
		else if (Time.time > clickObjectTime)
		{
			clicked = true;
			foreach (KeyValuePair<GameObject, InteractiveObject> interactiveObject in interactiveObjects)
			{
				interactiveObject.Value.ForceActivate(interactiveObject.Key);
			}
		}
	}

	public override void Activate()
	{
		activated = true;
		clicked = false;
		clickObjectTime = Time.time + 1f;
		interactiveObjects = new Dictionary<GameObject, InteractiveObject>();
		foreach (CharacterGlobals character in CharacterTest.Instance.GetCharacters())
		{
			GameObject g = Object.Instantiate(splinePrefab, character.transform.position + launchPositionOffset, splinePrefab.transform.rotation) as GameObject;
			InteractiveObject component = Utils.GetComponent<InteractiveObject>(g);
			if ((bool)component)
			{
				component.hidden = false;
				interactiveObjects.Add(character.gameObject, component);
			}
			else
			{
				CspUtils.DebugLog("There was a problem spawning the spline");
			}
		}
	}
}
