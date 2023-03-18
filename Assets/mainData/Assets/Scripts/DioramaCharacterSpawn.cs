using System.Collections;
using UnityEngine;

public class DioramaCharacterSpawn : CharacterSpawn
{
	private const float VO_DELAY = 5f;

	private bool wasMultiplayer;

	public void SetSnapOffset(float offset)
	{
		spawnRayPointOffset = offset;
	}

	public void SetSpawnedCharacterCount(int spawnedCharacterCount)
	{
		if (spawnedCharacterCount > 1)
		{
			wasMultiplayer = true;
		}
	}

	protected override void FinalSpawnSetup(GameObject newCharacter, CharacterSpawnData spawnData)
	{
		base.FinalSpawnSetup(newCharacter, spawnData);
		CharacterGlobals characterGlobals = newCharacter.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
		characterGlobals.behaviorManager.defaultBehaviorType = "BehaviorVictory";
		characterGlobals.motionController.gravity = 0f;
		newCharacter.transform.position = GetSpawnGroundPosition(newCharacter.GetComponent<CharacterController>());
		ActiveMission activeMission = AppShell.Instance.SharedHashTable["ActiveMission"] as ActiveMission;
		if (activeMission != null && activeMission.CurrentStage == activeMission.LastStage)
		{
			string action = (!wasMultiplayer) ? "complete_solo" : "complete_multi";
			CoroutineContainer.GetInstance(newCharacter).StartCoroutine(PlayDelayedVO(VOManager.Instance.GetVO(action, newCharacter)));
		}
	}

	private IEnumerator PlayDelayedVO(ResolvedVOAction vo)
	{
		yield return new WaitForSeconds(5f);
		VOManager.Instance.PlayResolvedVO(vo);
	}
}
