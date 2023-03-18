using System.Collections.Generic;

public class LoginStatusNotifier
{
	public enum LoginStep
	{
		RequestToken,
		BadToken,
		BadPassword,
		BadUserName,
		PlatformError,
		AccountExpired,
		AccountSuspended,
		AccountBanned,
		ReadToken,
		RequestNotificationServer,
		ConnectToNotificationServer,
		RequestEntitlements,
		BadProtocol,
		BadSession,
		GameFull,
		RetryEntitlementRequest,
		RequestUserProfile,
		TimedOut,
		UnknownError
	}

	private LoginStep lastStep;

	private static readonly Dictionary<LoginStep, string> messages;

	public LoginStep LastStep
	{
		get
		{
			return lastStep;
		}
	}

	public string Message
	{
		get
		{
			string value;
			if (messages.TryGetValue(LastStep, out value))
			{
				return value;
			}
			return "Unknown Last Step: " + LastStep;
		}
	}

	static LoginStatusNotifier()
	{
		Dictionary<LoginStep, string> dictionary = new Dictionary<LoginStep, string>();
		dictionary.Add(LoginStep.RequestToken, "Sampling DNA");
		dictionary.Add(LoginStep.BadToken, "Fake ID detected.");
		dictionary.Add(LoginStep.BadUserName, "No Super Hero known by that name.");
		dictionary.Add(LoginStep.BadPassword, "Access Denied!");
		dictionary.Add(LoginStep.PlatformError, "WARN: Consult the Oracle.");
		dictionary.Add(LoginStep.AccountExpired, "INSERT COIN");
		dictionary.Add(LoginStep.AccountSuspended, "No Game For You!");
		dictionary.Add(LoginStep.AccountBanned, "Red-card.  You're out of the game.");
		dictionary.Add(LoginStep.ReadToken, "Performing Retinal Scan");
		dictionary.Add(LoginStep.RequestNotificationServer, "Looking up Directions on Map");
		dictionary.Add(LoginStep.ConnectToNotificationServer, "Interdimensional Teleportation Initiated");
		dictionary.Add(LoginStep.RequestEntitlements, "Warming up Helicarrier");
		dictionary.Add(LoginStep.BadProtocol, "Insufficient Pym Particles for Disinflation");
		dictionary.Add(LoginStep.BadSession, "Bad or Missing Session");
		dictionary.Add(LoginStep.GameFull, "The Game is full right now, try again later");
		dictionary.Add(LoginStep.RetryEntitlementRequest, "Entry Denied - Begging Hulk to Open Side Door");
		dictionary.Add(LoginStep.RequestUserProfile, "Amplifying Tachyons");
		dictionary.Add(LoginStep.TimedOut, "Reply hazy, ask again later.");
		dictionary.Add(LoginStep.UnknownError, "Uh oh, something surprising went wrong.");
		messages = dictionary;
	}

	public void Notify(LoginStep step)
	{
		lastStep = step;
		AppShell.Instance.EventMgr.Fire(this, new LoginStatusMessage(step, Message));
	}
}
