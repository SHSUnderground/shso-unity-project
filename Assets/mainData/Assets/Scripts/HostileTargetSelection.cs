using UnityEngine;

public class HostileTargetSelection : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool PlayAnimation;

	private bool playAnimation;

	private GameObject targetObject;

	private Animation[] AnimComponents;

	private Vector3 HeadOffset;

	private GameObject Arrow;

	private GameObject Base;

	public static readonly float kArrowOffset = 1.5f;

	public GameObject TargetObject
	{
		get
		{
			return targetObject;
		}
	}

	private void Awake()
	{
		Arrow = base.transform.Find("arrow").gameObject;
		if (Arrow == null)
		{
			CspUtils.DebugLog("No arrow child found for HostileTargetSelection!");
		}
		Base = base.transform.Find("base").gameObject;
		if (Base == null)
		{
			CspUtils.DebugLog("No base child found for HostileTargetSelection!");
		}
		AnimComponents = GetComponentsInChildren<Animation>();
		HeadOffset = new Vector3(0f, kArrowOffset, 0f);
		if (AnimComponents == null)
		{
			return;
		}
		Animation[] animComponents = AnimComponents;
		foreach (Animation animation in animComponents)
		{
			if (animation.gameObject.name == "arrow")
			{
				animation["Arrow_loop"].wrapMode = WrapMode.Loop;
				animation["Arrow_add"].speed = 0.5f;
				animation["Arrow_loop"].speed = 0.7f;
			}
			else if (!(animation.gameObject.name == "base"))
			{
			}
		}
	}

	private void Update()
	{
		if (targetObject != null)
		{
			base.transform.position = targetObject.transform.position;
		}
	}

	private void LateUpdate()
	{
		if (PlayAnimation != playAnimation)
		{
			playAnimation = PlayAnimation;
			Play(playAnimation);
		}
		if (!(Arrow != null))
		{
			return;
		}
		Animation[] animComponents = AnimComponents;
		foreach (Animation animation in animComponents)
		{
			if (animation.gameObject.name == "arrow" && animation.isPlaying)
			{
				Vector3 localPosition = Arrow.transform.localPosition;
				localPosition.y += HeadOffset.y;
				Arrow.transform.localPosition = localPosition;
			}
		}
	}

	public void TargetEntity(GameObject go)
	{
		if (targetObject == go)
		{
			return;
		}
		targetObject = go;
		if (!(go == null))
		{
			Transform transform = Utils.FindNodeInChildren(targetObject.transform, "Head");
			if (transform != null)
			{
				HeadOffset = transform.position - targetObject.transform.position;
				HeadOffset.y += 0.25f;
			}
			else
			{
				HeadOffset = new Vector3(0f, kArrowOffset, 0f);
			}
			Play(true);
		}
	}

	public void Play(bool play)
	{
		if (AnimComponents == null)
		{
			return;
		}
		Animation[] animComponents = AnimComponents;
		foreach (Animation animation in animComponents)
		{
			if (play)
			{
				animation.Rewind();
				if (animation.gameObject.name == "arrow")
				{
					animation.Play("Arrow_add");
					animation.Play("Arrow_loop", AnimationPlayMode.Queue);
				}
				else
				{
					animation.Play();
				}
			}
			else
			{
				animation.Stop();
			}
		}
	}
}
