using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public Player player;
    public Camera camera;

    public List<Vector3> positions;
    public int index = 0;

    void Start(){
        camera = Camera.main;
        EventHandler.Subscribe(GameEvent.Beat, Cut);
    }
	
	void Update () {
        camera.transform.LookAt(player.transform);
	}

    public void SpawnCamera(float worldSize){
        Vector3 spawn = Vector3.one;
        spawn.y = 0;
        spawn = spawn.normalized * worldSize;
        spawn.y = 25;

        positions.Add(spawn);

        spawn.x = -spawn.x;
        positions.Add(spawn);

        spawn.z = -spawn.z;
        positions.Add(spawn);

        spawn.x = -spawn.x;
        positions.Add(spawn);
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        for(int i = 0; i < positions.Count; i++){
            Gizmos.DrawCube(positions[i], Vector3.one);
        }
    }

    public void Cut(GameEventArgs eventArgs){
        BeatArgs args = eventArgs as BeatArgs;
        if(args.beat == 0){
            camera.transform.position = positions[index];

            index++;
            if(index >= positions.Count)
                index = 0;
        }
    }
}
