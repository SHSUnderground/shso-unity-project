using System.Collections;
using UnityEngine;

public class AIControllerPigeon : EmoteListener
{
	public delegate void TakeoffStartedDelegate(AIControllerPigeon pigeon);

	public delegate void TakeoffFinishedDelegate(AIControllerPigeon pigeon);

	private const float kPassOutDuration = 2f;

	public GameObject onStartled;

	public EffectSequence landingFX;

	private bool hangingOut = true;

	private Animation animationComp;

	private Transform motionExportTransform;

	private Vector3 originalAnimOffset = Vector3.zero;

	private readonly string[] idleAnims = new string[2]
	{
		"movement_idle",
		"movement_idle_2"
	};

	private readonly string[] takeoffAnims = new string[2]
	{
		"fly_takeoff",
		"fly_takeoff_2"
	};

	private readonly string[] landAnims = new string[2]
	{
		"fly_land",
		"fly_land_2"
	};

	private readonly string[] falldownAnims = new string[2]
	{
		"falldown",
		"falldown_2"
	};

	private float idleSwitchTime = -1f;

	public TakeoffStartedDelegate OnTakeoffStarted;

	public TakeoffFinishedDelegate OnTakeoffFinished;

	public bool Idling
	{
		get
		{
			return hangingOut;
		}
	}

	public void RewindAnimation()
	{
		if (animationComp != null)
		{
			animationComp.Rewind();
		}
	}

	public IEnumerator DoLanding()
	{
		if (onStartled != null)
		{
			onStartled.active = false;
		}
		PlayLandingFX();
		if (animationComp != null)
		{
			string animName = landAnims[Random.Range(0, landAnims.Length)];
			animationComp.Play(animName);
			animationComp.wrapMode = WrapMode.ClampForever;
			yield return new WaitForSeconds(animationComp.GetClip(animName).length);
			animationComp.transform.localPosition = originalAnimOffset;
		}
		if (!IsSafeToLand())
		{
			StartCoroutine(Takeoff());
		}
		else
		{
			StartIdling(false);
		}
	}

	public bool IsSafeToLand()
	{
		SphereCollider component = Utils.GetComponent<SphereCollider>(base.gameObject, Utils.SearchChildren);
		if ((bool)component)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, component.radius);
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				if (IsScaryHuman(collider.gameObject))
				{
					return false;
				}
			}
			return true;
		}
		return true;
	}

	public void Awake()
	{
		if (onStartled != null)
		{
			onStartled.active = false;
		}
	}

	public void Start()
	{
		animationComp = Utils.GetComponent<Animation>(base.gameObject, Utils.SearchChildren);
		if (animationComp == null)
		{
			CspUtils.DebugLog("Could not find animation component as a child of " + base.gameObject.name);
			return;
		}
		originalAnimOffset = animationComp.transform.localPosition;
		motionExportTransform = Utils.FindNodeInChildren(base.transform, "motion_export");
		if (motionExportTransform == null)
		{
			motionExportTransform = Utils.FindNodeInChildren(base.transform, "motion export");
			if (motionExportTransform == null)
			{
				CspUtils.DebugLog("Could not find motion export or motion transfer nodes on " + base.gameObject.name);
				return;
			}
		}
		StartIdling();
	}

	public void OnTriggerEnter(Collider other)
	{
		if (hangingOut && IsScaryHuman(other.gameObject))
		{
			StartCoroutine(Takeoff());
		}
	}

	public void Update()
	{
		if (hangingOut && Time.time > idleSwitchTime)
		{
			StartIdling();
		}
		PerformRootMotion();
	}

	protected void PerformRootMotion()
	{
		Vector3 eulerAngles = motionExportTransform.localRotation.eulerAngles;
		float y = eulerAngles.y;
		eulerAngles.y = eulerAngles.z;
		eulerAngles.z = y;
		animationComp.transform.localRotation = Quaternion.Euler(eulerAngles);
		Vector3 localPosition = animationComp.transform.localPosition;
		Vector3 localPosition2 = motionExportTransform.localPosition;
		localPosition.z = 0f - localPosition2.y;
		animationComp.transform.localPosition = localPosition;
	}

	public override void OnEmoteBroadcast(sbyte emoteID, GameObject broadcaster)
	{
		if (EmotesDefinition.Instance.GetEmoteById(emoteID).command == "rude")
		{
			StartCoroutine(DelayedPassOut());
		}
	}

	private bool IsScaryHuman(GameObject obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (Utils.GetComponent<PlayerCombatController>(obj) != null)
		{
			return true;
		}
		SpawnData component = Utils.GetComponent<SpawnData>(obj);
		if (component == null)
		{
			return false;
		}
		return (component.spawnType & CharacterSpawn.Type.Player) != 0;
	}

	private void StartIdling()
	{
		StartIdling(true);
	}

	private void StartIdling(bool crossfade)
	{
		if (animationComp != null)
		{
			string text = idleAnims[Random.Range(0, idleAnims.Length)];
			if (crossfade)
			{
				animationComp.CrossFade(text);
			}
			else
			{
				animationComp.Play(text);
			}
			animationComp.wrapMode = WrapMode.Loop;
			idleSwitchTime = Time.time + animationComp.GetClip(text).length;
		}
		hangingOut = true;
	}

	public IEnumerator Takeoff()
	{
		hangingOut = false;
		if (OnTakeoffStarted != null)
		{
			OnTakeoffStarted(this);
		}
		if (onStartled != null)
		{
			onStartled.active = true;
		}
		if (animationComp != null)
		{
			string animName = takeoffAnims[Random.Range(0, takeoffAnims.Length)];
			animationComp.CrossFade(animName);
			animationComp.wrapMode = WrapMode.Once;
			yield return new WaitForSeconds(animationComp.GetClip(animName).length);
		}
		if (OnTakeoffFinished != null)
		{
			OnTakeoffFinished(this);
		}
	}

	public IEnumerator PassOut()
	{
		if (hangingOut)
		{
			hangingOut = false;
			if (animationComp != null)
			{
				string animName = falldownAnims[Random.Range(0, falldownAnims.Length)];
				animationComp.CrossFade(animName);
				animationComp.wrapMode = WrapMode.ClampForever;
				yield return new WaitForSeconds(animationComp.GetClip(animName).length + 2f);
			}
			yield return StartCoroutine(Takeoff());
		}
	}

	public IEnumerator DelayedPassOut()
	{
		yield return new WaitForSeconds(2f);
		yield return StartCoroutine(PassOut());
	}

	protected void PlayLandingFX()
	{
		if (landingFX != null)
		{
			GameObject gameObject = Object.Instantiate(landingFX.gameObject) as GameObject;
			if (gameObject != null)
			{
				Utils.AttachGameObject(base.gameObject, gameObject);
				EffectSequence component = gameObject.GetComponent<EffectSequence>();
				component.Initialize(base.gameObject, delegate(EffectSequence s)
				{
					Object.Destroy(s.gameObject);
				}, null);
				component.StartSequence();
			}
			else
			{
				CspUtils.DebugLog("Prefab instance is null!  Odd!");
			}
		}
	}
}
