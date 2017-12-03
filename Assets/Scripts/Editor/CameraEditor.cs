using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CameraManager))]
public class CameraEditor : Editor {

    public override void OnInspectorGUI(){
        DrawDefaultInspector();
        if(!Application.isPlaying) return;

        CameraManager script = target as CameraManager;

        if(GUILayout.Button("Generate")){
            script.RespawnCameras();

        }


    }

}
