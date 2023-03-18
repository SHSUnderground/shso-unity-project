using UnityEngine;

public interface IInteractiveObjectChild
{
	void Initialize(InteractiveObject owner, GameObject model);

	float GetLength();
}
