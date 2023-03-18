using System;
using UnityEngine;

[Serializable]
public class PhysicMaterialEx
{
	public enum MaterialType
	{
		None,
		Adamantium,
		Neutronium,
		Plastic,
		Ceramic,
		Rubber,
		Steel,
		Stone,
		Wood,
		HQ
	}

	public MaterialType name;

	public PhysicMaterial baseMaterial;

	public float density;

	public bool isImmobile;

	public ShsAudioSource collisionAudioSmall;

	public ShsAudioSource collisionAudioMedium;

	public ShsAudioSource collisionAudioLarge;

	public ShsAudioSource GetCollisionAudio(float objectVolume)
	{
		if (objectVolume < 5f)
		{
			return collisionAudioSmall;
		}
		if (objectVolume < 40f)
		{
			return collisionAudioMedium;
		}
		return collisionAudioLarge;
	}
}
