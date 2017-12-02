using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public Player player;

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

    public void Start(){
        Generate();
    }

	public void Generate () {
        for(int i = pillars.Count - 1; i >= 0; i--){
            Destroy(pillars[i]);
            pillars.RemoveAt(i);
        }

        Vector3 offset = new Vector3(-size/2, 0, -size/2);

        for(int y = 0; y < size; y++){
            for(int x = 0; x < size; x++){
                float h = height + Random.Range(0,randomHeight);
                Vector3 pos = new Vector3(x * blockSize, h / 2, y * blockSize) + offset;

                GameObject g = Instantiate(pillarPrefab, pos, Quaternion.identity);
                g.transform.localScale = new Vector3(blockSize,h,blockSize);
                g.transform.SetParent(transform);
                     
                pillars.Add(g);
            }
        }

        if(keyObject == null)
            keyObject = Instantiate(keyPrefab);

        if(lockObject == null)
            lockObject = Instantiate(lockPrefab);

        RandomSpawn(player.transform);
        player.transform.position += Vector3.up * 4;
        keyObject.transform.SetParent(null);
        RandomSpawn(keyObject.transform);
        RandomSpawn(lockObject.transform);

	}

    public void RandomSpawn(Transform transform){
        Vector3 spawn = new Vector3(
                Random.Range(0,size),
                0,
                Random.Range(0,size)
            );
        int index = (int)(spawn.y * size + spawn.y);
        spawn.y = pillars[index].transform.localScale.y/2 + transform.localScale.y;

        transform.position = spawn;
    }

	
}
