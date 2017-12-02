using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatDetector : MonoBehaviour {
    public int beatCounter = 0;
    public float beatDelay = 2.67f;
    private float nextBeat = 0;
    private int beat = 0;

    public CameraManager camera;
    public AudioSource source;

    void Start()
    {
        nextBeat = Time.time + beatDelay;

        AudioSource source = GetComponent<AudioSource>();
        source.PlayDelayed(beatDelay);
    }

    void Update(){
        if(nextBeat < Time.time){
            nextBeat += beatDelay;

            beat++;
            if(beat == 4)beat = 0;
            beatCounter++;

            BeatArgs args = new BeatArgs();
            args.beat = beat;
            args.totalBeats = beatCounter;
            EventHandler.TriggerEvent(GameEvent.Beat, args);
        }

    }
}
