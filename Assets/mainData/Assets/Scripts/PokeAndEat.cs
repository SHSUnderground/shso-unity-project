using UnityEngine;

public class PokeAndEat : PokeAndJiggle
{
	public GameObject[] Menu;

	public Transform FoodSpawnLocation;

	protected OnDone onDone;

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		this.onDone = onDone;
		PlayerBlob playerBlob = new PlayerBlob(this, player, OnDonePoking, null);
		return playerBlob.Start();
	}

	public void OnDonePoking(GameObject player, CompletionStateEnum completionState)
	{
		Vector3 position = Vector3.zero;
		Quaternion rotation = Quaternion.identity;
		if (FoodSpawnLocation != null)
		{
			position = FoodSpawnLocation.transform.position;
			rotation = FoodSpawnLocation.transform.rotation;
		}
		GameObject gameObject = Object.Instantiate(Menu[Random.Range(0, Menu.Length)], position, rotation) as GameObject;
		if (gameObject != null)
		{
			BehaviorManager component = player.GetComponent<BehaviorManager>();
			if (component != null)
			{
				BehaviorEat behaviorEat = component.requestChangeBehavior<BehaviorEat>(false);
				if (behaviorEat != null)
				{
					behaviorEat.Initialize(gameObject, true, OnDoneEating);
				}
			}
		}
		if (onDone != null)
		{
			onDone(player, completionState);
		}
	}

	protected void OnDoneEating(GameObject objEaten)
	{
		Object.Destroy(objEaten);
	}
}
