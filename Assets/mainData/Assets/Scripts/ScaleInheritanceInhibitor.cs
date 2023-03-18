using UnityEngine;

public class ScaleInheritanceInhibitor : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private Transform masterTransform;

	private void Start()
	{
		masterTransform = Utils.FindNodeInChildren(base.gameObject.transform, "master");
		if (masterTransform == null)
		{
			CspUtils.DebugLog("master node not found in ScaleInheritanceInhibitor!  Scales will be inherited.");
		}
	}

	private void LateUpdate()
	{
		if (!(masterTransform == null))
		{
		}
	}

	protected void invertChildScales(Transform parentTransform)
	{
		foreach (Transform item in parentTransform)
		{
			if (item.gameObject.name == "male_med_PalmBone01 1 " && item.gameObject.active)
			{
				CspUtils.DebugLog(parentTransform.gameObject.name + " lossy scale before: " + parentTransform.lossyScale.ToString());
				CspUtils.DebugLog(item.gameObject.name + " lossy scale before: " + item.lossyScale.ToString());
				CspUtils.DebugLog(item.gameObject.name + " local scale before: " + item.localScale.ToString());
			}
			item.localRotation = Quaternion.identity;
			item.localScale = Vector3.one;
			invertChildScales(item);
			if (item.gameObject.name == "male_med_PalmBone01 1 " && item.gameObject.active)
			{
				CspUtils.DebugLog(item.gameObject.name + " local scale after: " + item.localScale.ToString());
				CspUtils.DebugLog(item.gameObject.name + " lossy scale after: " + item.lossyScale.ToString());
			}
		}
	}
}
