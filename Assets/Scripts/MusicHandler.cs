using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicHandler : MonoBehaviour {
    public int beatCounter = 0;

    public double beatDelay = 2.67f;

    private double nextBeat = 0;
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


    public UnityEngine.UI.Slider slider;

    void Start()
    {
        sources = GetComponents<AudioSource>();
        SetupAudioSources();
        
        ApplyNext();
        VolumeSlider(slider.value);
    }


    void Update(){
        if(nextBeat < AudioSettings.dspTime){
            nextBeat += beatDelay;

            beat++;
            if(beat == 8){
                beat = 0;
                if(goToNextBpm){
                    ApplyNext();
                    goToNextBpm = false;
                    sources[sources.Length - 1].Play();
                }
            }

            if(beat == 7){
                float endVolume = 0;
                if(!isVariating){
                    isVariating = true;
                    variatingGroupIndex = lastVariationIndex;
                    while(variatingGroupIndex == lastVariationIndex){
                        variatingGroupIndex = Random.Range(0, variationGroups.Length);
                    }
                    endVolume = -80;

                    lastVariationIndex = variatingGroupIndex;

                }else{
                    isVariating = false;
                    endVolume = 0;
                }

                float offset = (float)((nextBeat - beatDelay) - AudioSettings.dspTime);
                StartCoroutine(FadeGroupTo(variationGroups[variatingGroupIndex], endVolume, (float)(Time.time - offset + beatDelay * delayMultiplier[variatingGroupIndex])));
            }

            if((beat+1) % CameraManager.instance.changeCameraOnBeat == 0){
                sources[sources.Length - 1].PlayScheduled(nextBeat);
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
        nextBeat = AudioSettings.dspTime + beatDelay;

        songs[songIndex].ApplyTo(sources);

        Play();
    }

    public void Play(){PlayDelayed(0);}
    public void PlayDelayed(float delay){
        for(int i = 0; i < sources.Length-1; i++){
            sources[i].PlayDelayed(delay);
        }
    }

    public void Stop(){
        for(int i = 0; i < sources.Length-1; i++){
            sources[i].Stop();
        }
    }

    void SetupAudioSources(){
        for(int i = 0; i < sources.Length; i++){
            sources[i].playOnAwake = false;

            if(i != sources.Length - 1)
                sources[i].loop = true;

            sources[i].outputAudioMixerGroup = mixerGroup[i];
        }
    }

    public IEnumerator FadeGroupTo(string group, float endValue, float endTime){
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

            float v = Mathf.Lerp(startValue, endValue, tt);
            mixer.SetFloat(group, v);
            yield return null;
        }

        mixer.SetFloat(group, endValue);

    }

    public void VolumeSlider(float volume){
        mixer.SetFloat("Volume", (fadeInCurve.Evaluate(volume) - 1) * 80f);
    }

}
