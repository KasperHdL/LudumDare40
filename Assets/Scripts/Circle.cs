using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour {

    public List<Vector3> points;
    public List<bool> pointsFree;

    public float angle;
    public float speed;
    public float radius;
    private float curSpeed;
    private float curRadius;


	void Start () {
        points = new List<Vector3>();
        pointsFree = new List<bool>();

        EventHandler.Subscribe(GameEvent.FinishedLevel, FinishedLevel);
        EventHandler.Subscribe(GameEvent.StartFinish, StartFinish);

        for(int i = 0; i < Game.instance.numRequiredKeys; i++){
            points.Add(Vector3.zero);
            pointsFree.Add(true);
        }

	}

    public int GetIndex(){
        for(int i = 0; i < points.Count; i++){
            if(pointsFree[i]){
                pointsFree[i] = false;
                return i;
            }
        }
        return -1;
    }

    public void FinishedLevel(GameEventArgs eventArgs){
        if(Game.instance.numRequiredKeys >= points.Count){
            points.Add(Vector3.zero);
            pointsFree.Add(true);

            angle = (2 * Mathf.PI) / points.Count;
        }

        for(int i = 0; i < pointsFree.Count; i++)
            pointsFree[i] = true;

        curSpeed = speed;
        curRadius = radius;
    }

    public void StartFinish(GameEventArgs eventArgs){
        StartCoroutine(Finish(1f));
    }

    IEnumerator Finish(float duration){
        float startTime = Time.time;
        float endTime = startTime + duration;

        float endSpeed = speed * 3;
        float endRadius = 0;

        float t; 
        while(endTime > Time.time){
            t = (Time.time - startTime) / duration;

            curSpeed = Mathf.Lerp(speed, endSpeed, t);
            curRadius = Mathf.Lerp(radius, endRadius, t);

            yield return null;
        }

    }
	
	void Update () {
        for(int i = 0; i < points.Count; i++){
            float t = Time.time * curSpeed + angle * i;

            Vector3 circle = new Vector3(
                    Mathf.Cos(t),
                    0,
                    Mathf.Sin(t)
                    );

            circle *= curRadius;


            points[i] = transform.position + circle;

        }
	}
}
