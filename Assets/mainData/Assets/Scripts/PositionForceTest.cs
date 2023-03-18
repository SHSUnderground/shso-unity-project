using UnityEngine;

public class PositionForceTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected string animName;

	protected Transform blasterTransform;

	protected CharacterController characterController;

	protected Vector3 lastPosition;

	protected Animation animationComponent;

	protected bool applyMotion;

	protected bool firstUpdate = true;

	private void Start()
	{
		animName = "new_bone_test_newanim";
		blasterTransform = Utils.FindNodeInChildren(base.gameObject.transform, "motion_export");
		characterController = (GetComponent("CharacterController") as CharacterController);
		animationComponent = (GetComponentInChildren(typeof(Animation)) as Animation);
	}

	private void Update()
	{
	}

	public void doMotion()
	{
		applyMotion = true;
		firstUpdate = true;
		animationComponent.Play(animName);
	}

	private void LateUpdate()
	{
		if (!applyMotion)
		{
			return;
		}
		if (!animationComponent.IsPlaying(animName))
		{
			applyMotion = false;
			return;
		}
		if (firstUpdate)
		{
			firstUpdate = false;
		}
		else
		{
			Vector3 motion = blasterTransform.position - lastPosition;
			characterController.Move(motion);
		}
		lastPosition = blasterTransform.position;
	}
}
