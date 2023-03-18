using System.Collections.Generic;
using UnityEngine;

public class BehaviorDieBoss : BehaviorDie
{
	protected float closeDistance = 7f;

	protected Vector3 cameraOffset;

	protected float originalDistance;

	protected CameraLiteSpring cameraLite;

	protected CharacterController[] characters;

	protected CutSceneImageFadeEvent fadeEvent;

	protected HashSet<Renderer> disabledRenderers;

	public override void Initialize(GameObject attacker, float newDuration)
	{
		base.Initialize(attacker, newDuration);
		blinkEnabled = false;
		CharacterSpawn.ConnectCameras(owningObject);
		cameraLite = (CameraLiteManager.Instance.GetCurrentCamera() as CameraLiteSpring);
		if (cameraLite != null)
		{
			originalDistance = cameraLite.distanceFromTarget;
		}
		disabledRenderers = new HashSet<Renderer>();
		characters = (Object.FindObjectsOfType(typeof(CharacterController)) as CharacterController[]);
		CharacterController[] array = characters;
		foreach (CharacterController characterController in array)
		{
			if (characterController.gameObject != owningObject)
			{
				LodBase componentInChildren = characterController.GetComponentInChildren<LodBase>();
				if (componentInChildren != null)
				{
					componentInChildren.SetVisible(false);
				}
				else
				{
					SkinnedMeshRenderer componentInChildren2 = characterController.GetComponentInChildren<SkinnedMeshRenderer>();
					if (componentInChildren2 != null)
					{
						componentInChildren2.enabled = false;
					}
				}
				BehaviorManager component = characterController.GetComponent<BehaviorManager>();
				if (component != null && (component.getBehavior() is BehaviorAttackApproach || component.getBehavior() is BehaviorAttackBase))
				{
					component.endBehavior();
				}
			}
			Renderer[] componentsInChildren = characterController.GetComponentsInChildren<Renderer>(true);
			if (componentsInChildren != null)
			{
				Renderer[] array2 = componentsInChildren;
				foreach (Renderer renderer in array2)
				{
					if (!(renderer is SkinnedMeshRenderer))
					{
						if (!renderer.enabled)
						{
							disabledRenderers.Add(renderer);
						}
						else
						{
							renderer.enabled = false;
						}
					}
				}
			}
			Light[] componentsInChildren2 = characterController.GetComponentsInChildren<Light>();
			if (componentsInChildren2 != null)
			{
				Light[] array3 = componentsInChildren2;
				foreach (Light light in array3)
				{
					light.enabled = false;
				}
			}
		}
		fadeEvent = (owningObject.AddComponent(typeof(CutSceneImageFadeEvent)) as CutSceneImageFadeEvent);
		fadeEvent.textureColor = Color.white;
		fadeEvent.useColor = true;
		fadeEvent.fadeStart = 0f;
		fadeEvent.eventTime = 0.5f;
		fadeEvent.fadeIn = false;
		fadeEvent.textureSource = "common_bundle|white";
		fadeEvent.Start();
		fadeEvent.StartEvent();
		if (charGlobals != null)
		{
			BossAIControllerBrawler bossAIControllerBrawler = charGlobals.brawlerCharacterAI as BossAIControllerBrawler;
			if (bossAIControllerBrawler != null)
			{
				closeDistance = bossAIControllerBrawler.deathCamDistance;
				cameraOffset = bossAIControllerBrawler.deathCamOffset;
			}
		}
		CameraLiteManager.Instance.cameraOffset += cameraOffset;
		ShsAudioSource.PlayAutoSound(ShsAudioSourceList.GetList("MissionGlobal").GetSource("boss_ko"));
		GameController controller = GameController.GetController();
		if (controller != null && controller.LocalPlayer != null)
		{
			PlayerInputController component2 = controller.LocalPlayer.GetComponent<PlayerInputController>();
			if (component2 != null)
			{
				component2.AllowInput = false;
			}
			PlayerCombatController component3 = controller.LocalPlayer.GetComponent<PlayerCombatController>();
			if (component3 != null)
			{
				component3.SuppressHeroUpEffect(true);
			}
		}
		SHSBrawlerMainWindow sHSBrawlerMainWindow = (SHSBrawlerMainWindow)GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow"];
		if (sHSBrawlerMainWindow != null)
		{
			sHSBrawlerMainWindow.EnablePowerButton(false);
			sHSBrawlerMainWindow.SuppressPowerStateChange(true);
		}
		CameraShakeManager.CreateSingleton();
		CameraShakeManager.Instance.EnableShakes(false);
	}

	public override void behaviorUpdate()
	{
		if (cameraLite != null)
		{
			float distanceFromTarget = Mathf.Lerp(originalDistance, closeDistance, elapsedTime + 0.5f);
			cameraLite.distanceFromTarget = distanceFromTarget;
		}
		if (elapsedTime >= 0.2f && elapsedTime < 0.5f)
		{
			Time.timeScale = Mathf.Lerp(1f, 0.2f, (elapsedTime - 0.5f) / 0.3f);
		}
		else if (elapsedTime >= 0.5f)
		{
			Time.timeScale = Mathf.Lerp(0.2f, 1f, (elapsedTime - 0.8f) / 1.5f);
		}
		base.behaviorUpdate();
	}

	public override void behaviorEnd()
	{
		Time.timeScale = 1f;
		CharacterController[] array = characters;
		foreach (CharacterController characterController in array)
		{
			if (characterController == null)
			{
				continue;
			}
			if (characterController.gameObject != owningObject)
			{
				LodBase componentInChildren = characterController.GetComponentInChildren<LodBase>();
				if (componentInChildren != null)
				{
					componentInChildren.SetVisible(true);
				}
				else
				{
					SkinnedMeshRenderer componentInChildren2 = characterController.GetComponentInChildren<SkinnedMeshRenderer>();
					if (componentInChildren2 != null)
					{
						componentInChildren2.enabled = true;
					}
				}
			}
			Renderer[] componentsInChildren = characterController.GetComponentsInChildren<Renderer>(true);
			if (componentsInChildren != null)
			{
				Renderer[] array2 = componentsInChildren;
				foreach (Renderer renderer in array2)
				{
					if (!(renderer is SkinnedMeshRenderer) && !disabledRenderers.Contains(renderer))
					{
						renderer.enabled = true;
					}
				}
			}
			Light[] componentsInChildren2 = characterController.GetComponentsInChildren<Light>();
			if (componentsInChildren2 != null)
			{
				Light[] array3 = componentsInChildren2;
				foreach (Light light in array3)
				{
					light.enabled = true;
				}
			}
		}
		disabledRenderers.Clear();
		disabledRenderers = null;
		if (cameraLite != null)
		{
			cameraLite.distanceFromTarget = originalDistance;
		}
		CameraLiteManager.Instance.cameraOffset -= cameraOffset;
		if (GameController.GetController().LocalPlayer != null)
		{
			CharacterSpawn.ConnectCameras(GameController.GetController().LocalPlayer);
			PlayerInputController component = GameController.GetController().LocalPlayer.GetComponent<PlayerInputController>();
			if (component != null)
			{
				component.AllowInput = true;
			}
			PlayerCombatController component2 = GameController.GetController().LocalPlayer.GetComponent<PlayerCombatController>();
			if (component2 != null)
			{
				component2.SuppressHeroUpEffect(false);
			}
		}
		SHSBrawlerMainWindow sHSBrawlerMainWindow = (SHSBrawlerMainWindow)GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow"];
		if (sHSBrawlerMainWindow != null)
		{
			sHSBrawlerMainWindow.EnablePowerButton(true);
			sHSBrawlerMainWindow.SuppressPowerStateChange(false);
		}
		CameraShakeManager.Instance.EnableShakes(true);
		base.behaviorEnd();
	}
}
