using SmartFoxClientAPI;
using SmartFoxClientAPI.Data;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

public class LoggingSmartFoxClientProxy : SmartFoxClient
{
	private static int clientCount;

	private static int activeClientCount;

	private readonly int clientId;

	private static readonly XmlTextWriter logWriter;

	private static bool initializedLogWriter;

	private static readonly DateTime startTime;

	public LoggingSmartFoxClientProxy()
	{
		Interlocked.Increment(ref clientCount);
		lock (logWriter)
		{
			if (!initializedLogWriter)
			{
				logWriter.WriteStartDocument();
				logWriter.WriteStartElement("executions");
				initializedLogWriter = true;
			}
		}
		clientId = clientCount;
		Interlocked.Increment(ref activeClientCount);
	}

	static LoggingSmartFoxClientProxy()
	{
		startTime = DateTime.Now;
		logWriter = new XmlTextWriter(Stream.Null, Encoding.UTF8);
	}

	public new void Dispose()
	{
		base.Dispose();
		if (activeClientCount <= 1)
		{
			logWriter.Close();
		}
		Interlocked.Decrement(ref activeClientCount);
	}

	protected void LogEntry(string method)
	{
		lock (logWriter)
		{
			logWriter.WriteStartElement("execution");
			logWriter.WriteElementString("userId", myUserId + string.Empty);
			logWriter.WriteElementString("clientId", clientId + string.Empty);
			logWriter.WriteElementString("method", method);
			logWriter.WriteElementString("timestamp", (long)DateTime.Now.Subtract(startTime).TotalMilliseconds + string.Empty);
			logWriter.WriteEndElement();
		}
	}

	protected void LogEntry(string method, LoggerObject[] parameters)
	{
		lock (logWriter)
		{
			logWriter.WriteStartElement("execution");
			logWriter.WriteElementString("userId", myUserId + string.Empty);
			logWriter.WriteElementString("clientId", clientId + string.Empty);
			logWriter.WriteElementString("method", method);
			logWriter.WriteElementString("timestamp", (long)DateTime.Now.Subtract(startTime).TotalMilliseconds + string.Empty);
			if (parameters != null && parameters.Length > 0)
			{
				logWriter.WriteStartElement("arguments");
				for (int i = 0; i < parameters.Length; i++)
				{
					logWriter.WriteStartElement("argument");
					logWriter.WriteAttributeString("position", i + string.Empty);
					logWriter.WriteElementString("type", parameters[i].OriginalType.FullName);
					logWriter.WriteElementString("value", parameters[i].SerializedObject);
					logWriter.WriteEndElement();
				}
				logWriter.WriteEndElement();
			}
			logWriter.WriteEndElement();
		}
	}

	public new void AddBuddy(string buddyName)
	{
		LogEntry("AddBody", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromString(buddyName)
		});
		base.AddBuddy(buddyName);
	}

	public new void AutoJoin()
	{
		LogEntry("AutoJoin");
		base.AutoJoin();
	}

	public new void ClearBuddyList()
	{
		LogEntry("ClearBuddyList");
		base.ClearBuddyList();
	}

	public new void Connect(string ipAdr)
	{
		LogEntry("Connect", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromString(ipAdr)
		});
		base.Connect(ipAdr);
	}

	public new void Connect(string ipAdr, int port)
	{
		LogEntry("Connect", new LoggerObject[2]
		{
			LoggerObjectSerializer.fromString(ipAdr),
			LoggerObjectSerializer.fromInt(port)
		});
		base.Connect(ipAdr, port);
	}

	public new void CreateRoom(Hashtable roomObj)
	{
		LogEntry("CreateRoom", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromHashtable(roomObj)
		});
		base.CreateRoom(roomObj);
	}

	public new void CreateRoom(NewRoomDescriptor roomObj)
	{
		LogEntry("CreateRoom", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromObject(roomObj)
		});
		base.CreateRoom(roomObj);
	}

	public new void CreateRoom(Hashtable roomObj, int roomId)
	{
		LogEntry("CreateRoom", new LoggerObject[2]
		{
			LoggerObjectSerializer.fromHashtable(roomObj),
			LoggerObjectSerializer.fromInt(roomId)
		});
		base.CreateRoom(roomObj, roomId);
	}

	public new void CreateRoom(NewRoomDescriptor roomObj, int roomId)
	{
		LogEntry("CreateRoom", new LoggerObject[2]
		{
			LoggerObjectSerializer.fromObject(roomObj),
			LoggerObjectSerializer.fromInt(roomId)
		});
		base.CreateRoom(roomObj, roomId);
	}

	public new void Disconnect()
	{
		LogEntry("Disconnect");
		base.Disconnect();
	}

	public new Room GetActiveRoom()
	{
		LogEntry("GetActiveRoom");
		return base.GetActiveRoom();
	}

	public new Hashtable GetAllRooms()
	{
		LogEntry("GetAllRooms");
		return base.GetAllRooms();
	}

	public new Buddy GetBuddyById(int id)
	{
		LogEntry("GetBuddyById", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromInt(id)
		});
		return base.GetBuddyById(id);
	}

	public new Buddy GetBuddyByName(string buddyName)
	{
		LogEntry("GetBuddyByName", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromString(buddyName)
		});
		return base.GetBuddyByName(buddyName);
	}

	public new void GetBuddyRoom(Buddy buddy)
	{
		LogEntry("GetBuddyRoom", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromObject(buddy)
		});
		base.GetBuddyRoom(buddy);
	}

	public new string GetConnectionMode()
	{
		LogEntry("GetConnectionMode");
		return base.GetConnectionMode();
	}

	public new int GetHttpPollSpeed()
	{
		LogEntry("GetHttpPollSpeed");
		return base.GetHttpPollSpeed();
	}

	public new void GetRandomKey()
	{
		LogEntry("GetRandomKey");
		base.GetRandomKey();
	}

	public new string GetRawProtocolSeparator()
	{
		LogEntry("GetRawProtocolSeparator");
		return base.GetRawProtocolSeparator();
	}

	public new Room GetRoom(int roomId)
	{
		LogEntry("GetRoom", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromInt(roomId)
		});
		return base.GetRoom(roomId);
	}

	public new Room GetRoomByName(string roomName)
	{
		LogEntry("GetRoomByName", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromString(roomName)
		});
		return base.GetRoomByName(roomName);
	}

	public new void GetRoomList()
	{
		LogEntry("GetRoomList");
		base.GetRoomList();
	}

	public new string GetUploadPath()
	{
		LogEntry("GetUploadPath");
		return base.GetUploadPath();
	}

	public new string GetVersion()
	{
		LogEntry("GetVersion");
		return base.GetVersion();
	}

	public new bool IsConnected()
	{
		return base.IsConnected();
	}

	public new void JoinRoom(object newRoom)
	{
		LogEntry("JoinRoom", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromObject(newRoom)
		});
		base.JoinRoom(newRoom);
	}

	public new void JoinRoom(object newRoom, string pword)
	{
		LogEntry("JoinRoom", new LoggerObject[2]
		{
			LoggerObjectSerializer.fromObject(newRoom),
			LoggerObjectSerializer.fromString(pword)
		});
		base.JoinRoom(newRoom, pword);
	}

	public new void JoinRoom(object newRoom, string pword, bool isSpectator)
	{
		LogEntry("JoinRoom", new LoggerObject[3]
		{
			LoggerObjectSerializer.fromObject(newRoom),
			LoggerObjectSerializer.fromString(pword),
			LoggerObjectSerializer.fromBool(isSpectator)
		});
		base.JoinRoom(newRoom, pword, isSpectator);
	}

	public new void JoinRoom(object newRoom, string pword, bool isSpectator, bool dontLeave)
	{
		LogEntry("JoinRoom", new LoggerObject[4]
		{
			LoggerObjectSerializer.fromObject(newRoom),
			LoggerObjectSerializer.fromString(pword),
			LoggerObjectSerializer.fromBool(isSpectator),
			LoggerObjectSerializer.fromBool(dontLeave)
		});
		base.JoinRoom(newRoom, pword, isSpectator, dontLeave);
	}

	public new void JoinRoom(object newRoom, string pword, bool isSpectator, bool dontLeave, int oldRoom)
	{
		LogEntry("JoinRoom", new LoggerObject[5]
		{
			LoggerObjectSerializer.fromObject(newRoom),
			LoggerObjectSerializer.fromString(pword),
			LoggerObjectSerializer.fromBool(isSpectator),
			LoggerObjectSerializer.fromBool(dontLeave),
			LoggerObjectSerializer.fromInt(oldRoom)
		});
		base.JoinRoom(newRoom, pword, isSpectator, dontLeave, oldRoom);
	}

	public new void LeaveRoom(int roomId)
	{
		LogEntry("LeaveRoom", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromInt(roomId)
		});
		base.LeaveRoom(roomId);
	}

	public new void LoadBuddyList()
	{
		LogEntry("LoadBuddyList");
		base.LoadBuddyList();
	}

	public new void LoadConfig()
	{
		LogEntry("LoadConfig");
		base.LoadConfig();
	}

	public new void LoadConfig(string configFile)
	{
		LogEntry("LoadConfig", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromString(configFile)
		});
		base.LoadConfig(configFile);
	}

	public new void LoadConfig(string configFile, bool autoConnect)
	{
		LogEntry("LoadConfig", new LoggerObject[2]
		{
			LoggerObjectSerializer.fromString(configFile),
			LoggerObjectSerializer.fromBool(autoConnect)
		});
		base.LoadConfig(configFile, autoConnect);
	}

	public new void Login(string zone, string name, string pass)
	{
		LogEntry("Login", new LoggerObject[3]
		{
			LoggerObjectSerializer.fromString(zone),
			LoggerObjectSerializer.fromString(name),
			LoggerObjectSerializer.fromString(pass)
		});
		base.Login(zone, name, pass);
	}

	public new void Logout()
	{
		LogEntry("Logout");
		base.Logout();
	}

	public new int NumEventsInEventQueue()
	{
		return base.NumEventsInEventQueue();
	}

	public new void ProcessEventQueue()
	{
		LogEntry("ProcessEventQueue");
		base.ProcessEventQueue();
	}

	public new void ProcessSingleEventInEventQueue()
	{
		LogEntry("ProcessSingleEventInEventQueue");
		base.ProcessSingleEventInEventQueue();
	}

	public new void RemoveBuddy(string buddyName)
	{
		LogEntry("RemoveBuddy", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromString(buddyName)
		});
		base.RemoveBuddy(buddyName);
	}

	public new void RoundTripBench()
	{
		LogEntry("RoundTripBench");
		base.RoundTripBench();
	}

	public new void SendBuddyPermissionResponse(bool allowBuddy, string targetBuddy)
	{
		LogEntry("SendBuddyPermissionResponse", new LoggerObject[2]
		{
			LoggerObjectSerializer.fromBool(allowBuddy),
			LoggerObjectSerializer.fromString(targetBuddy)
		});
		base.SendBuddyPermissionResponse(allowBuddy, targetBuddy);
	}

	public new void SendModeratorMessage(string message, string type)
	{
		LogEntry("SendModeratorMessage", new LoggerObject[2]
		{
			LoggerObjectSerializer.fromString(message),
			LoggerObjectSerializer.fromString(type)
		});
		base.SendModeratorMessage(message, type);
	}

	public new void SendModeratorMessage(string message, string type, int id)
	{
		LogEntry("SendModeratorMessage", new LoggerObject[3]
		{
			LoggerObjectSerializer.fromString(message),
			LoggerObjectSerializer.fromString(type),
			LoggerObjectSerializer.fromInt(id)
		});
		base.SendModeratorMessage(message, type, id);
	}

	public new void SendObject(SFSObject obj)
	{
		LogEntry("SendObject", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromSFSObject(obj)
		});
		base.SendObject(obj);
	}

	public new void SendObject(SFSObject obj, int roomId)
	{
		LogEntry("SendObject", new LoggerObject[2]
		{
			LoggerObjectSerializer.fromSFSObject(obj),
			LoggerObjectSerializer.fromInt(roomId)
		});
		base.SendObject(obj, roomId);
	}

	public new void SendObjectToGroup(SFSObject obj, ArrayList userList)
	{
		LogEntry("SendObjectToGroup", new LoggerObject[2]
		{
			LoggerObjectSerializer.fromSFSObject(obj),
			LoggerObjectSerializer.fromArrayList(userList)
		});
		base.SendObjectToGroup(obj, userList);
	}

	public new void SendObjectToGroup(SFSObject obj, ArrayList userList, int roomId)
	{
		LogEntry("SendObjectToGroup", new LoggerObject[3]
		{
			LoggerObjectSerializer.fromSFSObject(obj),
			LoggerObjectSerializer.fromArrayList(userList),
			LoggerObjectSerializer.fromInt(roomId)
		});
		base.SendObjectToGroup(obj, userList, roomId);
	}

	public new void SendPrivateMessage(string message, int recipientId)
	{
		LogEntry("SendPrivateMessage", new LoggerObject[2]
		{
			LoggerObjectSerializer.fromString(message),
			LoggerObjectSerializer.fromInt(recipientId)
		});
		base.SendPrivateMessage(message, recipientId);
	}

	public new void SendPrivateMessage(string message, int recipientId, int roomId)
	{
		LogEntry("SendPrivateMessage", new LoggerObject[3]
		{
			LoggerObjectSerializer.fromString(message),
			LoggerObjectSerializer.fromInt(recipientId),
			LoggerObjectSerializer.fromInt(roomId)
		});
		base.SendPrivateMessage(message, recipientId, roomId);
	}

	public new void SendPublicMessage(string message)
	{
		LogEntry("SendPublicMessage", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromString(message)
		});
		base.SendPublicMessage(message);
	}

	public new void SendPublicMessage(string message, int roomId)
	{
		LogEntry("SendPublicMessage", new LoggerObject[2]
		{
			LoggerObjectSerializer.fromString(message),
			LoggerObjectSerializer.fromInt(roomId)
		});
		base.SendPublicMessage(message, roomId);
	}

	public new void SendXtMessage(string xtName, string cmd, ICollection paramObj)
	{
		LogEntry("SendXtMessage", new LoggerObject[3]
		{
			LoggerObjectSerializer.fromString(xtName),
			LoggerObjectSerializer.fromString(cmd),
			LoggerObjectSerializer.fromCollection(paramObj)
		});
		base.SendXtMessage(xtName, cmd, paramObj);
	}

	public new void SendXtMessage(string xtName, string cmd, ICollection paramObj, string type)
	{
		LogEntry("SendXtMessage", new LoggerObject[4]
		{
			LoggerObjectSerializer.fromString(xtName),
			LoggerObjectSerializer.fromString(cmd),
			LoggerObjectSerializer.fromCollection(paramObj),
			LoggerObjectSerializer.fromString(type)
		});
		base.SendXtMessage(xtName, cmd, paramObj, type);
	}

	public new void SendXtMessage(string xtName, string cmd, ICollection paramObj, string type, int roomId)
	{
		LogEntry("SendXtMessage", new LoggerObject[5]
		{
			LoggerObjectSerializer.fromString(xtName),
			LoggerObjectSerializer.fromString(cmd),
			LoggerObjectSerializer.fromCollection(paramObj),
			LoggerObjectSerializer.fromString(type),
			LoggerObjectSerializer.fromInt(roomId)
		});
		base.SendXtMessage(xtName, cmd, paramObj, type, roomId);
	}

	public new void SetBuddyBlockStatus(string buddyName, bool status)
	{
		LogEntry("SetBuddyBlockStatus", new LoggerObject[2]
		{
			LoggerObjectSerializer.fromString(buddyName),
			LoggerObjectSerializer.fromBool(status)
		});
		base.SetBuddyBlockStatus(buddyName, status);
	}

	public new void SetBuddyVariables(Hashtable varList)
	{
		LogEntry("SetBuddyVariables", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromHashtable(varList)
		});
		base.SetBuddyVariables(varList);
	}

	public new void SetHttpPollSpeed(int sp)
	{
		LogEntry("SetHttpPollSpeed", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromInt(sp)
		});
		base.SetHttpPollSpeed(sp);
	}

	public new void SetIsConnected(bool b)
	{
		LogEntry("SetIsConnected", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromBool(b)
		});
		base.SetIsConnected(b);
	}

	public new void SetRawProtocolSeparator(string value)
	{
		LogEntry("SetRawProtocolSeparator", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromString(value)
		});
		base.SetRawProtocolSeparator(value);
	}

	public new void SetRoomVariables(ArrayList varList)
	{
		LogEntry("SetRoomVariables", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromArrayList(varList)
		});
		base.SetRoomVariables(varList);
	}

	public new void SetRoomVariables(ArrayList varList, int roomId)
	{
		LogEntry("SetRoomVariables", new LoggerObject[2]
		{
			LoggerObjectSerializer.fromArrayList(varList),
			LoggerObjectSerializer.fromInt(roomId)
		});
		base.SetRoomVariables(varList, roomId);
	}

	public new void SetRoomVariables(ArrayList varList, int roomId, bool setOwnership)
	{
		LogEntry("SetRoomVariables", new LoggerObject[3]
		{
			LoggerObjectSerializer.fromArrayList(varList),
			LoggerObjectSerializer.fromInt(roomId),
			LoggerObjectSerializer.fromBool(setOwnership)
		});
		base.SetRoomVariables(varList, roomId, setOwnership);
	}

	public new void SetUserVariables(Hashtable varObj)
	{
		LogEntry("SetUserVariables", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromHashtable(varObj)
		});
		base.SetUserVariables(varObj);
	}

	public new void SetUserVariables(Hashtable varObj, int roomId)
	{
		LogEntry("SetUserVariables", new LoggerObject[2]
		{
			LoggerObjectSerializer.fromHashtable(varObj),
			LoggerObjectSerializer.fromInt(roomId)
		});
		base.SetUserVariables(varObj, roomId);
	}

	public new void SwitchPlayer()
	{
		LogEntry("SwitchPlayer");
		base.SwitchPlayer();
	}

	public new void SwitchPlayer(int roomId)
	{
		LogEntry("SwitchPlayer", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromInt(roomId)
		});
		base.SwitchPlayer(roomId);
	}

	public new void SwitchSpectator()
	{
		LogEntry("SwitchSwitchSpectatorPlayer");
		base.SwitchSpectator();
	}

	public new void SwitchSpectator(int roomId)
	{
		LogEntry("SwitchSpectator", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromInt(roomId)
		});
		base.SwitchSpectator(roomId);
	}

	public new void UploadFile(string filePath)
	{
		LogEntry("UploadFile", new LoggerObject[1]
		{
			LoggerObjectSerializer.fromString(filePath)
		});
		base.UploadFile(filePath);
	}

	public new void UploadFile(string filePath, int id)
	{
		LogEntry("UploadFile", new LoggerObject[2]
		{
			LoggerObjectSerializer.fromString(filePath),
			LoggerObjectSerializer.fromInt(id)
		});
		base.UploadFile(filePath, id);
	}

	public new void UploadFile(string filePath, int id, string nick)
	{
		LogEntry("UploadFile", new LoggerObject[3]
		{
			LoggerObjectSerializer.fromString(filePath),
			LoggerObjectSerializer.fromInt(id),
			LoggerObjectSerializer.fromString(nick)
		});
		base.UploadFile(filePath, id, nick);
	}

	public new void UploadFile(string filePath, int id, string nick, int port)
	{
		LogEntry("UploadFile", new LoggerObject[4]
		{
			LoggerObjectSerializer.fromString(filePath),
			LoggerObjectSerializer.fromInt(id),
			LoggerObjectSerializer.fromString(nick),
			LoggerObjectSerializer.fromInt(port)
		});
		base.UploadFile(filePath, id, nick, port);
	}
}
