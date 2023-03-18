using UnityEngine;

public class OrientToLocalPlayer : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private static GameObject _player;

	protected static GameObject Player
	{
		get
		{
			if (_player == null)
			{
				_player = GameController.GetController().LocalPlayer;
			}
			return _player;
		}
	}

	private void Update()
	{
		if (Player != null)
		{
			Vector3 position = Player.transform.position;
			Vector3 position2 = base.transform.position;
			position.y = position2.y;
			base.transform.LookAt(position);
		}
	}
}
