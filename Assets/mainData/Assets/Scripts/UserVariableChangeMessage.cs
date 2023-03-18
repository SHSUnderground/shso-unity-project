using System.Collections;

public class UserVariableChangeMessage : ShsEventMessage
{
	public int userId;

	public Hashtable changedVars;

	public UserVariableChangeMessage(int userId, Hashtable changedVars)
	{
		this.userId = userId;
		this.changedVars = changedVars;
	}
}
