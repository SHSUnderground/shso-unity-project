using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;



public class CabToU3d : MonoBehaviour {

	//private List<AssetInfo> assets = new List<AssetInfo>();
	System.Security.Cryptography.MD5CryptoServiceProvider md = null;
        

	string cachePath = "C:\\Users\\cpinh\\AppData\\LocalLow\\Unity\\WebPlayer\\Cache\\SHS\\";
	string cabPath = "D:\\SHSO-CABS\\";
	string stubCabName = "CustomAssetBundle-621023522351998429979685bf658da7";  
																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																												
	Hashtable cabMap = new Hashtable(){

		{"gameworld_activity_objects.unity3d","CustomAssetBundle-bb1832f89ef216642be829941c50bbbd"},
		{"general.unity3d","CustomAssetBundle-70176f3cf95f78247bf082eab4c79068"},
		{"global_persistent_audio.unity3d","CustomAssetBundle-fd67f0886bb2a1148b6ad06fbe5d9dbf"},
		{"en-us_strings.unity3d","CustomAssetBundle-d7c42b2e7e070b34f9baeb850fe5a28e"},
		{"en_us_font_bundle.unity3d","CustomAssetBundle-58d3cb5c97f451a4e8d855a8e1ba6382"},
		{"common_bundle.unity3d","CustomAssetBundle-87dc77116285bda4cbf5160de1dd6fbf"},
		{"en_us_common_bundle.unity3d","CustomAssetBundle-1809e44292531de499087c16db48c690"},
		{"debug_bundle.unity3d","CustomAssetBundle-fdeb5c9793ce5eb4ab1772e3b4f53d48"},
		{"characters_bundle.unity3d","CustomAssetBundle-574fef0ead715fc45989c09fb6da021e"},
		{"en_us_characters_bundle.unity3d","CustomAssetBundle-4a737d92e21f54347b93b385118d46c5"},
		{"notification_bundle.unity3d","CustomAssetBundle-4d10bf5b352d8c445aa66a4f04a13b4e"},
		{"en_us_notification_bundle.unity3d","CustomAssetBundle-db26e812288ac12408ce4b37cc77b70b"},
		{"communication_bundle.unity3d","CustomAssetBundle-2fd2f040b8a4fee4c882468ad7c4977a"},
		{"en_us_communication_bundle.unity3d","CustomAssetBundle-4dc92d85253534344b6d842bf2211b73"},
		{"options_bundle.unity3d","CustomAssetBundle-4c088f44125fad944a7c13210ee6822d"},
		{"en_us_options_bundle.unity3d","CustomAssetBundle-19dff6b10ecbc5943a1c424521b75adc"},
		{"prizewheel_bundle.unity3d","CustomAssetBundle-810c4691e35723c4691f28a0d7a50b95"},
		{"en_us_prizewheel_bundle.unity3d","CustomAssetBundle-1cf26f20fa5619d40bde05d2de95318c"},
		{"shopping_bundle.unity3d","CustomAssetBundle-4a737d92e21f54347b93b385118d46c5"},
		{"en_us_shopping_bundle.unity3d","CustomAssetBundle-a82ea4f6a7ea1e344a6caa8e2396ac45"},
		{"zonechooser_bundle.unity3d","CustomAssetBundle-0ef231bd7baf95f4a996d343f9c9b147"},
		{"en_us_zonechooser_bundle.unity3d","CustomAssetBundle-974fc5e7c7f8f23409f6c7801edfde5f"},
		{"missions_bundle.unity3d","CustomAssetBundle-6e15c1b82f7328244b7e556378d3be2e"},
		{"en_us_missions_bundle.unity3d","CustomAssetBundle-5a43f6f8830782548832e1c1490e1662"},
		{"en_us_missionflyers_bundle.unity3d","CustomAssetBundle-dcc075b6e62d0a442a10481770f0fd94"},
		{"brawlergadget_bundle.unity3d","CustomAssetBundle-649e6d4edb4c24546996fad398f94b8d"},
		{"en_us_brawlergadget_bundle.unity3d","CustomAssetBundle-0c135ad50eb7b1849a06a0584682d6a9"},
		{"cardgamegadget_bundle.unity3d","CustomAssetBundle-84f17a610b889fa43bafc84e491f3c98"},
		{"en_us_cardgamegadget_bundle.unity3d","CustomAssetBundle-e18c4ea7bf73fdd42bd86a88b5518d02"},
		{"mysquadgadget_bundle.unity3d","CustomAssetBundle-503b5e5c7176462458425a0c09c42ff9"},
		{"en_us_mysquadgadget_bundle.unity3d","CustomAssetBundle-f092978e5b09e0f40a194457787e2551"},
		{"challengerewards_bundle.unity3d","CustomAssetBundle-8562139aec0988645b9efe927c58c358"},
		{"achievement_bundle.unity3d","CustomAssetBundle-4c46fa1d9f6e77a468868442dfe5a932"},
		{"en_us_achievement_bundle.unity3d","CustomAssetBundle-c8d26adbf5cdcd84a975bd62c2da4b7f"},
		{"persistent_bundle.unity3d","CustomAssetBundle-4a9bd57e8e641ca41964c582b840a884"},
		{"en_us_persistent_bundle.unity3d","CustomAssetBundle-44453a944ea8cfe4aadd9cd19ae7686c"},
		{"smarttip_bundle.unity3d","CustomAssetBundle-bd8fcc270b46cdc49a831f15369cc549"},
		{"loading_bundle.unity3d","CustomAssetBundle-623d8db7256c2204ca6f017198717594"},
		{"en_us_loading_bundle.unity3d","CustomAssetBundle-3feb211e042787a4997fdacc067b454c"},
		//{"ui_audio.unity3d","CustomAssetBundle-c583943e2ccb2f14f82b057567ed3493"},
		{"hq_bundle.unity3d","CustomAssetBundle-788be68a11cebdf4ab601ab41c252f04"},
		{"en_us_hq_bundle.unity3d","CustomAssetBundle-ab78f217ab7f19e4ebf84c1788167001"},
		{"items_bundle.unity3d","CustomAssetBundle-c436840e8b013684299e308fa3ee6efd"},
		{"goodiescommon_bundle.unity3d","CustomAssetBundle-fa3447e11f7520e4184f2741d46673f3"},
		{"goodieseffects_bundle.unity3d","CustomAssetBundle-b667db71fc9ffcf429b47880c6bb9591"},
		{"goodiesstats_bundle.unity3d","CustomAssetBundle-ed3fa4917bef4854cb1c71f957f6e0e9"},
		{"potioneffects_bundle.unity3d","CustomAssetBundle-025ed58a9106f1244b859125144d0a7f"},
		{"login_bundle.unity3d","CustomAssetBundle-f3fc6e640aeb4354ca6d133f7b9d60a6"},
		{"en_us_login_bundle.unity3d","CustomAssetBundle-3187669466c5cd144856f54f53ff04cc"},
		{"en_us_cardgame_tutorial_bundle.unity3d","CustomAssetBundle-8c80d288b5d54c74e9ddf170aff11908"},
		{"gameworld_common_objects.unity3d","CustomAssetBundle-c16b603c91a09b547a9563788c4007ef"},
		{"hud_bundle.unity3d","CustomAssetBundle-14e5e6cf949a0b74f829873e681d23aa"},
		{"gameworld_bundle.unity3d","CustomAssetBundle-622100ef7ef099f4984ee3659948726f"},
		{"en_us_gameworld_bundle.unity3d","CustomAssetBundle-96d979f90f3cc5f45bf01ac0cd184d3f"},
		{"spiderman_04.unity3d","CustomAssetBundle-44c618df8c40b2b45b4051aa382c7e7e"},
		//{"mr_placeholder_01.unity3d","CustomAssetBundle-4900de06f5032fd4ebcb658335f140da"},
		{"daily_bugle2_scenario.unity3d","CustomAssetBundle-3af1212b9daccf2469a4d8a180a82a09"},
		{"daily_bugle2.unity3d","CustomAssetBundle-bda4e6859dbdaed4ab6936edd696c460"},
		{"city_ambiance_audio.unity3d","CustomAssetBundle-a98b496afcffc604e959c9173d1195f9"},
		{"spider_man_ultimate_vo_audio.unity3d","CustomAssetBundle-59a660abae92341469db65d3383ad5b0"},
		{"jean_grey_fx.unity3d","CustomAssetBundle-4fad359f715a0ce4dae5b1fe372c6654"},
		{"brawler_bundle.unity3d","CustomAssetBundle-50f5dce0314db6d47bded71065c849cc"},
		{"en_us_brawler_bundle.unity3d","CustomAssetBundle-21e87198453cc6642acd8a3f9cce4230"},
		{"kingpin_fx.unity3d","CustomAssetBundle-8721c5330d393e64dbda812c0818a467"},
		{"shared_character_fx.unity3d","CustomAssetBundle-ab09f0d9a65a69647b3c45e6810d54ac"},
		{"shared_brawler_fx.unity3d","CustomAssetBundle-8f7f3c1f2816ff14d9c7e9f21fcb5523"},
		//{"kingpin_thug_01.unity3d","CustomAssetBundle-c5ae7103d8f6bff45803452fff99fb26"},
		//{"kingpin_thug_02.unity3d","CustomAssetBundle-0e49ec809528db64b9a1c4a607a77a5a"},
		//{"kingpin_thug_03.unity3d","CustomAssetBundle-d37b822db68ffdb4ca48ae9bcc3afe22"},
		{"kingpin_boss_vo_audio.unity3d","CustomAssetBundle-c49ed819daf058d46813d8a954812fe6"},
		//{"hecks_kitchen_alley_2_audio.unity3d","CustomAssetBundle-6e0c6d92dc1cbe345b3100d122795d98"},
		//{"Hecks_Kitchen_tile_Background_01_Lightning.unity3d","CustomAssetBundle-2bf0ea2497b8b0a4d8ad50025c8dc4b7"},
		//{"kingpin_boss_02.unity3d","CustomAssetBundle-d2780ea8517ffeb4da271d5c643fe201"},
		//{"baxter.unity3d","CustomAssetBundle-3465eabfcf201144cafab543385ace93"},
		//{"Villainville.unity3d","CustomAssetBundle-d27f60472feff00488eee015df0dc346"},
		//{"Villainville_scenario.unity3d","CustomAssetBundle-5fce7ce5c74821945865575b84b109b0"},
		{"black_widow_01.unity3d","CustomAssetBundle-11e8abccecc59d548abd14c6b7f8f3d2"},
		{"carnage_01.unity3d","CustomAssetBundle-87674e9c6557ad84fa9ecfe6e35a5c7b"},
		{"cyclops_01.unity3d","CustomAssetBundle-666c33b25a2d8a84280422dd48635c4a"},
		//{"silver_surfer_02.unity3d","CustomAssetBundle-2e582b6941f432f4b82ed0fe78a7cc86"},
		{"falcon_01.unity3d","CustomAssetBundle-721a9600ec983254186134dea6f52296"},
		{"hulk_01.unity3d","CustomAssetBundle-116b0c36b3c43bb479e63b73acedf358"},
		{"jean_grey_01.unity3d","CustomAssetBundle-ad2dcb69feadbef43b5c2833452ce9a5"},
		{"ms_marvel_01.unity3d","CustomAssetBundle-7ea062ccf93329544a189d57ecb897e5"},
		{"ironman_01.unity3d","CustomAssetBundle-33871964ec8b1d94999191563ed99bcc"},
		{"kingpin_boss_02.unity3d","CustomAssetBundle-d2780ea8517ffeb4da271d5c643fe201"},
		{"she_hulk_01.unity3d","CustomAssetBundle-db3fe439b4cf3154c81495603941c146"},
		{"silver_surfer_02.unity3d","CustomAssetBundle-2e582b6941f432f4b82ed0fe78a7cc86"},
		{"super_skrull_01.unity3d","CustomAssetBundle-306f2458c4693214eb3000bd8d598099"},
		{"thing_01.unity3d","CustomAssetBundle-a41072177488444498b1e1037d6c0b85"},
		{"spiderman_01.unity3d","CustomAssetBundle-68b6995dd1cb75e4184f9b917b7b64dc"},
		//{"baxter_scenario.unity3d","CustomAssetBundle-16bcc258d8a6e9b4b85ee960fb8f6658"},
		//{"baxter.unity3d","CustomAssetBundle-60c08d4f7cbcf5042b54d7676fe3c5ca"},
		//{"Asgard.unity3d","CustomAssetBundle-5fbf55080243b8d42bab600deb08b544"},
		//{"Asgard_scenario.unity3d","CustomAssetBundle-a6856d78bf5dd08489a5e636ffda9940"},
		//{"Villainville.unity3d","CustomAssetBundle-742bd3eca769a9d4391e44a8fef098ce"},
		//{"Villainville_scenario.unity3d","CustomAssetBundle-aa11557d8e9924f4eb2a48320b1f83a0"},
		{"brawler_audio.unity3d","CustomAssetBundle-b447867be4d34b447b76631a16aa55b5"},
		{"dr_doom_boss_01.unity3d","CustomAssetBundle-b0a1fc75e20a0d54780e62da360ae05a"},
		{"dr_octopus_boss_01.unity3d","CustomAssetBundle-1de8d9ee5c4a9ea44aad42628666d76d"},
		{"dr_octopus_boss_vo_audio.unity3d","CustomAssetBundle-8b3045cccdc177c42a2d8237554c1888"},
		{"iron_man_fx.unity3d","CustomAssetBundle-171e84a25962f8f4bbb15b2954e8afc6"},
		{"black_widow_fx.unity3d","CustomAssetBundle-066ba9b6358a69a419d201edf7e339cc"},
		{"daredevil_fx.unity3d","CustomAssetBundle-82af5bf257e29da439c990cb20c3b2b3"},
		{"american_dream_vo_audio.unity3d","CustomAssetBundle-a4caa4b6c52473c49b4b0fc9d84b307b"},
		{"angel_vo_audio.unity3d","CustomAssetBundle-6ef80b07b512264429a17a83f6003593"},
		{"annihilus_playable_vo_audio.unity3d","CustomAssetBundle-9dd134a588a500041b1e18591a979e7c"},
		{"ant_man_vo_audio.unity3d","CustomAssetBundle-c94ba68e66c7f524bb39bafb05b175ad"},
		{"anti_venom_vo_audio.unity3d","CustomAssetBundle-7e8d2e6107dbc4043afdef63590ef10e"},
		{"archangel_vo_audio.unity3d","CustomAssetBundle-e622699469ce0d84785d9a9fb625f5db"},
		{"giant_man_vo_audio.unity3d","CustomAssetBundle-012efca9b2af7b6408d33223039d958c"},
		{"iron_man_vo_audio.unity3d","CustomAssetBundle-65316d19d35789e4fab5e9844019639e"},
		{"punisher_vo_audio.unity3d","CustomAssetBundle-df30b298561415046abb1031669ba9e3"},
		{"punisher_thunderbolts_vo_audio.unity3d","CustomAssetBundle-7777f8a754da5f8479a13f118b296da7"},
		{"spider_man_vo_audio.unity3d","CustomAssetBundle-51156d82942e2614a8d1bb49f59a30fd"},
		//{"spider_man_ultimate_vo_audio.unity3d","CustomAssetBundle-59a660abae92341469db65d3383ad5b0"},
		{"juggernaut_boss_vo_audio.unity3d","CustomAssetBundle-ed5208e040832fd408a0cfbf424323db"},
		{"loki_boss_vo_audio.unity3d","CustomAssetBundle-1369140aad3630c4b8e11ee6b57a9186"},
		{"surtur_boss_vo_audio.unity3d","CustomAssetBundle-5ab87ecbfb818a74582e96d636dc8299"},
		{"ymir_boss_vo_audio.unity3d","CustomAssetBundle-89ee91457ec86f74983b142791faf373"},
		{"generic_adult_male_beta_vo_audio.unity3d","CustomAssetBundle-39a0c46e35089df49a5bac61a14952e6"},
		{"hecks_kitchen_rooftops_1_audio.unity3d","CustomAssetBundle-9c93e68eb759b744781b20d7832fad61"},
		{"hecks_kitchen_rooftops_2_audio.unity3d","CustomAssetBundle-fb664181d05f3ac418798cfed23ec4e8"},
		{"hecks_kitchen_alley_1_audio.unity3d","CustomAssetBundle-fce89b9a5fd78e04db6b6c1920290a4e"},
		{"hecks_kitchen_alley_2_audio.unity3d","CustomAssetBundle-6e0c6d92dc1cbe345b3100d122795d98"},
		{"redskull_boss_audio.unity3d","CustomAssetBundle-6ef44dd69197b3f47b6265cba239d043"},
		{"redskull_alternatereality_audio.unity3d","CustomAssetBundle-92ad02da3af6e9c4686ff24cb98661bd"},
		{"vv_factory_1_audio.unity3d","CustomAssetBundle-7731a7d1a3e0d074b97115352bfb47a2"},
		{"vv_factory_2_audio.unity3d","CustomAssetBundle-036ab6f84b719624295571c449228df6"},
		{"vv_factory_3_audio.unity3d","CustomAssetBundle-0823cfd2ca0594e40a0aaaad4e09773d"},
		{"vv_sewers_1_audio.unity3d","CustomAssetBundle-65a1d91bbd1099d41be716536e416ae7"},
		{"vv_sewers_2_audio.unity3d","CustomAssetBundle-81fd68d31dcebd8449968ca8957d464d"},
		{"vv_sewers_3_audio.unity3d","CustomAssetBundle-0c13dda1b97956c4b96959d037ed4039"},
		{"vv_streets_1_audio.unity3d","CustomAssetBundle-8704cd3ec8e35fc4cbb6e91f5af161ba"},
		{"vv_streets_2_audio.unity3d","CustomAssetBundle-cabdac88aece5ac48ae86f356d9cac29"},
		{"vv_streets_3_audio.unity3d","CustomAssetBundle-8f9a9bb6fe015704b8244561970ac193"},
		{"shc_rooftops_1_audio.unity3d","CustomAssetBundle-4608a88b896c9f44294107b0c417a1cd"},
		{"shc_rooftops_2_audio.unity3d","CustomAssetBundle-2a3abc605a0a91547a2fe7f2f80e37b7"},
		{"shc_rooftops_3_audio.unity3d","CustomAssetBundle-ec03d44935a61164e91acc6616939c8e"},
		{"shc_subway_1_audio.unity3d","CustomAssetBundle-e9b2ba264956ef144a93d6267f91e9d0"},
		{"shc_subway_2_audio.unity3d","CustomAssetBundle-40a5a11fe1d407343a442d15c18b0ab3"},
		{"onslaught_astralplane_audio.unity3d","CustomAssetBundle-691f94631f91f3449b0fd4c8ce0ff8b4"},
		{"onslaught_boss_audio.unity3d","CustomAssetBundle-49524a0f16378a94c84f9fb30c4c6e75"},
		{"skrull_spaceship_1_audio.unity3d","CustomAssetBundle-41f3a03f28a30b1468da82267e3aa5a5"},
		{"skrull_spaceship_2_audio.unity3d","CustomAssetBundle-2cc113203c691c14f8d1a06631ac70ee"},
		{"skrull_spaceship_3_audio.unity3d","CustomAssetBundle-fb85f57930544284a8fa044974a7c655"},
		{"mission_complete_audio.unity3d","CustomAssetBundle-6aebe4e50a63857449c67e99e72e2710"},
		{"helicarrier_audio.unity3d","CustomAssetBundle-89d33433f84e06c47ad8b9849194ed5b"},
		{"silent_audio.unity3d","CustomAssetBundle-e0c8ede7b19820848b6a61a39011a478"},
		{"ui_audio.unity3d","CustomAssetBundle-c583943e2ccb2f14f82b057567ed3493"},
		{"gameworld_bugle_audio.unity3d","CustomAssetBundle-95b3bff490762fb4ba708eeeeae65a55"},
		{"baxter_fallen_1_audio.unity3d","CustomAssetBundle-324ebc3091c145a4398f59386ea0c7c5"},
		{"baxter_fallen_2_audio.unity3d","CustomAssetBundle-57acfee191c9da74aae9c24d391987e8"},
		{"asteroid_m_1_audio.unity3d","CustomAssetBundle-519e8b3e6d68c7744ab18df8a5738b0f"},
		{"asteroid_m_2_audio.unity3d","CustomAssetBundle-f47bb69a673946544ad0471e7f4771ec"},
		{"asteroid_m_3_audio.unity3d","CustomAssetBundle-655789ca87e2f744197857c642982ed8"},
		{"asgard_maze_audio.unity3d","CustomAssetBundle-c7e98ee12fa9bf841baa427c663438c8"},
		{"asgard_frost_3_audio.unity3d","CustomAssetBundle-10d2b08ecc111004aaa64acf79a7e504"},
		{"asgard_fire_1_audio.unity3d","CustomAssetBundle-79c0a5defa7522546a190dfcfa8d6c61"},
		{"asgard_fire_2_audio.unity3d","CustomAssetBundle-351221a5167e54e48b82f6d502ac7bd7"},
		{"asgard_fire_3_audio.unity3d","CustomAssetBundle-1f765d7c69addc243a509f5ec287a288"},
		{"asgard_crazydoorroom_audio.unity3d","CustomAssetBundle-b5ecbb4d2c1697840b0d1fa8c5de933a"},
		{"asgard_city_1_audio.unity3d","CustomAssetBundle-25c5356b8ff67d44c84aa5cb4c309e55"},
		{"asgard_city_2_audio.unity3d","CustomAssetBundle-c2db29826e2d5b24d863bcef405fd5d5"},
		{"asgard_city_3_audio.unity3d","CustomAssetBundle-da50ad62e4bff984d8f55eeb74f5e04c"},
		{"deckbuilder_ui_audio.unity3d","CustomAssetBundle-1181247414705d64eaf7d15d07b39f2a"},
		{"juggernaut_boss_01.unity3d","CustomAssetBundle-47960c4ad733ff645841a0217894ced4"},
		{"loki_boss_01.unity3d","CustomAssetBundle-7ea4f72523abcb345a648a5390a6804e"},
		{"sabretooth_boss_01.unity3d","CustomAssetBundle-23ecd57a718256243864fa35a7004486"},
		{"surtur_boss_01.unity3d","CustomAssetBundle-af604bdc2ed4cd74e9e05a857bf6cc9c"},
		//{"ymir_boss_vo_audio.unity3d","CustomAssetBundle-d705d4284a9a1db4087dc79cf0dfa2bb"},
		{"Asgard_Fire_Arena.unity3d","CustomAssetBundle-0c6ec92b6bde6f94c93c810f6610cda6"},
		{"Asgard_Fire_Tile_Diagonal.unity3d","CustomAssetBundle-59084cd3d99bb6f4fa0224f6951b19a0"},
		{"Asgard_Fire_Tile_End.unity3d","CustomAssetBundle-5fc76c0b24cff1e458fe652aafa55b24"},
		{"Asgard_Fire_Tile_Exterior_Corner.unity3d","CustomAssetBundle-1e42a713ff598b04ebac34e4fb1b7f43"},
		{"aim_agent_01.unity3d","CustomAssetBundle-0d5d30a634f3efb4e84d9958fe0d5443"},
		{"aim_agent_02.unity3d","CustomAssetBundle-6a26747f42845964e8c742ab3a20c9cc"},
		{"aim_agent_04.unity3d","CustomAssetBundle-8e131227fa8b25142b463131a430e699"},
		{"doombot_01.unity3d","CustomAssetBundle-c17bee0da7ea59d4c9a54813a2175161"},
		{"doombot_02.unity3d","CustomAssetBundle-c17bee0da7ea59d4c9a54813a2175161"},
		{"doombot_03.unity3d","CustomAssetBundle-c17bee0da7ea59d4c9a54813a2175161"},
		{"doombot_05.unity3d","CustomAssetBundle-c17bee0da7ea59d4c9a54813a2175161"},
		{"doombot_popcorn_minion_01.unity3d","CustomAssetBundle-59a8d160b97cac2499cac9ebe07aca87"},
		{"dr_doombot_01.unity3d","CustomAssetBundle-d81db15be3d66ab44a0f0d608ab8acd9"},
		{"extremis_trooper_01.unity3d","CustomAssetBundle-4cecac7fad3632f4d9d0e0c86044c205"},
		{"extremis_trooper_02.unity3d","CustomAssetBundle-515c90840a5a9934da99bf0e4f899912"},
		{"extremis_trooper_03.unity3d","CustomAssetBundle-db3fee528a2ca1e4384fbb863df47008"},
		{"fire_giant_01.unity3d","CustomAssetBundle-094e1f2b686774145883d89137eabadb"},
		{"frost_giant_01.unity3d","CustomAssetBundle-83387bf48d06ec343ace3af980430790"},
		{"kingpin_thug_01.unity3d","CustomAssetBundle-c5ae7103d8f6bff45803452fff99fb26"},
		{"kingpin_thug_02.unity3d","CustomAssetBundle-0e49ec809528db64b9a1c4a607a77a5a"},
		{"kingpin_thug_03.unity3d","CustomAssetBundle-d37b822db68ffdb4ca48ae9bcc3afe22"},
		{"lizardling_01.unity3d","CustomAssetBundle-569949c9800b43b4995df193c008a30b"},
		{"lizardling_02.unity3d","CustomAssetBundle-f95772d644c3f614797ebf4f8aa06d2e"},
		{"lizardling_03.unity3d","CustomAssetBundle-6e72ecc03ac0fcb4193a4c247b1f1c8e"},
		{"loki_mimic_01.unity3d","CustomAssetBundle-ebb12d8e57dc3a240a176220b37ae9d9"},
		{"loki_mimic_02.unity3d","CustomAssetBundle-c8deec3cef5c059489db698abd533ee2"},
		{"loki_mimic_03.unity3d","CustomAssetBundle-ed6df6fcae27c38478e593dca00aa0f4"},
		{"modok_mini_01.unity3d","CustomAssetBundle-ffa6099c40c29b743a5dc48bee060612"},
		{"monkey_king_rabbit_minion_01.unity3d","CustomAssetBundle-6e08e4c049ccf9547bcfd8981249d5a7"},
		{"mr_placeholder_01.unity3d","CustomAssetBundle-4900de06f5032fd4ebcb658335f140da"},
		{"npc_female_01.unity3d","CustomAssetBundle-71b918d2bf037c14688e8d5f865c24ad"},
		{"npc_male_01.unity3d","CustomAssetBundle-73aeb940ae81b18498106235d72e0d03"},
		{"npc_robber_01.unity3d","CustomAssetBundle-90fc715989008804baae763d8d59c329"},
		{"ock_bot_01.unity3d","CustomAssetBundle-7b79c7b4a2dca034796f328cd34f4cdd"},
		{"ock_bot_02.unity3d","CustomAssetBundle-db73615153957764d965d5973261ebdf"},
		{"ock_bot_03.unity3d","CustomAssetBundle-94e24a7edbd182f48af81002f7162925"},
		{"pigeon_minion_02.unity3d","CustomAssetBundle-cd81cebee78c0f24d9e3cc4f51f45ad5"},
		{"pigeon_minion_03.unity3d","CustomAssetBundle-462126054d2873d4ba0350708df8dee1"},
		{"pigeon_minion_04.unity3d","CustomAssetBundle-e9b7d164516cfe64b981677b0783d5f5"},
		{"pigeon_minion_05.unity3d","CustomAssetBundle-77878583e91e5ae46ab805193a430645"},
		{"rabbit_minion_01.unity3d","CustomAssetBundle-9aba69878610d3745a0572529a60785f"},
		{"rob_minion_01.unity3d","CustomAssetBundle-2c448b247b28c714eaf4ad628485a782"},
		{"rock_troll_01.unity3d","CustomAssetBundle-f47a57c72214f47458a99ee779c597fb"},
		{"rock_troll_02.unity3d","CustomAssetBundle-3131f0b034c6b4c41a40f88d9955ab98"},
		{"sentinel_01.unity3d","CustomAssetBundle-c6d722b0c778e354b9e358296c5d8a55"},
		{"sentinel_02.unity3d","CustomAssetBundle-8d597f51a425e094e968ff7bb776263f"},
		{"sentinel_03.unity3d","CustomAssetBundle-9a3801ebcfceb6046b5e7b8859684f6b"},
		{"sentinel_04.unity3d","CustomAssetBundle-2829082059dd2e54788e5bbfe44dd805"},
		{"shield_agent_01.unity3d","CustomAssetBundle-0b87449cc3734544ea4a7a418be273c1"},
		{"shield_agent_02.unity3d","CustomAssetBundle-7c31e0c77e9517d4da82dc903275f416"},
		{"shield_agent_03.unity3d","CustomAssetBundle-b720b55cee6ce01458f59a8a30e2a393"},
		{"shield_agent_04.unity3d","CustomAssetBundle-53fdd217dadb5cc4897fbdc03b97669c"},
		{"skrull_01.unity3d","CustomAssetBundle-11f851c648229c14b85e6c58cd891532"},
		{"skrull_02.unity3d","CustomAssetBundle-e31b788f170eec0498f314d8416831a7"},
		{"skrull_04.unity3d","CustomAssetBundle-3470c990d286b634aaae3e7fc067c72a"},
		{"skrull_popcorn_minion_01.unity3d","CustomAssetBundle-c17bee0da7ea59d4c9a54813a2175161"},
		{"surtur_popcorn_minion_01.unity3d","CustomAssetBundle-20f36a82a50583c47be99591f4192c93"},
		{"surtur_popcorn_minion_02.unity3d","CustomAssetBundle-34b33cc76fe3bd7449c53676433d474f"},
		{"tactical_force_01.unity3d","CustomAssetBundle-73b2cf002b20c8d4b992c593df5bb063"},
		{"ultron_popcorn_minion_01.unity3d","CustomAssetBundle-3ed22276e9c2edb4fbdc15743657736d"},
		{"ultron_popcorn_minion_02.unity3d","CustomAssetBundle-a7544961f2d35e440b4053d7b9af014f"},
		{"ultron_popcorn_minion_03.unity3d","CustomAssetBundle-f1d73da7ac0fb214ebda8c179d1b1889"},
		{"ultron_popcorn_minion_04.unity3d","CustomAssetBundle-93771397917c668449ea8ded098cf94e"},
		{"ultron_popcorn_minion_05.unity3d","CustomAssetBundle-4adc34d34f1ab7941aedf5728532d440"},
		{"ultron_popcorn_minion_06.unity3d","CustomAssetBundle-4467b5604a370264f94dc7f2dba77824"},
		{"ultron_popcorn_minion_07.unity3d","CustomAssetBundle-3433d094456e47e4c9c2e7f36925221c"},
		{"ymir_popcorn_minion_01.unity3d","CustomAssetBundle-d2093972322f2b04daf50dbb4fd03390"},
		{"Hecks_Kitchen_tile_Background_01_Lightning.unity3d","CustomAssetBundle-2bf0ea2497b8b0a4d8ad50025c8dc4b7"},
		{"Hecks_Kitchen_Tile_CorporateBldg_B01.unity3d","CustomAssetBundle-49f8983000a57d043b2c62340301796b"},
		{"office_bld_A01.unity3d","CustomAssetBundle-4ee8b058413adf341bb70fde40e8c7a4"},
		{"pipe_1.unity3d","CustomAssetBundle-02778d563d831034880e03c6fbb38c12"},
		{"pipe_2.unity3d","CustomAssetBundle-1d0ca6d061fcedd43abf357b90a1c55e"},
		{"Rooftop_Tiles_Background_01.unity3d","CustomAssetBundle-0c63465b511563b4f9ed86e0beeb2fad"},
		{"Rooftop_Tiles_Background_02.unity3d","CustomAssetBundle-d4e64353873a5ec48bcace25ca40805c"},
		{"Rooftop_Tiles_Background_03.unity3d","CustomAssetBundle-f2cd2f26c7d4ac049bb5dfdffb2dd2d5"},
		{"Rooftop_Tiles_Background_04.unity3d","CustomAssetBundle-49a662f15fcc3e14f9c504e2b706ffcf"},
		{"Rooftops_Buildingtop.unity3d","CustomAssetBundle-eb75158597b4b8f4f98873d2015cae7f"},
		{"Rooftops_Gate_Crane_01.unity3d","CustomAssetBundle-cc4312505f8fff541918f67f4f2f2750"},
		{"Rooftops_Gate_Crane_02.unity3d","CustomAssetBundle-96d36a8e5c1ea304c9af00984adfa7fd"},
		{"Rooftops_Misc.unity3d","CustomAssetBundle-b63adc330fd36a04c8229eb255a4e737"},
		{"Rooftops_Tile_Boss_Arena.unity3d","CustomAssetBundle-bac19b7340b28ba4693ca4cafec53669"},
		{"Rooftops_Tile_Construction_01.unity3d","CustomAssetBundle-b6905b4fcca103e4baadd85af0ac4c7a"},
		{"Rooftops_Tile_Filler.unity3d","CustomAssetBundle-e7c723921db966442a32033f9e685db0"},
		{"Rooftops_Tile_Generic.unity3d","CustomAssetBundle-38188eaea4c397e4da4b7a3e7dd20396"},
		{"Rooftops_Tile_Nunchucks_01.unity3d","CustomAssetBundle-8bb39551a48bc0c4e915c10b62196a2f"},
		{"Rooftops_Tile_Tiered_01.unity3d","CustomAssetBundle-f483d182fd20c1444b2d031bbd2443ca"},
		{"Rooftops_Tile_Tiered_02.unity3d","CustomAssetBundle-c1a82d7e18216e24b88a30bfc5c86878"},
		{"Rooftops_Tile_TieredDisc_01.unity3d","CustomAssetBundle-dd0bc6ce4efebdb46b698cb7a8859d79"},
		{"Rooftops_Tile_TieredDisc_02.unity3d","CustomAssetBundle-2ba1eed7f6191134b8815a534cb10dc8"},
		{"Rooftops_Tile_TShape_01.unity3d","CustomAssetBundle-064bdbb2a0dfabd4ba8e8a9fb081f479"},
		{"Rooftops_Tiles_Background.unity3d","CustomAssetBundle-0c63465b511563b4f9ed86e0beeb2fad"},
		{"Rooftops_Tiles_Skydome_01.unity3d","CustomAssetBundle-64590c1c9ec9590409b57c6d19952df7"},
		{"Sewer_Boss_Arena.unity3d","CustomAssetBundle-159782ec30f8d3241a1dd0914b7c62cc"},
		{"Sewer_Esses.unity3d","CustomAssetBundle-61de00dba68fc54438401191a65932bf"},
		{"Sewer_Round.unity3d","CustomAssetBundle-a4ea324cc51d9f94f8b379ffb31c0f89"},
		{"sewer_tile_drop.unity3d","CustomAssetBundle-ce74ce58705873c40917daf041099864"},
		{"Sewer_Tile_Drop_01.unity3d","CustomAssetBundle-ce74ce58705873c40917daf041099864"},
		{"sewer_tile_start_02.unity3d","CustomAssetBundle-ba535cd1fa62fb0469b0677630e35f3c"},
		{"Sewer_Turn.unity3d","CustomAssetBundle-b713f78c71f28464f9ffb6cc7309ec35"},
		{"SHC_Streets_Misc.unity3d","CustomAssetBundle-04956947e1de75c459b9b520c5ad36a8"},
		{"SHC_Streets_Museum_CARDGAME.unity3d","CustomAssetBundle-58a3de2903abf194d84d1f7be4f1b3ae"},
		{"SHC_Streets_Tile_Arena.unity3d","CustomAssetBundle-c74d7e24b62d5974592947c25fc70c96"},
		{"SHC_Streets_Tile_Corner_01.unity3d","CustomAssetBundle-c4f2eeb57396f07409c6752a2915af33"},
		{"SHC_Streets_Tile_crossroads.unity3d","CustomAssetBundle-a17993f0604652d4ab34b7d1dbe9516f"},
		{"SHC_Streets_Tile_curve.unity3d","CustomAssetBundle-8f2b96061c62562479ed1c6f34edca31"},
		{"SHC_Streets_Tile_End_02.unity3d","CustomAssetBundle-d07f808922d14d740a4954f8214f145b"},
		{"SHC_Streets_Tile_Gardenpath_01.unity3d","CustomAssetBundle-bdece14294973a444ac553a9c92a6752"},
		{"SHC_Streets_Tile_Overpass_01.unity3d","CustomAssetBundle-f4bca3e9376b9814cb435afdd54da6af"},
		{"SHC_Streets_Tile_park.unity3d","CustomAssetBundle-0db3cf1237f6d7d4681ba0371732046e"},
		{"SHC_Streets_Tile_roundabout.unity3d","CustomAssetBundle-19c0f82f81e2be643a7f6972cce71ecc"},
		{"SHC_Streets_Tile_start.unity3d","CustomAssetBundle-989715a64828ae34ab6ceee3c2bd6293"},
		{"SkrullShip_Boss_Arena.unity3d","CustomAssetBundle-9b1de64908abdff41b837546aa893651"},
		{"SkrullShip_Misc.unity3d","CustomAssetBundle-cce41d226f850a547a1854ed19eda79f"},
		{"SkrullShip_Tile_Background_01.unity3d","CustomAssetBundle-5362d8b170ad5ab4c94834e7cd27dfda"},
		{"SkrullShip_Tile_Background_03.unity3d","CustomAssetBundle-a8e1a0e209cb2eb438c768f59df338cc"},
		{"SkrullShip_Tile_Bay_01.unity3d","CustomAssetBundle-5d9a30111df27ba47b9e4934c54b8b0e"},
		{"SkrullShip_Tile_Bay_02.unity3d","CustomAssetBundle-39886a1ca99be8a45a9991f6342e3b4b"},
		{"SkrullShip_Tile_Bay_03.unity3d","CustomAssetBundle-5b7a856aa429e9b45afeebf066926723"},
		{"SkrullShip_Tile_Bay_04.unity3d","CustomAssetBundle-764877ed57d33ca43877183c8ca0270f"},
		{"SkrullShip_Tile_Bay_05.unity3d","CustomAssetBundle-a6f481279f7ddc840bc98c44d3b86260"},
		{"SkrullShip_Tile_Bay_06.unity3d","CustomAssetBundle-29a0849c33a30f349b7c11ec6011bcb6"},
		{"SkrullShip_Tile_Narrow_01.unity3d","CustomAssetBundle-019286dbd9e75cf418e882d01f71880a"},
		{"Skrullship_Tile_Skydome_02.unity3d","CustomAssetBundle-2be2ec0db55ed00468b65b3632f47223"},
		{"skybox_bldg2.unity3d","CustomAssetBundle-2bfc486f225d6b44d809c569aaa8ef41"},
		{"skyscraper01.unity3d","CustomAssetBundle-998fb5d007467f541b3513dacc764018"},
		{"Street_Corner_Gate_A.unity3d","CustomAssetBundle-5da7534766ae42b45bc339ea5629defc"},
		{"Street_Corner_Gate_Pipe_A.unity3d","CustomAssetBundle-9c9ae4c7318522344a7fe13209f267b4"},
		{"Street_Corner_Inside_A.unity3d","CustomAssetBundle-35d03c3098ffe8e439eb02827975a92d"},
		{"Street_Corner_Outside_A.unity3d","CustomAssetBundle-4841aefe94a47a046bfefa423f53d4cc"},
		{"Street_End_A.unity3d","CustomAssetBundle-94e2c6cce7b85e141a6641ab2095cc99"},
		{"Street_Gate_A.unity3d","CustomAssetBundle-386cad3be8e6c7a488e82921679ec0e7"},
		{"Street_Seg_A.unity3d","CustomAssetBundle-878d49dc5cc66e0439e1cdac4cd4cb2e"},
		{"Street_Seg_B2.unity3d","CustomAssetBundle-d6fca1c31137c6949bad9969ac59ef39"},
		{"Street_Start_A.unity3d","CustomAssetBundle-ea8c0b1d5886c5a4f816b0c0f88b1d25"},
		{"street_tile_cracks.unity3d","CustomAssetBundle-3b2a924445f35ae44aca0a3231f64d96"},
		{"Street_WallOff_01.unity3d","CustomAssetBundle-43eefbf581a7ba545a0dec6007dabd2a"},
		{"Street_WallOff_02.unity3d","CustomAssetBundle-99be2a1655bef4b48b024aec9eb25435"},
		{"streets_arena.unity3d","CustomAssetBundle-80b34a1b05f381e438bce1ded40efdeb"},
		{"streets_tile_junkyard_01.unity3d","CustomAssetBundle-7ced9963984ae924c93757a89c618859"},
		{"Subway_Tile_Arena.unity3d","CustomAssetBundle-8669a5c3dee9920488cf7bf33e873be1"},
		{"Subway_Tile_Bridge_01.unity3d","CustomAssetBundle-b5907413b2d53af4b9fd373482a50f0e"},
		{"Subway_Tile_Esses_01.unity3d","CustomAssetBundle-de4e05d109ab5d847ab3bb741556c7c5"},
		{"Subway_Tile_Overpass_01.unity3d","CustomAssetBundle-bf95294d6a38e5843b2df4b5de5649f7"},
		{"Subway_Tile_Rails_01.unity3d","CustomAssetBundle-2e5d432f753717546a443d418538fc3e"},
		{"Subway_Tile_Rails_02.unity3d","CustomAssetBundle-158464ee09ee5364f82e3c69111d4a60"},
		{"subway_tile_split.unity3d","CustomAssetBundle-44b7deb62bb4a704098844c93c1023d9"},
		{"Subway_Tile_Start_01.unity3d","CustomAssetBundle-4c5a9f6e45d98a742bd332c715424764"},
		{"Subway_Tile_Start_02.unity3d","CustomAssetBundle-1b943c987e1d4dd468dd0a5029390aa0"},
		{"Subway_Tile_Turn_01.unity3d","CustomAssetBundle-ec14aabd2846a744590d7292b8a7408a"},
		{"F4_Spacestation_BossArena.unity3d","CustomAssetBundle-3ff9fd320b145df4aa4811bdf878385d"},
		{"F4_Spacestation_BossArena_Dome.unity3d","CustomAssetBundle-eacad49693627d64692ab55c7fd73e92"},
		{"F4_Spacestation_planet.unity3d","CustomAssetBundle-e84227ce1ed5e344e87a7c4cecdf56bf"},
		{"F4_Spacestation_Spire_01.unity3d","CustomAssetBundle-d6a06d3d1e602444fbf96e28aad3d911"},
		{"F4_Spacestation_Tile_Arc_01.unity3d","CustomAssetBundle-765047a53e59d2645bcbb4422281b19a"},
		{"F4_Spacestation_Tile_Arc_02.unity3d","CustomAssetBundle-7f11e709dd1b06041970f965205190d7"},
		{"F4_Spacestation_Tile_Arc_03.unity3d","CustomAssetBundle-04780132ca56cc74e8667c4a3168edea"},
		{"F4_Spacestation_Tile_Arm.unity3d","CustomAssetBundle-ff052af18161aeb49a85812bcc714337"},
		{"F4_Spacestation_Tile_Arm_02.unity3d","CustomAssetBundle-a97ae2139bdcd32479d4333533ad5755"},
		{"F4_Spacestation_Tile_Background.unity3d","CustomAssetBundle-80bf01ea21feca344803eacfb2a31d25"},
		{"F4_Spacestation_Tile_Hub.unity3d","CustomAssetBundle-e89cfdf21c34ca04ab370944740c604a"},
		{"Factory_Boss_Arena.unity3d","CustomAssetBundle-287dfc9c5a2623e4aa0749c9d9f55380"},
		{"Factory_Catwalk.unity3d","CustomAssetBundle-28b0fe4a20dac9e4689ce48465da2870"},
		{"Factory_Catwalk_Lava.unity3d","CustomAssetBundle-af5b7667c24e09f4c9658e246c7f622b"},
		{"Factory_ConveyorRoom_01.unity3d","CustomAssetBundle-82e647d6271901b4b82eb0262f080fee"},
		{"Factory_ConveyorRoom_02.unity3d","CustomAssetBundle-e50d2f382d85a7a4db6f6e89fa38101c"},
		{"Factory_ConveyorRoom_03.unity3d","CustomAssetBundle-d3f2d0ceb2d816740bc334f2128dabd4"},
		{"Factory_HallBend_01.unity3d","CustomAssetBundle-2cd524634affb0f488952266ee7f3f22"},
		{"Factory_HallBend_Truncated_01.unity3d","CustomAssetBundle-d297e6c4d29622b45a883efafffc22d7"},
		{"Factory_HallStraight_01.unity3d","CustomAssetBundle-baccd2af564330d458e737ddc908165a"},
		{"Factory_Misc.unity3d","CustomAssetBundle-bd23ab97b5e1ff14a8c061c89f974ab5"},
		{"Factory_Molten_Lava.unity3d","CustomAssetBundle-41c6c2024b70f5d48b981cd21f122527"},
		{"factory_Room3.unity3d","CustomAssetBundle-44d257f2b51a08c498ef33a9c20712c5"},
		{"factory_tile_elevated.unity3d","CustomAssetBundle-4f2b3e0d0f3662045b307cc2b533b321"},
		{"factory_tile_start.unity3d","CustomAssetBundle-3b175750d99286d4da269cf92173a25a"},
		{"FactoryStart.unity3d","CustomAssetBundle-d863a34f8da22194fbed64ea26e8e659"},
		{"Generator.unity3d","CustomAssetBundle-fb33b21507ef57b48908a3516aad4373"},
		{"Asgard_Fire_Tile_Interior_Corner.unity3d","CustomAssetBundle-e49c49f0d596b0646a4e073349dbe7f0"},
		{"Asgard_Fire_Tile_Pathway_01.unity3d","CustomAssetBundle-b841b8c19b44bea4180a60cd3eda6543"},
		{"Asgard_Fire_Tile_Start.unity3d","CustomAssetBundle-c22fbdd01017c03478f530868c8133bd"},
		{"Asgard_Frost_Arena.unity3d","CustomAssetBundle-75afe90c42960db43be5e784b12319c0"},
		{"Asgard_Frost_Background_01.unity3d","CustomAssetBundle-871c0402aed2b5f4a9de858a2c7aa8ef"},
		{"Asgard_Frost_Background_02.unity3d","CustomAssetBundle-281731a077f3dd346aba096c951b0217"},
		{"Asgard_Frost_Tile_Cliffhanger_01.unity3d","CustomAssetBundle-b1ffb2e98f362204ebb1d04a8439b2fe"},
		{"Asgard_Frost_Tile_FocalPoint_01.unity3d","CustomAssetBundle-1643e2093cf69974e8fba8bb2b1c65ca"},
		{"Asgard_Frost_Tile_Hole_01.unity3d","CustomAssetBundle-8528b5ffc16baae46aa23ed7790f92e3"},
		{"Asgard_Frost_Tile_RockBridge_01.unity3d","CustomAssetBundle-383c55dfea3dbe946891028910850168"},
		{"Asgard_Frost_Tile_Turn_01.unity3d","CustomAssetBundle-4edad78199e329e468901f74db8d2bb1"},
		{"Asgard_Frost_Tile_Vista_01.unity3d","CustomAssetBundle-dfabb4d8cb743f8498c4fe10678e885a"},
		{"Asgard_Frost_Tile_Weave_01.unity3d","CustomAssetBundle-cfdf254cd4fc1914bbb6db67e623f510"},
		{"Asgard_Proper_Alleyway_01.unity3d","CustomAssetBundle-cb51ed5bbdea89f42a70e0ae3a313095"},
		{"Asgard_Proper_Arena.unity3d","CustomAssetBundle-fdc699b79d8e08f4482bae1f2d1674ce"},
		{"Asgard_Proper_Arena_Pillars_02.unity3d","CustomAssetBundle-2f3f2ee449ce4484ca738d787ea3755d"},
		{"Asgard_Proper_Arena_Pillars_02_Animated.unity3d","CustomAssetBundle-f5694321c35623a4c8ca354da3a1b19f"},
		{"Asgard_M102S_01_scenario.unity3d","CustomAssetBundle-6353e0364c106a140b57260fbed946ed"},
		{"Brawler_globals.unity3d","CustomAssetBundle-a3fc829e5ca7ef14a92477a13c93c1a0"},
		{"Carbon_001_S03M_BOSS_scenario.unity3d","CustomAssetBundle-5bdba113476849a45b5ebe55f257200c"},
		{"Factory_M006_S03M_BOSS_scenario.unity3d","CustomAssetBundle-20b008d1f78eb774585b75a9b9b577cd"},
		{"Factory_M020_S02M_BOSS_scenario.unity3d","CustomAssetBundle-a38ec4e7283bba440af195515c2867aa"},
		{"Factory_M025_S03M_BOSS_scenario.unity3d","CustomAssetBundle-6aebe4e50a63857449c67e99e72e2710"},
		{"Fluorine_001_S02M_BOSS_scenario.unity3d","CustomAssetBundle-ca97f49cbea1b3242ab05ad852c76280"},
		{"Fluorine_002_S02M_BOSS_scenario.unity3d","CustomAssetBundle-8721c5330d393e64dbda812c0818a467"},
		{"Helicarrier_M031_S02_BOSS_scenario.unity3d","CustomAssetBundle-ddb03f7f5a130e64aa12880156d860d5"},
		{"Helium_001_S03M_BOSS_scenario.unity3d","CustomAssetBundle-fbb9a169116e08647ad52a1648bf46b0"},
		{"Helium_002_S03M_BOSS_scenario.unity3d","CustomAssetBundle-4781b720e350e1840a76e66cc4c639a4"},
		{"Lithium_002_S01_scenario.unity3d","CustomAssetBundle-7dfdf18a60d23214f9d2612350a21a9e"},
		{"Lithium_002_S02_scenario.unity3d","CustomAssetBundle-653f25b8cdcf54f47a27ffdd4a9ad74e"},
		{"Lithium_002_S03_BOSS_scenario.unity3d","CustomAssetBundle-6c384f9e3618cef4eaf277ca69be1086"},
		{"M00X_Intro_Mission_scenario.unity3d","CustomAssetBundle-6db1d14de24504144b54f6eb96342704"},
		{"Neon_001_S02M_BOSS_scenario.unity3d","CustomAssetBundle-83387bf48d06ec343ace3af980430790"},
		{"Nitrogen_002_S03M_BOSS_scenario.unity3d","CustomAssetBundle-500b5d3d9f7a78e408656f66f3bd7556"},
		{"SHC_ROOFTOPS_M024_S03M_BOSS_scenario.unity3d","CustomAssetBundle-2cdd086e2b3b6f440ae7834eee7f252f"},
		{"SHC_ROOFTOPS_M09_S03M_BOSS_scenario.unity3d","CustomAssetBundle-8829d3ad7bf3a1e4a83c7aae804ccdf8"},
		{"SHC_Streets_M031_S02_scenario.unity3d","CustomAssetBundle-673506a1a324b2043b73a27d9a3486f4"},
		{"SHC_Subway_M031_S01_scenario.unity3d","CustomAssetBundle-e238481fced52c94c939c18d69995c64"},
		{"Streets_M01_S03M_BOSS_scenario.unity3d","CustomAssetBundle-d140ae94862d93f4ea2ad1aa41d6880e"},
		{"Streets_M101S_01_scenario.unity3d","CustomAssetBundle-12c14d51b993b9b4a81b9867b1a85116"},
		{"Asgard_Proper_BackgroundBuilding_02.unity3d","CustomAssetBundle-9d47c549da0c61a4e8d3d9a54833ff76"},
		{"Asgard_Proper_BackgroundBuilding_03.unity3d","CustomAssetBundle-3de5680df8b3526469e3b6d2106afad0"},
		{"Asgard_Proper_BackgroundBuilding_05.unity3d","CustomAssetBundle-448108813a699e141b15255e063ff1d8"},
		{"Asgard_Proper_Blocker.unity3d","CustomAssetBundle-7e8de493bed47264099551144f85f3cc"},
		{"Asgard_Proper_End_Cap.unity3d","CustomAssetBundle-81d4ada3cb21d7646a8c6f7d66f64b6f"},
		{"Asgard_Proper_Skydome_02.unity3d","CustomAssetBundle-fe28d68b9a1cb514b9bea104c0ca08ef"},
		{"Asgard_Proper_Spire.unity3d","CustomAssetBundle-130408c580e63744380615ae0d7d273d"},
		{"Asgard_Proper_Tile_End_01.unity3d","CustomAssetBundle-7e89c3cac67711846a9e34a5a26397ed"},
		{"Asgard_Proper_Tile_Interior_Corner.unity3d","CustomAssetBundle-f6bc9ea95cc89df468442b9689ef3702"},
		{"Asgard_Proper_Tile_LJoint_01.unity3d","CustomAssetBundle-be0b1647cd81c434d9ad734f1f304ef6"},
		{"Asgard_Proper_Tile_Start_01.unity3d","CustomAssetBundle-d2f2f9e1bc7df17479fde9ebac93ecfb"},
		{"Asgard_Proper_Tile_Statue_01.unity3d","CustomAssetBundle-958875666af313044b981eaab48107ac"},
		{"Asgard_Proper_Tile_Wall_01.unity3d","CustomAssetBundle-e812cba12947e4045b11b7447ec7ea8a"},
		{"Asgard_Proper_Tile_Wall_02.unity3d","CustomAssetBundle-fc5e32ff4677027419f03954e65473f3"},
		{"Asgard_Proper_WallExtend_01.unity3d","CustomAssetBundle-1636e494ac19d0f4084d32503d502255"},
		{"Asteroid_01.unity3d","CustomAssetBundle-11417051c1f8ed64cb60cdb60db2c3a4"},
		{"Asteroid_02.unity3d","CustomAssetBundle-436017e157a1ae94f94bfeb586eba930"},
		{"Asteroid_04.unity3d","CustomAssetBundle-710a7dfc9bdff844db187f01fb2a29e1"},
		{"Asteroid_05.unity3d","CustomAssetBundle-91c81f1e86852354390f2d9822f98e19"},
		{"asteroid_belt.unity3d","CustomAssetBundle-9d11dc311a8c19343844707438e81338"},
		{"Asteroid_tile_04.unity3d","CustomAssetBundle-710a7dfc9bdff844db187f01fb2a29e1"},
		{"Asteroid03.unity3d","CustomAssetBundle-8c61bab187c4ed442af0df37406e5489"},
		{"AsteroidM_Background_01.unity3d","CustomAssetBundle-250df188a64bc5345839cba4da1e17a9"},
		{"AsteroidM_Background_02.unity3d","CustomAssetBundle-5407d7550f1495a4db757d4990e78047"},
		{"AsteroidM_Background_03.unity3d","CustomAssetBundle-2ca6158f03387e14da94aff18caf725c"},
		{"asteroidM_barricade.unity3d","CustomAssetBundle-a2c9b08534a553245a8d654c24e081ff"},
		{"AsteroidM_blocking_1.unity3d","CustomAssetBundle-a4c98ad023efdae4dbe94f21725ba7e4"},
		{"AsteroidM_Boss_Arena.unity3d","CustomAssetBundle-ca1bd48605bd9f74ca7b29c6f551ac19"},
		{"AsteroidM_Skydome.unity3d","CustomAssetBundle-5406196fdda45c8449c2977a10917988"},
		{"AsteroidM_Tile_01.unity3d","CustomAssetBundle-11417051c1f8ed64cb60cdb60db2c3a4"},
		{"AsteroidM_Tile_02.unity3d","CustomAssetBundle-436017e157a1ae94f94bfeb586eba930"},
		{"AsteroidM_Tile_03.unity3d","CustomAssetBundle-8c61bab187c4ed442af0df37406e5489"},
		{"AsteroidM_Tile_04.unity3d","CustomAssetBundle-710a7dfc9bdff844db187f01fb2a29e1"},
		{"AsteroidM_Tile_05.unity3d","CustomAssetBundle-91c81f1e86852354390f2d9822f98e19"},
		{"AsteroidM_Tile_06.unity3d","CustomAssetBundle-1e119b3b23b2e9543afe8f7ff53190fd"},
		{"AsteroidM_Tile_07.unity3d","CustomAssetBundle-fe95425b2b3dc4a409e15258486fc79b"},
		{"Astroid_Background_01.unity3d","CustomAssetBundle-250df188a64bc5345839cba4da1e17a9"},
		{"Astroid_Background_02.unity3d","CustomAssetBundle-5407d7550f1495a4db757d4990e78047"},
		{"Astroid_Background_03.unity3d","CustomAssetBundle-2ca6158f03387e14da94aff18caf725c"},
		{"AvengersBaxter.unity3d","CustomAssetBundle-b0e803cae71e8c74e9314dfcdfc6d857"},
		{"ConveyorRoomWall_01.unity3d","CustomAssetBundle-07c36088c893d9e41829030a13275a86"},
		{"corporateBldg_B01.unity3d","CustomAssetBundle-cececcb6f2991ee40b8d2087d7255784"},
		{"Dark_Dimension_Tile_Diagonal.unity3d","CustomAssetBundle-b8b8c717863cd9848b935371798a66e2"},
		{"Dark_Dimension_Tile_End.unity3d","CustomAssetBundle-105af935a5868624a808629fc3a153dc"},
		{"Dark_Dimension_Tile_Exterior_Corner.unity3d","CustomAssetBundle-913e8467ed061964fbf00c29941ad956"},
		{"Dark_Dimension_Tile_Interior_Corner.unity3d","CustomAssetBundle-d634839f3fefd84458331c2f92874ab3"},
		{"Dark_Dimension_Tile_Pathway_01.unity3d","CustomAssetBundle-a04da1d1dabe0be4b917df0645aa65c2"},
		{"Dark_Dimension_Tile_Platform_01.unity3d","CustomAssetBundle-5ae6a89576ea54143afd46d0e653e168"},
		{"Dark_Dimension_Tile_Start.unity3d","CustomAssetBundle-729e63525d6d1c94d995647b9d0f25bc"},
		{"Dark_Dimension_Tile_Throne_Room.unity3d","CustomAssetBundle-711dc81ccecc0904ca29950572d1a176"},
		{"Asgard.unity3d","CustomAssetBundle-a6856d78bf5dd08489a5e636ffda9940"},
		{"Asgard_scenario.unity3d","CustomAssetBundle-73eecfce0c9ead743ac144e0278410e9"},
		{"baxter.unity3d","CustomAssetBundle-2bfc486f225d6b44d809c569aaa8ef41"},
		{"baxter_scenario.unity3d","CustomAssetBundle-16bcc258d8a6e9b4b85ee960fb8f6658"},
		//{"daily_bugle2.unity3d","CustomAssetBundle-3af1212b9daccf2469a4d8a180a82a09"},
		{"Villainville.unity3d","CustomAssetBundle-5fce7ce5c74821945865575b84b109b0"},
		{"Villainville_scenario.unity3d","CustomAssetBundle-e358c643420e10b41b41c445fc26b711"},
		{"green_goblin_fx.unity3d","CustomAssetBundle-6833f7474c902b04f82e36f22f030a1a"},
		{"war_machine_fx.unity3d","CustomAssetBundle-ced581f372e52c9459bb1688e3285b06"},
		{"hulk_fx.unity3d","CustomAssetBundle-566221f223777c048994918183e56946"},
		{"spiderman_fx.unity3d","CustomAssetBundle-c2ff432ae7cf55e499f9c2b294f9b58c"}


	};

	static public CabToU3d instance; //the instance of our class that will do the work

	 void Awake(){ //called when an instance awakes in the game
		instance = this; //set our static reference to our newly initialized instance
	}
	// Use this for initialization
	void Start () {
		//DoCoroutine();  // CSP - testing bundle load()
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public string getNewestDir(string path) {

		// string pattern = "*.";

		// DirectoryInfo dirInfo = new DirectoryInfo(path);
		// dirInfo.Refresh();

		// DirectoryInfo file = (from f in dirInfo.GetDirectories(pattern) 
		// 			orderby f.LastWriteTime descending 
		// 			select f).First();

		// CspUtils.DebugLog("file.FullName= " + file.FullName);

		// return file.FullName;

		return null;
	}

	public string DirSearch(List<string> files, string startDirectory)
	{
		try
		{
			foreach (string file in Directory.GetFiles(startDirectory, "*.*"))
			{
				string extension = Path.GetExtension(file);

				if (extension != null)
				{
					files.Add(file);
					if (file.Contains(stubCabName)) {
						CspUtils.DebugLog("file= " + file);
						return file;
					}
				}
			}

			foreach (string directory in Directory.GetDirectories(startDirectory))
			{
				string retval = DirSearch(files, directory);
				if (retval != null) {
					return retval;
				}
			}
		}
		catch (System.Exception e)
		{
			CspUtils.DebugLog(e.Message);
		}

		return null;

	}

	IEnumerator fix(DictionaryEntry cm) {
		WWW www = null;
		AssetBundle myBundle = null;

			CspUtils.DebugLog("downloading: " + cm.Key);

			www = WWW.LoadFromCacheOrDownload ("http://192.168.235.128/cab-stubs/" + cm.Key, 5);
			yield return www;
			if (www.error != null)
			{
				CspUtils.DebugLog (www.error);
				yield break;
			}


			myBundle = www.assetBundle;
			//var asset = myLoadedAssetBundle.mainAsset;
			if (myBundle == null)
			{
				CspUtils.DebugLog("myBundle is null!");
				yield break;
			}
			else {
				CspUtils.DebugLog("myBundle is loaded!");

				
				UnityEngine.Object obj = myBundle.Load("VO_Hero_she_hulk_red_Lift_audio");
				if (obj != null) {
					CspUtils.DebugLog("asset name using Load():" + obj.name);
				}
					

				UnityEngine.Object[] objs = myBundle.LoadAll();
				int i=0;
				foreach (UnityEngine.Object obj2 in objs)
				{	
					CspUtils.DebugLog(i + " loaded asset name:" + obj2.name);
					i++;
					
				}		

				myBundle.Unload(true);	

				// find newest directory....
				string newestDir = getNewestDir(cachePath);
				CspUtils.DebugLog("newestDir= " + newestDir);

				// ..and copy CAB file on top of stub CAB within.
				var list = new List<string>();
				string destFile = DirSearch(list, newestDir);
				File.Delete(destFile);  // delete stub cache file
				destFile = destFile + ".XYZ";  // give a temporary extension to mark as processes....remove all extensions later after all files have been processed.
				File.Copy(cabPath + cm.Value, destFile, true);   // copy our SHSO CAB file in
				
			}
			/////////////////////////////////////////////////////////////////////////////////////////////
	} 

	WaitForSeconds wait = new WaitForSeconds( 1.1f ) ;

	IEnumerator  doCabCacheFix() {


		CspUtils.DebugLog("doCabCacheFix() entered");

		// first thimg to do prob ought to be to nuke existing webplayer cache....to be done...


		foreach(DictionaryEntry cm in cabMap) {
			StartCoroutine( fix(cm) ) ;
			yield return wait ;
		}
    
		

		// //EditorApplication.isPaused = true;
		//yield break;
	}

	IEnumerator  doReadOnly() {
		WWW www = null;
		AssetBundle myBundle = null;

		CspUtils.DebugLog("doReadOnly() entered");

		// first thimg to do prob ought to be to nuke existing webplayer cache....to be done...


		foreach(DictionaryEntry cm in cabMap) {
			//Console.WriteLine("Key: {0}, Value: {1}", de.Key, de.Value);

			www = WWW.LoadFromCacheOrDownload ("http://192.168.235.128/cab-stubs/" + cm.Key, 5);
			yield return www;
			if (www.error != null)
			{
				CspUtils.DebugLog (www.error);
				yield break;
			}


			myBundle = www.assetBundle;
			//var asset = myLoadedAssetBundle.mainAsset;
			if (myBundle == null)
			{
				CspUtils.DebugLog("myBundle is null!");
				yield break;
			}
			else {
				CspUtils.DebugLog("myBundle is loaded!");

				
				UnityEngine.Object obj = myBundle.Load("VO_Hero_she_hulk_red_Lift_audio");
				if (obj != null) {
					CspUtils.DebugLog("asset name using Load():" + obj.name);
				}
					

				UnityEngine.Object[] objs = myBundle.LoadAll();
				int i=0;
				foreach (UnityEngine.Object obj2 in objs)
				{	
					CspUtils.DebugLog(i + " loaded asset name:" + obj2.name);
					i++;
					break;  // uncomment this to only log the first asset in bundle..
				}		

				myBundle.Unload(true);	
				
				
			}
			////////////////////////////////////////////////////////////////////////////////////////////

		}
    


		// //EditorApplication.isPaused = true;
	}

	IEnumerator testRemoteAlreadyLoaded() {
		//WWW www1 = new WWW("http://192.168.235.128/cab-stubs/gameworld_activity_objects.unity3d");
		WWW www1 = WWW.LoadFromCacheOrDownload("http://192.168.235.128/cab-stubs/gameworld_activity_objects.unity3d", 5);

		yield return www1;
		if (www1.error != null)
		{
			CspUtils.DebugLog (www1.error);
			yield break;
		}

		if (www1.assetBundle != null)
		{	
			CspUtils.DebugLog("www1 is not null!!!");

			CspUtils.DebugLog("www1.bytes = " + www1.bytes);			
			//File.WriteAllBytes("csp-test-file.txt", www1.bytes);
		}
		/////////////////////////////////
		WWW www2 = new WWW("http://192.168.235.128/cab-stubs/gameworld_common_objects.unity3d");

		yield return www2;
		if (www2.error != null)
		{
			CspUtils.DebugLog (www2.error);
			yield break;
		}

		if (www2.assetBundle != null)
			CspUtils.DebugLog("www2 is not null!!!");

		// resulting output: The asset bundle 'http://192.168.235.128/cab-stubs/gameworld_common_objects.unity3d' can't be loaded because another asset bundle with the same files are already loaded
	}

	
	IEnumerator testLocalAlreadyLoaded() {
		WWW www1 = new WWW("file:///C:/dev/RetroSquad/Assets/AssetBundles/SocialSpace/gameworld_activity_objects.unity3d");

		yield return www1;
		if (www1.error != null)
		{
			CspUtils.DebugLog (www1.error);
			yield break;
		}

		if (www1.assetBundle != null)
			CspUtils.DebugLog("www1 is not null!!!");

		/////////////////////////////////
		WWW www2 = new WWW("file:///C:/dev/RetroSquad/Assets/AssetBundles/SocialSpace/gameworld_common_objects.unity3d");

		yield return www2;
		if (www2.error != null)
		{
			CspUtils.DebugLog (www2.error);
			yield break;
		}

		if (www2.assetBundle != null)
			CspUtils.DebugLog("www2 is not null!!!");

		// results: both bundles load and are not null
	}


	static public void DoCoroutine(){
		GameObject fe = GameObject.Find("_FrontEndController");
		fe.active = false;
		GameObject ash = GameObject.Find("_AppShell");
		ash.active = false;

		CspUtils.DebugLog("DoCoroutine() entered");

		//instance.StartCoroutine("testLocalAlreadyLoaded");
		//instance.StartCoroutine("testRemoteAlreadyLoaded");
		//instance.StartCoroutine("doCabCacheFix"); //this will launch the coroutine on our instance
		//instance.StartCoroutine("doReadOnly"); //this will launch the coroutine on our instance

	}
}
