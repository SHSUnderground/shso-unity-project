using UnityEngine;

public class ColliderUtil
{
	public static void UnScale(GameObject obj, Vector3 originalScale)
	{
		Vector3 localScale = obj.transform.localScale;
		localScale.x = Mathf.Abs(localScale.x);
		localScale.y = Mathf.Abs(localScale.y);
		localScale.z = Mathf.Abs(localScale.z);
		originalScale.x = Mathf.Abs(originalScale.x);
		originalScale.y = Mathf.Abs(originalScale.y);
		originalScale.z = Mathf.Abs(originalScale.z);
		Vector3 scale = new Vector3((localScale.x == 0f) ? 0f : (originalScale.x / localScale.x), (localScale.y == 0f) ? 0f : (originalScale.y / localScale.y), (localScale.z == 0f) ? 0f : (originalScale.z / localScale.z));
		float num = Mathf.Max(originalScale.x, Mathf.Max(originalScale.y, originalScale.z));
		float num2 = Mathf.Max(localScale.x, Mathf.Max(localScale.y, localScale.z));
		float num3 = (num2 == 0f) ? 0f : (num / num2);
		float num4 = Mathf.Max(originalScale.x, originalScale.y);
		float num5 = Mathf.Max(localScale.x, localScale.y);
		float num6 = (num5 == 0f) ? 0f : (num4 / num5);
		float num7 = Mathf.Max(originalScale.x, originalScale.z);
		float num8 = Mathf.Max(localScale.x, localScale.z);
		float num9 = (num8 == 0f) ? 0f : (num7 / num8);
		float num10 = Mathf.Max(originalScale.y, originalScale.z);
		float num11 = Mathf.Max(localScale.y, localScale.z);
		float num12 = (num11 == 0f) ? 0f : (num10 / num11);
		SphereCollider[] components = obj.GetComponents<SphereCollider>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].radius *= num3;
		}
		BoxCollider[] components2 = obj.GetComponents<BoxCollider>();
		foreach (BoxCollider boxCollider in components2)
		{
			boxCollider.size.Scale(scale);
		}
		CapsuleCollider[] components3 = obj.GetComponents<CapsuleCollider>();
		foreach (CapsuleCollider capsuleCollider in components3)
		{
			switch (capsuleCollider.direction)
			{
			case 0:
				capsuleCollider.height *= scale.x;
				capsuleCollider.radius *= num12;
				break;
			case 1:
				capsuleCollider.height *= scale.y;
				capsuleCollider.radius *= num9;
				break;
			case 2:
				capsuleCollider.height *= scale.z;
				capsuleCollider.radius *= num6;
				break;
			}
		}
	}
}
