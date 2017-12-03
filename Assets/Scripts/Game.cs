using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    public static Game instance;

    [Header("Variables")]
    public bool isRunning = false;
    public int levelIndex = 0;
    public Color[] colors;

    [Header("References")]
    public World world;
    public CameraManager camera;
    public MusicHandler music;


    private Vector3 beforePauseVelocity;

    void Awake(){
        if(instance != null){
            Debug.Log("Multiple Game Instances");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void FinishedLevel(){
        world.Generate();
        levelIndex++;

        if(levelIndex == 5){
            music.NextBpm();
        }
        if(levelIndex == 10){
            camera.changeCameraOnBeat = 4;
        }
        if(levelIndex == 15){
            music.NextBpm();
        }
        if(levelIndex == 20){
            camera.changeCameraOnBeat = 2;
        }
        if(levelIndex == 30){
            camera.changeCameraOnBeat = 1;
        }
    }

    public void Pause(){
        isRunning = false;
        Player.instance.canControl = false;
        beforePauseVelocity = Player.instance.body.velocity;
        
        Player.instance.body.isKinematic = true;
    }
    public void Unpause(){
        isRunning = true;
        Player.instance.canControl = true;
        Player.instance.body.isKinematic = false;
        Player.instance.body.velocity = beforePauseVelocity;
    }
}
