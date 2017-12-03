using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Song : ScriptableObject {
    [Range(145,180)]public float bpm;

    public AudioClip combined;

    public AudioClip bass;
    public AudioClip lead;

    [Header("Drums")]
    //Drums
    public AudioClip drums;
    public AudioClip kicks;
    public AudioClip snares;
    public AudioClip toms;

    public void ApplyTo(AudioSource[] sources){
        if(sources.Length == 1){
            sources[0].clip = combined;
            return;
        }
        if(sources.Length == 3){
            sources[0].clip = drums;
            sources[1].clip = bass;
            sources[2].clip = lead;
            return;
        }
        if(sources.Length >= 5){
            sources[0].clip = kicks;
            sources[1].clip = snares;
            sources[2].clip = toms;

            sources[3].clip = bass;
            sources[4].clip = lead;
            return;
        }
    }
}
