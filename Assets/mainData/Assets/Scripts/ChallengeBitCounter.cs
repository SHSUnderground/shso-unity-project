using System;
using System.Collections.Generic;

public class ChallengeBitCounter : ChallengeClient
{
	protected Dictionary<object, byte> objectToBitMap;

	private byte _lastUsedBit;

	private long _bitMask;

	private long _counterSimple;

	private bool _useCounterSimple;

	public long CounterValue
	{
		get
		{
			return (!_useCounterSimple) ? challengeCounter.GetCurrentValue() : _counterSimple;
		}
		set
		{
			if (_useCounterSimple)
			{
				_counterSimple = value;
			}
			else
			{
				challengeCounter.SetCounter(value, SHSCounterType.ReportingMethodEnum.WebService);
			}
			LogChallengeStatus();
			if (IsClientChallengeMet())
			{
				NotifyOnClientChallengeMet();
			}
		}
	}

	public byte AssignBit(object obj)
	{
		return AssignBit(new object[1]
		{
			obj
		});
	}

	public byte AssignBit(object[] objArray)
	{
		if (objArray == null)
		{
			throw new ArgumentException();
		}
		foreach (object obj in objArray)
		{
			if (obj == null)
			{
				throw new ArgumentException();
			}
			if (objectToBitMap.ContainsKey(obj))
			{
				return objectToBitMap[obj];
			}
			objectToBitMap.Add(obj, _lastUsedBit);
		}
		_bitMask |= 1L << (int)_lastUsedBit;
		return _lastUsedBit++;
	}

	public void SetBit(object obj)
	{
		if (HasBit(obj))
		{
			SetBit(objectToBitMap[obj]);
		}
	}

	public void SetBit(byte bit)
	{
		CounterValue |= 1L << (int)bit;
	}

	public void SetAllBits()
	{
		CounterValue |= _bitMask;
	}

	public bool IsBitSet(object obj)
	{
		if (HasBit(obj))
		{
			return IsBitSet(objectToBitMap[obj]);
		}
		return false;
	}

	public bool IsBitSet(byte bit)
	{
		return (CounterValue & (1 << (int)bit)) != 0;
	}

	public bool AreAllBitsSet()
	{
		return (CounterValue ^ _bitMask) == 0;
	}

	public bool HasBit(object obj)
	{
		return obj != null && objectToBitMap.ContainsKey(obj);
	}

	public override bool IsClientChallengeMet()
	{
		return AreAllBitsSet();
	}

	protected override void OnClientChallengeEvent(object[] data)
	{
		if (data == null || data.Length <= 0)
		{
			return;
		}
		foreach (object obj in data)
		{
			if (!IsBitSet(obj) && !IsClientChallengeMet())
			{
				SetBit(obj);
			}
		}
	}

	public override void Initialize(ChallengeManager manager, ChallengeInfo info, ISHSCounterType counter, ChallengeManager.ChallengeCompleteDelegate onChallengeComplete)
	{
		base.Initialize(manager, info, counter, onChallengeComplete);
		objectToBitMap = new Dictionary<object, byte>();
		_lastUsedBit = 0;
		_bitMask = 0L;
		_useCounterSimple = false;
		foreach (ChallengeInfoParameters parameter in info.Parameters)
		{
			if (parameter.key == "counter_simple")
			{
				_useCounterSimple = bool.Parse(parameter.value);
			}
		}
	}

	public override void Dispose()
	{
		base.Dispose();
		objectToBitMap.Clear();
		objectToBitMap = null;
	}

	protected override string GetProgressLogString()
	{
		return Convert.ToString(CounterValue, 2) + "/" + Convert.ToString(_bitMask, 2);
	}
}
