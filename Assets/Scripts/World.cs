using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public Player player;
    public CameraManager cameraManager;

    public GameObject keyObject;
    public GameObject lockObject;

    public List<GameObject> pillars;

    public GameObject pillarPrefab;

    public GameObject keyPrefab;
    public GameObject lockPrefab;

    public int size = 5;
    public float blockSize = 2.5f;
    public float blockHeight = 1;
    public int randomHeight = 5;

    Vector3 offset;

    public void Start(){
        Generate();
        cameraManager.SpawnCamera();
    }

	public void Generate () {

        offset = new Vector3(-size/2, 0, -size/2);
        offset *= blockSize;

        int counter = 0;
        for(int y = 0; y < size ; y++){
            for(int x = 0; x < size; x++){
                int index = y * size + x;
                float h = blockHeight + Random.Range(0,randomHeight) * blockHeight;

                Vector3 pos = new Vector3(x * blockSize, h / 2, y * blockSize) + offset;

                SpawnPillar(index, pos, h);
                counter++;
            }
        }

        for(int y = -1; y <= size ; y++){
            for(int x = -1; x <= size; x++){
                if(y != -1 && y != size && x != -1 && x != size) continue;
                Direction direction = GetDirection(x,y);
                float h = blockHeight * 4 + Random.Range(0,3) * blockHeight;

                Vector3 pos = new Vector3(x * blockSize, h / 2, y * blockSize) + offset;

                SpawnPillar(counter,pos, h, direction);
                counter++;
            }
        }


        if(keyObject == null)
            keyObject = Instantiate(keyPrefab);

        if(lockObject == null)
            lockObject = Instantiate(lockPrefab);

        Vector3 spawn = RandomSpawn();

        player.transform.position = spawn + Vector3.up * 4;
        player.body.velocity = Vector3.zero;
        player.freezeUntilHitGround = true;

        keyObject.transform.SetParent(null);
        spawn = RandomSpawn();
        keyObject.transform.position = spawn + Vector3.up * keyObject.transform.localScale.y * 2;
        keyObject.GetComponent<Key>().Init();


        spawn = RandomSpawn();
        lockObject.transform.position = spawn + Vector3.up * lockObject.transform.localScale.y;

        cameraManager.SpawnCamera();
	}
    
    public void SpawnPillar(int index, Vector3 pos, float h, Direction direction = Direction.None){

        Pillar p;

        if(pillars.Count <= index){
            GameObject g = Instantiate(pillarPrefab, pos, Quaternion.identity);
            p = g.GetComponent<Pillar>();
            pillars.Add(g);
        }else{
            p = pillars[index].GetComponent<Pillar>();
            p.transform.position = pos;
        }

        p.transform.localScale = new Vector3(blockSize,h,blockSize);
        p.transform.SetParent(transform);
        p.Init(0, direction);
             
    }

    public Vector3 RandomSpawn(){
        Vector3 spawn = new Vector3(
                Random.Range(0,size),
                0,
                Random.Range(0,size)
            );

        int index = (int)(spawn.z * size + spawn.x);

        spawn.z *= blockSize;
        spawn.x *= blockSize;
        spawn.y = pillars[index].transform.localScale.y;


        return spawn + offset;

    }

    private Direction GetDirection(int x, int y){
        if(y != -1 && y != size && x != -1 && x != size) return Direction.None;

        int timesSet = 0;
        Direction d = Direction.None;

        if(y == size){
            d = Direction.South; 
            timesSet++;
        }
        if(x == size){
            d = Direction.East; 
            timesSet++;
        }
        if(y == -1){
            d = Direction.North; 
            timesSet++;
        }
        if(x == -1){
            d = Direction.West; 
            timesSet++;
        }

        if(timesSet > 1)
            d = Direction.Diagonal;

        return d;
    }

}
