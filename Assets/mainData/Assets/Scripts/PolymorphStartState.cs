using System;
using UnityEngine;

public class PolymorphStartState : PolymorphBaseState
{
	private float _startTime;

	private float _delayTime;

	public PolymorphStartState(PolymorphStateProxy proxy)
		: base(proxy)
	{
		_delayTime = 0f;
		_startTime = 0f;
	}

	public override void Enter(Type previousState)
	{
		base.Enter(previousState);
		_startTime = Time.time + _delayTime;
		if (!string.IsNullOrEmpty(base.stateData.PolymorphEffect) && EffectSequence.FindEffect(base.stateData.PolymorphEffect, base.stateData.OriginalObject) == null)
		{
			base.stateData.CreateEffect(base.stateData.PolymorphEffect, base.stateData.OriginalObject);
		}
	}

	public void Initialize(float delayTime)
	{
		_delayTime = delayTime;
		_startTime = 0f;
	}

	public override bool Done()
	{
		return Time.time >= _startTime;
	}

	public override Type GetNextState()
	{
		return base.stateData.GetPolymorphState();
	}
}
