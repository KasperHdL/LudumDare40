using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Direction{
    None, North, East, South, West, Diagonal
}

public class Pillar : MonoBehaviour {
    public Direction wallDirection;
    public float randomWallColorAmount = 0.25f;

    public Vector3 endSize;

    public Color color;
    public float playerColorLerp = 0.75f;

    public Vector3 startSize;
    public float beatHeight = 2f;
    public float heightDecrease = .5f;

    public int changeOnBeat;

    public float beatWallHeight = 10f;
    public float wallHeightDecrease = 5f;

    public bool isTouchingPlayer = false;

    public MeshRenderer meshRenderer;

    void Awake(){
        meshRenderer = GetComponent<MeshRenderer>();
    }

	void Start () {
        EventHandler.Subscribe(GameEvent.Beat, Beat);
	}

    public void Init(int changeOnBeat){
        this.changeOnBeat = changeOnBeat;
    }

    public void SetNext(Vector3 pos, Vector3 size){
        StartCoroutine(Lerp(pos, size, 1f));
    }
	
	void Update () {
        if(wallDirection != Direction.None){
            transform.localScale = transform.localScale - Vector3.up * wallHeightDecrease * Time.deltaTime;
        }
	}

    public void Beat(GameEventArgs eventArgs){
        BeatArgs args = eventArgs as BeatArgs;
        transform.localScale = startSize + Vector3.up * (wallDirection != Direction.None? beatWallHeight : beatHeight);

        if(args.beat % 2 == 1)
            NewColor();
    }

    public void OnCollisionEnter(Collision collision){
        if(collision.gameObject.tag.Equals("Player")){
            isTouchingPlayer = true;
            meshRenderer.material.SetColor("_Color", Color.Lerp(color, Color.white, playerColorLerp));
            meshRenderer.material.SetColor("_EmissionColor", Color.Lerp(color, Color.white, playerColorLerp));
        }

    }

    public void OnCollisionExit(Collision collision){
        if(collision.gameObject.tag.Equals("Player")){
            isTouchingPlayer = false;

            meshRenderer.material.SetColor("_Color", color);
            meshRenderer.material.SetColor("_EmissionColor", color);
        }
    }

    public void NewColor(){
        if(isTouchingPlayer) return;

        if(wallDirection == Direction.None)
            color = Game.instance.colors[Random.Range(0, Game.instance.colors.Length)];
        else if(wallDirection == Direction.Diagonal){
            color = Game.instance.colors[Game.instance.colors.Length -1];
        }
        else{
            color = Game.instance.colors[(int)wallDirection-1];

            color.r += Random.Range(-randomWallColorAmount,randomWallColorAmount);
            color.g += Random.Range(-randomWallColorAmount,randomWallColorAmount);
            color.b += Random.Range(-randomWallColorAmount,randomWallColorAmount);
        }

        meshRenderer.material.SetColor("_Color", color);
        meshRenderer.material.SetColor("_EmissionColor", color);
    }

    IEnumerator Lerp(Vector3 pos, Vector3 size, float duration){
        float start = Time.time;
        float end = start + duration;

        this.startSize = size;
        size += Vector3.up * (wallDirection != Direction.None? beatWallHeight : beatHeight);

        Vector3 startPos = transform.position;
        Vector3 startSize = transform.localScale;

        Vector3 p = startPos;
        p.y = 0;
        Vector3 s = startSize;
        s.y = 1;

        float t = 0;
        while(end > Time.time){
            t = (Time.time - start) / duration;

            transform.position = Vector3.Lerp(startPos, p, t);
            transform.localScale = Vector3.Lerp(startSize, s, t);

            yield return null;
        }

        start = Time.time;
        end = start + duration;


        t = 0;
        while(end > Time.time){
            t = (Time.time - start) / duration;

            transform.position = Vector3.Lerp(p, pos, t);
            transform.localScale = Vector3.Lerp(s, size, t);

            yield return null;
        }

        transform.position = pos;
        transform.localScale = size;

        Init(0);

    }

}
