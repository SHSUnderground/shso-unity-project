using System.Collections;
using UnityEngine;

public class CoinsTriggerAdaptor : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject emitterPrefab;

	public int coinAmount = 3;

	public float range = 3f;

	public float height = 4f;

	public float spacing = 5f;

	public float delay;

	public float force = 250f;

	public bool usePhysics = true;

	public string emitterData = string.Empty;

	public float emitterDelay;

	public bool emitOnPoke;

	public string coinPrefabName = "SHS_coin_pickup_prefab";

	public GameObject Owner;

	private Object emitterInstance;

	private CoinEmitterBase emitter;

	public EffectSequence pokeEmptyContainerSequence;

	public void Triggered()
	{
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		BehaviorManager component = Utils.GetComponent<BehaviorManager>(localPlayer);
		if (localPlayer == null || component == null)
		{
			return;
		}
		if (emitOnPoke)
		{
			BehaviorEmote behaviorEmote = component.requestChangeBehavior(typeof(BehaviorEmote), false) as BehaviorEmote;
			if (behaviorEmote != null)
			{
				behaviorEmote.Initialize(EmotesDefinition.Instance.GetEmoteByCommand("poke").id, true, false, OnPokeEvent);
			}
		}
		else
		{
			SpawnCoins();
		}
	}

	private void OnPokeEvent(EffectSequence seq, EventEffect effect)
	{
		if (effect.EventName.ToLower() == "turnon")
		{
			SpawnCoins();
		}
	}

	private void SpawnCoins()
	{
		CoinViewState component = Utils.GetComponent<CoinViewState>(base.transform.parent.gameObject, Utils.SearchChildren);
		if (component != null)
		{
			if (!component.CanPlayerUse(null))
			{
				return;
			}
			if (component.IsReadyToSpew())
			{
				if (AppShell.Instance != null && AppShell.Instance.EventMgr != null)
				{
					if (base.transform.parent.gameObject.name.ToLowerInvariant().Contains("tree"))
					{
						AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "poke_tree", 1, string.Empty);
					}
					else
					{
						AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "poke_gumball", 1, string.Empty);
					}
				}
				component.Reset();
			}
			else
			{
				GameObject g = Object.Instantiate(pokeEmptyContainerSequence.gameObject) as GameObject;
				EffectSequence component2 = Utils.GetComponent<EffectSequence>(g);
				if (component2 != null)
				{
					GameObject gameObject = Owner;
					if (gameObject == null)
					{
						gameObject = base.gameObject;
					}
					component2.Initialize(gameObject, delegate(EffectSequence killseq)
					{
						Object.Destroy(killseq.gameObject);
					}, null);
					component2.StartSequence();
					return;
				}
			}
		}
		if (emitterPrefab == null)
		{
			CspUtils.DebugLog("No associated coin emitter prefab, so no coins for y'all.");
		}
		else
		{
			StartCoroutine(StartEmitter());
		}
	}

	private IEnumerator StartEmitter()
	{
		yield return new WaitForSeconds(emitterDelay);
		emitterInstance = Object.Instantiate(emitterPrefab);
		emitter = Utils.GetComponent<CoinEmitterBase>((GameObject)emitterInstance);
		GameObject anchorGO = null;
		foreach (Transform trans in base.gameObject.transform)
		{
			if (trans.name == "EmitPoint")
			{
				anchorGO = trans.gameObject;
				break;
			}
		}
		if (anchorGO == null)
		{
			anchorGO = base.gameObject;
		}
		StartCoroutine(emitter.InitEmitter(base.gameObject, anchorGO, this));
	}
}
