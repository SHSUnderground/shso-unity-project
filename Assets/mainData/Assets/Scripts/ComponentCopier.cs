using System;
using System.Collections.Generic;
using UnityEngine;

public class ComponentCopier
{
	protected static Type[] customCopierTypes = new Type[1]
	{
		typeof(AnimationComponentCopier)
	};

	protected static Dictionary<Type, IComponentCopier> customCopiers;

	protected static IComponentCopier defaultCopier;

	public static Component Copy(Component sourceComponent, GameObject destination)
	{
		if (customCopiers == null)
		{
			InitCustomCopiers();
		}
		if (sourceComponent == null)
		{
			throw new ArgumentNullException("sourceComponent", "Tried to duplicate a null component");
		}
		if (destination == null)
		{
			throw new ArgumentNullException("destination", "Tried to copy a component to a null destination");
		}
		Type type = sourceComponent.GetType();
		Component component = destination.GetComponent(type);
		if (component == null)
		{
			component = destination.AddComponent(type);
		}
		IComponentCopier value;
		if (!customCopiers.TryGetValue(type, out value))
		{
			value = defaultCopier;
		}
		if (value != null)
		{
			value.Copy(type, sourceComponent, component);
		}
		return component;
	}

	protected static void InitCustomCopiers()
	{
		defaultCopier = new GenericComponentCopier();
		customCopiers = new Dictionary<Type, IComponentCopier>();
		Type[] array = customCopierTypes;
		foreach (Type type in array)
		{
			if (typeof(IComponentCopier).IsAssignableFrom(type))
			{
				IComponentCopier componentCopier = Activator.CreateInstance(type) as IComponentCopier;
				customCopiers[componentCopier.CopiedType] = componentCopier;
			}
		}
	}
}
