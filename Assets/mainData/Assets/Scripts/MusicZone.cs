using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("Audio/Music Zone")]
public class MusicZone : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public ShsAudioSource audioSrc;

	public Vector3 size = new Vector3(6f, 6f, 6f);

	public AnimationCurve rolloffCurve;

	public bool waitForPlayerSpawn = true;

	public float maxVolumeChangePerSecond = 0.5f;

	public bool alwaysRender;

	public bool renderCornerSpheres;

	public bool renderInnerBox = true;

	public bool renderOuterBox = true;

	private GameObject _cachedPlayer;

	private float _volume;

	private ShsAudioSource audioSrcInstance;

	[CompilerGenerated]
	private float _003CDestinationVolume_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CSuppressedByOtherMusic_003Ek__BackingField;

	protected GameObject Player
	{
		get
		{
			if (_cachedPlayer == null)
			{
				_cachedPlayer = GameController.GetController().LocalPlayer;
			}
			return _cachedPlayer;
		}
	}

	protected float HalfWidth
	{
		get
		{
			return size.x / 2f;
		}
	}

	protected float HalfHeight
	{
		get
		{
			return size.y / 2f;
		}
	}

	protected float HalfDepth
	{
		get
		{
			return size.z / 2f;
		}
	}

	protected float Volume
	{
		get
		{
			return _volume;
		}
		set
		{
			_volume = value;
			if (audioSrcInstance != null)
			{
				audioSrcInstance.Volume = value;
			}
		}
	}

	protected float DestinationVolume
	{
		[CompilerGenerated]
		get
		{
			return _003CDestinationVolume_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CDestinationVolume_003Ek__BackingField = value;
		}
	}

	protected bool SuppressedByOtherMusic
	{
		[CompilerGenerated]
		get
		{
			return _003CSuppressedByOtherMusic_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CSuppressedByOtherMusic_003Ek__BackingField = value;
		}
	}

	protected float EstimatedRolloffSize
	{
		get
		{
			if (rolloffCurve != null && rolloffCurve.length > 0)
			{
				return rolloffCurve[rolloffCurve.length - 1].time;
			}
			return 0f;
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

	private void PlayAudio()
	{
		if (audioSrc != null)
		{
			audioSrcInstance = ShsAudioSource.PlayFromPrefab(audioSrc.gameObject, base.transform, delegate(ShsAudioSource inst)
			{
				inst.RequestsCrossfade = false;
			});
			Utils.AttachGameObject(base.gameObject, audioSrcInstance.gameObject);
			Volume = 0f;
			StartCoroutine(CoAdjustVolume());
			AppShell.Instance.EventMgr.AddListener<MusicChangedMessage>(OnMusicChangedMessage);
		}
	}

	private void LateUpdate()
	{
		if (audioSrcInstance != null && rolloffCurve != null && Player != null)
		{
			if (SuppressedByOtherMusic || Player == null)
			{
				DestinationVolume = 0f;
				return;
			}
			Vector3 closestPoint = GetClosestPoint(Player.transform.position);
			float magnitude = (Player.transform.position - closestPoint).magnitude;
			DestinationVolume = rolloffCurve.Evaluate(magnitude);
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

	private IEnumerator CoAdjustVolume()
	{
		while (true)
		{
			if (audioSrcInstance != null)
			{
				audioSrcInstance.Volume = 0f;
				float maxDelta = maxVolumeChangePerSecond * Time.deltaTime;
				float volumeDelta = Mathf.Clamp(DestinationVolume - Volume, -1f * maxDelta, maxDelta);
				Volume += volumeDelta;
			}
			yield return 0;
		}
	}

	private void OnMusicChangedMessage(MusicChangedMessage e)
	{
		SuppressedByOtherMusic = (e.NewMusic != null);
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
		float estimatedRolloffSize = EstimatedRolloffSize;
		Vector3 vector = size + 2f * new Vector3(estimatedRolloffSize, estimatedRolloffSize, estimatedRolloffSize);
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
		float estimatedRolloffSize = EstimatedRolloffSize;
		Gizmos.DrawSphere(base.transform.position + b + b2 + b3, estimatedRolloffSize);
		Gizmos.DrawSphere(base.transform.position + b - b2 + b3, estimatedRolloffSize);
		Gizmos.DrawSphere(base.transform.position - b + b2 + b3, estimatedRolloffSize);
		Gizmos.DrawSphere(base.transform.position - b - b2 + b3, estimatedRolloffSize);
		Gizmos.DrawSphere(base.transform.position + b + b2 - b3, estimatedRolloffSize);
		Gizmos.DrawSphere(base.transform.position + b - b2 - b3, estimatedRolloffSize);
		Gizmos.DrawSphere(base.transform.position - b + b2 - b3, estimatedRolloffSize);
		Gizmos.DrawSphere(base.transform.position - b - b2 - b3, estimatedRolloffSize);
	}
}
