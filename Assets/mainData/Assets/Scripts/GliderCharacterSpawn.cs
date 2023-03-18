using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GliderCharacterSpawn : CharacterSpawn
{
	protected enum AnimState
	{
		unfurl,
		wait,
		furl,
		finished
	}

	public GameObject GliderStartPoint;

	public GameObject GliderEndPoint;

	public float heightOffset;

	public GameObject WarningDisplay;

	public GameObject EditorDisplay;

	public bool DestroyAtEndOfPath;

	public float AttackDelay = 1f;

	public float GliderSpeed = 1f;

	public float LeadIn;

	public float LeadOut;

	public float WarningUnfurlSpeed = 24f;

	public float WarningHoldTime = 0.5f;

	public string GliderAnimation = "movement_idle";

	public string GliderCombatEffect = "gliderDefaultAttack";

	[HideInInspector]
	public Vector3 pathStart;

	[HideInInspector]
	public Vector3 pathEnd;

	[HideInInspector]
	public Vector3 warningStart;

	[HideInInspector]
	public Vector3 warningEnd;

	[HideInInspector]
	public Vector3 pathVec;

	[HideInInspector]
	public float pathLength;

	protected AnimState warningMode = AnimState.furl;

	protected TrailRenderer warningTrail;

	protected GameObject sequenceSource;

	protected GameObject sequenceInstance;

	protected EffectSequence playingSequence;

	protected ParticleRenderer landingStrip;

	protected override void FinalSpawnSetup(GameObject newCharacter, CharacterSpawnData spawnData)
	{
		CharacterGlobals characterGlobals = newCharacter.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
		if (characterGlobals != null)
		{
			characterGlobals.motionController.setSpawnAnimation(null);
		}
		newCharacter.transform.position = pathStart;
		if (WarningDisplay != null)
		{
			StartCoroutine(WarningUpdate(characterGlobals));
		}
	}

	public override void Awake()
	{
		if (EditorDisplay != null)
		{
			Object.Destroy(EditorDisplay);
		}
		SpawnName = CharacterName;
		spawnedCharacters = new List<CharacterGlobals>();
		pathStart = GliderStartPoint.transform.position;
		pathEnd = GliderEndPoint.transform.position;
		warningStart = GliderStartPoint.transform.position;
		warningEnd = GliderEndPoint.transform.position;
		pathVec = default(Vector3);
		pathLength = 0f;
		GetPathPoints(ref warningStart, ref warningEnd, ref pathVec, ref pathLength);
		warningTrail = GetComponentInChildren<TrailRenderer>();
		EffectSequence componentInChildren = GetComponentInChildren<EffectSequence>();
		if (warningTrail != null)
		{
			warningTrail.time = 0f;
		}
		if (componentInChildren != null)
		{
			sequenceSource = componentInChildren.gameObject;
		}
		landingStrip = GetComponentInChildren<ParticleRenderer>();
		if (landingStrip != null)
		{
			landingStrip.transform.rotation = Quaternion.LookRotation(pathVec) * Quaternion.Euler(0f, 90f, 0f);
			landingStrip.transform.position = (warningStart + warningEnd) * 0.5f;
			Vector3 localScale = landingStrip.transform.localScale;
			localScale.x = pathLength * (1f - (LeadIn + LeadOut)) * 0.25f;
			landingStrip.transform.localScale = localScale;
			landingStrip.enabled = false;
		}
	}

	protected IEnumerator WarningUpdate(CharacterGlobals cg)
	{
		WarningDisplay.transform.rotation = Quaternion.LookRotation(pathVec);
		float warningPathLength = pathLength * (1f - (LeadIn + LeadOut));
		float warningUnfurlSpeed = WarningUnfurlSpeed / warningPathLength;
		float warningTime = 0f;
		warningMode = AnimState.unfurl;
		if (warningTrail != null)
		{
			warningTrail.time = 1f / warningUnfurlSpeed + WarningHoldTime;
			warningTrail.enabled = true;
		}
		if (landingStrip != null)
		{
			landingStrip.enabled = true;
		}
		if (sequenceSource != null)
		{
			if (sequenceInstance != null)
			{
				playingSequence.StopSequence(false);
				Object.Destroy(sequenceInstance);
			}
			sequenceInstance = (Object.Instantiate(sequenceSource) as GameObject);
			playingSequence = sequenceInstance.GetComponentInChildren<EffectSequence>();
			playingSequence.Initialize(base.gameObject, null, null);
			sequenceInstance.transform.position = warningStart;
			playingSequence.StartSequence();
		}
		yield return 0;
		if (cg != null)
		{
			BehaviorGliderSpawn gliderBehavior = cg.behaviorManager.forceChangeBehavior(typeof(BehaviorGliderSpawn)) as BehaviorGliderSpawn;
			gliderBehavior.Initialize(this);
		}
		while (warningMode != AnimState.finished)
		{
			switch (warningMode)
			{
			case AnimState.unfurl:
				WarningDisplay.transform.position = warningStart + pathVec * warningPathLength * warningTime;
				WarningDisplay.transform.localScale = new Vector3(1f, 1f, 1f);
				if (sequenceInstance != null)
				{
					sequenceInstance.transform.position = warningStart + pathVec * warningPathLength * warningTime;
				}
				warningTime += warningUnfurlSpeed * Time.deltaTime;
				if (warningTime >= 1f)
				{
					warningTime = 0f;
					warningMode = AnimState.wait;
				}
				break;
			case AnimState.wait:
				warningTime += Time.deltaTime;
				if (warningTime > WarningHoldTime)
				{
					warningTime = 0f;
					warningMode = AnimState.furl;
				}
				break;
			case AnimState.furl:
				warningTime += warningUnfurlSpeed * Time.deltaTime;
				if (warningTime >= 1f)
				{
					warningTime = 0f;
					warningMode = AnimState.finished;
					WarningDisplay.transform.localScale = new Vector3(0f, 0f, 0f);
					if (warningTrail != null)
					{
						warningTrail.time = 0f;
					}
					if (landingStrip != null)
					{
						landingStrip.enabled = false;
					}
					if (sequenceInstance != null)
					{
						playingSequence.StopSequence(false);
						Object.Destroy(sequenceInstance);
						playingSequence = null;
						sequenceInstance = null;
					}
				}
				break;
			}
			yield return 0;
		}
	}

	protected void GetPathPoints(ref Vector3 startPoint, ref Vector3 endPoint, ref Vector3 motionVec, ref float vecLength)
	{
		Vector3 a = startPoint;
		motionVec = endPoint - startPoint;
		vecLength = motionVec.magnitude;
		motionVec /= vecLength;
		if (LeadIn > 0f && LeadIn < 1f)
		{
			startPoint += motionVec * vecLength * LeadIn;
		}
		if (LeadOut > 0f && LeadOut < 1f)
		{
			endPoint = a + motionVec * vecLength * (1f - LeadOut);
		}
	}

	private void OnDrawGizmos()
	{
		if (GliderStartPoint != null && GliderEndPoint != null)
		{
			Vector3 startPoint = GliderStartPoint.transform.position;
			Vector3 endPoint = GliderEndPoint.transform.position;
			Vector3 motionVec = default(Vector3);
			float vecLength = 0f;
			GetPathPoints(ref startPoint, ref endPoint, ref motionVec, ref vecLength);
			if (LeadIn > 0f && LeadIn < 1f)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawLine(GliderStartPoint.transform.position, startPoint);
			}
			if (LeadOut > 0f && LeadOut < 1f)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawLine(endPoint, GliderEndPoint.transform.position);
			}
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(startPoint, endPoint);
		}
	}

	public override void CloneSpawner(CharacterSpawn target)
	{
		base.CloneSpawner(target);
		GliderCharacterSpawn gliderCharacterSpawn = target as GliderCharacterSpawn;
		if (gliderCharacterSpawn != null)
		{
			gliderCharacterSpawn.GliderStartPoint = GliderStartPoint;
			gliderCharacterSpawn.GliderEndPoint = GliderEndPoint;
			gliderCharacterSpawn.heightOffset = heightOffset;
			gliderCharacterSpawn.WarningDisplay = WarningDisplay;
			gliderCharacterSpawn.EditorDisplay = EditorDisplay;
			gliderCharacterSpawn.DestroyAtEndOfPath = DestroyAtEndOfPath;
			gliderCharacterSpawn.AttackDelay = AttackDelay;
			gliderCharacterSpawn.GliderSpeed = GliderSpeed;
			gliderCharacterSpawn.LeadIn = LeadIn;
			gliderCharacterSpawn.LeadOut = LeadOut;
			gliderCharacterSpawn.WarningUnfurlSpeed = WarningUnfurlSpeed;
			gliderCharacterSpawn.WarningHoldTime = WarningHoldTime;
			gliderCharacterSpawn.GliderAnimation = GliderAnimation;
			gliderCharacterSpawn.GliderCombatEffect = GliderCombatEffect;
		}
	}
}
