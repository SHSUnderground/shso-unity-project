// C# Example
// Builds an asset bundle from the selected objects in the project view.
// Once compiled go to "Menu" -> "Assets" and select one of the choices
// to build the Asset Bundle   //

using UnityEngine;
using UnityEditor;  
using System.IO;   
using System.Xml;
using System.Xml.Serialization;




public class ExportAssetBundles {   
    System.Security.Cryptography.MD5CryptoServiceProvider md = null;
         
    // [MenuItem("Assets/Convert CABs")]
    // static void ConvertCABs() {                 
    
        
    //     DirectoryInfo dir = new DirectoryInfo("Assets/CABS");
    //     FileInfo[] info = dir.GetFiles("*.*");
    //     foreach (FileInfo f in info) 
    //     {                                                            
    //       string fname = System.IO.Path.GetFileName(f.ToString());
    //       CspUtils.DebugLog("fname=" + fname); 
           
           
    //       string result = AssetDatabase.RenameAsset("Assets/CABS/" + fname, fname + ".asset");
    //       CspUtils.DebugLog("result=" + result);      
    //       Object main = AssetDatabase.LoadMainAssetAtPath ("Assets/CABS/" + fname + ".asset");       
    //       CspUtils.DebugLog("main.name=" + main.name);                                      
          
    //       // Load all the objects             
    //       Object[] objs =  AssetDatabase.LoadAllAssetsAtPath ("Assets/CABS/" + fname + ".asset");  
    //       foreach (Object o in objs)
    //       {                      
    //           if (o != null)
    //             CspUtils.DebugLog("o.name:" + o.name);
    //       }                                
          
    //       bool success =  BuildPipeline.BuildAssetBundle(main, objs, "Assets/WEBBUNDLES/" + fname + ".unity3d", BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
    //       CspUtils.DebugLog("success:" + success);   
                                     
    //       // if done, change asset filename so it is not asset anymore. 
    //       //result = AssetDatabase.RenameAsset("Assets/CABS/" + fname + ".asset", fname);  
    //       System.IO.File.Move("Assets/CABS/" + fname + ".asset", "Assets/CABS/" + fname);
    //       //CspUtils.DebugLog("result=" + result);             
           
           
           
    //     }
             
        
    // }



    [MenuItem("Assets/Build AssetBundle From Selection - Track dependencies")]
    static void ExportResource () {
        // Bring up save panel
        string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
        if (path.Length != 0) {
            // Build the resource file from the active selection.
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);  
            Object main = null;     
            Object[] allObjs = Selection.gameObjects;
            foreach (Object obj in allObjs)   {     
              //CspUtils.DebugLog("obj.name: " + obj.name); 
              //if (obj.name == "Asgard_M102S_01_scenario_Bundle")   //// UNCOMMENT THIS AND MODIFY TO CHOOSE A MAIN ASSET.
                main = obj;
            }  
            if (main != null)
                CspUtils.DebugLog("MainAsset will be: " + main.name); 
            //return;
            BuildPipeline.BuildAssetBundle(main, selection, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
            Selection.objects = selection;
        }
    }



    // [MenuItem("Assets/Build AssetBundle From Selection - No dependency tracking")]    // this seems to produce the closest web assetbundle in filesize
    // static void ExportResourceNoTrack () {
    //     // Bring up save panel
    //     string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
    //     if (path.Length != 0) {
    //         // Build the resource file from the active selection.
    //         BuildPipeline.BuildAssetBundle(Selection.activeObject, Selection.objects, path);
    //     }
    // }          
    


    // // [MenuItem("Assets/Build AssetBundle From Selection - 1")]
    // // static void ExportResource1 () {
    // //     // Bring up save panel
    // //     string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
    // //     if (path.Length != 0) {
    // //         // Build the resource file from the active selection.
    // //         Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
    // //         BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path + "1", BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle);
    // //         Selection.objects = selection;
    // //     }
    // // }        
    
    // // [MenuItem("Assets/Build AssetBundle From Selection - 2")]
    // // static void ExportResource2 () {
    // //     // Bring up save panel
    // //     string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
    // //     if (path.Length != 0) {
    // //         // Build the resource file from the active selection.
    // //         Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
    // //         BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path + "2", BuildAssetBundleOptions.CollectDependencies);
    // //         Selection.objects = selection;
    // //     }
    // // }        
    
    // // [MenuItem("Assets/Build AssetBundle From Selection - 3")]
    // // static void ExportResource3 () {
    // //     // Bring up save panel
    // //     string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
    // //     if (path.Length != 0) {
    // //         // Build the resource file from the active selection.
    // //         Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
    // //         BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path + "3", BuildAssetBundleOptions.CompleteAssets);
    // //         Selection.objects = selection;
    // //     }
    // // }               
    
    // // [MenuItem("Assets/Build AssetBundle From Selection - 4")]
    // // static void ExportResource4 () {
    // //     // Bring up save panel
    // //     string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
    // //     if (path.Length != 0) {
    // //         // Build the resource file from the active selection.
    // //         Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
    // //         BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path + "4", BuildAssetBundleOptions.DeterministicAssetBundle);
    // //         Selection.objects = selection;
    // //     }
    // // }       
    
    
    // // [MenuItem("Assets/Build AssetBundle From Selection - 5")]
    // // static void ExportResource5 () {
    // //     // Bring up save panel
    // //     string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
    // //     if (path.Length != 0) {
    // //         // Build the resource file from the active selection.
    // //         Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
    // //         BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path + "5", BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.CollectDependencies);
    // //         Selection.objects = selection;
    // //     }
    // // }
    
    // // [MenuItem("Assets/Build AssetBundle From Selection - 6")]
    // // static void ExportResource6 () {
    // //     // Bring up save panel
    // //     string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
    // //     if (path.Length != 0) {
    // //         // Build the resource file from the active selection.
    // //         Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
    // //         BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path + "6", BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.CompleteAssets);
    // //         Selection.objects = selection;
    // //     }
    // // }
  
    
    
    
    // // [MenuItem("Assets/Build AssetBundle From Selection - Do All 16 Options")]
    // // static void ExportResourceAll16 () {
    // //     // Bring up save panel                  
    // //     CspUtils.DebugLog("BuildAssetBundleOptions.CollectDependencies = " + (int)BuildAssetBundleOptions.CollectDependencies);   //  1048576                   0001 0000 0000 0000 0000 0000?
    // //     CspUtils.DebugLog("BuildAssetBundleOptions.CompleteAssets = " + (int)BuildAssetBundleOptions.CompleteAssets);             //  2097152                   0010 0000 0000 0000 0000 0000?
    // //     CspUtils.DebugLog("BuildAssetBundleOptions.DisableWriteTypeTree = " + (int)BuildAssetBundleOptions.DisableWriteTypeTree); //  67108864             0100 0000 0000 0000 0000 0000? 0000
    // //     CspUtils.DebugLog("BuildAssetBundleOptions.DeterministicAssetBundle = " + (int)BuildAssetBundleOptions.DeterministicAssetBundle);//268435456  ?0001 0000 0000 0000 0000 0000 0000 0000?
    // //     string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");  
  
    // //     int[] vals = new int[] {268435456, 67108864, 2097152, 1048576};
        
    // //     for (int i=0; i<16; i++) {               
          
        
    // //       //int flagVal = 0;          
    // //       UnityEditor.BuildAssetBundleOptions opts = (UnityEditor.BuildAssetBundleOptions)0;
    // //       if (((i) & 1) > 0) {   
    // //         opts = opts | BuildAssetBundleOptions.CollectDependencies;
    // //         //flagVal += vals[3];         
    // //         CspUtils.DebugLog(i); 
    // //         CspUtils.DebugLog(opts);
    // //       } 
    // //       if (((i) & 2) > 0) {
    // //         opts = opts | BuildAssetBundleOptions.CompleteAssets;
    // //         //flagVal += vals[2];        
    // //         CspUtils.DebugLog(i);
    // //         CspUtils.DebugLog(opts);
    // //       }
    // //       //if (((i) & 3) > 0)   {          
    // //      //   opts = opts | BuildAssetBundleOptions.DisableWriteTypeTree;
    // //      //   //flagVal += vals[1];  
    // //      //   CspUtils.DebugLog(i);
    // //      //   CspUtils.DebugLog(opts);
    // //      // }
    // //       if (((i) & 4) > 0) {         
    // //         opts = opts | BuildAssetBundleOptions.DeterministicAssetBundle;
    // //         //flagVal += vals[0];   
    // //         CspUtils.DebugLog(i);
    // //         CspUtils.DebugLog(opts);
    // //       }
    // //       if (path.Length != 0) {
    // //           // Build the resource file from the active selection.
    // //           Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);     
    // //           CspUtils.DebugLog("selection length=" + selection.Length);
    // //           BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path + i.ToString(), opts);
    // //           Selection.objects = selection;
    // //       }      
          
    // //     }
    // // }


}
