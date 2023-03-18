using UnityEngine;

[AddComponentMenu("Camera/Camera Lite Offset")]
public class CameraLiteOffset : CameraLite
{
	protected float rotationAngle;

	protected float verticalAngle = 25f;

	protected float distance = 5f;

	protected float rotationVelocity = 10f;

	protected float movementVelocity = 3f;

	protected bool autoHeight = true;

	protected HairTrafficController htc;

	protected CombatController combatController;

	public CameraOffsetData[] cameraSettings = new CameraOffsetData[1];

	protected Vector3 lookAtOffset = Vector3.up;

	protected int currentSettingsIndex;

	protected float currentSettingsEndTime;

	protected float transitionTimeRemaining;

	protected GameObject temp;

	protected GameObject destination;

	protected bool reverseAngle;

	protected float extraHeight;

	protected float extraRotation;

	public override void Reset()
	{
		base.Reset();
		InternalOverride();
	}

	public override void InitFromMgr()
	{
		base.InitFromMgr();
		InternalOverride();
	}

	protected void changeCurrentSettings(int newSettingsIndex)
	{
		extraHeight = 0f;
		currentSettingsIndex = newSettingsIndex;
		currentSettingsEndTime = cameraSettings[currentSettingsIndex].duration + Time.time;
		transitionTimeRemaining = cameraSettings[currentSettingsIndex].transitionTime;
	}

	protected void InternalOverride()
	{
		changeCurrentSettings(0);
		updateSettings();
		base.transform.position = destination.transform.position;
		base.transform.rotation = destination.transform.rotation;
	}

	public override void WakeFromMgr()
	{
		base.WakeFromMgr();
		InitFromMgr();
	}

	public override void UpdateFromMgr(float deltaTime)
	{
		updateSettings();
		base.transform.position = Vector3.Slerp(base.transform.position, destination.transform.position, deltaTime * movementVelocity);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, destination.transform.rotation, deltaTime * rotationVelocity);
		base.UpdateFromMgr(deltaTime);
	}

	private void Awake()
	{
		if (cameraSettings.Length >= 1)
		{
			cameraSettings[0].transitionTime = 0f;
			destination = new GameObject("destination");
			Utils.AttachGameObject(base.gameObject, destination);
			temp = new GameObject("temp");
			Utils.AttachGameObject(base.gameObject, temp);
			InitFromMgr();
		}
	}

	public void SetExtraRotation(float rotation)
	{
		extraRotation = rotation;
	}

	public void SetReverse(bool reverse)
	{
		reverseAngle = reverse;
	}

	public void SetExtraHeight(float height)
	{
		if (height == 0f)
		{
			autoHeight = true;
		}
		else
		{
			autoHeight = false;
		}
		extraHeight = height;
	}

	protected void updateSettings()
	{
		if (autoHeight && htc != null)
		{
			Vector3 position = htc.HairTrafficPoint.transform.position;
			float num = position.y;
			if (htc.MasterHeroOffsetTable.ContainsKey(target.gameObject.name))
			{
				num += htc.GetYOffset(target.gameObject);
			}
			extraHeight = Mathf.Max(0f, num - 2.5f);
		}
		if (transitionTimeRemaining > -999f)
		{
			CameraOffsetData cameraOffsetData = cameraSettings[currentSettingsIndex];
			Vector3 to = cameraOffsetData.lookAtOffset;
			if (reverseAngle)
			{
				to.x *= -1f;
			}
			if (transitionTimeRemaining <= 0f)
			{
				rotationAngle = cameraOffsetData.rotationAngle;
				verticalAngle = cameraOffsetData.verticalAngle;
				distance = cameraOffsetData.distance;
				rotationVelocity = cameraOffsetData.rotationVelocity;
				movementVelocity = cameraOffsetData.movementVelocity;
				lookAtOffset = to;
				transitionTimeRemaining = -999f;
			}
			else
			{
				float t = Time.deltaTime / transitionTimeRemaining;
				rotationAngle = Mathf.Lerp(rotationAngle, cameraOffsetData.rotationAngle, t);
				verticalAngle = Mathf.Lerp(verticalAngle, cameraOffsetData.verticalAngle, t);
				distance = Mathf.Lerp(distance, cameraOffsetData.distance, t);
				rotationVelocity = Mathf.Lerp(rotationVelocity, cameraOffsetData.rotationVelocity, t);
				movementVelocity = Mathf.Lerp(movementVelocity, cameraOffsetData.movementVelocity, t);
				lookAtOffset = Vector3.Lerp(lookAtOffset, to, t);
				transitionTimeRemaining -= Time.deltaTime;
			}
		}
		computeDestination();
		if (Time.time > currentSettingsEndTime && currentSettingsIndex < cameraSettings.Length - 1)
		{
			changeCurrentSettings(currentSettingsIndex + 1);
		}
	}

	protected virtual void computeDestination()
	{
		if (!(target == null))
		{
			float num = (!reverseAngle) ? 1 : (-1);
			Vector3 eulerAngles = new Vector3(0f - verticalAngle, rotationAngle * num, 0f);
			temp.transform.position = target.position + lookAtOffset + temp.transform.up * extraHeight;
			temp.transform.eulerAngles = eulerAngles;
			destination.transform.position = temp.transform.position + temp.transform.forward * (distance + extraHeight);
			destination.transform.LookAt(temp.transform);
			if (extraRotation != 0f)
			{
				destination.transform.Rotate(Vector3.up, extraRotation);
			}
		}
	}

	public override void SetTarget(Transform newTarget)
	{
		base.SetTarget(newTarget);
		combatController = (newTarget.GetComponent(typeof(CombatController)) as CombatController);
		htc = (newTarget.GetComponent(typeof(HairTrafficController)) as HairTrafficController);
	}
}
