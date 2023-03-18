using System.Text;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ShsCharacterController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected CharacterController charController;

	[HideInInspector]
	public string pickupBone;

	public static float GroundOffset = 0.235f;

	public Vector3 center
	{
		get
		{
			return charController.center;
		}
		set
		{
			charController.center = value;
		}
	}

	public CollisionFlags collisionFlags
	{
		get
		{
			return charController.collisionFlags;
		}
	}

	public float height
	{
		get
		{
			return charController.height;
		}
		set
		{
			charController.height = value;
		}
	}

	public bool isGrounded
	{
		get
		{
			return charController.isGrounded;
		}
	}

	public float radius
	{
		get
		{
			return charController.radius;
		}
		set
		{
			charController.radius = value;
		}
	}

	public Vector3 velocity
	{
		get
		{
			return (!(charController == null)) ? charController.velocity : Vector3.zero;
		}
	}

	protected void Start()
	{
		charController = (GetComponent(typeof(CharacterController)) as CharacterController);
	}

	protected void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (((1 << hit.collider.gameObject.layer) & 0x18900) == 0)
		{
			hit.collider.gameObject.SendMessage("OnCharacterCollided", hit, SendMessageOptions.DontRequireReceiver);
		}
	}

	public CollisionFlags Move(Vector3 motion)
	{
		Vector3 position = charController.transform.position;
		CollisionFlags collisionFlags = charController.Move(motion);
		if (!charController.isGrounded && (collisionFlags & CollisionFlags.Below) == 0)
		{
			Vector3 position2 = base.gameObject.transform.position;
			float num = position2.y - position.y;
			RaycastHit hitInfo;
			if (num < 0f && FindGround(new Vector3(position2.x, position.y, position2.z), 0f - num, out hitInfo))
			{
				float magnitude = (hitInfo.point - position2).magnitude;
				if (magnitude > 0.05f)
				{
					float magnitude2 = motion.magnitude;
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendLine(base.gameObject.name + " moved through ground <" + hitInfo.collider.gameObject.name + "> at < " + position2.x + " , " + position2.y + " , " + position2.z + " > ");
					object[] obj = new object[7]
					{
						"   teleporting to <",
						null,
						null,
						null,
						null,
						null,
						null
					};
					Vector3 point = hitInfo.point;
					obj[1] = point.x;
					obj[2] = " , ";
					Vector3 point2 = hitInfo.point;
					obj[3] = point2.y;
					obj[4] = " , ";
					Vector3 point3 = hitInfo.point;
					obj[5] = point3.z;
					obj[6] = " >";
					stringBuilder.AppendLine(string.Concat(obj));
					stringBuilder.AppendLine("   hit delta is <" + magnitude + ">, move delta is <" + magnitude2 + ">, and percentage is <" + magnitude / magnitude2 * 100f + ">");
					CspUtils.DebugLogError(stringBuilder.ToString());
					base.gameObject.transform.position = hitInfo.point;
				}
			}
		}
		return collisionFlags;
	}

	public bool SimpleMove(Vector3 speed)
	{
		return charController.SimpleMove(speed);
	}

	public static bool FindGround(Vector3 position, float distance, out RaycastHit hitInfo)
	{
		return FindGround(position, distance, out hitInfo, null);
	}

	public static bool FindGround(Vector3 position, float distance, out RaycastHit hitInfo, StringBuilder sb)
	{
		Ray ray = new Ray(position, Vector3.down);
		RaycastHit[] array = Physics.RaycastAll(ray, distance, -2101253);
		if (sb != null)
		{
			sb.AppendLine("Raycast from <" + ray.GetPoint(0f).ToString() + "> hit " + array.Length + " colliders");
		}
		int num = -1;
		float num2 = float.MinValue;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].collider.isTrigger && !(array[i].collider.gameObject.name == "KillyMcKillington"))
			{
				Vector3 point = array[i].point;
				if (point.y > num2)
				{
					num = i;
					Vector3 point2 = array[i].point;
					num2 = point2.y;
				}
			}
		}
		if (num >= 0)
		{
			hitInfo = array[num];
			return true;
		}
		hitInfo = default(RaycastHit);
		return false;
	}

	public static Vector3 FindPositionOnGround(CharacterController charControl, Vector3 groundPosition, float offset)
	{
		if (charControl == null)
		{
			return groundPosition;
		}
		float num = charControl.height - charControl.radius * 2f;
		if (num < 0f)
		{
			num = 0f;
		}
		float num2 = groundPosition.y + charControl.radius + num / 2f;
		num2 += offset;
		float num3 = num2;
		Vector3 center = charControl.center;
		float y = num3 - center.y;
		return new Vector3(groundPosition.x, y, groundPosition.z);
	}
}
