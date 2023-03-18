using System.Collections.Generic;
using UnityEngine;

public class CharacterCombinationBridge : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private const float ACTIVE_WINDOW_LIFETIME = 5f;

	private const float ACTIVE_WINDOW_PERIOD = 5f;

	private const float OBJECTIVE_TIMEOUT = 30f;

	private List<BasePresetCombination> _ccList;

	private HashSet<string> _ccDisplayed;

	private string _ccActive;

	private float _ccActiveTime;

	private SHSBrawlerCharacterCombinationWindow _ccWindow;

	private float _ccWindowPeriodStart;

	private bool _objectiveShown;

	private float _objectiveTimeoutStart;

	public CharacterCombinationBridge()
	{
		_ccList = new List<BasePresetCombination>();
		_ccDisplayed = new HashSet<string>();
		_ccActive = null;
		_ccActiveTime = -1f;
		_ccWindow = null;
		_ccWindowPeriodStart = 0f;
		_objectiveShown = false;
		_objectiveTimeoutStart = -1f;
	}

	public void DestroyBridge()
	{
		ReleaseCharacterCombinationWindow();
		Object.Destroy(this);
	}

	public void ClearCombinations()
	{
		if (_ccList != null)
		{
			_ccList.Clear();
		}
	}

	public void ClearDisplayedCombinations()
	{
		if (_ccDisplayed != null)
		{
			_ccDisplayed.Clear();
		}
	}

	public void PushCombination(BasePresetCombination combination)
	{
		if (combination != null && _ccList != null)
		{
			if (string.IsNullOrEmpty(combination.Id))
			{
				CspUtils.DebugLog("CharacterCombinationBridge::PushCombination() - combination id is null or empty for combination with display name <" + combination.DisplayName + ">");
			}
			else if (_ccList.Contains(combination))
			{
				CspUtils.DebugLog("CharacterCombinationBridge::PushCombination() - combination id is already in map and cannot add it");
			}
			else
			{
				_ccList.Add(combination);
			}
		}
	}

	public void ActivateCombination(BasePresetCombination combination)
	{
		if (combination == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(combination.Id))
		{
			CspUtils.DebugLog("CharacterCombinationBridge::ActivateCombination() - combination id is null or empty for combination with display name <" + combination.DisplayName + ">");
			return;
		}
		if (_ccList != null && !_ccList.Contains(combination))
		{
			_ccList.Add(combination);
		}
		if (!IsActiveCombination(combination.Id))
		{
			_ccActive = combination.Id;
			_ccActiveTime = Time.time;
		}
	}

	public void DeactivateCombination(BasePresetCombination combination)
	{
		if (combination == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(combination.Id))
		{
			CspUtils.DebugLog("CharacterCombinationBridge::DeactivateCombination() - combination id is null or empty for combination with display name <" + combination.DisplayName + ">");
			return;
		}
		if (_ccList != null)
		{
			_ccList.Remove(combination);
		}
		if (IsActiveCombination(combination.Id))
		{
			_ccActive = null;
			_ccActiveTime = 0f;
		}
		if (_ccDisplayed != null)
		{
			_ccDisplayed.Add(combination.Id);
		}
	}

	public void UpdateCombination(BasePresetCombination combination)
	{
		if (combination != null)
		{
			if (!IsActiveCombination(combination))
			{
				CspUtils.DebugLog("CharacterCombinationBridge::UpdateCombination() - combination is not active");
			}
			else if (_ccWindow != null)
			{
				_ccWindow.SetCharacters(combination.ActiveCharacters, false);
			}
		}
	}

	public void ShowActiveCombination()
	{
		BasePresetCombination activeCombination = GetActiveCombination();
		if (activeCombination == null)
		{
			CspUtils.DebugLog("CharacterCombinationBridge::ShowActiveCombination() - active combination not found");
			return;
		}
		SHSBrawlerCharacterCombinationWindow characterCombinationWindow = GetCharacterCombinationWindow();
		if (characterCombinationWindow != null)
		{
			characterCombinationWindow.IsVisible = true;
			characterCombinationWindow.SetCombination(activeCombination.ActiveCharacters, activeCombination.DisplayName, activeCombination.DisplayDescription);
			_ccWindowPeriodStart = Time.time;
		}
	}

	public void HideActiveCombination()
	{
		SHSBrawlerCharacterCombinationWindow characterCombinationWindow = GetCharacterCombinationWindow();
		if (characterCombinationWindow != null)
		{
			characterCombinationWindow.IsVisible = false;
		}
	}

	public SHSBrawlerCharacterCombinationWindow GetCharacterCombinationWindow()
	{
		if (_ccWindow != null)
		{
			return _ccWindow;
		}
		SHSBrawlerMainWindow sHSBrawlerMainWindow = GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow"] as SHSBrawlerMainWindow;
		if (sHSBrawlerMainWindow == null)
		{
			return null;
		}
		_ccWindow = new SHSBrawlerCharacterCombinationWindow();
		sHSBrawlerMainWindow.Add(_ccWindow);
		return _ccWindow;
	}

	public void ReleaseCharacterCombinationWindow()
	{
		if (_ccWindow != null)
		{
			SHSBrawlerMainWindow sHSBrawlerMainWindow = GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow"] as SHSBrawlerMainWindow;
			if (sHSBrawlerMainWindow != null)
			{
				sHSBrawlerMainWindow.Remove(_ccWindow);
			}
			_ccWindow = null;
		}
	}

	public BasePresetCombination GetActiveCombination()
	{
		if (string.IsNullOrEmpty(_ccActive))
		{
			return null;
		}
		if (_ccList == null)
		{
			return null;
		}
		foreach (BasePresetCombination cc in _ccList)
		{
			if (cc != null && cc.Id == _ccActive)
			{
				return cc;
			}
		}
		return null;
	}

	public BasePresetCombination GetCombination()
	{
		if (_ccList == null || _ccList.Count < 1)
		{
			return null;
		}
		return _ccList[Random.Range(0, _ccList.Count - 1)];
	}

	public bool IsActiveCombination(BasePresetCombination combination)
	{
		return combination != null && IsActiveCombination(combination.Id);
	}

	public bool IsActiveCombination(string comboId)
	{
		return !string.IsNullOrEmpty(comboId) && !string.IsNullOrEmpty(_ccActive) && comboId == _ccActive;
	}

	public bool HasActiveCombination()
	{
		return _ccActive != null;
	}

	public bool HasInactiveCombinations()
	{
		if (_ccList == null || _ccList.Count < 1)
		{
			return false;
		}
		return _ccList.Count > 1 || GetActiveCombination() == null;
	}

	public bool HasShownCombination(BasePresetCombination combination)
	{
		return combination != null && HasShownCombination(combination.Id);
	}

	public bool HasShownCombination(string comboId)
	{
		return !string.IsNullOrEmpty(comboId) && _ccDisplayed != null && _ccDisplayed.Contains(comboId);
	}

	public bool CanShowCombination()
	{
		if (!_objectiveShown && _objectiveTimeoutStart > 0f && Time.time - _objectiveTimeoutStart < 30f)
		{
			return false;
		}
		ActiveMission activeMission = GetActiveMission();
		if (activeMission != null && activeMission.CurrentStage > 1)
		{
			return false;
		}
		return Time.time - _ccWindowPeriodStart >= 5f;
	}

	public void OnEnable()
	{
		AppShell.Instance.EventMgr.AddListener<PresetCombinationsApplyMessage>(OnPresetCombinationsApply);
		AppShell.Instance.EventMgr.AddListener<BrawlerMissionBriefCompleteMessage>(OnMissionBriefComplete);
	}

	public void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<PresetCombinationsApplyMessage>(OnPresetCombinationsApply);
		AppShell.Instance.EventMgr.RemoveListener<BrawlerMissionBriefCompleteMessage>(OnMissionBriefComplete);
	}

	public void Update()
	{
		ActiveMission activeMission = GetActiveMission();
		if (activeMission != null && activeMission.CurrentStage > 1)
		{
			DestroyBridge();
			return;
		}
		if (HasActiveCombination() && Time.time - _ccActiveTime >= 5f)
		{
			if (!HasInactiveCombinations())
			{
				HideActiveCombination();
			}
			DeactivateCombination(GetActiveCombination());
		}
		if (!HasActiveCombination() && HasInactiveCombinations() && CanShowCombination())
		{
			BasePresetCombination combination = GetCombination();
			ActivateCombination(combination);
			ShowActiveCombination();
		}
	}

	private ActiveMission GetActiveMission()
	{
		return AppShell.Instance.SharedHashTable["ActiveMission"] as ActiveMission;
	}

	private void OnPresetCombinationsApply(PresetCombinationsApplyMessage msg)
	{
		if (msg.Combinations != null)
		{
			if (_objectiveTimeoutStart < 0f)
			{
				_objectiveTimeoutStart = Time.time;
			}
			ClearCombinations();
			foreach (BasePresetCombination combination in msg.Combinations)
			{
				if (!HasShownCombination(combination) && combination.IsApplied)
				{
					PushCombination(combination);
				}
			}
		}
	}

	private void OnMissionBriefComplete(BrawlerMissionBriefCompleteMessage msg)
	{
		_objectiveShown = true;
		_objectiveTimeoutStart = 0f;
		ResetWindowPeriod();
	}

	private void ResetWindowPeriod()
	{
		_ccWindowPeriodStart = Time.time - 5f;
		_ccWindowPeriodStart = Time.time - 5f + 0.5f;
		if (_ccWindowPeriodStart < 0f)
		{
			_ccWindowPeriodStart = 0f;
		}
	}
}
