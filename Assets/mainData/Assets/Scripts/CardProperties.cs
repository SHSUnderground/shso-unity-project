using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardProperties : MonoBehaviour, IInputHandler
{
	public enum CardAnimation
	{
		flip,
		drop,
		play,
		keep,
		down_play,
		down_draw,
		block,
		block_opponent
	}

	public enum CardEffect
	{
		KeeperActivate,
		KeeperMisfire,
		KeeperDestroy,
		HighlightFactor,
		HighlightBlock,
		AttackBlocked
	}

	public CardEffect debugEffect = CardEffect.AttackBlocked;

	public bool playEffect;

	public BattleCard Card;

	public GameObject cardMeshObject;

	public GameObject KeeperActivateEffect;

	public GameObject KeeperMisfireEffect;

	public GameObject KeeperDestroyEffect;

	public GameObject HighlightFactorEffect;

	public GameObject HighlightBlockEffect;

	public GameObject AttackBlockedEffect;

	protected GameObject keeperActivateInstance;

	protected GameObject keeperMisfireInstance;

	protected GameObject keeperDestroyInstance;

	protected GameObject[] highlightFactorInstance;

	protected GameObject highlightBlockInstance;

	protected GameObject attackBlockedInstance;

	public CardAnimation animName = CardAnimation.drop;

	public bool playAnim;

	public bool playAnimReverse;

	public SHSInput.InputRequestorType InputRequestorType
	{
		get
		{
			return SHSInput.InputRequestorType.UI;
		}
	}

	public bool CanHandleInput
	{
		get
		{
			return true;
		}
	}

	public CardProperties()
	{
		Card = null;
		animName = CardAnimation.drop;
		playAnim = false;
		highlightFactorInstance = new GameObject[2];
	}

	public void PlayKeeperActivate(bool show)
	{
		RemoveEffect(ref keeperActivateInstance);
		if (show)
		{
			keeperActivateInstance = (UnityEngine.Object.Instantiate(KeeperActivateEffect) as GameObject);
			Utils.AttachGameObject(cardMeshObject, keeperActivateInstance);
		}
	}

	public void PlayKeeperMisfire(bool show)
	{
		RemoveEffect(ref keeperMisfireInstance);
		if (show)
		{
			keeperMisfireInstance = (UnityEngine.Object.Instantiate(KeeperMisfireEffect) as GameObject);
			Utils.AttachGameObject(cardMeshObject, keeperMisfireInstance);
			StartCoroutine(CoPlayKeeperMisfire());
		}
	}

	private IEnumerator CoPlayKeeperMisfire()
	{
		yield return new WaitForSeconds(2f);
		PlayKeeperMisfire(false);
	}

	public void PlayKeeperDestroy(bool show)
	{
		RemoveEffect(ref keeperDestroyInstance);
		if (show)
		{
			keeperDestroyInstance = (UnityEngine.Object.Instantiate(KeeperDestroyEffect) as GameObject);
			Utils.AttachGameObject(cardMeshObject, keeperDestroyInstance);
			StartCoroutine(CoPlayKeeperDestroy());
		}
	}

	private IEnumerator CoPlayKeeperDestroy()
	{
		yield return new WaitForSeconds(2f);
		PlayKeeperDestroy(false);
	}

	public void PlayHighlightFactor(bool show)
	{
		RemoveEffect(ref highlightFactorInstance[0]);
		RemoveEffect(ref highlightFactorInstance[1]);
		if (!show)
		{
			return;
		}
		highlightFactorInstance[0] = (UnityEngine.Object.Instantiate(HighlightFactorEffect) as GameObject);
		Utils.AttachGameObject(cardMeshObject, highlightFactorInstance[0]);
		BattleCard.Factor[] array = null;
		array = ((Card == null) ? new BattleCard.Factor[1]
		{
			BattleCard.Factor.Tech
		} : Card.AttackFactors);
		string textureHandle = string.Format("{0}_icon", BattleCard.FactorToString(array[0]));
		SetTextureOnEffect(highlightFactorInstance[0], textureHandle);
		if (array.Length == 1)
		{
			Animation componentInChildren = highlightFactorInstance[0].GetComponentInChildren<Animation>();
			if (componentInChildren != null)
			{
				componentInChildren.Play("cardgame_attackhighlight_single");
				componentInChildren.PlayQueued("cardgame_attackhighlight_pulse");
			}
			return;
		}
		highlightFactorInstance[1] = (UnityEngine.Object.Instantiate(HighlightFactorEffect) as GameObject);
		Utils.AttachGameObject(cardMeshObject, highlightFactorInstance[1]);
		textureHandle = string.Format("{0}_icon", BattleCard.FactorToString(array[1]));
		SetTextureOnEffect(highlightFactorInstance[1], textureHandle);
		Animation componentInChildren2 = highlightFactorInstance[0].GetComponentInChildren<Animation>();
		if (componentInChildren2 != null)
		{
			componentInChildren2.Play("cardgame_attackhighlight_dual_top");
			componentInChildren2.PlayQueued("cardgame_attackhighlight_pulse");
		}
		componentInChildren2 = highlightFactorInstance[1].GetComponentInChildren<Animation>();
		if (componentInChildren2 != null)
		{
			componentInChildren2.Play("cardgame_attackhighlight_dual_bottom");
			componentInChildren2.PlayQueued("cardgame_attackhighlight_pulse");
		}
		Utils.AttachGameObject(cardMeshObject, highlightFactorInstance[1]);
	}

	public void PlayHighlightBlock(bool show)
	{
		RemoveEffect(ref highlightBlockInstance);
		if (show)
		{
			highlightBlockInstance = (UnityEngine.Object.Instantiate(HighlightBlockEffect) as GameObject);
			if (Card != null)
			{
				string textureHandle = string.Format("{0}_block", BattleCard.FactorToString(Card.BlockFactors[0]));
				SetTextureOnEffect(highlightBlockInstance, textureHandle);
			}
			Utils.AttachGameObject(cardMeshObject, highlightBlockInstance);
			Animation componentInChildren = highlightBlockInstance.GetComponentInChildren<Animation>();
			if (componentInChildren != null)
			{
				componentInChildren.PlayQueued("cardgame_blockhighlight_pulse");
			}
		}
	}

	public void PlayAttackBlocked()
	{
		RemoveEffect(ref attackBlockedInstance);
		attackBlockedInstance = (UnityEngine.Object.Instantiate(AttackBlockedEffect) as GameObject);
		EffectSequence componentInChildren = attackBlockedInstance.GetComponentInChildren<EffectSequence>();
		if (componentInChildren != null)
		{
			componentInChildren.Initialize(attackBlockedInstance, delegate
			{
				UnityEngine.Object.Destroy(attackBlockedInstance);
			}, null);
			string textureHandle = string.Format("{0}_block", BattleCard.FactorToString(Card.BlockFactors[0]));
			SetTextureOnEffect(attackBlockedInstance, textureHandle);
			Utils.AttachGameObject(cardMeshObject, attackBlockedInstance);
			componentInChildren.StartSequence();
		}
		else
		{
			UnityEngine.Object.Destroy(attackBlockedInstance);
		}
	}

	public void PlayBlockClash(bool mine)
	{
		if (mine)
		{
			Card.Animation.Play("block");
		}
		else
		{
			Card.Animation.Play("block_opponent");
		}
	}

	private void RemoveEffect(ref GameObject effectInstance)
	{
		if (effectInstance != null)
		{
			Utils.DetachGameObject(effectInstance);
			UnityEngine.Object.Destroy(effectInstance);
			effectInstance = null;
		}
	}

	private void AttachOneShotEffect(GameObject effectPrefab)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(effectPrefab) as GameObject;
		Utils.AttachChildrenToGameObject(cardMeshObject, gameObject);
		UnityEngine.Object.Destroy(gameObject);
	}

	private void SetTextureOnEffect(GameObject effectInstance, string textureHandle)
	{
		if (!(CardGameController.Instance == null))
		{
			Texture2D texture2D = CardGameController.Instance.CardGameBundle.Load(textureHandle) as Texture2D;
			if (texture2D == null)
			{
				CspUtils.DebugLog("Could not find " + textureHandle);
				return;
			}
			MeshRenderer componentInChildren = effectInstance.GetComponentInChildren<MeshRenderer>();
			Material material = new Material(componentInChildren.material);
			material.SetTexture("_MainTex", texture2D);
			componentInChildren.material = material;
		}
	}

	public void StopAllEffects()
	{
		PlayKeeperActivate(false);
		PlayKeeperMisfire(false);
		PlayKeeperDestroy(false);
		PlayHighlightFactor(false);
		PlayHighlightBlock(false);
	}

	private void Start()
	{
		MeshRenderer meshRenderer = base.gameObject.GetComponentInChildren(typeof(MeshRenderer)) as MeshRenderer;
		cardMeshObject = meshRenderer.gameObject;
	}

	private void Update()
	{
		if (playAnim)
		{
			Card.Animation.Rewind();
			string text = animName.ToString();
			Card.Animation[text].speed = 1f;
			Card.Animation.Play(text);
			playAnim = false;
		}
		if (playAnimReverse)
		{
			Card.Animation.Rewind();
			string text2 = animName.ToString();
			Card.Animation[text2].speed = -1f;
			Card.Animation[text2].time = Card.Animation[text2].length;
			Card.Animation.Play(text2);
			playAnimReverse = false;
		}
		if (playEffect)
		{
			StopAllEffects();
			switch (debugEffect)
			{
			case CardEffect.AttackBlocked:
				PlayAttackBlocked();
				break;
			case CardEffect.HighlightBlock:
				PlayHighlightBlock(true);
				break;
			case CardEffect.HighlightFactor:
				PlayHighlightFactor(true);
				break;
			case CardEffect.KeeperActivate:
				PlayKeeperActivate(true);
				break;
			case CardEffect.KeeperDestroy:
				PlayKeeperDestroy(true);
				break;
			case CardEffect.KeeperMisfire:
				PlayKeeperMisfire(true);
				break;
			}
			playEffect = false;
		}
	}

	private void OnMouseEnter()
	{
		AppShell.Instance.EventMgr.Fire(this, new CardCollection3DMessage(CardCollection3DMessage.CC3DEvent.CardMouseEnter, base.gameObject));
	}

	private void OnMouseUp()
	{
		if (GameController.GetController().controllerType != GameController.ControllerType.DeckBuilder)
		{
			AppShell.Instance.EventMgr.Fire(this, new CardCollection3DMessage(CardCollection3DMessage.CC3DEvent.CardMouseLeftClick, base.gameObject));
		}
	}

	public Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> GetKeyList(GUIControl.KeyInputState inputState)
	{
		throw new NotImplementedException();
	}

	public void ConfigureKeyBanks()
	{
		throw new NotImplementedException();
	}

	public bool IsDescendantHandler(IInputHandler handler)
	{
		return this == (CardProperties)handler;
	}
}
