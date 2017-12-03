using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Direction{
    None, North, East, South, West, Diagonal
}

public class Pillar : MonoBehaviour {
    public Direction wallDirection;
    public float randomWallColorAmount = 0.25f;

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

    public void Init(int changeOnBeat, Direction direction){
        startSize = transform.localScale;
        transform.localScale = startSize + Vector3.up * (wallDirection != Direction.None? beatWallHeight : beatHeight);
        this.changeOnBeat = changeOnBeat;

        wallDirection = direction;

        NewColor();
    }
	
	void Update () {
        if(wallDirection != Direction.None){
            transform.localScale = transform.localScale - Vector3.up * wallHeightDecrease * Time.deltaTime;
        }
	}

    public void Beat(GameEventArgs eventArgs){
        BeatArgs args = eventArgs as BeatArgs;
        transform.localScale = startSize + Vector3.up * (wallDirection != Direction.None? beatWallHeight : beatHeight);

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

}
