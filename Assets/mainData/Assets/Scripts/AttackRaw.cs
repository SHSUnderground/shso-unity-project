using System.Xml.Serialization;

[XmlType(AnonymousType = true)]
[XmlRoot(ElementName = "attack", Namespace = "", IsNullable = false)]
public class AttackRaw
{
	[XmlElement(ElementName = "name")]
	public string name;

	[XmlElement(ElementName = "chance")]
	public float chance;

	[XmlElement(ElementName = "bonus_chance_per_extra_target")]
	public string bonus_chance_per_extra_target;

	[XmlElement(ElementName = "extra_target_maximum_distance")]
	public string extra_target_maximum_distance;

	[XmlElement(ElementName = "extra_target_measured_from_attacker")]
	public string extra_target_measured_from_attacker;

	[XmlElement(ElementName = "minimum_range")]
	public string minimum_range;

	[XmlElement(ElementName = "maximum_range")]
	public float maximum_range = 1f;

	[XmlElement(ElementName = "minimum_health_percent")]
	public string minimum_health_percent;

	[XmlElement(ElementName = "maximum_health_percent")]
	public string maximum_health_percent;

	[XmlElement(ElementName = "cooldown_start_available")]
	public string cooldown_start_available;

	[XmlElement(ElementName = "cooldown_refresh_time")]
	public string cooldown_refresh_time;

	[XmlElement(ElementName = "cooldown_expire_time")]
	public string cooldown_expire_time;

	[XmlElement(ElementName = "attack_name")]
	public string attack_name;

	[XmlElement(ElementName = "behavior")]
	public string behavior = "BehaviorAttackBase";

	[XmlElement(ElementName = "anim")]
	public string anim = string.Empty;

	[XmlElement(ElementName = "desired_range")]
	public float desired_range = 0.5f;

	[XmlElement(ElementName = "interruptible_by")]
	public int interruptible_by = 4;

	[XmlElement(ElementName = "effect")]
	public string effect;

	[XmlElement(ElementName = "triggered_effect")]
	public string triggered_effect;

	[XmlElement(ElementName = "stop_effect_on_chain")]
	public bool stop_effect_on_chain;

	[XmlElement(ElementName = "power_cost")]
	public float power_cost;

	[XmlElement(ElementName = "move_start_time")]
	public float move_start_time;

	[XmlElement(ElementName = "move_arrive_time")]
	public float move_arrive_time;

	[XmlElement(ElementName = "collision_off_time")]
	public float collision_off_time = -1f;

	[XmlElement(ElementName = "collision_on_time")]
	public float collision_on_time = -1f;

	[XmlElement(ElementName = "move_speed")]
	public float move_speed = 1.5f;

	[XmlElement(ElementName = "chain_start_time")]
	public float chain_start_time;

	[XmlElement(ElementName = "chain_end_time")]
	public float chain_end_time;

	[XmlElement(ElementName = "chain_requires_impact")]
	public bool chain_requires_impact;

	[XmlElement(ElementName = "face_target")]
	public bool face_target = true;

	[XmlElement(ElementName = "track_target")]
	public bool track_target = true;

	[XmlElement(ElementName = "allow_lateral_translation")]
	public bool allow_lateral_translation;

	[XmlElement(ElementName = "attack_duration")]
	public float attack_duration;

	[XmlElement(ElementName = "forward_speed")]
	public float forward_speed;

	[XmlElement(ElementName = "forward_on_hit")]
	public bool forward_on_hit;

	[XmlElement(ElementName = "environmental_attack")]
	public bool environmental_attack;

	[XmlElement(ElementName = "targetless_attack")]
	public bool targetless_attack;

	[XmlElement(ElementName = "ignore_height_difference")]
	public bool ignore_height_difference;

	[XmlElement(ElementName = "target_effect")]
	public string target_effect;

	[XmlElement(ElementName = "hide_on_teleport")]
	public bool hide_on_teleport;

	[XmlElement(ElementName = "total_targets")]
	public int total_targets = -1;

	[XmlElement(ElementName = "attack_delay")]
	public float attack_delay;

	[XmlElement(ElementName = "minimum_delay")]
	public float minimum_delay;

	[XmlElement(ElementName = "pinball_pause")]
	public float pinball_pause;

	[XmlElement(ElementName = "teleport_effect")]
	public string teleport_effect = string.Empty;

	[XmlElement(ElementName = "pinball_prefab")]
	public string pinball_prefab = string.Empty;

	[XmlElement(ElementName = "linear_pinball")]
	public bool linear_pinball = true;

	[XmlElement(ElementName = "has_shot_info_tag")]
	public bool has_shot_info_tag;

	[XmlElement(ElementName = "arc_multiplier")]
	public float arc_multiplier;

	[XmlElement(ElementName = "pinball_start_node")]
	public string pinball_start_node = string.Empty;

	[XmlElement(ElementName = "pinball_turn_duration")]
	public float pinball_turn_duration;

	[XmlElement(ElementName = "pinball_cannot_be_hit")]
	public bool pinball_cannot_be_hit = true;

	[XmlElement(ElementName = "attack_child_name")]
	public string[] attack_child_names;

	[XmlElement(ElementName = "auto_chain_attack_name")]
	public string auto_chain_attack_name;

	[XmlElement(ElementName = "stop_on_enemy_collision")]
	public bool stop_on_enemy_collision = true;

	[XmlElement(ElementName = "summons")]
	public SummonsRaw[] summons;

	[XmlElement(ElementName = "always_render_max_beam")]
	public bool always_render_max_beam;

	[XmlElement(ElementName = "beam_hit_geometry_effect")]
	public string beam_hit_geometry_effect = string.Empty;

	[XmlElement(ElementName = "beam_hit_nothing_effect")]
	public string beam_hit_nothing_effect = string.Empty;

	[XmlElement(ElementName = "impact")]
	public ImpactRaw[] impacts;
}
