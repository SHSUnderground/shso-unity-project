using System.Collections.Generic;

public interface IExpendHandler
{
	int ExpendRequestId
	{
		get;
	}

	string OwnableTypeId
	{
		get;
	}

	int Timeout
	{
		get;
		set;
	}

	float StartTime
	{
		get;
	}

	List<KeyValuePair<float, string>> ExpendHistoryLog
	{
		get;
	}

	ExpendableDefinition ExpendableDefinition
	{
		get;
	}

	ExpendHandlerState State
	{
		get;
	}

	void Initialize(int RequestId, ExpendableDefinition definition, ExpendablesManager.ExpendHandlerCompleteCallback managerCallback, ExpendablesManager.ExpendHandlerCompleteCallback onCompleteCallback);

	void OnExpendPreEffect();

	void OnExpendStart();

	void OnExpendServerAuthorizationSuccess(ConsumedPotionMessage message);

	void OnExpendServerAuthorizationFailed(ConsumedPotionMessage message);

	void LogExpendAction(string action);

	void Update();
}
