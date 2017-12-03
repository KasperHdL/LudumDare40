using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public static Player instance;

    public bool canControl = true;
    public bool finishing = false;
    public Vector3 finishPos;
    public AnimationCurve finishCurve;

    public int numKeys = 0;

    public float movementForce;
    public float airForce;

    [Header("Jump")]
    public float jumpForce;
    public float onGroundCheckDistance = 0.1f;
    public float jumpDelay = 0.5f;
    private float nextJump = 0;

    public bool freezeUntilHitGround = false;

    public Vector3 keyPosition;

    public Rigidbody body;

    void Awake(){
        if(instance != null){
            Destroy(gameObject);
            return;
        }
        instance = this;
        body = GetComponent<Rigidbody>();
    }

    void Start(){
        EventHandler.Subscribe(GameEvent.FinishedLevel, FinishedLevel);
        EventHandler.Subscribe(GameEvent.StartFinish, StartFinish);
    }

	
	void Update () {
        if(!canControl || finishing) return;

        bool groundCheck = Physics.Raycast(transform.position, -Vector3.up, onGroundCheckDistance);
        if(freezeUntilHitGround){
            if(groundCheck) freezeUntilHitGround = false;
            else return;
        }

        bool jump = Input.GetKey(KeyCode.Space);
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");


        //Movement
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = Camera.main.transform.right;
        right.y = 0;
        right.Normalize();


        Vector3 direction = x * right + y * forward;

        Vector3 force = direction * Time.deltaTime * (groundCheck ? movementForce : airForce);
        body.AddForce(force);


        //Jump
        if(jump && nextJump < Time.time && groundCheck){

            force = Vector3.up * jumpForce;
            body.AddForce(force, ForceMode.Impulse);
            nextJump = Time.time + jumpDelay;
        }
	}


    public void StartFinish(GameEventArgs eventArgs){
        finishing = true;

        StartCoroutine(LerpToFinish(1f));

        freezeUntilHitGround = true;

        body.velocity = Vector3.zero;
        body.useGravity = false;

    }



    public void FinishedLevel(GameEventArgs eventArgs){
        body.isKinematic = false;
        body.useGravity = true;
    }

    IEnumerator LerpToFinish(float duration){
        float start = Time.time;
        float end = start + duration;

        Vector3 startPos = transform.position;

        float t = 0;
        float tt = 0;

        while(end > Time.time){
            t = (Time.time - start) / duration;
            tt = finishCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPos, finishPos, tt);

            yield return null;
        }

        transform.position = finishPos;
        finishing = false;
    }
}
