using System.Collections;
using UnityEngine;

[AddComponentMenu("Lab/Emotor/Fight Coach")]
[RequireComponent(typeof(CharacterTargetSource))]
public class FightCoach : MonoBehaviour, IEmotor
{
	public enum OperationMode
	{
		Primary,
		Secondary,
		All
	}

	private GameObject _actualTarget;

	private BehaviorManager _actualTargetBM;

	public OperationMode Mode;

	public int SecondaryIndex = 1;

	public float PostAttackDelay = 2f;

	public float OpponentSpawnDelay = 5f;

	public Transform OpponentSpawn;

	public GameObject OpponentPrefab;

	public bool HideOpponent = true;

	public string CurrentStatus = "Disabled";

	private bool _runMotor;

	private bool _motorRunning;

	private CharacterTargetSource _targetSource;

	public event Emotor.EmoteNotifyDelegate OnEmoteStart;

	public event Emotor.EmoteNotifyDelegate OnEmoteStop;

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
		GameObject opponent = null;
		Vector3 targetStart = default(Vector3);
		Vector3 targetStartForward = default(Vector3);
		try
		{
			ChangeStatus("Starting up");
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
						yield return null;
					}
					ChangeStatus("Waiting for a target");
				}
			}
			targetStart = _actualTarget.transform.position;
			targetStartForward = _actualTarget.transform.forward;
			ChangeStatus("Spawning opponent");
			opponent = (Object.Instantiate(OpponentPrefab, OpponentSpawn.position, OpponentSpawn.rotation) as GameObject);
			ChangeStatus("Waiting for spawn");
			yield return null;
			CharacterSpawnerGlue glue = Utils.GetComponent<CharacterSpawnerGlue>(opponent);
			if (glue != null)
			{
				while (glue.SpawnedCharacter == null)
				{
					yield return null;
				}
				Object.Destroy(opponent);
				opponent = glue.SpawnedCharacter;
				if (HideOpponent)
				{
					SkinnedMeshRenderer opponentSkin = Utils.GetComponent<SkinnedMeshRenderer>(opponent, Utils.SearchChildren);
					if (opponentSkin != null)
					{
						opponentSkin.enabled = false;
					}
				}
			}
			CombatController opponentCombat = Utils.GetComponent<CombatController>(opponent);
			yield return new WaitForSeconds(OpponentSpawnDelay);
			bool secondary = Mode == OperationMode.Secondary;
			int secondaryIndex = (Mode != OperationMode.Secondary) ? 1 : SecondaryIndex;
			while (_runMotor)
			{
				ChangeStatus("Attacking");
				if (this.OnEmoteStart != null)
				{
					this.OnEmoteStart(null);
				}
				BehaviorAttackBase attackBehavior = RequestAttack(opponent, secondary, secondaryIndex);
				if (attackBehavior == null)
				{
					break;
				}
				while (_runMotor && _actualTargetBM.getBehavior() is BehaviorAttackBase)
				{
					if (opponentCombat != null)
					{
						opponentCombat.restoreHealth();
					}
					yield return null;
				}
				if (!_runMotor)
				{
					break;
				}
				ChangeStatus("Moving to spawn");
				CharacterMotionController mc2 = Utils.GetComponent<CharacterMotionController>(_actualTarget);
				if (mc2 != null)
				{
					mc2.setDestination(targetStart, targetStartForward);
				}
				yield return null;
				if (!_runMotor)
				{
					break;
				}
				while (_runMotor && mc2.getVelocityFlat().magnitude > 0.01f)
				{
					yield return null;
				}
				if (!_runMotor)
				{
					break;
				}
				ChangeStatus("Finished");
				if (this.OnEmoteStop != null)
				{
					this.OnEmoteStop(null);
				}
				if (Mode != OperationMode.All)
				{
					break;
				}
				ChangeStatus("Waiting");
				yield return new WaitForSeconds(PostAttackDelay);
				if (!_runMotor)
				{
					break;
				}
				secondary = true;
				secondaryIndex++;
				CombatController controller = Utils.GetComponent<CombatController>(_actualTarget);
				if (secondaryIndex > controller.maximumSecondaryAttackChain)
				{
					break;
				}
			}
			base.gameObject.active = false;
		}
		finally
		{
			if (opponent != null)
			{
				Object.Destroy(opponent);
			}
			_motorRunning = false;
			CharacterMotionController mc = Utils.GetComponent<CharacterMotionController>(_actualTarget);
			if (mc != null)
			{
				mc.setDestination(targetStart, targetStartForward);
			}
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

	private BehaviorAttackBase RequestAttack(GameObject opponent, bool secondary, int secondaryIndex)
	{
		BehaviorAttackBase behaviorAttackBase = null;
		if (_actualTarget == null || _actualTargetBM == null)
		{
			ChangeStatus("Attack failed - No target");
			return null;
		}
		if (opponent == null)
		{
			ChangeStatus("Attack failed - No opponent");
			return null;
		}
		CombatController component = Utils.GetComponent<CombatController>(_actualTarget);
		if (component == null)
		{
			ChangeStatus("Attack failed - No controller found");
			return null;
		}
		component.SetSecondaryAttack(secondaryIndex);
		CombatController.AttackData currentAttackData = component.getCurrentAttackData(secondary, false);
		if (currentAttackData == null)
		{
			ChangeStatus("Attack failed - No attack data");
			return null;
		}
		if (!component.createAttackBehavior(opponent, currentAttackData, secondary, true))
		{
			ChangeStatus("Attack failed - Failed creating attack behavior");
			return null;
		}
		behaviorAttackBase = (_actualTargetBM.getBehavior() as BehaviorAttackBase);
		if (behaviorAttackBase == null)
		{
			ChangeStatus("Attack failed - No attack behavior found.");
		}
		return behaviorAttackBase;
	}

	private void Log(string message)
	{
	}
}
