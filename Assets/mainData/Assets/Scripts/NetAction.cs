public abstract class NetAction
{
	public enum NetActionType
	{
		NetAction,
		NetActionPositionUpdate,
		NetActionPositionFull,
		NetActionJump,
		NetActionJumpCancel,
		NetActionEmote,
		NetActionHotSpot,
		NetActionHotSpotController,
		NetActionInteractiveObjectController,
		NetActionAttack,
		NetActionImpact,
		NetActionDie,
		NetActionPickupThrowable,
		NetActionDropThrowable,
		NetActionPickupPickup,
		NetActionPositionFullRotation,
		NetActionExitDoor,
		NetActionSit,
		NetActionStand,
		NetActionCancel,
		NetActionHitObject,
		NetActionMultiHotspotSync,
		NetActionDirectedMenuChat,
		NetActionProjectile,
		NetActionCombatEffect,
		NetActionPlayerStatus,
		NetActionMultishotUpdate,
		NetActionPolymorph,
		NetActionChargeCollision,
		NetActionLeveledUpStart,
		NetActionLeveledUpEnd,
		NetActionChallengeUpStart,
		NetActionChallengeUpEnd,
		NetActionVO
	}

	public float timestamp;

	public NetAction()
	{
	}

	public virtual void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		writer.Write(timestamp);
	}

	public virtual void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		timestamp = reader.ReadFloat();
	}

	public abstract NetActionType getType();

	public override string ToString()
	{
		return "NetAction: " + timestamp;
	}
}
