using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicHandler : MonoBehaviour {
    public int beatCounter = 0;

    public float beatDelay = 2.67f;

    private float nextBeat = 0;
    private int beat = 0;

    public Song[] songs;
    private int songIndex = -1;

    public AudioMixer mixer;
    public AudioMixerGroup[] mixerGroup;
    public AudioSource[] sources;

    public bool goToNextBpm = false;

    public bool isVariating = false;

    public AnimationCurve fadeInCurve;
    public AnimationCurve fadeOutCurve;

    public string[] variationGroups;
    public float[] delayMultiplier;
    public int variatingGroupIndex;
    public int lastVariationIndex;



    void Start()
    {
        sources = GetComponents<AudioSource>();
        SetupAudioSources();
        
        ApplyNext();
    }


    void Update(){
        if(nextBeat < Time.time){
            nextBeat += beatDelay;

            beat++;
            if(beat == 8){
                beat = 0;
                if(goToNextBpm){
                    ApplyNext();
                    goToNextBpm = false;
                }
            }

            if(beat == 7){
                if(!isVariating){
                    isVariating = true;
                    variatingGroupIndex = lastVariationIndex;
                    while(variatingGroupIndex == lastVariationIndex){
                        variatingGroupIndex = Random.Range(0, variationGroups.Length);
                    }

                    StartCoroutine(FadeGroupTo(variationGroups[variatingGroupIndex], -80, nextBeat + beatDelay * delayMultiplier[variatingGroupIndex]));
                    lastVariationIndex = variatingGroupIndex;

                }else{
                    isVariating = false;
                    StartCoroutine(FadeGroupTo(variationGroups[variatingGroupIndex], 0, nextBeat + beatDelay * delayMultiplier[variatingGroupIndex]));
                }
            }

            beatCounter++;

            BeatArgs args = new BeatArgs();
            args.beat = beat;
            args.totalBeats = beatCounter;
            EventHandler.TriggerEvent(GameEvent.Beat, args);
        }

    }

    public void NextBpm(){
        goToNextBpm = true;
    }

    private void ApplyNext(){
        songIndex++;
        if(songIndex == songs.Length){
        //    songIndex = songs.Length - 1;
            songIndex = 0;
        }

        beatDelay = 1/(songs[songIndex].bpm / 60);
        nextBeat = Time.time + beatDelay;

        songs[songIndex].ApplyTo(sources);

        Play();
    }

    public void Play(){PlayDelayed(0);}
    public void PlayDelayed(float delay){
        for(int i = 0; i < sources.Length; i++){
            sources[i].PlayDelayed(delay);
        }
    }

    public void Stop(){
        for(int i = 0; i < sources.Length; i++){
            sources[i].Stop();
        }
    }

    void SetupAudioSources(){
        for(int i = 0; i < sources.Length; i++){
            sources[i].playOnAwake = false;
            sources[i].loop = true;
            sources[i].outputAudioMixerGroup = mixerGroup[i];
        }
    }

    IEnumerator FadeGroupTo(string group, float endValue, float endTime){
        float startValue;
        mixer.GetFloat(group, out startValue);

        bool onIn = startValue < endValue;


        float startTime = Time.time;
        float duration = endTime - startTime;

        float t = 0;
        float tt = 0;
        while(endTime > Time.time){
            t = (Time.time - startTime) / duration;
            if(onIn){
                tt = fadeInCurve.Evaluate(t);
            }else{
                tt = fadeOutCurve.Evaluate(t);
            }
            Debug.Log(tt);

            float v = Mathf.Lerp(startValue, endValue, tt);
            mixer.SetFloat(group, v);
            yield return null;
        }

        mixer.SetFloat(group, endValue);

    }

}
