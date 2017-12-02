using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public int levelIndex = 0;
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
    public int height = 1;
    public int randomHeight = 5;

    Vector3 offset;

    public void Start(){
        Generate();
        cameraManager.SpawnCamera((size - 2) * blockSize);
    }

	public void Generate () {
        for(int i = pillars.Count - 1; i >= 0; i--){
            Destroy(pillars[i]);
            pillars.RemoveAt(i);
        }

        offset = new Vector3(-size/2, 0, -size/2);
        offset *= blockSize;

        int counter = 0;
        for(int y = 0; y < size ; y++){
            for(int x = 0; x < size; x++){
                int index = y * size + x;
                float h = height + Random.Range(0,randomHeight);

                Vector3 pos = new Vector3(x * blockSize, h / 2, y * blockSize) + offset;

                SpawnPillar(index, pos, h, false);
                counter++;
            }
        }

        for(int y = -1; y <= size ; y++){
            for(int x = -1; x <= size; x++){
                if(y != -1 && y != size && x != -1 && x != size) continue;
                float h = 4 + Random.Range(0,3);

                Vector3 pos = new Vector3(x * blockSize, h / 2, y * blockSize) + offset;

                SpawnPillar(counter,pos, h, true);
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
        keyObject.transform.position = spawn + Vector3.up * keyObject.transform.localScale.y;


        spawn = RandomSpawn();
        lockObject.transform.position = spawn + Vector3.up * lockObject.transform.localScale.y;

        //cameraManager.SpawnCamera(size * blockSize);

        levelIndex++;
	}
    
    public void SpawnPillar(int index, Vector3 pos, float h, bool isWall){

        Pillar p;

        if(pillars.Count <= index){
            GameObject g = Instantiate(pillarPrefab, pos, Quaternion.identity);
            p = g.GetComponent<Pillar>();
            pillars.Add(g);
        }else{
            p = pillars[index].GetComponent<Pillar>();
        }

        p.transform.localScale = new Vector3(blockSize,h,blockSize);
        p.transform.SetParent(transform);
        p.Init(isWall);
             
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
}
