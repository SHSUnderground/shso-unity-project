using System.Collections.Generic;

public class SquadBattleAttackPattern
{
	public class AttackSequenceEntry
	{
		public enum AttackSequenceType
		{
			Left,
			Right,
			Super,
			Emote,
			PowerEmote
		}

		public AttackSequenceType attackType;

		public int attackIndex;

		public bool chain;

		public bool isSecondaryAttacker;

		public int damageRequirement;

		public bool isHarmless()
		{
			return attackType == AttackSequenceType.Emote || attackType == AttackSequenceType.PowerEmote;
		}
	}

	public List<AttackSequenceEntry> attackSequence;

	protected string attackSequenceString = "L1";

	public bool RepeatSequence;

	public bool DelaySecondCharacterSpawn;

	public string ZeroDamageEmote = "emote_cheer";

	public string AttackSequenceString
	{
		get
		{
			return attackSequenceString;
		}
		set
		{
			attackSequenceString = value;
			createAttackSequence();
		}
	}

	public SquadBattleAttackPattern()
	{
	}

	public SquadBattleAttackPattern(SquadBattleAttackPattern copy)
	{
		AttackSequenceString = copy.AttackSequenceString;
		RepeatSequence = copy.RepeatSequence;
		ZeroDamageEmote = copy.ZeroDamageEmote;
		DelaySecondCharacterSpawn = copy.DelaySecondCharacterSpawn;
	}

	protected void createAttackSequence()
	{
		//Discarded unreachable code: IL_020e
		attackSequence = new List<AttackSequenceEntry>();
		string[] array = attackSequenceString.Split(';');
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			string[] array3 = text.Split(',');
			int num = array3.Length;
			string[] array4 = array3;
			foreach (string text2 in array4)
			{
				if (string.IsNullOrEmpty(text2))
				{
					continue;
				}
				AttackSequenceEntry attackSequenceEntry = new AttackSequenceEntry();
				attackSequenceEntry.isSecondaryAttacker = false;
				string text3 = text2;
				string[] array5 = text2.Split(':');
				if (array5.Length > 1)
				{
					if (int.TryParse(array5[0], out attackSequenceEntry.damageRequirement))
					{
						text3 = array5[1];
					}
					else
					{
						CspUtils.DebugLog("Failed to process damage requirement for attack sequence: " + attackSequenceString);
					}
				}
				int num2 = 0;
				if (text3[0] == 'A')
				{
					num2 = 1;
				}
				else if (text3[0] == 'B')
				{
					attackSequenceEntry.isSecondaryAttacker = true;
					num2 = 1;
				}
				switch (text3[num2])
				{
				case 'L':
					attackSequenceEntry.attackType = AttackSequenceEntry.AttackSequenceType.Left;
					break;
				case 'R':
					attackSequenceEntry.attackType = AttackSequenceEntry.AttackSequenceType.Right;
					break;
				case 'S':
					attackSequenceEntry.attackType = AttackSequenceEntry.AttackSequenceType.Super;
					break;
				case 'E':
					attackSequenceEntry.attackType = AttackSequenceEntry.AttackSequenceType.Emote;
					break;
				case 'P':
					attackSequenceEntry.attackType = AttackSequenceEntry.AttackSequenceType.PowerEmote;
					break;
				default:
					CspUtils.DebugLog("Unknown attack type in attack choreography: " + text3);
					continue;
				}
				if (attackSequenceEntry.attackType == AttackSequenceEntry.AttackSequenceType.Emote)
				{
					try
					{
						string command = text3.Substring(num2 + 1);
						attackSequenceEntry.attackIndex = EmotesDefinition.Instance.GetEmoteByCommand(command).id;
					}
					catch
					{
						CspUtils.DebugLog("Unknown emote in attack choreography: " + text3);
						continue;
					}
				}
				else if (!int.TryParse(text3.Substring(num2 + 1), out attackSequenceEntry.attackIndex))
				{
					CspUtils.DebugLog("Unknown attack index in attack choreography: " + text3);
					continue;
				}
				if (--num > 0)
				{
					attackSequenceEntry.chain = true;
				}
				else
				{
					attackSequenceEntry.chain = false;
				}
				attackSequence.Add(attackSequenceEntry);
			}
		}
	}
}
