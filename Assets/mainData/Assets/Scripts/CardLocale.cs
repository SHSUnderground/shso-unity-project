using System.Collections.Generic;

public class CardLocale : Dictionary<int, string>
{
	public const int STRING_ID_DECK = 100;

	public const int STRING_ID_HAND = 101;

	public const int STRING_ID_BLOCK = 102;

	public const int STRING_ID_DAMAGE = 103;

	public const int STRING_ID_MISFIRE = 104;

	public const int GAME_OVER_WIN = 134;

	public const int GAME_OVER_LOSE = 135;

	public const int PICK_DISCARD = 136;

	public const int COIN_FLIP_YES = 200;

	public const int COIN_FLIP_NO = 201;

	public const int STRING_ID_HUMAN_BLOCK = 127;

	public const int STRING_ID_OPPONENT_BLOCK = 128;

	public const int STRING_ID_SELECT_SOAK_CARD = 129;

	public const int STRING_ID_PICK_ATTACK_OR_PASS = 130;

	public const int STRING_ID_PICK_BLOCK_OR_RISK_IT = 131;

	public const int STRING_ID_PICK_PASS = 132;

	public const int STRING_ID_PICK_RISK_IT = 133;

	public const int STRING_ID_PICK_TO_DECK = 150;

	public const int STRING_ID_PICK_DEPLOY = 151;

	public const int STRING_ID_PICK_DESTROY = 152;

	public const int STRING_ID_PICK_TO_HAND = 153;

	public const int STRING_ID_PICK_KEEPER = 154;

	public const int STRING_ID_PICK_RECYCLE = 155;

	public const int STRING_ID_PICK_REVEAL = 156;

	public const int STRING_ID_PICK_DONE = 157;

	public const int STRING_ID_HUMAN_KEEPER_MISFIRED = 140;

	public const int STRING_ID_OPPONENT_KEEPER_MISFIRED = 141;

	public const int STRING_ID_PLAYER_STARTS = 300;

	public const int STRING_ID_OPPONENT_STARTS = 301;

	public const int STRING_ID_COIN_FLIP_HEADS = 401;

	public const int STRING_ID_COIN_FLIP_TAILS = 402;

	public const int STRING_ID_DEALER_FIRST_TURN = 500;

	public const int STRING_ID_DEALER_POWER = 501;

	public const int STRING_ID_DEALER_NEW_TURN = 502;

	public const int STRING_ID_DEALER_PASS = 503;

	public const int STRING_ID_DEALER_PLAY = 504;

	public const int STRING_ID_DEALER_BLOCK = 505;

	public const int STRING_ID_DEALER_LUCKY_BLOCK = 506;

	public const int STRING_ID_DEALER_DAMAGE = 507;

	public const int STRING_ID_DEALER_SPECIAL = 508;

	public const int STRING_ID_DEALER_NEW_KEEPER = 509;

	public const int STRING_ID_DEALER_DISCARD = 510;

	public const int STRING_ID_DEALER_TRIGGER_KEEPER = 511;

	public const int STRING_ID_DEALER_MISFIRE_KEEPER = 512;

	public const int STRING_ID_PICK_NUMBER = 600;

	public const int STRING_ID_PICK_FACTOR = 601;

	public const int STRING_ID_PICK_DESTINATION = 602;

	private static CardLocale _instance = new CardLocale();

	private CardLocale()
	{
		Add(100, "#cardgame_deck");
		Add(101, "#cardgame_hand");
		Add(102, "#cardgame_block");
		Add(103, "#cardgame_damage");
		Add(104, "#cardgame_misfire");
		Add(127, "#cardgame_you_deck_blocked");
		Add(128, "#cardgame_opponent_deck_blocked");
		Add(129, "#cardgame_select_soak_card");
		Add(130, "#cardgame_pick_attack_or_pass");
		Add(131, "#cardgame_pick_block_or_risk_it");
		Add(132, "#cardgame_pick_pass");
		Add(133, "#cardgame_pick_risk_it");
		Add(134, "#cardgame_you_win");
		Add(135, "#cardgame_opponent_wins");
		Add(136, "#cardgame_pick_discard");
		Add(140, "#cardgame_player_keeper_misfired");
		Add(141, "#cardgame_opponent_keeper_misfired");
		Add(150, "#cardgame_pick_to_deck");
		Add(151, "#cardgame_pick_deploy");
		Add(152, "#cardgame_pick_destroy");
		Add(153, "#cardgame_pick_to_hand");
		Add(154, "#cardgame_pick_keeper");
		Add(155, "#cardgame_pick_recycle");
		Add(156, "#cardgame_pick_reveal");
		Add(157, "#cardgame_pick_done");
		Add(200, "#cardgame_power_increased");
		Add(201, "#cardgame_power_no_increase");
		Add(300, "#cardgame_player_goes_first");
		Add(301, "#cardgame_opponent_goes_first");
		Add(401, "#cardgame_heads");
		Add(402, "#cardgame_tails");
		Add(500, "#cardgame_dealer_first_turn");
		Add(501, "#cardgame_dealer_power");
		Add(502, "#cardgame_dealer_new_turn");
		Add(503, "#cardgame_dealer_pass");
		Add(504, "#cardgame_dealer_play");
		Add(505, "#cardgame_dealer_block");
		Add(506, "#cardgame_dealer_lucky_block");
		Add(507, "#cardgame_dealer_damage");
		Add(508, "#cardgame_dealer_special");
		Add(509, "#cardgame_dealer_new_keeper");
		Add(510, "#cardgame_dealer_discard");
		Add(511, "#cardgame_dealer_trigger_keeper");
		Add(512, "#cardgame_dealer_misfire_keeper");
		Add(600, "#cardgame_pick_number");
		Add(601, "#cardgame_pick_factor");
		Add(602, "#cardgame_pick_destination");
	}

	public static string Parse(int msg)
	{
		return (!_instance.ContainsKey(msg)) ? string.Empty : _instance[msg];
	}
}
