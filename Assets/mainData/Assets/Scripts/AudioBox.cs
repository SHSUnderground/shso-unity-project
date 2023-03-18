using UnityEngine;

[AddComponentMenu("Audio/Audio Box")]
public class AudioBox : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public ShsAudioSource audioSrc;

	public Vector3 size = new Vector3(6f, 6f, 6f);

	public bool waitForPlayerSpawn = true;

	public bool alwaysRender;

	public float estimatedAudioSize = 3f;

	public bool renderCornerSpheres;

	public bool renderInnerBox;

	public bool renderOuterBox = true;

	private GameObject audioSrcInstance;

	public float HalfWidth
	{
		get
		{
			return size.x / 2f;
		}
	}

	public float HalfHeight
	{
		get
		{
			return size.y / 2f;
		}
	}

	public float HalfDepth
	{
		get
		{
			return size.z / 2f;
		}
	}

	private void Start()
	{
		if (waitForPlayerSpawn)
		{
			AppShell.Instance.EventMgr.AddListener<LocalPlayerChangedMessage>(OnLocalPlayerChanged);
		}
		else
		{
			PlayAudio();
		}
	}

	private void OnLocalPlayerChanged(LocalPlayerChangedMessage e)
	{
		AppShell.Instance.EventMgr.RemoveListener<LocalPlayerChangedMessage>(OnLocalPlayerChanged);
		PlayAudio();
	}

	private void OnDestroy()
	{
		if (audioSrcInstance != null)
		{
			Object.Destroy(audioSrcInstance);
		}
	}

	private void LateUpdate()
	{
		if (audioSrcInstance != null)
		{
			Vector3 point = Camera.main.transform.position + 0.75f * Camera.main.transform.forward;
			audioSrcInstance.transform.position = GetClosestPoint(point);
		}
	}

	private void PlayAudio()
	{
		if (audioSrc != null)
		{
			audioSrcInstance = ShsAudioSource.PlayFromPrefab(audioSrc.gameObject, base.transform).gameObject;
			Utils.AttachGameObject(base.gameObject, audioSrcInstance);
		}
	}

	private Vector3 GetClosestPoint(Vector3 point)
	{
		Vector3 position = base.transform.InverseTransformPoint(point);
		position.x = Mathf.Clamp(position.x, 0f - HalfWidth, HalfWidth);
		position.y = Mathf.Clamp(position.y, 0f - HalfHeight, HalfHeight);
		position.z = Mathf.Clamp(position.z, 0f - HalfDepth, HalfDepth);
		return base.transform.TransformPoint(position);
	}

	private void OnDrawGizmos()
	{
		if (alwaysRender)
		{
			DrawGizmos();
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (!alwaysRender)
		{
			DrawGizmos();
		}
	}

	private void DrawGizmos()
	{
		if (renderCornerSpheres)
		{
			DrawCornerSpheres();
		}
		if (renderInnerBox)
		{
			DrawInnerBox();
		}
		if (renderOuterBox)
		{
			DrawOuterBox();
		}
	}

	private void DrawInnerBox()
	{
		Gizmos.color = new Color(0.3f, 0f, 0.3f, 0.3f);
		DrawOrientedCube(size);
	}

	private void DrawOuterBox()
	{
		Gizmos.color = new Color(0.3f, 0f, 0.3f, 0.3f);
		Vector3 vector = size + 2f * new Vector3(estimatedAudioSize, estimatedAudioSize, estimatedAudioSize);
		DrawOrientedCube(vector);
	}

	private void DrawOrientedCube(Vector3 size)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
		Gizmos.DrawCube(Vector3.zero, size);
		Gizmos.matrix = matrix;
	}

	private void DrawCornerSpheres()
	{
		Vector3 b = HalfWidth * base.transform.right;
		Vector3 b2 = HalfHeight * base.transform.up;
		Vector3 b3 = HalfDepth * base.transform.forward;
		Gizmos.color = new Color(0.2f, 0f, 0.4f, 0.3f);
		Gizmos.DrawSphere(base.transform.position + b + b2 + b3, estimatedAudioSize);
		Gizmos.DrawSphere(base.transform.position + b - b2 + b3, estimatedAudioSize);
		Gizmos.DrawSphere(base.transform.position - b + b2 + b3, estimatedAudioSize);
		Gizmos.DrawSphere(base.transform.position - b - b2 + b3, estimatedAudioSize);
		Gizmos.DrawSphere(base.transform.position + b + b2 - b3, estimatedAudioSize);
		Gizmos.DrawSphere(base.transform.position + b - b2 - b3, estimatedAudioSize);
		Gizmos.DrawSphere(base.transform.position - b + b2 - b3, estimatedAudioSize);
		Gizmos.DrawSphere(base.transform.position - b - b2 - b3, estimatedAudioSize);
	}
}
