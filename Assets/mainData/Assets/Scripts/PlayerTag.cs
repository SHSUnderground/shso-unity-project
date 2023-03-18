using UnityEngine;
using System.Collections;

// THIS CLASS ADDED BY CSP. TEMPORARY REPLACEMENT FOR PLAYERBILLBOARD, WHICH I AM HAVING DIFFICULT WITH TEXT RENDERING.

 [RequireComponent (typeof(GUIText))]
 public class PlayerTag : MonoBehaviour
 {
     public Transform npcNamePos;
     public string npcName;

     public Font tagFont;

     public float xOffset = 0F;
     public float yOffset = 0F;
 
     Camera cam;
     Vector2 screenPos;
     GUIText npcGui;

     GUIStyle myStyle;
 
     void Start ()
     {
         cam = Camera.main;
         //npcNamePos = GameObject.Find("npcNamePos").transform;
         npcGui = GetComponent<GUIText>();
         myStyle = new GUIStyle();
         myStyle.font = tagFont;
         myStyle.normal.textColor = new Color(220F,225F,56F);   //Color.yellow;
     }
     
     void Update ()
     {
         screenPos = cam.WorldToScreenPoint(npcNamePos.position);
         //CspUtils.DebugLog("The position of the NPC is"  + screenPos);
     }
 
     void OnGUI()
     {
         var centeredStyle = GUI.skin.GetStyle("Label");
         centeredStyle.alignment = TextAnchor.UpperCenter;
         npcGui.material.color = Color.green;
       
         GUI.TextField(new Rect(screenPos.x + xOffset, (Screen.height - screenPos.y) + yOffset, 125, 25), npcName, myStyle);
     }
 }