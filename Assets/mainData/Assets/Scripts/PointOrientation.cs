using System.Runtime.CompilerServices;
using UnityEngine;

public class PointOrientation
{
	[CompilerGenerated]
	private Vector3 _003CPosition_003Ek__BackingField;

	[CompilerGenerated]
	private Quaternion _003CRotation_003Ek__BackingField;

	public Vector3 Position
	{
		[CompilerGenerated]
		get
		{
			return _003CPosition_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CPosition_003Ek__BackingField = value;
		}
	}

	public Quaternion Rotation
	{
		[CompilerGenerated]
		get
		{
			return _003CRotation_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CRotation_003Ek__BackingField = value;
		}
	}

	public Vector3 Direction
	{
		get
		{
			return Rotation.eulerAngles;
		}
		set
		{
			Rotation = Quaternion.LookRotation(value);
		}
	}

	public PointOrientation()
	{
		Position = Vector3.zero;
		Rotation = Quaternion.identity;
	}

	public PointOrientation(Vector3 position, Quaternion rotation)
	{
		Position = position;
		Rotation = rotation;
	}

	public PointOrientation(Vector3 position, Vector3 direction)
	{
		Position = position;
		Direction = direction;
	}

	public PointOrientation(Transform transform)
	{
		Position = transform.position;
		Rotation = transform.rotation;
	}
}
