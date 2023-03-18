using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ExpendHandlerBase : IExpendHandler
{
	private float startTime;

	private List<KeyValuePair<float, string>> expendHistoryLog;

	private ExpendablesManager.ExpendHandlerCompleteCallback OnManagerCompleteCallback;

	private ExpendablesManager.ExpendHandlerCompleteCallback OnCompleteCallback;

	[CompilerGenerated]
	private int _003CExpendRequestId_003Ek__BackingField;

	[CompilerGenerated]
	private string _003COwnableTypeId_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CTimeout_003Ek__BackingField;

	[CompilerGenerated]
	private ExpendableDefinition _003CExpendableDefinition_003Ek__BackingField;

	[CompilerGenerated]
	private ExpendHandlerState _003CState_003Ek__BackingField;

	public virtual int ExpendRequestId
	{
		[CompilerGenerated]
		get
		{
			return _003CExpendRequestId_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CExpendRequestId_003Ek__BackingField = value;
		}
	}

	public virtual string OwnableTypeId
	{
		[CompilerGenerated]
		get
		{
			return _003COwnableTypeId_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003COwnableTypeId_003Ek__BackingField = value;
		}
	}

	public virtual int Timeout
	{
		[CompilerGenerated]
		get
		{
			return _003CTimeout_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CTimeout_003Ek__BackingField = value;
		}
	}

	public virtual ExpendableDefinition ExpendableDefinition
	{
		[CompilerGenerated]
		get
		{
			return _003CExpendableDefinition_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CExpendableDefinition_003Ek__BackingField = value;
		}
	}

	public List<KeyValuePair<float, string>> ExpendHistoryLog
	{
		get
		{
			return expendHistoryLog;
		}
	}

	public virtual ExpendHandlerState State
	{
		[CompilerGenerated]
		get
		{
			return _003CState_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CState_003Ek__BackingField = value;
		}
	}

	public float StartTime
	{
		get
		{
			return startTime;
		}
	}

	public virtual void Initialize(int requestId, ExpendableDefinition definition, ExpendablesManager.ExpendHandlerCompleteCallback managerCallback, ExpendablesManager.ExpendHandlerCompleteCallback onCompleteCallback)
	{
		ExpendableDefinition = definition;
		ExpendRequestId = requestId;
		OwnableTypeId = definition.OwnableTypeId;
		OnManagerCompleteCallback = managerCallback;
		OnCompleteCallback = onCompleteCallback;
		LogExpendAction("Handler initialized for <" + ExpendRequestId + "> for ownable <" + OwnableTypeId + ">");
		Timeout = 15;
		State = ExpendHandlerState.AwaitingServerAuthorization;
		startTime = Time.time;
	}

	public virtual void OnExpendPreEffect()
	{
		LogExpendAction("Expend Pre Effect called for <" + ExpendRequestId + "> for ownable <" + OwnableTypeId + ">");
	}

	public virtual void OnExpendStart()
	{
		LogExpendAction("Expend Started for <" + ExpendRequestId + "> for ownable <" + OwnableTypeId + ">");
	}

	public virtual void OnExpendComplete()
	{
		LogExpendAction("Expend Completed for <" + ExpendRequestId + "> for ownable <" + OwnableTypeId + ">");
		State = ExpendHandlerState.Completed;
		HandlerCleanup();
	}

	public void OnExpendServerAuthorizationSuccess(ConsumedPotionMessage message)
	{
		LogExpendAction("Server auth Success for <" + ExpendRequestId + "> for ownable <" + OwnableTypeId + ">");
		State = ExpendHandlerState.Expending;
		OnExpendStart();
	}

	public void OnExpendServerAuthorizationFailed(ConsumedPotionMessage message)
	{
		LogExpendAction("Server auth FAILED for <" + ExpendRequestId + "> for ownable <" + OwnableTypeId + ">. Reason: " + message.ErrorCode);
		State = ExpendHandlerState.Failed;
		HandlerCleanup();
	}

	protected virtual void HandlerCleanup()
	{
		if (OnManagerCompleteCallback != null)
		{
			OnManagerCompleteCallback(this);
		}
		if (OnCompleteCallback != null)
		{
			OnCompleteCallback(this);
		}
	}

	public virtual void LogExpendAction(string action)
	{
		if (expendHistoryLog == null)
		{
			expendHistoryLog = new List<KeyValuePair<float, string>>();
		}
		expendHistoryLog.Add(new KeyValuePair<float, string>(Time.time, action));
	}

	public virtual void Update()
	{
		if (State == ExpendHandlerState.AwaitingServerAuthorization && Time.time - StartTime > (float)Timeout)
		{
			LogExpendAction("Timed out waiting for server authority.");
			State = ExpendHandlerState.TimedOut;
			HandlerCleanup();
			if (OnCompleteCallback != null)
			{
				OnCompleteCallback(this);
			}
		}
	}
}
