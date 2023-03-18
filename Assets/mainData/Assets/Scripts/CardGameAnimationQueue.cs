using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using System;

public class CardGameAnimationQueue : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public class QueuedAnimation
	{
		public string label;

		public CardGameAnimDelegate fn;

		public bool proceedOnFinish;

		public QueuedAnimation(string _label, CardGameAnimDelegate _fn, bool _proceed)
		{
			label = _label;
			fn = _fn;
			proceedOnFinish = _proceed;
		}
	}

	public delegate void CardGameAnimDelegate();

	private Queue<QueuedAnimation> queue = new Queue<QueuedAnimation>();

	private string currentAnimation;

	private bool busy;

	public string CurrentAnimation
	{
		get
		{
			return currentAnimation;
		}
	}

	public Queue<QueuedAnimation> Contents
	{
		get
		{
			return queue;
		}
	}

	public int QueueSize
	{
		get
		{
			return queue.Count;
		}
	}

	public bool QueueBusy
	{
		get
		{
			return busy;
		}
	}

	private CardGameAnimationQueue()
	{
		busy = true;
	}

	public void Enqueue(string label, CardGameAnimDelegate d, bool proceed)
	{
		queue.Enqueue(new QueuedAnimation(label, d, proceed));
		if (!busy)
		{
			AppShell.Instance.StartCoroutine(HandleNextMessage());
		}
	}

	public void OnAnimFinished(ShsEventMessage evt)
	{
		Resume();
	}

	public void Suspend()
	{
		busy = true;
	}

	public void Resume()
	{
		busy = false;
		AppShell.Instance.StartCoroutine(HandleNextMessage());
	}

	private void OnEnable()
	{
		///////// block added by CSP for testing //////////////
		//CspUtils.DebugLog("CGAQ QueueSize = " + QueueSize);
		//if (queue.Count > 0)
		//{
		//	QueuedAnimation anim = queue.Dequeue();
		//	CspUtils.DebugLog(" anim.label= " + anim.label);
		//}
		//EditorApplication.isPaused = true;
		try {
			CspUtils.DebugLog("CGAQ  AppShell.Instance.ToString() =" + AppShell.Instance.ToString());
			CspUtils.DebugLog("CGAQ  AppShell.Instance.EventMgr.ToString() =" + AppShell.Instance.EventMgr.ToString());
			CspUtils.DebugLog("CGAQ  AppShell.Instance.active = " + AppShell.Instance.active);
			CspUtils.DebugLog("CGAQ  AppShell.Instance.gameObject.active = " + AppShell.Instance.gameObject.active);
			//EditorApplication.isPaused = true;
		}
		catch (Exception e) {
			CspUtils.DebugLog("CGAQ e= " + e.StackTrace);
		}
		///////////////////////////////////////////////////////

		
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.AnimFinished>(OnAnimFinished);
		Resume();
	}

	private void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.AnimFinished>(OnAnimFinished);
	}

	private IEnumerator HandleNextMessage()
	{
		CspUtils.DebugLog("CGAQ coroutine HandleNextMessage() started!");
		while (!busy && queue.Count > 0)
		{
			QueuedAnimation anim = queue.Dequeue();
			currentAnimation = anim.label;
			busy = !anim.proceedOnFinish;
			anim.fn();
		}
		yield return 0;
	}
}
