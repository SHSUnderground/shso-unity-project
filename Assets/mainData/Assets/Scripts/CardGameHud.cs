using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameHud : OrthographicHud
{
	public enum LightningType
	{
		lightning_curvey_tex,
		lightning_purple_tex,
		lightning_tex,
		lightningforked_tex,
		storm_poweremote1_lightning_tex,
		thorlightning_add_tex,
		thorlightning2_add_tex,
		thorlightning3_add_tex
	}

	private List<MeshOpacity> BaseHudOpacity;

	private MeshOpacity CoinPanelOpacity;

	private GameObject CardGameCoin;

	private Transform powerIndicatorWaypoint;

	private GameObject CenterPanel;

	public void Initialize(CardGamePlayer[] players)
	{
		MeshOpacity meshOpacity = null;
		BaseHudOpacity = new List<MeshOpacity>();
		PlayerPanelProperties component;
		for (int i = 0; i < players.Length; i++)
		{
			CspUtils.DebugLog("Initializing panels for player " + players[i].Info.PlayerID);
			GameObject original = (i != 0) ? CardGameController.Instance.opponentHudPrefab : CardGameController.Instance.playerHudPrefab;
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			if (gameObject == null)
			{
				CspUtils.DebugLogError("Failed to instantiate HUD panel");
			}
			Utils.AttachGameObject(Camera.gameObject, gameObject);
			component = Utils.GetComponent<PlayerPanelProperties>(gameObject);
			component.Initialize(Camera);
			Transform transform = gameObject.transform.Find("SidePanel");
			players[i].SidePanel = transform.gameObject.GetComponent<RetreatingPanel>();
			Transform transform2 = transform.transform.Find("DeckPanel/CardLayout_Stock");
			players[i].Stock.GameObj = transform2.gameObject;
			transform2 = transform.transform.Find("DeckPanel/DeckCounter");
			players[i].deckCounterComponent = Utils.GetComponent<DeckCounter>(transform2.gameObject);
			meshOpacity = Utils.GetComponent<MeshOpacity>(transform2.parent.gameObject);
			if (meshOpacity != null)
			{
				BaseHudOpacity.Add(meshOpacity);
			}
			transform2 = transform.transform.Find("DiscardPanel/CardLayout_Waste");
			players[i].Discard.GameObj = transform2.gameObject;
			meshOpacity = Utils.GetComponent<MeshOpacity>(transform2.parent.gameObject);
			if (meshOpacity != null)
			{
				BaseHudOpacity.Add(meshOpacity);
			}
			Transform transform3 = gameObject.transform.Find("EdgePanel");
			players[i].EdgePanel = transform3.gameObject.GetComponent<RetreatingPanel>();
			Transform transform4 = transform3.transform.Find("PlayPanel");
			players[i].PlayPanel = transform4.gameObject.GetComponent<RetreatingPanel>();
			transform2 = transform3.transform.Find("HandPanel");
			meshOpacity = Utils.GetComponent<MeshOpacity>(transform2.gameObject);
			if (meshOpacity != null)
			{
				BaseHudOpacity.Add(meshOpacity);
			}
			transform2 = transform2.transform.Find("CardLayout_Hand");
			players[i].Hand.GameObj = transform2.gameObject;
			transform2 = transform3.transform.Find("KeeperPanel");
			players[i].KeeperTrayOpacity = transform2.gameObject.GetComponent<MeshOpacity>();
			transform2 = transform2.transform.Find("CardLayout_Keepers");
			players[i].Keepers.GameObj = transform2.gameObject;
			transform2 = transform3.transform.Find("PlayPanel");
			Transform transform5 = transform2.transform.Find("CardLayout_Show");
			players[i].Show.GameObj = transform5.gameObject;
			Transform transform6 = transform2.transform.Find("CardLayout_Played");
			players[i].Played.GameObj = transform6.gameObject;
			GameObject gameObject2 = transform.transform.Find("JeopardyMeter").gameObject;
			transform2 = gameObject2.transform.Find("CardLayout_Jeopardy");
			players[i].Jeopardy.GameObj = transform2.gameObject;
			JeopardyMeter component2 = gameObject2.GetComponent<JeopardyMeter>();
			component2.Initialize(players[i]);
			players[i].JeopardyMeterScript = component2;
			transform2 = gameObject.transform.Find("CardLayout_Hidden");
			players[i].Hidden.GameObj = transform2.gameObject;
		}
		CenterPanel = (UnityEngine.Object.Instantiate(CardGameController.Instance.centerPanelPrefab) as GameObject);
		Utils.AttachGameObject(Camera.gameObject, CenterPanel);
		component = Utils.GetComponent<PlayerPanelProperties>(CenterPanel);
		component.Initialize(Camera);
		GameObject gameObject3 = UnityEngine.Object.Instantiate(CardGameController.Instance.powerPanelPrefab) as GameObject;
		Utils.AttachGameObject(Camera.gameObject, gameObject3);
		component = Utils.GetComponent<PlayerPanelProperties>(gameObject3);
		component.Initialize(Camera);
		MeshOpacity component3 = gameObject3.GetComponent<MeshOpacity>();
		component3.alpha = 0f;
		BaseHudOpacity.Add(component3);
		powerIndicatorWaypoint = gameObject3.transform.Find("PowerIndicator_waypoint");
		gameObject3 = (UnityEngine.Object.Instantiate(CardGameController.Instance.coinPanelPrefab) as GameObject);
		Utils.AttachGameObject(Camera.gameObject, gameObject3);
		component = Utils.GetComponent<PlayerPanelProperties>(gameObject3);
		component.Initialize(Camera);
		CardGameCoin = gameObject3.transform.FindChild("PowerCoin").gameObject;
		CoinPanelOpacity = gameObject3.GetComponent<MeshOpacity>();
	}

	public void ShowCoinPanel(bool show)
	{
		CoinPanelOpacity.alpha = (show ? 1 : 0);
	}

	public void ShowPanels(float animTime)
	{
		AnimPath path = AnimPath.Linear(0f, 1f, animTime);
		AnimationClipManager.Add(new AnimClipFunction(path, delegate(float t)
		{
			foreach (MeshOpacity item in BaseHudOpacity)
			{
				item.alpha = t;
			}
		}));
		AppShell.Instance.EventMgr.Fire(this, new CardGameEvent.ShowCardGameHud());
	}

	public void HidePanels(float animTime)
	{
		AnimPath path = AnimPath.Linear(1f, 0f, animTime);
		AnimationClipManager.Add(new AnimClipFunction(path, delegate(float t)
		{
			foreach (MeshOpacity item in BaseHudOpacity)
			{
				item.alpha = t;
			}
		}));
	}

	public void FlipCoin(bool heads, bool showPlusOne)
	{
		StartCoroutine(CoFlipCoin(heads, showPlusOne));
	}

	protected IEnumerator CoFlipCoin(bool heads, bool showPlusOne)
	{
		GameObject powerupEffect = null;
		GameObject plusOneEffect = null;
		string animName = (!heads) ? "coin_tails_arc" : ((!showPlusOne) ? "coin_heads_no_power_arc" : "coin_heads_arc");
		GameObject coin = CardGameCoin.transform.FindChild(animName).gameObject;
		Animation anim = coin.GetComponent<Animation>();
		anim.Rewind();
		anim.Play();
		CardGameController.Instance.AudioManager.Play(CardGameAudioManager.SFX.CoinToss);
		if (heads && showPlusOne)
		{
			yield return new WaitForSeconds(1f);
			UnityEngine.Object powerupEffectPrefab = CardGameController.Instance.GetEffectByName("CoinPowerup_Prefab");
			powerupEffect = (UnityEngine.Object.Instantiate(powerupEffectPrefab) as GameObject);
			Utils.AttachGameObject(coin.transform.parent.parent.gameObject, powerupEffect);
			powerupEffect.transform.position = coin.transform.position;
			yield return new WaitForSeconds(1.3f);
			anim.Stop();
			float animTime = 0.5f;
			float timer = 0f;
			Vector3 startPos = coin.transform.position;
			while (timer < animTime)
			{
				Vector3 pos = Vector3.Lerp(startPos, powerIndicatorWaypoint.position, timer / animTime);
				coin.transform.position = pos;
				timer += Time.deltaTime;
				if (timer > 0.375f && plusOneEffect == null)
				{
					UnityEngine.Object plusOnePrefab = CardGameController.Instance.GetEffectByName("PlusOne_Prefab");
					plusOneEffect = (UnityEngine.Object.Instantiate(plusOnePrefab) as GameObject);
					Utils.AttachGameObject(coin.transform.parent.parent.gameObject, plusOneEffect);
					plusOneEffect.transform.position = coin.transform.position;
					plusOneEffect.transform.localScale = new Vector3(20f, 20f, 1f);
				}
				yield return 0;
			}
			coin.transform.position = powerIndicatorWaypoint.position;
			UnityEngine.Object.Destroy(powerupEffect);
			if (showPlusOne)
			{
				UnityEngine.Object.Destroy(plusOneEffect, 0.8f);
			}
			CardGameController.Instance.SetPower(CardGameController.Instance.powerLevel + 1);  // added by CSP
		}
		else
		{
			yield return new WaitForSeconds(3.25f);
		}
		AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.AnimFinished());
	}

	public void PlayCardClash(BattleCard weapon, BattleCard blocker)
	{
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Expected O, but got Unknown
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Expected O, but got Unknown
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Expected O, but got Unknown
		Vector3 position = weapon.CardObj.transform.position;
		Vector3 position2 = blocker.CardObj.transform.position;
		Vector3 position3 = CenterPanel.transform.FindChild("lightning_0").position;
		Vector3 position4 = CenterPanel.transform.FindChild("lightning_1").position;
		UnityEngine.Object effectByName = CardGameController.Instance.GetEffectByName("CardClash_Prefab");
		GameObject effect0 = UnityEngine.Object.Instantiate(effectByName) as GameObject;
		GameObject effect = UnityEngine.Object.Instantiate(effectByName) as GameObject;
		LineRenderer component = effect0.GetComponent<LineRenderer>();
		LineRenderer component2 = effect.GetComponent<LineRenderer>();
		component.SetPosition(0, position);
		component2.SetPosition(0, position2);
		component.SetPosition(1, position3);
		component2.SetPosition(1, position4);
		MeshOpacity lightningOpacity0 = effect0.GetComponent<MeshOpacity>();
		MeshOpacity lightningOpacity = effect.GetComponent<MeshOpacity>();
		float time = 0.1f;
		float time2 = 0.8f;
		float time3 = 0.1f;
		AnimClip animClip = new AnimClipFunction(AnimPath.Linear(0f, 1f, time), delegate(float a)
		{
			lightningOpacity0.alpha = a;
			lightningOpacity.alpha = a;
		});
		AnimClip hold = new AnimClipFunction(AnimPath.Linear(0f, 0f, time2), delegate
		{
		});
		AnimClip fadeOut = new AnimClipFunction(AnimPath.Linear(1f, 0f, time3), delegate(float a)
		{
			lightningOpacity0.alpha = a;
			lightningOpacity.alpha = a;
		});
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			AnimationClipManager.Add(hold);
		};
		hold.OnFinished += (Action)(object)(Action)delegate
		{
			AnimationClipManager.Add(fadeOut);
		};
		fadeOut.OnFinished += (Action)(object)(Action)delegate
		{
			UnityEngine.Object.Destroy(effect0);
			UnityEngine.Object.Destroy(effect);
		};
		AnimationClipManager.Add(animClip);
	}
}
