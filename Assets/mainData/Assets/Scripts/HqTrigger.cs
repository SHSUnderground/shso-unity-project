public class HqTrigger : HqToy
{
	protected HqObject2 hqObj;

	protected bool isOn;

	public virtual bool IsOn
	{
		get
		{
			return isOn;
		}
	}

	public virtual void Start()
	{
		isOn = false;
		hqObj = Utils.GetComponent<HqObject2>(base.gameObject);
		if (hqObj == null)
		{
			hqObj = Utils.GetComponent<HqObject2>(base.gameObject, Utils.SearchParents);
		}
	}

	public virtual void TurnOn()
	{
		base.gameObject.active = false;
		base.gameObject.active = true;
		isOn = true;
	}

	public virtual void TurnOff()
	{
		isOn = false;
	}
}
