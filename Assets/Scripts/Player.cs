using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public Player instance;
    public World world;

    public bool hasKey = false;

    public float movementForce;
    public float airForce;

    [Header("Jump")]
    public float jumpForce;
    public float onGroundCheckDistance = 0.1f;
    public float jumpDelay = 0.5f;
    private float nextJump = 0;

    public bool freezeUntilHitGround = false;


    public Rigidbody body;

    void Awake(){
        if(instance != null){
            Destroy(gameObject);
            return;
        }
        instance = this;
        body = GetComponent<Rigidbody>();
    }

	
	void Update () {
        bool groundCheck = Physics.Raycast(transform.position, -Vector3.up, onGroundCheckDistance);
        if(freezeUntilHitGround){
            if(groundCheck) freezeUntilHitGround = false;
            else return;
        }

        bool jump = Input.GetKeyDown(KeyCode.Space);
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

            force = Vector3.up * jumpForce * Time.deltaTime;
            body.AddForce(force, ForceMode.Impulse);
            nextJump = Time.time + jumpDelay;
        }
	}

    public void OnTriggerEnter(Collider coll){
        if(coll.gameObject.tag.Equals("Key")){
            hasKey = true;
            coll.gameObject.transform.SetParent(transform);
            coll.gameObject.transform.localPosition = Vector3.up;
        }
    }

    public void OnCollisionEnter(Collision coll){
        if(hasKey && coll.collider.gameObject.tag.Equals("Lock")){
            world.Generate();
            hasKey = false;
        }
    }
}
