using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {

    public Vector3 startPos;
    public Vector3 hoverPoint;
    public bool onPlayer = false;

    public float freeSpeedY;
    public float gotSpeedY;

    public float freeRadius;
    public float gotRadius;

    public float freeCirclingSpeed;
    public float gotCirclingSpeed;

	void Start () {
        EventHandler.Subscribe(GameEvent.Beat, Beat);
        EventHandler.Subscribe(GameEvent.PlayerGotKey, GotKey);
        startPos = transform.localPosition;
	}
	
	void Update () {
        float t = Time.time * (onPlayer ? gotCirclingSpeed : freeCirclingSpeed);

        Vector3 circle = new Vector3(
                Mathf.Cos(t),
                0,
                Mathf.Sin(t)
                );

        circle *= (onPlayer ? gotRadius : freeRadius);


        hoverPoint -= Vector3.up * Time.deltaTime * (onPlayer ? gotSpeedY : freeSpeedY);
        transform.localPosition = circle + hoverPoint;
		
	}

    void GotKey(GameEventArgs eventArgs){
        onPlayer = true;
        startPos = transform.localPosition;
        hoverPoint = Vector3.zero;
    }

    void Beat(GameEventArgs eventArgs){
        BeatArgs args = eventArgs as BeatArgs;

        hoverPoint = startPos;

        freeCirclingSpeed = -freeCirclingSpeed;
    }
}
