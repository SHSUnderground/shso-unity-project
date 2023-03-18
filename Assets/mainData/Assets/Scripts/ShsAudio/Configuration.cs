namespace ShsAudio
{
	public class Configuration
	{
		private static bool _flavorVOEnabled = true;

		public static bool TutorialVOEnabled
		{
			get
			{
				return Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.TutorialVOAllow);
			}
		}

		public static bool UseLocalizedTutorialVO
		{
			get
			{
				return Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.UseLocalizedTutorialVO);
			}
		}

		public static bool UseEnglishOnlyTutorialVO
		{
			get
			{
				return !UseLocalizedTutorialVO;
			}
		}

		public static bool FlavorVOEnabled
		{
			get
			{
				return _flavorVOEnabled && Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.FlavorVOAllow);
			}
			set
			{
				_flavorVOEnabled = value;
			}
		}

		public static bool UseLocalizedFlavorVO
		{
			get
			{
				return Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.UseLocalizedFlavorVO);
			}
		}

		public static bool UseEnglishOnlyFlavorVO
		{
			get
			{
				return !UseLocalizedFlavorVO;
			}
		}
	}
}
