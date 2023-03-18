using UnityEngine;

public interface IAutomationBehaviorAPI
{
	bool move(Vector3 v);

	bool pickup(string o);

	bool throwat();

	bool fling();

	bool place();

	bool fight();

	bool collect();
}
