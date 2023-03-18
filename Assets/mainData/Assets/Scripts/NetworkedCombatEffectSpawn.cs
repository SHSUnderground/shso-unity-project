using UnityEngine;

public class NetworkedCombatEffectSpawn : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string effectName = string.Empty;

	public bool makeEffectChild;

	protected void Start()
	{
		if (effectName == null && effectName == string.Empty)
		{
			return;
		}
		CharacterGlobals component = base.transform.root.GetComponent<CharacterGlobals>();
		if (component == null || component.combatController == null || component.networkComponent == null)
		{
			return;
		}
		if (component.networkComponent.IsOwner())
		{
			component.combatController.createEffect(effectName, null);
		}
		if (!makeEffectChild)
		{
			return;
		}
		GameObject gameObject = GameObject.Find(effectName + "(Clone)");
		if (gameObject == null)
		{
			CspUtils.DebugLog("Networked effect was not found, could not attach!");
			return;
		}
		Vector3 localPosition = gameObject.transform.localPosition;
		Quaternion localRotation = gameObject.transform.localRotation;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = localPosition;
		gameObject.transform.localRotation = localRotation;
		ReferenceCombatController component2 = gameObject.GetComponent<ReferenceCombatController>();
		if (component2 != null)
		{
			component2.SetReferenceObject(base.transform.root.gameObject);
		}
	}
}
