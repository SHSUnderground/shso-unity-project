using UnityEngine;

public class RallyPathNode : PathNodeBase
{
	public RallyPathNode[] nextNodes;

	public bool startNode;

	public float radius = 0.25f;

	public float duration = 5f;

	private Vector3 gizmo_cachedNextDirection = Vector3.zero;

	public override void Awake()
	{
		base.Awake();
		Type = NodeType.Rally;
	}

	public override void Start()
	{
		RallyPathNode[] array = nextNodes;
		foreach (RallyPathNode x in array)
		{
			if (x == null)
			{
			}
		}
	}

	public override void OnDrawGizmos()
	{
		Gizmos.DrawSphere(base.transform.position, 0.05f);
		Matrix4x4 matrix = Gizmos.matrix;
		if (nextNodes.Length > 0 && nextNodes[0] != null)
		{
			RallyPathNode rallyPathNode = nextNodes[0];
			GameObject gameObject = rallyPathNode.gameObject;
			Gizmos.DrawLine(base.transform.position, gameObject.transform.position);
			Vector3 vector = gameObject.transform.position - base.transform.position;
			if (gizmo_cachedNextDirection == Vector3.zero || vector != gizmo_cachedNextDirection)
			{
				gizmo_cachedNextDirection = vector;
				Quaternion a = Quaternion.LookRotation(vector);
				Transform transform = Utils.FindNodeInChildren(base.transform, "editor_only");
				if (transform != null)
				{
					Transform transform2 = Utils.FindNodeInChildren(transform, "direction finder");
					if (Quaternion.Angle(a, transform2.rotation) > 0.01f)
					{
						transform.localRotation = Quaternion.identity;
						transform2.LookAt(gameObject.transform);
					}
				}
			}
		}
		Gizmos.matrix = matrix;
		Color color = Gizmos.color;
		Gizmos.color = new Color(0.15f, 0.23f, 0.42f, 0.3f);
		Matrix4x4 matrix2 = default(Matrix4x4);
		matrix2.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(1f, 0.2f, 1f));
		Gizmos.matrix = matrix2;
		Gizmos.DrawSphere(base.transform.position, radius);
		Gizmos.color = color;
		Gizmos.matrix = matrix;
	}
}
