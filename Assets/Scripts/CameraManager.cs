using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public static CameraManager instance;

    public Player player;
    public World world;
    public Camera camera;

    public List<Vector3> positions;
    public int index = 0;

    public int changeCameraOnBeat = 8;

    public float sizeMultiplier;
    public float minHeight = 5;
    public float maxHeight = 15;

    public float lookY;

    void Start(){
        instance = this;

        camera = Camera.main;
        EventHandler.Subscribe(GameEvent.Beat, Cut);
    }
	
    public void Update(){
        Vector3 ppos = player.transform.position;
//        Vector3 kpos = world.keyObject.GetComponent<Key>().startPos;

        Vector3 sum = ppos;

        camera.transform.LookAt(sum);
    }

    public void SpawnCamera(){
        float worldSize = world.size * world.blockSize;

        Vector3 spawn = Random.onUnitSphere;
        spawn.y = 0;
        spawn = spawn.normalized * worldSize * sizeMultiplier;
        spawn.y = Random.Range(minHeight, maxHeight);

        positions.Add(spawn);

    }

    public void RespawnCameras(){
        int num = positions.Count;
        positions.Clear();

        for(int i = 0; i < num; i++)
            SpawnCamera();

        NextCamera();
    }

    public void SpawnDiagonalCameras(float worldSize){
        Vector3 spawn = Vector3.one;
        spawn.y = 0;
        spawn = spawn.normalized * worldSize;
        spawn.y = 10;

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

    public void NextCamera(){
        index++;
        if(index >= positions.Count)
            index = 0;

        camera.transform.position = positions[index];

        float x = Vector3.Dot(Vector3.right, camera.transform.position);
        float z = Vector3.Dot(Vector3.forward, camera.transform.position);

        Vector3 dia = Vector3.one;
        dia.y = 0;
        float d = Vector3.Dot(dia, camera.transform.position);

        float ax = Mathf.Abs(x);
        float az = Mathf.Abs(z);
        float ad = Mathf.Abs(d);

        Vector3 LookDirection = Vector3.zero;


        if(ax > az && ax > ad){
            LookDirection = Mathf.Sign(-x) * Vector3.right;
        }
        if(az > ax && az > ad){
            LookDirection = Mathf.Sign(-z) * Vector3.forward;
        }
        if(ad > ax && ad > az){
            LookDirection = Mathf.Sign(-d) * Vector3.one;
        }

        LookDirection.Normalize();
        LookDirection.y = lookY;
        camera.transform.rotation = Quaternion.LookRotation(LookDirection);

    }

    public void Cut(GameEventArgs eventArgs){
        BeatArgs args = eventArgs as BeatArgs;
        if(!Game.instance.transitioning && args.totalBeats % changeCameraOnBeat == 0){
            NextCamera();
        }
    }
}
