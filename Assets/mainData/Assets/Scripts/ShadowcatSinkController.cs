using UnityEngine;

public class ShadowcatSinkController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float sinkDuration = 1f;

	private float sinkDistance = -1f;

	private float originalY;

	private float startTime;

	private void InitializeFromRecoil(BehaviorRecoilAttach.RecoilInitParams recoilInfo)
	{
		CharacterGlobals component = Utils.GetComponent<CharacterGlobals>(recoilInfo.target, Utils.SearchParents);
		if (component == null)
		{
			CspUtils.DebugLog("No CharacterGlobals found in ancestry of " + ToString());
			Finish();
		}
		else
		{
			sinkDistance = component.characterController.height / 2f;
		}
		Vector3 localPosition = base.transform.localPosition;
		originalY = localPosition.y;
		startTime = Time.time;
	}

	private void Update()
	{
		float num = Time.time - startTime;
		if (num > sinkDuration)
		{
			Finish();
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		localPosition.y = Mathf.Lerp(originalY, originalY - sinkDistance, num / sinkDuration);
		base.transform.localPosition = localPosition;
	}

	private void Finish()
	{
		Object.Destroy(base.gameObject);
	}
}
