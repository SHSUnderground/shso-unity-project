using System;
using UnityEngine;

public class CombatEffectMessage : CombatEffectBase
{
	public new void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		_sourceCombatController = sourceCombatController;
		SendCombatEffectMessage(combatEffectData.startMessage, combatEffectData.startMessageArgs);
	}

	protected override void ReleaseEffect()
	{
		SendCombatEffectMessage(combatEffectData.endMessage, combatEffectData.endMessageArgs);
		base.ReleaseEffect();
	}

	protected void SendCombatEffectMessage(string msgCall, string[] msgArgs)
	{
		if (msgCall != null)
		{
			if (msgArgs != null)
			{
				base.transform.parent.gameObject.SendMessage(msgCall, msgArgs, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				base.transform.parent.gameObject.SendMessage(msgCall, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public static void BuildCombatEffectMessage(string msgFull, out string msgCall, out string[] msgArgs)
	{
		msgCall = string.Empty;
		msgArgs = null;
		if (msgFull == string.Empty)
		{
			return;
		}
		int num = msgFull.IndexOf('(');
		if (num < 0)
		{
			num = msgFull.Length;
		}
		msgCall = msgFull.Substring(0, num);
		msgCall = msgCall.Trim();
		if (msgCall.Length <= 0)
		{
			msgCall = null;
			return;
		}
		int num2 = msgFull.LastIndexOf(')');
		if (num2 < 0)
		{
			num2 = msgFull.Length;
		}
		if (num2 > num)
		{
			num++;
			string text = msgFull.Substring(num, num2 - num);
			msgArgs = text.Split(new char[1]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < msgArgs.Length; i++)
			{
				msgArgs[i] = msgArgs[i].Trim();
			}
		}
	}
}
