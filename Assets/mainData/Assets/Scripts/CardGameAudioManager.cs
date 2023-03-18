using System.Collections.Generic;
using UnityEngine;

public class CardGameAudioManager
{
	public enum SFX
	{
		None,
		AttackPlayed,
		DeckBlock,
		HandBlock,
		BlockClash,
		CardReveal,
		CardHover,
		CardDraw,
		FactorAnimal,
		FactorElemental,
		FactorEnergy,
		FactorSpeed,
		FactorStrength,
		FactorTech,
		FactorMulti,
		Heal,
		IntroSplashA,
		IntroSplashB,
		KeeperActivate,
		KeeperDestroy,
		OpponentTurn,
		PlayerTurn,
		PassButtonClick,
		PowerStay,
		PowerUp,
		PowerSpin,
		SlomoStart,
		SlomoEnd,
		YouLose,
		YouWin,
		Jeopardy,
		CardMoveZone,
		CardMovePlace,
		CoinToss,
		JeopardyToDiscardBG,
		JeopardyToDiscardCard,
		NewGame,
		DamageIndicator
	}

	public enum Announcer
	{
		DamageLevel1,
		DamageLevel2,
		DamageLevel3,
		DamageLevel4,
		DamageLevel5,
		DamageLevel6,
		DamageLevel7,
		DamageLevel8,
		DamageLevel9,
		KeeperActivate,
		KeeperNew,
		Misfire,
		LuckyBlock,
		PowerUp,
		SpecialEffect
	}

	private string[] SFXHandleList = new string[38]
	{
		string.Empty,
		"FX_CardGame_Attack_Played_audio",
		"FX_CardGame_Block_Deck_audio",
		"FX_CardGame_Block_Hand_audio",
		"FX_CardGame_Block_Clash_audio",
		"FX_CardGame_Card_Revealed_audio",
		"FX_CardGame_CardHover_audio",
		"FX_CardGame_Draw_audio",
		"FX_CardGame_Factor_Animal_audio",
		"FX_CardGame_Factor_Elemental_audio",
		"FX_CardGame_Factor_Energy_audio",
		"FX_CardGame_Factor_Speed_audio",
		"FX_CardGame_Factor_Strength_audio",
		"FX_CardGame_Factor_Tech_audio",
		"FX_CardGame_Factor_Multi_Factor_audio",
		"FX_CardGame_Heal_audio",
		"FX_CardGame_IntroSplashA_audio",
		"FX_CardGame_IntroSplashB_audio",
		"FX_CardGame_Keeper_Activate_audio",
		"FX_CardGame_Keeper_Destroy_audio",
		"FX_CardGame_Opponent_Turn_Start_audio",
		"FX_CardGame_Player_Turn_Start_audio",
		"FX_CardGame_PassButton_Click_audio",
		"FX_CardGame_Power_Stay_audio",
		"FX_CardGame_Power_Up_audio",
		"FX_CardGame_PowerSpinner_audio",
		"FX_CardGame_Slomo_End_audio",
		"FX_CardGame_Slomo_Start_audio",
		"FX_CardGame_YouLose_audio",
		"FX_CardGame_YouWin_audio",
		"FX_CardGame_Jeopardy_audio",
		"FX_CardGame_CardMove_Zone_audio",
		"FX_CardGame_CardMove_Place_audio",
		"FX_CardGame_Coin_Flip_audio",
		"FX_CardGame_Cards_JeopardyToDiscard_BackLayer_audio",
		"FX_CardGame_Cards_JeopardyToDiscard_audio",
		"FX_CardGame_NewGame_Begin_audio",
		"FX_CardGame_Damage_Indicator_audio"
	};

	public readonly string VO_ANNOUNCER_VOICE = "general_cardgame";

	private string[] AnnouncerHandleList = new string[15]
	{
		"VO_Announcer_Impressive_audio",
		"VO_Announcer_Awesome_audio",
		"VO_Announcer_Awesome_audio",
		"VO_Announcer_Amazing_audio",
		"VO_Announcer_Amazing_audio",
		"VO_Announcer_Astonishing_audio",
		"VO_Announcer_Sensational_audio",
		"VO_Announcer_Overwhelming_audio",
		"VO_Announcer_Insane_audio",
		"VO_Announcer_Keeper_audio",
		"VO_Announcer_Newkeeper_audio",
		"VO_Announcer_Misfire_audio",
		"VO_Announcer_LuckyBlock_audio",
		"VO_Announcer_PowerUp_audio",
		"VO_Announcer_SpecialEffect_audio"
	};

	private string[] AnnouncerVODescriptors = new string[15]
	{
		"Incredible",
		"Awesome",
		"Awesome",
		"Amazing",
		"Amazing",
		"Astonishing",
		"Sensational",
		"Overwhelming",
		"Insane",
		"Keeper",
		"NewKeeper",
		"MisfireGood",
		"LuckyBlockGood",
		"PowerUp",
		"SpecialEffect"
	};

	private Dictionary<SFX, GameObject> SFXPrefabs;

	private Dictionary<Announcer, GameObject> AnnouncerPrefabs;

	public CardGameAudioManager()
	{
		SFXPrefabs = new Dictionary<SFX, GameObject>();
		AnnouncerPrefabs = new Dictionary<Announcer, GameObject>();
	}

	public void OnAudioLoaded(AssetBundleLoadResponse response, object extraData)
	{
		for (int i = 1; i < SFXHandleList.Length; i++)
		{
			string text = SFXHandleList[i];
			GameObject gameObject = (GameObject)response.Bundle.Load(text);
			if (gameObject == null)
			{
				CspUtils.DebugLog("Failed to load " + text + " prefab");
			}
			else
			{
				SFXPrefabs.Add((SFX)i, gameObject);
			}
		}
		for (int j = 0; j < AnnouncerHandleList.Length; j++)
		{
			string text2 = AnnouncerHandleList[j];
			GameObject gameObject2 = (GameObject)response.Bundle.Load(text2);
			if (gameObject2 == null)
			{
				CspUtils.DebugLog("Failed to load " + text2 + " prefab");
			}
			else
			{
				AnnouncerPrefabs.Add((Announcer)j, gameObject2);
			}
		}
	}

	public void Play(SFX audioId)
	{
		GameObject gameObject = SFXPrefabs[audioId];
		if (gameObject == null)
		{
			CspUtils.DebugLog("Tried to play a non-existant SFX audio asset with id " + audioId.ToString());
		}
		else
		{
			ShsAudioSource.PlayAutoSound(gameObject);
		}
	}

	public void Play(Announcer audioId)
	{
		if (!Play(AnnouncerVODescriptors[(int)audioId]))
		{
			GameObject gameObject = AnnouncerPrefabs[audioId];
			if (gameObject == null)
			{
				CspUtils.DebugLog("Tried to play a non-existant announcer audio asset with id " + audioId.ToString());
			}
			else
			{
				ShsAudioSource.PlayAutoSound(gameObject);
			}
		}
	}

	public bool Play(string voName)
	{
		ResolvedVOAction vO = VOManager.Instance.GetVO("announce_cardgame", VOInputString.FromStrings(VO_ANNOUNCER_VOICE, voName));
		if (vO != null && vO.IsResolved)
		{
			VOManager.Instance.PlayResolvedVO(vO);
			return true;
		}
		return false;
	}

	public ShsAudioSource PlaySequential(SFX audioId, ShsAudioSource previous)
	{
		ShsAudioSource result = previous;
		if (previous != null)
		{
			if (previous.IsPlaying)
			{
				previous.Stop();
			}
			previous.Play();
		}
		else
		{
			GameObject gameObject = SFXPrefabs[audioId];
			if (gameObject == null)
			{
				CspUtils.DebugLog("Tried to play a non-existant SFX audio asset with id " + audioId.ToString());
			}
			else
			{
				result = ShsAudioSource.PlayFromPrefab(gameObject);
			}
		}
		return result;
	}

	public void PlayFromServer(int number, bool isLocalCard)
	{
		switch (number)
		{
		case 0:
			Play(SFX.AttackPlayed);
			break;
		case 2:
			Play(SFX.KeeperActivate);
			Play(Announcer.KeeperNew);
			break;
		case 3:
			Play(SFX.KeeperDestroy);
			Play("KeeperDestroyed");
			break;
		case 4:
			Play(Announcer.SpecialEffect);
			break;
		case 5:
			Play((!isLocalCard) ? "MisfireGood" : "MisfireBad");
			break;
		case 6:
			Play((!isLocalCard) ? "UnblockableBad" : "UnblockableGood");
			break;
		default:
			CspUtils.DebugLog("CardGameAnnouncer.PlayFromServer(): Unknown sound number " + number + ".");
			break;
		}
	}

	public void PlayFactorAttack(BattleCard.Factor factor)
	{
		switch (factor)
		{
		case BattleCard.Factor.Animal:
			Play(SFX.FactorAnimal);
			break;
		case BattleCard.Factor.Elemental:
			Play(SFX.FactorElemental);
			break;
		case BattleCard.Factor.Energy:
			Play(SFX.FactorEnergy);
			break;
		case BattleCard.Factor.Speed:
			Play(SFX.FactorSpeed);
			break;
		case BattleCard.Factor.Strength:
			Play(SFX.FactorStrength);
			break;
		case BattleCard.Factor.Tech:
			Play(SFX.FactorTech);
			break;
		default:
			Play(SFX.AttackPlayed);
			break;
		}
	}

	public void PlayAnnouncerDamage(int damage, bool inflictedOnOpponent)
	{
		if (inflictedOnOpponent)
		{
			string[] array = null;
			if (damage >= 9)
			{
				array = new string[3]
				{
					"Spectacular",
					"Invincible",
					"Uncanny"
				};
			}
			else if (damage >= 6)
			{
				array = new string[4]
				{
					"Sensational",
					"Incredible",
					"Astonishing",
					"Fantastic"
				};
			}
			else if (damage >= 4)
			{
				array = new string[3]
				{
					"Mighty",
					"Awesome",
					"Amazing"
				};
			}
			if (array != null)
			{
				Play(array[Random.Range(0, array.Length)]);
			}
		}
		else
		{
			string[] array2 = null;
			if (damage >= 8)
			{
				array2 = new string[4]
				{
					"HangInThere",
					"ThatStings",
					"ThatsGottaHurt",
					"AwMan"
				};
			}
			else if (damage >= 4)
			{
				array2 = new string[4]
				{
					"Ouch",
					"Oo",
					"Ow",
					"Augh"
				};
			}
			if (array2 != null)
			{
				Play(array2[Random.Range(0, array2.Length)]);
			}
		}
	}

	public void PlayEndGameVO(bool playerWon)
	{
		string[] array = null;
		array = ((!playerWon) ? new string[3]
		{
			"BetterLuckNextTime",
			"GetEmNextTime",
			"ToughBreak"
		} : new string[4]
		{
			"AmazingVictory",
			"ExcellentWin",
			"Excelsior",
			"AwesomeGame"
		});
		Play(array[Random.Range(0, array.Length)]);
	}
}
