using UnityEngine;

public class CameraLiteManagerSquadBattle : CameraLiteManager
{
	public CameraLiteSelection[] arenaCameras;

	public CameraLiteSelection[] characterCameras;

	public CameraLiteSelection[] combatCameras;

	public CameraLiteSelection[] introCameras;

	protected int totalArenaWeight;

	protected int totalCharacterWeight;

	protected int totalCombatWeight;

	protected int totalIntroWeight;

	protected CameraLiteSelection lastCombatCamera;

	public bool suspended;

	public override void LateUpdate()
	{
		if (!suspended)
		{
			base.LateUpdate();
		}
	}

	protected override void initializeCameras()
	{
		suspended = false;
		totalArenaWeight = 0;
		CameraLiteSelection[] array = arenaCameras;
		foreach (CameraLiteSelection cameraLiteSelection in array)
		{
			totalArenaWeight += cameraLiteSelection.selectionWeight;
		}
		totalCharacterWeight = 0;
		CameraLiteSelection[] array2 = characterCameras;
		foreach (CameraLiteSelection cameraLiteSelection2 in array2)
		{
			totalCharacterWeight += cameraLiteSelection2.selectionWeight;
		}
		totalCombatWeight = 0;
		CameraLiteSelection[] array3 = combatCameras;
		foreach (CameraLiteSelection cameraLiteSelection3 in array3)
		{
			totalCombatWeight += cameraLiteSelection3.selectionWeight;
		}
		totalIntroWeight = 0;
		CameraLiteSelection[] array4 = introCameras;
		foreach (CameraLiteSelection cameraLiteSelection4 in array4)
		{
			totalIntroWeight += cameraLiteSelection4.selectionWeight;
		}
		base.LateUpdate();
		suspended = true;
	}

	protected CameraLiteSelection chooseRandomCamera(int totalWeight, CameraLiteSelection[] cameraChoices)
	{
		return chooseRandomCamera(totalWeight, cameraChoices, null);
	}

	protected CameraLiteSelection chooseRandomCamera(int totalWeight, CameraLiteSelection[] cameraChoices, CameraLiteSelection previousCamera)
	{
		int num = Random.Range(0, totalWeight + 1);
		int num2 = 0;
		foreach (CameraLiteSelection cameraLiteSelection in cameraChoices)
		{
			if (previousCamera == null || cameraLiteSelection != previousCamera)
			{
				num2 += cameraLiteSelection.selectionWeight;
			}
			if (num2 >= num)
			{
				return cameraLiteSelection;
			}
		}
		return null;
	}

	public CameraLiteSelection ArenaCamera()
	{
		CardGameController.Instance.ShowPlayerBillboard(SquadBattlePlayerEnum.Left, 1f);
		CardGameController.Instance.ShowPlayerBillboard(SquadBattlePlayerEnum.Right, 1f);
		return chooseRandomCamera(totalArenaWeight, arenaCameras);
	}

	public CameraLiteSelection CharacterCamera()
	{
		CardGameController.Instance.HidePlayerBillboard(SquadBattlePlayerEnum.Left, 1f);
		CardGameController.Instance.HidePlayerBillboard(SquadBattlePlayerEnum.Right, 1f);
		return chooseRandomCamera(totalCharacterWeight, characterCameras);
	}

	public CameraLiteSelection CombatCamera()
	{
		CameraLiteSelection cameraLiteSelection;
		for (cameraLiteSelection = null; cameraLiteSelection == null; cameraLiteSelection = chooseRandomCamera(totalCombatWeight, combatCameras, lastCombatCamera))
		{
		}
		lastCombatCamera = cameraLiteSelection;
		CardGameController.Instance.HidePlayerBillboard(SquadBattlePlayerEnum.Left, 1f);
		CardGameController.Instance.HidePlayerBillboard(SquadBattlePlayerEnum.Right, 1f);
		return cameraLiteSelection;
	}

	public CameraLiteSelection IntroCamera()
	{
		if (introCameras.Length == 0)
		{
			return chooseRandomCamera(totalCharacterWeight, characterCameras);
		}
		return chooseRandomCamera(totalIntroWeight, introCameras);
	}
}
