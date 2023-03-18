using UnityEngine;

public class PlayerInputTrigger : TriggerBase
{
	public PlayerInputController.inputEventType eventToCheck;

	public float requiredTime;

	private PlayerInputController inputToCheck;

	protected float inputPressedTime;

	private void Start()
	{
		PlayerInputController[] array = Utils.FindObjectsOfType<PlayerInputController>();
		if (array.Length > 0)
		{
			inputToCheck = array[0];
		}
		if (inputToCheck == null)
		{
			AppShell.Instance.EventMgr.AddListener<EntitySpawnMessage>(OnEntitySpawnedEvent);
		}
	}

	private void OnDisable()
	{
		if (inputToCheck == null)
		{
			AppShell.Instance.EventMgr.RemoveListener<EntitySpawnMessage>(OnEntitySpawnedEvent);
		}
	}

	private void Update()
	{
		if (inputToCheck != null)
		{
			if (inputToCheck.CurrentInputActions[(int)eventToCheck])
			{
				inputPressedTime += Time.deltaTime;
			}
			else
			{
				inputPressedTime = 0f;
			}
			if (inputPressedTime > requiredTime)
			{
				OnTriggered(inputToCheck.gameObject);
			}
		}
	}

	protected void OnEntitySpawnedEvent(EntitySpawnMessage e)
	{
		if ((e.spawnType & CharacterSpawn.Type.Local) != 0 && (e.spawnType & CharacterSpawn.Type.Player) != 0)
		{
			inputToCheck = Utils.GetComponent<PlayerInputController>(e.go);
			AppShell.Instance.EventMgr.RemoveListener<EntitySpawnMessage>(OnEntitySpawnedEvent);
		}
	}
}
