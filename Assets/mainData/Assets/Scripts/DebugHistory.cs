using System.Collections.Generic;
using UnityEngine;

public class DebugHistory : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected class Event
	{
		protected float time;

		public Event()
		{
			time = Time.time;
		}

		public virtual void Log(GameObject owner)
		{
			CspUtils.DebugLog(owner.name + " : " + time);
		}
	}

	protected class PositionEvent : Event
	{
		protected Vector3 pos;

		public PositionEvent(Vector3 pos)
		{
			this.pos = pos;
		}

		public override void Log(GameObject owner)
		{
			CspUtils.DebugLog(owner.name + " : " + time + " : " + pos);
		}
	}

	protected class OwnershipEvent : Event
	{
		protected int oldOwner;

		protected int newOwner;

		public OwnershipEvent(int oldOwner, int newOwner)
		{
			this.oldOwner = oldOwner;
			this.newOwner = newOwner;
		}

		public override void Log(GameObject owner)
		{
			CspUtils.DebugLog(owner.name + " : " + time + " : old = " + oldOwner.ToString() + " : " + newOwner.ToString());
		}
	}

	protected class NetActionLog : Event
	{
		protected string dump;

		public NetActionLog(NetAction action)
		{
			dump = action.ToString();
		}

		public override void Log(GameObject owner)
		{
			CspUtils.DebugLog(owner.name + " : " + time + " : NetAction = " + dump);
		}
	}

	public int maxEventCount = 5000;

	public bool dumpNow;

	protected CharacterGlobals charGlobals;

	protected Queue<Event> history;

	protected Vector3 lastPosition = Vector3.zero;

	protected int lastOwner = -1;

	public void Awake()
	{
		history = new Queue<Event>();
		lastPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		lastOwner = -1;
	}

	public void Start()
	{
		charGlobals = Utils.GetComponent<CharacterGlobals>(base.gameObject);
	}

	public void Update()
	{
		if ((base.transform.position - lastPosition).sqrMagnitude >= 0.001f)
		{
			AddEvent(new PositionEvent(base.transform.position));
			lastPosition = base.transform.position;
		}
		if (lastOwner != charGlobals.networkComponent.NetOwnerId)
		{
			AddEvent(new OwnershipEvent(lastOwner, charGlobals.networkComponent.NetOwnerId));
			lastOwner = charGlobals.networkComponent.NetOwnerId;
		}
		if (dumpNow)
		{
			dumpNow = false;
			Dump();
		}
	}

	public void Dump()
	{
		foreach (Event item in history)
		{
			item.Log(base.gameObject);
		}
	}

	public void AddAction(NetAction action)
	{
		AddEvent(new NetActionLog(action));
	}

	protected void DebugCollision()
	{
		CspUtils.DebugLog("Handling DebugCollision message on " + base.gameObject.name);
		Dump();
	}

	protected void AddEvent(Event evt)
	{
		history.Enqueue(evt);
		if (history.Count > maxEventCount)
		{
			history.Dequeue();
		}
	}
}
