using CardGame;
using UnityEngine;

public class CardGameEvent
{
	public class ServerMessage : ShsEventMessage
	{
		public readonly string[] args;

		public ServerMessage(string[] payload)
		{
			args = payload;
		}

		public override string ToString()
		{
			switch (args[0])
			{
			case "Anim":
				return string.Format("Anim: type={0}, serverID={1}, playerOwned={2}", args[2], args[3], args[4]);
			case "Block":
				return string.Format("Block: weaponID={0}, blockerID={1}, meDefending={2}", args[2], args[3], args[4]);
			case "Buff":
				return string.Format("Buff: playerID={0}, type={1}, card={2}, buffMe={3}", args[2], args[3], args[4], args[5]);
			case "Damage":
				return string.Format("Damage: weaponID={0}, casualties={1}, srcDeprecated={2}, mine={3}, typeList={4}, total={5}, becomeKeeper={6}, killKeeper={7}", args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]);
			case "Debug":
				return string.Format("Debug: {0}", args[2]);
			case "GameOver":
				return string.Format("GameOver: reason={0}, iWon={1}", args[2], args[3]);
			case "Highlight":
				return string.Format("Highlight: animType={0}, cardID={1}, activate={2}", args[2], args[3], args[4]);
			case "Info":
				return string.Format("Info: message={0}, arg1={1}, arg2={2}", args[2], args[3], args[4]);
			case "InitCards":
				return string.Format("InitCards: min={0}, max={1}, mine={2}", args[2], args[3], args[4]);
			case "MoveCard":
				return string.Format("MoveCard: cardID={0}, src={1}, dest={2}, mine={3}, cardType={4}", args[2], args[3], args[4], args[5], args[6]);
			case "NewTurn":
				return string.Format("NewTurn: mine={0}", args[2]);
			case "NoRewards":
				return "NoRewards";
			case "PickCard":
				return string.Format("PickCard: cards={0}, canPass={1}, pickCardType={2}", args[2], args[3], args[4]);
			case "PickFactor":
				return string.Format("PickFactor: {0}", args[0]);
			case "PickNumber":
				return string.Format("PickNumber: min={0}, max={1}", args[2], args[3]);
			case "SetPower":
				return string.Format("SetPower: newPower={0}, coinFlip={1}", args[2], args[3]);
			default:
				return string.Format("Unknown server message: {0}", args[0]);
			}
		}
	}

	public class AvatarSpawned : ShsEventMessage
	{
		public SquadBattleCharacterSpawnData SpawnData;

		public SquadBattlePlayerEnum PlayerEnum;

		public AvatarSpawned(SquadBattleCharacterSpawnData SpawnData, SquadBattlePlayerEnum PlayerEnum)
		{
			this.SpawnData = SpawnData;
			this.PlayerEnum = PlayerEnum;
		}
	}

	public class AnimFinished : ShsEventMessage
	{
	}

	public class CombatFinished : ShsEventMessage
	{
	}

	public class ResumeServerQueue : ShsEventMessage
	{
	}

	public class SlowMotion : ShsEventMessage
	{
	}

	public class NoMoSloMo : ShsEventMessage
	{
	}

	public class DealerChat : ShsEventMessage
	{
		public enum MessageType
		{
			Dealer,
			Prompt
		}

		public MessageType msgType;

		public int msgId;

		public string text;

		public object[] args;

		public DealerChat(MessageType msgType, int msgId, string text, params object[] args)
		{
			this.msgType = msgType;
			this.msgId = msgId;
			this.text = text;
			this.args = args;
		}
	}

	public class FullCardDetails : ShsEventMessage
	{
		public Texture2D textureSource;

		public bool leftSide;

		public FullCardDetails(Texture2D textureSource, bool leftSide)
		{
			this.textureSource = textureSource;
			this.leftSide = leftSide;
		}
	}

	public class KeeperMisfire : ShsEventMessage
	{
		public int playerIndex;

		public KeeperMisfire(int _playerIndex)
		{
			playerIndex = _playerIndex;
		}
	}

	public enum AttackResultType
	{
		KeeperMisfire,
		HandBlock,
		DeckBlock,
		LuckyBlock,
		Damage
	}

	public class AttackResult : ShsEventMessage
	{
		public AttackResultType type;

		public int playerIndex;

		public bool isDamage;

		public bool isDeckBlock;

		public int damageDone;

		public AttackResult(int _playerIndex, AttackResultType _type, bool _isDamage, bool _isDeckBlock, int _damage)
		{
			type = _type;
			playerIndex = _playerIndex;
			isDamage = _isDamage;
			isDeckBlock = _isDeckBlock;
			damageDone = _damage;
		}
	}

	public class ClickedPass : ShsEventMessage
	{
		public PickCardType pickCardType;

		public ClickedPass(PickCardType pickCardType)
		{
			this.pickCardType = pickCardType;
		}
	}

	public class EnablePassButton : ShsEventMessage
	{
		public enum PassButtonType
		{
			Pass,
			Done
		}

		public PassButtonType ButtonType;

		public PickCardType pickCardType;

		public EnablePassButton(PassButtonType ButtonType, PickCardType pickCardType)
		{
			this.pickCardType = pickCardType;
			this.ButtonType = ButtonType;
		}
	}

	public class DisablePassButton : ShsEventMessage
	{
	}

	public class ClickedPoke : ShsEventMessage
	{
	}

	public class ShowPokeButton : ShsEventMessage
	{
	}

	public class HidePokeButton : ShsEventMessage
	{
	}

	public class EnablePokeButton : ShsEventMessage
	{
		public bool shouldBeEnabled;

		public EnablePokeButton(bool ShouldBeEnabled)
		{
			shouldBeEnabled = ShouldBeEnabled;
		}
	}

	public class PlayerPoked : ShsEventMessage
	{
		public bool isLocalPlayerPoked;

		public int timer;

		public PlayerPoked(int Timer, bool IsLocalPlayer)
		{
			isLocalPlayerPoked = IsLocalPlayer;
			timer = Timer;
		}
	}

	public class PokedTimerCompleted : ShsEventMessage
	{
	}

	public class DeckListLoaded : ShsEventMessage
	{
	}

	public class ShowVersusIcon : ShsEventMessage
	{
	}

	public class IntroSequenceFinished : ShsEventMessage
	{
	}

	public class SetPowerLevel : ShsEventMessage
	{
		public int oldPower;

		public int newPower;

		public SetPowerLevel(int oldPower, int newPower)
		{
			this.oldPower = oldPower;
			this.newPower = newPower;
		}
	}

	public class ShowCardGameHud : ShsEventMessage
	{
	}

	public class CardGameResults : ShsEventMessage
	{
		public int tickets;

		public int xp;

		public int silver;

		public string card;

		public string ownable;

		public CardGameResults(int tickets, int xp, int silver, string card, string ownable)
		{
			this.tickets = tickets;
			this.xp = xp;
			this.silver = silver;
			this.card = card;
			this.ownable = ownable;
		}
	}
}
