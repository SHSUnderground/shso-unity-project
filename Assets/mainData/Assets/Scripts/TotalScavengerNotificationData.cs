public class TotalScavengerNotificationData : NotificationData
{
	public TotalScavengerNotificationData(int ownableTypeID, int total)
		: base(NotificationType.TotalScavenger, NotificationOrientation.Left)
	{
		data1 = ownableTypeID;
		data2 = total;
	}

	public int getOwnableTypeID()
	{
		return data1;
	}

	public int getTotalItems()
	{
		return data2;
	}
}
