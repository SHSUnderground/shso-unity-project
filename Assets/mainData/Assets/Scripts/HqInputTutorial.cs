using UnityEngine;

public class HqInputTutorial : HqInput
{
	public bool allowPausing;

	private bool paused;

	public bool Paused
	{
		get
		{
			return paused;
		}
		set
		{
			paused = value;
			if (paused)
			{
				(HqController2.Instance as HqControllerTutorial).GoToPauseMode();
			}
			else
			{
				(HqController2.Instance as HqControllerTutorial).GoToPlayMode();
			}
		}
	}

	public override void Start()
	{
		fsm = new ShsFSM();
		fsm.AddState(new TutorialCameraMovementSection(this));
		fsm.AddState(new TutorialPausePlaySection(this));
		fsm.AddState(new TutorialChangeRoomsSection(this));
		fsm.AddState(new TutorialFollowHeroSection(this));
		fsm.AddState(new TutorialPaintRoomSection(this));
		fsm.AddState(new TutorialPlaceItemsSection(this));
		fsm.AddState(new TutorialPlaceHeroSection(this));
		fsm.AddState(new TutorialCommandSection(this));
		fsm.GotoState<TutorialCameraMovementSection>();
		AppShell.Instance.EventMgr.AddListener<HQRoomZoomRequestMessage>(base.onZoomRequest);
	}

	public void GotoCameraMovementSection()
	{
		fsm.GotoState<TutorialCameraMovementSection>();
	}

	public void GotoPausePlaySection()
	{
		fsm.GotoState<TutorialPausePlaySection>();
	}

	public void GotoChangeRoomSection()
	{
		fsm.GotoState<TutorialChangeRoomsSection>();
	}

	public void GotoFollowHeroSection()
	{
		fsm.GotoState<TutorialFollowHeroSection>();
	}

	public void GotoPaintRoomSection()
	{
		fsm.GotoState<TutorialPaintRoomSection>();
	}

	public void GotoPlaceItemsSection()
	{
		fsm.GotoState<TutorialPlaceItemsSection>();
	}

	public void GotoPlaceHeroSection()
	{
		fsm.GotoState<TutorialPlaceHeroSection>();
	}

	public void GotoCommandSection()
	{
		fsm.GotoState<TutorialCommandSection>();
	}

	public override void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<HQRoomZoomRequestMessage>(base.onZoomRequest);
	}

	public GameObject PickObject()
	{
		Ray ray = Camera.main.ScreenPointToRay(SHSInput.mousePosition);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 1000f, 3174912))
		{
			if (hitInfo.collider.gameObject == null)
			{
				return null;
			}
			GameObject gameObject = hitInfo.collider.gameObject;
			while (gameObject != null)
			{
				HqObject2 component = Utils.GetComponent<HqObject2>(gameObject);
				if (component != null)
				{
					return gameObject;
				}
				AIControllerHQ component2 = Utils.GetComponent<AIControllerHQ>(gameObject);
				if (component2 != null && component2.CanPickUp)
				{
					return gameObject;
				}
				if (gameObject.transform.parent == null)
				{
					return null;
				}
				gameObject = gameObject.transform.parent.gameObject;
			}
		}
		return null;
	}

	public override void Update()
	{
		base.Update();
		if (allowPausing && SHSInput.GetKeyDown(KeyCode.Pause))
		{
			if (!Paused)
			{
				Paused = true;
			}
			else
			{
				Paused = false;
			}
		}
	}
}
