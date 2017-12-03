using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {
    public bool pickedup = false;
    private int circlePointIndex;

    [Header("References")]
    public Circle circle;
    private ReflectionProbe probe;
    private AudioSource source;

    [Header("Circle")]
    public AnimationCurve toCircleCurve;
    public float toCircleDuration;


    [Header("Hovering")]
    public Vector3 startPos;
    public Vector3 hoverPoint;

    public float hoverRadius;
    public float hoverSpeed;

    private bool animating = false;

	void Start () {
        EventHandler.Subscribe(GameEvent.Beat, Beat);

        probe = GetComponent<ReflectionProbe>();
        source = GetComponent<AudioSource>();
	}

    public void Init(Vector3 position){
        StopAllCoroutines();
        StartCoroutine(LerpToPoint(position, 1, 1));
        hoverPoint = startPos = position;
        pickedup = false;
    }
	
	void Update () {
        if(animating) return;
        if(pickedup){
            transform.position = circle.points[circlePointIndex];
        }else{

            float t = Time.time * hoverSpeed;

            Vector3 circle = new Vector3(
                    Mathf.Cos(t),
                    0,
                    Mathf.Sin(t)
                    );

            circle *= hoverRadius;


            hoverPoint -= Vector3.up * Time.deltaTime * hoverSpeed;
            transform.position = circle + hoverPoint;
        }
		
	}

    void LateUpdate(){
        probe.RenderProbe();
    }


    void Beat(GameEventArgs eventArgs){
        BeatArgs args = eventArgs as BeatArgs;

        hoverPoint = startPos;
    }

    public void OnTriggerEnter(Collider coll){
        if(!pickedup && coll.gameObject.tag.Equals("Player")){
            pickedup = true;

            Player.instance.numKeys++;

            circlePointIndex = circle.GetIndex();
            StartCoroutine(LerpToCircle(toCircleDuration));

            source.Play();

            EventHandler.TriggerEvent(GameEvent.PlayerGotKey);

        }
    }

    IEnumerator LerpToCircle(float duration){
        animating = true;
        float start = Time.time;
        float end = start = duration;

        Vector3 startPos = transform.position;

        float t = 0;
        float tt = 0;
        while(end > Time.time){
            t = (Time.time - start) / duration;
            tt = toCircleCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPos, circle.points[circlePointIndex], tt);

            yield return null;
        }

        animating = false;
    }

    IEnumerator LerpToPoint(Vector3 endPos, float duration, float delay){
        animating = true;
        yield return new WaitForSeconds(delay);
        float start = Time.time;
        float end = start + duration;

        Vector3 startPos = transform.position;

        float t = 0;
        float tt = 0;
        while(end > Time.time){
            t = (Time.time - start) / duration;
            tt = toCircleCurve.Evaluate(t);

            float a = Time.time * hoverSpeed;

            Vector3 circle = new Vector3(
                    Mathf.Cos(a),
                    0,
                    Mathf.Sin(a)
                    );

            circle *= hoverRadius;

            transform.position = Vector3.Lerp(startPos, endPos + circle, tt);

            yield return null;
        }

        animating = false;
    }
}
