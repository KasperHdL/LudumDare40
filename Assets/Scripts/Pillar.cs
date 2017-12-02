using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour {
    public bool isWall; 

    public Vector3 startSize;
    public float beatHeight = 2f;
    public float heightDecrease = .5f;

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

    public void Init(bool isWall){
        startSize = transform.localScale;
        this.isWall = isWall;
        ColorPillar();
    }
	
	void Update () {
        transform.localScale = transform.localScale - Vector3.up * (isWall ? wallHeightDecrease : heightDecrease) * Time.deltaTime;
		
	}

    public void Beat(GameEventArgs eventArgs){
        transform.localScale = startSize + Vector3.up * (isWall ? beatWallHeight : beatHeight);

        BeatArgs args = eventArgs as BeatArgs;
        if(args.totalBeats % 6 == 0)
            ColorPillar();
    }

    public void OnCollisionEnter(Collision collision){
        if(collision.gameObject.tag.Equals("Player")){
            isTouchingPlayer = true;
            meshRenderer.material.SetColor("_Color", Color.white);
            meshRenderer.material.SetColor("_EmissionColor", Color.white);
        }

    }

    public void OnCollisionExit(Collision collision){
        if(collision.gameObject.tag.Equals("Player")){
            isTouchingPlayer = false;
            ColorPillar();
        }
    }

    public void ColorPillar(){
        if(isTouchingPlayer) return;

        Color c = Game.instance.colors[Random.Range(0, Game.instance.colors.Length)];

        meshRenderer.material.SetColor("_Color", c);
        meshRenderer.material.SetColor("_EmissionColor", c);
    }

}
