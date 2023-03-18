using UnityEngine;

public class EffectSequenceControllerNode : MonoBehaviour, IInteractiveObjectChild
{
	public bool interruptExisting;

	public EffectSequenceCollection sequences;

	protected EffectSequenceController controller;

	protected InteractiveObject owner;

	public void Initialize(InteractiveObject owner, GameObject model)
	{
		this.owner = owner;
	}

	public float GetLength()
	{
		return 0f;
	}

	private void Update()
	{
		if (controller == null)
		{
			AcquireController();
		}
		if (sequences != null && controller != null && controller.GetActiveCollection() != sequences)
		{
			if (!interruptExisting)
			{
				controller.SetActiveCollection(sequences);
			}
			else
			{
				controller.ForceSetActiveCollection(sequences);
			}
		}
	}

	public void OnDisable()
	{
		if (sequences != null && controller != null && sequences.loop)
		{
			controller.SetActiveCollection(null);
		}
	}

	private void AcquireController()
	{
		if (!(owner != null))
		{
			return;
		}
		InteractiveObjectController[] controllers = owner.Controllers;
		int num = 0;
		EffectSequenceController x;
		while (true)
		{
			if (num < controllers.Length)
			{
				InteractiveObjectController interactiveObjectController = controllers[num];
				x = (interactiveObjectController as EffectSequenceController);
				if (x != null)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		controller = x;
	}
}
