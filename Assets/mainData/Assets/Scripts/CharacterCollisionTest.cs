using System.Text;
using UnityEngine;

public class CharacterCollisionTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public CharacterGlobals charGlobals;

	public Vector3 originalPosition;

	public Vector3 lastPosition;

	public bool blockLog;

	public bool blockOutOfWorld;

	public void Start()
	{
		charGlobals = base.gameObject.GetComponent<CharacterGlobals>();
		originalPosition = base.transform.position;
		lastPosition = originalPosition;
		blockLog = false;
		blockOutOfWorld = false;
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Slash) && Input.GetKey(KeyCode.LeftControl))
		{
			DumpCharacter();
		}
		if (Input.GetKeyDown(KeyCode.Period) && Input.GetKey(KeyCode.LeftControl))
		{
			DumpRayCast();
		}
		Vector3 vector = lastPosition = base.transform.position;
		if (vector.y <= -75f)
		{
			if (!blockOutOfWorld)
			{
				StringBuilder stringBuilder = new StringBuilder();
				RaycastHit hitInfo;
				if (ShsCharacterController.FindGround(new Vector3(vector.x, vector.y + 1000f, vector.z), 5000f, out hitInfo))
				{
					stringBuilder.AppendLine("Player <" + base.gameObject.name + "> is likely outside world at <" + vector.x + "," + vector.y + "," + vector.z + ">");
					stringBuilder.AppendLine("  ray casting from above hit ground");
				}
				else
				{
					stringBuilder.AppendLine("Player <" + base.gameObject.name + "> is likely outside world at <" + vector.x + "," + vector.y + "," + vector.z + ">");
					stringBuilder.AppendLine("  ray casting from above did NOT hit ground!!!");
				}
				blockOutOfWorld = true;
				CspUtils.DebugLog(stringBuilder.ToString());
			}
		}
		else
		{
			blockOutOfWorld = false;
		}
	}

	protected void DumpCharacter()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Character <" + base.gameObject.name + ">");
		stringBuilder.AppendLine("  behavior: " + charGlobals.behaviorManager.getBehavior().ToString());
		stringBuilder.AppendLine("  original position: " + originalPosition.x + " , " + originalPosition.y + " , " + originalPosition.z);
		stringBuilder.AppendLine("  last position: " + lastPosition.x + " , " + lastPosition.y + " , " + lastPosition.z);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("  Raycast:");
		RaycastHit hitInfo;
		if (ShsCharacterController.FindGround(new Vector3(lastPosition.x, lastPosition.y + 1000f, lastPosition.z), 50000f, out hitInfo))
		{
			stringBuilder.AppendLine("    hit ground <" + hitInfo.collider.gameObject.name + ">");
		}
		else
		{
			stringBuilder.AppendLine("    dot NOT hit ground");
		}
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("  Colliders:");
		Object[] array = Object.FindSceneObjectsOfType(typeof(Collider));
		for (int i = 0; i < array.Length; i++)
		{
			Collider collider = (Collider)array[i];
			stringBuilder.AppendLine("    Name: " + collider.gameObject.name.ToString());
			stringBuilder.AppendLine("      Active: " + ((!collider.gameObject.active) ? "false" : "true"));
			stringBuilder.AppendLine("      Type: " + ((!collider.isTrigger) ? "collision" : "trigger"));
			stringBuilder.AppendLine("      Layer: " + LayerMask.LayerToName(collider.gameObject.layer));
		}
		stringBuilder.AppendLine();
		CspUtils.DebugLog(stringBuilder.ToString());
	}

	protected void DumpRayCast()
	{
		Vector3 position = new Vector3(lastPosition.x, lastPosition.y, lastPosition.z);
		position.y += 1000f;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Raycast <" + position.x + "," + position.y + "," + position.z + ">");
		RaycastHit hitInfo;
		if (ShsCharacterController.FindGround(position, 50000f, out hitInfo, stringBuilder))
		{
			stringBuilder.AppendLine("  hit ground <" + hitInfo.collider.gameObject.name + ">");
		}
		else
		{
			stringBuilder.AppendLine("  dot NOT hit ground");
		}
		stringBuilder.AppendLine();
		CspUtils.DebugLog(stringBuilder.ToString());
	}
}
