using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

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

    public PostProcessingProfile cameraProfile;
    public DepthOfFieldModel.Settings DOFmodel;

    public AnimationCurve fadeCurve;
    public IEnumerator fadeRoutine;

    private Vector3 beforePauseVelocity;

    void Awake(){
        if(instance != null){
            Debug.Log("Multiple Game Instances");
            Destroy(gameObject);
            return;
        }
        instance = this;

        cameraProfile = Camera.main.GetComponent<PostProcessingBehaviour>().profile;

        cameraProfile.depthOfField.enabled = true;
        DOFmodel = cameraProfile.depthOfField.settings;
           
        DOFmodel.focusDistance = 1;
        cameraProfile.depthOfField.settings = DOFmodel;
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
        StartCoroutine(music.FadeGroupTo("LowpassFreq", 1500, Time.time + 0.5f));

        //Camera DOF
        cameraProfile.depthOfField.enabled = true;
        if(fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        fadeRoutine = FadeDOFTo(1, 1);
        StartCoroutine(fadeRoutine);
    }

    public void Unpause(){
        StartCoroutine(music.FadeGroupTo("LowpassFreq", 22000, Time.time + 1f));
        StartCoroutine(UnpauseGame());

        //DOF
        if(fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        fadeRoutine = FadeDOFTo(2, 1, true);
        StartCoroutine(fadeRoutine);
    }

    private IEnumerator UnpauseGame(){
        yield return new WaitForSeconds(1);

        isRunning = true;
        Player.instance.canControl = true;
        Player.instance.body.isKinematic = false;
        Player.instance.body.velocity = beforePauseVelocity;
    }

    private IEnumerator FadeDOFTo(float v, float duration, bool disableDOF = false){
        float startValue = cameraProfile.depthOfField.settings.focusDistance;
        float startTime = Time.time;
        float end = startTime + duration;

        float t = 0;
        float tt = 0;


        while(end > Time.time){
            t = (Time.time - startTime) / duration;
            tt = fadeCurve.Evaluate(t);

            DOFmodel.focusDistance = Mathf.Lerp(startValue, v, tt);
            cameraProfile.depthOfField.settings = DOFmodel;

            yield return null;
        }

        DOFmodel.focusDistance = v;
        cameraProfile.depthOfField.settings = DOFmodel;


        if(disableDOF){
            cameraProfile.depthOfField.enabled = false;
        }
        
    }

}
