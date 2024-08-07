using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;

public class CspUtils
{
    public static string version = "18.4";  // use this as the client version number....make sure always length of 9 chars.
    public static string disclaimer = "SHSO Remake is a non-profit fan game project. It is not affiliated with Gazillion, The Amazing Society, Marvel or Disney in any way.\nThe developers of this project have no intent to infringe said copyrights or registered trademarks. No financial gain is made from this project.\n\nSHS is a trademark of Marvel and Disney. All copyrights and registered trademarks regarding the SHS and Marvel characters belong to Marvel and Disney.\nOriginal SHSO assets used in this project are property of Gazillion, The Amazing Society, Marvel, Disney and their respective owners.";
    
    public static bool printDebug = false;

    public static void DebugLog(object s) {
        try {
            if (Debug.isDebugBuild || printDebug)
                {
                    Debug.Log("[INFO]");
                    Debug.Log(s);
                }
        }
        catch (Exception e) {
		
           }
    }

    public static void DebugLogWarning(object s) {
        try {
            if (Debug.isDebugBuild || printDebug)
                {
                    Debug.Log("[WARN]");
                    Debug.Log(s);
                }
            }
        catch (Exception e) {

            }
    }

    public static void DebugLogError (object s) {
        try {
            if (Debug.isDebugBuild || printDebug)
                {
                    Debug.Log("[ERROR]");
                    Debug.Log(s);
                }
            }
        catch (Exception e) {

            }
    }

    public static int maxFilename = 7;

    public static int MaxFilename
    {
        get
        {
            return maxFilename;
        }
        set
        {
            maxFilename = value;
        }
    }

    public static int maxFilenameHalloween = 5;

    public static string halloweenSuffix = "";

    public static string HalloweenSuffix 
    {
        get
        {
            return halloweenSuffix;
        }
        set
        {
            halloweenSuffix = value;
        }
    }

    public static string halloweenPathSuffix = "";

    public static string HalloweenPathSuffix 
    {
        get
        {
            return halloweenPathSuffix;
        }
        set
        {
            halloweenPathSuffix = value;
        }
    }

    public static UnityEngine.Object CspLoad(AssetBundle bundle, string assetName) {
        UnityEngine.Object[] objs = null;
        //UnityEngine.Object[] objs = bundle.LoadAll();  
        //UnityEngine.Object request = bundle.Load(assetName, typeof(UnityEngine.Object));
        UnityEngine.Object request = bundle.Load(assetName);
        if (request == null) {
            objs = bundle.LoadAll();
            request = bundle.Load(assetName);
        }

        UnityEngine.Object asset2 = request;
        if (asset2 == null) {	// CSP - LoadAll does work, though. //////
            CspUtils.DebugLog("asset2 == null in CspLoad() for assetName=" + assetName);
            int i=0;
            foreach (UnityEngine.Object obj in objs)	
            {	
                //CspUtils.DebugLog(i + " loaded asset name:" + obj.name);
                i++;
                if (obj.name.ToLower() == assetName.ToLower()) {
                        asset2 = obj;
                        CspUtils.DebugLog("FOUND asset:" + assetName);
                        CspUtils.DebugLog("of type :" + asset2.GetType().ToString() + " in bundle=" + bundle.name);
                        return asset2;
                }	
            }		
        }
        else
        	CspUtils.DebugLog("ASSET FOUND USING Load()! assetName=" + assetName + " bundle=" + bundle.name);
        return asset2;
	}

    public static UnityEngine.Object CspLoadObj(CachedAssetBundle cachedBundle, string assetName, AssetBundleRequest request, UnityEngine.Object[] objs)
    {
        UnityEngine.Object asset2 = null;

        asset2 = request.asset;
        if (asset2 == null) {	// CSP - LoadAll does work, though. //////
            CspUtils.DebugLog("asset2 == null in StartAsyncAssetLoad() for assetName=" + assetName);
            int i=0;
            foreach (UnityEngine.Object obj in objs)	
            {	
                //CspUtils.DebugLog(i + " loaded asset name:" + obj.name);
                i++;
                if (obj.name.ToLower() == assetName.ToLower()) {
                        asset2 = obj;
                        CspUtils.DebugLog("FOUND asset:" + assetName+ " in bundle " + cachedBundle.RequestPath);
                        CspUtils.DebugLog("of type :" + asset2.GetType().ToString());
                        break;
                }	
            }	 	          
        }

        return asset2;
    }


    public static object findComponentByType(GameObject gameObject, string type) {
            Component  component = gameObject.GetComponent(type);  
            if (component != null)
                return component;

            Component [] components = gameObject.GetComponents(typeof(Component));  // CSP - loop over all components because single get not working.
			for (int i = 0; i < components.Length; ++i) {
				CspUtils.DebugLog("component name = " + components[i].name);
				CspUtils.DebugLog("component type = " + components[i].GetType());	
				if (components[i].GetType().ToString() == type) {
					CspUtils.DebugLog(type + " found!");
					//characterSpawn = (CharacterSpawn)(components[i]);
					return components[i];
				}
			}

            return null;
    }

    public static void MyDelay(int seconds)
    {
        DateTime dt = DateTime.Now + TimeSpan.FromSeconds(seconds);
    
        do { 
                DebugLog(DateTime.Now);
        } while (DateTime.Now < dt);
    }

    public static void OnAcceptBrawler(Matchmaker2.Ticket ticket)
    {
        CspUtils.DebugLog(ticket + " Console Command OnAcceptBrawler");
        AppShell.Instance.SharedHashTable["BrawlerAirlockPlaySolo"] = false;
        OnBrawlerTicket(ticket);
    }

    public static void OnBrawlerTicket(Matchmaker2.Ticket ticket)
    {
        if (ticket.status != 0)
        {
            CspUtils.DebugLog("Matchmaker returned " + ticket.status);
            return;
        }
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(ticket.ticket);
        XmlNode xmlNode = xmlDocument.SelectSingleNode("//mission");
        if (xmlNode == null)
        {
            CspUtils.DebugLog("Brawler ticket does not contain the mission: " + ticket.ticket);
            return;
        }
        AppShell.Instance.SharedHashTable["BrawlerTicket"] = ticket;
        ActiveMission activeMission = new ActiveMission(xmlNode.InnerText);
        activeMission.BecomeActiveMission();
        AppShell.Instance.Transition(GameController.ControllerType.Brawler);
    }

    public static void garbageCollect () {		// method added by CSP ...  also see CleanCacheCSP.cs

		Resources.UnloadUnusedAssets();
		//EditorUtility.UnloadUnusedAssetsIgnoreManagedReferences();
		System.GC.Collect();
	}

}