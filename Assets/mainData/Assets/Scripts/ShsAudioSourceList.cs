using ShsAudio;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ShsAudioSourceList : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	[Serializable]
	public class SFX
	{
		public string name;

		public GameObject sfx;
	}

	public SFX[] sounds;

	private TransactionMonitor preloadTransaction;

	private TransactionMonitor bundleLoadTransaction;

	public static ShsAudioSourceList GetList(string listName)
	{
		GameObject gameObject = Resources.Load("Audio/Lists/" + listName) as GameObject;
		return (!(gameObject != null)) ? null : gameObject.GetComponent<ShsAudioSourceList>();
	}

	public GameObject GetSource(string name)
	{
		SFX[] array = sounds;
		foreach (SFX sFX in array)
		{
			if (sFX.name == name)
			{
				return sFX.sfx;
			}
		}
		return null;
	}

	public void PreloadAll(TransactionMonitor parentTransaction)
	{
		PreloadAll(parentTransaction, true, float.MaxValue);
	}

	public void PreloadAll(TransactionMonitor parentTransaction, bool unloadOnSceneTransition)
	{
		PreloadAll(parentTransaction, unloadOnSceneTransition, float.MaxValue);
	}

	public void PreloadAll(TransactionMonitor parentTransaction, float timeout)
	{
		PreloadAll(parentTransaction, true, timeout);
	}

	public void PreloadAll(TransactionMonitor parentTransaction, bool unloadOnSceneTransition, float timeout)
	{
		if (preloadTransaction != null)
		{
			parentTransaction.AddChild(preloadTransaction);
			return;
		}
		GameObject userData = new GameObject("_sfx_preload");
		preloadTransaction = TransactionMonitor.CreateTransactionMonitor("SfxPreloadTransaction", OnSFXPreloadTransactionDone, timeout, userData);
		List<string> list = new List<string>();
		SFX[] array = sounds;
		foreach (SFX sFX in array)
		{
			bool flag = false;
			ShsAudioSource component = sFX.sfx.GetComponent<ShsAudioSource>();
			if (component != null)
			{
				ShsAudioBase.AudioClipReference[] clips = component.Clips;
				foreach (ShsAudioBase.AudioClipReference audioClipReference in clips)
				{
					bool flag2 = audioClipReference.IsBundleNameInAssetName();
					if (audioClipReference.BundleName != 0 || flag2)
					{
						flag = true;
						string item = (!flag2) ? audioClipReference.BundleName.ToString() : audioClipReference.AssetName.Substring(0, audioClipReference.AssetName.IndexOf('|'));
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
				}
			}
			if (flag)
			{
				preloadTransaction.AddStep(sFX.name, TransactionMonitor.DumpTransactionStatus);
			}
		}
		if (list.Count > 0)
		{
			bundleLoadTransaction = TransactionMonitor.CreateTransactionMonitor(OnBundleTransactionDone, float.MaxValue, userData);
			preloadTransaction.AddChild(bundleLoadTransaction);
			parentTransaction.AddChild(preloadTransaction);
			foreach (string item2 in list)
			{
				bundleLoadTransaction.AddStep(item2);
			}
			foreach (string item3 in list)
			{
				AppShell.Instance.BundleLoader.FetchAssetBundle(Helpers.GetAudioBundleName(item3), OnBundleLoaded, item3, unloadOnSceneTransition);
			}
		}
		else
		{
			OnSFXPreloadTransactionDone(TransactionMonitor.ExitCondition.Success, null, userData);
		}
	}

	private void OnBundleLoaded(AssetBundleLoadResponse response, object userData)
	{
		bundleLoadTransaction.CompleteStep(userData as string);
	}

	private void OnBundleTransactionDone(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		bundleLoadTransaction = null;
		if (exit != 0)
		{
			preloadTransaction.Fail("Bundle load failed: " + error);
			return;
		}
		SFX[] array = sounds;
		foreach (SFX sFX in array)
		{
			ShsAudioSource.PreloadSound(sFX.sfx, userData as GameObject, OnSoundPreloaded, sFX.name);
		}
	}

	private void OnSoundPreloaded(ShsAudioSource audioSrcInstance, object userData)
	{
		UnityEngine.Object.Destroy(audioSrcInstance.gameObject);
		preloadTransaction.CompleteStep(userData as string);
	}

	private void OnSFXPreloadTransactionDone(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		if (exit != 0)
		{
			CspUtils.DebugLog("Failed to preload sounds for " + base.gameObject.name + ": " + error);
		}
		UnityEngine.Object.Destroy(userData as GameObject);
		preloadTransaction = null;
	}
}
