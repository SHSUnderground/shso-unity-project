using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationComponentCopier : GenericComponentCopier
{
	public override bool CopiesProperties
	{
		get
		{
			return true;
		}
	}

	public override Type CopiedType
	{
		get
		{
			return typeof(Animation);
		}
	}

	public override void Copy(Type type, Component source, Component destination)
	{
		Animation animation = source as Animation;
		Animation animation2 = destination as Animation;
		List<AnimationState> list = new List<AnimationState>();
		foreach (AnimationState item in animation2)
		{
			list.Add(item);
		}
		foreach (AnimationState item2 in list)
		{
			animation2.RemoveClip(item2.clip);
		}
		foreach (AnimationState item3 in animation)
		{
			animation2.AddClip(item3.clip, item3.name);
		}
		base.Copy(type, source, destination);
	}
}
