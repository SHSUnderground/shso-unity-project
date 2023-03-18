using UnityEngine;

[AddComponentMenu("Lab/Emotor/Stepper")]
[RequireComponent(typeof(IEmotor))]
public class EmotorStepper : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject Target;

	public bool OnStart;

	public bool OnStop;

	private void Start()
	{
		IEmotor emotor = GetComponent(typeof(IEmotor)) as IEmotor;
		emotor.OnEmoteStart += delegate
		{
			if (base.enabled && OnStart)
			{
				Target.active = true;
			}
		};
		emotor.OnEmoteStop += delegate
		{
			if (base.enabled && OnStop)
			{
				Target.active = true;
			}
		};
	}
}
