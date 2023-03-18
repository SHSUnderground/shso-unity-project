using UnityEngine;

public class TroubleBotColliderSink : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected virtual void OnCollisionEnter(Collision other)
	{
		TroubleBotActivityObject component = Utils.GetComponent<TroubleBotActivityObject>(this);
		if (!component.Destroyed && component != null)
		{
			component.ActionTriggered(ActivityObjectActionNameEnum.Collision);
		}
	}
}
