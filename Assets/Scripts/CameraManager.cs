using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public Player player;
    public Camera camera;

    public List<Vector3> positions;
    public int index = 0;

    public float nextCut = 0;
    public float cutDelay = 4;

    void Start(){
        camera = Camera.main;

    }
	
	void Update () {
        if(nextCut < Time.time){
            nextCut = Time.time + cutDelay;

            camera.transform.position = positions[index];

            index++;
            if(index >= positions.Count)
                index = 0;

        }

        camera.transform.LookAt(player.transform);
	}

    public void SpawnCamera(float worldSize){
        Vector3 spawn = Random.insideUnitSphere;
        spawn.y = 0;
        spawn = spawn.normalized * worldSize;
        spawn.y = 15;

        positions.Add(spawn);
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        for(int i = 0; i < positions.Count; i++){
            Gizmos.DrawCube(positions[i], Vector3.one);
        }
    }
}
