using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public static Game instance;

    public Color[] colors;

    void Awake(){
        if(instance != null){
            Debug.Log("Multiple Game Instances");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
}
