using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntitlementsViewerWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private SHSStyle headerStyle;

	private Vector2 scrollPos;

	public EntitlementsViewerWindow(string name)
		: base(name, null)
	{
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		GUILayout.BeginArea(new Rect(30f, 30f, 700f, base.rect.height - 50f));
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		GUILayout.Label("Entitlements", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		GUILayout.Label("NOTE: THIS DOES NOT CHANGE YOUR SERVER ENTITLEMENTS...", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		GUILayout.Label("THIS IS FOR TESTING UI AND CLIENT STATE ONLY.", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		GUILayout.Label("USE GMAN FOR CHANGING ENTITLEMENTS ON BOTH SERVER/CLIENT IN UNISON", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		GUILayout.BeginVertical();
		GUILayout.Space(20f);
		GUILayout.BeginHorizontal(GUILayout.Width(700f));
		GUILayout.Label("LOCATION", headerStyle.UnityStyle, GUILayout.Width(90f));
		GUILayout.Label("NAME", headerStyle.UnityStyle, GUILayout.Width(190f));
		GUILayout.Label("PLAYER", headerStyle.UnityStyle, GUILayout.Width(75f));
		GUILayout.Label("CHECK", headerStyle.UnityStyle, GUILayout.Width(75f));
		GUILayout.EndHorizontal();
		foreach (KeyValuePair<Entitlements.EntitlementFlagEnum, Entitlements.Entitlement> item in Enumerable.OrderBy<KeyValuePair<Entitlements.EntitlementFlagEnum, Entitlements.Entitlement>, Entitlements.EntitlementFlagEnum>(Singleton<Entitlements>.instance.EntitlementsSet, delegate(KeyValuePair<Entitlements.EntitlementFlagEnum, Entitlements.Entitlement> x)
		{
			return x.Key;
		}))
		{
			Entitlements.Entitlement value = item.Value;
			if (value.Type == Entitlements.EntitlementLocationTypeEnum.Aggregate)
			{
				GUILayout.BeginHorizontal(GUILayout.Width(700f));
				GUILayout.Label(value.Type.ToString(), headerStyle.UnityStyle, GUILayout.Width(90f));
				GUILayout.Label(value.EntitlementFlag.ToString(), headerStyle.UnityStyle, GUILayout.Width(190f));
				GUILayout.Label((!value.PermissionCheck()) ? "DENY" : "ALLOW", headerStyle.UnityStyle, GUILayout.Width(75f));
				GUILayout.EndHorizontal();
				foreach (Entitlements.EntitlementFlagEnum subFlag in ((Entitlements.AggregateEntitlement)value).SubFlags)
				{
					GUILayout.BeginHorizontal(GUILayout.Width(300f));
					GUILayout.Space(75f);
					GUILayout.Label(subFlag.ToString(), headerStyle.UnityStyle, GUILayout.Width(190f));
					GUILayout.EndHorizontal();
				}
				GUILayout.Space(4f);
			}
			else
			{
				GUILayout.BeginHorizontal(GUILayout.Width(700f));
				GUILayout.Label(value.Type.ToString(), headerStyle.UnityStyle, GUILayout.Width(90f));
				GUILayout.Label(value.EntitlementFlag.ToString(), headerStyle.UnityStyle, GUILayout.Width(190f));
				Color color = GUI.color;
				if (value.PlayerPermission.HasValue)
				{
					GUI.color = Color.green;
				}
				else
				{
					GUI.color = Color.yellow;
				}
				GUILayout.Label((!value.PlayerPermission.HasValue) ? value.DefaultPermission.ToString() : value.PlayerPermission.Value.ToString(), headerStyle.UnityStyle, GUILayout.Width(75f));
				GUI.color = color;
				GUILayout.Label((!value.PermissionCheck()) ? "DENY" : "ALLOW", headerStyle.UnityStyle, GUILayout.Width(75f));
				if (GUILayout.Button("Toggle", GUILayout.Width(50f)))
				{
					bool flag = (!value.PlayerPermission.HasValue) ? value.DefaultPermission : value.PlayerPermission.Value;
					Singleton<Entitlements>.instance.ConfigureEntitlement(value.EntitlementFlag, !flag);
				}
				GUILayout.Space(4f);
				GUILayout.EndHorizontal();
			}
		}
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	public override void OnShow()
	{
		base.OnShow();
		headerStyle = GUIManager.Instance.StyleManager.GetStyle("DebugConsoleEventHeader");
		scrollPos = Vector2.zero;
	}
}
