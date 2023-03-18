using UnityEngine;

public class CameraLiteArena : CameraLite
{
	public float rotationAngle;

	public float verticalAngle = 25f;

	public float[] distanceAtKeeperCount = new float[1]
	{
		20f
	};

	protected GameObject targetCopy;

	private void Awake()
	{
		targetCopy = new GameObject("target copy");
		Utils.AttachGameObject(base.gameObject, targetCopy);
	}

	public override void UpdateFromMgr(float deltaTime)
	{
		Vector3 eulerAngles = new Vector3(0f - verticalAngle, rotationAngle, 0f);
		targetCopy.transform.position = target.position;
		targetCopy.transform.eulerAngles = eulerAngles;
		int num = SquadBattleCharacterController.Instance.HighestKeeperPosition();
		if (num < 0)
		{
			num = 0;
		}
		if (num > distanceAtKeeperCount.Length - 1)
		{
			num = distanceAtKeeperCount.Length - 1;
		}
		Vector3 to = targetCopy.transform.forward * distanceAtKeeperCount[num];
		base.transform.position = Vector3.Slerp(base.transform.position, to, deltaTime);
		base.transform.LookAt(targetCopy.transform.position);
		base.UpdateFromMgr(deltaTime);
	}
}
