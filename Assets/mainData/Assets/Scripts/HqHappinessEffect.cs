using System.Runtime.CompilerServices;
using UnityEngine;

public class HqHappinessEffect : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public HappinessLevelInfo[] PausedDisplay = new HappinessLevelInfo[6]
	{
		AIControllerHQ.Mood.Happy,
		AIControllerHQ.Mood.Pleasant,
		AIControllerHQ.Mood.Content,
		AIControllerHQ.Mood.Indifferent,
		AIControllerHQ.Mood.Disgruntled,
		AIControllerHQ.Mood.Enraged
	};

	public HappinessLevelInfo[] PlayDisplay = new HappinessLevelInfo[6]
	{
		AIControllerHQ.Mood.Happy,
		AIControllerHQ.Mood.Pleasant,
		AIControllerHQ.Mood.Content,
		AIControllerHQ.Mood.Indifferent,
		AIControllerHQ.Mood.Disgruntled,
		AIControllerHQ.Mood.Enraged
	};

	protected AIControllerHQ.Mood currentMood;

	protected GameObject currentParticleSystem;

	protected AIControllerHQ aiController;

	[CompilerGenerated]
	private bool _003CPaused_003Ek__BackingField;

	public AIControllerHQ AiController
	{
		set
		{
			if (aiController == null)
			{
				aiController = value;
				SetParent();
				Mood = aiController.CurrentMood;
				base.gameObject.name = aiController.CharacterName + "_mood_icon";
			}
		}
	}

	public bool Paused
	{
		[CompilerGenerated]
		private get
		{
			return _003CPaused_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CPaused_003Ek__BackingField = value;
		}
	}

	protected AIControllerHQ.Mood Mood
	{
		set
		{
			if (currentParticleSystem != null)
			{
				Utils.ActivateTree(currentParticleSystem, false);
			}
			currentMood = value;
			HappinessLevelInfo[] array = (!aiController.Paused) ? PlayDisplay : PausedDisplay;
			currentParticleSystem = null;
			HappinessLevelInfo[] array2 = array;
			int num = 0;
			HappinessLevelInfo happinessLevelInfo;
			while (true)
			{
				if (num < array2.Length)
				{
					happinessLevelInfo = array2[num];
					if (happinessLevelInfo.mood == currentMood)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			currentParticleSystem = happinessLevelInfo.particleSystem;
		}
	}

	public void Start()
	{
		HappinessLevelInfo[] pausedDisplay = PausedDisplay;
		foreach (HappinessLevelInfo happinessLevelInfo in pausedDisplay)
		{
			if (happinessLevelInfo.particleSystem != null)
			{
				happinessLevelInfo.particleSystem.active = false;
			}
		}
		HappinessLevelInfo[] playDisplay = PlayDisplay;
		foreach (HappinessLevelInfo happinessLevelInfo2 in playDisplay)
		{
			if (happinessLevelInfo2.particleSystem != null)
			{
				happinessLevelInfo2.particleSystem.active = false;
			}
		}
	}

	public void Update()
	{
		if (aiController == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (currentMood != aiController.CurrentMood || Paused != aiController.Paused)
		{
			Mood = aiController.CurrentMood;
			Paused = aiController.Paused;
		}
		if (!(currentParticleSystem != null))
		{
			return;
		}
		if (!aiController.IsInActiveRoom || (aiController.gameObject.renderer != null && !aiController.gameObject.renderer.enabled))
		{
			currentParticleSystem.active = false;
		}
		else if (!aiController.IsFlingaObjectActive && aiController.IsInActiveRoom)
		{
			if (currentParticleSystem.particleEmitter != null)
			{
				currentParticleSystem.particleEmitter.enabled = true;
			}
			currentParticleSystem.active = true;
			currentParticleSystem.renderer.enabled = true;
			base.gameObject.transform.rotation = Quaternion.identity;
		}
	}

	protected void SetParent()
	{
		if (aiController == null)
		{
			return;
		}
		Transform transform = Utils.FindNodeInChildren(aiController.gameObject.transform, "Head");
		base.gameObject.transform.rotation = Quaternion.identity;
		if (transform != null)
		{
			base.gameObject.transform.parent = transform;
			base.gameObject.transform.localPosition = Vector3.zero;
			return;
		}
		Transform currentHighest = Utils.FindNodeInChildren(aiController.gameObject.transform, "motion_transfer");
		if (currentHighest != null)
		{
			FindHighestTransform(currentHighest, ref currentHighest);
		}
		else
		{
			FindHighestTransform(aiController.gameObject.transform, ref currentHighest);
		}
		CspUtils.DebugLog("Could not find Head bone for " + aiController.CharacterName + " using bone " + currentHighest.gameObject.name);
		transform = Utils.FindNodeInChildren(aiController.gameObject.transform, "motion_export");
		if (transform != null)
		{
			base.gameObject.transform.parent = transform;
			Transform transform2 = base.gameObject.transform;
			Vector3 position = transform.position;
			float x = position.x;
			Vector3 position2 = currentHighest.position;
			float y = position2.y;
			Vector3 position3 = transform.position;
			transform2.position = new Vector3(x, y, position3.z);
		}
		else
		{
			CspUtils.DebugLog("Could not find transform to attach to!");
		}
	}

	protected void FindHighestTransform(Transform parent, ref Transform currentHighest)
	{
		if (!(currentHighest == null))
		{
			Vector3 position = parent.position;
			float y = position.y;
			Vector3 position2 = currentHighest.position;
			if (!(y > position2.y))
			{
				goto IL_0032;
			}
		}
		currentHighest = parent;
		goto IL_0032;
		IL_0032:
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			FindHighestTransform(child, ref currentHighest);
		}
	}
}
