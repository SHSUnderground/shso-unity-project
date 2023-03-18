using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml.XPath;

public class MenuChatManager
{
	private readonly Dictionary<GameController.ControllerType, MenuChatGroup> chatMenuRoots;

	public MenuChatManager()
	{
		chatMenuRoots = new Dictionary<GameController.ControllerType, MenuChatGroup>();
	}

	public MenuChatGroup GetMenuForGameArea(GameController.ControllerType controllerType)
	{
		if (chatMenuRoots.ContainsKey(controllerType))
		{
			return chatMenuRoots[controllerType];
		}
		return chatMenuRoots[GameController.ControllerType.SocialSpace];
	}

	public void LoadConfiguration(DataWarehouse data)
	{
		XPathNavigator value = data.GetValue("//menus");
		XPathNodeIterator xPathNodeIterator = value.SelectChildren("menu", string.Empty);
		while (xPathNodeIterator.MoveNext())
		{
			string value2 = xPathNodeIterator.Current.SelectSingleNode("gamearea").Value;
			GameController.ControllerType key = (GameController.ControllerType)(int)Enum.Parse(typeof(GameController.ControllerType), value2);
			MenuChatGroup menuChatGroup = new MenuChatGroup();
			menuChatGroup.IsRoot = true;
			XPathNodeIterator xPathNodeIterator2 = xPathNodeIterator.Current.SelectChildren("menuchatgroup", string.Empty);
			while (xPathNodeIterator2.MoveNext())
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(MenuChatGroup));
				MenuChatGroup item = (MenuChatGroup)xmlSerializer.Deserialize(new StringReader(xPathNodeIterator2.Current.OuterXml));
				menuChatGroup.MenuChatGroups.Add(item);
				menuChatGroup.WireUpLineage();
			}
			chatMenuRoots[key] = menuChatGroup;
		}
	}
}
