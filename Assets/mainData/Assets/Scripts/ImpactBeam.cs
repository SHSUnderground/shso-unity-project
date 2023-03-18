using System;
using System.Collections.Generic;
using UnityEngine;

public class ImpactBeam : ImpactMelee
{
	protected Component[] laserBeamScripts;

	protected EffectSequence geometryHitEffect;

	protected EffectSequence noHitEffect;

	private static bool JCLDEBUG;

	public override void ImpactBegin(CharacterGlobals newCharGlobals, CombatController.AttackData newAttackData, CombatController newTargetCombatController)
	{
		base.ImpactBegin(newCharGlobals, newAttackData, newTargetCombatController);
		if (!(createdEffect != null))
		{
			return;
		}
		laserBeamScripts = createdEffect.GetComponentsInChildren(typeof(LaserBeamScript), true);
		if (laserBeamScripts != null)
		{
			Component[] array = laserBeamScripts;
			for (int i = 0; i < array.Length; i++)
			{
				LaserBeamScript laserBeamScript = (LaserBeamScript)array[i];
				Utils.ActivateTree(laserBeamScript.gameObject, false);
			}
		}
		else
		{
			CspUtils.DebugLog("Beam attack (" + newAttackData.attackName + ") could not find Laser Beam Script, will not scale appearance properly");
		}
	}

	protected static int CompareRaycastHitByDistance(RaycastHit x, RaycastHit y)
	{
		if (x.distance > y.distance)
		{
			return 1;
		}
		if (y.distance > x.distance)
		{
			return -1;
		}
		return 0;
	}

	public override void ImpactUpdate(float elapsedTime)
	{
		base.ImpactUpdate(elapsedTime);
		if (JCLDEBUG)
		{
			CspUtils.DebugLog("ImpactBeam ImpactUpdate");
		}
		if (createdEffect != null && fired && !colliderActive)
		{
			UnityEngine.Object.Destroy(createdEffect);
		}
		if (!colliderActive)
		{
			return;
		}
		if (JCLDEBUG)
		{
			CspUtils.DebugLog("ImpactBeam ImpactUpdate 2");
		}
		float num = 0f;
		float num2 = attackData.maximumRange;
		if (charGlobals != null && charGlobals.characterController != null)
		{
			num2 += charGlobals.characterController.radius;
		}
		RaycastHit hitInfo = default(RaycastHit);
		bool flag = false;
		Transform transform = colliderObject.transform.parent.transform;
		if (colliderDisabled || (impactData.impactEndTime > 0f && elapsedTime >= impactData.impactEndTime))
		{
			num = 1f;
		}
		else if (Physics.Raycast(transform.position, transform.forward, out hitInfo, num2, -275280407))
		{
			flag = true;
			num = hitInfo.distance;
		}
		else
		{
			num = num2;
		}
		if (JCLDEBUG)
		{
			CspUtils.DebugLog("ImpactBeam ImpactUpdate 3 " + num);
		}
		if (num > 1f && impactData.maximumTargetsHit > 0)
		{
			RaycastHit[] array = Physics.RaycastAll(transform.position, transform.forward, num, 2101248);
			List<RaycastHit> list = new List<RaycastHit>();
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit item = array2[i];
				CombatController combatController = item.collider.gameObject.GetComponent(typeof(CombatController)) as CombatController;
				if (!(combatController == null) && !list.Contains(item) && combatController.faction != CombatController.Faction.Neutral && ((impactData.hitsFriends && combatController.faction == charGlobals.combatController.faction) || (impactData.hitsEnemies && combatController.faction != charGlobals.combatController.faction)))
				{
					list.Add(item);
					if (JCLDEBUG)
					{
						CspUtils.DebugLog(list.Count + " hit " + item.collider.gameObject.name + " distance " + item.distance);
					}
				}
			}
			if (list.Count > 0)
			{
				list.Sort(CompareRaycastHitByDistance);
				if (list.Count >= impactData.maximumTargetsHit)
				{
					if (JCLDEBUG)
					{
						CspUtils.DebugLog("setting scale to hit index " + (impactData.maximumTargetsHit - 1));
					}
					num = list[impactData.maximumTargetsHit - 1].distance;
				}
				else
				{
					if (JCLDEBUG)
					{
						CspUtils.DebugLog("setting scale to last hit");
					}
					num = list[list.Count - 1].distance;
				}
			}
		}
		Transform transform2 = colliderObject.transform;
		Vector3 localScale = colliderObject.transform.localScale;
		float x = localScale.x;
		Vector3 localScale2 = colliderObject.transform.localScale;
		transform2.localScale = new Vector3(x, localScale2.y, num + 1f);
		colliderObject.transform.localPosition = new Vector3(0f, 0f, (num + 1f) / 2f);
		float num3 = num;
		if (flag && attackData.alwaysRenderMaxBeam)
		{
			num3 = hitInfo.distance;
			num3 = ExtendBeamIntoGeometry(num3, 1f, transform.forward, hitInfo.normal);
			num3 = Mathf.Min(num3, num2);
			PlayGeometryHitEffect(hitInfo, transform.forward);
		}
		else if (!flag)
		{
			if (JCLDEBUG)
			{
				CspUtils.DebugLog("ImpactBeam ImpactUpdate 4 " + num3);
			}
			if (attackData.alwaysRenderMaxBeam)
			{
				num3 = num2;
				if (JCLDEBUG)
				{
					CspUtils.DebugLog("ImpactBeam ImpactUpdate 5 " + num3);
				}
			}
			if (geometryHitEffect != null)
			{
				geometryHitEffect.Cancel();
			}
			PlayNoHitEffect(transform.position + transform.forward * num3, transform.forward);
		}
		if (!(createdEffect != null) || laserBeamScripts == null)
		{
			return;
		}
		Component[] array3 = laserBeamScripts;
		for (int j = 0; j < array3.Length; j++)
		{
			LaserBeamScript laserBeamScript = (LaserBeamScript)array3[j];
			if (!laserBeamScript.gameObject.active)
			{
				Utils.ActivateTree(laserBeamScript.gameObject, true);
			}
			if (JCLDEBUG)
			{
				CspUtils.DebugLog("ImpactBeam ImpactUpdate 6 " + num3);
			}
			laserBeamScript.BeamLength = num3;
		}
	}

	public override void ImpactEnd()
	{
		if (geometryHitEffect != null)
		{
			geometryHitEffect.Cancel();
		}
		if (noHitEffect != null)
		{
			noHitEffect.Cancel();
		}
		base.ImpactEnd();
	}

	private void PlayGeometryHitEffect(RaycastHit location, Vector3 beamDirection)
	{
		if (noHitEffect != null)
		{
			noHitEffect.Cancel();
		}
		if (geometryHitEffect == null && !string.IsNullOrEmpty(attackData.beamHitGeometryEffect))
		{
			charGlobals.effectsList.TryOneShot(attackData.beamHitGeometryEffect, null, charGlobals, out geometryHitEffect);
		}
		if (geometryHitEffect != null)
		{
			geometryHitEffect.transform.position = location.point;
			Vector3 rhs = Vector3.Cross(beamDirection, location.normal);
			Vector3 forward = Vector3.Cross(location.normal, rhs);
			geometryHitEffect.transform.rotation = Quaternion.LookRotation(forward);
		}
	}

	private void PlayNoHitEffect(Vector3 point, Vector3 beamDirection)
	{
		if (noHitEffect == null && !string.IsNullOrEmpty(attackData.beamHitNothingEffect))
		{
			charGlobals.effectsList.TryOneShot(attackData.beamHitNothingEffect, null, charGlobals, out noHitEffect);
		}
		if (noHitEffect != null)
		{
			noHitEffect.transform.position = point;
			noHitEffect.transform.rotation = Quaternion.LookRotation(beamDirection);
		}
	}

	private float ExtendBeamIntoGeometry(float currentLength, float beamWidth, Vector3 beamDir, Vector3 geomNormal)
	{
		float num = Vector3.Angle(beamDir, geomNormal);
		if (num > 89f)
		{
			return currentLength;
		}
		float f = num * ((float)Math.PI / 180f);
		float num2 = 0.5f * beamWidth * Mathf.Sin(f) / Mathf.Cos(f);
		return currentLength + num2;
	}
}
