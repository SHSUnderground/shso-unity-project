using System;
using System.Collections.Generic;
using UnityEngine;

public class NetType
{
	public enum Basic
	{
		INT,
		LONG,
		FLOAT,
		STRING,
		VECTOR3,
		QUATERNION,
		GONETID,
		GAMEOBJECT,
		LAST_ENTRY
	}

	protected static Type[] systemTypes = new Type[63]
	{
		typeof(int),
		typeof(long),
		typeof(float),
		typeof(string),
		typeof(Vector3),
		typeof(Quaternion),
		typeof(GoNetId),
		typeof(GameObject),
		typeof(NetworkMessage),
		typeof(TestMessage),
		typeof(AggregateMessage),
		typeof(NetActionMessage),
		typeof(ClientReadyMessage),
		typeof(NewEntityMessage),
		typeof(DeleteEntityMessage),
		typeof(AssignTargetMessage),
		typeof(MatchmakingInvitationMessage),
		typeof(ScenarioEventMessage),
		typeof(ObjectSpawnMessage),
		typeof(SetPetMessage),
		typeof(ForceEmoteMessage),
		typeof(SetTitleMessage),
		typeof(TrapImpactMessage),
		typeof(BrawlerStageCompleteMessage),
		typeof(PickupSpawnMessage),
		typeof(RemoteImpactMessage),
		typeof(PolymorphSpawnMessage),
		typeof(ScenarioEventServerTimeMessage),
		typeof(SpawnPrestigeMessage),
		typeof(NetAction),
		typeof(NetActionPositionUpdate),
		typeof(NetActionPositionFull),
		typeof(NetActionJump),
		typeof(NetActionJumpCancel),
		typeof(NetActionEmote),
		typeof(NetActionHotSpot),
		typeof(NetActionHotSpotController),
		typeof(NetActionInteractiveObjectController),
		typeof(NetActionAttack),
		typeof(NetActionImpact),
		typeof(NetActionDie),
		typeof(NetActionPickupThrowable),
		typeof(NetActionDropThrowable),
		typeof(NetActionPickupPickup),
		typeof(NetActionPositionFullRotation),
		typeof(NetActionExitDoor),
		typeof(NetActionSit),
		typeof(NetActionStand),
		typeof(NetActionCancel),
		typeof(NetActionHitObject),
		typeof(NetActionMultiHotspotSync),
		typeof(NetActionDirectedMenuChat),
		typeof(NetActionProjectile),
		typeof(NetActionCombatEffect),
		typeof(NetActionPlayerStatus),
		typeof(NetActionMultishotUpdate),
		typeof(NetActionPolymorph),
		typeof(NetActionChargeCollision),
		typeof(NetActionLeveledUpStart),
		typeof(NetActionLeveledUpEnd),
		typeof(NetActionChallengeUpStart),
		typeof(NetActionChallengeUpEnd),
		typeof(NetActionVO)
	};

	protected static Dictionary<Type, byte> netTypeLookup = null;

	public static byte GetNetType(Type t)
	{
		return netTypeLookup[t];
	}

	public static object ObjectFactory(int idx)
	{
		return Activator.CreateInstance(systemTypes[idx]);
	}

	public static void Initialize()
	{
		if (netTypeLookup == null)
		{
			netTypeLookup = new Dictionary<Type, byte>();
			for (byte b = 0; b < systemTypes.Length; b = (byte)(b + 1))
			{
				netTypeLookup[systemTypes[b]] = b;
			}
		}
	}
}
