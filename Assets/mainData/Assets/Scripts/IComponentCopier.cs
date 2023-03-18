using System;
using UnityEngine;

public interface IComponentCopier
{
	Type CopiedType
	{
		get;
	}

	void Copy(Type type, Component source, Component destination);
}
