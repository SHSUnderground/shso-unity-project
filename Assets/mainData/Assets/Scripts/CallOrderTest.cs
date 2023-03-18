using UnityEngine;

public class CallOrderTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected bool bUpdateLogged;

	protected bool bLateUpdateLogged;

	protected bool bFixedUpdateLogged;

	private void Awake()
	{
		CspUtils.DebugLog("Awake was called");
		bUpdateLogged = false;
	}

	private void Start()
	{
		CspUtils.DebugLog("Start was called");
	}

	private void OnEnable()
	{
		CspUtils.DebugLog("OnEnable was called");
	}

	private void OnDisable()
	{
		CspUtils.DebugLog("OnDisable was called");
	}

	private void Update()
	{
		if (!bUpdateLogged)
		{
			CspUtils.DebugLog("Update was called");
			bUpdateLogged = true;
		}
	}

	private void LateUpdate()
	{
		if (!bLateUpdateLogged)
		{
			CspUtils.DebugLog("LateUpdate was called");
			bLateUpdateLogged = true;
		}
	}

	private void FixedUpdate()
	{
		if (!bFixedUpdateLogged)
		{
			CspUtils.DebugLog("FixedUpdate was called");
			bFixedUpdateLogged = true;
		}
	}

	private void OnWillRenderObject()
	{
		CspUtils.DebugLog("OnWillRenderObject: " + base.gameObject.name + " distance to camera = " + (Camera.main.transform.position - base.transform.position).magnitude);
	}
}
