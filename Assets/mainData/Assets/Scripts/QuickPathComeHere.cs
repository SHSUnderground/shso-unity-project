using System.Collections;
using UnityEngine;

[AddComponentMenu("Lab/QuickPath/Come Here")]
[RequireComponent(typeof(CharacterTargetSource))]
public class QuickPathComeHere : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private class BehaviorArrivalNotifier : BehaviorMovement
	{
		public event Utils.VoidAction OnArrived;

		public override void motionArrived()
		{
			if (this.OnArrived != null)
			{
				this.OnArrived();
			}
			base.motionArrived();
		}
	}

	public GameObject Chain;

	public bool Jump;

	public bool Force;

	private bool _trigger = true;

	private void Update()
	{
		if (!_trigger)
		{
			return;
		}
		GameObject character = Utils.GetComponent<CharacterTargetSource>(this).Character;
		if (!character)
		{
			Log("waiting for target");
			return;
		}
		BehaviorManager component = Utils.GetComponent<BehaviorManager>(character);
		if (component != null)
		{
			BehaviorArrivalNotifier behaviorArrivalNotifier = component.requestChangeBehavior(typeof(BehaviorArrivalNotifier), false) as BehaviorArrivalNotifier;
			if (behaviorArrivalNotifier == null)
			{
				if (Force)
				{
					behaviorArrivalNotifier = (component.forceChangeBehavior(typeof(BehaviorArrivalNotifier)) as BehaviorArrivalNotifier);
					if (behaviorArrivalNotifier == null)
					{
						Log("failed forcing target to change behavior");
					}
				}
				else
				{
					Log("target is busy");
				}
				return;
			}
			if (Chain != null)
			{
				behaviorArrivalNotifier.OnArrived += delegate
				{
					CharacterTargetSource component3 = Utils.GetComponent<CharacterTargetSource>(Chain);
					if (component3 != null)
					{
						component3.Target = Utils.GetComponent<CharacterTargetSource>(Chain).Target;
						component3.enabled = true;
					}
					Chain.active = true;
				};
			}
		}
		else
		{
			Log("could not find behavior manager");
		}
		CharacterMotionController component2 = Utils.GetComponent<CharacterMotionController>(character);
		if (component2 != null)
		{
			if (Jump)
			{
				component2.jumpPressed();
			}
			component2.setDestination(base.transform.position, base.transform.forward);
		}
		else
		{
			Log("could not find motion controller");
		}
		_trigger = false;
		StartCoroutine(Finish());
	}

	private IEnumerator Finish()
	{
		yield return new WaitForEndOfFrame();
		Log("finished");
		base.gameObject.active = false;
		_trigger = true;
	}

	private void Log(string message)
	{
	}
}
