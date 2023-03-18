using System.Xml.Serialization;
using UnityEngine;

public class ImpactRaw
{
	[XmlElement(ElementName = "impact_type")]
	public int impact_type;

	[XmlElement(ElementName = "collider")]
	public string collider;

	[XmlElement(ElementName = "effect")]
	public string effect;

	[XmlElement(ElementName = "impact_effect")]
	public string impact_effect = string.Empty;

	[XmlElement(ElementName = "impact_face_camera")]
	public bool impact_face_camera = true;

	[XmlElement(ElementName = "hits_friends")]
	public bool hits_friends;

	[XmlElement(ElementName = "hits_enemies")]
	public bool hits_enemies = true;

	[XmlElement(ElementName = "maximum_hits_per_target")]
	public int maximum_hits_per_target;

	[XmlElement(ElementName = "maximum_targets_hit")]
	public int maximum_targets_hit = 1;

	[XmlElement(ElementName = "minimum_distance")]
	public float minimum_distance;

	[XmlElement(ElementName = "tool_damage_table_setting")]
	public string tool_damage_table_setting;

	[XmlElement(ElementName = "damage")]
	public float damage;

	[XmlElement(ElementName = "damage_ss")]
	public string damage_ss = string.Empty;

	[XmlElement(ElementName = "damage_sf")]
	public float damage_sf = 1f;

	[XmlElement(ElementName = "tool_damage_scale")]
	public string tool_damage_scale;

	[XmlElement(ElementName = "pushback_duration")]
	public float pushback_duration = 0.5f;

	[XmlElement(ElementName = "pushback_duration_ss")]
	public string pushback_duration_ss = string.Empty;

	[XmlElement(ElementName = "pushback_duration_sf")]
	public float pushback_duration_sf;

	[XmlElement(ElementName = "pushback_velocity")]
	public float pushback_velocity = 2f;

	[XmlElement(ElementName = "pushback_velocity_ss")]
	public string pushback_velocity_ss = string.Empty;

	[XmlElement(ElementName = "pushback_velocity_sf")]
	public float pushback_velocity_sf;

	[XmlElement(ElementName = "recoil")]
	public int recoil = 1;

	[XmlElement(ElementName = "recoil_duration")]
	public float recoil_duration = -100f;

	[XmlElement(ElementName = "recoil_duration_ss")]
	public string recoil_duration_ss = string.Empty;

	[XmlElement(ElementName = "recoil_duration_sf")]
	public float recoil_duration_sf;

	[XmlElement(ElementName = "launch_velocity")]
	public float launch_velocity;

	[XmlElement(ElementName = "launch_velocity_ss")]
	public string launch_velocity_ss = string.Empty;

	[XmlElement(ElementName = "launch_velocity_sf")]
	public float launch_velocity_sf;

	[XmlElement(ElementName = "collider_angle_limit")]
	public float collider_angle_limit = 45f;

	[XmlElement(ElementName = "rotate_target_to_impact")]
	public bool rotate_target_to_impact = true;

	[XmlElement(ElementName = "show_collider")]
	public bool show_collider;

	[XmlElement(ElementName = "impact_start_time")]
	public float impact_start_time;

	[XmlElement(ElementName = "impact_end_time")]
	public float impact_end_time;

	[XmlElement(ElementName = "collider_scale")]
	public float collider_scale = 1f;

	[XmlElement(ElementName = "collider_scale_ss")]
	public string collider_scale_stat = string.Empty;

	[XmlElement(ElementName = "collider_scale_sf")]
	public float collider_scale_factor = 1f;

	[XmlElement(ElementName = "pushback_from_collider")]
	public bool pushback_from_collider;

	[XmlElement(ElementName = "force_attach")]
	public bool force_attach;

	[XmlElement(ElementName = "firing_time")]
	public float firing_time;

	[XmlElement(ElementName = "projectile_name")]
	public string projectile_name;

	[XmlElement(ElementName = "projectile_speed")]
	public float projectile_speed;

	[XmlElement(ElementName = "projectile_lifespan")]
	public float projectile_lifespan;

	[XmlElement(ElementName = "projectile_aimed")]
	public bool projectile_aimed = true;

	[XmlElement(ElementName = "projectile_rotate_to_velocity")]
	public bool projectile_rotate_to_velocity = true;

	[XmlElement(ElementName = "projectile_attached")]
	public bool projectile_attached;

	[XmlElement(ElementName = "projectile_gravity")]
	public float projectile_gravity;

	[XmlElement(ElementName = "projectile_explosion_radius")]
	public float projectile_explosion_radius;

	[XmlElement(ElementName = "projectile_destroy_on_impact")]
	public bool projectile_destroy_on_impact = true;

	[XmlElement(ElementName = "projectile_destroy_on_collision")]
	public bool projectile_destroy_on_collision = true;

	[XmlElement(ElementName = "projectile_impact_stick_time")]
	public float projectile_impact_stick_time;

	[XmlElement(ElementName = "projectile_collision_stick_time")]
	public float projectile_collision_stick_time;

	[XmlElement(ElementName = "projectile_environmental")]
	public bool projectile_environmental;

	[XmlElement(ElementName = "projectile_create_immediate")]
	public bool projectile_create_immediate;

	[XmlElement(ElementName = "projectile_additional_targets")]
	public int projectile_additional_targets;

	[XmlElement(ElementName = "projectile_return_arc")]
	public int projectile_return_arc;

	[XmlElement(ElementName = "projectile_ballistic")]
	public bool projectile_ballistic;

	[XmlElement(ElementName = "projectile_ballistic_lob")]
	public bool projectile_ballistic_lob;

	[XmlElement(ElementName = "projectile_scaled_to_owner")]
	public bool projectile_scaled_to_owner;

	[XmlElement(ElementName = "target_aim_offset")]
	public float target_aim_offset;

	[XmlElement(ElementName = "impact_matrix_type")]
	public int impact_matrix_type;

	[XmlElement(ElementName = "projectile_target_reticle")]
	public string projectile_target_reticle;

	[XmlElement(ElementName = "attacker_remove_effect")]
	public string attacker_remove_effect;

	[XmlElement(ElementName = "required_combat_effect")]
	public string required_combat_effect;

	[XmlElement(ElementName = "event_name")]
	public string event_name;

	[XmlElement(ElementName = "event_object_name")]
	public string event_object_name;

	[XmlElement(ElementName = "pickup_name")]
	public string pickup_name;

	[XmlElement(ElementName = "target_combat_effect")]
	public string target_combat_effect = string.Empty;

	[XmlElement(ElementName = "target_remove_combat_effect")]
	public string target_remove_combat_effect = string.Empty;

	[XmlElement(ElementName = "attacker_combat_effect", IsNullable = true)]
	public string[] attacker_combat_effect;

	[XmlElement(ElementName = "collider_offset")]
	public Vector3 collider_offset;

	[XmlElement(ElementName = "next_impact_delay")]
	public float next_impact_delay;

	[XmlElement(ElementName = "use_recoil_duration_on_attach")]
	public bool use_recoil_duration_on_attach;

	[XmlElement(ElementName = "attach_prefab_name")]
	public string attach_prefab_name;

	[XmlElement(ElementName = "attach_anim_name")]
	public string attach_anim_name;

	[XmlElement(ElementName = "attach_uses_rotation")]
	public bool attach_uses_rotation;

	[XmlElement(ElementName = "stun_anim_speed")]
	public float stun_anim_speed = -1f;

	[XmlElement(ElementName = "summons")]
	public SummonsRaw[] summons;
}
