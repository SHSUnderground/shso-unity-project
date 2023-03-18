using System;
using UnityEngine;

public class BrawlerThrowableEffect : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public const float kScalePadding = 1.1f;

	public const float kHeightOffset = 0.05f;

	private GameObject rectangularEffect;

	private GameObject cylindricalEffect;

	private Vector3 rectangularEffectSize;

	private Vector3 cylindricalEffectSize;

	private void Awake()
	{
		rectangularEffect = base.transform.Find("rectangular").gameObject;
		cylindricalEffect = base.transform.Find("cylindrical").gameObject;
		if ((bool)rectangularEffect)
		{
			GameObject gameObject = rectangularEffect.transform.Find("active_top").gameObject;
			if (gameObject != null)
			{
				rectangularEffectSize = gameObject.GetComponent<MeshFilter>().mesh.bounds.size;
			}
			else
			{
				CspUtils.DebugLog("Unable to find active_top in rectangular BrawlerThrowableEffect");
			}
		}
		else
		{
			CspUtils.DebugLog("Unable to find rectangular portion of BrawlerThrowableEffect");
		}
		if ((bool)cylindricalEffect)
		{
			GameObject gameObject2 = cylindricalEffect.transform.Find("active_top").gameObject;
			if (gameObject2 != null)
			{
				cylindricalEffectSize = gameObject2.GetComponent<MeshFilter>().mesh.bounds.size;
			}
			else
			{
				CspUtils.DebugLog("Unable to find active_top in cylindrical BrawlerThrowableEffect");
			}
		}
		else
		{
			CspUtils.DebugLog("Unable to find cylindrical portion of BrawlerThrowableEffect");
		}
	}

	private void Update()
	{
	}

	public void Attach(GameObject target)
	{
		MeshFilter componentInChildren = target.GetComponentInChildren<MeshFilter>();
		if (!(componentInChildren == null))
		{
			Vector3 size = componentInChildren.mesh.bounds.size;
			float num = 1f;
			float num2 = 1f;
			if (target.tag == "ThrowableCylindrical")
			{
				float num3 = Math.Max(size.x, size.y);
				num = num3 / cylindricalEffectSize.x;
				num2 = num3 / cylindricalEffectSize.y;
				Utils.ActivateTree(rectangularEffect, false);
				Utils.ActivateTree(cylindricalEffect, true);
				Animation component = cylindricalEffect.GetComponent<Animation>();
				component.Rewind();
				component.Play();
			}
			else
			{
				num = size.x / rectangularEffectSize.x;
				num2 = size.y / rectangularEffectSize.y;
				Utils.ActivateTree(rectangularEffect, true);
				Utils.ActivateTree(cylindricalEffect, false);
				Animation component2 = rectangularEffect.GetComponent<Animation>();
				component2.Rewind();
				component2.Play();
			}
			base.transform.localPosition = new Vector3(0f, 0f, (0f - size.z) / 2f + 0.05f);
			base.transform.localScale = new Vector3(num * 1.1f, num2 * 1.1f, 1f);
		}
	}
}
