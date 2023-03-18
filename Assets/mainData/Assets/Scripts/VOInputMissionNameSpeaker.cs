using System.Collections.Generic;
using UnityEngine;

public class VOInputMissionNameSpeaker : IVOInputResolver
{
	protected static Dictionary<string, object> missionNameSpeakers;

	protected static void InitDictionary()
	{
		if (missionNameSpeakers == null)
		{
			missionNameSpeakers = new Dictionary<string, object>();
			missionNameSpeakers.Add("be_1001_1_RedSkull001", "red_skull_boss");
			missionNameSpeakers.Add("be_1001_1_RedSkull001A", "red_skull_boss");
			missionNameSpeakers.Add("bo_1001_1_Abomination001", "abomination_boss");
			missionNameSpeakers.Add("bo_1002_1_Modok001", "modok_boss");
			missionNameSpeakers.Add("ca_1001_1_Venom001", "venom_boss");
			missionNameSpeakers.Add("ca_1001_1_Venom001A", "venom_boss");
			missionNameSpeakers.Add("fl_1001_1_Bullseye001", "bullseye_boss");
			missionNameSpeakers.Add("fl_1002_1_Kingpin001", "kingpin_boss");
			missionNameSpeakers.Add("he_1001_1_Loki001", "loki_boss");
			missionNameSpeakers.Add("he_1002_1_YmirSurtur001", new string[2]
			{
				"ymir_boss",
				"surtur_boss"
			});
			missionNameSpeakers.Add("he_1003_1_Enchantress001", "enchantress_boss");
			missionNameSpeakers.Add("li_1002_1_Juggernaut001", "juggernaut_boss");
			missionNameSpeakers.Add("m_1001_1_DocOck001", "dr_octopus_boss");
			missionNameSpeakers.Add("m_1002_1_SuperSkrull001", "super_skrull_boss");
			missionNameSpeakers.Add("t_1002_1_SuperSkrull001", "super_skrull_boss");
			missionNameSpeakers.Add("m_1005_1_Magneto001", "magneto_boss");
			missionNameSpeakers.Add("t_1005_1_Magneto001", "magneto_boss");
			missionNameSpeakers.Add("m_1006_1_DrDoom001", "dr_doom_boss");
			missionNameSpeakers.Add("m_1006_1_DrDoom001A", "dr_doom_boss");
			missionNameSpeakers.Add("m_1007_1_FingFangFoom001", "fin_fang_foom_boss");
			missionNameSpeakers.Add("m_1008_1_Ultron001", "ultron_boss");
			missionNameSpeakers.Add("m_1008_1_Ultron001A", "ultron_boss");
			missionNameSpeakers.Add("m_1009_1_GreenGoblin001", "green_goblin_boss");
			missionNameSpeakers.Add("t_1009_1_GreenGoblin001", "green_goblin_boss");
			missionNameSpeakers.Add("m_1011_1_GreenGoblin002", "green_goblin_boss");
			missionNameSpeakers.Add("m_1012_1_SuperSkrull002", "super_skrull_boss");
			missionNameSpeakers.Add("m_1013_1_Mystique001", "mystique_boss");
			missionNameSpeakers.Add("m_1013_1_Mystique001A", "mystique_boss");
			missionNameSpeakers.Add("m_1014_1_Annihilus001", "annihilus_boss");
			missionNameSpeakers.Add("t_1014_1_Annihilus001", "annihilus_boss");
			missionNameSpeakers.Add("m_1015_1_SuperSkrull003", "super_skrull_boss");
			missionNameSpeakers.Add("m_1016_1_DocOck002", "dr_octopus_boss");
			missionNameSpeakers.Add("m_1017_1_Magneto002", "magneto_boss");
			missionNameSpeakers.Add("m_1018_1_FinFangFoom002", "fin_fang_foom_boss");
			missionNameSpeakers.Add("m_1019_1_Venom001", "venom_boss");
			missionNameSpeakers.Add("m_1020_1_MoleMan001", "moleman_boss");
			missionNameSpeakers.Add("m_1024_1_Lizard001", "lizard_boss");
			missionNameSpeakers.Add("m_1025_1_Mysterio001", "spider_man_black");
			missionNameSpeakers.Add("m_1025_1_Mysterio001A", "spider_man_black");
			missionNameSpeakers.Add("m_1026_1_Magneto003", "magneto_boss");
			missionNameSpeakers.Add("m_1026_1_Magneto003A", "magneto_boss");
			missionNameSpeakers.Add("m_100X_1_Sabretooth001", "sabretooth_boss");
			missionNameSpeakers.Add("m_1027_1_Dracula001", "dracula_boss_01");
			missionNameSpeakers.Add("m_1027_1_Dracula001A", "dracula_boss_01");
			missionNameSpeakers.Add("m_1031_1_TitaniumMan001", "titanium_man_boss");
			missionNameSpeakers.Add("m_1031_1_TitaniumMan001A", "titanium_man_boss");
			missionNameSpeakers.Add("m_1032_1_Malekith001", "captain_america_avengers");
			missionNameSpeakers.Add("m_1032_1_Malekith001A", "captain_america_avengers");
			missionNameSpeakers.Add("m_1033_1_ImpossibleMan001", "impossible_man_playable");
			missionNameSpeakers.Add("m_1033_1_ImpossibleMan001A", "impossible_man_playable");
			missionNameSpeakers.Add("m_1034_1_Villains001", "taskmaster_playable");
			missionNameSpeakers.Add("m_1034_1_Villains001A", "taskmaster_playable");
			missionNameSpeakers.Add("m_1035_1_WinterSoldier001", "captain_america_stealth");
			missionNameSpeakers.Add("m_1035_1_WinterSoldier001A", "captain_america_stealth");
			missionNameSpeakers.Add("m_1037_1_Thanos001", "thanos_boss");
			missionNameSpeakers.Add("m_1037_1_Thanos001A", "thanos_boss");
		}
	}

	public void SetVOParams(string[] parameters)
	{
	}

	public string ResolveVOInput(GameObject emitter, IEnumerable<string> previousInputs)
	{
		InitDictionary();
		foreach (string previousInput in previousInputs)
		{
			object value;
			if (missionNameSpeakers.TryGetValue(previousInput, out value))
			{
				if (value is string)
				{
					return value as string;
				}
				if (!(value is string[]))
				{
					break;
				}
				string[] array = value as string[];
				return array[Random.Range(0, array.Length)];
			}
		}
		return string.Empty;
	}
}
