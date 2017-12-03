using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public Player player;
    public CameraManager cameraManager;
    public Circle circle;

    public List<GameObject> keyObject;

    public List<GameObject> pillars;

    public GameObject pillarPrefab;

    public GameObject keyPrefab;
    
    public Vector3 keySpawnPos;

    public int size = 5;
    public float blockSize = 2.5f;
    public float blockHeight = 1;
    public int randomHeight = 5;

    private List<Vector2> occupied;

    Vector3 offset;

    public void Start(){
        occupied = new List<Vector2>();
        occupied.Add(new Vector2(2,2));

        keyObject = new List<GameObject>();

        Generate();


        Vector3 spawn = RandomSpawn();
        player.transform.position = player.finishPos;


        cameraManager.SpawnCamera();
    }

	public void Generate () {
        occupied.Clear();
        occupied.Add(new Vector2(2,2));

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


        for(int i = keyObject.Count; i < Game.instance.numRequiredKeys; i++){
            keyObject.Add(Instantiate(keyPrefab, keySpawnPos, Quaternion.identity) as GameObject);
            keyObject[i].GetComponent<Key>().circle = circle;
        }


        for(int i = 0; i < Game.instance.numRequiredKeys; i++){
            Vector3 spawn = RandomSpawn();
            Vector3 pos = spawn + Vector3.up * keyObject[i].transform.localScale.y * 4;

            if(i >= keyObject.Count){
                keyObject.Add(Instantiate(keyPrefab, spawn, Quaternion.identity) as GameObject);
                keyObject[i].GetComponent<Key>().circle = circle;
            }

            keyObject[i].GetComponent<Key>().Init(pos);
        }

        cameraManager.SpawnCamera();
	}
    
    public void SpawnPillar(int index, Vector3 pos, float h, Direction direction = Direction.None){

        Pillar p;

        if(pillars.Count <= index){
            GameObject g = Instantiate(pillarPrefab, pos, Quaternion.identity);
            g.transform.localScale = new Vector3(blockSize, 0.1f, blockSize);

            p = g.GetComponent<Pillar>();
            p.transform.SetParent(transform);
            pillars.Add(g);
        }else{
            p = pillars[index].GetComponent<Pillar>();
        }

        p.wallDirection = direction;
        p.SetNext(pos, new Vector3(blockSize, h, blockSize));
    }

    public Vector3 RandomSpawn(){
        Vector3 spawn;

        while(true){
            spawn = new Vector3(
                    Random.Range(0,size),
                    0,
                    Random.Range(0,size)
                );

            bool found = false;
            for(int i = 0; i < occupied.Count; i++){
                if(spawn.x == occupied[i].x && spawn.z == occupied[i].y){
                    found = true;
                    break;
                }
            }
            if(!found) break;
        }
        occupied.Add(new Vector2(spawn.x, spawn.z));

        int index = (int)(spawn.z * size + spawn.x);

        spawn.z *= blockSize;
        spawn.x *= blockSize;
        spawn.y = pillars[index].GetComponent<Pillar>().startSize.y / 2;


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
