using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterTargetSource))]
[AddComponentMenu("Lab/Emotor/Emotor")]
public class Emotor : MonoBehaviour, IEmotor
{
	public enum OperationMode
	{
		Random,
		RandomStep,
		Sequential,
		SequentialLoop,
		SequentialStep,
		Review
	}

	public delegate void EmoteNotifyDelegate(EmotesDefinition.EmoteDefinition emoteDef);

	private GameObject _actualTarget;

	private BehaviorManager _actualTargetBM;

	public OperationMode Mode = OperationMode.Sequential;

	public float MinDelay;

	public float MaxDelay;

	public float MaxEmoteLength = 4f;

	public bool ReviewNext;

	public string[] EmoteSet = new string[0];

	public int CurrentEmoteIndex = -1;

	public int CurrentEmoteId = -1;

	public string CurrentEmoteName = string.Empty;

	public string CurrentStatus = "Disabled";

	private bool _runMotor;

	private bool _motorRunning;

	private CharacterTargetSource _targetSource;

	public event EmoteNotifyDelegate OnEmoteStart;

	public event EmoteNotifyDelegate OnEmoteStop;

	public void Emotor_Start()
	{
		base.enabled = true;
	}

	public void Emotor_Stop()
	{
		base.enabled = false;
	}

	private void OnEnable()
	{
		_targetSource = (_targetSource ?? Utils.GetComponent<CharacterTargetSource>(this));
		_runMotor = true;
		if (!_motorRunning)
		{
			StartCoroutine(Run());
		}
	}

	private void OnDisable()
	{
		_runMotor = false;
	}

	private IEnumerator Run()
	{
		_motorRunning = true;
		try
		{
			ChangeStatus("Starting up");
			while (EmotesDefinition.Instance == null)
			{
				yield return new WaitForEndOfFrame();
			}
			LoadEmoteSet();
			if (CurrentEmoteIndex >= EmoteSet.Length)
			{
				CurrentEmoteIndex = -1;
			}
			while (true)
			{
				if (!_runMotor)
				{
					yield break;
				}
				if (_targetSource.Character != _actualTarget && _targetSource.Character != null)
				{
					_actualTarget = null;
				}
				if (_actualTarget == null)
				{
					while (_runMotor && _actualTarget == null)
					{
						_actualTarget = Utils.GetComponent<CharacterTargetSource>(this).Character;
						if (_actualTarget != null)
						{
							_actualTargetBM = Utils.GetComponent<BehaviorManager>(_actualTarget);
						}
						if (_actualTargetBM == null)
						{
							yield return new WaitForEndOfFrame();
						}
						ChangeStatus("Waiting for a target");
					}
				}
				ChangeStatus("Selecting");
				EmotesDefinition.EmoteDefinition emoteDef = PickNextEmote();
				UpdateCurrentEmoteState(emoteDef);
				if (CurrentEmoteIndex < 0)
				{
					ChangeStatus("Done");
					base.gameObject.active = false;
					yield break;
				}
				if (emoteDef == null)
				{
					ChangeStatus("Invalid emote (" + EmoteSet[CurrentEmoteIndex] + ")");
				}
				else
				{
					ChangeStatus("Waiting");
					yield return new WaitForEndOfFrame();
					ChangeStatus("Requesting");
					BehaviorEmote emote = RequestEmote(emoteDef, false);
					if (emote == null)
					{
						ChangeStatus("Request failed.  Forcing request");
						emote = RequestEmote(emoteDef, true);
						if (emote == null)
						{
							ChangeStatus("Forced emote request failed");
							continue;
						}
					}
					if (_actualTargetBM.getBehavior() != emote)
					{
						ChangeStatus("Waiting for previous emote to finish");
						while (_actualTargetBM.getBehavior() != emote)
						{
							yield return new WaitForEndOfFrame();
						}
					}
					ChangeStatus("Playing");
					if (this.OnEmoteStart != null)
					{
						Log("Starting emote " + emoteDef.command);
						this.OnEmoteStart(emoteDef);
					}
					float emoteStartTime = Time.time;
					while (_runMotor && _actualTargetBM.getBehavior() == emote && !HasEmoteExpired(emote, emoteStartTime))
					{
						yield return new WaitForEndOfFrame();
					}
					ChangeStatus("Finished");
					if (this.OnEmoteStop != null)
					{
						Log("Stopping emote " + emoteDef.command);
						this.OnEmoteStop(emoteDef);
					}
					if (_runMotor && MaxDelay > 0f)
					{
						yield return new WaitForSeconds(Random.Range(MinDelay, MaxDelay));
					}
					if (Mode == OperationMode.SequentialStep || Mode == OperationMode.RandomStep)
					{
						break;
					}
				}
			}
			base.gameObject.active = false;
		}
		finally
		{
			_motorRunning = false;
		}
	}

	private void ChangeStatus(string status)
	{
		if (status != CurrentStatus)
		{
			Log("Status: " + CurrentStatus);
		}
		CurrentStatus = status;
	}

	private void LoadEmoteSet()
	{
		if (EmoteSet.Length == 0)
		{
			List<string> list = new List<string>();
			foreach (EmotesDefinition.EmoteDefinition item in EmotesDefinition.Instance)
			{
				list.Add(item.command);
			}
			EmoteSet = list.ToArray();
		}
	}

	private BehaviorEmote RequestEmote(EmotesDefinition.EmoteDefinition emoteDef, bool force)
	{
		BehaviorEmote behaviorEmote = null;
		try
		{
			BehaviorBase behaviorBase = (!force) ? _actualTargetBM.requestChangeBehavior(typeof(BehaviorEmote), false) : _actualTargetBM.forceChangeBehavior(typeof(BehaviorEmote));
			behaviorEmote = (behaviorBase as BehaviorEmote);
			if (behaviorEmote == null)
			{
				return null;
			}
			if (!behaviorEmote.Initialize(emoteDef.id))
			{
				_actualTargetBM.endBehavior();
				behaviorEmote = null;
			}
		}
		catch
		{
		}
		if (behaviorEmote != null)
		{
			FacialAnimation component = Utils.GetComponent<FacialAnimation>(_actualTarget);
			if (component != null)
			{
				component.SetFacialExpression(FacialAnimation.Expression.Normal);
			}
		}
		return behaviorEmote;
	}

	private EmotesDefinition.EmoteDefinition PickNextEmote()
	{
		int num = 1;
		if (Mode == OperationMode.Random)
		{
			num = Random.Range(1, EmoteSet.Length - 1);
		}
		else if (Mode == OperationMode.Review)
		{
			if (!ReviewNext)
			{
				num = ((CurrentEmoteIndex < 0) ? (-CurrentEmoteIndex) : 0);
			}
			else
			{
				ReviewNext = false;
			}
		}
		CurrentEmoteIndex += num;
		if (Mode != OperationMode.Sequential)
		{
			CurrentEmoteIndex %= EmoteSet.Length;
		}
		else if (CurrentEmoteIndex >= EmoteSet.Length)
		{
			CurrentEmoteIndex = -1;
			return null;
		}
		return EmotesDefinition.Instance.GetEmoteByCommand(EmoteSet[CurrentEmoteIndex]);
	}

	private void UpdateCurrentEmoteState(EmotesDefinition.EmoteDefinition emoteDef)
	{
		if (CurrentEmoteIndex < 0 || emoteDef == null)
		{
			CurrentEmoteId = -1;
			CurrentEmoteName = string.Empty;
		}
		else
		{
			CurrentEmoteId = emoteDef.id;
			CurrentEmoteName = emoteDef.command;
		}
		Log(string.Format("New emote state for index/id {0}/{1}: {2}", CurrentEmoteIndex, (emoteDef == null) ? "null" : emoteDef.id.ToString(), CurrentEmoteName));
	}

	private bool HasEmoteExpired(BehaviorEmote emote, float startTime)
	{
		if (Mode == OperationMode.Review && ReviewNext)
		{
			return true;
		}
		if (!emote.allowUserInput())
		{
			return false;
		}
		if (MaxEmoteLength <= 0f)
		{
			return false;
		}
		return Time.time - startTime > MaxEmoteLength;
	}

	private void Log(string message)
	{
	}
}
