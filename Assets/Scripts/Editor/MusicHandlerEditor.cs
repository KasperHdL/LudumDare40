using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(MusicHandler))]
public class MusicHandlerEditor : Editor {

    public override void OnInspectorGUI(){
        DrawDefaultInspector();
        if(!Application.isPlaying) return;

        MusicHandler script = target as MusicHandler;

        if(GUILayout.Button("Stop")){
            script.Stop();
        }

        if(GUILayout.Button("Play")){
            script.PlayDelayed(0);
        }

        if(GUILayout.Button("Next")){
            script.NextBpm();
        }

    }

}
