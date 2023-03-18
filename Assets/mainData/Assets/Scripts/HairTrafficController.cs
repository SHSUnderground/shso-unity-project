using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairTrafficController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public const float AUTO_HIDE_TIMEOUT = 15f;

	protected const float HEIGHT_ABOVE_HEAD = 1f;

	private const float hairLerpSpeed = 7f;

	private const int animChangeFrameRate = 3;

	public GameObject player;

	public PlayerBillboard billboard;

	public int playerId = -1;

	private HeroOffsetData.OffsetMode currentOffsetMode;

	private Hashtable masterHeroOffsetTable = HeroOffsetData.AllHeroOffsetData;

	private GameObject hairTrafficPoint;

	private DirectedMenuChat CurrentMenuChat;

	private PlayerStatus currentStatus;

	private OpenChatBubbleManager currentChat;

	private AnimClipManager apm = new AnimClipManager();

	private AnimClip autoHide;

	private Animation animationComponent;

	private string currAnimation = string.Empty;

	private string currHero = string.Empty;

	private float currentOffset;

	private float currentDefaultOffset;

	private float offsetTimer;

	private float animationTimer;

	private float offsetDestination;

	private bool initOffset;

	private int frameCount;

	private float offsetThresh = 0.05f;

	public int billboardDisableCount;

	private SpawnData spawnData;

	private SpawnData.PlayerType playerType;

	private string playerName;

	private bool init;

	private Transform[] allTransforms;

	private Transform baseTransform;

	private AnimClip toggelBillboard;

	private float currentBillboardAlpha = 1f;

	public HeroOffsetData.OffsetMode CurrentOffsetMode
	{
		get
		{
			return currentOffsetMode;
		}
	}

	public Hashtable MasterHeroOffsetTable
	{
		get
		{
			return masterHeroOffsetTable;
		}
	}

	public GameObject HairTrafficPoint
	{
		get
		{
			if (hairTrafficPoint == null)
			{
				hairTrafficPoint = new GameObject("Hair Traffic Point");
				hairTrafficPoint.transform.parent = base.gameObject.transform;
				hairTrafficPoint.transform.localPosition = new Vector3(0f, 1f, 0f);
			}
			return hairTrafficPoint;
		}
	}

	public void BeginningDirectedMenuChat(DirectedMenuChat CurrentMenuChat)
	{
		HideStatus();
		HideOpenChat();
		this.CurrentMenuChat = CurrentMenuChat;
		ToggleBillboard(false);
		BeginAutoHide();
	}

	public void BeginningStatus(PlayerStatus currentStatus)
	{
		HideDirectedMenuChat();
		HideOpenChat();
		this.currentStatus = currentStatus;
		ToggleBillboard(false);
		CancelAutoHide();
	}

	public void BeginningOpenChat(OpenChatBubbleManager currentChat)
	{
		HideDirectedMenuChat();
		HideStatus();
		HideOpenChat();
		this.currentChat = currentChat;
		ToggleBillboard(false);
		CancelAutoHide();
	}

	private void BeginAutoHide()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		AnimClip animClip = SHSAnimations.Generic.Wait(15f);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			HideDirectedMenuChat();
			HideStatus();
			ToggleBillboard(true);
		};
		apm.SwapOut(ref autoHide, animClip);
	}

	private void CancelAutoHide()
	{
		apm.RemoveIfUnfinished(autoHide);
	}

	public void HideDirectedMenuChat()
	{
		if (CurrentMenuChat != null)
		{
			CurrentMenuChat.HideSpeechBubble();
		}
		CurrentMenuChat = null;
	}

	public void HideStatus()
	{
		if (currentStatus != null)
		{
			currentStatus.HideStatus();
		}
		currentStatus = null;
	}

	public void HideOpenChat()
	{
		if (currentChat != null)
		{
			ToggleBillboard(true);
			currentChat.HideAllChat();
		}
		currentChat = null;
	}

	private void OnEnable()
	{
		AppShell.Instance.EventMgr.AddListener<GameWorldOpenChatMessage>(OnOpenChat);
		AppShell.Instance.EventMgr.AddListener<GameWorldMenuChatMessage>(OnMenuChat);
		AppShell.Instance.EventMgr.AddListener<RequestChangeBehaviorMessage>(OnChangeBehavior);
	}

	private void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<GameWorldOpenChatMessage>(OnOpenChat);
		AppShell.Instance.EventMgr.RemoveListener<GameWorldMenuChatMessage>(OnMenuChat);
		AppShell.Instance.EventMgr.RemoveListener<RequestChangeBehaviorMessage>(OnChangeBehavior);
	}

	public float GetHairOffset()
	{
		if (!init)
		{
			init = true;
			baseTransform = Utils.FindNodeInChildren(player.transform, "motion_transfer");
			if (baseTransform != null)
			{
				allTransforms = baseTransform.GetComponentsInChildren<Transform>();
			}
		}
		float num = float.MinValue;
		Transform[] array = allTransforms;
		foreach (Transform transform in array)
		{
			Vector3 position = transform.position;
			if (position.y > num)
			{
				Vector3 position2 = transform.position;
				num = position2.y;
			}
		}
		float num2 = num;
		Vector3 position3 = baseTransform.position;
		return num2 - position3.y;
	}

	public float GetYOffset(GameObject playerObject)
	{
		HeroOffsetData heroOffsetData = (HeroOffsetData)masterHeroOffsetTable[playerObject.name];
		currentDefaultOffset = heroOffsetData.DefaultOffset;
		if (animationComponent == null)
		{
			animationComponent = (Animation)playerObject.GetComponent(typeof(Animation));
		}
		if (currHero != playerObject.name)
		{
			currHero = playerObject.name;
			initOffset = false;
		}
		if (!initOffset)
		{
			initOffset = true;
			currentOffset = heroOffsetData.DefaultOffset;
			offsetDestination = heroOffsetData.DefaultOffset;
		}
		if (frameCount >= 3)
		{
			foreach (AnimationState item in animationComponent)
			{
				if (animationComponent.IsPlaying(item.name))
				{
					if (currAnimation != item.name)
					{
						animationTimer = 0f;
					}
					currAnimation = item.name;
					break;
				}
			}
			bool flag = heroOffsetData.DoesAscendIntervalExistForAnimation(HeroOffsetData.OffsetMode.Normal, currAnimation);
			bool flag2 = heroOffsetData.DoesDescendIntervalExistForAnimation(HeroOffsetData.OffsetMode.Normal, currAnimation);
			bool flag3 = heroOffsetData.DoesOffsetExistForAnimation(currentOffsetMode, currAnimation);
			bool flag4 = false;
			bool flag5 = false;
			if (flag)
			{
				float ascendTimeInterval = heroOffsetData.GetAscendTimeInterval(HeroOffsetData.OffsetMode.Normal, currAnimation);
				if (animationTimer >= ascendTimeInterval)
				{
					flag4 = true;
				}
			}
			else
			{
				flag4 = true;
			}
			if (flag2)
			{
				float descendTimeInterval = heroOffsetData.GetDescendTimeInterval(HeroOffsetData.OffsetMode.Normal, currAnimation);
				if (animationTimer >= descendTimeInterval)
				{
					flag5 = true;
				}
			}
			if (!flag5)
			{
				if (flag4)
				{
					if (flag3)
					{
						offsetDestination = heroOffsetData.GetAnimationOffset(currentOffsetMode, currAnimation);
					}
					else
					{
						offsetDestination = heroOffsetData.DefaultOffset;
					}
					offsetTimer = 0f;
				}
			}
			else
			{
				offsetTimer = 0f;
				offsetDestination = heroOffsetData.DefaultOffset;
			}
			frameCount = 0;
		}
		return currentOffset;
	}

	public void OnChangeBehavior(RequestChangeBehaviorMessage msg)
	{
		if (msg.PlayerId == playerId && animationComponent != null)
		{
			string b = string.Empty;
			foreach (AnimationState item in animationComponent)
			{
				if (animationComponent.IsPlaying(item.name))
				{
					b = item.name;
					break;
				}
			}
			if (currAnimation == b)
			{
				animationTimer = 0f;
				offsetTimer = 0f;
				offsetDestination = currentDefaultOffset;
			}
		}
	}

	public void Update()
	{
		apm.Update(Time.deltaTime);
		animationTimer += Time.deltaTime;
		offsetTimer += Time.deltaTime * 7f;
		if (offsetTimer <= 1f)
		{
			currentOffset = Mathf.Lerp(currentOffset, offsetDestination, offsetTimer);
		}
		frameCount++;
	}

	public float GetSmoothHairOffset(float lastOffset)
	{
		float num = GetHairOffset();
		float f = num - lastOffset;
		if (Mathf.Abs(f) > offsetThresh)
		{
			num = lastOffset + offsetThresh * Mathf.Sign(f);
		}
		return num;
	}

	public void OnMenuChat(GameWorldMenuChatMessage msg)
	{
		if (!(msg.player != base.gameObject))
		{
			if (spawnData == null)
			{
				spawnData = Utils.GetComponent<SpawnData>(player);
			}
			if (spawnData != null)
			{
				spawnData.GetSquadRelation(out playerName, out playerType, false);
				OpenChatMessage msg2 = CommunicationManager.ProcessChatMessage(msg, playerName, playerId, playerType);
				AppShell.Instance.EventMgr.Fire(this, msg2);
				OpenChatBubbleManager.SetChatMessage(player, msg2);
			}
		}
	}

	public void OnOpenChat(GameWorldOpenChatMessage msg)
	{
		CspUtils.DebugLog("OnOpenChat playerId=" + playerId + " msg.sendingPlayerId=" + msg.sendingPlayerId);
		CspUtils.DebugLog(Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ParentalOpenChatDeny));
		CspUtils.DebugLog(Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.OpenChatAllow));
		
		if (msg.sendingPlayerId == playerId && Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ParentalOpenChatDeny) && Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.OpenChatAllow))
		{
			if (spawnData == null)
			{
				spawnData = Utils.GetComponent<SpawnData>(player);
			}
			if (spawnData != null)
			{
				spawnData.GetSquadRelation(out playerName, out playerType, false);
				CspUtils.DebugLog("OnOpenChat playerName=" + playerName);
				
				OpenChatMessage msg2 = CommunicationManager.ProcessChatMessage(msg, playerName, playerType);
				AppShell.Instance.EventMgr.Fire(this, msg2);
				OpenChatBubbleManager.SetChatMessage(player, msg2);
			}
		}
	}

	public void ToggleBillboard(bool state)
	{
		ToggleBillboard(state, 0.5f);
	}

	public void ToggleBillboard(bool state, float animTime)
	{
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Expected O, but got Unknown
		if (billboard == null)
		{
			return;
		}
		billboardDisableCount += ((!state) ? 1 : (-1));
		billboardDisableCount = Mathf.Max(billboardDisableCount, 0);
		if ((!state || billboardDisableCount <= 0) && (state || billboardDisableCount == 1))
		{
			if (state)
			{
				Utils.ActivateTree(billboard.gameObject, true);
				billboard.UpdateRelation();
			}
			Renderer[] components = Utils.GetComponents<Renderer>(billboard, Utils.SearchChildren);
			List<Material> materials = new List<Material>();
			Renderer[] array = components;
			foreach (Renderer renderer in array)
			{
				materials.Add(renderer.material);
			}
			float num = currentBillboardAlpha;
			float num2 = state ? 1 : 0;
			float time = SHSAnimations.GenericFunctions.FrationalTime((!state) ? 1 : 0, num2, num, animTime);
			AnimClip animClip = AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(num, num2, time), delegate(float x)
			{
				currentBillboardAlpha = x;
				SetAlpha(materials, x);
			});
			if (!state)
			{
				animClip.OnFinished += (Action)(object)(Action)delegate
				{
					Utils.ActivateTree(billboard.gameObject, false);
					apm.Remove(toggelBillboard);
				};
			}
			billboard.UpdateRenderers();
			apm.SwapOut(ref toggelBillboard, animClip);
		}
	}

	private void SetAlpha(List<Material> mats, float alpha)
	{
		foreach (Material mat in mats)
		{
			Color color = mat.color;
			color.a = alpha;
			mat.color = color;
		}
	}
}
