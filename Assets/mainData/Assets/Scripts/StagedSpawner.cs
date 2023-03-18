using System;
using System.Collections;
using UnityEngine;
//using UnityEditor;

public class StagedSpawner : IDisposable
{
	public delegate void SpawnFinished(CharacterGlobals player);

	protected delegate void PODMoved(CharacterGlobals player);

	protected bool disposed;

	protected SpawnPoint spawnPoint;

	protected string placeholderCharacterName;

	protected string characterName;

	protected bool spawning;

	protected GameObject placeholder;

	protected GameObject cameraStandIn;

	protected SpawnFinished onFinished;

	protected Vector3 swapPosition;

	protected Quaternion swapRotation;

	public static float debug_swapDelay;

	public StagedSpawner(SpawnPoint spawnPoint, string placeholderCharacterName, string characterName, GameObject initialCameraStandIn)
	{
		this.spawnPoint = spawnPoint;
		this.placeholderCharacterName = placeholderCharacterName;
		this.characterName = characterName;
		cameraStandIn = initialCameraStandIn;
		placeholder = null;
		disposed = false;
	}

	~StagedSpawner()
	{
		Dispose(false);
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (disposed)
		{
			return;
		}
		if (disposing)
		{
			disposed = true;
			if (placeholder != null)
			{
				CspUtils.DebugLog("Destorying... " + placeholder.name);
				UnityEngine.Object.Destroy(placeholder);
			}
		}
		disposed = true;
		spawnPoint = null;
		cameraStandIn = null;
		placeholder = null;
	}

	public void Spawn(SpawnFinished onFinished)
	{
		if (!disposed && !spawning)
		{
			spawning = true;
			this.onFinished = onFinished;
			spawnPoint.SpawnPlayer(placeholderCharacterName, OnPlaceholderSpawned);
		}
	}

	private void OnPlaceholderSpawned(GameObject placeholder)
	{
		if (disposed)
		{
			CspUtils.DebugLog("Destorying... " + placeholder.name);
			UnityEngine.Object.Destroy(placeholder);
			return;
		}
		this.placeholder = placeholder;
		Utils.GetComponent<CharacterMotionController>(placeholder).DisableNetUpdates(true);
		if (cameraStandIn != null)
		{
			Utils.GetComponent<BehaviorManager>(placeholder).StartCoroutine(MovePOD(placeholder, StartRealSpawn));
		}
		else
		{
			StartRealSpawn(null);
		}
	}

	private void StartRealSpawn(CharacterGlobals unusedParameter)
	{
		if (!disposed)
		{
			spawnPoint.SpawnPlayer(characterName, OnRealSpawned, OnTransactionInitialized, placeholder);
		}
	}

	private void OnTransactionInitialized(CharacterSpawn spawner, TransactionMonitor spawnTransaction)
	{
		if (!disposed)
		{
			spawnTransaction.AddStepDelegate("assetLoad", OnRealAssetsLoaded);
			spawnTransaction.AddStep("placeholderIdle");
		}
	}

	private void OnRealAssetsLoaded(string step, bool success, TransactionMonitor transaction)
	{
		if (!disposed)
		{
			GameObject gameObject = CreateStandIn();
			if (gameObject != null)
			{
				CoroutineContainer coroutineContainer = Utils.AddComponent<CoroutineContainer>(gameObject);
				coroutineContainer.StartCoroutine(WaitToSwap(transaction));
			}
		}
	}

	protected GameObject CreateStandIn()
	{
		if (disposed)
		{
			return null;
		}
		if (placeholder != null)
		{
			cameraStandIn = new GameObject("__StandIn__");
			cameraStandIn.transform.position = placeholder.transform.position;
			cameraStandIn.transform.rotation = placeholder.transform.rotation;
			PlayerOcclusionDetector.Instance.myPlayer = cameraStandIn;
			CameraTargetHelper componentInChildren = placeholder.GetComponentInChildren<CameraTargetHelper>();
			//////////////////////////////////////
			CspUtils.DebugLog(placeholder.name + " CreateStandIn componentInChildren=" + componentInChildren);

			int children = placeholder.transform.childCount;
         	for (int j = 0; j < children; ++j) {
				Transform cTrans = placeholder.transform.GetChild(j);
             	CspUtils.DebugLog("For loop: " + cTrans);

				Component [] components = cTrans.gameObject.GetComponents(typeof(Component));  // CSP - loop over all components because single get not working.
				for (int i = 0; i < components.Length; ++i) {
					CspUtils.DebugLog("component name = " + components[i].name);
					CspUtils.DebugLog("component type = " + components[i].GetType());	
					if (components[i].GetType().ToString() == "CameraTargetHelper") {
						CspUtils.DebugLog("component found!");
						componentInChildren = (CameraTargetHelper)(components[i]);
						break;
					}
				} 

			}

			
			/////////////////////////////////////
			if (componentInChildren != null)
			{
				componentInChildren.SetTarget(cameraStandIn.transform);
				//EditorApplication.isPaused = true;
			}
			return cameraStandIn;
		}
		CspUtils.DebugLog("Placeholder is gone");
		return null;
	}

	protected IEnumerator WaitToSwap(TransactionMonitor transaction)
	{
		CspUtils.DebugLog("WaitToSwap debug_swapDelay=" + debug_swapDelay);
		if (debug_swapDelay > 0f)
		{
			yield return new WaitForSeconds(debug_swapDelay);
		}
		CspUtils.DebugLog("WaitToSwap disposed=" + disposed);
		if (disposed)
		{
			yield break;
		}
		CspUtils.DebugLog("WaitToSwap disposed=" + disposed);
		CspUtils.DebugLog("WaitToSwap placeholder=" + placeholder);
		while (!disposed && placeholder != null)
		{
			BehaviorManager behaviorMan = Utils.GetComponent<BehaviorManager>(placeholder);
			CspUtils.DebugLog("WaitToSwap behaviorMan=" + behaviorMan);
			if (!(behaviorMan.getBehavior() is BehaviorMovement) || !behaviorMan.currentBehaviorInterruptible(typeof(BehaviorMovement)))
			{
				CspUtils.DebugLog("WaitToSwap Before yield");
				yield return 0;
				CspUtils.DebugLog("WaitToSwap After yield");
				continue;
			}
			break;
		}
		CspUtils.DebugLog("WaitToSwap disposed=" + disposed);
		CspUtils.DebugLog("WaitToSwap placeholder=" + placeholder);
		if (!disposed && !(placeholder == null))
		{
			swapPosition = placeholder.transform.position;
			swapRotation = placeholder.transform.rotation;
			Utils.GetComponent<SpawnData>(placeholder).Despawn(EntityDespawnMessage.despawnType.defeated);
			CspUtils.DebugLog("WaitToSwap Before yield");
			yield return 0;
			CspUtils.DebugLog("WaitToSwap After yield");
			transaction.CompleteStep("placeholderIdle");
		}
	}

	private void OnRealSpawned(GameObject realCharacter)
	{
		if (disposed)
		{
			CspUtils.DebugLog("Destorying... " + realCharacter.name);
			UnityEngine.Object.Destroy(realCharacter);
			return;
		}
		realCharacter.transform.position = swapPosition;
		realCharacter.transform.rotation = swapRotation;
		Utils.GetComponent<BehaviorManager>(realCharacter).StartCoroutine(MovePOD(realCharacter, Finish));
	}

	protected IEnumerator MovePOD(GameObject destination, PODMoved onFinished)
	{
		yield return new WaitForFixedUpdate();
		yield return 0;
		CspUtils.DebugLog(destination.name + " MovePOD disposed=" + disposed);
		if (!disposed)
		{
			CameraTargetHelper cameraHook = cameraStandIn.GetComponentInChildren<CameraTargetHelper>();
			
			//////////////////////////////////////
			//cameraStandIn.AddComponent ("CameraTargetHelper"); 
			CspUtils.DebugLog(cameraStandIn.name + " MovePOD cameraHook=" + cameraHook);

			int children = cameraStandIn.transform.childCount;
         	for (int j = 0; j < children; ++j) {
				Transform cTrans = cameraStandIn.transform.GetChild(j);
             	CspUtils.DebugLog("For loop: " + cTrans);

				Component [] components = cTrans.gameObject.GetComponents(typeof(Component));  // CSP - loop over all components because single get not working.
				for (int i = 0; i < components.Length; ++i) {
					CspUtils.DebugLog("component name = " + components[i].name);
					CspUtils.DebugLog("component type = " + components[i].GetType());	
					if (components[i].GetType().ToString() == "CameraTargetHelper") {
						CspUtils.DebugLog("component found!");
						cameraHook = (CameraTargetHelper)(components[i]);
						break;
					}
				} 

			}

			//EditorApplication.isPaused = true;
			/////////////////////////////////////


			if (cameraHook != null)
			{
				cameraHook.SetTarget(destination.transform);
			}
			CspUtils.DebugLog("MovePOD - Destorying... " + cameraStandIn.name);
			UnityEngine.Object.Destroy(cameraStandIn);
			cameraStandIn = null;
			if (onFinished != null)
			{
				onFinished(Utils.GetComponent<CharacterGlobals>(destination));
			}
		}
	}

	protected void Finish(CharacterGlobals player)
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(CharacterController));
		for (int i = 0; i < array.Length; i++)
		{
			CharacterController characterController = (CharacterController)array[i];
			if (characterController != player.collider)
			{
				Physics.IgnoreCollision(characterController, player.collider);
			}
		}
		spawning = false;
		if (onFinished != null)
		{
			onFinished(player);
		}
	}
}
