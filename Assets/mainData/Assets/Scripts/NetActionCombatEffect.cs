using UnityEngine;

public class NetActionCombatEffect : NetAction
{
	public string combatEffectName;

	public bool remove;

	public GameObject source;

	public bool usePrefabSource;

	public NetActionCombatEffect()
	{
	}

	public NetActionCombatEffect(string newCombatEffectName, bool newRemove, GameObject newSource, bool newUsePrefabSource)
	{
		combatEffectName = newCombatEffectName;
		remove = newRemove;
		source = newSource;
		usePrefabSource = newUsePrefabSource;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(combatEffectName);
		writer.Write(remove);
		writer.Write(source);
		writer.Write(usePrefabSource);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		combatEffectName = reader.ReadString();
		remove = reader.ReadBoolean();
		source = reader.ReadGameObject();
		usePrefabSource = reader.ReadBoolean();
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionCombatEffect;
	}

	public override string ToString()
	{
		return "NetActionCombatEffect: " + timestamp;
	}
}
