using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TransactionMonitor
{
	public enum ExitCondition
	{
		Success,
		Fail,
		TimedOut,
		NotExited
	}

	public class Step
	{
		public string name;

		public string updateNotice;

		public bool complete;

		public OnStepDelegate onComplete;

		public float weight;

		public float percentComplete;

		public float startTime;

		public float endTime;

		public ExitCondition result;

		public string bundlePath;

		public Step(string name, OnStepDelegate onComplete, string notice)
		{
			this.name = name;
			updateNotice = notice;
			complete = false;
			this.onComplete = onComplete;
			weight = 1f;
			percentComplete = 0f;
			startTime = Time.time;
			result = ExitCondition.NotExited;
		}
	}

	public delegate void OnCompleteDelegate(ExitCondition exit, string error, object userData);

	public delegate void OnCancelDelegate(ExitCondition exit, object userData);

	public delegate void OnStepDelegate(string step, bool success, TransactionMonitor transaction);

	private static int transIdSeed;

	private string id = GenerateTransactionId();

	private float weight;

	public float timeout;

	private WaitWatcherMgr.WaitWatcher watcherRef;

	protected object userData;

	protected List<Step> steps;

	protected int completed;

	protected float percentComplete;

	protected List<TransactionMonitor> children;

	public TransactionMonitor parent;

	public float startTime;

	public float endTime;

	public ExitCondition result;

	private bool allStepsCompleted;

	private static bool TransactionLoggingEnabled = true;

	private static List<TransactionMonitor> transactionHistory = new List<TransactionMonitor>();

	public string Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public float Weight
	{
		get
		{
			return weight;
		}
		set
		{
			weight = value;
			recalcPercentComplete();
		}
	}

	public WaitWatcherMgr.WaitWatcher WatcherRef
	{
		set
		{
			watcherRef = value;
		}
	}

	public object UserData
	{
		get
		{
			return userData;
		}
	}

	public IEnumerable<string> OpenSteps
	{
		get
		{
			foreach (Step s in steps)
			{
				if (!s.complete)
				{
					yield return s.name;
				}
			}
		}
	}

	public IEnumerable<string> CompletedSteps
	{
		get
		{
			foreach (Step s in steps)
			{
				if (s.complete)
				{
					yield return s.name;
				}
			}
		}
	}

	public List<Step> Steps
	{
		get
		{
			return steps;
		}
	}

	public int StepCount
	{
		get
		{
			return steps.Count;
		}
	}

	public float PercentComplete
	{
		get
		{
			return percentComplete;
		}
	}

	public List<TransactionMonitor> Transactions
	{
		get
		{
			return children;
		}
	}

	public bool IsCompleted
	{
		get
		{
			return completed >= steps.Count + children.Count;
		}
	}

	public static List<TransactionMonitor> TransactionHistory
	{
		get
		{
			return transactionHistory;
		}
	}

	public event OnCompleteDelegate onComplete;

	public event OnCancelDelegate onCancel;

	public TransactionMonitor(OnCompleteDelegate onComplete, float timeout, object userData)
	{
		this.onComplete = onComplete;
		this.timeout = timeout * 3.0f;  // CSP - added * 3 to make longer for testing.
		this.userData = userData;
		steps = new List<Step>();
		children = new List<TransactionMonitor>();
		completed = 0;
		parent = null;
		percentComplete = 0f;
		weight = 1f;
		startTime = Time.time;
		result = ExitCondition.NotExited;
	}

	public static string GenerateTransactionId()
	{
		int num = ++transIdSeed;
		return "Transaction_" + num.ToString();
	}

	public bool HasStep(string step)
	{
		return FindStep(step) != null;
	}

	public void AddChild(TransactionMonitor child)
	{
		if (child.parent != null && child.parent != this)
		{
			child.parent.RemoveChild(child);
		}
		child.parent = this;
		children.Add(child);
		recalcPercentComplete();
	}

	public void RemoveChild(TransactionMonitor child)
	{
		if (children.Contains(child))
		{
			child.parent = null;
			children.Remove(child);
			recalcPercentComplete();
			checkForCompletion();
		}
	}

	public void AddStep(string name)
	{
		AddStep(name, null, string.Empty);
	}

	public void AddStep(string name, string notice)
	{
		AddStep(name, null, notice);
	}

	public void AddStep(string name, OnStepDelegate onComplete)
	{
		AddStep(name, onComplete, string.Empty);
	}

	public void AddStep(string name, OnStepDelegate onComplete, string notice)
	{
		Step step = FindStep(name);
		if (step != null)
		{
			throw new Exception("Step <" + name + "> already exists");
		}
		step = new Step(name, onComplete, notice);
		steps.Add(step);
		recalcPercentComplete();
	}

	public void AddStepDelegate(string name, OnStepDelegate onComplete)
	{
		Step step = FindStep(name);
		if (step == null)
		{
			throw new Exception("Step <" + name + "> does not exist: cannot assign completion delegate");
		}
		step.onComplete = (OnStepDelegate)Delegate.Combine(step.onComplete, onComplete);
	}

	public void AddStepBundle(string name, string bundlePath)
	{
		Step step = FindStep(name);
		if (step == null)
		{
			throw new Exception("Step <" + name + "> does not exist: cannot assign bundle to monitor");
		}
		step.bundlePath = bundlePath;
	}

	private Step FindStep(string name)
	{
		foreach (Step step in steps)
		{
			if (step.name == name)
			{
				return step;
			}
		}
		return null;
	}

	public void CompleteStep(string name)
	{
		CompleteStep(name, string.Empty);
	}

	public void CompleteStep(string name, string updateNotice)
	{
		Step step = FindStep(name);
		if (step != null)
		{
			if (!step.complete)
			{
				step.complete = true;
				step.endTime = Time.time;
				step.result = ExitCondition.Success;
				step.percentComplete = 1f;
				if (updateNotice.Length > 0)
				{
					step.updateNotice = updateNotice;
				}
				if (step.onComplete != null)
				{
					step.onComplete(step.name, true, this);
					step.onComplete = null;
				}
				completed++;
				recalcPercentComplete();
				AppShell.Instance.EventMgr.Fire(null, new WaitWatcherEvent.StepComplete(this, step.name, step.updateNotice));
			}
		}
		else
		{
			CspUtils.DebugLog("CompleteStep did not find <" + name + ">");
		}
		checkForCompletion();
	}

	public void SetStepWeight(string name, float newWeight)
	{
		Step step = FindStep(name);
		if (step == null)
		{
			CspUtils.DebugLog("Transaction Monitor couldn't set weight on step '" + name + "' in transaction '" + Id + "'.");
			return;
		}
		step.weight = newWeight;
		recalcPercentComplete();
	}

	public void CompleteChildTransaction(TransactionMonitor child)
	{
		if (!children.Contains(child))
		{
			CspUtils.DebugLog("CompleteChildTransaction did not find the requested child among its children.");
			return;
		}
		completed++;
		child.MarkComplete();
		recalcPercentComplete();
		checkForCompletion();
	}

	protected void checkForCompletion()
	{
		allStepsCompleted = (completed >= steps.Count + children.Count);
		CspUtils.DebugLog("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! allStepsCompleted for " + this.id + " = " + allStepsCompleted);
		CspUtils.DebugLog("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! DUMP:");
		DumpStatus();
		//if (watcherRef == null)
		//	CspUtils.DebugLog("watcherRef == null");
		if (allStepsCompleted && watcherRef == null)
		{
			CspUtils.DebugLog("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! CloseTransaction() for " + this.id);
			CloseTransaction();
		}
	}

	public void CloseTransaction()
	{
		endTime = Time.time;
		result = ExitCondition.Success;
		AppShell.Instance.EventMgr.Fire(null, new WaitWatcherEvent.TransactionComplete(this, ExitCondition.Success, string.Empty));
		if (this.onComplete != null)
		{
			this.onComplete(ExitCondition.Success, string.Empty, userData);
			this.onComplete = null;
		}
		if (parent != null)
		{
			parent.CompleteChildTransaction(this);
		}
	}

	public void FailStep(string name, string error)
	{
		Step step = FindStep(name);
		if (step != null && step.onComplete != null)
		{
			step.onComplete(step.name, false, this);
			step.onComplete = null;
			step.result = ExitCondition.Fail;
			step.endTime = Time.time;
		}
		if (this.onCancel != null)
		{
			this.onCancel(ExitCondition.Fail, userData);
		}
		if (this.onComplete != null)
		{
			this.onComplete(ExitCondition.Fail, error, userData);
			this.onComplete = null;
		}
		AppShell.Instance.EventMgr.Fire(null, new WaitWatcherEvent.TransactionComplete(this, ExitCondition.Fail, error));
		if (parent != null)
		{
			parent.Fail(error);
		}
	}

	public void Fail(string error)
	{
		endTime = Time.time;
		result = ExitCondition.Fail;
		if (this.onCancel != null)
		{
			this.onCancel(ExitCondition.Fail, userData);
		}
		if (this.onComplete != null)
		{
			this.onComplete(ExitCondition.Fail, error, userData);
			this.onComplete = null;
		}
		AppShell.Instance.EventMgr.Fire(null, new WaitWatcherEvent.TransactionComplete(this, ExitCondition.Fail, error));
		if (parent != null)
		{
			parent.Fail(error);
		}
	}

	public bool Update()
	{
		if (this.onComplete == null)
		{
			CspUtils.DebugLog("Transaction has completed but is still being updated");
			return false;
		}
		timeout -= Time.deltaTime;
		if (timeout < 0f)
		{
			endTime = Time.time;
			result = ExitCondition.TimedOut;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Transaction <" + Id + "> Timed Out:");
			foreach (Step step in steps)
			{
				stringBuilder.AppendLine("  step <" + step.name + "> = " + ((!step.complete) ? "open" : "completed"));
			}
			CspUtils.DebugLog(stringBuilder.ToString());
			this.onComplete(ExitCondition.TimedOut, "Timed Out", userData);
		}
		return true;
	}

	public void UpdateMonitoredTransaction()
	{
		foreach (Step step in steps)
		{
			if (!string.IsNullOrEmpty(step.bundlePath) && step.percentComplete != 1f)
			{
				float num = AppShell.Instance.BundleLoader.CheckAssetBundleSize(step.bundlePath);
				if (num > 1f)
				{
					float num2 = AppShell.Instance.BundleLoader.CheckAssetBundleRemaining(step.bundlePath);
					float num3 = step.percentComplete = (num - num2) / num;
				}
			}
		}
		foreach (TransactionMonitor child in children)
		{
			child.UpdateMonitoredTransaction();
		}
	}

	public bool IsStepCompleted(string name)
	{
		foreach (Step step in steps)
		{
			if (step.name == name)
			{
				return step.complete;
			}
		}
		throw new Exception("Unknown step <" + name + ">");
	}

	protected void recalcPercentComplete()
	{
		percentComplete = 0f;
		float num = 0f;
		float num2 = 0f;
		foreach (Step step in steps)
		{
			num += step.weight;
			num2 += ((!step.complete) ? (step.weight * step.percentComplete) : step.weight);
		}
		foreach (TransactionMonitor child in children)
		{
			num += child.Weight;
			num2 += child.PercentComplete * child.Weight;
		}
		if (num >= 1f)
		{
			percentComplete = num2 / num;
		}
		if (parent != null)
		{
			parent.recalcPercentComplete();
		}
	}

	public void MarkComplete()
	{
		percentComplete = 1f;
	}

	public override string ToString()
	{
		return "Transaction_" + Id;
	}

	public void DumpStatus()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Transaction: " + Id);
		stringBuilder.AppendLine();
		stringBuilder.Append(" open: ");
		foreach (string openStep in OpenSteps)
		{
			stringBuilder.Append(openStep);
			stringBuilder.Append("  ");
		}
		stringBuilder.AppendLine();
		stringBuilder.Append(" completed: ");
		foreach (string completedStep in CompletedSteps)
		{
			stringBuilder.Append(completedStep);
			stringBuilder.Append("  ");
		}
		stringBuilder.AppendLine();
		stringBuilder.Append("children: ");
		foreach (TransactionMonitor child in children)
		{
			stringBuilder.Append(child.Id);
			stringBuilder.Append("(");
			stringBuilder.Append(child.PercentComplete * 100f);
			stringBuilder.Append("%)  ");
		}
		stringBuilder.AppendLine();
		CspUtils.DebugLog(stringBuilder.ToString());
		foreach (TransactionMonitor child2 in children)
		{
			child2.DumpStatus();
		}
	}

	public static void DumpTransactionStatus(string step, bool success, TransactionMonitor transaction)
	{
	}

	public static TransactionMonitor CreateTransactionMonitor(OnCompleteDelegate onComplete, float timeOut, object userData)
	{
		TransactionMonitor item = new TransactionMonitor(onComplete, timeOut, userData);
		if (TransactionLoggingEnabled)
		{
			transactionHistory.Add(item);
		}
		return item;
	}

	public static TransactionMonitor CreateTransactionMonitor(string id, OnCompleteDelegate onComplete, float timeOut, object userData)
	{
		TransactionMonitor transactionMonitor = new TransactionMonitor(onComplete, timeOut, userData);
		transactionMonitor.Id = id;
		if (TransactionLoggingEnabled)
		{
			transactionHistory.Add(transactionMonitor);
		}
		return transactionMonitor;
	}
}
