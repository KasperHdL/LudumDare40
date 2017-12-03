using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour {
    public GameObject canvas;
    public Player player;

    public GameObject pauseObject;
    public Text startButton;


    void Start(){
        if(player.canControl){
            StartButton();
        }
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            canvas.SetActive(!canvas.activeSelf);
            pauseObject.SetActive(canvas.activeSelf);

            if(canvas.activeSelf){
                Game.instance.Pause();
            }else{
                Game.instance.Unpause();
            }
        }
    }

    public void StartButton(){
        canvas.SetActive(false);
        Game.instance.Unpause();
        startButton.text = "Resume";
    }
}
