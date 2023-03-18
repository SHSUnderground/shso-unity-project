using System.Collections.Generic;
using UnityEngine;

public class BrawlerOrthographicHud : OrthographicHud
{
	public static readonly string HeroUpEffect = "HeroUpEffect";

	private EffectSequence _heroUpEffectSequence;

	private List<string> _hudEffectPrefabNames;

	private Dictionary<string, GameObject> _hudEffectPrefabMap;

	private AssetBundle _hudBundle;

	private bool _isVisible;

	private bool _heroUpVisible;

	public bool IsVisible
	{
		get
		{
			return _isVisible;
		}
	}

	public void InitializeHudData(DataWarehouse hudData)
	{
		if (hudData == null)
		{
			CspUtils.DebugLog("Brawler Orthographic Hud cannot be initialized due to null hud data");
			return;
		}
		_hudEffectPrefabNames = new List<string>();
		int count = hudData.GetCount("brawler_orthographic_hud_data/hud_effect_prefab");
		for (int i = 0; i < count; i++)
		{
			_hudEffectPrefabNames.Add(hudData.GetString("brawler_orthographic_hud_data/hud_effect_prefab", i));
		}
		if (IsInitialized())
		{
			CreateHud();
		}
	}

	public void InitializeHudBundle(AssetBundle hudBundle)
	{
		_hudBundle = hudBundle;
		if (IsInitialized())
		{
			CreateHud();
		}
	}

	public bool IsInitialized()
	{
		return _hudBundle != null && _hudEffectPrefabNames != null;
	}

	public void ShowHeroUpEffect(bool show)
	{
		_heroUpVisible = show;
		if (!IsVisible)
		{
			return;
		}
		if (show)
		{
			if (_heroUpEffectSequence == null)
			{
				_heroUpEffectSequence = CreateHudEffect(HeroUpEffect);
				if (_heroUpEffectSequence != null)
				{
					_heroUpEffectSequence.Initialize(Camera.gameObject, OnHeroUpEffectSequenceDone, null);
				}
			}
		}
		else if (_heroUpEffectSequence != null)
		{
			_heroUpEffectSequence.StopSequence(false);
			_heroUpEffectSequence = null;
		}
	}

	public void Hide()
	{
		if (IsVisible)
		{
			bool heroUpVisible = _heroUpVisible;
			ShowHeroUpEffect(false);
			_heroUpVisible = heroUpVisible;
			_isVisible = false;
		}
	}

	public void Show()
	{
		if (!IsVisible)
		{
			_isVisible = true;
			ShowHeroUpEffect(_heroUpVisible);
		}
	}

	private void CreateHud()
	{
		_hudEffectPrefabMap = new Dictionary<string, GameObject>();
		foreach (string hudEffectPrefabName in _hudEffectPrefabNames)
		{
			if (_hudEffectPrefabMap.ContainsKey(hudEffectPrefabName))
			{
				CspUtils.DebugLog("BrawlerOrthographicHud::Create() - prefab <" + hudEffectPrefabName + "> is already created and stored in hud");
			}
			else
			{
				GameObject gameObject = _hudBundle.Load(hudEffectPrefabName) as GameObject;
				if (gameObject == null)
				{
					CspUtils.DebugLog("BrawlerOrthographicHud::CreateHud()- prefab <" + hudEffectPrefabName + "> was not found in bundle and will not be created");
				}
				else
				{
					_hudEffectPrefabMap.Add(hudEffectPrefabName, gameObject);
				}
			}
		}
		_hudEffectPrefabNames.Clear();
		_isVisible = true;
	}

	private EffectSequence CreateHudEffect(string hudEffectPrefabName)
	{
		if (_hudEffectPrefabMap == null)
		{
			CspUtils.DebugLog("BrawlerOrthographicHud::CreateHudEffect() - hud has not been created");
			return null;
		}
		if (!_hudEffectPrefabMap.ContainsKey(hudEffectPrefabName))
		{
			return null;
		}
		GameObject gameObject = _hudEffectPrefabMap[hudEffectPrefabName];
		if (gameObject == null)
		{
			CspUtils.DebugLog("BrawlerOrthographicHud::CreateHudEffect() - effect prefab <" + hudEffectPrefabName + "> is null in hud");
			return null;
		}
		GameObject gameObject2 = Object.Instantiate(gameObject) as GameObject;
		if (gameObject2 == null)
		{
			CspUtils.DebugLog("BrawlerOrthographicHud::CreateHudEffect() - failed to instantiate hud effect prefab <" + hudEffectPrefabName + ">");
			return null;
		}
		AttachToHud(gameObject2);
		return gameObject2.GetComponentInChildren<EffectSequence>();
	}

	private void OnHeroUpEffectSequenceDone(EffectSequence heroUpEffectSequence)
	{
		if (heroUpEffectSequence != null && heroUpEffectSequence.transform.parent != null)
		{
			Object.Destroy(heroUpEffectSequence.transform.parent.gameObject);
		}
	}

	private void OnGUIDrawingEnabled(GUIDrawingEnabledMessage msg)
	{
		if (msg.DrawingEnabled)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	private void OnEnable()
	{
		if (AppShell.Instance != null && AppShell.Instance.EventMgr != null)
		{
			AppShell.Instance.EventMgr.AddListener<GUIDrawingEnabledMessage>(OnGUIDrawingEnabled);
		}
	}

	private void OnDisable()
	{
		if (AppShell.Instance != null && AppShell.Instance.EventMgr != null)
		{
			AppShell.Instance.EventMgr.RemoveListener<GUIDrawingEnabledMessage>(OnGUIDrawingEnabled);
		}
	}
}
