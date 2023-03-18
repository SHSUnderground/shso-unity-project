public class GameAreaAvailableNotificationData : NotificationData
{
	public AssetBundleLoader.BundleGroup group;

	public GameAreaAvailableNotificationData(AssetBundleLoader.BundleGroup group)
		: base(NotificationType.GameAreaAvailable, NotificationOrientation.Right)
	{
		this.group = group;
	}
}
