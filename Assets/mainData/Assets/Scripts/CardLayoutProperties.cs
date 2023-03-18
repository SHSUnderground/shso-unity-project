using UnityEngine;

public class CardLayoutProperties : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum LayoutType
	{
		Stack,
		Arch,
		Hand,
		Grid
	}

	public enum AlignmentType
	{
		Negative,
		Center,
		Positive
	}

	public LayoutType shape;

	public AlignmentType alignment;

	public float spacingX;

	public float spacingY;

	public float spacingZ;

	public float offsetX;

	public float offsetY;

	public float randomRotation;

	public int MaxCardsBeforeOverlapping;

	public bool overlapAsStack;

	public int gridRowWidth = 3;

	public int visibleRowCount = 2;

	public int visibleCardCount = -1;

	public string cardAnimationName;

	public GameObject cardEffectPrefab;

	public CardLayoutProperties()
	{
		shape = LayoutType.Stack;
		alignment = AlignmentType.Positive;
		randomRotation = 0f;
		MaxCardsBeforeOverlapping = 0;
		spacingX = 0f;
		spacingY = 0f;
		spacingZ = 0f;
		offsetX = 0f;
		offsetY = 0f;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "CardLayoutIcon.png");
	}
}
