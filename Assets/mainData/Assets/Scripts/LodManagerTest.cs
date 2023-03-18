using System;
using UnityEngine;

public class LodManagerTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool OnOff = true;

	public int ForceLod;

	public bool ResetDistance;

	public LodBase.Mode mode;

	public float[] distances;

	private bool _OnOff = true;

	private LodBase.Mode _mode;

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftBracket))
		{
			OnOff = true;
		}
		if (Input.GetKeyDown(KeyCode.RightBracket))
		{
			OnOff = false;
		}
		if (Input.GetKeyDown(KeyCode.Keypad0))
		{
			ForceLod = 0;
			_OnOff = !OnOff;
		}
		if (Input.GetKeyDown(KeyCode.Keypad1))
		{
			ForceLod = 1;
			_OnOff = !OnOff;
		}
		if (Input.GetKeyDown(KeyCode.Keypad2))
		{
			ForceLod = 2;
			_OnOff = !OnOff;
		}
		if (Input.GetKeyDown(KeyCode.Keypad3))
		{
			ForceLod = 3;
			_OnOff = !OnOff;
		}
		if (OnOff != _OnOff)
		{
			_OnOff = OnOff;
			if (OnOff)
			{
				LodCharacter[] array = Utils.FindObjectsOfType<LodCharacter>();
				foreach (LodCharacter lod in array)
				{
					TurnLodOn(lod);
				}
			}
			else
			{
				LodCharacter[] array2 = Utils.FindObjectsOfType<LodCharacter>();
				foreach (LodCharacter lod2 in array2)
				{
					TurnLodOff(lod2);
				}
			}
		}
		if (ResetDistance)
		{
			ResetDistance = false;
			LodCharacter[] array3 = Utils.FindObjectsOfType<LodCharacter>();
			foreach (LodCharacter lodDistance in array3)
			{
				SetLodDistance(lodDistance);
			}
		}
		if (mode != _mode)
		{
			_mode = mode;
			LodCharacter[] array4 = Utils.FindObjectsOfType<LodCharacter>();
			foreach (LodCharacter lodMode in array4)
			{
				SetLodMode(lodMode);
			}
		}
	}

	private void TurnLodOn(LodCharacter lod)
	{
		lod.enabled = true;
	}

	private void TurnLodOff(LodCharacter lod)
	{
		lod.ForceSetLod(ForceLod);
		lod.enabled = false;
	}

	private void SetLodDistance(LodCharacter lod)
	{
		lod.distances = new float[distances.Length];
		Array.Copy(distances, lod.distances, distances.Length);
	}

	private void SetLodMode(LodCharacter lod)
	{
		lod.mode = mode;
	}
}
