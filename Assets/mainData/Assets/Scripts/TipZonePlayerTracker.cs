using System.Collections;
using UnityEngine;

public class TipZonePlayerTracker : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string tipKey;

	public static readonly float idleTime = 10f;

	private void Start()
	{
		if (Random.Range(0f, 1f) < 0.2f)
		{
			StartCoroutine(ShowTip(2f));
		}
		else
		{
			StartCoroutine(TrackIdle());
		}
	}

	private IEnumerator ShowTip(float delayTime)
	{
		if (delayTime > 0f)
		{
			yield return new WaitForSeconds(delayTime);
		}
		GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, tipKey, (IGUIDialogNotification)null, GUIControl.ModalLevelEnum.None);
		Object.Destroy(this);
	}

	private IEnumerator TrackIdle()
	{
		float lastNonIdleTime = Time.time;
		BehaviorManager behaviorMan = Utils.GetComponent<CharacterGlobals>(base.gameObject).behaviorManager;
		while (true)
		{
			BehaviorMovement movement = behaviorMan.getBehavior() as BehaviorMovement;
			if (movement == null || movement.CurrentMovementState != BehaviorMovement.MovementState.Idle)
			{
				lastNonIdleTime = Time.time;
			}
			if (Time.time - lastNonIdleTime > idleTime)
			{
				break;
			}
			yield return 0;
		}
		StartCoroutine(ShowTip(0f));
	}
}
