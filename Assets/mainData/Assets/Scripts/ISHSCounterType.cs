using System.Collections.Generic;

public interface ISHSCounterType
{
	string Name
	{
		get;
	}

	string Path
	{
		get;
	}

	string Id
	{
		get;
	}

	CounterCycleTypeEnum CycleType
	{
		get;
	}

	string CycleCounter
	{
		get;
	}

	long MaxAccumulationPerCycle
	{
		get;
	}

	Dictionary<string, ISHSCounterType> SubCounters
	{
		get;
	}

	Dictionary<SHSCounterBank, Dictionary<string, long>> QualifierValues
	{
		get;
	}

	ISHSCounterType Parent
	{
		get;
	}

	long GetCurrentValue();

	long GetCurrentValue(SHSCounterBank bank);

	long GetCurrentValue(string qualifierKey);

	long GetCurrentValue(SHSCounterBank bank, string qualifierKey);

	void InitCurrentValue(string qualifierKey, long value);

	void InitCurrentValue(SHSCounterBank bank, string qualifierKey, long value);

	void InitCurrentValue(long value);

	void InitCurrentValue(SHSCounterBank bank, long value);

	void Configure(DataWarehouse data);

	void Update(double Time);

	void Reset();

	void AddCounterBank(SHSCounterBank bank);

	void RemoveCounterBank(SHSCounterBank bank);

	bool AddCounter();

	bool AddCounter(SHSCounterBank bank);

	bool AddCounter(long Count);

	bool AddCounter(SHSCounterBank bank, long Count);

	bool AddCounter(string qualifierKey);

	bool AddCounter(SHSCounterBank bank, string qualifierKey);

	bool AddCounter(string qualifierKey, long Count);

	bool AddCounter(SHSCounterBank bank, string qualifierKey, long Count);

	bool AddCounter(string qualifierKey, long Count, bool reportEach);

	bool AddCounter(SHSCounterBank bank, string qualifierKey, long Count, bool reportEach);

	bool SetCounter(long Count);

	bool SetCounter(SHSCounterBank bank, long Count);

	bool SetCounter(long Count, SHSCounterType.ReportingMethodEnum reportingMethod);

	bool SetCounter(SHSCounterBank bank, long Count, SHSCounterType.ReportingMethodEnum reportingMethod);

	bool SetCounter(string qualifierKey, long Count);

	bool SetCounter(SHSCounterBank bank, string qualifierKey, long Count);

	bool SetCounter(string qualifierKey, long Count, SHSCounterType.ReportingMethodEnum reportingMethod);

	bool SetCounter(SHSCounterBank bank, string qualifierKey, long Count, SHSCounterType.ReportingMethodEnum reportingMethod);
}
