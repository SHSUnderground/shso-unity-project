using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorSimulate : BehaviorBase
{
	private const int DEFAULT_ROOM_AFFINITY = 3;

	private const int DEFAULT_ROOM_TIMEOUT = 3;

	private AIControllerHQ aiController;

	private float nextActivityTime;

	private OnBehaviorDone doneCallback;

	public HqItem.Claim currentClaim;

	public void Initialize(AIControllerHQ controller, OnBehaviorDone callback)
	{
		aiController = controller;
		int value = 3;
		aiController.AIData.room_affinities.TryGetValue(aiController.CurrentRoom.Id, out value);
		nextActivityTime = value * 3;
		doneCallback = callback;
		charGlobals.motionController.teleportTo(owningObject.transform.position + Vector3.up * 100f);
		aiController.FlingaObject.transform.position = aiController.transform.position + Vector3.up * 100f;
		currentClaim = aiController.CurrentClaim;
		if (currentClaim != null)
		{
			aiController.AddToUsedItems(currentClaim.item);
		}
	}

	private HqRoom2 GetBestRoom(List<HqRoom2> roomList)
	{
		HqRoom2 result = null;
		float[] array = new float[roomList.Count];
		int num = 0;
		int num2 = 0;
		foreach (HqRoom2 room in roomList)
		{
			int value = 3;
			aiController.AIData.room_affinities.TryGetValue(room.Id, out value);
			num += value;
			array[num2++] = num;
		}
		int num3 = UnityEngine.Random.Range(1, num);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] >= (float)num3)
			{
				result = roomList[i];
				break;
			}
		}
		return result;
	}

	public override void behaviorUpdate()
	{
		if (Paused)
		{
			return;
		}
		base.behaviorUpdate();
		if (!(elapsedTime >= nextActivityTime))
		{
			return;
		}
		elapsedTime = 0f;
		if (aiController == null)
		{
			CspUtils.DebugLog("AIController is null Character: " + owningObject.name);
			return;
		}
		if (currentClaim != null)
		{
			aiController.AddToUsedItems(currentClaim.item);
			currentClaim.ReleaseClaim(aiController);
			currentClaim = null;
		}
		aiController.ReleaseCurrentItem();
		if (aiController.InJail)
		{
			return;
		}
		List<HqRoom2> list = new List<HqRoom2>();
		if (aiController.CurrentRoom.RoomState == HqRoom2.AccessState.Locked)
		{
			list.Add(aiController.CurrentRoom);
		}
		else
		{
			if (HqController2.Instance.ActiveRoom.RoomState == HqRoom2.AccessState.Unlocked)
			{
				list.Add(HqController2.Instance.ActiveRoom);
			}
			foreach (KeyValuePair<string, HqRoom2> room in HqController2.Instance.Rooms)
			{
				if (!list.Contains(room.Value) && room.Value.RoomState == HqRoom2.AccessState.Unlocked)
				{
					list.Add(room.Value);
				}
			}
		}
		HqRoom2 hqRoom = null;
		HqItem hqItem = null;
		if (aiController.IsAngry && aiController.CurrentRoom.RoomState != HqRoom2.AccessState.Locked)
		{
			hqItem = aiController.FindItemToEat(HqController2.Instance.ActiveRoom);
		}
		else
		{
			aiController.FindItemToUse(list);
		}
		if (hqItem != null)
		{
			aiController.Log("Chose item " + hqItem.gameObject.name + " in room " + hqItem.Room.Id, 1);
			hqRoom = hqItem.Room;
		}
		else
		{
			hqRoom = GetBestRoom(list);
		}
		if (hqRoom != null)
		{
			if (hqRoom != aiController.CurrentRoom)
			{
				aiController.GoToRoom(hqRoom);
				if (hqRoom == HqController2.Instance.ActiveRoom)
				{
					Vector3 randomDoor = hqRoom.RandomDoor;
					hqRoom.PlayTransporterEffect(randomDoor);
					charGlobals.motionController.teleportTo(randomDoor);
					if (hqItem != null)
					{
						doneCallback(hqItem.gameObject);
					}
					else
					{
						doneCallback(null);
					}
					return;
				}
			}
		}
		else
		{
			hqRoom = aiController.CurrentRoom;
		}
		if (hqRoom.RoomState != HqRoom2.AccessState.Unlocked)
		{
			nextActivityTime = 9f;
			aiController.Log("BehaviorSimulate tried to choose room that is not unlocked: " + hqRoom.Id, 1);
			return;
		}
		if (hqItem != null)
		{
			aiController.Log("Claiming " + hqItem.gameObject.name + " in BehaviorSimulate.", 4);
			currentClaim = hqItem.NextAvailableClaim;
			if (currentClaim != null)
			{
				currentClaim.Claimer = aiController;
			}
			aiController.AddToUsedItems(hqItem);
		}
		int value = 3;
		aiController.AIData.room_affinities.TryGetValue(hqRoom.Id, out value);
		nextActivityTime = value * 3;
		aiController.Log("BehaviorSimulate has choosen room " + hqRoom.Id + " for " + nextActivityTime + " seconds", 1);
	}

	public override void behaviorEnd()
	{
		base.behaviorEnd();
	}

	public override void behaviorCancel()
	{
		base.behaviorBegin();
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override bool useMotionControllerGravity()
	{
		return false;
	}

	public override bool allowForcedInterrupt(Type newBehaviorType)
	{
		return true;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return true;
	}
}
