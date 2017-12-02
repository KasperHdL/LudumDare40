using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(World))]
public class WorldEditor : Editor {

    public override void OnInspectorGUI(){
        DrawDefaultInspector();
        if(!Application.isPlaying) return;

        World script = target as World;

        if(GUILayout.Button("Generate")){
            script.Generate();

        }


    }

}
