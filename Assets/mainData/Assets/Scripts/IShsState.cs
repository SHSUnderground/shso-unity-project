using System;

public interface IShsState
{
	void Enter(Type previousState);

	void Update();

	void Leave(Type nextState);
}
