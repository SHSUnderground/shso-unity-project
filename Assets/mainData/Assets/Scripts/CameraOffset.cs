using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Audio/Camera Offset")]
public class CameraOffset : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private Vector3 originalPosition;

	private Stack<GameObject> playerStack = new Stack<GameObject>();

	private GameObject _cachedLocalPlayer;

	private GameObject Player
	{
		get
		{
			FlushNullPlayersOnStack();
			return (playerStack.Count <= 0) ? LocalPlayer : playerStack.Peek();
		}
	}

	private GameObject LocalPlayer
	{
		get
		{
			if (_cachedLocalPlayer == null)
			{
				_cachedLocalPlayer = GameController.GetController().LocalPlayer;
			}
			return _cachedLocalPlayer;
		}
	}

	private void Start()
	{
		originalPosition = base.gameObject.transform.position;
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		if (localPlayer != null)
		{
			playerStack.Push(localPlayer);
		}
	}

	private void OnEnable()
	{
		StartCoroutine(RegisterEventListeners());
	}

	private IEnumerator RegisterEventListeners()
	{
		while (AppShell.Instance == null || AppShell.Instance.EventMgr == null)
		{
			yield return 0;
		}
		AppShell.Instance.EventMgr.AddListener<PlayerInputControllerCreated>(OnPlayerInputControllerCreated);
		AppShell.Instance.EventMgr.AddListener<PlayerInputControllerDestroyed>(OnPlayerInputControllerDestroyed);
	}

	private void OnDisable()
	{
		if (AppShell.Instance != null && AppShell.Instance.EventMgr != null)
		{
			AppShell.Instance.EventMgr.RemoveListener<PlayerInputControllerCreated>(OnPlayerInputControllerCreated);
			AppShell.Instance.EventMgr.RemoveListener<PlayerInputControllerDestroyed>(OnPlayerInputControllerDestroyed);
		}
	}

	private void Update()
	{
		GameObject player = Player;
		if (player != null)
		{
			Vector3 b = Camera.main.transform.position - player.transform.position;
			base.transform.position = originalPosition + b;
		}
	}

	private void OnPlayerInputControllerCreated(PlayerInputControllerCreated e)
	{
		if (e.Controller != null)
		{
			FlushNullPlayersOnStack();
			playerStack.Push(e.Controller.gameObject);
		}
	}

	private void OnPlayerInputControllerDestroyed(PlayerInputControllerDestroyed e)
	{
		FlushNullPlayersOnStack();
		if (e.Controller != null && playerStack.Count > 0 && playerStack.Peek() == e.Controller.gameObject)
		{
			playerStack.Pop();
		}
	}

	private void FlushNullPlayersOnStack()
	{
		while (playerStack.Count > 0 && playerStack.Peek() == null)
		{
			playerStack.Pop();
		}
	}
}
