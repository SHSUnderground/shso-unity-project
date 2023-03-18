public class CerebroController : GameController
{
	protected static CerebroController instance;

	public override void Start()
	{
		CspUtils.DebugLog("CerebroController::Start()");
		base.Start();
	}

	private void Update()
	{
	}

	public override void Awake()
	{
		CspUtils.DebugLog("CerebroController::Awake()");
		base.Awake();
		if (instance != null)
		{
			CspUtils.DebugLog("A second CerebroController is being created.  This may lead to instabilities!");
		}
		else
		{
			instance = this;
		}
	}
}
