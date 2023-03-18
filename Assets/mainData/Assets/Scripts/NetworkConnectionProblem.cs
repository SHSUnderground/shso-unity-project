using UnityEngine;

public class NetworkConnectionProblem : ShsEventMessage
{
	public GameObject character;

	public bool responding;

	public NetworkConnectionProblem(GameObject character, bool responding)
	{
		this.character = character;
		this.responding = responding;
	}
}
