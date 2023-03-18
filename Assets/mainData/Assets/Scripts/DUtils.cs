using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class DUtils : MonoBehaviour {
	
	public static List<string> single = new List<string>();

	
	public static string dutil(string msg){
	
		string filtered = "";
		
		bool first = true;
		
		foreach(string m in msg.Split(' ')){
		
			
			if(single.Contains(m.ToLower().TrimEnd('.',',','?','!','/' ))){
				
				if(first){
					filtered = "#";	
					first = false;
				}else{
					
					filtered += " " + "#";
					
				}
				
			}else{
			
				if(first){
					filtered = m;
					first = false;
				}else{
					filtered += " " + m;
				}
				
			}
			
		}
		Debug.Log("FILTER: "+filtered);
		
		return filtered;
		
	}
	
}
