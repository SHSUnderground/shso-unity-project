using UnityEngine;

[AddComponentMenu("VO/VO Emitter")]
public class VOEmitter : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string overrideName;

	public string EmitterName
	{
		get
		{
			if (string.IsNullOrEmpty(overrideName))
			{
				return base.name;
			}
			return overrideName;
		}
	}

	public static string GetEmitterName(GameObject emitter)
	{
		if (emitter == null)
		{
			return "NULL";
		}
		VOEmitter component = emitter.GetComponent<VOEmitter>();
		if (component != null)
		{
			return component.EmitterName;
		}
		return emitter.name;
	}
}
