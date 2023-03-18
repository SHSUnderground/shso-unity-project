using System;
using System.Collections.Generic;

public class ChallengeClient : ChallengeBase
{
	private static readonly string ClientMsgReqPreamble = "client_msg_req:";

	private Dictionary<string, string[]> _clientMsgReqs;

	public ChallengeClient()
	{
		challengeValidationSource = ChallengeValidationEnum.ClientOnly;
	}

	public virtual bool IsClientChallengeMet()
	{
		throw new NotImplementedException();
	}

	protected virtual void OnClientChallengeEvent(object[] data)
	{
		throw new NotImplementedException();
	}

	public override void Initialize(ChallengeManager manager, ChallengeInfo info, ISHSCounterType counter, ChallengeManager.ChallengeCompleteDelegate onChallengeComplete)
	{
		base.Initialize(manager, info, counter, onChallengeComplete);
		if (manager.WatcherCounter.GetCurrentValue() != info.ChallengeId)
		{
			InitializeCounterValue();
			manager.WatcherCounter.SetCounter(info.ChallengeId, SHSCounterType.ReportingMethodEnum.WebService);
		}
		_clientMsgReqs = new Dictionary<string, string[]>();
		foreach (ChallengeInfoParameters parameter in info.Parameters)
		{
			string text = parameter.key.ToLower();
			if (text.StartsWith(ClientMsgReqPreamble))
			{
				string text2 = text.Substring(ClientMsgReqPreamble.Length);
				if (_clientMsgReqs.ContainsKey(text2))
				{
					CspUtils.DebugLog("ChallengeClient::Initialize() - message requirement <" + text2 + "> is specified twice");
				}
				else
				{
					_clientMsgReqs.Add(text2, parameter.value.ToLower().Split('|'));
				}
			}
		}
	}

	protected void NotifyOnClientChallengeMet()
	{
		if (onChallengeComplete != null)
		{
			onChallengeComplete(this);
		}
		challengeStatus = ChallengeStatus.AwaitingServerConfirmation;
		LogChallengeStatus();
	}

	public virtual void InitializeCounterValue()
	{
		challengeCounter.SetCounter(0L, SHSCounterType.ReportingMethodEnum.WebService);
	}

	public override void HandleChallengeEvent(ChallengeEventMessage msg)
	{
		if (!(msg.MessageType != messageType))
		{
			foreach (KeyValuePair<string, string[]> clientMsgReq in _clientMsgReqs)
			{
				if (msg == null || msg.MessageRequirements == null || !msg.MessageRequirements.ContainsKey(clientMsgReq.Key))
				{
					return;
				}
				bool flag = false;
				string[] value = clientMsgReq.Value;
				foreach (string b in value)
				{
					if (msg.MessageRequirements[clientMsgReq.Key] == b)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return;
				}
			}
			OnClientChallengeEvent(msg.ExtraData);
		}
	}

	public override void Ready()
	{
		base.Ready();
		if (IsClientChallengeMet())
		{
			NotifyOnClientChallengeMet();
		}
	}

	public override void Dispose()
	{
		base.Dispose();
		_clientMsgReqs = null;
	}
}
