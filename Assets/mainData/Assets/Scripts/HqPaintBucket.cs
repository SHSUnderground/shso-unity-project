using UnityEngine;

[AddComponentMenu("Hq/Hq Paint Bucket")]
public class HqPaintBucket : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected const string STR_PAINTBUCKET_PROMPT = "#hq_paint_bucket_apply_prompt";

	protected const string STR_THEME_ALREADY_APPLIED = "#hq_theme_already_applied_alert";

	protected bool hasBeenDropped;

	protected string previousTheme;

	public void Update()
	{
		if (hasBeenDropped)
		{
			return;
		}
		HqObject2 component = Utils.GetComponent<HqObject2>(base.gameObject, Utils.SearchChildren);
		if (!(component != null) || component.State != typeof(HqObject2.HqObjectPlacement))
		{
			return;
		}
		Item value = null;
		UserProfile profile = HqController2.Instance.Profile;
		ItemDefinition itemDef = null;
		if (profile != null && component.InventoryId != null)
		{
			profile.AvailableItems.TryGetValue(component.InventoryId, out value);
		}
		if (value == null)
		{
			return;
		}
		itemDef = value.Definition;
		if (itemDef != null)
		{
			if (HqController2.Instance.ActiveRoom.ThemeName == itemDef.Name)
			{
				GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, "#hq_theme_already_applied_alert", delegate
				{
				}, GUIControl.ModalLevelEnum.Default);
				AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(new string[1]
				{
					itemDef.Id
				}, CollectionResetMessage.ActionType.Add, "Items"));
				HqController2.Instance.ActiveRoom.DelItem(base.gameObject);
				Object.Destroy(base.gameObject);
			}
			else
			{
				previousTheme = HqController2.Instance.ActiveRoom.ThemeName;
				ApplyPaintBucket(itemDef);
				GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate(string Id, GUIDialogWindow.DialogState state)
				{
					bool flag = false;
					if (state == GUIDialogWindow.DialogState.Ok)
					{
						if (itemDef.HeroSet == null || itemDef.HeroSet.ToLower() == "none")
						{
							ShsAudioSource.PlayAutoSound(HqController2.Instance.Sounds.GetSource("paint_applied"));
							HqController2.Instance.ActiveRoom.ConsumeItem(base.gameObject);
							flag = true;
							HqController2.Instance.ActiveRoom.Save();
						}
						else
						{
							CspUtils.DebugLogError("Please ensure paint has no HeroSet or that its HeroSet is set to 'none'");
						}
					}
					if (!flag)
					{
						ApplyPaintBucket(itemDef);
						AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(new string[1]
						{
							itemDef.Id
						}, CollectionResetMessage.ActionType.Add, "Items"));
						HqController2.Instance.ActiveRoom.DelItem(base.gameObject);
						HqController2.Instance.ActiveRoom.ApplyTheme(previousTheme, true);
					}
					Object.Destroy(base.gameObject);
				});
				SHSCommonDialogWindow sHSCommonDialogWindow = new SHSCommonDialogWindow("common_bundle|notification_icon_hqedit", new Vector2(175f, 216f), new Vector2(0f, 0f), string.Empty, "common_bundle|L_mshs_button_ok", "common_bundle|L_mshs_button_cancel", typeof(SHSDialogOkButton), typeof(SHSDialogCancelButton), false);
				sHSCommonDialogWindow.TitleText = "#hq_paint_bucket_apply_prompt";
				sHSCommonDialogWindow.Text = "#hq_paint_bucket_apply_prompt";
				sHSCommonDialogWindow.NotificationSink = notificationSink;
				GUIManager.Instance.ShowDynamicWindow(sHSCommonDialogWindow, GUIControl.ModalLevelEnum.Full);
			}
			hasBeenDropped = true;
		}
	}

	protected void ApplyPaintBucket(ItemDefinition itemDef)
	{
		if (itemDef.HeroSet != null)
		{
			string heroThemeName = HqController2.Instance.GetHeroThemeName(itemDef.HeroSet);
			HqController2.Instance.ActiveRoom.ApplyTheme(heroThemeName, true);
		}
		else
		{
			HqController2.Instance.ActiveRoom.ApplyTheme("#HQE_NAME_" + itemDef.Id, true);
		}
	}
}
