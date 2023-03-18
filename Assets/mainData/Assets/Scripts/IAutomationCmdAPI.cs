public interface IAutomationCmdAPI
{
	bool execute();

	bool isCompleted();

	bool isReady();

	bool precheckOk();
}
